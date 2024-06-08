using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using BrightIdeasSoftware;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Rec;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Elmanager.Settings;
using Elmanager.UI;
using Elmanager.Utilities;
using Control = System.Windows.Forms.Control;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using StringUtils = Elmanager.Utilities.StringUtils;

namespace Elmanager.ReplayViewer;

internal partial class ReplayViewerForm : FormMod
{
    private bool _draggingScreen;
    private Point _lastLocation;
    private int _lastMouseX;
    private Vector _moveStartPosition;
    private bool _zooming;
    private ReplayController _replayController = null!;
    private readonly TypedObjectListView<PlayListObject> _typedPlayList;
    private readonly TaskCompletionSource _tcs = new();
    private readonly FullScreenController _fullScreenController;
    private bool _bikeClicked;

    public ReplayViewerForm()
    {
        InitializeComponent();
        _typedPlayList = new TypedObjectListView<PlayListObject>(PlayList);
        _fullScreenController = new FullScreenController(this, ViewerResized, new List<Control> { TabControl1 });
        ViewerBox.HandleCreated += (_, _) =>
        {
            ViewerBox.Context.SwapInterval = 0;
            Initialize();
            ViewerResized();
            _tcs.SetResult();
        };
    }

    internal async Task WaitInit()
    {
        await _tcs.Task;
    }

    internal void SetReplays(ElmaFileObject<Replay> replay)
    {
        var oneReplay = new List<ElmaFileObject<Replay>> { replay };
        SetReplays(oneReplay);
    }

    internal void SetReplays(List<ElmaFileObject<Replay>> replays)
    {
        _replayController.SetReplays(replays);
        timeBar.Value = 0;
        UpdateControlColor(DrivingLinePanel, _replayController.PlayListReplays[0].DrivingLineColor);
        PlayList.SetObjects(_replayController.PlayListReplays);
        Text = Level.GetPossiblyInternal(replays[0].Obj.LevelFilename) + " - Replay viewer";
        if (replays[0].Obj.WrongLevelVersion)
            Text += " (wrong version)";
        if (!Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault)
            PlayList.Items[0].Selected = true;
        _typedPlayList.CheckedObjects = _typedPlayList.Objects;
        _replayController.SetInitialView();
        PlayList.AutoResizeColumns();
        UpdateEventsLists();
    }

    private static void UpdateControlColor(Control control, Color newColor)
    {
        control.BackColor = newColor;
    }

    private void UpdateLabels(bool refresh = false)
    {
        if (_replayController.PlayListReplays.Count == 0)
        {
            return;
        }
        TimeBox.Text = _replayController.CurrentTime.ToTimeString();
        timeBar.Value =
            (int)(Math.Min(_replayController.CurrentTime, _replayController.MaxTime) / _replayController.MaxTime * timeBar.Maximum);
        ShowCoordinates();
        SpeedLabel.Text = "Speed: " + _replayController.GetSpeed().ToString("F2");
        if (refresh)
        {
            TimeBox.Refresh();
            SpeedLabel.Refresh();
            CoordinateLabel.Refresh();
        }
    }

    private void ChangeRectColor(object sender, EventArgs e)
    {
        var clickedRect = (Panel)sender;
        ColorDialog1.Color = clickedRect.BackColor;
        if (ColorDialog1.ShowDialog() == DialogResult.OK)
        {
            var color = ColorDialog1.Color;
            var j = GetSelectedIndex();
            clickedRect.BackColor = color;
            if (sender.Equals(DrivingLinePanel))
            {
                if (j >= 0)
                    _replayController.PlayListReplays[j].DrivingLineColor = color;
                else
                    return;
            }

            RenderingOptionsChanged();
        }
    }

    private void CoordinateLabelClick(object sender, EventArgs e)
    {
        Global.AppSettings.ReplayViewer.ShowBikeCoords = !Global.AppSettings.ReplayViewer.ShowBikeCoords;
        ShowCoordinates();
    }

    private async void EventListBoxSelectedIndexChanged(object sender, EventArgs e)
    {
        await _replayController.PausePlaying();
        _replayController.SelectedEventChanged(EventListBox.SelectedIndex);
        UpdateLabels();
    }

