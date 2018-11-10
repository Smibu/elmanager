using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class FrameTool : ToolBase, IEditorTool
    {
        private Polygon _currentPolygon;
        private double _frameRadius = 0.2;
        private List<Polygon> _frames;
        private FrameType _frameType = FrameType.Normal;

        private enum FrameType
        {
            Normal,
            Inward,
            Outward
        }

        internal FrameTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool Framing { get; set; }

        public void Activate()
        {
            _frameRadius = Global.AppSettings.LevelEditor.FrameRadius;
            UpdateHelp();
            Renderer.AdditionalPolys = ExtraPolygons;
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = Framing
                                           ? "Mouse left: create frame; mouse right: cancel; +/-: adjust width; Space: Change type - " + _frameType
                                           : "Click the polygon to frame.";
        }

        public void ExtraRendering()
        {
            if (!Framing) return;
            Renderer.DrawPolygon(_frames[0], Color.Blue);
            Renderer.DrawPolygon(_frames[1], Color.Blue);
        }

        public void InActivate()
        {
            Global.AppSettings.LevelEditor.FrameRadius = _frameRadius;
            Renderer.AdditionalPolys = null;
            if (!Framing) return;
            CancelFraming();
            ResetHighlight();
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (!Framing) return;
            switch (key.KeyCode)
            {
                case Constants.Increase:
                    _frameRadius += 0.05;
                    break;
                case Constants.Decrease:
                    if (_frameRadius > 0.05)
                    {
                        _frameRadius -= 0.05;
                    }
                    break;
                case Keys.Space:
                    switch (_frameType)
                    {
                        case FrameType.Normal:
                            _frameType = FrameType.Inward;
                            break;
                        case FrameType.Inward:
                            _frameType = FrameType.Outward;
                            break;
                        case FrameType.Outward:
                            _frameType = FrameType.Normal;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    UpdateHelp();
                    break;
            }
            UpdateFrame();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            int nearestVertexIndex = GetNearestVertexIndex(CurrentPos);
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    if (nearestVertexIndex >= -1 && !Framing)
                    {
                        Framing = true;
                        _currentPolygon = NearestPolygon;
                        if (!_currentPolygon.IsCounterClockwise)
                        {
                            _currentPolygon.ChangeOrientation();
                        }
                        _currentPolygon.Mark = PolygonMark.Selected;
                        Lev.Polygons.Remove(_currentPolygon);
                        _frames = new List<Polygon>();
                        _frames.AddRange(MakeClosedPipe(_currentPolygon, _frameRadius));
                    }
                    else if (Framing)
                    {
                        Lev.Polygons.AddRange(_frames);
                        _frames = null;
                        Framing = false;
                        LevEditor.Modified = true;
                    }
                    break;
                case MouseButtons.Right:
                    if (Framing)
                        CancelFraming();
                    break;
            }
            UpdateHelp();
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            if (!Framing)
            {
                ResetHighlight();
                int nearestVertex = GetNearestVertexIndex(p);
                if (nearestVertex >= -1)
                {
                    ChangeCursorToHand();
                    if (NearestPolygon.Mark != PolygonMark.Selected)
                        NearestPolygon.Mark = PolygonMark.Highlight;
                }
                else
                    ChangeToDefaultCursorIfHand();
            }
            else
                ChangeToDefaultCursorIfHand();
        }

        public void MouseOutOfEditor()
        {
            if (!Framing)
            {
                ResetHighlight();
            }
        }

        public void MouseUp()
        {
        }

        private void ExtraPolygons()
        {
            if (!Framing) return;
            Renderer.DrawFilledTriangles(_frames[0].Decomposition);
            Renderer.DrawFilledTriangles(_frames[1].Decomposition);
        }

        private void CancelFraming()
        {
            Lev.Polygons.Add(_currentPolygon);
            _currentPolygon.Mark = PolygonMark.None;
            Framing = false;
        }

        private void UpdateFrame()
        {
            _frames = MakeClosedPipe(_currentPolygon, _frameRadius);
        }

        private List<Polygon> MakeClosedPipe(Polygon pipeLine, double radius)
        {
            List<Polygon> p = new List<Polygon> { new Polygon(), new Polygon() };
            if (pipeLine.Vertices.Count < 2)
                return p;
            for (int i = 0; i < pipeLine.Vertices.Count; i++)
            {
                switch (_frameType)
                {
                    case FrameType.Normal:
                        p[0].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                        p[1].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                        break;
                    case FrameType.Inward:
                        p[0].Add(pipeLine[i]);
                        p[1].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                        break;
                    case FrameType.Outward:
                        p[0].Add(pipeLine[i]);
                        p[1].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            p[0].UpdateDecomposition();
            p[1].UpdateDecomposition();
            return p;
        }

        public override bool Busy => Framing;
    }
}