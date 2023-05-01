using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class SelectionTool : ToolBase, IEditorTool
{
    private LevModification _currLevModification;
    private Vector _lockCenter; //for lock lines -mode
    private Vector _lockNext; //for lock lines -mode
    private Vector _lockPrev; //for lock lines -mode
    private bool _lockingLines;
    private Vector _moveStartPosition;
    private Polygon? _selectionPoly;

    private bool FreeSelecting => _selectionPoly is { };

    private Vector _selectionStartPoint;
    private double _mouseTrip;
    private Vector _lastMousePosition;

    internal SelectionTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool Moving { get; set; }

    private bool RectSelecting { get; set; }

    public void Activate()
    {
        UpdateHelp();
    }

    public void ExtraRendering()
    {
        if (RectSelecting)
            Renderer.DrawRectangle(_selectionStartPoint, CurrentPos, Color.Blue);
        else if (_selectionPoly is { })
            Renderer.DrawPolygon(_selectionPoly, Color.Blue);
    }

    public List<Polygon> GetExtraPolygons()
    {
        return new();
    }

    public void InActivate()
    {
        Moving = false;
        RectSelecting = false;
    }

    public void KeyDown(KeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case Keys.Space:
                LevEditor.TransformMenuItemClick();
                break;
            case Keys.D1:
                UpdateAnimNumbers(1);
                break;
            case Keys.D2:
                UpdateAnimNumbers(2);
                break;
            case Keys.D3:
                UpdateAnimNumbers(3);
                break;
            case Keys.D4:
                UpdateAnimNumbers(4);
                break;
            case Keys.D5:
                UpdateAnimNumbers(5);
                break;
            case Keys.D6:
                UpdateAnimNumbers(6);
                break;
            case Keys.D7:
                UpdateAnimNumbers(7);
                break;
            case Keys.D8:
                UpdateAnimNumbers(8);
                break;
            case Keys.D9:
                UpdateAnimNumbers(9);
                break;
        }
    }

    private void UpdateAnimNumbers(int animNum)
    {
        var selected = Lev.Objects.Where(obj =>
            obj.Type == ObjectType.Apple && obj.Position.Mark == VectorMark.Selected).ToList();
        bool updated = selected.Any(obj => obj.AnimationNumber != animNum);
        selected.ForEach(obj => obj.AnimationNumber = animNum);
        if (updated)
        {
            LevEditor.SetModified(LevModification.Objects);
        }

        ShowObjectInfo(GetNearestObjectIndex(CurrentPos));
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        Vector p = CurrentPos;
        _currLevModification = LevModification.Nothing;
        var nearestVertexIndex = GetNearestVertexInfo(p);
        int nearestObjectIndex = GetNearestObjectIndex(p);
        int nearestPictureIndex = GetNearestPictureIndex(p);
        var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                var somethingGrabbed = true;
                if (nearestVertexIndex is { } nvi && Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        MarkAllAs(VectorMark.None);
                    }

                    nvi.Polygon.MarkVectorsAs(VectorMark.Selected);
                    var inearest = nvi.Polygon.ToIPolygon();
                    foreach (var polygon in Lev.Polygons.Where(polygon => polygon.ToIPolygon().Within(inearest)))
                    {
                        polygon.MarkVectorsAs(VectorMark.Selected);
                    }

                    foreach (var obj in Lev.Objects)
                    {
                        if (nvi.Polygon.AreaHasPoint(obj.Position))
                        {
                            obj.Mark = VectorMark.Selected;
                        }
                    }

                    foreach (var pic in Lev.GraphicElements)
                    {
                        if (nvi.Polygon.AreaHasPoint(pic.Position))
                        {
                            pic.Position = pic.Position with { Mark = VectorMark.Selected };
                        }
                    }

                    EndSelectionHandling();
                }
                else if (nearestVertexIndex is NearestVertexInfo.VertexInfo vi)
                {
                    var v = vi.Polygon[vi.Index];
                    vi.Polygon.Vertices[vi.Index] = HandleMark(v);
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        _lockCenter = vi.Polygon[vi.Index];
                        _lockPrev = vi.Polygon[vi.Index - 1];
                        _lockNext = vi.Polygon[vi.Index + 1];
                        _lockingLines = true;
                        _moveStartPosition = _lockCenter;
                    }
                }
                else if (nearestVertexIndex is NearestVertexInfo.EdgeInfo ei)
                {
                    int nearestSegmentIndex = ei.Polygon.GetNearestSegmentIndex(p);
                    AdjustForGrid(ref p);
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        MarkAllAs(VectorMark.None);
                        p.Mark = VectorMark.Selected;
                        ei.Polygon.Insert(nearestSegmentIndex + 1, p);
                        LevEditor.SetModified(ei.Polygon.IsGrass ? LevModification.Decorations : LevModification.Ground);
                    }
                    else
                    {
                        if (
                            !(ei.Polygon[nearestSegmentIndex].Mark == VectorMark.Selected &&
                              ei.Polygon[nearestSegmentIndex + 1].Mark == VectorMark.Selected))
                        {
                            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                MarkAllAs(VectorMark.None);
                                ei.Polygon.MarkVectorsAs(VectorMark.Selected);
                            }
                        }

                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            ei.Polygon.MarkVectorsAs(
                                ei.Polygon.Vertices.TrueForAll(v => v.Mark == VectorMark.Selected)
                                    ? VectorMark.None
                                    : VectorMark.Selected);
                        }
                    }

                    EndSelectionHandling();
                }
                else if (nearestObjectIndex >= 0)
                    HandleMark(Lev.Objects[nearestObjectIndex]);
                else if (nearestPictureIndex >= 0)
                    HandleMark(Lev.GraphicElements[nearestPictureIndex]);
                else if (nearestBodyPart is { })
                {
                    LevEditor.PlayController.CurrentBodyPart = nearestBodyPart;
                    LevEditor.PlayController.PlayerSelection = VectorMark.Selected;
                    EndSelectionHandling();
                }
                else
                {
                    somethingGrabbed = false;
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        MarkAllAs(VectorMark.None);
                        LevEditor.PreserveSelection();
                    }

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        _selectionPoly = new Polygon();
                        _selectionPoly.Add(CurrentPos);
                        _mouseTrip = 0;
                        _lastMousePosition = CurrentPos;
                    }
                    else
                    {
                        _selectionStartPoint = p;
                        RectSelecting = true;
                    }
                }

                if (somethingGrabbed && LevEditor.PlayController.PlayerSelection == VectorMark.Selected)
                {
                    LevEditor.PlayController.CurrentBodyPart = LevEditor.PlayController.Driver!.GetBody();
                }

                LevEditor.UpdateSelectionInfo();
                break;
            case MouseButtons.Right:
                break;

            case MouseButtons.Middle:
                break;
        }
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (Moving)
        {
            AdjustForGrid(ref p);
            if (_lockingLines)
            {
                p = GeometryUtils.OrthogonalProjection(_lockCenter,
                    GeometryUtils.DistanceFromLine(_lockCenter, _lockNext, p) <
                    GeometryUtils.DistanceFromLine(_lockCenter, _lockPrev, p)
                        ? _lockNext
                        : _lockPrev, p);
            }
            if (Equals(p, _moveStartPosition))
            {
                return;
            }
            Vector delta = p - _moveStartPosition;
            Vector.MarkDefault = VectorMark.Selected;
            var anythingMoved = LevModification.Nothing;
            foreach (Polygon x in Lev.Polygons)
            {
                bool polygonMoved = false;
                for (int i = 0; i < x.Vertices.Count; i++)
                {
                    if (x.Vertices[i].Mark != VectorMark.Selected) continue;
                    x.Vertices[i] += delta;
                    polygonMoved = true;
                }

                if (polygonMoved)
                {
                    x.UpdateDecompositionOrGrassSlopes(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                    anythingMoved |= x.IsGrass ? LevModification.Decorations : LevModification.Ground;
                }
            }

            foreach (LevObject x in Lev.Objects.Where(x => x.Position.Mark == VectorMark.Selected))
            {
                x.Position += delta;
                anythingMoved |= LevModification.Objects;
            }

            foreach (GraphicElement z in Lev.GraphicElements.Where(z => z.Position.Mark == VectorMark.Selected))
            {
                z.Position += delta;
                anythingMoved |= LevModification.Decorations;
            }

            _currLevModification = anythingMoved;
            if (anythingMoved.HasFlag(LevModification.Ground) || anythingMoved.HasFlag(LevModification.Objects))
            {
                LevEditor.PlayController.UpdateEngine(Lev);
            }

            if (LevEditor.PlayController.CurrentBodyPart is { })
            {
                LevEditor.PlayController.CurrentBodyPart.Position += delta;
                LevEditor.PlayController.ShouldRestartAfterResuming = false;
            }

            Vector.MarkDefault = VectorMark.None;
            _moveStartPosition = p;
        }
        else if (_selectionPoly is { })
        {
            _mouseTrip += (p - _lastMousePosition).Length;
            _lastMousePosition = p;
            double step = 0.02 * ZoomCtrl.ZoomLevel;
            if (_mouseTrip > step)
            {
                while (!(_mouseTrip < step))
                    _mouseTrip -= step;
                _selectionPoly.Add(p);
            }
        }
        else if (!Busy)
        {
            ResetHighlight();
            var nearestVertex = GetNearestVertexInfo(p);
            int nearestObject = GetNearestObjectIndex(p);
            int nearestTextureIndex = GetNearestPictureIndex(p);
            var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
            if (nearestVertex is NearestVertexInfo.EdgeInfo ei)
            {
                ChangeCursorToHand();
                ei.Polygon.Mark = PolygonMark.Highlight;
                LevEditor.HighlightLabel.Text = ei.Polygon.IsGrass ? "Grass" : "Ground";
                LevEditor.HighlightLabel.Text += " polygon, " + ei.Polygon.Vertices.Count + " vertices";
            }
            else if (nearestVertex is NearestVertexInfo.VertexInfo vi)
            {
                ChangeCursorToHand();
                Vector b = vi.Polygon.Vertices[vi.Index];
                if (b.Mark == VectorMark.None)
                {
                    b.Mark = VectorMark.Highlight;
                    vi.Polygon.Vertices[vi.Index] = b;
                }
                LevEditor.HighlightLabel.Text = vi.Polygon.IsGrass ? "Grass" : "Ground";
                LevEditor.HighlightLabel.Text += " polygon, vertex " + (vi.Index + 1) + " of " +
                                                 vi.Polygon.Vertices.Count + " vertices";
            }
            else if (nearestObject >= 0)
            {
                ChangeCursorToHand();
                if (Lev.Objects[nearestObject].Mark == VectorMark.None)
                    Lev.Objects[nearestObject].Mark = VectorMark.Highlight;
                ShowObjectInfo(nearestObject);
            }
            else if (nearestTextureIndex >= 0)
            {
                ChangeCursorToHand();
                if (Lev.GraphicElements[nearestTextureIndex].Mark == VectorMark.None)
                    Lev.GraphicElements[nearestTextureIndex].Mark = VectorMark.Highlight;
                ShowTextureInfo(nearestTextureIndex);
            }
            else if (nearestBodyPart is { })
            {
                ChangeCursorToHand();
                LevEditor.HighlightLabel.Text = "Player";
                if (LevEditor.PlayController.PlayerSelection == VectorMark.None)
                {
                    LevEditor.PlayController.PlayerSelection = VectorMark.Highlight;
                }
            }
            else
            {
                ChangeToDefaultCursorIfHand();
                LevEditor.HighlightLabel.Text = "";
            }
        }
    }

    public void MouseOutOfEditor()
    {
        ResetHighlight();
    }

    public void MouseUp()
    {
        if (RectSelecting || FreeSelecting)
        {
            double selectionxMin = 0;
            double selectionxMax = 0;
            double selectionyMax = 0;
            double selectionyMin = 0;
            if (RectSelecting)
            {
                selectionxMin = Math.Min(CurrentPos.X, _selectionStartPoint.X);
                selectionxMax = Math.Max(CurrentPos.X, _selectionStartPoint.X);
                selectionyMax = Math.Max(CurrentPos.Y, _selectionStartPoint.Y);
                selectionyMin = Math.Min(CurrentPos.Y, _selectionStartPoint.Y);
            }

            var grassFilter = LevEditor.EffectiveGrassFilter;
            var groundFilter = LevEditor.EffectiveGroundFilter;
            var appleFilter = LevEditor.EffectiveAppleFilter;
            var killerFilter = LevEditor.EffectiveKillerFilter;
            var flowerFilter = LevEditor.EffectiveFlowerFilter;
            var pictureFilter = LevEditor.EffectivePictureFilter;
            var textureFilter = LevEditor.EffectiveTextureFilter;
            foreach (Polygon x in Lev.Polygons)
            {
                if ((x.IsGrass && grassFilter) || (!x.IsGrass && groundFilter))
                {
                    for (var index = 0; index < x.Vertices.Count; index++)
                    {
                        Vector t = x.Vertices[index];
                        if (_selectionPoly is null)
                            MarkSelectedInArea(ref t, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                        else
                        {
                            MarkSelectedInArea(ref t, _selectionPoly);
                        }

                        x.Vertices[index] = t;
                    }
                }
            }

            foreach (var t in Lev.Objects)
            {
                var type = t.Type;
                var x = t;
                if (type == ObjectType.Start || (type == ObjectType.Apple && appleFilter) ||
                    (type == ObjectType.Killer && killerFilter) ||
                    (type == ObjectType.Flower && flowerFilter))
                    if (_selectionPoly is null)

                        MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin,
                            selectionyMax);
                    else
                    {
                        MarkSelectedInArea(ref x, _selectionPoly);
                    }
            }

            foreach (var z in Lev.GraphicElements)
            {
                var x = z;
                if ((z is GraphicElement.Picture && pictureFilter) || (z is GraphicElement.Texture && textureFilter))
                    if (_selectionPoly is null)
                        MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                    else
                    {
                        MarkSelectedInArea(ref x, _selectionPoly);
                    }
            }

            if (LevEditor.PlayController.PlayingOrPaused)
            {
                var x = LevEditor.PlayController.Driver!.Body.Location;
                x.Mark = LevEditor.PlayController.PlayerSelection;
                if (_selectionPoly is null)
                {
                    MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                }
                else
                {
                    MarkSelectedInArea(ref x, _selectionPoly);
                }

                LevEditor.PlayController.PlayerSelection = x.Mark;
            }

            LevEditor.UpdateSelectionInfo();
            LevEditor.PreserveSelection();
            RectSelecting = false;
            _selectionPoly = null;
        }
        else if (Moving)
        {
            Moving = false;
            _lockingLines = false;
            LevEditor.PlayController.CurrentBodyPart = null;
            LevEditor.SetModified(_currLevModification);
        }
    }

    private static void MarkSelectedInArea<T>(ref T z, Polygon selectionPoly) where T : IPositionable
    {
        if (selectionPoly.AreaHasPoint(z.Position))
        {
            z.Mark = z.Mark switch
            {
                VectorMark.None => VectorMark.Selected,
                VectorMark.Selected => VectorMark.None,
                _ => z.Mark
            };
        }
        else if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            z.Mark = VectorMark.None;
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = "LMouse: select level elements; LShift: bend edge; LShift + click: lock edge angle; LAlt + click: select all inside.";
    }

    private static void MarkSelectedInArea<T>(ref T z, double selectionxMin, double selectionxMax,
        double selectionyMin, double selectionyMax) where
        T : IPositionable
    {
        if (z.X < selectionxMax && z.X > selectionxMin && z.Y < selectionyMax && z.Y > selectionyMin)
        {
            z.Mark = z.Mark switch
            {
                VectorMark.None => VectorMark.Selected,
                VectorMark.Selected => VectorMark.None,
                _ => z.Mark
            };
        }
        else if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            z.Mark = VectorMark.None;
    }

    private T HandleMark<T>(T v) where T : IPositionable
    {
        if (!Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            if (v.Mark != VectorMark.Selected)
                MarkAllAs(VectorMark.None);
            v.Mark = VectorMark.Selected;
        }
        else
        {
            v.Mark = v.Mark != VectorMark.Selected
                ? VectorMark.Selected
                : VectorMark.None;
        }

        EndSelectionHandling();
        return v;
    }

    private void EndSelectionHandling()
    {
        Moving = true;
        LevEditor.PreserveSelection();
        AdjustForGrid(ref CurrentPos);
        _moveStartPosition = CurrentPos;
    }

    private void ShowObjectInfo(int currentObjectIndex)
    {
        if (currentObjectIndex < 0)
        {
            return;
        }

        LevObject currObj = Lev.Objects[currentObjectIndex];
        switch (currObj.Type)
        {
            case ObjectType.Apple:
                LevEditor.HighlightLabel.Text = "Apple: ";
                switch (currObj.AppleType)
                {
                    case AppleType.Normal:
                        LevEditor.HighlightLabel.Text += "Normal";
                        break;
                    case AppleType.GravityUp:
                        LevEditor.HighlightLabel.Text += "Gravity up";
                        break;
                    case AppleType.GravityDown:
                        LevEditor.HighlightLabel.Text += "Gravity down";
                        break;
                    case AppleType.GravityLeft:
                        LevEditor.HighlightLabel.Text += "Gravity left";
                        break;
                    case AppleType.GravityRight:
                        LevEditor.HighlightLabel.Text += "Gravity right";
                        break;
                }

                LevEditor.HighlightLabel.Text += ", animation number: " + currObj.AnimationNumber;
                break;
            case ObjectType.Killer:
                LevEditor.HighlightLabel.Text = "Killer";
                break;
            case ObjectType.Flower:
                LevEditor.HighlightLabel.Text = "Flower";
                break;
            case ObjectType.Start:
                LevEditor.HighlightLabel.Text = "Start";
                break;
        }
    }

    private void ShowTextureInfo(int index)
    {
        var graphicElement = Lev.GraphicElements[index];
        LevEditor.HighlightLabel.Text = graphicElement switch
        {
            GraphicElement.Picture p => "Picture: " + p.PictureInfo.Name + ", distance: " + graphicElement.Distance +
                                        ", clipping: " + graphicElement.Clipping,
            GraphicElement.Texture t => "Texture: " + t.TextureInfo.Name + ", mask: " + t.MaskInfo.Name +
                                        ", distance: " + graphicElement.Distance + ", clipping: " +
                                        graphicElement.Clipping,
            _ => LevEditor.HighlightLabel.Text
        };
    }

    public override bool Busy => RectSelecting || FreeSelecting || Moving;
}