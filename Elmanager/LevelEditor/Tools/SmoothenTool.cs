using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Utilities;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class SmoothenTool : ToolBase, IEditorTool
{
    private List<Polygon> _smoothPolys = new();
    private int _smoothSteps = 3;
    private int _smoothVertexOffset = 50;
    private bool _unsmooth;
    private double _unsmoothAngle = 10;
    private double _unsmoothLength = 1.0;

    internal SmoothenTool(LevelEditorForm editor) : base(editor)
    {
    }

    private SmoothState? Smoothing { get; set; }

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
        if (Smoothing is { })
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
        if (Smoothing is { })
        {
            switch (key.KeyCode)
            {
                case KeyUtils.Increase:
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
                case KeyUtils.Decrease:
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
                    if (Smoothing is null)
                    {
                        Smoothing = SmoothState.All;
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
        var info = GetNearestVertexInfo(CurrentPos);
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                if (Smoothing is { })
                {
                    switch (Smoothing)
                    {
                        case SmoothState.AllSmooth:
                        {
                            Lev.Polygons.RemoveAll(IsSmoothable);
                            break;
                        }
                        case SmoothState.PolygonSmooth p:
                            Lev.Polygons.Remove(p.P);
                            break;
                    }

                    Lev.Polygons.AddRange(_smoothPolys);
                    Smoothing = null;
                    LevEditor.SetModified(LevModification.Ground);
                    LevEditor.UpdateSelectionInfo();
                    foreach (Polygon x in _smoothPolys)
                        x.UpdateDecomposition();
                }
                else if (info is { } v)
                {
                    Smoothing = SmoothState.Polygon(v.Polygon);
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
        if (Smoothing is null)
        {
            ResetHighlight();
            if (GetNearestVertexInfo(p) is { } v)
            {
                v.Polygon.Mark = PolygonMark.Highlight;
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
        if (Smoothing is { })
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
        if (Smoothing is null) return;
        Smoothing = null;
        ResetHighlight();
    }

    private void UpdatePolygonSmooth()
    {
        _smoothPolys = Smoothing switch
        {
            SmoothState.AllSmooth => Lev.Polygons.Where(IsSmoothable)
                .Select(x => ApplyPolygonSmooth(x, true))
                .ToList(),
            SmoothState.PolygonSmooth(var p) => new List<Polygon>
            {
                ApplyPolygonSmooth(p, false)
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Polygon ApplyPolygonSmooth(Polygon p, bool onlySelected) =>
        _unsmooth
            ? p.Unsmoothen(_unsmoothAngle, _unsmoothLength, onlySelected)
            : p.Smoothen(_smoothSteps, _smoothVertexOffset / 100.0, onlySelected);

    public override bool Busy => Smoothing is { };
}