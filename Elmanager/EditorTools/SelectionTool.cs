using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Forms;
using Elmanager.LevEditor;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.EditorTools
{
    internal class SelectionTool : ToolBase, IEditorTool
    {
        private LevModification _currLevModification;
        private Vector _lockCenter; //for lock lines -mode
        private Vector _lockNext; //for lock lines -mode
        private Vector _lockPrev; //for lock lines -mode
        private bool _lockingLines;
        private Vector _moveStartPosition;
        private Polygon _selectionPoly;

        private bool FreeSelecting { get; set; }

        private Vector _selectionStartPoint;
        private double _mouseTrip;
        private Vector _lastMousePosition;

        internal SelectionTool(LevelEditor editor)
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
            else if (FreeSelecting)
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
            int nearestVertexIndex = GetNearestVertexIndex(p);
            int nearestObjectIndex = GetNearestObjectIndex(p);
            int nearestPictureIndex = GetNearestPictureIndex(p);
            var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    var somethingGrabbed = true;
                    if (nearestVertexIndex >= -1 && Keyboard.IsKeyDown(Key.LeftAlt))
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            MarkAllAs(VectorMark.None);
                        }

                        NearestPolygon.MarkVectorsAs(VectorMark.Selected);
                        var inearest = NearestPolygon.ToIPolygon();
                        foreach (var polygon in Lev.Polygons.Where(polygon => polygon.ToIPolygon().Within(inearest)))
                        {
                            polygon.MarkVectorsAs(VectorMark.Selected);
                        }

                        foreach (var obj in Lev.Objects)
                        {
                            if (NearestPolygon.AreaHasPoint(obj.Position))
                            {
                                obj.Position.Mark = VectorMark.Selected;
                            }
                        }

                        foreach (var pic in Lev.Pictures)
                        {
                            if (NearestPolygon.AreaHasPoint(pic.Position))
                            {
                                pic.Position.Mark = VectorMark.Selected;
                            }
                        }

                        EndSelectionHandling();
                    }
                    else if (nearestVertexIndex >= 0)
                    {
                        var v = NearestPolygon[nearestVertexIndex];
                        HandleMark(ref v);
                        NearestPolygon.Vertices[nearestVertexIndex] = v;
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            _lockCenter = NearestPolygon[nearestVertexIndex];
                            _lockPrev = NearestPolygon[nearestVertexIndex - 1];
                            _lockNext = NearestPolygon[nearestVertexIndex + 1];
                            _lockingLines = true;
                            _moveStartPosition = _lockCenter;
                        }
                    }
                    else if (nearestVertexIndex == -1)
                    {
                        int nearestSegmentIndex = NearestPolygon.GetNearestSegmentIndex(p);
                        AdjustForGrid(p);
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            MarkAllAs(VectorMark.None);
                            p.Mark = VectorMark.Selected;
                            NearestPolygon.Insert(nearestSegmentIndex + 1, p);
                            LevEditor.SetModified(NearestPolygon.IsGrass ? LevModification.Decorations : LevModification.Ground);
                        }
                        else
                        {
                            if (
                                !(NearestPolygon[nearestSegmentIndex].Mark == VectorMark.Selected &&
                                  NearestPolygon[nearestSegmentIndex + 1].Mark == VectorMark.Selected))
                            {
                                if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                                {
                                    MarkAllAs(VectorMark.None);
                                    NearestPolygon.MarkVectorsAs(VectorMark.Selected);
                                }
                            }

                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                NearestPolygon.MarkVectorsAs(
                                    NearestPolygon.Vertices.TrueForAll(v => v.Mark == VectorMark.Selected)
                                        ? VectorMark.None
                                        : VectorMark.Selected);
                            }
                        }

                        EndSelectionHandling();
                    }
                    else if (nearestObjectIndex >= 0)
                        HandleMark(ref Lev.Objects[nearestObjectIndex].Position);
                    else if (nearestPictureIndex >= 0)
                        HandleMark(ref Lev.Pictures[nearestPictureIndex].Position);
                    else if (nearestBodyPart != null)
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
                            FreeSelecting = true;
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
                        LevEditor.PlayController.CurrentBodyPart = LevEditor.PlayController.Driver.GetBody();
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
                AdjustForGrid(p);
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
                        x.UpdateDecomposition();
                        anythingMoved |= x.IsGrass ? LevModification.Decorations : LevModification.Ground;
                    }
                }

                foreach (LevObject x in Lev.Objects.Where(x => x.Position.Mark == VectorMark.Selected))
                {
                    x.Position += delta;
                    anythingMoved |= LevModification.Objects;
                }

                foreach (Picture z in Lev.Pictures.Where(z => z.Position.Mark == VectorMark.Selected))
                {
                    z.Position += delta;
                    anythingMoved |= LevModification.Decorations;
                }

                _currLevModification = anythingMoved;
                if (anythingMoved.HasFlag(LevModification.Ground) || anythingMoved.HasFlag(LevModification.Objects))
                {
                    LevEditor.PlayController.UpdateEngine(Lev);
                }

                if (LevEditor.PlayController.CurrentBodyPart != null)
                {
                    LevEditor.PlayController.CurrentBodyPart.Position += delta;
                }

                Vector.MarkDefault = VectorMark.None;
                _moveStartPosition = p;
            }
            else if (FreeSelecting)
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
                int nearestVertex = GetNearestVertexIndex(p);
                int nearestObject = GetNearestObjectIndex(p);
                int nearestTextureIndex = GetNearestPictureIndex(p);
                var nearestBodyPart = LevEditor.PlayController.GetNearestDriverBodyPart(p, CaptureRadiusScaled);
                if (nearestVertex == -1)
                {
                    ChangeCursorToHand();
                    NearestPolygon.Mark = PolygonMark.Highlight;
                    LevEditor.HighlightLabel.Text = NearestPolygon.IsGrass ? "Grass" : "Ground";
                    LevEditor.HighlightLabel.Text += " polygon, " + NearestPolygon.Count + " vertices";
                }
                else if (nearestVertex >= 0)
                {
                    ChangeCursorToHand();
                    Vector b = NearestPolygon.Vertices[nearestVertex];
                    if (b.Mark == VectorMark.None)
                    {
                        b.Mark = VectorMark.Highlight;
                        NearestPolygon.Vertices[nearestVertex] = b;
                    }
                    LevEditor.HighlightLabel.Text = NearestPolygon.IsGrass ? "Grass" : "Ground";
                    LevEditor.HighlightLabel.Text += " polygon, vertex " + (nearestVertex + 1) + " of " +
                                                     NearestPolygon.Count + " vertices";
                }
                else if (nearestObject >= 0)
                {
                    ChangeCursorToHand();
                    if (Lev.Objects[nearestObject].Position.Mark == VectorMark.None)
                        Lev.Objects[nearestObject].Position.Mark = VectorMark.Highlight;
                    ShowObjectInfo(nearestObject);
                }
                else if (nearestTextureIndex >= 0)
                {
                    ChangeCursorToHand();
                    if (Lev.Pictures[nearestTextureIndex].Position.Mark == VectorMark.None)
                        Lev.Pictures[nearestTextureIndex].Position.Mark = VectorMark.Highlight;
                    ShowTextureInfo(nearestTextureIndex);
                }
                else if (nearestBodyPart != null)
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
                            if (RectSelecting)
                                MarkSelectedInArea(ref t, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                            else
                            {
                                MarkSelectedInArea(ref t, _selectionPoly);
                            }

                            x.Vertices[index] = t;
                        }
                    }
                }

                foreach (LevObject t in Lev.Objects)
                {
                    ObjectType type = t.Type;
                    if (type == ObjectType.Start || (type == ObjectType.Apple && appleFilter) ||
                        (type == ObjectType.Killer && killerFilter) ||
                        (type == ObjectType.Flower && flowerFilter))
                        if (RectSelecting)
                            MarkSelectedInArea(ref t.Position, selectionxMin, selectionxMax, selectionyMin,
                                selectionyMax);
                        else
                        {
                            MarkSelectedInArea(ref t.Position, _selectionPoly);
                        }
                }

                foreach (Picture z in Lev.Pictures)
                {
                    if ((z.IsPicture && pictureFilter) || (!z.IsPicture && textureFilter))
                        if (RectSelecting)
                            MarkSelectedInArea(ref z.Position, selectionxMin, selectionxMax, selectionyMin, selectionyMax);
                        else
                        {
                            MarkSelectedInArea(ref z.Position, _selectionPoly);
                        }
                }

                if (LevEditor.PlayController.PlayingOrPaused)
                {
                    var x = LevEditor.PlayController.Driver.Body.Location;
                    x.Mark = LevEditor.PlayController.PlayerSelection;
                    if (RectSelecting)
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
                FreeSelecting = false;
            }
            else if (Moving)
            {
                Moving = false;
                _lockingLines = false;
                LevEditor.PlayController.CurrentBodyPart = null;
                LevEditor.SetModified(_currLevModification);
            }
        }

        private static void MarkSelectedInArea(ref Vector z, Polygon selectionPoly)
        {
            if (selectionPoly.AreaHasPoint(z))
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
            LevEditor.InfoLabel.Text = "Left mouse button: select level elements; Left Shift: Bend edge";
        }

        private static void MarkSelectedInArea(ref Vector z, double selectionxMin, double selectionxMax,
            double selectionyMin, double selectionyMax)
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

        private void HandleMark(ref Vector v)
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
        }

        private void EndSelectionHandling()
        {
            Moving = true;
            LevEditor.PreserveSelection();
            AdjustForGrid(CurrentPos);
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
            Picture picture = Lev.Pictures[index];
            if (picture.IsPicture)
                LevEditor.HighlightLabel.Text = "Picture: " + picture.Name +
                                                ", distance: " +
                                                picture.Distance + ", clipping: " + picture.Clipping;
            else
            {
                LevEditor.HighlightLabel.Text = "Texture: " + picture.TextureName + ", mask: " + picture.Name +
                                                ", distance: " +
                                                picture.Distance + ", clipping: " + picture.Clipping;
            }
        }

        public override bool Busy => RectSelecting || FreeSelecting || Moving;
    }
}