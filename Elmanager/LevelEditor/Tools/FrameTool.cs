using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Utilities;

namespace Elmanager.LevelEditor.Tools;

internal class FrameTool : ToolBase, IEditorTool
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

    internal FrameTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool Framing => _frame is { };

    public void Activate()
    {
        _frameRadius = Global.AppSettings.LevelEditor.FrameRadius;
        UpdateHelp();
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = Framing
            ? $"LMouse: create frame; +/-: adjust width; Space: change type ({_frameType}); RMouse: cancel."
            : "LMouse: select polygon to frame.";
    }

    public void ExtraRendering()
    {
        if (_frame is null) return;
        Renderer.DrawPolygon(_frame.Frames[0], Color.Blue);
        Renderer.DrawPolygon(_frame.Frames[1], Color.Blue);
    }

    public TransientElements GetTransientElements() =>
        _frame is not null ? TransientElements.FromPolygons(_frame.Frames) : TransientElements.Empty;

    public void InActivate()
    {
        Global.AppSettings.LevelEditor.FrameRadius = _frameRadius;
        if (!Framing) return;
        CancelFraming();
        ResetHighlight();
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (_frame is null) return;
        switch (key.KeyCode)
        {
            case KeyUtils.Increase:
                _frameRadius += 0.05;
                break;
            case KeyUtils.Decrease:
                if (_frameRadius > 0.05)
                {
                    _frameRadius -= 0.05;
                }

                break;
            case Keys.Space:
                _frameType = _frameType switch
                {
                    FrameType.Normal => FrameType.Inward,
                    FrameType.Inward => FrameType.Outward,
                    FrameType.Outward => FrameType.Normal,
                    _ => throw new ArgumentOutOfRangeException()
                };

                UpdateHelp();
                break;
        }

        UpdateFrame(_frame.Polygon);
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
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
                }
                else if (_frame is { })
                {
                    Lev.Polygons.AddRange(_frame.Frames);
                    _frame = null;
                    LevEditor.SetModified(LevModification.Ground);
                }

                break;
            case MouseButtons.Right:
                if (Framing)
                    CancelFraming();
                break;
        }

        UpdateHelp();
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (!Framing)
        {
            ResetHighlight();
            if (GetNearestVertexInfo(p) is { } v)
            {
                ChangeCursorToHand();
                if (v.Polygon.Mark != PolygonMark.Selected)
                    v.Polygon.Mark = PolygonMark.Highlight;
            }
            else
                ChangeToDefaultCursorIfHand();
        }
        else
            ChangeToDefaultCursorIfHand();
    }

    public void MouseOutOfEditor()
    {
        if (!Framing)
        {
            ResetHighlight();
        }
    }

    public void MouseUp()
    {
    }

    private void CancelFraming()
    {
        if (_frame is null)
        {
            return;
        }
        Lev.Polygons.Add(_frame.Polygon);
        _frame = null;
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

        p[0].UpdateDecomposition();
        p[1].UpdateDecomposition();
        return p;
    }

    public override bool Busy => Framing;
}