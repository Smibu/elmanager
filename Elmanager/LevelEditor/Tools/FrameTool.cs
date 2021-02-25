using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Utilities;

namespace Elmanager.LevelEditor.Tools
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

        internal FrameTool(LevelEditorForm editor)
            : base(editor)
        {
        }

        private bool Framing { get; set; }

        public void Activate()
        {
            _frameRadius = Global.AppSettings.LevelEditor.FrameRadius;
            UpdateHelp();
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

        public List<Polygon> GetExtraPolygons()
        {
            var polys = new List<Polygon>();
            if (Framing)
            {
                polys = _frames;
            }

            return polys;
        }

        public void InActivate()
        {
            Global.AppSettings.LevelEditor.FrameRadius = _frameRadius;
            if (!Framing) return;
            CancelFraming();
            ResetHighlight();
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (!Framing) return;
            switch (key.KeyCode)
            {
                case KeyUtils.Increase:
                    _frameRadius += 0.05;
                    break;
                case KeyUtils.Decrease:
                    if (_frameRadius > 0.05)
                    {
                        _frameRadius -= 0.05;
                    }

                    break;
                case Keys.Space:
                    _frameType = _frameType switch
                    {
                        FrameType.Normal => FrameType.Inward,
                        FrameType.Inward => FrameType.Outward,
                        FrameType.Outward => FrameType.Normal,
                        _ => throw new ArgumentOutOfRangeException()
                    };

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
                        LevEditor.SetModified(LevModification.Ground);
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
            List<Polygon> p = new List<Polygon> {new(), new()};
            if (pipeLine.Vertices.Count < 2)
                return p;
            for (int i = 0; i < pipeLine.Vertices.Count; i++)
            {
                switch (_frameType)
                {
                    case FrameType.Normal:
                        p[0].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                        p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                        break;
                    case FrameType.Inward:
                        p[0].Add(pipeLine[i]);
                        p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                        break;
                    case FrameType.Outward:
                        p[0].Add(pipeLine[i]);
                        p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
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