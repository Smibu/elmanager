using System;
using System.Collections.Generic;
using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class FrameTool : ToolBase, IEditorTool
{
    private FrameState? _frame;
    private double _frameRadius = 0.2;
    private FrameType _frameType = FrameType.Normal;

    private enum FrameType
    {
        Normal,
        Inward,
        Outward
    }

    public FrameTool(ILevelEditor editor)
        : base(editor)
    {
    }

    private bool Framing => _frame is { };

    public void Activate()
    {
        _frameRadius = LevEditor.Settings.FrameRadius;
    }

    public string GetHelp() =>
        Framing
            ? $"LMouse: create frame; +/-: adjust width; Space: change type ({_frameType}); RMouse: cancel."
            : "LMouse: select polygon to frame.";

    public void ExtraRendering()
    {
        if (_frame is null) return;
        Renderer.DrawPolygon(_frame.Frames[0], Color.Blue);
        Renderer.DrawPolygon(_frame.Frames[1], Color.Blue);
    }

    public TransientElements GetTransientElements(bool hasFocus) =>
        _frame is not null ? TransientElements.FromPolygons(_frame.Frames) : TransientElements.Empty;

    public LevVisualChange InActivate()
    {
        LevEditor.Settings.FrameRadius = _frameRadius;
        if (Framing)
        {
            CancelFraming();
            ResetHighlight();
            return LevVisualChange.Ground | LevVisualChange.Grass;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        if (_frame is null) return LevVisualChange.Nothing;
        switch (key.KeyCode)
        {
            case EditorKeyUtils.Increase:
                _frameRadius += 0.05;
                break;
            case EditorKeyUtils.Decrease:
                if (_frameRadius > 0.05)
                {
                    _frameRadius -= 0.05;
                }

                break;
            case EditorKey.Space:
                _frameType = _frameType switch
                {
                    FrameType.Normal => FrameType.Inward,
                    FrameType.Inward => FrameType.Outward,
                    FrameType.Outward => FrameType.Normal,
                    _ => throw new ArgumentOutOfRangeException()
                };

                break;
        }

        UpdateFrame(_frame.Polygon);
        return LevVisualChange.Ground | LevVisualChange.Grass;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:
                if (GetNearestVertexInfo(CurrentPos) is { } v && !Framing)
                {
                    var poly = v.Polygon;
                    if (!poly.IsCounterClockwise)
                    {
                        poly.ChangeOrientation();
                    }

                    poly.Mark = PolygonMark.None;
                    Lev.Polygons.Remove(poly);
                    UpdateFrame(poly);
                    return LevVisualChange.Ground | LevVisualChange.Grass;
                }
                else if (_frame is { })
                {
                    Lev.Polygons.AddRange(_frame.Frames);
                    _frame = null;
                    LevEditor.SetModified(LevModification.Ground | LevModification.Grass);
                }

                break;
            case EditorMouseButton.Right:
                if (Framing)
                    return CancelFraming();
                break;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (!Framing)
        {
            HighlightTarget? highlight = null;
            if (GetNearestVertexInfo(p) is { } v)
            {
                ChangeCursorToHand();
                if (v.Polygon.Mark != PolygonMark.Selected)
                    highlight = new HighlightTarget.PolygonTarget(v.Polygon);
            }
            else
                ChangeToDefaultCursorIfHand();

            LevEditor.CurrentHighlight = highlight;
        }
        else
            ChangeToDefaultCursorIfHand();

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        if (!Framing)
        {
            ResetHighlight();
        }
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
    }

    private LevVisualChange CancelFraming()
    {
        if (_frame is null)
        {
            return LevVisualChange.Nothing;
        }

        var poly = _frame.Polygon;
        Lev.Polygons.Add(poly);
        _frame = null;
        return LevVisualChange.Ground | LevVisualChange.Grass;
    }

    private void UpdateFrame(Polygon p)
    {
        _frame = new FrameState(p, MakeClosedPipe(p, _frameRadius));
    }

    private List<Polygon> MakeClosedPipe(Polygon pipeLine, double radius)
    {
        var p = new List<Polygon> { new(), new() };
        if (pipeLine.Vertices.Count < 2)
            return p;
        for (int i = 0; i < pipeLine.Vertices.Count; i++)
        {
            switch (_frameType)
            {
                case FrameType.Normal:
                    p[0].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                    p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                    break;
                case FrameType.Inward:
                    p[0].Add(pipeLine[i]);
                    p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], radius));
                    break;
                case FrameType.Outward:
                    p[0].Add(pipeLine[i]);
                    p[1].Add(GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -radius));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return p;
    }

    public override bool Busy => Framing;
}
