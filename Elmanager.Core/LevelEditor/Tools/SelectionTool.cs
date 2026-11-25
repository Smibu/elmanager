using System;
using System.Drawing;
using System.Linq;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class SelectionTool : ToolBase, IEditorTool
{
    private LevModification _currLevModification;
    private SelectionState _state = new Idle();

    private union SelectionState(Idle, Moving, RectSelecting, FreeSelecting);

    private record Idle;

    private record Moving(Vector Start, LockLines? Lock);

    private record LockLines(Vector Center, Vector Prev, Vector Next);

    private record RectSelecting(Vector Start);

    private record FreeSelecting(Polygon Poly, double MouseTrip, Vector LastPosition);

    public SelectionTool(ILevelEditor editor)
        : base(editor)
    {
    }

    public void Activate()
    {
    }

    public void ExtraRendering()
    {
        if (_state is RectSelecting rect)
            Renderer.DrawRectangle(rect.Start, CurrentPos, Color.Blue);
        else if (_state is FreeSelecting free)
            Renderer.DrawPolygon(free.Poly, Color.Blue);
    }

    public LevVisualChange InActivate()
    {
        _state = new Idle();
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case EditorKey.Space:
                LevEditor.TransformMenuItemClick();
                break;
            case EditorKey.D1:
                UpdateAnimNumbers(1);
                break;
            case EditorKey.D2:
                UpdateAnimNumbers(2);
                break;
            case EditorKey.D3:
                UpdateAnimNumbers(3);
                break;
            case EditorKey.D4:
                UpdateAnimNumbers(4);
                break;
            case EditorKey.D5:
                UpdateAnimNumbers(5);
                break;
            case EditorKey.D6:
                UpdateAnimNumbers(6);
                break;
            case EditorKey.D7:
                UpdateAnimNumbers(7);
                break;
            case EditorKey.D8:
                UpdateAnimNumbers(8);
                break;
            case EditorKey.D9:
                UpdateAnimNumbers(9);
                break;
        }

        return LevVisualChange.Apples;
    }

    private void UpdateAnimNumbers(int animNum)
    {
        var selected = Lev.Objects.Where(obj =>
            obj.Type == ObjectType.Apple && obj.Position.Mark == VectorMark.Selected).ToList();
        bool updated = selected.Any(obj => obj.AnimationNumber != animNum);
        selected.ForEach(obj => obj.AnimationNumber = animNum);
        if (updated)
        {
            LevEditor.SetModified(LevModification.Apples);
        }

        ShowObjectInfo(GetNearestObjectIndex(CurrentPos));
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        Vector p = CurrentPos;
        _currLevModification = LevModification.Nothing;
        var nearestVertexIndex = GetNearestVertexInfo(p);
        int nearestObjectIndex = GetNearestObjectIndex(p);
        int nearestPictureIndex = GetNearestPictureIndex(p);
        var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:
                var somethingGrabbed = true;
                if (nearestVertexIndex is { } nvi && Keyboard.IsKeyDown(ModifierKey.LeftAlt))
                {
                    if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
                    if (Keyboard.IsKeyDown(ModifierKey.LeftShift))
                    {
                        var center = vi.Polygon[vi.Index];
                        _state = new Moving(center, new LockLines(center, vi.Polygon[vi.Index - 1], vi.Polygon[vi.Index + 1]));
                    }
                }
                else if (nearestVertexIndex is NearestVertexInfo.EdgeInfo ei)
                {
                    int nearestSegmentIndex = ei.Polygon.GetNearestSegmentIndex(p);
                    AdjustForGrid(ref p);
                    if (Keyboard.IsKeyDown(ModifierKey.LeftShift))
                    {
                        MarkAllAs(VectorMark.None);
                        p.Mark = VectorMark.Selected;
                        ei.Polygon.Insert(nearestSegmentIndex + 1, p);
                        LevEditor.SetModified(ei.Polygon.IsGrass ? LevModification.Grass : LevModification.Ground);
                    }
                    else
                    {
                        if (
                            !(ei.Polygon[nearestSegmentIndex].Mark == VectorMark.Selected &&
                              ei.Polygon[nearestSegmentIndex + 1].Mark == VectorMark.Selected))
                        {
                            if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
                            {
                                MarkAllAs(VectorMark.None);
                                ei.Polygon.MarkVectorsAs(VectorMark.Selected);
                            }
                        }

                        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
                    if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
                    {
                        MarkAllAs(VectorMark.None);
                        LevEditor.PreserveSelection();
                    }

                    if (Keyboard.IsKeyDown(ModifierKey.LeftShift))
                    {
                        var poly = new Polygon();
                        poly.Add(CurrentPos);
                        _state = new FreeSelecting(poly, 0, CurrentPos);
                    }
                    else
                    {
                        _state = new RectSelecting(p);
                    }
                }

                if (somethingGrabbed && LevEditor.PlayController.PlayerSelection == VectorMark.Selected)
                {
                    LevEditor.PlayController.CurrentBodyPart = LevEditor.PlayController.Driver!.GetBody();
                }

                LevEditor.UpdateSelectionInfo();
                break;
            case EditorMouseButton.Right:
                break;

            case EditorMouseButton.Middle:
                break;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_state is Moving moving)
        {
            AdjustForGrid(ref p);
            if (moving.Lock is { } lockLines)
            {
                p = GeometryUtils.OrthogonalProjection(lockLines.Center,
                    GeometryUtils.DistanceFromLine(lockLines.Center, lockLines.Next, p) <
                    GeometryUtils.DistanceFromLine(lockLines.Center, lockLines.Prev, p)
                        ? lockLines.Next
                        : lockLines.Prev, p);
            }
            if (Equals(p, moving.Start))
            {
                return LevVisualChange.Nothing;
            }
            Vector delta = p - moving.Start;
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
                    x.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                    anythingMoved |= x.IsGrass ? LevModification.Grass : LevModification.Ground;
                }
            }

            foreach (LevObject x in Lev.Objects.Where(x => x.Position.Mark == VectorMark.Selected))
            {
                x.Position += delta;
                anythingMoved |= x.Type switch
                {
                    ObjectType.Apple => LevModification.Apples,
                    ObjectType.Killer => LevModification.Killers,
                    ObjectType.Flower => LevModification.Flowers,
                    ObjectType.Start => LevModification.Start,
                    _ => LevModification.Nothing
                };
            }

            foreach (GraphicElement z in Lev.GraphicElements.Where(z => z.Position.Mark == VectorMark.Selected))
            {
                z.Position += delta;
                anythingMoved |= z.IsPicture ? LevModification.Pictures : LevModification.Textures;
            }

            _currLevModification = anythingMoved;
            if (anythingMoved.HasFlag(LevModification.Ground) || anythingMoved.HasFlag(LevModification.Apples) || anythingMoved.HasFlag(LevModification.Killers) || anythingMoved.HasFlag(LevModification.Flowers))
            {
                LevEditor.PlayController.UpdateEngine(Lev);
            }

            if (LevEditor.PlayController.CurrentBodyPart is { })
            {
                LevEditor.PlayController.CurrentBodyPart.Position += delta;
                LevEditor.PlayController.ShouldRestartAfterResuming = false;
            }

            Vector.MarkDefault = VectorMark.None;
            _state = moving with { Start = p };
            return (LevVisualChange)anythingMoved;
        }
        else if (_state is FreeSelecting free)
        {
            var mouseTrip = free.MouseTrip + (p - free.LastPosition).Length;
            double step = 0.02 * ZoomCtrl.ZoomLevel;
            if (mouseTrip > step)
            {
                while (!(mouseTrip < step))
                    mouseTrip -= step;
                free.Poly.Add(p);
            }

            _state = free with { MouseTrip = mouseTrip, LastPosition = p };
        }
        else if (!Busy)
        {
            HighlightTarget? highlight = null;
            var nearestVertex = GetNearestVertexInfo(p);
            int nearestObject = GetNearestObjectIndex(p);
            int nearestTextureIndex = GetNearestPictureIndex(p);
            var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
            if (nearestVertex is NearestVertexInfo.EdgeInfo ei)
            {
                ChangeCursorToHand();
                highlight = new HighlightTarget.PolygonTarget(ei.Polygon);
                LevEditor.HighlightText = ei.Polygon.IsGrass ? "Grass" : "Ground";
                LevEditor.HighlightText += " polygon, " + ei.Polygon.Vertices.Count + " vertices";
            }
            else if (nearestVertex is NearestVertexInfo.VertexInfo vi)
            {
                ChangeCursorToHand();
                if (vi.Polygon.Vertices[vi.Index].Mark == VectorMark.None)
                {
                    highlight = new HighlightTarget.VertexTarget(vi.Polygon, vi.Index);
                }
                LevEditor.HighlightText = vi.Polygon.IsGrass ? "Grass" : "Ground";
                LevEditor.HighlightText += " polygon, vertex " + (vi.Index + 1) + " of " +
                                                 vi.Polygon.Vertices.Count + " vertices";
            }
            else if (nearestObject >= 0)
            {
                ChangeCursorToHand();
                if (Lev.Objects[nearestObject].Mark == VectorMark.None)
                    highlight = new HighlightTarget.ObjectTarget(Lev.Objects[nearestObject]);
                ShowObjectInfo(nearestObject);
            }
            else if (nearestTextureIndex >= 0)
            {
                ChangeCursorToHand();
                if (Lev.GraphicElements[nearestTextureIndex].Mark == VectorMark.None)
                    highlight = new HighlightTarget.GraphicElementTarget(Lev.GraphicElements[nearestTextureIndex]);
                ShowTextureInfo(nearestTextureIndex);
            }
            else if (nearestBodyPart is { })
            {
                ChangeCursorToHand();
                LevEditor.HighlightText = "Player";
                if (LevEditor.PlayController.PlayerSelection == VectorMark.None)
                {
                    highlight = new HighlightTarget.PlayerTarget();
                }
            }
            else
            {
                ChangeToDefaultCursorIfHand();
                LevEditor.HighlightText = "";
            }

            LevEditor.CurrentHighlight = highlight;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        ResetHighlight();
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
        if (_state is RectSelecting or FreeSelecting)
        {
            double selectionxMin = 0;
            double selectionxMax = 0;
            double selectionyMax = 0;
            double selectionyMin = 0;
            if (_state is RectSelecting rect)
            {
                selectionxMin = Math.Min(CurrentPos.X, rect.Start.X);
                selectionxMax = Math.Max(CurrentPos.X, rect.Start.X);
                selectionyMax = Math.Max(CurrentPos.Y, rect.Start.Y);
                selectionyMin = Math.Min(CurrentPos.Y, rect.Start.Y);
            }

            var selectionPoly = _state is FreeSelecting free ? free.Poly : null;

            var grassFilter = LevEditor.SelectionFilter.EffectiveGrassFilter;
            var groundFilter = LevEditor.SelectionFilter.EffectiveGroundFilter;
            var appleFilter = LevEditor.SelectionFilter.EffectiveAppleFilter;
            var killerFilter = LevEditor.SelectionFilter.EffectiveKillerFilter;
            var flowerFilter = LevEditor.SelectionFilter.EffectiveFlowerFilter;
            var startFilter = LevEditor.SelectionFilter.EffectiveStartFilter;
            var pictureFilter = LevEditor.SelectionFilter.EffectivePictureFilter;
            var textureFilter = LevEditor.SelectionFilter.EffectiveTextureFilter;
            foreach (Polygon x in Lev.Polygons)
            {
                if ((x.IsGrass && grassFilter) || (!x.IsGrass && groundFilter))
                {
                    for (var index = 0; index < x.Vertices.Count; index++)
                    {
                        Vector t = x.Vertices[index];
                        if (selectionPoly is null)
                            MarkSelectedInArea(ref t, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                        else
                        {
                            MarkSelectedInArea(ref t, selectionPoly);
                        }

                        x.Vertices[index] = t;
                    }
                }
            }

            foreach (var t in Lev.Objects)
            {
                var type = t.Type;
                var x = t;
                if ((type == ObjectType.Start && startFilter) ||
                    (type == ObjectType.Apple && appleFilter) ||
                    (type == ObjectType.Killer && killerFilter) ||
                    (type == ObjectType.Flower && flowerFilter))
                    if (selectionPoly is null)

                        MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin,
                            selectionyMax);
                    else
                    {
                        MarkSelectedInArea(ref x, selectionPoly);
                    }
            }

            foreach (var z in Lev.GraphicElements)
            {
                var x = z;
                if ((z.IsPicture && pictureFilter) ||
                    (z.IsTexture && textureFilter))
                    if (selectionPoly is null)
                        MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                    else
                    {
                        MarkSelectedInArea(ref x, selectionPoly);
                    }
            }

            if (LevEditor.PlayController.PlayingOrPaused)
            {
                var x = LevEditor.PlayController.Driver!.Body.Location;
                x.Mark = LevEditor.PlayController.PlayerSelection;
                if (selectionPoly is null)
                {
                    MarkSelectedInArea(ref x, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                }
                else
                {
                    MarkSelectedInArea(ref x, selectionPoly);
                }

                LevEditor.PlayController.PlayerSelection = x.Mark;
            }

            LevEditor.UpdateSelectionInfo();
            LevEditor.PreserveSelection();
            _state = new Idle();
        }
        else if (_state is Moving)
        {
            _state = new Idle();
            LevEditor.PlayController.CurrentBodyPart = null;
            LevEditor.SetModified(_currLevModification);
        }
    }

    private void MarkSelectedInArea<T>(ref T z, Polygon selectionPoly) where T : IPositionable
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
        else if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
            z.Mark = VectorMark.None;
    }

    public string GetHelp() =>
        "LMouse: select level elements; LShift: bend edge; LShift + click: lock edge angle; LAlt + click: select all inside.";

    private void MarkSelectedInArea<T>(ref T z, double selectionxMin, double selectionxMax,
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
        else if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
            z.Mark = VectorMark.None;
    }

    private T HandleMark<T>(T v) where T : IPositionable
    {
        if (!Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
        LevEditor.PreserveSelection();
        AdjustForGrid(ref CurrentPos);
        _state = new Moving(CurrentPos, null);
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
                LevEditor.HighlightText = "Apple: ";
                switch (currObj.AppleType)
                {
                    case AppleType.Normal:
                        LevEditor.HighlightText += "Normal";
                        break;
                    case AppleType.GravityUp:
                        LevEditor.HighlightText += "Gravity up";
                        break;
                    case AppleType.GravityDown:
                        LevEditor.HighlightText += "Gravity down";
                        break;
                    case AppleType.GravityLeft:
                        LevEditor.HighlightText += "Gravity left";
                        break;
                    case AppleType.GravityRight:
                        LevEditor.HighlightText += "Gravity right";
                        break;
                }

                LevEditor.HighlightText += ", animation number: " + currObj.AnimationNumber;
                break;
            case ObjectType.Killer:
                LevEditor.HighlightText = "Killer";
                break;
            case ObjectType.Flower:
                LevEditor.HighlightText = "Flower";
                break;
            case ObjectType.Start:
                LevEditor.HighlightText = "Start";
                break;
        }
    }

    private void ShowTextureInfo(int index)
    {
        var graphicElement = Lev.GraphicElements[index];
        LevEditor.HighlightText = graphicElement.Kind switch
        {
            GraphicElement.Picture p =>
                $"Picture: {p.PictureInfo.Name}, distance: {graphicElement.Distance}, clipping: {graphicElement.Clipping}",
            GraphicElement.Texture t =>
                $"Texture: {t.TextureInfo.Name}, mask: {t.MaskInfo.Name}, distance: {graphicElement.Distance}, clipping: {graphicElement.Clipping}",
            GraphicElement.MissingPicture p => $"Missing picture: {p.Name}, distance: {graphicElement.Distance}, clipping: {graphicElement.Clipping}",
            GraphicElement.MissingTexture t => $"Missing texture: {t.TextureName}, mask: {t.MaskName}, distance: {graphicElement.Distance}, clipping: {graphicElement.Clipping}",
            _ => LevEditor.HighlightText
        };
    }

    public override bool Busy => _state is not Idle;
}
