using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class CutConnectTool : ToolBase, IEditorTool
    {
        private bool _connecting;
        private Vector _start;
        private bool _startSelected;

        internal CutConnectTool(LevelEditor editor) : base(editor)
        {
        }

        private bool StartSelected
        {
            get { return _startSelected; }
            set
            {
                _startSelected = value;
                _Busy = value;
            }
        }

        public void Activate()
        {
            UpdateHelp();
            Renderer.RedrawScene();
        }

        public void ExtraRendering()
        {
            if (StartSelected)
                Renderer.DrawLine(_start, CurrentPos, Color.Blue);
        }

        public void InActivate()
        {
            StartSelected = false;
        }

        public void KeyDown(KeyEventArgs key)
        {
            switch (key.KeyCode)
            {
                case Keys.Space:
                    if (!StartSelected)
                    {
                        _connecting = !_connecting;
                        UpdateHelp();
                    }
                    break;
            }
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    AdjustForGrid(CurrentPos);
                    if (!StartSelected)
                    {
                        StartSelected = true;
                        _start = CurrentPos;
                    }
                    else
                    {
                        if (_connecting)
                            TryConnect(_start, CurrentPos);
                        else
                            Cut(_start, CurrentPos);
                    }
                    break;
                case MouseButtons.Right:
                    StartSelected = false;
                    break;
            }
            Renderer.RedrawScene();
            UpdateHelp();
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            AdjustForGrid(CurrentPos);
            if (StartSelected)
                Renderer.RedrawScene();
        }

        public void MouseOutOfEditor()
        {
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        public void UpdateHelp()
        {
            if (_connecting)
            {
                LevEditor.InfoLabel.Text = StartSelected
                                               ? "Left mouse button: select second vertex of the connection edge."
                                               : "Left mouse button: select first vertex of the connection edge.";
            }
            else
            {
                LevEditor.InfoLabel.Text = StartSelected
                                               ? "Left mouse button: select second vertex of the cutting edge."
                                               : "Left mouse button: select first vertex of the cutting edge.";
            }
        }

        private void Cut(Vector v1, Vector v2)
        {
            MarkAllAs(Geometry.VectorMark.None);
            bool anythingCut = false;
            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
            {
                Polygon x = Lev.Polygons[i];
                List<Polygon> cutPolygons = x.Cut(v1, v2, 0.01 * Renderer.ZoomLevel);
                if (cutPolygons == null) continue;
                anythingCut = true;
                Lev.Polygons.Remove(x);
                Lev.Polygons.AddRange(cutPolygons);
                foreach (Polygon z in cutPolygons)
                    z.UpdateDecomposition();
            }
            StartSelected = false;
            if (anythingCut)
                LevEditor.Modified = true;
        }

        private void TryConnect(Vector v1, Vector v2)
        {
            MarkAllAs(Geometry.VectorMark.None);
            List<Polygon> intersectingPolygons =
                Lev.Polygons.Where(x => !x.IsGrass && x.IntersectsWith(v1, v2)).ToList();
            if (intersectingPolygons.Count == 2)
            {
                Polygon connected = Geometry.Connect(intersectingPolygons[0], intersectingPolygons[1], _start,
                                                     CurrentPos, Renderer.ZoomLevel * 0.01);
                if (connected != null)
                {
                    Lev.Polygons.Remove(intersectingPolygons[0]);
                    Lev.Polygons.Remove(intersectingPolygons[1]);
                    Lev.Polygons.Add(connected);
                    LevEditor.Modified = true;
                }
            }
            StartSelected = false;
            Renderer.RedrawScene();
        }
    }
}