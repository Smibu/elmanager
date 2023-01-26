using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Physics;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.LevelEditor.Playing;

internal class PlayController
{
    private Engine? _engine;
    public bool PlayingStopRequested { get; set; }
    private bool PlayingRestartRequested { get; set; }

    public PlaySettings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            SetInvulnerability();
        }
    }

    private void SetInvulnerability()
    {
        if (_engine != null)
        {
            _engine.Invulnerable = _settings.DyingBehavior == DyingBehavior.BeInvulnerable ||
                                   CurrentBodyPart is not null;
        }
    }

    public bool PlayingOrPaused => Playing || PlayState == PlayState.Paused;

    public bool Playing => PlayState == PlayState.Playing;

    public PlayState PlayState
    {
        get => _playState;
        set
        {
            _playState = value;
            if (value != PlayState.Playing)
            {
                _timer.Stop();
            }
            else
            {
                Debug.Assert(Driver != null, nameof(Driver) + " != null");
                if (Driver.Condition == DriverCondition.Dead && ShouldRestartAfterResuming)
                {
                    PlayingRestartRequested = true;
                }
                else
                {
                    Driver.Condition = DriverCondition.Alive;
                    _timer.Start();
                }
            }
        }
    }

    private readonly InputKeys _keys = new();
    public bool FollowDriver { get; set; }
    public bool ShouldRestartAfterResuming { get; set; }
    public (int, int)? ResetViewPortRequested { get; set; }
    private TaggedBodyPart? _currentBodyPart;
    private PlaySettings _settings = new();
    private readonly Stopwatch _timer = new();
    private Task? _playTask;
    private VectorMark _playerSelection;
    public readonly PlayerRenderOpts RenderOptsLgr = new(Color.Black, true, true, false);
    public readonly PlayerRenderOpts RenderOptsFrame = new(Color.Black, true, false, false);
    private PlayState _playState;
    private Driver? _savedDriverState;
    private SaveLoadRequest _saveLoadRequest = SaveLoadRequest.None;

    private enum SaveLoadRequest
    {
        None,
        Save,
        Load,
    }

    public Driver? Driver { get; private set; }

    public TaggedBodyPart? GetNearestDriverBodyPart(Vector p, double limit) =>
        PlayingOrPaused
            ? Driver?.BodyParts().OrderBy(bp => bp.Position.Dist(p)).FirstOrDefault(bp =>
                bp.Position.Dist(p) < Math.Max(ElmaRenderer.ObjectRadius, limit))
            : null;

    public VectorMark PlayerSelection
    {
        get => _playerSelection;
        set
        {
            _playerSelection = value;
            if (value == VectorMark.Selected)
            {
                FollowDriver = false;
            }
        }
    }

    public TaggedBodyPart? CurrentBodyPart
    {
        get => _currentBodyPart;
        set
        {
            _currentBodyPart = value;
            FollowDriver = false;
            SetInvulnerability();
        }
    }

    public bool Paused => PlayState == PlayState.Paused;

    public void UpdateGravity(AppleType apple)
    {
        if (Driver is null)
        {
            return;
        }

        Driver.GravityDirection = apple switch
        {
            AppleType.Normal => Driver.GravityDirection,
            AppleType.GravityUp => GravityDirection.Up,
            AppleType.GravityDown => GravityDirection.Down,
            AppleType.GravityLeft => GravityDirection.Left,
            AppleType.GravityRight => GravityDirection.Right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void UpdateInputKeys()
    {
        _keys.Gas = KeyboardUtils.IsKeyDown(Settings.Gas);
        _keys.Brake = KeyboardUtils.IsKeyDown(Settings.Brake) || KeyboardUtils.IsKeyDown(Settings.BrakeAlias);
        _keys.LeftVolt = KeyboardUtils.IsKeyDown(Settings.LeftVolt);
        _keys.RightVolt = KeyboardUtils.IsKeyDown(Settings.RightVolt);
        _keys.AloVolt = KeyboardUtils.IsKeyDown(Settings.AloVolt);
        _keys.Turn = KeyboardUtils.IsKeyDown(Settings.Turn);
        if (_keys.IsAnyDown && Settings.FollowDriverOption == FollowDriverOption.WhenPressingKey &&
            CurrentBodyPart is null)
        {
            FollowDriver = true;
        }
        if (KeyboardUtils.IsKeyDown(Settings.Save))
        {
            _saveLoadRequest = SaveLoadRequest.Save;
        }
        else if (KeyboardUtils.IsKeyDown(Settings.Load) && _savedDriverState != null)
        {
            _saveLoadRequest = SaveLoadRequest.Load;
        }
    }

    public void UpdateEngine(Level lev)
    {
        _engine?.InitPolysAndObjects(lev.Polygons, lev.Objects);
        if (Paused)
        {
            ShouldRestartAfterResuming = false;
        }
    }

    public void NotifyDeletedApples(HashSet<int> deletedApples)
    {
        Driver?.TakenApples.RemoveWhere(deletedApples.Contains);
    }

    public event Action? PlayingPaused;

    public async Task BeginLoop(Level lev, SceneSettings sceneSettings, ElmaRenderer renderer,
        ZoomController zoomCtrl, Action render)
    {
        _playTask = BeginLoopImpl(lev, sceneSettings, renderer, zoomCtrl, render);
        await WaitUntilStop();
    }

    private async Task WaitUntilStop()
    {
        if (_playTask is not null)
        {
            await _playTask;
        }
    }

    public async Task StopPlaying()
    {
        PlayingStopRequested = true;
        await WaitUntilStop();
    }

    public async Task NotifyLevelChanged()
    {
        await StopPlaying();
        _savedDriverState = null;
    }

    private async Task BeginLoopImpl(Level lev, SceneSettings sceneSettings, ElmaRenderer renderer,
        ZoomController zoomCtrl, Action render)
    {
        _engine = new Engine(lev.Polygons, lev.Objects);
        SetInvulnerability();
        Driver = _engine.init_driver();
        var maxPhysStep = new ElmaTime(0.0055);
        var rec = new RideRecorder();
        _timer.Reset();
        PlayState = PlayState.Playing;
        PlayingStopRequested = false;
        ShouldRestartAfterResuming = false;
        renderer.MakeNoneCurrent();
        FollowDriver = Settings.FollowDriverOption == FollowDriverOption.WhenPressingKey;
        sceneSettings.FadedObjectIndices = Driver.TakenApples;
        PlayerSelection = VectorMark.None;
        var rnd = new Random();
        var maxFpsVariation = Settings.PhysicsFps / 10;
        await Task.Run(() =>
        {
            renderer.MakeCurrent();
            _timer.Restart();
            var physElapsed = 0.0;
            while (!PlayingStopRequested)
            {
                var elapsed = _timer.ElapsedMilliseconds;
                while (physElapsed < elapsed)
                {
                    var fps = Settings.ConstantFps
                        ? Settings.PhysicsFps
                        : Math.Min(Settings.PhysicsFps + rnd.NextInt64(-maxFpsVariation, maxFpsVariation), 1000);
                    var step = ElmaTime.FromMilliSeconds(1000.0 / fps);
                    if (step > maxPhysStep)
                    {
                        step = maxPhysStep;
                    }

                    try
                    {
                        _engine.next_frame(Driver, _keys, rec, step);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Driver.OutOfBounds = true;
                        break;
                    }

                    physElapsed += step.ToMilliSeconds();
                }

                if (ResetViewPortRequested is var (w, h))
                {
                    renderer.ResetViewport(w, h);
                    ResetViewPortRequested = null;
                }

                if (FollowDriver)
                {
                    zoomCtrl.CenterX = Driver.Body.Location.X;
                    zoomCtrl.CenterY = Driver.Body.Location.Y;
                }

                if (CurrentBodyPart != null)
                {
                    Driver.SetPosition(CurrentBodyPart, Paused);
                }

                if (Driver.Condition == DriverCondition.Finished)
                {
                    PlayingStopRequested = true;
                }

                void RestartPlaying()
                {
                    Driver = _engine.init_driver();
                    sceneSettings.FadedObjectIndices = Driver.TakenApples;
                    physElapsed = 0.0;
                    _timer.Restart();
                }

                if (PlayingRestartRequested)
                {
                    PlayingRestartRequested = false;
                    RestartPlaying();
                }

                if (Driver.Bugged || Driver.Condition == DriverCondition.Dead)
                {
                    if (_savedDriverState is null)
                    {
                        switch (Settings.DyingBehavior)
                        {
                            case DyingBehavior.PausePlaying when !Driver.Bugged:
                                if (!Paused)
                                {
                                    ShouldRestartAfterResuming = true;
                                    PlayState = PlayState.Paused;
                                    PlayingPaused?.Invoke();
                                }

                                break;
                            case DyingBehavior.RestartPlaying:
                                RestartPlaying();
                                break;
                            case DyingBehavior.BeInvulnerable when !Driver.Bugged:
                                // Should be unreachable.
                                break;
                            default:
                                PlayingStopRequested = true;
                                break;
                        }
                    }
                    else
                    {
                        _saveLoadRequest = SaveLoadRequest.Load;
                    }
                }

                try
                {
                    render();
                }
                catch (InvalidOperationException)
                {
                    GL.End();
                }

                switch (_saveLoadRequest)
                {
                    case SaveLoadRequest.Save:
                        _savedDriverState = Driver.Clone();
                        _saveLoadRequest = SaveLoadRequest.None;
                        break;
                    case SaveLoadRequest.Load when _savedDriverState is not null:
                        Driver = _savedDriverState.Clone();
                        sceneSettings.FadedObjectIndices = Driver.TakenApples;
                        _saveLoadRequest = SaveLoadRequest.None;
                        break;
                }
            }

            renderer.MakeNoneCurrent();
        });
        renderer.MakeCurrent();
        sceneSettings.FadedObjectIndices.Clear();
        PlayState = PlayState.Stopped;
        _engine = null;
        PlayingStopRequested = false;
    }
}