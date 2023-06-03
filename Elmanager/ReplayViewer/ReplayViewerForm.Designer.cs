using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Elmanager.Rec;
using Elmanager.UI;
using OpenTK.WinForms;

namespace Elmanager.ReplayViewer
{
    internal partial class ReplayViewerForm
    {

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
                _replayController.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        //The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        private void InitializeComponent()
        {
            components = new Container();
            SaveFileDialog1 = new SaveFileDialog();
            ColorDialog1 = new ColorDialog();
            ViewerBox = new GLControl();
            contextMenuStrip1 = new ContextMenuStrip(components);
            applesToolStripMenuItem = new ToolStripMenuItem();
            leftVoltsToolStripMenuItem = new ToolStripMenuItem();
            rightVoltsToolStripMenuItem = new ToolStripMenuItem();
            supervoltsToolStripMenuItem = new ToolStripMenuItem();
            turnsToolStripMenuItem = new ToolStripMenuItem();
            groundtouchesToolStripMenuItem = new ToolStripMenuItem();
            gasOnToolStripMenuItem = new ToolStripMenuItem();
            gasOffToolStripMenuItem = new ToolStripMenuItem();
            toolTip1 = new ToolTip(components);
            playbackSpeedBar = new TrackBarMod();
            TabControl1 = new TabControlMod();
            TabPage1 = new TabPage();
            button1 = new Button();
            PlayList = new CustomObjectListView();
            OlvColumn1 = new OLVColumn();
            OlvColumn2 = new OLVColumn();
            FullScreenButton = new Button();
            SnapShotButton = new Button();
            EventListBox = new ListBox();
            SpeedLabel = new Label();
            PlayButton = new Button();
            StopButton = new Button();
            ZoomFillButton = new Button();
            TimeBox = new TextBox();
            PrevFrameButton = new Button();
            NextFrameButton = new Button();
            timeBar = new TrackBarMod();
            CoordinateLabel = new Label();
            TabPage2 = new TabPage();
            flowLayoutPanel = new FlowLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            ShowDriverPathBox = new CheckBox();
            followAlsoWhenZooming = new CheckBox();
            LockedCamBox = new CheckBox();
            FollowDriverBox = new CheckBox();
            LoopPlayingBox = new CheckBox();
            HideStartObjectBox = new CheckBox();
            multiSpyBox = new CheckBox();
            TransparentInactiveBox = new CheckBox();
            PictBackGroundBox = new CheckBox();
            SelectNoPlayersBox = new CheckBox();
            PlayerFramesBox = new CheckBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            Label4 = new Label();
            MouseWheelZoomBox = new NumericTextBox();
            viewerSizeYBox = new NumericTextBox();
            label2 = new Label();
            Label5 = new Label();
            MouseClickZoomBox = new NumericTextBox();
            viewerSizeXBox = new NumericTextBox();
            label1 = new Label();
            optionsPanel3 = new Panel();
            RenderingSettingsButton = new Button();
            ActivePlayerPanel = new Panel();
            InActivePlayerPanel = new Panel();
            DrivingLinePanel = new Panel();
            DrivingLineLabel = new Label();
            ActivePLabel = new Label();
            InactivePLabel = new Label();
            contextMenuStrip1.SuspendLayout();
            ((ISupportInitialize)playbackSpeedBar).BeginInit();
            TabControl1.SuspendLayout();
            TabPage1.SuspendLayout();
            ((ISupportInitialize)PlayList).BeginInit();
            ((ISupportInitialize)timeBar).BeginInit();
            TabPage2.SuspendLayout();
            flowLayoutPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            optionsPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // SaveFileDialog1
            // 
            SaveFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png";
            // 
            // ColorDialog1
            // 
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = true;
            // 
            // ViewerBox
            // 
            ViewerBox.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            ViewerBox.APIVersion = new Version(3, 3, 0, 0);
            ViewerBox.Dock = DockStyle.Fill;
            ViewerBox.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            ViewerBox.IsEventDriven = true;
            ViewerBox.Location = new Point(788, 0);
            ViewerBox.Margin = new Padding(6, 6, 6, 6);
            ViewerBox.Name = "ViewerBox";
            ViewerBox.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            ViewerBox.Size = new Size(678, 670);
            ViewerBox.TabIndex = 100;
            ViewerBox.MouseDown += ViewerMouseDown;
            ViewerBox.MouseMove += ViewerMouseMoving;
            ViewerBox.MouseUp += ViewerMouseUp;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(32, 32);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { applesToolStripMenuItem, leftVoltsToolStripMenuItem, rightVoltsToolStripMenuItem, supervoltsToolStripMenuItem, turnsToolStripMenuItem, groundtouchesToolStripMenuItem, gasOnToolStripMenuItem, gasOffToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(258, 324);
            // 
            // applesToolStripMenuItem
            // 
            applesToolStripMenuItem.Checked = true;
            applesToolStripMenuItem.CheckOnClick = true;
            applesToolStripMenuItem.CheckState = CheckState.Checked;
            applesToolStripMenuItem.Name = "applesToolStripMenuItem";
            applesToolStripMenuItem.Size = new Size(257, 40);
            applesToolStripMenuItem.Text = "Apples";
            // 
            // leftVoltsToolStripMenuItem
            // 
            leftVoltsToolStripMenuItem.CheckOnClick = true;
            leftVoltsToolStripMenuItem.Name = "leftVoltsToolStripMenuItem";
            leftVoltsToolStripMenuItem.Size = new Size(257, 40);
            leftVoltsToolStripMenuItem.Text = "Left volts";
            // 
            // rightVoltsToolStripMenuItem
            // 
            rightVoltsToolStripMenuItem.CheckOnClick = true;
            rightVoltsToolStripMenuItem.Name = "rightVoltsToolStripMenuItem";
            rightVoltsToolStripMenuItem.Size = new Size(257, 40);
            rightVoltsToolStripMenuItem.Text = "Right volts";
            // 
            // supervoltsToolStripMenuItem
            // 
            supervoltsToolStripMenuItem.CheckOnClick = true;
            supervoltsToolStripMenuItem.Name = "supervoltsToolStripMenuItem";
            supervoltsToolStripMenuItem.Size = new Size(257, 40);
            supervoltsToolStripMenuItem.Text = "Supervolts";
            // 
            // turnsToolStripMenuItem
            // 
            turnsToolStripMenuItem.CheckOnClick = true;
            turnsToolStripMenuItem.Name = "turnsToolStripMenuItem";
            turnsToolStripMenuItem.Size = new Size(257, 40);
            turnsToolStripMenuItem.Text = "Turns";
            // 
            // groundtouchesToolStripMenuItem
            // 
            groundtouchesToolStripMenuItem.CheckOnClick = true;
            groundtouchesToolStripMenuItem.Name = "groundtouchesToolStripMenuItem";
            groundtouchesToolStripMenuItem.Size = new Size(257, 40);
            groundtouchesToolStripMenuItem.Text = "Groundtouches";
            // 
            // gasOnToolStripMenuItem
            // 
            gasOnToolStripMenuItem.CheckOnClick = true;
            gasOnToolStripMenuItem.Name = "gasOnToolStripMenuItem";
            gasOnToolStripMenuItem.Size = new Size(257, 40);
            gasOnToolStripMenuItem.Text = "Gas on (approx)";
            // 
            // gasOffToolStripMenuItem
            // 
            gasOffToolStripMenuItem.CheckOnClick = true;
            gasOffToolStripMenuItem.Name = "gasOffToolStripMenuItem";
            gasOffToolStripMenuItem.Size = new Size(257, 40);
            gasOffToolStripMenuItem.Text = "Gas off (approx)";
            // 
            // toolTip1
            // 
            toolTip1.AutomaticDelay = 1;
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 300;
            toolTip1.ReshowDelay = 0;
            // 
            // playbackSpeedBar
            // 
            playbackSpeedBar.BackColor = Color.White;
            playbackSpeedBar.LargeChange = 10;
            playbackSpeedBar.Location = new Point(0, 70);
            playbackSpeedBar.Margin = new Padding(6, 6, 6, 6);
            playbackSpeedBar.Minimum = -10;
            playbackSpeedBar.Name = "playbackSpeedBar";
            playbackSpeedBar.Size = new Size(332, 90);
            playbackSpeedBar.TabIndex = 126;
            toolTip1.SetToolTip(playbackSpeedBar, "Playback speed: 1x");
            // 
            // TabControl1
            // 
            TabControl1.Controls.Add(TabPage1);
            TabControl1.Controls.Add(TabPage2);
            TabControl1.Dock = DockStyle.Left;
            TabControl1.Location = new Point(0, 0);
            TabControl1.Margin = new Padding(6, 6, 6, 6);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new Size(788, 670);
            TabControl1.TabIndex = 99;
            // 
            // TabPage1
            // 
            TabPage1.Controls.Add(button1);
            TabPage1.Controls.Add(playbackSpeedBar);
            TabPage1.Controls.Add(PlayList);
            TabPage1.Controls.Add(FullScreenButton);
            TabPage1.Controls.Add(SnapShotButton);
            TabPage1.Controls.Add(EventListBox);
            TabPage1.Controls.Add(SpeedLabel);
            TabPage1.Controls.Add(PlayButton);
            TabPage1.Controls.Add(StopButton);
            TabPage1.Controls.Add(ZoomFillButton);
            TabPage1.Controls.Add(TimeBox);
            TabPage1.Controls.Add(PrevFrameButton);
            TabPage1.Controls.Add(NextFrameButton);
            TabPage1.Controls.Add(timeBar);
            TabPage1.Controls.Add(CoordinateLabel);
            TabPage1.Location = new Point(8, 46);
            TabPage1.Margin = new Padding(6, 6, 6, 6);
            TabPage1.Name = "TabPage1";
            TabPage1.Padding = new Padding(6, 6, 6, 6);
            TabPage1.Size = new Size(772, 616);
            TabPage1.TabIndex = 0;
            TabPage1.Text = "Player";
            TabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(346, 128);
            button1.Margin = new Padding(6, 6, 6, 6);
            button1.Name = "button1";
            button1.Size = new Size(416, 82);
            button1.TabIndex = 127;
            button1.Text = "Click and hold to zoom in/out";
            button1.UseVisualStyleBackColor = true;
            button1.MouseDown += ZoomButtonMouseDown;
            button1.MouseMove += ZoomButtonMouseMove;
            button1.MouseUp += ZoomButtonMouseUp;
            // 
            // PlayList
            // 
            PlayList.AllColumns.Add(OlvColumn1);
            PlayList.AllColumns.Add(OlvColumn2);
            PlayList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            PlayList.CellEditUseWholeCell = false;
            PlayList.CheckBoxes = true;
            PlayList.Columns.AddRange(new ColumnHeader[] { OlvColumn1, OlvColumn2 });
            PlayList.FullRowSelect = true;
            PlayList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PlayList.Location = new Point(0, 384);
            PlayList.Margin = new Padding(6, 6, 6, 6);
            PlayList.Name = "PlayList";
            PlayList.ShowGroups = false;
            PlayList.ShowImagesOnSubItems = true;
            PlayList.Size = new Size(328, 212);
            PlayList.TabIndex = 100;
            PlayList.UseCompatibleStateImageBehavior = false;
            PlayList.UseOverlays = false;
            PlayList.View = View.Details;
            PlayList.SelectionChanged += PlayListSelectionChanged;
            PlayList.ItemChecked += PlayListSelectionChanged;
            PlayList.MouseWheel += MouseWheelZoom;
            // 
            // OlvColumn1
            // 
            OlvColumn1.AspectName = "";
            OlvColumn1.Text = "Filename";
            OlvColumn1.Width = 110;
            // 
            // OlvColumn2
            // 
            OlvColumn2.AspectName = "";
            OlvColumn2.Text = "Player";
            OlvColumn2.Width = 50;
            // 
            // FullScreenButton
            // 
            FullScreenButton.Location = new Point(546, 70);
            FullScreenButton.Margin = new Padding(6, 6, 6, 6);
            FullScreenButton.Name = "FullScreenButton";
            FullScreenButton.Size = new Size(216, 50);
            FullScreenButton.TabIndex = 99;
            FullScreenButton.Text = "Full screen";
            FullScreenButton.UseVisualStyleBackColor = true;
            FullScreenButton.Click += ToggleFullScreen;
            // 
            // SnapShotButton
            // 
            SnapShotButton.Location = new Point(346, 70);
            SnapShotButton.Margin = new Padding(6, 6, 6, 6);
            SnapShotButton.Name = "SnapShotButton";
            SnapShotButton.Size = new Size(186, 50);
            SnapShotButton.TabIndex = 98;
            SnapShotButton.Text = "Get snapshot";
            SnapShotButton.UseVisualStyleBackColor = true;
            SnapShotButton.Click += SnapShotButtonClick;
            // 
            // EventListBox
            // 
            EventListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            EventListBox.ContextMenuStrip = contextMenuStrip1;
            EventListBox.FormattingEnabled = true;
            EventListBox.IntegralHeight = false;
            EventListBox.ItemHeight = 32;
            EventListBox.Location = new Point(346, 384);
            EventListBox.Margin = new Padding(6, 6, 6, 6);
            EventListBox.Name = "EventListBox";
            EventListBox.Size = new Size(412, 210);
            EventListBox.TabIndex = 79;
            EventListBox.SelectedIndexChanged += EventListBoxSelectedIndexChanged;
            EventListBox.MouseWheel += MouseWheelZoom;
            // 
            // SpeedLabel
            // 
            SpeedLabel.AutoSize = true;
            SpeedLabel.Location = new Point(344, 216);
            SpeedLabel.Margin = new Padding(6, 0, 6, 0);
            SpeedLabel.Name = "SpeedLabel";
            SpeedLabel.Size = new Size(86, 32);
            SpeedLabel.TabIndex = 96;
            SpeedLabel.Text = "Speed:";
            // 
            // PlayButton
            // 
            PlayButton.Location = new Point(188, 14);
            PlayButton.Margin = new Padding(6, 6, 6, 6);
            PlayButton.Name = "PlayButton";
            PlayButton.Size = new Size(150, 50);
            PlayButton.TabIndex = 53;
            PlayButton.Text = "Play/Pause";
            PlayButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            StopButton.Location = new Point(618, 14);
            StopButton.Margin = new Padding(6, 6, 6, 6);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(144, 50);
            StopButton.TabIndex = 55;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = true;
            // 
            // ZoomFillButton
            // 
            ZoomFillButton.Location = new Point(0, 14);
            ZoomFillButton.Margin = new Padding(6, 6, 6, 6);
            ZoomFillButton.Name = "ZoomFillButton";
            ZoomFillButton.Size = new Size(168, 50);
            ZoomFillButton.TabIndex = 56;
            ZoomFillButton.Text = "Zoom fill";
            ZoomFillButton.UseVisualStyleBackColor = true;
            // 
            // TimeBox
            // 
            TimeBox.Location = new Point(420, 18);
            TimeBox.Margin = new Padding(6, 6, 6, 6);
            TimeBox.Name = "TimeBox";
            TimeBox.Size = new Size(108, 39);
            TimeBox.TabIndex = 60;
            TimeBox.Text = "00:00,000";
            TimeBox.KeyDown += KeyHandler;
            // 
            // PrevFrameButton
            // 
            PrevFrameButton.Location = new Point(346, 14);
            PrevFrameButton.Margin = new Padding(6, 6, 6, 6);
            PrevFrameButton.Name = "PrevFrameButton";
            PrevFrameButton.Size = new Size(58, 50);
            PrevFrameButton.TabIndex = 61;
            PrevFrameButton.Text = "<";
            PrevFrameButton.UseVisualStyleBackColor = true;
            // 
            // NextFrameButton
            // 
            NextFrameButton.Location = new Point(546, 14);
            NextFrameButton.Margin = new Padding(6, 6, 6, 6);
            NextFrameButton.Name = "NextFrameButton";
            NextFrameButton.Size = new Size(58, 50);
            NextFrameButton.TabIndex = 62;
            NextFrameButton.Text = ">";
            NextFrameButton.UseVisualStyleBackColor = true;
            // 
            // timeBar
            // 
            timeBar.BackColor = SystemColors.Window;
            timeBar.LargeChange = 10;
            timeBar.Location = new Point(0, 288);
            timeBar.Margin = new Padding(6, 6, 6, 6);
            timeBar.Maximum = 1000;
            timeBar.Name = "timeBar";
            timeBar.Size = new Size(768, 90);
            timeBar.TabIndex = 54;
            timeBar.TickStyle = TickStyle.None;
            timeBar.Scroll += PlayBarScroll;
            timeBar.MouseDown += Goto;
            timeBar.MouseMove += ShowTime;
            // 
            // CoordinateLabel
            // 
            CoordinateLabel.AutoSize = true;
            CoordinateLabel.Cursor = Cursors.Hand;
            CoordinateLabel.Location = new Point(344, 260);
            CoordinateLabel.Margin = new Padding(6, 0, 6, 0);
            CoordinateLabel.Name = "CoordinateLabel";
            CoordinateLabel.Size = new Size(223, 32);
            CoordinateLabel.TabIndex = 54;
            CoordinateLabel.Text = "Mouse coordinates:";
            CoordinateLabel.Click += CoordinateLabelClick;
            CoordinateLabel.MouseDown += CoordinateLabelClick;
            // 
            // TabPage2
            // 
            TabPage2.Controls.Add(flowLayoutPanel);
            TabPage2.Location = new Point(8, 46);
            TabPage2.Margin = new Padding(6, 6, 6, 6);
            TabPage2.Name = "TabPage2";
            TabPage2.Padding = new Padding(6, 6, 6, 6);
            TabPage2.Size = new Size(772, 616);
            TabPage2.TabIndex = 1;
            TabPage2.Text = "Options";
            TabPage2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel.Controls.Add(tableLayoutPanel1);
            flowLayoutPanel.Controls.Add(tableLayoutPanel2);
            flowLayoutPanel.Controls.Add(optionsPanel3);
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.Margin = new Padding(6, 6, 6, 6);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(644, 521);
            flowLayoutPanel.TabIndex = 0;
            flowLayoutPanel.WrapContents = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(ShowDriverPathBox, 0, 0);
            tableLayoutPanel1.Controls.Add(followAlsoWhenZooming, 0, 2);
            tableLayoutPanel1.Controls.Add(LockedCamBox, 1, 0);
            tableLayoutPanel1.Controls.Add(FollowDriverBox, 0, 1);
            tableLayoutPanel1.Controls.Add(LoopPlayingBox, 1, 1);
            tableLayoutPanel1.Controls.Add(HideStartObjectBox, 0, 5);
            tableLayoutPanel1.Controls.Add(multiSpyBox, 1, 4);
            tableLayoutPanel1.Controls.Add(TransparentInactiveBox, 1, 2);
            tableLayoutPanel1.Controls.Add(PictBackGroundBox, 0, 3);
            tableLayoutPanel1.Controls.Add(SelectNoPlayersBox, 1, 3);
            tableLayoutPanel1.Controls.Add(PlayerFramesBox, 0, 4);
            tableLayoutPanel1.Location = new Point(6, 6);
            tableLayoutPanel1.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(632, 216);
            tableLayoutPanel1.TabIndex = 147;
            // 
            // ShowDriverPathBox
            // 
            ShowDriverPathBox.AutoSize = true;
            ShowDriverPathBox.Location = new Point(0, 0);
            ShowDriverPathBox.Margin = new Padding(0);
            ShowDriverPathBox.Name = "ShowDriverPathBox";
            ShowDriverPathBox.Size = new Size(227, 36);
            ShowDriverPathBox.TabIndex = 52;
            ShowDriverPathBox.Text = "Show driver path";
            ShowDriverPathBox.UseVisualStyleBackColor = true;
            // 
            // followAlsoWhenZooming
            // 
            followAlsoWhenZooming.AutoSize = true;
            followAlsoWhenZooming.Location = new Point(0, 72);
            followAlsoWhenZooming.Margin = new Padding(0);
            followAlsoWhenZooming.Name = "followAlsoWhenZooming";
            followAlsoWhenZooming.Size = new Size(269, 36);
            followAlsoWhenZooming.TabIndex = 56;
            followAlsoWhenZooming.Text = "...also when zooming";
            followAlsoWhenZooming.UseVisualStyleBackColor = true;
            // 
            // LockedCamBox
            // 
            LockedCamBox.AutoSize = true;
            LockedCamBox.Location = new Point(290, 0);
            LockedCamBox.Margin = new Padding(0);
            LockedCamBox.Name = "LockedCamBox";
            LockedCamBox.Size = new Size(205, 36);
            LockedCamBox.TabIndex = 53;
            LockedCamBox.Text = "Locked camera";
            LockedCamBox.UseVisualStyleBackColor = true;
            // 
            // FollowDriverBox
            // 
            FollowDriverBox.AutoSize = true;
            FollowDriverBox.Location = new Point(0, 36);
            FollowDriverBox.Margin = new Padding(0);
            FollowDriverBox.Name = "FollowDriverBox";
            FollowDriverBox.Size = new Size(183, 36);
            FollowDriverBox.TabIndex = 54;
            FollowDriverBox.Text = "Follow driver";
            FollowDriverBox.UseVisualStyleBackColor = true;
            // 
            // LoopPlayingBox
            // 
            LoopPlayingBox.AutoSize = true;
            LoopPlayingBox.Location = new Point(290, 36);
            LoopPlayingBox.Margin = new Padding(0);
            LoopPlayingBox.Name = "LoopPlayingBox";
            LoopPlayingBox.Size = new Size(184, 36);
            LoopPlayingBox.TabIndex = 55;
            LoopPlayingBox.Text = "Loop playing";
            LoopPlayingBox.UseVisualStyleBackColor = true;
            // 
            // HideStartObjectBox
            // 
            HideStartObjectBox.AutoSize = true;
            HideStartObjectBox.Location = new Point(0, 180);
            HideStartObjectBox.Margin = new Padding(0);
            HideStartObjectBox.Name = "HideStartObjectBox";
            HideStartObjectBox.Size = new Size(222, 36);
            HideStartObjectBox.TabIndex = 62;
            HideStartObjectBox.Text = "Hide start object";
            HideStartObjectBox.UseVisualStyleBackColor = true;
            // 
            // multiSpyBox
            // 
            multiSpyBox.AutoSize = true;
            multiSpyBox.Location = new Point(290, 144);
            multiSpyBox.Margin = new Padding(0);
            multiSpyBox.Name = "multiSpyBox";
            multiSpyBox.Size = new Size(145, 36);
            multiSpyBox.TabIndex = 61;
            multiSpyBox.Text = "Multi spy";
            multiSpyBox.UseVisualStyleBackColor = true;
            // 
            // TransparentInactiveBox
            // 
            TransparentInactiveBox.AutoSize = true;
            TransparentInactiveBox.Location = new Point(290, 72);
            TransparentInactiveBox.Margin = new Padding(0);
            TransparentInactiveBox.Name = "TransparentInactiveBox";
            TransparentInactiveBox.Size = new Size(341, 36);
            TransparentInactiveBox.TabIndex = 57;
            TransparentInactiveBox.Text = "Transparent inactive players";
            TransparentInactiveBox.UseVisualStyleBackColor = true;
            // 
            // PictBackGroundBox
            // 
            PictBackGroundBox.AutoSize = true;
            PictBackGroundBox.Location = new Point(0, 108);
            PictBackGroundBox.Margin = new Padding(0);
            PictBackGroundBox.Name = "PictBackGroundBox";
            PictBackGroundBox.Size = new Size(290, 36);
            PictBackGroundBox.TabIndex = 58;
            PictBackGroundBox.Text = "Pictures in background";
            PictBackGroundBox.UseVisualStyleBackColor = true;
            // 
            // SelectNoPlayersBox
            // 
            SelectNoPlayersBox.AutoSize = true;
            SelectNoPlayersBox.Location = new Point(290, 108);
            SelectNoPlayersBox.Margin = new Padding(0);
            SelectNoPlayersBox.Name = "SelectNoPlayersBox";
            SelectNoPlayersBox.Size = new Size(342, 36);
            SelectNoPlayersBox.TabIndex = 59;
            SelectNoPlayersBox.Text = "Select no players by default";
            SelectNoPlayersBox.UseVisualStyleBackColor = true;
            // 
            // PlayerFramesBox
            // 
            PlayerFramesBox.AutoSize = true;
            PlayerFramesBox.Location = new Point(0, 144);
            PlayerFramesBox.Margin = new Padding(0);
            PlayerFramesBox.Name = "PlayerFramesBox";
            PlayerFramesBox.Size = new Size(242, 36);
            PlayerFramesBox.TabIndex = 60;
            PlayerFramesBox.Text = "Player frames only";
            PlayerFramesBox.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(Label4, 0, 0);
            tableLayoutPanel2.Controls.Add(MouseWheelZoomBox, 1, 0);
            tableLayoutPanel2.Controls.Add(viewerSizeYBox, 3, 2);
            tableLayoutPanel2.Controls.Add(label2, 2, 2);
            tableLayoutPanel2.Controls.Add(Label5, 0, 1);
            tableLayoutPanel2.Controls.Add(MouseClickZoomBox, 1, 1);
            tableLayoutPanel2.Controls.Add(viewerSizeXBox, 1, 2);
            tableLayoutPanel2.Controls.Add(label1, 0, 2);
            tableLayoutPanel2.Location = new Point(6, 234);
            tableLayoutPanel2.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(542, 153);
            tableLayoutPanel2.TabIndex = 148;
            // 
            // Label4
            // 
            Label4.Anchor = AnchorStyles.Right;
            Label4.AutoSize = true;
            Label4.Location = new Point(6, 9);
            Label4.Margin = new Padding(6, 0, 6, 0);
            Label4.Name = "Label4";
            Label4.Size = new Size(281, 32);
            Label4.TabIndex = 121;
            Label4.Text = "Mouse wheel zoom step:";
            // 
            // MouseWheelZoomBox
            // 
            MouseWheelZoomBox.BackColor = SystemColors.Window;
            MouseWheelZoomBox.DefaultValue = 0D;
            MouseWheelZoomBox.Location = new Point(299, 6);
            MouseWheelZoomBox.Margin = new Padding(6, 6, 6, 6);
            MouseWheelZoomBox.Name = "MouseWheelZoomBox";
            MouseWheelZoomBox.Size = new Size(94, 39);
            MouseWheelZoomBox.TabIndex = 63;
            MouseWheelZoomBox.Text = "1";
            MouseWheelZoomBox.TextChanged += MouseWheelZoomBoxTextChanged;
            // 
            // viewerSizeYBox
            // 
            viewerSizeYBox.BackColor = SystemColors.Window;
            viewerSizeYBox.DefaultValue = 0D;
            viewerSizeYBox.Location = new Point(442, 108);
            viewerSizeYBox.Margin = new Padding(6, 6, 6, 6);
            viewerSizeYBox.Name = "viewerSizeYBox";
            viewerSizeYBox.Size = new Size(94, 39);
            viewerSizeYBox.TabIndex = 66;
            viewerSizeYBox.Text = "1";
            viewerSizeYBox.KeyUp += ViewerSizeBoxKeyUp;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.AutoSize = true;
            label2.Location = new Point(405, 111);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(25, 32);
            label2.TabIndex = 145;
            label2.Text = "x";
            // 
            // Label5
            // 
            Label5.Anchor = AnchorStyles.Right;
            Label5.AutoSize = true;
            Label5.Location = new Point(23, 60);
            Label5.Margin = new Padding(6, 0, 6, 0);
            Label5.Name = "Label5";
            Label5.Size = new Size(264, 32);
            Label5.TabIndex = 123;
            Label5.Text = "Mouse click zoom step:";
            // 
            // MouseClickZoomBox
            // 
            MouseClickZoomBox.BackColor = SystemColors.Window;
            MouseClickZoomBox.DefaultValue = 0D;
            MouseClickZoomBox.Location = new Point(299, 57);
            MouseClickZoomBox.Margin = new Padding(6, 6, 6, 6);
            MouseClickZoomBox.Name = "MouseClickZoomBox";
            MouseClickZoomBox.Size = new Size(94, 39);
            MouseClickZoomBox.TabIndex = 64;
            MouseClickZoomBox.Text = "1";
            MouseClickZoomBox.TextChanged += MouseClickZoomBoxTextChanged;
            // 
            // viewerSizeXBox
            // 
            viewerSizeXBox.BackColor = SystemColors.Window;
            viewerSizeXBox.DefaultValue = 0D;
            viewerSizeXBox.Location = new Point(299, 108);
            viewerSizeXBox.Margin = new Padding(6, 6, 6, 6);
            viewerSizeXBox.Name = "viewerSizeXBox";
            viewerSizeXBox.Size = new Size(94, 39);
            viewerSizeXBox.TabIndex = 65;
            viewerSizeXBox.Text = "1";
            viewerSizeXBox.KeyUp += ViewerSizeBoxKeyUp;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(149, 111);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(138, 32);
            label1.TabIndex = 142;
            label1.Text = "Viewer size:";
            // 
            // optionsPanel3
            // 
            optionsPanel3.AutoSize = true;
            optionsPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            optionsPanel3.Controls.Add(RenderingSettingsButton);
            optionsPanel3.Controls.Add(ActivePlayerPanel);
            optionsPanel3.Controls.Add(InActivePlayerPanel);
            optionsPanel3.Controls.Add(DrivingLinePanel);
            optionsPanel3.Controls.Add(DrivingLineLabel);
            optionsPanel3.Controls.Add(ActivePLabel);
            optionsPanel3.Controls.Add(InactivePLabel);
            optionsPanel3.Location = new Point(6, 399);
            optionsPanel3.Margin = new Padding(6, 6, 6, 6);
            optionsPanel3.Name = "optionsPanel3";
            optionsPanel3.Size = new Size(607, 116);
            optionsPanel3.TabIndex = 149;
            // 
            // RenderingSettingsButton
            // 
            RenderingSettingsButton.Location = new Point(240, 60);
            RenderingSettingsButton.Margin = new Padding(6, 6, 6, 6);
            RenderingSettingsButton.Name = "RenderingSettingsButton";
            RenderingSettingsButton.Size = new Size(230, 50);
            RenderingSettingsButton.TabIndex = 71;
            RenderingSettingsButton.Text = "Rendering settings";
            RenderingSettingsButton.UseVisualStyleBackColor = true;
            RenderingSettingsButton.Click += RenderingSettingsButtonClick;
            // 
            // ActivePlayerPanel
            // 
            ActivePlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            ActivePlayerPanel.Cursor = Cursors.Hand;
            ActivePlayerPanel.Location = new Point(0, 0);
            ActivePlayerPanel.Margin = new Padding(6, 6, 6, 6);
            ActivePlayerPanel.Name = "ActivePlayerPanel";
            ActivePlayerPanel.Size = new Size(38, 38);
            ActivePlayerPanel.TabIndex = 132;
            ActivePlayerPanel.Click += ChangeRectColor;
            // 
            // InActivePlayerPanel
            // 
            InActivePlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            InActivePlayerPanel.Cursor = Cursors.Hand;
            InActivePlayerPanel.Location = new Point(0, 66);
            InActivePlayerPanel.Margin = new Padding(6, 6, 6, 6);
            InActivePlayerPanel.Name = "InActivePlayerPanel";
            InActivePlayerPanel.Size = new Size(38, 38);
            InActivePlayerPanel.TabIndex = 133;
            InActivePlayerPanel.Click += ChangeRectColor;
            // 
            // DrivingLinePanel
            // 
            DrivingLinePanel.BorderStyle = BorderStyle.FixedSingle;
            DrivingLinePanel.Cursor = Cursors.Hand;
            DrivingLinePanel.Location = new Point(240, 0);
            DrivingLinePanel.Margin = new Padding(6, 6, 6, 6);
            DrivingLinePanel.Name = "DrivingLinePanel";
            DrivingLinePanel.Size = new Size(38, 38);
            DrivingLinePanel.TabIndex = 134;
            DrivingLinePanel.Click += ChangeRectColor;
            // 
            // DrivingLineLabel
            // 
            DrivingLineLabel.AutoSize = true;
            DrivingLineLabel.Location = new Point(292, 6);
            DrivingLineLabel.Margin = new Padding(6, 0, 6, 0);
            DrivingLineLabel.Name = "DrivingLineLabel";
            DrivingLineLabel.Size = new Size(309, 32);
            DrivingLineLabel.TabIndex = 131;
            DrivingLineLabel.Text = "Current player's driving line";
            // 
            // ActivePLabel
            // 
            ActivePLabel.AutoSize = true;
            ActivePLabel.Location = new Point(52, 6);
            ActivePLabel.Margin = new Padding(6, 0, 6, 0);
            ActivePLabel.Name = "ActivePLabel";
            ActivePLabel.Size = new Size(151, 32);
            ActivePLabel.TabIndex = 129;
            ActivePLabel.Text = "Active player";
            // 
            // InactivePLabel
            // 
            InactivePLabel.AutoSize = true;
            InactivePLabel.Location = new Point(52, 72);
            InactivePLabel.Margin = new Padding(6, 0, 6, 0);
            InactivePLabel.Name = "InactivePLabel";
            InactivePLabel.Size = new Size(168, 32);
            InactivePLabel.TabIndex = 130;
            InactivePLabel.Text = "Inactive player";
            // 
            // ReplayViewerForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1466, 670);
            Controls.Add(ViewerBox);
            Controls.Add(TabControl1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            KeyPreview = true;
            Margin = new Padding(6, 6, 6, 6);
            MinimumSize = new Size(1472, 671);
            Name = "ReplayViewerForm";
            Text = "Replay viewer";
            FormClosing += ViewerClosing;
            KeyDown += KeyHandler;
            MouseWheel += MouseWheelZoom;
            contextMenuStrip1.ResumeLayout(false);
            ((ISupportInitialize)playbackSpeedBar).EndInit();
            TabControl1.ResumeLayout(false);
            TabPage1.ResumeLayout(false);
            TabPage1.PerformLayout();
            ((ISupportInitialize)PlayList).EndInit();
            ((ISupportInitialize)timeBar).EndInit();
            TabPage2.ResumeLayout(false);
            TabPage2.PerformLayout();
            flowLayoutPanel.ResumeLayout(false);
            flowLayoutPanel.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            optionsPanel3.ResumeLayout(false);
            optionsPanel3.PerformLayout();
            ResumeLayout(false);
        }

        internal Label CoordinateLabel;
        internal TrackBarMod timeBar;
        internal CheckBox LoopPlayingBox;
        internal Button NextFrameButton;
        internal Button PrevFrameButton;
        internal TextBox TimeBox;
        internal Button ZoomFillButton;
        internal Button StopButton;
        internal CheckBox ShowDriverPathBox;
        internal CheckBox FollowDriverBox;
        internal Button PlayButton;
        internal SaveFileDialog SaveFileDialog1;
        internal ColorDialog ColorDialog1;
        internal ListBox EventListBox;
        internal Label SpeedLabel;
        internal TabControlMod TabControl1;
        internal TabPage TabPage1;
        internal TabPage TabPage2;
        internal Button SnapShotButton;
        internal Button FullScreenButton;
        internal CheckBox LockedCamBox;
        internal CheckBox TransparentInactiveBox;
        internal CheckBox PictBackGroundBox;
        internal GLControl ViewerBox;
        internal TrackBarMod playbackSpeedBar;
        internal Label Label5;
        internal Label Label4;
        internal Panel ActivePlayerPanel;
        internal Panel InActivePlayerPanel;
        internal Panel DrivingLinePanel;
        internal Label DrivingLineLabel;
        internal Label ActivePLabel;
        internal Label InactivePLabel;
        internal Button RenderingSettingsButton;
        internal CheckBox PlayerFramesBox;
        internal NumericTextBox MouseClickZoomBox;
        internal NumericTextBox MouseWheelZoomBox;
        internal CheckBox SelectNoPlayersBox;
        internal OLVColumn OlvColumn1;
        internal OLVColumn OlvColumn2;
        internal CheckBox HideStartObjectBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem applesToolStripMenuItem;
        private ToolStripMenuItem leftVoltsToolStripMenuItem;
        private ToolStripMenuItem rightVoltsToolStripMenuItem;
        private ToolStripMenuItem supervoltsToolStripMenuItem;
        private ToolStripMenuItem turnsToolStripMenuItem;
        private ToolStripMenuItem groundtouchesToolStripMenuItem;
        private ToolStripMenuItem gasOnToolStripMenuItem;
        private ToolStripMenuItem gasOffToolStripMenuItem;
        private IContainer components;
        private ToolTip toolTip1;
        private Button button1;
        private CheckBox multiSpyBox;
        private Label label2;
        internal NumericTextBox viewerSizeYBox;
        internal NumericTextBox viewerSizeXBox;
        private Label label1;
        internal CheckBox followAlsoWhenZooming;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel optionsPanel3;
        private FlowLayoutPanel flowLayoutPanel;
        internal CustomObjectListView PlayList;
    }

}
