using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rec;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.Graphics.OpenGL;
using OpenTK.WinForms;
using Timer = System.Timers.Timer;

namespace Elmanager.ReplayViewer
{
    internal class ReplayController : IDisposable
    {
        public void Dispose()
        {
            Renderer.Dispose();
        }

        private readonly uint[] _colourValues =
        {
            0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFF00, 0xFFFF00FF,
            0xFF00FFFF, 0xFF000000, 0xFF800000, 0xFF008000, 0xFF000080,
            0xFF808000, 0xFF800080, 0xFF008080, 0xFF808080, 0xFFC00000,
            0xFF00C000, 0xFF0000C0, 0xFFC0C000, 0xFFC000C0, 0xFF00C0C0,
            0xFFC0C0C0, 0xFF400000, 0xFF004000, 0xFF000040, 0xFF404000,
            0xFF400040, 0xFF004040, 0xFF404040, 0xFF200000, 0xFF002000,
            0xFF000020, 0xFF202000, 0xFF200020, 0xFF002020, 0xFF202020,
            0xFF600000, 0xFF006000, 0xFF000060, 0xFF606000, 0xFF600060,
            0xFF006060, 0xFF606060, 0xFFA00000, 0xFF00A000, 0xFF0000A0,
            0xFFA0A000, 0xFFA000A0, 0xFF00A0A0, 0xFFA0A0A0, 0xFFE00000,
            0xFF00E000, 0xFF0000E0, 0xFFE0E000, 0xFFE000E0, 0xFF00E0E0,
            0xFFE0E0E0
        };

        private List<PlayerEvent<LogicalEventType>> _currentEvents;
        private List<int> _activePlayerIndices = new();
        private bool _playing;
        private bool _requestPlayCancel;
        private List<int> _visiblePlayerIndices = new();
        internal Level Lev;
        private double _playBackSpeed = 1.0;
        private readonly Stopwatch _playTimer = new();
        private bool _loopPlaying;
        private bool _wrongLevVersion;
        private double _frameStep = 0.02;
        private double _initialTime;
        private bool _showDriverPath;
        private bool _followDriver;
        private bool _drawInActiveAsTransparent;
        private Color _activePlayerColor;
        private Color _inActivePlayerColor;
        private bool _drawOnlyPlayerFrames;
        private bool _hideStartObject;
        private bool _multiSpy;
        private List<PlayerEvent<LogicalEventType>> _currentPlayerAppleEvents;
        private int _selectedReplayIndex;
        private double _fixx;
        private double _fixy;
        private readonly Timer _timer = new(25); // Don't update unnecessarily often to avoid overhead
        private Task _playTask;

        public event ElapsedEventHandler PlayingElapsed
        {
            add => _timer.Elapsed += value;
            remove => _timer.Elapsed -= value;
        }

        public ReplayController(GLControl viewerBox, RenderingSettings replayViewerRenderingSettings)
        {
            Renderer = new ElmaRenderer(viewerBox, replayViewerRenderingSettings);
        }

        private void DrawPlayers()
        {
            foreach (var x in _visiblePlayerIndices)
            {
                var isSelected = _activePlayerIndices.Contains(x);
                var opts = new PlayerRenderOpts {
                    Color = isSelected ? _activePlayerColor : _inActivePlayerColor,
                    IsActive = isSelected,
                    UseGraphics = !_drawOnlyPlayerFrames,
                    UseTransparency = !isSelected && _drawInActiveAsTransparent
                };
                Renderer.DrawPlayer(PlayListReplays[x].Player.GetInterpolatedState(CurrentTime),
                    opts,
                    SceneSettings);
            }
        }

