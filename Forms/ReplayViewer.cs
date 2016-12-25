using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using OpenTK;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.Forms
{
    partial class ReplayViewer
    {
        private readonly uint[] _colourValues = new[]
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

        private PlayerEvent[] _currentEvents;
        private bool _draggingScreen;
        private bool _fullScreen;
        private Point _lastLocation;
        private int _lastMouseX;
        private Vector _moveStartPosition;
        private List<PlayListObject> _playListReplays;
        private ElmaRenderer _renderer;
        private FormWindowState _windowState;
        private Vector _zoomRectStartPoint;
        private bool _zoomRecting;
        private bool _zooming;

        internal ReplayViewer(Replay rp)
        {
            InitializeComponent();
            Initialize();
            SetReplays(rp);
            ViewerResized();
        }

        internal ReplayViewer(List<Replay> replaysToPlay)
        {
            InitializeComponent();
            Initialize();
            SetReplays(replaysToPlay);
            ViewerResized();
        }

        internal void SetReplays(Replay replay)
        {
            var oneReplay = new List<Replay> {replay};
            SetReplays(oneReplay);
        }

        internal void SetReplays(List<Replay> replays)
        {
            _playListReplays = new List<PlayListObject>();
            foreach (Replay replay in replays)
            {
                _playListReplays.Add(new PlayListObject(Path.GetFileNameWithoutExtension(replay.FileName), 1,
                                                        replay.Player1));
                if (replay.IsMulti)
                    _playListReplays.Add(new PlayListObject(Path.GetFileNameWithoutExtension(replay.FileName), 2,
                                                            replay.Player2));
            }
            timeBar.Value = 0;
            _renderer.InitializeReplays(replays);
            _renderer.DrivingLineColors = new Color[_playListReplays.Count];
            for (int i = 0; i < _renderer.DrivingLineColors.Length; i++)
                _renderer.DrivingLineColors[i] = Color.FromArgb((int) _colourValues[i % _colourValues.Count()]);
            UpdateControlColor(DrivingLinePanel, _renderer.DrivingLineColors[0]);
            PlayList.SetObjects(_playListReplays);
            Text = Utils.GetPossiblyInternal(replays[0].LevelFilename) + " - Replay viewer";
            if (replays[0].WrongLevelVersion)
                Text += " (wrong version)";
            if (Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault)
                _renderer.ZoomFill();
            else
                _renderer.ZoomLevel = Global.AppSettings.ReplayViewer.ZoomLevel;
            if (!Global.AppSettings.ReplayViewer.DontSelectPlayersByDefault)
                PlayList.Items[0].Selected = true;
            PlayList.CheckedObjects = (IList) PlayList.Objects;
            UpdateEventsLists();
        }

        private static void UpdateControlColor(Control control, Color newColor)
        {
            control.BackColor = newColor;
        }

        private void AfterRendering()
        {
            TimeBox.Text = _renderer.CurrentTime.ToTimeString();
            timeBar.Value =
                (int) (Math.Min(_renderer.CurrentTime, _renderer.MaxTime) / _renderer.MaxTime * timeBar.Maximum);
            ShowCoordinates();
            SpeedLabel.Text = "Speed: " + _renderer.GetSpeed().ToString("F2");
        }

        private void ChangeRectColor(object sender, EventArgs e)
        {
            var clickedRect = (Panel) sender;
            ColorDialog1.Color = clickedRect.BackColor;
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color i = ColorDialog1.Color;
                int j = GetSelectedIndex();
                clickedRect.BackColor = i;
                if (sender.Equals(DrivingLinePanel))
                {
                    if (j >= 0)
                        _renderer.DrivingLineColors[j] = i;
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

        private void CoordinateLabelClick(object sender, MouseEventArgs e)
        {
        }

        private void CustomRendering()
        {
            if (_zoomRecting)
            {
                Vector z = GetMouseCoordinates();
                _renderer.DrawRectangle(_zoomRectStartPoint.X, -_zoomRectStartPoint.Y, z.X, -z.Y, Color.Blue);
            }
        }

        private void EventListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            int i = EventListBox.SelectedIndex;
            if (!_renderer.Playing)
            {
                if (i == _currentEvents.Count())
                    UpdateCurrFrameFromTime(_playListReplays[GetSelectedIndex()].Player.Time);
                else if (i >= 0)
                    UpdateCurrFrameFromTime(_currentEvents[i].Time);
            }
        }

        private void FullScreen(object sender, EventArgs e)
        {
            Resize -= ViewerResized;
            _windowState = WindowState;
            WindowState = FormWindowState.Normal;
            TabControl1.Visible = false;
            FormBorderStyle = FormBorderStyle.None;
            ViewerBox.Location = new Point(0, 0);
            TopMost = true;
            _renderer.SetFullScreenMode(DisplayDevice.Default.AvailableResolutions[ResolutionBox.SelectedIndex]);
            WindowState = FormWindowState.Maximized;
            _fullScreen = true;
            ViewerBox.Size = Size;
            ViewerResized();
            Resize += ViewerResized;
        }

        private Vector GetMouseCoordinates()
        {
            Point mousePosNoTr = ViewerBox.PointToClient(MousePosition);
            var mousePos = new Vector
                               {
                                   X =
                                       _renderer.XMin +
                                       mousePosNoTr.X * (_renderer.XMax - _renderer.XMin) / ViewerBox.Width,
                                   Y =
                                       _renderer.YMax -
                                       mousePosNoTr.Y * (_renderer.YMax - _renderer.YMin) / ViewerBox.Height
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
            double selectedTime = _renderer.MaxTime * ((e.Location.X - 13) / (double) (timeBar.Width - 27));
            selectedTime = Math.Min(selectedTime, _renderer.MaxTime);
            selectedTime = Math.Max(selectedTime, 0);
            timeBar.Value = (int) (selectedTime * 1000 / _renderer.MaxTime);
            UpdateCurrFrameFromTime(selectedTime);
        }

        private void Initialize()
        {
            MemberInfo[] playListObjectMembers =
                typeof (PlayListObject).GetMembers(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i <= 1; i++)
                PlayList.GetColumn(i).AspectName = playListObjectMembers[i + 4].Name;
            _renderer = new ElmaRenderer(ViewerBox, Global.AppSettings.ReplayViewer.RenderingSettings);
            _renderer.UpdateReplaySettings();
            _renderer.AfterDrawing = AfterRendering;
            _renderer.CustomRendering = CustomRendering;
            foreach (DisplayResolution resolution in DisplayDevice.Default.AvailableResolutions)
            {
                ResolutionBox.Items.Add(resolution.Width + "x" + resolution.Height + "x" + resolution.BitsPerPixel +
                                        ", " + resolution.RefreshRate + " Hz");
                if (resolution.Equals(DisplayDevice.Default.SelectResolution(1, 1, 1, 1)))
                    ResolutionBox.SelectedIndex = ResolutionBox.Items.Count - 1;
            }
            Size = Global.AppSettings.ReplayViewer.Size;
            playbackSpeedBar.Value = 0;
            FollowDriverBox.Checked = Global.AppSettings.ReplayViewer.FollowDriver;
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

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (!_fullScreen)
                        Close();
                    else
                    {
                        _fullScreen = false;
                        Resize -= ViewerResized;
                        DisplayDevice.Default.RestoreResolution();
                        FormBorderStyle = FormBorderStyle.Sizable;
                        TopMost = false;
                        WindowState = _windowState;
                        TabControl1.Visible = true;
                        ViewerBox.Location = new Point(TabControl1.Width + 1, 0);
                        ViewerBox.Size = new Size(Width - TabControl1.Width - 17, Height - 36);
                        Resize += ViewerResized;
                        ViewerResized();
                    }
                    break;
                case Keys.Space:
                    _renderer.TogglePlay();
                    break;
                case Constants.Decrease:
                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        _renderer.PreviousFrame();
                    break;
                case Constants.Increase:
                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        _renderer.NextFrame();
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
                    FullScreen(null, null);
                    break;
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    Utils.BeginArrowScroll(_renderer);
                    break;
                case Keys.F5:
                    _renderer.ZoomFill();
                    break;
                case Keys.Enter:
                    TextBoxKeyPress(e);
                    break;
                default:
                    e.SuppressKeyPress = false;
                    break;
            }
        }

        private void MouseClickZoomBoxTextChanged(object sender, EventArgs e)
        {
            Global.AppSettings.ReplayViewer.MouseClickStep = MouseClickZoomBox.ValueAsInt;
        }

        private void MouseWheelZoom(object sender, MouseEventArgs e)
        {
            Zoom(e.Delta > 0, 1 - Global.AppSettings.ReplayViewer.MouseWheelStep / 100.0);
        }

        private void Zoom(bool zoomIn, double zoomFactor)
        {
            _renderer.Zoom(GetMouseCoordinates(), zoomIn, zoomFactor);
            Global.AppSettings.ReplayViewer.ZoomLevel = _renderer.ZoomLevel;
        }

        private void MouseWheelZoomBoxTextChanged(object sender, EventArgs e)
        {
            Global.AppSettings.ReplayViewer.MouseWheelStep = MouseWheelZoomBox.ValueAsInt;
        }

        private void PlayBarScroll(object sender, EventArgs e)
        {
            _renderer.Playing = false;
            _renderer.CurrentTime = timeBar.Value / (double) timeBar.Maximum * _renderer.MaxTime;
            _renderer.DrawSceneDefault();
        }

        private void PlayListSelectionChanged(object sender, EventArgs e)
        {
            int i = GetSelectedIndex();
            _renderer.ActivePlayerIndices.Clear();
            foreach (int x in PlayList.SelectedIndices)
                _renderer.ActivePlayerIndices.Add(x);
            _renderer.VisiblePlayerIndices.Clear();
            foreach (int x in PlayList.CheckedIndices)
                _renderer.VisiblePlayerIndices.Add(x);
            _renderer.FocusIndicesChanged();
            if (i < 0)
            {
                _renderer.DrawSceneDefault();
                DrivingLinePanel.Enabled = false;
            }
            else
            {
                DrivingLinePanel.Enabled = true;
                UpdateControlColor(DrivingLinePanel, _renderer.DrivingLineColors[i]);
            }
            UpdateEventsLists();
            _renderer.DrawSceneDefault();
        }

        private void RenderingOptionsChanged(object sender = null, EventArgs e = null)
        {
            SaveViewerSettings();
            double speed = Math.Pow(2, playbackSpeedBar.Value);
            _renderer.SetPlayBackSpeed(speed);
            toolTip1.SetToolTip(playbackSpeedBar, "Playback speed: " + speed + "x");
            _renderer.UpdateReplaySettings();
            _renderer.RedrawScene();
        }

        private void RenderingSettingsButtonClick(object sender, EventArgs e)
        {
            var settingsForm = new RenderingSettingsForm(Global.AppSettings.ReplayViewer.RenderingSettings);
            settingsForm.Changed += x =>
                                        {
                                            _renderer.UpdateSettings(x);
                                            _renderer.RedrawScene();
                                        };
            settingsForm.ShowDialog();
        }

        private void SaveViewerSettings()
        {
            var settings = Global.AppSettings.ReplayViewer;
            settings.Size = Size;
            settings.FollowDriver = FollowDriverBox.Checked;
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
            ViewerBox.Paint += _renderer.RedrawScene;
            ZoomFillButton.MouseDown += _renderer.ZoomFill;
            PlayButton.MouseDown += _renderer.TogglePlay;
            StopButton.MouseDown += _renderer.StopPlaying;
            PrevFrameButton.MouseDown += _renderer.PreviousFrame;
            NextFrameButton.MouseDown += _renderer.NextFrame;
            ShowDriverPathBox.CheckedChanged += RenderingOptionsChanged;
            LockedCamBox.CheckedChanged += RenderingOptionsChanged;
            LoopPlayingBox.CheckedChanged += RenderingOptionsChanged;
            FollowDriverBox.CheckedChanged += RenderingOptionsChanged;
            PictBackGroundBox.CheckedChanged += RenderingOptionsChanged;
            TransparentInactiveBox.CheckedChanged += RenderingOptionsChanged;
            PlayerFramesBox.CheckedChanged += RenderingOptionsChanged;
            HideStartObjectBox.CheckedChanged += RenderingOptionsChanged;
            playbackSpeedBar.ValueChanged += RenderingOptionsChanged;
            multiSpyBox.CheckedChanged += RenderingOptionsChanged;
            playbackSpeedBar.MouseWheel += TrackBarScrolled;
            timeBar.MouseWheel += TrackBarScrolled;
            ResolutionBox.MouseWheel += TrackBarScrolled;
            TabControl1.KeyDown += KeyHandler;
            foreach (ToolStripMenuItem toolStripMenuItem in contextMenuStrip1.Items)
            {
                toolStripMenuItem.CheckedChanged += UpdateEventsLists;
            }
        }

        private void ShowCoordinates()
        {
            Vector x;
            if (Global.AppSettings.ReplayViewer.ShowBikeCoords)
            {
                x = _renderer.GetBikeCoordinates();
                CoordinateLabel.Text = "Bike X: " + x.X.ToString("F3") + " Y: " + x.Y.ToString("F3");
            }
            else
            {
                x = GetMouseCoordinates();
                CoordinateLabel.Text = "Mouse X: " + x.X.ToString("F3") + " Y: " + x.Y.ToString("F3");
            }
        }

        private void ShowTime(object sender, MouseEventArgs e)
        {
            if (e.Location == _lastLocation)
                return;
            toolTip1.SetToolTip(timeBar,
                                (_renderer.MaxTime * ((e.Location.X - 13) / (double) (timeBar.Width - 27))).ToTimeString
                                    ());
            _lastLocation = e.Location;
        }

        private void SnapShotButtonClick(object sender, EventArgs e)
        {
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                _renderer.SaveSnapShot(SaveFileDialog1.FileName);
        }

        private void TextBoxKeyPress(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && TimeBox.Text.Length == 9)
                UpdateCurrFrameFromTime(Utils.StringToTime(TimeBox.Text));
        }

        private void TrackBarScrolled(int delta)
        {
            Zoom(delta > 0, 1 - Global.AppSettings.ReplayViewer.MouseWheelStep / 100.0);
        }

        private void UpdateCurrFrameFromTime(double newTime)
        {
            newTime = Math.Max(newTime, 0);
            _renderer.CurrentTime = newTime;
            _renderer.DrawSceneDefault();
        }

        private void UpdateEventsLists(object sender = null, EventArgs e = null)
        {
            int i = GetSelectedIndex();
            if (i >= 0)
            {
                var typesToShow = new List<ReplayEventType>();
                if (applesToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.AppleTake);
                if (supervoltsToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.SuperVolt);
                if (leftVoltsToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.LeftVolt);
                if (rightVoltsToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.RightVolt);
                if (turnsToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.Turn);
                if (groundtouchesToolStripMenuItem.Checked)
                    typesToShow.Add(ReplayEventType.GroundTouch);
                _currentEvents = _playListReplays[i].Player.GetEvents(typesToShow.ToArray());
                Utils.PutEventsToList(_playListReplays[i].Player, EventListBox,
                                          _playListReplays[0].Player.Finished || _playListReplays[i].Player.FakeFinish,
                                          _currentEvents);
            }
            else
                EventListBox.Items.Clear();
            if (sender != null)
            {
                contextMenuStrip1.Show();
            }
        }

        private void ViewerClosing(object sender, CancelEventArgs e)
        {
            _renderer.Playing = false;
            DisplayDevice.Default.RestoreResolution();
            _renderer.Dispose();
            _renderer = null;
            SaveViewerSettings();
        }

        private void ViewerMouseDown(object sender, MouseEventArgs e)
        {
            Vector z = GetMouseCoordinates();
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        _zoomRecting = true;
                        _zoomRectStartPoint = z;
                    }
                    else
                        Zoom(e.Button == MouseButtons.Left,
                            1 - Global.AppSettings.ReplayViewer.MouseClickStep / 100.0);
                    break;
                case MouseButtons.Right:
                    Zoom(e.Button == MouseButtons.Left,
                        1 - Global.AppSettings.ReplayViewer.MouseClickStep / 100.0);
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
            Vector z = GetMouseCoordinates();
            if (_draggingScreen)
            {
                _renderer.CenterX = _moveStartPosition.X + (_renderer.XMax + _renderer.XMin) / 2 - z.X;
                _renderer.CenterY = _moveStartPosition.Y + (_renderer.YMax + _renderer.YMin) / 2 - z.Y;
                _renderer.RedrawScene();
            }
            else if (_zoomRecting)
                _renderer.RedrawScene();
        }

        private void ViewerMouseUp(object sender, MouseEventArgs e)
        {
            if (_zoomRecting)
            {
                _zoomRecting = false;
                _renderer.ZoomRect(_zoomRectStartPoint, GetMouseCoordinates());
            }
            _draggingScreen = false;
        }

        private void ViewerResized(object sender = null, EventArgs e = null)
        {
            if (sender!=null)
            {
                viewerSizeXBox.Text = ViewerBox.Width.ToString();
                viewerSizeYBox.Text = ViewerBox.Height.ToString();
            }
            _renderer.ResetViewport(ViewerBox.Width, ViewerBox.Height);
            _renderer.RedrawScene();
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
                    _renderer.ZoomLevel *= 1.05;
                }
                else
                {
                    _renderer.ZoomLevel /= 1.05;
                }
                _renderer.RedrawScene();
                _lastMouseX = MousePosition.X;
                Global.AppSettings.ReplayViewer.ZoomLevel = _renderer.ZoomLevel;
            }
        }

        private void ZoomButtonMouseUp(object sender, MouseEventArgs e)
        {
            _zooming = false;
        }

        private struct PlayListObject
        {
// ReSharper disable UnaccessedField.Local
            public string FileName;
            public int PlayerNum;
            internal Player Player;
// ReSharper restore UnaccessedField.Local

            internal PlayListObject(string fileName, int num, Player player)
            {
                FileName = fileName;
                PlayerNum = num;
                Player = player;
            }
        }
    }
}