    private void ToggleFullScreen(object? sender, EventArgs? e)
    {
        _fullScreenController.Toggle();
    }

    private Vector GetMouseCoordinates()
    {
        var mousePosNoTr = ViewerBox.PointToClient(MousePosition);
        var z = _replayController.ZoomCtrl;
        var mousePos = new Vector
        {
            X =
                z.Cam.XMin +
                mousePosNoTr.X * (z.Cam.XMax - z.Cam.XMin) / ViewerBox.Width,
            Y =
                z.Cam.YMax -
                mousePosNoTr.Y * (z.Cam.YMax - z.Cam.YMin) / ViewerBox.Height
        };
        return mousePos;
    }

    private int GetSelectedIndex()
    {
        if (PlayList.SelectedIndices.Count == 0)
            return -1;
        return PlayList.SelectedIndices[0];
    }

    private void Goto(object sender, MouseEventArgs e)
    {
        var selectedTime = _replayController.MaxTime * ((e.Location.X - 13) / (double)(timeBar.Width - 27));
        selectedTime = Math.Min(selectedTime, _replayController.MaxTime);
        selectedTime = Math.Max(selectedTime, 0);
        timeBar.Value = (int)(selectedTime * 1000 / _replayController.MaxTime);
        _replayController.UpdateCurrFrameFromTime(selectedTime);
    }

