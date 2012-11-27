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
        private bool _framing;

        internal FrameTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool Framing
        {
            get { return _framing; }
            set
            {
                _framing = value;
                _Busy = value;
            }
        }

        #region IEditorTool Members

        public void Activate()
        {
            UpdateHelp();
            Renderer.AdditionalPolys = ExtraPolygons;
            Renderer.RedrawScene();
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = Framing
                                           ? "Left mouse button: create frame; right mouse button: cancel; +/-: adjust frame width."
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
                    UpdateFrame();
                    break;
                case Constants.Decrease:
                    if (_frameRadius > 0.05)
                    {
                        _frameRadius -= 0.05;
                        UpdateFrame();
                    }
                    break;
            }
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
            Renderer.RedrawScene();
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
                    ChangeToDefaultCursor();
                Renderer.RedrawScene();
            }
            else
                ChangeToDefaultCursor();
        }

        public void MouseOutOfEditor()
        {
            if (!Framing)
            {
                ResetHighlight();
                Renderer.RedrawScene();
            }
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        #endregion

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
            Renderer.RedrawScene();
        }

        private static List<Polygon> MakeClosedPipe(Polygon pipeLine, double radius)
        {
            List<Polygon> p = new List<Polygon> {new Polygon(), new Polygon()};
            if (pipeLine.Vertices.Count < 2)
                return p;
            for (int i = 0; i < pipeLine.Vertices.Count; i++)
            {
                p[0].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                p[1].Add(Geometry.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
            }
            p[0].UpdateDecomposition();
            p[1].UpdateDecomposition();
            return p;
        }
    }
}