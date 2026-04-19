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
    }

    public void ExtraRendering()
    {
        if (_pipeSpec is { })
        {
            Renderer.DrawLineStrip(_pipeSpec.Pipeline, Color.Blue);
        }
    }

    public TransientElements GetTransientElements(bool hasFocus)
    {
        if (_pipeSpec is { })
        {
            return new TransientElements([_pipeSpec.Pipe], _pipeSpec.Apples, []);
        }

        return TransientElements.Empty;
    }

    public LevVisualChange InActivate()
    {
        _pipeSpec = null;
        Global.AppSettings.LevelEditor.PipeRadius = _pipeRadius;
        return LevVisualChange.Ground | LevVisualChange.Apples;
    }

    public LevVisualChange KeyDown(KeyEventArgs key)
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

        return LevVisualChange.Ground | LevVisualChange.Apples;
    }

    private void UpdatePipeSpec()
    {
        if (_pipeSpec is { })
            _pipeSpec = new PipeSpec(_pipeSpec.Pipeline, _pipeRadius, _pipeMode, _appleDistance, _appleAmount);
    }

    public LevVisualChange MouseDown(MouseEventArgs mouseData)
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
                        _pipeSpec = null;
                        LevEditor.SetModified(LevModification.Ground | LevModification.Apples);
                    }
                    else
                    {
                        _pipeSpec = null;
                        return LevVisualChange.Ground | LevVisualChange.Apples;
                    }
                }

                break;
        }

        UpdatePipeSpec();

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        if (_pipeSpec is { })
        {
            _pipeSpec.Pipeline.Vertices[^1] = CurrentPos;
            UpdatePipeSpec();
            return LevVisualChange.Ground | LevVisualChange.Apples;
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor() => LevVisualChange.Nothing;

    public void MouseUp()
    {
    }

    public string GetHelp()
    {
        var text = "LMouse: create pipe; Space: change mode (";
        switch (_pipeMode)
        {
            case PipeMode.NoApples:
                text += "no apples)";
                break;
            case PipeMode.ApplesDistance:
                text += $"variable distance); Ctrl + +/-: adjust distance ({_appleDistance:F2})";
                break;
            case PipeMode.ApplesAmount:
                text += $"variable apples); Ctrl + +/-: adjust amount ({_appleAmount})";
                break;
        }

        text += $"; +/- or Pg Up/Down: adjust pipe radius ({_pipeRadius:F2})";
        return text;
    }

    public override bool Busy => CreatingPipe;
}
