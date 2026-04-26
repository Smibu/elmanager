using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class SmoothenTool : ToolBase, IEditorTool
{
    private List<Polygon> _smoothPolys = new();
    private int _smoothSteps = 3;
    private int _smoothVertexOffset = 50;
    private bool _unsmooth;
    private double _unsmoothAngle = 10;
    private double _unsmoothLength = 1.0;

    public SmoothenTool(ILevelEditor editor) : base(editor)
    {
    }

    private SmoothState? Smoothing { get; set; }

    public void Activate()
    {
        _smoothSteps = Math.Max(LevEditor.Settings.SmoothSteps, 3);
        _smoothVertexOffset = Math.Max(LevEditor.Settings.SmoothVertexOffset, 50);
        _unsmoothAngle = Math.Max(LevEditor.Settings.UnsmoothAngle, 1);
        _unsmoothLength = Math.Max(LevEditor.Settings.UnsmoothLength, 0.1);
    }

    public void ExtraRendering()
    {
        if (Smoothing is { })
            foreach (Polygon x in _smoothPolys)
                Renderer.DrawPolygon(x, Color.Red);
    }

    public LevVisualChange InActivate()
    {
        LevEditor.Settings.SmoothSteps = _smoothSteps;
        LevEditor.Settings.SmoothVertexOffset = _smoothVertexOffset;
        LevEditor.Settings.UnsmoothAngle = _unsmoothAngle;
        LevEditor.Settings.UnsmoothLength = _unsmoothLength;
        CancelSmoothing();
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        if (Smoothing is { })
        {
            switch (key.KeyCode)
            {
                case EditorKeyUtils.Increase:
                    if (!_unsmooth)
                    {
                        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
                        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
                case EditorKeyUtils.Decrease:
                    if (!_unsmooth)
                    {
                        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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
                        if (Keyboard.IsKeyDown(ModifierKey.LeftCtrl))
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

            UpdatePolygonSmooth();
        }
        else
        {
            switch (key.KeyCode)
            {
                case EditorKey.Space:
                    if (Smoothing is null)
                    {
                        Smoothing = SmoothState.All;
                        _unsmooth = Keyboard.IsKeyDown(ModifierKey.LeftCtrl);
                        UpdatePolygonSmooth();
                    }

                    break;
            }
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        var info = GetNearestVertexInfo(CurrentPos);
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:
                if (Smoothing is { })
                {
                    switch (Smoothing)
                    {
                        case SmoothState.AllSmooth:
                            Lev.Polygons.RemoveAll(IsSmoothable);
                            break;
                        case SmoothState.PolygonSmooth p:
                            Lev.Polygons.Remove(p.P);
                            break;
                    }

                    Lev.Polygons.AddRange(_smoothPolys);
                    Smoothing = null;
                    LevEditor.SetModified(LevModification.Ground);
                    LevEditor.UpdateSelectionInfo();
                    foreach (Polygon x in _smoothPolys)
                        x.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                }
                else if (info is { } v)
                {
                    Smoothing = SmoothState.Polygon(v.Polygon);
                    ResetHighlight();
                    _unsmooth = Keyboard.IsKeyDown(ModifierKey.LeftCtrl);
                    UpdatePolygonSmooth();
                }

                break;
            case EditorMouseButton.Right:
                CancelSmoothing();
                break;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (Smoothing is null)
        {
            ResetHighlight();
            if (GetNearestVertexInfo(p) is { } v)
            {
                LevEditor.CurrentHighlight = new HighlightTarget.PolygonTarget(v.Polygon);
                ChangeCursorToHand();
            }
            else
                ChangeToDefaultCursorIfHand();
        }
        else
            ChangeToDefaultCursorIfHand();

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        ResetHighlight();
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
    }

    public string GetHelp()
    {
        if (Smoothing is { })
        {
            var text = "LMouse: apply; (Ctrl) + +/-: adjust parameters";
            if (!_unsmooth)
                text += " (" + _smoothSteps + ", " +
                                            (_smoothVertexOffset / 100.0).ToString("F2") +
                                            ")";
            else
                text += " (" + _unsmoothLength.ToString("F2") + ", " +
                                            _unsmoothAngle.ToString("F2") + ")";

            text += "; RMouse: cancel.";
            return text;
        }
        else
            return "LMouse: smooth a polygon; Space: smooth selected. Hold Ctrl to unsmooth.";
    }

    private static bool IsSmoothable(Polygon p)
    {
        for (int i = 0; i < p.Vertices.Count; i++)
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
