using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Elmanager.Physics;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.LevEditor.Playing
{
    internal class PlayController
    {
        private Engine _engine;
        public bool PlayingStopRequested { get; set; }

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
                    Driver.Condition = DriverCondition.Alive;
                    _timer.Start();
                }
            }
        }

        private readonly InputKeys _keys = new();
        public bool FollowDriver { get; set; }
        public (int, int)? ResetViewPortRequested { get; set; }
        private TaggedBodyPart _currentBodyPart;
        private PlaySettings _settings = new();
        private readonly Stopwatch _timer = new();
        private Task _playTask;
        private VectorMark _playerSelection;
        public readonly PlayerRenderOpts RenderOptsLgr = new(Color.Black, true, true, false);
        public readonly PlayerRenderOpts RenderOptsFrame = new(Color.Black, true, false, false);
        private PlayState _playState;

        public Driver Driver { get; private set; }

        public TaggedBodyPart GetNearestDriverBodyPart(Vector p, double limit)
        {
            return Driver?.BodyParts().OrderBy(bp => bp.Position.Dist(p)).FirstOrDefault(bp =>
                bp.Position.Dist(p) < Math.Max(ElmaRenderer.ObjectRadius, limit));
        }

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

        public TaggedBodyPart CurrentBodyPart
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
            _keys.Gas = Utils.IsKeyDown(Settings.Gas);
            _keys.Brake = Utils.IsKeyDown(Settings.Brake);
            _keys.LeftVolt = Utils.IsKeyDown(Settings.LeftVolt);
            _keys.RightVolt = Utils.IsKeyDown(Settings.RightVolt);
            _keys.AloVolt = Utils.IsKeyDown(Settings.AloVolt);
            _keys.Turn = Utils.IsKeyDown(Settings.Turn);
            if (_keys.IsAnyDown && Settings.FollowDriverOption == FollowDriverOption.WhenPressingKey &&
                CurrentBodyPart is null)
            {
                FollowDriver = true;
            }
        }

        public void UpdateEngine(Level lev)
        {
            _engine?.InitPolysAndObjects(lev.Polygons, lev.Objects);
        }

        public void NotifyDeletedApples(HashSet<int> deletedApples)
        {
            _engine?.TakenApples.RemoveWhere(deletedApples.Contains);
        }

        public event Action PlayingPaused;

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
            renderer.MakeNoneCurrent();
            FollowDriver = Settings.FollowDriverOption == FollowDriverOption.WhenPressingKey;
            sceneSettings.FadedObjectIndices = _engine.TakenApples;
            PlayerSelection = VectorMark.None;
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
                        //var step = ElmaTime.FromMilliSeconds(elapsed - physElapsed);
                        var step = ElmaTime.FromMilliSeconds(1);
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

                    if (Driver.Bugged || Driver.Condition == DriverCondition.Dead)
                    {
                        switch (Settings.DyingBehavior)
                        {
                            case DyingBehavior.PausePlaying when !Driver.Bugged:
                                if (!Paused)
                                {
                                    PlayState = PlayState.Paused;
                                    PlayingPaused();
                                }

                                break;
                            case DyingBehavior.RestartPlaying:
                                Driver = _engine.init_driver();
                                sceneSettings.FadedObjectIndices = _engine.TakenApples;
                                physElapsed = 0.0;
                                _timer.Restart();
                                break;
                            case DyingBehavior.BeInvulnerable when !Driver.Bugged:
                                // Should be unreachable.
                                break;
                            default:
                                PlayingStopRequested = true;
                                break;
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
}