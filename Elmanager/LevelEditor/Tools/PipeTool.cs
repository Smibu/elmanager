using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Utilities;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class PipeTool : ToolBase, IEditorTool
{
    private const double AppleDistanceStep = 0.25;
    private PipeMode _pipeMode = PipeMode.NoApples;
    private double _pipeRadius = 1.0;
    private const double PipeStep = 0.02;
    private PipeSpec? _pipeSpec;
    private int _appleAmount = 20;
    private double _appleDistance = 3.0;

    internal PipeTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool CreatingPipe => _pipeSpec is { };

    public void Activate()
    {
        _pipeRadius = Global.AppSettings.LevelEditor.PipeRadius;
        UpdateHelp();
    }

    public void ExtraRendering()
    {
        if (_pipeSpec is { })
        {
            Renderer.DrawLineStrip(_pipeSpec.Pipeline, Color.Blue);
            if (Global.AppSettings.LevelEditor.RenderingSettings.ShowGroundEdges)
                Renderer.DrawPolygon(_pipeSpec.Pipe, Global.AppSettings.LevelEditor.RenderingSettings.GroundEdgeColor);
            foreach (LevObject x in _pipeSpec.Apples)
            {
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectFrames)
                    Renderer.DrawCircle(x.Position, ElmaRenderer.ObjectRadius,
                        Global.AppSettings.LevelEditor.RenderingSettings.AppleColor);
                if (Renderer.LgrGraphicsLoaded && Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects)
                    Renderer.DrawApple(x.Position);
            }
        }
    }

    public List<Polygon> GetExtraPolygons()
    {
        var polys = new List<Polygon>();
        if (_pipeSpec is { })
        {
            polys.Add(_pipeSpec.Pipe);
        }
        return polys;
    }

    public void InActivate()
    {
        _pipeSpec = null;
        Global.AppSettings.LevelEditor.PipeRadius = _pipeRadius;
    }

    public void KeyDown(KeyEventArgs key)
    {
        double radiusStep = PipeStep;
        switch (key.KeyCode)
        {
            case KeyUtils.IncreaseBig:
                radiusStep *= 10;
                goto case KeyUtils.Increase;
            case KeyUtils.Increase:
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    switch (_pipeMode)
                    {
                        case PipeMode.NoApples:
                            break;

                        case PipeMode.ApplesDistance:
                            _appleDistance += AppleDistanceStep;
                            break;
                        case PipeMode.ApplesAmount:
                            _appleAmount++;
                            break;
                    }
                }
                else
                    _pipeRadius += radiusStep;

                break;
            case KeyUtils.DecreaseBig:
                radiusStep *= 10;
                goto case KeyUtils.Decrease;
            case KeyUtils.Decrease:
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    switch (_pipeMode)
                    {
                        case PipeMode.NoApples:
                            break;

                        case PipeMode.ApplesDistance:
                            if (_appleDistance > AppleDistanceStep) _appleDistance -= AppleDistanceStep;
                            break;
                        case PipeMode.ApplesAmount:
                            if (_appleAmount > 1) _appleAmount--;
                            break;
                    }
                }
                else if (_pipeRadius > radiusStep) _pipeRadius -= radiusStep;

                break;
            case Keys.Space:
                _pipeMode = _pipeMode switch
                {
                    PipeMode.NoApples => PipeMode.ApplesDistance,
                    PipeMode.ApplesDistance => PipeMode.ApplesAmount,
                    PipeMode.ApplesAmount => PipeMode.NoApples,
                    _ => _pipeMode
                };

                break;
        }

        UpdatePipeSpec();

        UpdateHelp();
    }

    private void UpdatePipeSpec()
    {
        if (_pipeSpec is { })
            _pipeSpec = new PipeSpec(_pipeSpec.Pipeline, _pipeRadius, _pipeMode, _appleDistance, _appleAmount);
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                if (_pipeSpec is { })
                    _pipeSpec.Pipeline.Add(CurrentPos);
                else
                {
                    var pipeline = new Polygon();
                    pipeline.Add(CurrentPos);
                    pipeline.Add(CurrentPos);
                    _pipeSpec = new PipeSpec(pipeline, _pipeRadius, _pipeMode, _appleDistance, _appleAmount);
                }

                break;
            case MouseButtons.Right:
                if (_pipeSpec is { })
                {
                    _pipeSpec.Pipeline.RemoveLastVertex();
                    UpdatePipeSpec();
                    if (_pipeSpec.Pipeline.Vertices.Count > 1)
                    {
                        Lev.Polygons.Add(_pipeSpec.Pipe);
                        Lev.Objects.AddRange(_pipeSpec.Apples);
                        LevEditor.SetModified(LevModification.Ground);
                    }

                    _pipeSpec = null;
                }

                break;
        }

        UpdatePipeSpec();

        UpdateHelp();
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        if (_pipeSpec is { })
        {
            _pipeSpec.Pipeline.Vertices[^1] = CurrentPos;
            UpdatePipeSpec();
        }
    }

    public void MouseOutOfEditor()
    {
    }

    public void MouseUp()
    {
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = "LMouse: create pipe; Space: change mode (";
        switch (_pipeMode)
        {
            case PipeMode.NoApples:
                LevEditor.InfoLabel.Text += "no apples)";
                break;
            case PipeMode.ApplesDistance:
                LevEditor.InfoLabel.Text += $"variable distance); Ctrl + +/-: adjust distance ({_appleDistance:F2})";
                break;
            case PipeMode.ApplesAmount:
                LevEditor.InfoLabel.Text += $"variable apples); Ctrl + +/-: adjust amount ({_appleAmount})";
                break;
        }

        LevEditor.InfoLabel.Text += $"; +/- or Pg Up/Down: adjust pipe radius ({_pipeRadius:F2})";

    }

    public override bool Busy => CreatingPipe;
}