        internal void UpdateReplaySettings()
        {
            _showDriverPath = Global.AppSettings.ReplayViewer.ShowDriverPath;
            _followDriver = Global.AppSettings.ReplayViewer.FollowDriver;
            SceneSettings.PicturesInBackground = Global.AppSettings.ReplayViewer.PicturesInBackGround;
            _drawInActiveAsTransparent = Global.AppSettings.ReplayViewer.DrawTransparentInactive;
            _frameStep = Global.AppSettings.ReplayViewer.FrameStep;
            _loopPlaying = Global.AppSettings.ReplayViewer.LoopPlaying;
            _activePlayerColor = Global.AppSettings.ReplayViewer.ActivePlayerColor;
            _inActivePlayerColor = Global.AppSettings.ReplayViewer.InactivePlayerColor;
            _drawOnlyPlayerFrames = Global.AppSettings.ReplayViewer.DrawOnlyPlayerFrames;
            _hideStartObject = Global.AppSettings.ReplayViewer.HideStartObject;
            _multiSpy = Global.AppSettings.ReplayViewer.MultiSpy;
        }

        internal void NextFrame()
        {
            if (!_playing)
            {
                CurrentTime += _frameStep;
                if (CurrentTime > MaxTime)
                    CurrentTime = 0;
                UpdateCameraAndDrawScene();
            }
        }

        public void UpdateCameraAndDrawSceneIfNotPlaying()
        {
            if (!Playing)
            {
                UpdateCameraAndDrawScene();
            }
        }
        
        private void UpdateCameraAndDrawScene()
        {
            UpdateCamera(_followDriver);
            RedrawScene();
        }

        internal void PreviousFrame()
        {
            if (!_playing)
            {
                CurrentTime -= _frameStep;
                if (CurrentTime < 0)
                    CurrentTime = MaxTime;
                UpdateCameraAndDrawScene();
            }
        }

        internal void SetPlayBackSpeed(double newSpeed)
        {
            _playBackSpeed = newSpeed;
            if (_playing)
            {
                _initialTime = CurrentTime;
                _playTimer.Restart();
            }
        }

        internal async Task StopPlaying()
        {
            await PausePlaying();
            CurrentTime = 0;
            UpdateCameraAndDrawScene();
        }

        public async Task PausePlaying()
        {
            _requestPlayCancel = true;
            if (_playTask != null)
            {
                await _playTask;
            }
        }

        internal async void TogglePlay()
        {
            if (_playing)
            {
                _requestPlayCancel = true;
                return;
            }
            _playTask = StartPlaying();
            await _playTask;
        }

