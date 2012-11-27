using System;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Forms;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.EditorTools
{
    internal class EllipseTool : ToolBase, IEditorTool
    {
        private bool _creatingEllipse;
        private Polygon _ellipse;
        private Vector _ellipseCenter;
        private int _ellipseSteps = 10;

        internal EllipseTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool CreatingEllipse
        {
            get { return _creatingEllipse; }
            set
            {
                _creatingEllipse = value;
                _Busy = value;
            }
        }

        public void Activate()
        {
            UpdateHelp();
            Renderer.AdditionalPolys = ExtraPolys;
            Renderer.RedrawScene();
        }

        public void ExtraRendering()
        {
            if (CreatingEllipse)
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowGroundEdges)
                    Renderer.DrawPolygon(_ellipse, Global.AppSettings.LevelEditor.RenderingSettings.GroundEdgeColor);
        }

        public void InActivate()
        {
            if (!CreatingEllipse) return;
            CreatingEllipse = false;
            Renderer.RedrawScene();
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (!CreatingEllipse) return;
            switch (key.KeyCode)
            {
                case Constants.Increase:
                    _ellipseSteps++;
                    UpdateEllipse();
                    break;
                case Constants.Decrease:
                    if (_ellipseSteps > 3)
                    {
                        _ellipseSteps--;
                        UpdateEllipse();
                    }
                    break;
            }
            UpdateHelp();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    if (CreatingEllipse)
                    {
                        CreatingEllipse = false;
                        Lev.Polygons.Add(_ellipse);
                        LevEditor.Modified = true;
                        Renderer.RedrawScene();
                    }
                    else
                    {
                        _ellipseCenter = CurrentPos;
                        CreatingEllipse = true;
                    }
                    break;
                case MouseButtons.Right:
                    if (CreatingEllipse)
                    {
                        CreatingEllipse = false;
                        Renderer.RedrawScene();
                    }
                    break;
            }
            UpdateHelp();
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            AdjustForGrid(CurrentPos);
            UpdateEllipse();
        }

        public void MouseOutOfEditor()
        {
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        public void UpdateHelp()
        {
            if (CreatingEllipse)
                LevEditor.InfoLabel.Text = "+/-: adjust number of sides. Edges in ellipse: " + _ellipseSteps;
            else
                LevEditor.InfoLabel.Text = "Left mouse button: select center point of the ellipse.";
        }

        private void ExtraPolys()
        {
            if (CreatingEllipse)
                Renderer.DrawFilledTriangles(_ellipse.Decomposition);
        }

        private void UpdateEllipse()
        {
            if (!CreatingEllipse) return;
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                double dist =
                    Math.Sqrt((CurrentPos.X - _ellipseCenter.X) * (CurrentPos.X - _ellipseCenter.X) +
                              (CurrentPos.Y - _ellipseCenter.Y) * (CurrentPos.Y - _ellipseCenter.Y));
                _ellipse = Polygon.Ellipse(_ellipseCenter, dist, dist, 0, _ellipseSteps);
            }
            else
                _ellipse = Polygon.Ellipse(_ellipseCenter, CurrentPos.X - _ellipseCenter.X,
                                           CurrentPos.Y - _ellipseCenter.Y, 0, _ellipseSteps);
            Renderer.RedrawScene();
        }
    }
}