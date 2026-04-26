using System;
using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class EllipseTool : ToolBase, IEditorTool
{
    private Polygon? _ellipse;
    private Vector? _ellipseCenter;
    private int _ellipseSteps = 10;

    public EllipseTool(ILevelEditor editor)
        : base(editor)
    {
    }

    public override bool Busy => CreatingEllipse;

    private bool CreatingEllipse => _ellipseCenter is { };

    public void Activate()
    {
        _ellipseSteps = Math.Max(LevEditor.Settings.EllipseSteps, 3);
    }

    public void ExtraRendering()
    {
        if (_ellipse is { })
        {
            if (LevEditor.RenderingSettings.ShowGroundEdges)
            {
                Renderer.DrawPolygon(_ellipse, LevEditor.RenderingSettings.GroundEdgeColor);
            }
        }
    }

    public TransientElements GetTransientElements(bool hasFocus)
    {
        var polys = new List<Polygon>();
        if (_ellipse is { })
        {
            polys.Add(_ellipse);
        }

        return TransientElements.FromPolygons(polys);
    }

    public LevVisualChange InActivate()
    {
        LevEditor.Settings.EllipseSteps = _ellipseSteps;
        if (CreatingEllipse)
        {
            _ellipseCenter = null;
            _ellipse = null;
            return LevVisualChange.Ground;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        if (!CreatingEllipse) return LevVisualChange.Nothing;
        switch (key.KeyCode)
        {
            case EditorKeyUtils.Increase:
                _ellipseSteps++;
                UpdateEllipse();
                break;
            case EditorKeyUtils.Decrease:
                if (_ellipseSteps > 3)
                {
                    _ellipseSteps--;
                    UpdateEllipse();
                }

                break;
        }

        return LevVisualChange.Ground;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:
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
            case EditorMouseButton.Right:
                if (CreatingEllipse)
                {
                    return InActivate();
                }

                break;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        UpdateEllipse();
        return _ellipse is not null ? LevVisualChange.Ground : LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
    }

    public string GetHelp() =>
        CreatingEllipse
            ? $"+/-: adjust number of sides ({_ellipseSteps}); RMouse: cancel."
            : "LMouse: select center point of the ellipse.";

    private void UpdateEllipse()
    {
        if (_ellipseCenter is not { } c) return;
        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
