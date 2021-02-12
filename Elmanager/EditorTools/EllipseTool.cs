using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Forms;
using Elmanager.LevEditor;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.EditorTools
{
    internal class EllipseTool : ToolBase, IEditorTool
    {
        private Polygon _ellipse;
        private Vector? _ellipseCenter;
        private int _ellipseSteps = 10;

        internal EllipseTool(LevelEditor editor)
            : base(editor)
        {
        }

        public override bool Busy => CreatingEllipse;

        private bool CreatingEllipse => _ellipseCenter != null;

        public void Activate()
        {
            _ellipseSteps = Math.Max(Global.AppSettings.LevelEditor.EllipseSteps, 3);
            UpdateHelp();
        }

        public void ExtraRendering()
        {
            if (CreatingEllipse)
            {
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowGroundEdges)
                {
                    Renderer.DrawPolygon(_ellipse, Global.AppSettings.LevelEditor.RenderingSettings.GroundEdgeColor);
                }
            }
        }

        public List<Polygon> GetExtraPolygons()
        {
            var polys = new List<Polygon>();
            if (CreatingEllipse)
            {
                polys.Add(_ellipse);
            }

            return polys;
        }

        public void InActivate()
        {
            Global.AppSettings.LevelEditor.EllipseSteps = _ellipseSteps;
            if (!CreatingEllipse) return;
            _ellipseCenter = null;
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
                        _ellipseCenter = null;
                        Lev.Polygons.Add(_ellipse);
                        LevEditor.SetModified(LevModification.Ground);
                    }
                    else
                    {
                        _ellipseCenter = CurrentPos;
                        UpdateEllipse();
                    }

                    break;
                case MouseButtons.Right:
                    if (CreatingEllipse)
                    {
                        _ellipseCenter = null;
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

        public void MouseUp()
        {
        }

        public void UpdateHelp()
        {
            if (CreatingEllipse)
                LevEditor.InfoLabel.Text = "+/-: adjust number of sides. Edges in ellipse: " + _ellipseSteps;
            else
                LevEditor.InfoLabel.Text = "Left mouse button: select center point of the ellipse.";
        }

        private void UpdateEllipse()
        {
            if (_ellipseCenter is not { } c) return;
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                double dist =
                    Math.Sqrt((CurrentPos.X - c.X) * (CurrentPos.X - c.X) +
                              (CurrentPos.Y - c.Y) * (CurrentPos.Y - c.Y));
                _ellipse = Polygon.Ellipse(c, dist, dist, 0, _ellipseSteps);
            }
            else
                _ellipse = Polygon.Ellipse(c, CurrentPos.X - c.X,
                    CurrentPos.Y - c.Y, 0, _ellipseSteps);
        }
    }
}