    private void Initialize()
    {
        UiUtils.ConfigureColumns<PlayListObject>(PlayList, hiddenColumns: new[] { "Player" });
        _replayController = new ReplayController(ViewerBox, Global.AppSettings.ReplayViewer.RenderingSettings);
        _replayController.UpdateReplaySettings();

        Size = Global.AppSettings.ReplayViewer.Size;
        playbackSpeedBar.Value = 0;
        FollowDriverBox.Checked = Global.AppSettings.ReplayViewer.FollowDriver;
        followAlsoWhenZooming.Checked = Global.AppSettings.ReplayViewer.FollowAlsoWhenZooming;
        LoopPlayingBox.Checked = Global.AppSettings.ReplayViewer.LoopPlaying;
        ShowDriverPathBox.Checked = Global.AppSettings.ReplayViewer.ShowDriverPath;
        MouseClickZoomBox.Text = Global.AppSettings.ReplayViewer.MouseClickStep.ToString();
        MouseWheelZoomBox.Text = Global.AppSettings.ReplayViewer.MouseWheelStep.ToString();
        LockedCamBox.Checked = Global.AppSettings.ReplayViewer.LockedCamera;
        TransparentInactiveBox.Checked = Global.AppSettings.ReplayViewer.DrawTransparentInactive;
        PictBackGroundBox.Checked = Global.AppSettings.ReplayViewer.PicturesInBackGround;
        PlayerFramesBox.Checked = Global.AppSettings.ReplayViewer.DrawOnlyPlayerFrames;
        SelectNoPlayersBox.Checked = Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault;
        HideStartObjectBox.Checked = Global.AppSettings.ReplayViewer.HideStartObject;
        multiSpyBox.Checked = Global.AppSettings.ReplayViewer.MultiSpy;
        WindowState = Global.AppSettings.ReplayViewer.WindowState;
        UpdateControlColor(ActivePlayerPanel, Global.AppSettings.ReplayViewer.ActivePlayerColor);
        UpdateControlColor(InActivePlayerPanel, Global.AppSettings.ReplayViewer.InactivePlayerColor);
        SetupEventHandlers();
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (m.Msg == NativeUtils.WmKeydown)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    CameraUtils.BeginArrowScroll(RedrawSceneIfNotPlaying, _replayController.ZoomCtrl);
                    break;
            }
        }

        return base.ProcessCmdKey(ref m, keyData);
    }

    private void KeyHandler(object? sender, KeyEventArgs e)
    {
        e.SuppressKeyPress = true;
        switch (e.KeyCode)
        {
            case Keys.Escape:
                if (!_fullScreenController.IsFullScreen)
                    Close();
                else
                {
                    _fullScreenController.Restore();
                }

                break;
            case Keys.Space:
                _replayController.TogglePlay();
                break;
            case KeyUtils.Decrease:
                if (!IsCtrlDown())
                    _replayController.PreviousFrame();
                break;
            case KeyUtils.Increase:
                if (!IsCtrlDown())
                    _replayController.NextFrame();
                break;
            case Keys.Oemcomma:
                if (PlayList.SelectedIndex == -1)
                    PlayList.SelectedIndex = PlayList.Items.Count - 1;
                else
                    PlayList.SelectedIndex--;
                break;
            case Keys.OemPeriod:
                if (PlayList.SelectedIndex == PlayList.Items.Count - 1)
                    PlayList.SelectedIndex = -1;
                else
                    PlayList.SelectedIndex++;
                break;
            case Keys.F11:
                ToggleFullScreen(null, null);
                break;
            case Keys.F5:
                _replayController.ZoomCtrl.ZoomFill(RenderingSettings);
                break;
            case Keys.Enter:
                TextBoxKeyPress(e);
                break;
            default:
                e.SuppressKeyPress = false;
                break;
        }
    }

    private RenderingSettings RenderingSettings => Global.AppSettings.ReplayViewer.RenderingSettings;

    private static bool IsCtrlDown() => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

    private void MouseClickZoomBoxTextChanged(object sender, EventArgs e)
    {
        Global.AppSettings.ReplayViewer.MouseClickStep = MouseClickZoomBox.ValueAsInt;
    }

    private void MouseWheelZoom(object sender, MouseEventArgs e)
    {
        if (!ViewerBox.ClientRectangle.Contains(ViewerBox.PointToClient(Cursor.Position)))
        {
            return;
        }
        Zoom(e.Delta > 0, 1 - Global.AppSettings.ReplayViewer.MouseWheelStep / 100.0);
    }

    private void Zoom(bool zoomIn, double zoomFactor)
    {
        _replayController.ZoomCtrl.Zoom(GetMouseCoordinates(), zoomIn, zoomFactor, RenderingSettings);
        Global.AppSettings.ReplayViewer.ZoomLevel = _replayController.ZoomCtrl.ZoomLevel;
    }

    private void MouseWheelZoomBoxTextChanged(object sender, EventArgs e)
    {
        Global.AppSettings.ReplayViewer.MouseWheelStep = MouseWheelZoomBox.ValueAsInt;
    }

    private async void PlayBarScroll(object sender, EventArgs e)
    {
        await _replayController.PausePlaying();
        _replayController.UpdateCurrFrameFromTime(timeBar.Value / (double)timeBar.Maximum * _replayController.MaxTime);
        UpdateLabels(true);
    }

    private void PlayListSelectionChanged(object sender, EventArgs e)
    {
        var i = GetSelectedIndex();
        _replayController.SelectedReplayChanged(
            i,
            PlayList.SelectedIndices.Cast<int>(),
            PlayList.CheckedIndices.Cast<int>()
        );
        if (i < 0)
        {
            RedrawSceneIfNotPlaying();
            DrivingLinePanel.Enabled = false;
        }
        else
        {
            DrivingLinePanel.Enabled = true;
            UpdateControlColor(DrivingLinePanel, _replayController.PlayListReplays[i].DrivingLineColor);
        }

        UpdateEventsLists();
        if (_bikeClicked)
        {
            _replayController.RedrawSceneIfNotPlaying();
        }
        else
        {
            _replayController.UpdateCameraAndDrawSceneIfNotPlaying();
        }

        _bikeClicked = false;
    }

    private void RenderingOptionsChanged(object? sender = null, EventArgs? e = null)
    {
        SaveViewerSettings();
        var speed = Math.Pow(2, playbackSpeedBar.Value);
        _replayController.SetPlayBackSpeed(speed);
        toolTip1.SetToolTip(playbackSpeedBar, "Playback speed: " + speed + "x");
        _replayController.UpdateReplaySettings();
        RedrawSceneIfNotPlaying();
    }

    private void RenderingSettingsButtonClick(object sender, EventArgs e)
    {
        var settingsForm = new RenderingSettingsForm(Global.AppSettings.ReplayViewer.RenderingSettings);
        settingsForm.Changed += x =>
        {
            _replayController.Renderer.UpdateSettings(_replayController.Lev!, x);
            RedrawSceneIfNotPlaying();
        };
        settingsForm.ShowDialog();
    }

    private void SaveViewerSettings()
    {
        var settings = Global.AppSettings.ReplayViewer;
        settings.Size = Size;
        settings.FollowDriver = FollowDriverBox.Checked;
        settings.FollowAlsoWhenZooming = followAlsoWhenZooming.Checked;
        settings.LoopPlaying = LoopPlayingBox.Checked;
        settings.ShowDriverPath = ShowDriverPathBox.Checked;
        settings.LockedCamera = LockedCamBox.Checked;
        settings.DrawTransparentInactive = TransparentInactiveBox.Checked;
        settings.PicturesInBackGround = PictBackGroundBox.Checked;
        settings.ActivePlayerColor = ActivePlayerPanel.BackColor;
        settings.InactivePlayerColor = InActivePlayerPanel.BackColor;
        settings.DrawOnlyPlayerFrames = PlayerFramesBox.Checked;
        settings.DontSelectPlayersByDefault = SelectNoPlayersBox.Checked;
        settings.HideStartObject = HideStartObjectBox.Checked;
        settings.WindowState = WindowState;
        settings.MultiSpy = multiSpyBox.Checked;
    }

    private void SetupEventHandlers()
    {
        Resize += ViewerResized;
        var ope = new Action(() => UpdateLabels());
        _replayController.PlayingElapsed += (_, _) =>
        {
            Invoke(ope);
        };

        ZoomFillButton.MouseDown += (_, _) => _replayController.ZoomCtrl.ZoomFill(RenderingSettings);
        PlayButton.MouseDown += (_, _) => _replayController.TogglePlay();
        StopButton.MouseDown += async (_, _) =>
        {
            await _replayController.StopPlaying();
            UpdateLabels();
        };
        PrevFrameButton.MouseDown += (_, _) =>
        {
            _replayController.PreviousFrame();
            UpdateLabels();
        };
        NextFrameButton.MouseDown += (_, _) =>
        {
            _replayController.NextFrame();
            UpdateLabels();
        };
        ShowDriverPathBox.CheckedChanged += RenderingOptionsChanged;
        LockedCamBox.CheckedChanged += RenderingOptionsChanged;
        LoopPlayingBox.CheckedChanged += RenderingOptionsChanged;
        FollowDriverBox.CheckedChanged += RenderingOptionsChanged;
        followAlsoWhenZooming.CheckedChanged += RenderingOptionsChanged;
        PictBackGroundBox.CheckedChanged += RenderingOptionsChanged;
        TransparentInactiveBox.CheckedChanged += RenderingOptionsChanged;
        PlayerFramesBox.CheckedChanged += RenderingOptionsChanged;
        HideStartObjectBox.CheckedChanged += RenderingOptionsChanged;
        playbackSpeedBar.ValueChanged += RenderingOptionsChanged;
        multiSpyBox.CheckedChanged += RenderingOptionsChanged;
        playbackSpeedBar.MouseWheel += TrackBarScrolled;
        timeBar.MouseWheel += TrackBarScrolled;
        TabControl1.KeyDown += KeyHandler;
        foreach (ToolStripMenuItem toolStripMenuItem in contextMenuStrip1.Items)
        {
            toolStripMenuItem.CheckedChanged += UpdateEventsLists;
        }
    }

    private void ShowCoordinates()
    {
        if (Global.AppSettings.ReplayViewer.ShowBikeCoords)
        {
            var x = _replayController.GetBikeCoordinates();
            CoordinateLabel.Text = "Bike X: " + x.X.ToString("F3") + " Y: " + x.Y.ToString("F3");
        }
        else
        {
            var x = GetMouseCoordinates();
            CoordinateLabel.Text = "Mouse X: " + x.X.ToString("F3") + " Y: " + x.Y.ToString("F3");
        }
    }

    private void ShowTime(object sender, MouseEventArgs e)
    {
        if (e.Location == _lastLocation)
            return;
        toolTip1.SetToolTip(timeBar,
            (_replayController.MaxTime * ((e.Location.X - 13) / (double)(timeBar.Width - 27))).ToTimeString
                ());
        _lastLocation = e.Location;
    }

    private void SnapShotButtonClick(object sender, EventArgs e)
    {
        if (_replayController.Playing)
        {
            return;
        }
        if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
        {
            using var bmp = ElmaRenderer.GetSnapShotOfCurrent(_replayController.ZoomCtrl);
            bmp.Save(SaveFileDialog1.FileName, ImageFormat.Png);
        }
    }

    private void TextBoxKeyPress(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && TimeBox.Text.Length == 9)
            _replayController.UpdateCurrFrameFromTime(StringUtils.StringToTime(TimeBox.Text));
    }

    private void TrackBarScrolled(int delta)
    {
        // TODO: Was this ever needed?
        // Zoom(delta > 0, 1 - Global.AppSettings.ReplayViewer.MouseWheelStep / 100.0);
    }

    private void UpdateEventsLists(object? sender = null, EventArgs? e = null)
    {
        var i = GetSelectedIndex();
        if (i >= 0)
        {
            var typesToShow = new List<LogicalEventType>();
            if (applesToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.AppleTake);
            if (supervoltsToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.SuperVolt);
            if (leftVoltsToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.LeftVolt);
            if (rightVoltsToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.RightVolt);
            if (turnsToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.Turn);
            if (groundtouchesToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.GroundTouch);
            if (gasOnToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.GasOn);
            if (gasOffToolStripMenuItem.Checked)
                typesToShow.Add(LogicalEventType.GasOff);
            var evs = _replayController.UpdateEvents(typesToShow);
            var p = _replayController.GetSelectedPlayer();
            PutEventsToList(p, EventListBox, p.Finished || p.FakeFinish, evs);
        }
        else
            EventListBox.Items.Clear();

        if (sender != null)
        {
            contextMenuStrip1.Show();
        }
    }

    private async void ViewerClosing(object sender, CancelEventArgs e)
    {
        SaveViewerSettings();
        if (_replayController.Playing)
        {
            e.Cancel = true; // must make sure playing thread has stopped before closing
            await _replayController.PausePlaying();
            Close();
        }
    }

    private void ViewerMouseDown(object sender, MouseEventArgs e)
    {
        var z = GetMouseCoordinates();
        switch (e.Button)
        {
            case MouseButtons.Left:
                if (_replayController.HighlightPlayerIndex is { } i)
                {
                    if (IsCtrlDown())
                    {
                        if (PlayList.SelectedIndices.Contains(i))
                        {
                            PlayList.SelectedIndices.Remove(i);
                        }
                        else
                        {
                            PlayList.SelectedIndices.Add(i);
                        }
                    }
                    else
                    {
                        PlayList.SelectedIndex = i;
                    }

                    _bikeClicked = true;
                }
                break;
            case MouseButtons.Right:
                break;
            case MouseButtons.Middle:
                _moveStartPosition = z;
                _draggingScreen = true;
                break;
        }
    }

    private void ViewerMouseMoving(object sender, MouseEventArgs e)
    {
        ShowCoordinates();
        var mouse = GetMouseCoordinates();
        var zoomCtrl = _replayController.ZoomCtrl;
        if (_draggingScreen)
        {
            zoomCtrl.CenterX = _moveStartPosition.X + (zoomCtrl.Cam.XMax + zoomCtrl.Cam.XMin) / 2 - mouse.X;
            zoomCtrl.CenterY = _moveStartPosition.Y + (zoomCtrl.Cam.YMax + zoomCtrl.Cam.YMin) / 2 - mouse.Y;
        }
        else
        {
            var ind = _replayController.UpdateHighlight(mouse);
            ViewerBox.Cursor = ind is not null ? Cursors.Hand : Cursors.Default;
        }
        RedrawSceneIfNotPlaying();
    }

    private void ViewerMouseUp(object sender, MouseEventArgs e)
    {
        _draggingScreen = false;
    }

    private async void ViewerResized(object? sender = null, EventArgs? e = null)
    {
        if (sender != null)
        {
            viewerSizeXBox.Text = ViewerBox.Width.ToString();
            viewerSizeYBox.Text = ViewerBox.Height.ToString();
        }

        if (_replayController != null)
        {
            await _replayController.ResetViewport(ViewerBox.Width, ViewerBox.Height);
            if (_replayController.Lev != null)
            {
                RedrawSceneIfNotPlaying();
            }
        }
    }

    private void ViewerSizeBoxKeyUp(object sender, KeyEventArgs e)
    {
        Resize -= ViewerResized;
        ClientSize = new Size(viewerSizeXBox.ValueAsInt + ViewerBox.Location.X, viewerSizeYBox.ValueAsInt);
        ViewerResized();
        Resize += ViewerResized;
    }

    private void ZoomButtonMouseDown(object sender, MouseEventArgs e)
    {
        _zooming = true;
        _lastMouseX = MousePosition.X;
    }

    private void ZoomButtonMouseMove(object sender, MouseEventArgs e)
    {
        if (_zooming)
        {
            if (_lastMouseX == MousePosition.X)
                return;
            if (MousePosition.X - _lastMouseX < 0)
            {
                _replayController.ZoomCtrl.ZoomLevel *= 1.05;
            }
            else
            {
                _replayController.ZoomCtrl.ZoomLevel /= 1.05;
            }

            RedrawSceneIfNotPlaying();
            _lastMouseX = MousePosition.X;
            Global.AppSettings.ReplayViewer.ZoomLevel = _replayController.ZoomCtrl.ZoomLevel;
        }
    }

    private void RedrawSceneIfNotPlaying()
    {
        _replayController.RedrawSceneIfNotPlaying();
    }

    private void ZoomButtonMouseUp(object sender, MouseEventArgs e)
    {
        _zooming = false;
    }

    private static void PutEventsToList(Player player, ListBox listBox, bool finished,
        List<PlayerEvent<LogicalEventType>> selectedEvents)
    {
        int turnCounter = 0;
        int leftVoltCounter = 0;
        int rightVoltCounter = 0;
        int superVoltCounter = 0;
        int gtCounter = 0;
        int appleCounter = 0;
        int gasCounter = 0;
        double lastEventTime = 0;
        listBox.Items.Clear();
        foreach (var e in selectedEvents)
        {
            double eventTime = e.Time;
            string strToAdd;
            switch (e.Type)
            {
                case LogicalEventType.AppleTake:
                    appleCounter++;
                    strToAdd = "Apple " + appleCounter;
                    break;
                case LogicalEventType.SuperVolt:
                    superVoltCounter++;
                    strToAdd = "Supervolt " + superVoltCounter;
                    break;
                case LogicalEventType.LeftVolt:
                    leftVoltCounter++;
                    strToAdd = "Left volt " + leftVoltCounter;
                    break;
                case LogicalEventType.RightVolt:
                    rightVoltCounter++;
                    strToAdd = "Right volt " + rightVoltCounter;
                    break;
                case LogicalEventType.Turn:
                    turnCounter++;
                    strToAdd = "Turn " + turnCounter;
                    break;
                case LogicalEventType.GroundTouch:
                    gtCounter++;
                    strToAdd = "Touch " + gtCounter;
                    break;
                case LogicalEventType.GasOn:
                    gasCounter++;
                    strToAdd = "Gas on " + gasCounter;
                    break;
                case LogicalEventType.GasOff:
                    strToAdd = "Gas off " + gasCounter;
                    break;
                default:
                    throw new Exception("Unknown ReplayEventType.");
            }

            listBox.Items.Add(strToAdd + ": " + eventTime.ToTimeString() + " + " +
                              (eventTime - lastEventTime).ToTimeString());
            lastEventTime = eventTime;
        }

        if ((finished && player.FakeFinish) || player.Finished)
            listBox.Items.Add("Flower: " + StringUtils.ToTimeString(player.Time) + " + " +
                              (player.Time - lastEventTime).ToTimeString());
    }
}