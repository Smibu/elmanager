using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Forms;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.EditorTools
{
    internal class SmoothenTool : ToolBase, IEditorTool
    {
        private Polygon _currentPolygon;
        private bool _smoothAll;
        private List<Polygon> _smoothPolys;
        private int _smoothSteps = 3;
        private int _smoothVertexOffset = 50;
        private bool _unsmooth;
        private double _unsmoothAngle = 10;
        private double _unsmoothLength = 1.0;

        internal SmoothenTool(LevelEditor editor) : base(editor)
        {
        }

        private bool Smoothing { get; set; }

        public void Activate()
        {
            _smoothSteps = Math.Max(Global.AppSettings.LevelEditor.SmoothSteps, 3);
            _smoothVertexOffset = Math.Max(Global.AppSettings.LevelEditor.SmoothVertexOffset, 50);
            _unsmoothAngle = Math.Max(Global.AppSettings.LevelEditor.UnsmoothAngle, 1);
            _unsmoothLength = Math.Max(Global.AppSettings.LevelEditor.UnsmoothLength, 0.1);
            UpdateHelp();
        }

        public void ExtraRendering()
        {
            if (Smoothing)
                foreach (Polygon x in _smoothPolys)
                    Renderer.DrawPolygon(x, Color.Red);
        }

        public List<Polygon> GetExtraPolygons()
        {
            return new();
        }

        public void InActivate()
        {
            Global.AppSettings.LevelEditor.SmoothSteps = _smoothSteps;
            Global.AppSettings.LevelEditor.SmoothVertexOffset = _smoothVertexOffset;
            Global.AppSettings.LevelEditor.UnsmoothAngle = _unsmoothAngle;
            Global.AppSettings.LevelEditor.UnsmoothLength = _unsmoothLength;
            CancelSmoothing();
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (Smoothing)
            {
                switch (key.KeyCode)
                {
                    case Constants.Increase:
                        if (!_unsmooth)
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                if (_smoothVertexOffset < 100)
                                    _smoothVertexOffset += 1;
                            }
                            else
                            {
                                _smoothSteps++;
                            }
                        }
                        else
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                if (_unsmoothAngle < 180)
                                    _unsmoothAngle += 2;
                            }
                            else
                            {
                                if (_unsmoothLength < 20)
                                    _unsmoothLength += 0.1;
                            }
                        }

                        break;
                    case Constants.Decrease:
                        if (!_unsmooth)
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                if (_smoothVertexOffset > 50)
                                    _smoothVertexOffset -= 1;
                            }
                            else
                            {
                                if (_smoothSteps > 2)
                                    _smoothSteps--;
                            }
                        }
                        else
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                if (_unsmoothAngle > 0)
                                    _unsmoothAngle -= 2;
                            }
                            else
                            {
                                if (_unsmoothLength > 0.1)
                                    _unsmoothLength -= 0.1;
                            }
                        }

                        break;
                }

                UpdateHelp();
                UpdatePolygonSmooth();
            }
            else
            {
                switch (key.KeyCode)
                {
                    case Keys.Space:
                        if (!Smoothing)
                        {
                            Smoothing = true;
                            _smoothAll = true;
                            _unsmooth = Keyboard.IsKeyDown(Key.LeftCtrl);
                            UpdateHelp();
                            UpdatePolygonSmooth();
                        }

                        break;
                }
            }
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            int nearestVertexIndex = GetNearestVertexIndex(CurrentPos);
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    if (Smoothing)
                    {
                        if (_smoothAll)
                        {
                            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
                            {
                                Polygon x = Lev.Polygons[i];
                                if (IsSmoothable(x))
                                    Lev.Polygons.RemoveAt(i);
                            }
                        }
                        else
                            Lev.Polygons.Remove(_currentPolygon);

                        Lev.Polygons.AddRange(_smoothPolys);
                        Smoothing = false;
                        LevEditor.Modified = true;
                        LevEditor.UpdateSelectionInfo();
                        foreach (Polygon x in _smoothPolys)
                            x.UpdateDecomposition();
                    }
                    else if (nearestVertexIndex >= -1)
                    {
                        Smoothing = true;
                        _smoothAll = false;
                        _currentPolygon = NearestPolygon;
                        ResetHighlight();
                        _unsmooth = Keyboard.IsKeyDown(Key.LeftCtrl);
                        UpdateHelp();
                        UpdatePolygonSmooth();
                    }

                    break;
                case MouseButtons.Right:
                    CancelSmoothing();
                    break;
            }

            UpdateHelp();
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            if (!Smoothing)
            {
                int nearest = GetNearestVertexIndex(p);
                ResetHighlight();
                if (nearest >= -1)
                {
                    NearestPolygon.Mark = PolygonMark.Highlight;
                    ChangeCursorToHand();
                }
                else
                    ChangeToDefaultCursorIfHand();
            }
            else
                ChangeToDefaultCursorIfHand();
        }

        public void MouseOutOfEditor()
        {
            ResetHighlight();
        }

        public void MouseUp()
        {
        }

        public void UpdateHelp()
        {
            if (Smoothing)
            {
                LevEditor.InfoLabel.Text = "Left click: apply; right click: cancel; (Ctrl) + +/-: adjust parameters.";
                if (!_unsmooth)
                    LevEditor.InfoLabel.Text += " (" + _smoothSteps + ", " +
                                                (_smoothVertexOffset / 100.0).ToString("F2") +
                                                ")";
                else
                    LevEditor.InfoLabel.Text += " (" + _unsmoothLength.ToString("F2") + ", " +
                                                _unsmoothAngle.ToString("F2") + ")";
            }
            else
                LevEditor.InfoLabel.Text = "Click a polygon or press Space to smooth selected. Hold Ctrl to unsmooth.";
        }

        private static bool IsSmoothable(Polygon p)
        {
            for (int i = 0; i < p.Count; i++)
                if (p[i].Mark == VectorMark.Selected && p[i + 1].Mark == VectorMark.Selected &&
                    p[i + 2].Mark == VectorMark.Selected)
                    return true;
            return false;
        }

        private void CancelSmoothing()
        {
            if (!Smoothing) return;
            Smoothing = false;
            ResetHighlight();
        }

        private void UpdatePolygonSmooth()
        {
            _smoothPolys = new List<Polygon>();
            if (_smoothAll)
            {
                foreach (Polygon x in Lev.Polygons.Where(IsSmoothable))
                {
                    _smoothPolys.Add(!_unsmooth
                        ? x.Smoothen(_smoothSteps, _smoothVertexOffset / 100.0, true)
                        : x.Unsmoothen(_unsmoothAngle, _unsmoothLength, true));
                }
            }
            else
            {
                _smoothPolys.Add(_unsmooth
                    ? _currentPolygon.Unsmoothen(_unsmoothAngle, _unsmoothLength, false)
                    : _currentPolygon.Smoothen(_smoothSteps, _smoothVertexOffset / 100.0, false));
            }
        }

        public override bool Busy => Smoothing;
    }
}