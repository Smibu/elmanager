using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Utilities;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class EllipseTool : ToolBase, IEditorTool
{
    private Polygon? _ellipse;
    private Vector? _ellipseCenter;
    private int _ellipseSteps = 10;

    internal EllipseTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    public override bool Busy => CreatingEllipse;

    private bool CreatingEllipse => _ellipseCenter is { };

    public void Activate()
    {
        _ellipseSteps = Math.Max(Global.AppSettings.LevelEditor.EllipseSteps, 3);
        UpdateHelp();
    }

    public void ExtraRendering()
    {
        if (_ellipse is { })
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
        if (_ellipse is { })
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
        _ellipse = null;
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (!CreatingEllipse) return;
        switch (key.KeyCode)
        {
            case KeyUtils.Increase:
                _ellipseSteps++;
                UpdateEllipse();
                break;
            case KeyUtils.Decrease:
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
                if (_ellipse is { })
                {
                    Lev.Polygons.Add(_ellipse);
                    InActivate();
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
                    InActivate();
                }

                break;
        }

        UpdateHelp();
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
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