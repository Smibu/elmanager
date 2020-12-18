using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class CutConnectTool : ToolBase, IEditorTool
    {
        private Vector _start;

        internal CutConnectTool(LevelEditor editor) : base(editor)
        {
        }

        private bool StartSelected { get; set; }

        public void Activate()
        {
            UpdateHelp();
        }

        public void ExtraRendering()
        {
            if (StartSelected)
                Renderer.DrawLine(_start, CurrentPos, Color.Blue);
        }

        public List<Polygon> GetExtraPolygons()
        {
            return new List<Polygon>();
        }

        public void InActivate()
        {
            StartSelected = false;
        }

        public void KeyDown(KeyEventArgs key)
        {
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
                        if (!TryConnect(_start, CurrentPos))
                            Cut(_start, CurrentPos);
                    }

                    break;
                case MouseButtons.Right:
                    StartSelected = false;
                    break;
            }

            UpdateHelp();
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            AdjustForGrid(CurrentPos);
        }

        public void MouseOutOfEditor()
        {
        }

        public void MouseUp()
        {
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = StartSelected
                ? "Left mouse button: set second vertex of the cut/connection edge."
                : "Left mouse button: set first vertex of the cut/connection edge.";
        }

        private void Cut(Vector v1, Vector v2)
        {
            MarkAllAs(VectorMark.None);
            bool anythingCut = false;
            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
            {
                Polygon x = Lev.Polygons[i];
                List<Polygon> cutPolygons = x.Cut(v1, v2, 0.01 * ZoomCtrl.ZoomLevel);
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

        private bool TryConnect(Vector v1, Vector v2)
        {
            MarkAllAs(VectorMark.None);
            List<Polygon> intersectingPolygons =
                Lev.Polygons.Where(x => !x.IsGrass && x.IntersectsWith(v1, v2)).ToList();
            bool anythingConnected = false;
            if (intersectingPolygons.Count == 2)
            {
                Polygon connected = GeometryUtils.Connect(intersectingPolygons[0], intersectingPolygons[1], _start,
                    CurrentPos, ZoomCtrl.ZoomLevel * 0.01);
                if (connected != null)
                {
                    Lev.Polygons.Remove(intersectingPolygons[0]);
                    Lev.Polygons.Remove(intersectingPolygons[1]);
                    Lev.Polygons.Add(connected);
                    anythingConnected = true;
                }
            }

            StartSelected = false;
            if (anythingConnected)
            {
                LevEditor.Modified = true;
            }

            return anythingConnected;
        }

        public override bool Busy => StartSelected;
    }
}