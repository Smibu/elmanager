using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Elmanager.Application;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Rec;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Elmanager.UI;
using OpenTK.GLControl;
using Timer = System.Timers.Timer;

namespace Elmanager.ReplayViewer;

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

    private List<PlayerEvent<LogicalEventType>> _currentEvents = new();
    private List<int> _activePlayerIndices = new();
    private bool _playing;
    private bool _requestPlayCancel;
    private List<int> _visiblePlayerIndices = new();
    public int? HighlightPlayerIndex { get; private set; }
    internal Level? Lev;
    private double _playBackSpeed = 1.0;
    private readonly Stopwatch _playTimer = new();
    private bool _loopPlaying;
    private bool _wrongLevVersion;
    private double _frameStep = 0.02;
    private double _initialTime;
    private bool _showDriverPath;
    private bool _followDriver;
    private bool _drawInActiveAsTransparent;
    private bool _drawOnlyPlayerFrames;
    private bool _multiSpy;
    private List<PlayerEvent<LogicalEventType>> _currentPlayerAppleEvents = new();
    private int _selectedReplayIndex;
    private int _lastHiddenAppleCount = -1;
    private double _fixx;
    private double _fixy;
    private readonly Timer _timer = new(25); // Don't update unnecessarily often to avoid overhead
    private Task? _playTask;
    private readonly ReplayViewerRenderingSettings _settings;

    public event ElapsedEventHandler PlayingElapsed
    {
        add => _timer.Elapsed += value;
        remove => _timer.Elapsed -= value;
    }

    public ReplayController(GLControl viewerBox, ReplayViewerRenderingSettings replayViewerRenderingSettings)
    {
        Renderer = new ElmaRenderer(viewerBox, replayViewerRenderingSettings);
        _settings = replayViewerRenderingSettings;
    }

    private void DrawPlayers()
    {
        var players = new List<(PlayerState State, PlayerRenderOpts Opts)>();
        foreach (var x in _visiblePlayerIndices)
        {
            var isSelected = _activePlayerIndices.Contains(x);
            var opts = new PlayerRenderOpts
            {
                Color = isSelected ? _settings.ActivePlayerColor : _settings.InactivePlayerColor,
                IsActive = isSelected,
                UseGraphics = !_drawOnlyPlayerFrames,
                UseTransparency = !isSelected && _drawInActiveAsTransparent
            };
            players.Add((PlayListReplays[x].Player.GetInterpolatedState(CurrentTime), opts));
        }
        Renderer.DrawPlayers(players, _settings);
    }

    internal void UpdateReplaySettings()
    {
        _showDriverPath = Global.AppSettings.ReplayViewer.ShowDriverPath;
        _followDriver = Global.AppSettings.ReplayViewer.FollowDriver;
        _drawInActiveAsTransparent = Global.AppSettings.ReplayViewer.DrawTransparentInactive;
        _frameStep = Global.AppSettings.ReplayViewer.FrameStep;
        _loopPlaying = Global.AppSettings.ReplayViewer.LoopPlaying;
        _drawOnlyPlayerFrames = Global.AppSettings.ReplayViewer.DrawOnlyPlayerFrames;
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
        UpdateHiddenObjectsIfNeeded();
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
        _requestPlayCancel = false;
        var tcs = new TaskCompletionSource();

        void StopLoop()
        {
            System.Windows.Forms.Application.Idle -= OnIdle;
            tcs.TrySetResult();
        }

        void OnIdle(object? sender, EventArgs e)
        {
            while (NativeUtils.IsApplicationIdle())
            {
                if (_requestPlayCancel)
                {
                    StopLoop();
                    return;
                }

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
                        StopLoop();
                        return;
                    }
                }
            }
        }

        System.Windows.Forms.Application.Idle += OnIdle;
        await tcs.Task;

        _playing = false;
        _playTimer.Stop();
        _timer.Stop();
    }

    internal double CurrentTime { get; private set; }

    internal double MaxTime { get; private set; }

    internal List<PlayListObject> PlayListReplays { get; private set; } = new();

    internal ZoomController ZoomCtrl { get; private set; } = null!;

    internal ElmaRenderer Renderer { get; }

    private SceneSettings SceneSettings { get; } = new();

    internal Vector GetBikeCoordinates()
    {
        var f = FirstActivePlayer;
        if (f == null)
        {
            return new Vector();
        }

        var r = f.GetInterpolatedState(CurrentTime);
        return r.GlobalBody;
    }

    private Player? FirstActivePlayer => GetActivePlayers().Select(p => p.Player).FirstOrDefault();

    internal double GetSpeed()
    {
        var first = FirstActivePlayer;
        if (first == null)
        {
            return 0.0;
        }

        var i1 = first.GetInterpolatedState(CurrentTime);
        var i2 = first.GetInterpolatedState(Math.Max(CurrentTime - 1 / 30.0, 0));
        var xdiff = i1.GlobalBody.X - i2.GlobalBody.X;
        var ydiff = i1.GlobalBody.Y - i2.GlobalBody.Y;
        return Math.Sqrt(xdiff * xdiff + ydiff * ydiff) * Player.SpeedFactor;
    }

    private void UpdateHiddenObjectsIfNeeded()
    {
        if (Lev == null) return;

        var hiddenCount = 0;

        if (!_wrongLevVersion && _activePlayerIndices.Count > 0)
        {
            hiddenCount = BinarySearchEventCount(_currentPlayerAppleEvents, CurrentTime);
        }

        if (hiddenCount != _lastHiddenAppleCount)
        {
            _lastHiddenAppleCount = hiddenCount;
            HashSet<int>? hidden = null;
            if (hiddenCount > 0)
            {
                hidden = new HashSet<int>(hiddenCount);
                for (var i = 0; i < hiddenCount; i++)
                {
                    hidden.Add(_currentPlayerAppleEvents[i].Info);
                }
            }

            Renderer.SetHiddenObjects(Lev, hidden);
        }
    }

    private static readonly IComparer<PlayerEvent<LogicalEventType>> EventTimeComparer =
        Comparer<PlayerEvent<LogicalEventType>>.Create((a, b) => a.Time.CompareTo(b.Time));

    private static int BinarySearchEventCount(List<PlayerEvent<LogicalEventType>> events, double time)
    {
        var index = events.BinarySearch(new PlayerEvent<LogicalEventType>(default, time), EventTimeComparer);
        if (index < 0)
            return ~index;
        while (index > 0 && events[index - 1].Time == time)
            index--;
        return index;
    }

    private void UpdateCamera(bool zoomToDriver)
    {
        _fixx = 0.0;
        _fixy = 0.0;
        var firstActive = FirstActivePlayer;
        var firstVisible = FirstVisiblePlayer;
        if (firstActive != null && zoomToDriver)
        {
            var r = firstActive.GetInterpolatedState(CurrentTime);
            ZoomCtrl.CenterX = r.GlobalBody.X;
            ZoomCtrl.CenterY = r.GlobalBody.Y;
            var jf = ZoomCtrl.Cam.FixJitter(Renderer.ViewportWidth, Renderer.ViewportHeight);
            _fixx = jf.X;
            _fixy = jf.Y;
        }
        else if (_multiSpy && firstVisible != null)
        {
            var player = firstVisible.Player;
            var fr = player.GetInterpolatedState(CurrentTime);
            var xmin = fr.GlobalBody.X;
            var xmax = xmin;
            var ymin = fr.GlobalBody.Y;
            var ymax = ymin;
            foreach (var p in GetVisiblePlayers())
            {
                var r = p.Player.GetInterpolatedState(CurrentTime);
                var gx = r.GlobalBody.X;
                xmin = Math.Min(gx, xmin);
                xmax = Math.Max(gx, xmax);
                var gy = r.GlobalBody.Y;
                ymin = Math.Min(gy, ymin);
                ymax = Math.Max(gy, ymax);
            }

            ZoomCtrl.CenterX = (xmin + xmax) / 2;
            ZoomCtrl.CenterY = (ymin + ymax) / 2;
            ZoomCtrl.ZoomLevel = Math.Max((xmax + 5 - ZoomCtrl.CenterX) / Renderer.AspectRatio,
                ymax + 5 - ZoomCtrl.CenterY);
            ZoomCtrl.ZoomLevel = Math.Max(ZoomCtrl.ZoomLevel, 5);
        }
    }

    private PlayListObject? FirstVisiblePlayer => GetVisiblePlayers().FirstOrDefault();

    public bool Playing => _playing;

    private void FocusIndicesChanged()
    {
        if (!_wrongLevVersion && FirstActivePlayer is { } ap)
        {
            _currentPlayerAppleEvents = ap.GetEvents(LogicalEventType.AppleTake);
        }
        else
        {
            _currentPlayerAppleEvents.Clear();
        }

        _lastHiddenAppleCount = -1;
        UpdateHiddenObjectsIfNeeded();
    }

    public void SetReplays(List<ElmaFileObject<Replay>> replays)
    {
        PlayListReplays = new List<PlayListObject>();
        foreach (var replay in replays)
        {
            var playerNum = 1;
            foreach (var player in replay.Obj.Players)
            {
                PlayListReplays.Add(new PlayListObject(Path.GetFileNameWithoutExtension(replay.File.FileName),
                    playerNum,
                    player));
                playerNum++;
            }
        }

        var lev = replays[0].Obj.GetLevel();
        ZoomCtrl = new ZoomController(new ElmaCamera(),
            lev,
            RedrawSceneIfNotPlaying)
        { ZoomLevel = 5.0 };
        MaxTime = PlayListReplays.Max(p => p.Player.FrameCount) / 30.0;
        CurrentTime = 0.0;
        Lev = lev;
        Renderer.UpdateSettings(lev, _settings);
        _activePlayerIndices = new List<int>();
        _visiblePlayerIndices = new List<int>();
        _lastHiddenAppleCount = 0;
        _wrongLevVersion = replays[0].Obj.WrongLevelVersion;
        for (var i = 0; i < PlayListReplays.Count; i++)
            PlayListReplays[i].DrivingLineColor = Color.FromArgb((int)_colourValues[i % _colourValues.Length]);
    }

    public void SetInitialView()
    {
        if (Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault)
            ZoomCtrl.ZoomFill(_settings, Renderer.AspectRatio);
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
        Renderer.DrawScene(ZoomCtrl.Cam, 0, SceneSettings);

        ZoomCtrl.Cam.CenterX += _fixx;
        ZoomCtrl.Cam.CenterY += _fixy;
        Renderer.SetCamera(ZoomCtrl.Cam);
        _fixx = 0;
        _fixy = 0;
        DrawPlayers();

        if (_showDriverPath)
        {
            foreach (var p in GetActivePlayers().Where(p => p.Player.FrameCount > 1))
                Renderer.DrawLineStrip(p.Player.GlobalBody, p.DrivingLineColor);
        }

        if (HighlightPlayerIndex is { } i)
        {
            var p = PlayListReplays[i].Player;
            var s = p.GetInterpolatedState(CurrentTime);
            Renderer.DrawCircle(s.LeftWheel, ElmaConstants.WheelRadius, Color.Yellow, _settings.CircleDrawingAccuracy);
            Renderer.DrawCircle(s.RightWheel, ElmaConstants.WheelRadius, Color.Yellow, _settings.CircleDrawingAccuracy);
            Renderer.DrawCircle(s.Head, ElmaConstants.HeadRadius, Color.Yellow, _settings.CircleDrawingAccuracy);
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

    public void ResetViewport(int width, int height)
    {
        Renderer.ResetViewport(width, height);
    }

    public int? UpdateHighlight(Vector mousePos)
    {
        HighlightPlayerIndex = null;
        foreach (var index in _activePlayerIndices.AsEnumerable().Reverse()
                     .Concat(_visiblePlayerIndices.AsEnumerable().Reverse()))
        {
            var p = PlayListReplays[index].Player;
            var s = p.GetInterpolatedState(CurrentTime);
            if (s.LeftWheel.Dist(mousePos) < ElmaConstants.WheelRadius ||
                s.RightWheel.Dist(mousePos) < ElmaConstants.WheelRadius ||
                s.Head.Dist(mousePos) < ElmaConstants.HeadRadius)
            {
                HighlightPlayerIndex = index;
                break;
            }
        }

        return HighlightPlayerIndex;
    }
}