        private async Task StartPlaying()
        {
            if (CurrentTime > MaxTime)
            {
                CurrentTime = 0;
                UpdateCameraAndDrawScene();
            }

            _playing = true;
            double elapsedTime = 0;
            _initialTime = CurrentTime;
            _playTimer.Restart();
            _timer.Start();
            Renderer.MakeNoneCurrent();
            _requestPlayCancel = false;
            await Task.Run(() =>
            {
                Renderer.MakeCurrent();
                while (!_requestPlayCancel)
                {
                    CurrentTime = _initialTime + elapsedTime;
                    UpdateCameraAndDrawScene();
                    elapsedTime = _playTimer.ElapsedMilliseconds / 1000.0 * _playBackSpeed;
                    if (CurrentTime > MaxTime)
                    {
                        if (_loopPlaying)
                        {
                            CurrentTime = 0;
                            _initialTime = 0;
                            elapsedTime = 0;
                            _playTimer.Restart();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Renderer.MakeNoneCurrent();
            });
            Renderer.MakeCurrent();
            _playing = false;
            _playTimer.Stop();
            _timer.Stop();
        }

        internal double CurrentTime { get; private set; }

        internal double MaxTime { get; private set; }

        internal List<PlayListObject> PlayListReplays { get; private set; }

        internal ZoomController ZoomCtrl { get; private set; }

        internal ElmaRenderer Renderer { get; }

        public SceneSettings SceneSettings { get; } = new();

        internal Vector GetBikeCoordinates()
        {
            var f = FirstActivePlayer;
            if (f == null)
            {
                return new Vector();
            }

            var r = f.GetInterpolatedState(CurrentTime);
            return new Vector(r.GlobalBodyX, r.GlobalBodyY);
        }

        private Player FirstActivePlayer => GetActivePlayers().Select(p => p.Player).FirstOrDefault();

        internal double GetSpeed()
        {
            var first = FirstActivePlayer;
            if (first == null)
            {
                return 0.0;
            }

            var i1 = first.GetInterpolatedState(CurrentTime);
            var i2 = first.GetInterpolatedState(Math.Max(CurrentTime - 1 / 30.0, 0));
            var xdiff = i1.GlobalBodyX - i2.GlobalBodyX;
            var ydiff = i1.GlobalBodyY - i2.GlobalBodyY;
            return Math.Sqrt(xdiff * xdiff + ydiff * ydiff) * Player.SpeedFactor;
        }

        private void UpdateCamera(bool zoomToDriver)
        {
            SceneSettings.HiddenObjectIndices.Clear();
            if (!_wrongLevVersion && _activePlayerIndices.Count > 0)
            {
                foreach (var ev in _currentPlayerAppleEvents.TakeWhile(ev => ev.Time < CurrentTime))
                {
                    SceneSettings.HiddenObjectIndices.Add(ev.Info);
                }
            }

            if (_hideStartObject)
            {
                SceneSettings.HiddenObjectIndices.Add(Lev.Objects.FindIndex(o => o.Type == ObjectType.Start));
            }

            _fixx = 0.0;
            _fixy = 0.0;
            var firstActive = FirstActivePlayer;
            var firstVisible = FirstVisiblePlayer;
            if (firstActive != null && zoomToDriver)
            {
                var r = firstActive.GetInterpolatedState(CurrentTime);
                ZoomCtrl.CenterX = r.GlobalBodyX;
                ZoomCtrl.CenterY = r.GlobalBodyY;
                var jf = ZoomCtrl.Cam.FixJitter();
                _fixx = jf.X;
                _fixy = jf.Y;
            }
            else if (_multiSpy && firstVisible != null)
            {
                var player = firstVisible.Player;
                var fr = player.GetInterpolatedState(CurrentTime);
                var xmin = fr.GlobalBodyX;
                var xmax = xmin;
                var ymin = fr.GlobalBodyY;
                var ymax = ymin;
                foreach (var p in GetVisiblePlayers())
                {
                    var r = p.Player.GetInterpolatedState(CurrentTime);
                    var gx = r.GlobalBodyX;
                    xmin = Math.Min(gx, xmin);
                    xmax = Math.Max(gx, xmax);
                    var gy = r.GlobalBodyY;
                    ymin = Math.Min(gy, ymin);
                    ymax = Math.Max(gy, ymax);
                }

                ZoomCtrl.CenterX = (xmin + xmax) / 2;
                ZoomCtrl.CenterY = (ymin + ymax) / 2;
                ZoomCtrl.ZoomLevel = Math.Max((xmax + 5 - ZoomCtrl.CenterX) / ZoomCtrl.Cam.AspectRatio,
                    ymax + 5 - ZoomCtrl.CenterY);
                ZoomCtrl.ZoomLevel = Math.Max(ZoomCtrl.ZoomLevel, 5);
            }
        }

        private PlayListObject FirstVisiblePlayer => GetVisiblePlayers().FirstOrDefault();

        public bool Playing => _playing;

        private void FocusIndicesChanged()
        {
            if (!_wrongLevVersion && _activePlayerIndices.Count > 0)
            {
                _currentPlayerAppleEvents = FirstActivePlayer.GetEvents(LogicalEventType.AppleTake);
            }
        }

        public void SetReplays(List<Replay> replays)
        {
            PlayListReplays = new List<PlayListObject>();
            foreach (var replay in replays)
            {
                var playerNum = 1;
                foreach (var player in replay.Players)
                {
                    PlayListReplays.Add(new PlayListObject(Path.GetFileNameWithoutExtension(replay.FileName),
                        playerNum,
                        player));
                    playerNum++;
                }
            }

            var lev = replays[0].GetLevel();
            ZoomCtrl = new ZoomController(new ElmaCamera(),
                lev,
                Global.AppSettings.ReplayViewer.RenderingSettings,
                RedrawSceneIfNotPlaying) {ZoomLevel = 5.0};
            MaxTime = PlayListReplays.Max(p => p.Player.FrameCount) / 30.0;
            CurrentTime = 0.0;
            Lev = lev;
            Renderer.InitializeLevel(lev);
            _activePlayerIndices = new List<int>();
            _visiblePlayerIndices = new List<int>();
            _wrongLevVersion = replays[0].WrongLevelVersion;
            for (var i = 0; i < PlayListReplays.Count; i++)
                PlayListReplays[i].DrivingLineColor = Color.FromArgb((int) _colourValues[i % _colourValues.Length]);
        }

        public void SetInitialView()
        {
            if (Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault)
                ZoomCtrl.ZoomFill();
            else
            {
                ZoomCtrl.ZoomLevel = Global.AppSettings.ReplayViewer.ZoomLevel;
                UpdateCameraAndDrawScene();
            }
        }

        public void SelectedReplayChanged(int index, IEnumerable<int> active, IEnumerable<int> selected)
        {
            _selectedReplayIndex = index;
            _activePlayerIndices = active.ToList();
            _visiblePlayerIndices = selected.ToList();
            FocusIndicesChanged();
        }

        public void SelectedEventChanged(int i)
        {
            if (!_playing)
            {
                if (i == _currentEvents.Count)
                    UpdateCurrFrameFromTime(PlayListReplays[_selectedReplayIndex].Player.Time);
                else if (i >= 0)
                    UpdateCurrFrameFromTime(_currentEvents[i].Time);
            }
        }

        public void UpdateCurrFrameFromTime(double newTime)
        {
            CurrentTime = Math.Max(newTime, 0);
            UpdateCameraAndDrawSceneIfNotPlaying();
        }

        public void RedrawSceneIfNotPlaying()
        {
            if (!_playing)
            {
                RedrawScene();
            }
        }

        private void RedrawScene()
        {
            Renderer.DrawScene(ZoomCtrl.Cam, SceneSettings);

            GL.Translate(-_fixx, -_fixy, 0);
            DrawPlayers();
            GL.Translate(_fixx, _fixy, 0);

            if (_showDriverPath)
            {
                GL.Disable(EnableCap.Texture2D);
                foreach (var p in GetActivePlayers().Where(p => p.Player.FrameCount > 1))
                {
                    GL.Color4(p.DrivingLineColor);
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (var b in p.Player.GlobalBody)
                        GL.Vertex2(b.X, b.Y);
                    GL.End();
                }
            }

            Renderer.Swap();
        }

        private IEnumerable<PlayListObject> GetActivePlayers()
        {
            return _activePlayerIndices.Select(i => PlayListReplays[i]);
        }

        private IEnumerable<PlayListObject> GetVisiblePlayers()
        {
            return _visiblePlayerIndices.Select(i => PlayListReplays[i]);
        }

        public List<PlayerEvent<LogicalEventType>> UpdateEvents(List<LogicalEventType> typesToShow)
        {
            _currentEvents = GetSelectedPlayer().GetEvents(typesToShow.ToArray());
            return _currentEvents;
        }

        public Player GetSelectedPlayer()
        {
            return PlayListReplays[_selectedReplayIndex].Player;
        }

        public async Task ResetViewport(int width, int height)
        {
            // We can't reset the viewport on the current thread if we're playing,
            // so as a cheap workaround, we stop playing and then continue.
            if (!_playing)
            {
                Renderer.ResetViewport(width, height);
            }
            else
            {
                await PausePlaying();
                Renderer.ResetViewport(width, height);
                TogglePlay();
            }
        }
    }
}