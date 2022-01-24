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

    private int _appleAmount = 20;
    private double _appleDistance = 3.0;
    private List<LevObject> _apples;
    private Polygon _pipe;
    private PipeMode _pipeMode = PipeMode.NoApples;
    private double _pipeRadius = 1.0;
    private const double PipeStep = 0.02;
    private Polygon _pipeline;

    public enum PipeMode
    {
        NoApples = 0,
        ApplesAmount = 1,
        ApplesDistance = 2
    }

    internal PipeTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool CreatingPipe => _pipeline != null;

    public void Activate()
    {
        _pipeRadius = Global.AppSettings.LevelEditor.PipeRadius;
        UpdateHelp();
    }

    public void ExtraRendering()
    {
        if (CreatingPipe)
        {
            Renderer.DrawLineStrip(_pipeline, Color.Blue);
            if (Global.AppSettings.LevelEditor.RenderingSettings.ShowGroundEdges)
                Renderer.DrawPolygon(_pipe, Global.AppSettings.LevelEditor.RenderingSettings.GroundEdgeColor);
            foreach (LevObject x in _apples)
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
        if (CreatingPipe)
        {
            polys.Add(_pipe);
        }
        return polys;
    }

    public void InActivate()
    {
        _pipeline = null;
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
                            if (_appleDistance > AppleDistanceStep)
                                _appleDistance -= AppleDistanceStep;
                            break;
                        case PipeMode.ApplesAmount:
                            if (_appleAmount > 1)
                                _appleAmount--;
                            break;
                    }
                }
                else if (_pipeRadius > radiusStep)
                    _pipeRadius -= radiusStep;

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

        if (CreatingPipe)
        {
            UpdatePipe(_pipeline);
        }

        UpdateHelp();
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                if (CreatingPipe)
                    _pipeline.Add(CurrentPos);
                else
                {
                    _pipeline = new Polygon();
                    _apples = new List<LevObject>();
                    _pipeline.Add(CurrentPos);
                    _pipeline.Add(CurrentPos);
                }

                break;
            case MouseButtons.Right:
                if (CreatingPipe)
                {
                    _pipeline.RemoveLastVertex();
                    UpdatePipe(_pipeline);
                    if (_pipeline.Count > 1)
                    {
                        Lev.Polygons.Add(_pipe);
                        Lev.Objects.AddRange(_apples);
                        LevEditor.SetModified(LevModification.Ground);
                    }

                    _pipeline = null;
                }

                break;
        }

        if (CreatingPipe)
        {
            UpdatePipe(_pipeline);
        }

        UpdateHelp();
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        if (CreatingPipe)
        {
            _pipeline.Vertices[_pipeline.Vertices.Count - 1] = CurrentPos;
            UpdatePipe(_pipeline);
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
        LevEditor.InfoLabel.Text = "Space: change mode - ";
        const string help = "(adjust with +/- or Page Up/Down)";
        switch (_pipeMode)
        {
            case PipeMode.NoApples:
                LevEditor.InfoLabel.Text +=
                    string.Format("Mode: No apples - Pipe radius: {0:F2} {1}", _pipeRadius, help);
                break;
            case PipeMode.ApplesDistance:
                LevEditor.InfoLabel.Text +=
                    string.Format(
                        "Mode: Apples (distance: {0:F2} (adjust with Ctrl + +/-)) - Pipe radius: {1:F2} {2}",
                        _appleDistance, _pipeRadius, help);
                break;
            case PipeMode.ApplesAmount:
                LevEditor.InfoLabel.Text += string.Format("Mode: {0} apples - Pipe radius: {1:F2} {2}",
                    _appleAmount, _pipeRadius, help);
                break;
        }
    }

    private List<LevObject> CalculateApples(double distance)
    {
        List<LevObject> apples = new List<LevObject>();
        double currentDistanceToApple = distance;
        for (int i = 0; i <= _pipeline.Count - 2; i++)
        {
            Vector z = _pipeline[i + 1] - _pipeline[i];
            Vector zUnit = z.Unit();
            double currVectorLength = z.Length;
            if (currentDistanceToApple < currVectorLength)
            {
                double currVectorTrip = currentDistanceToApple;
                while (!(currVectorTrip > currVectorLength))
                {
                    apples.Add(new LevObject(_pipeline[i] + zUnit * currVectorTrip, ObjectType.Apple,
                        AppleType.Normal));
                    currVectorTrip += distance;
                }

                currentDistanceToApple = currVectorTrip - currVectorLength;
            }
            else
                currentDistanceToApple -= currVectorLength;
        }

        return apples;
    }

    private void UpdatePipe(Polygon pipeLine)
    {
        Polygon p = new Polygon();
        if (pipeLine.Count < 2)
            return;
        double angle = (pipeLine[1] - pipeLine[0]).Angle;
        p.Add(pipeLine[0] + new Vector(angle + 90) * _pipeRadius);
        p.Add(pipeLine[0] - new Vector(angle + 90) * _pipeRadius);
        for (int i = 1; i <= pipeLine.Count - 2; i++)
        {
            angle = (pipeLine[i + 1] - pipeLine[i]).Angle;
            Vector point = GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -_pipeRadius);
            p.Add(point);
        }

        p.Add(pipeLine.GetLastVertex() - new Vector(angle + 90) * _pipeRadius);
        p.Add(pipeLine.GetLastVertex() + new Vector(angle + 90) * _pipeRadius);
        for (int i = pipeLine.Count - 2; i >= 1; i--)
        {
            Vector point = GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], _pipeRadius);
            p.Add(point);
        }

        _pipe = p;
        _pipe.UpdateDecomposition();
        switch (_pipeMode)
        {
            case PipeMode.ApplesDistance:
                _apples = CalculateApples(_appleDistance);
                break;
            case PipeMode.ApplesAmount:
                double pipelineLength = 0.0;
                for (int i = 0; i <= pipeLine.Count - 2; i++)
                    pipelineLength += (pipeLine[i + 1] - pipeLine[i]).Length;
                pipelineLength += 0.1;
                _apples = CalculateApples(pipelineLength / (_appleAmount + 1));
                break;
            case PipeMode.NoApples:
                _apples = new List<LevObject>();
                break;
        }
    }

    public override bool Busy => CreatingPipe;
}