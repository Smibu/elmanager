using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Utilities;

namespace Elmanager.LevelEditor.Tools;

internal class DrawTool : ToolBase, IEditorTool
{
    private const double ThresholdAdjustStep = 0.125;
    private Polygon? _currentPolygon;
    private Vector _lastMousePosition;
    private double _mouseTrip;

    internal DrawTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool Drawing => _currentPolygon is { };

    public void Activate()
    {
    }

    public string GetHelp() =>
        $"Hold LMouse to create vertex; +/-: adjust threshold ({Global.AppSettings.LevelEditor.DrawStep:F2})";

    public void ExtraRendering()
    {
        if (_currentPolygon is { })
            Renderer.DrawLine(_currentPolygon.GetLastVertex(), _currentPolygon.Vertices[0], Color.Red);
    }

    public LevVisualChange InActivate()
    {
        _currentPolygon = null;
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(KeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case KeyUtils.Increase:
                Global.AppSettings.LevelEditor.DrawStep += ThresholdAdjustStep;
                break;
            case KeyUtils.Decrease:
                if (Global.AppSettings.LevelEditor.DrawStep > ThresholdAdjustStep)
                {
                    Global.AppSettings.LevelEditor.DrawStep -= ThresholdAdjustStep;
                }

                break;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                _currentPolygon = new Polygon();
                _currentPolygon.Add(CurrentPos);
                _mouseTrip = 0;
                _lastMousePosition = CurrentPos;
                break;
            case MouseButtons.Right:
                break;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_currentPolygon is null) return LevVisualChange.Nothing;
        _mouseTrip += (p - _lastMousePosition).Length;
        _lastMousePosition = p;
        var scaledStep = Global.AppSettings.LevelEditor.DrawStep * ZoomCtrl.ZoomLevel * 0.1;
        if (_mouseTrip > scaledStep)
        {
            while (!(_mouseTrip < scaledStep))
                _mouseTrip -= scaledStep;
            _currentPolygon.Add(p);
            if (_currentPolygon.Vertices.Count == 3)
                Lev.Polygons.Add(_currentPolygon);
            return LevVisualChange.Ground;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
        if (_currentPolygon is null) return;
        if (_currentPolygon.Vertices.Count > 2)
        {
            LevEditor.SetModified(LevModification.Ground);
        }

        _currentPolygon = null;
    }

    public override bool Busy => Drawing;
}