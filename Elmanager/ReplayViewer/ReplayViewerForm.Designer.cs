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
            this.components = new Container();
            this.SaveFileDialog1 = new SaveFileDialog();
            this.ColorDialog1 = new ColorDialog();
            this.ViewerBox = new GLControl();
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.applesToolStripMenuItem = new ToolStripMenuItem();
            this.leftVoltsToolStripMenuItem = new ToolStripMenuItem();
            this.rightVoltsToolStripMenuItem = new ToolStripMenuItem();
            this.supervoltsToolStripMenuItem = new ToolStripMenuItem();
            this.turnsToolStripMenuItem = new ToolStripMenuItem();
            this.groundtouchesToolStripMenuItem = new ToolStripMenuItem();
            this.gasOnToolStripMenuItem = new ToolStripMenuItem();
            this.gasOffToolStripMenuItem = new ToolStripMenuItem();
            this.toolTip1 = new ToolTip(this.components);
            this.playbackSpeedBar = new TrackBarMod();
            this.TabControl1 = new TabControlMod();
            this.TabPage1 = new TabPage();
            this.button1 = new Button();
            this.PlayList = new CustomObjectListView();
            this.OlvColumn1 = ((OLVColumn)(new OLVColumn()));
            this.OlvColumn2 = ((OLVColumn)(new OLVColumn()));
            this.FullScreenButton = new Button();
            this.SnapShotButton = new Button();
            this.EventListBox = new ListBox();
            this.SpeedLabel = new Label();
            this.PlayButton = new Button();
            this.StopButton = new Button();
            this.ZoomFillButton = new Button();
            this.TimeBox = new TextBox();
            this.PrevFrameButton = new Button();
            this.NextFrameButton = new Button();
            this.timeBar = new TrackBarMod();
            this.CoordinateLabel = new Label();
            this.TabPage2 = new TabPage();
            this.followAlsoWhenZooming = new CheckBox();
            this.label2 = new Label();
            this.viewerSizeYBox = new NumericTextBox();
            this.viewerSizeXBox = new NumericTextBox();
            this.label1 = new Label();
            this.multiSpyBox = new CheckBox();
            this.HideStartObjectBox = new CheckBox();
            this.SelectNoPlayersBox = new CheckBox();
            this.MouseClickZoomBox = new NumericTextBox();
            this.MouseWheelZoomBox = new NumericTextBox();
            this.PlayerFramesBox = new CheckBox();
            this.RenderingSettingsButton = new Button();
            this.ActivePlayerPanel = new Panel();
            this.InActivePlayerPanel = new Panel();
            this.DrivingLinePanel = new Panel();
            this.DrivingLineLabel = new Label();
            this.ActivePLabel = new Label();
            this.InactivePLabel = new Label();
            this.Label5 = new Label();
            this.Label4 = new Label();
            this.TransparentInactiveBox = new CheckBox();
            this.PictBackGroundBox = new CheckBox();
            this.LockedCamBox = new CheckBox();
            this.FollowDriverBox = new CheckBox();
            this.ShowDriverPathBox = new CheckBox();
            this.LoopPlayingBox = new CheckBox();
            this.flowLayoutPanel = new FlowLayoutPanel();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.optionsPanel3 = new Panel();
            this.contextMenuStrip1.SuspendLayout();
            ((ISupportInitialize)(this.playbackSpeedBar)).BeginInit();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            ((ISupportInitialize)(this.PlayList)).BeginInit();
            ((ISupportInitialize)(this.timeBar)).BeginInit();
            this.TabPage2.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.optionsPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png";
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.AnyColor = true;
            this.ColorDialog1.FullOpen = true;
            // 
            // ViewerBox
            // 
            this.ViewerBox.Dock = DockStyle.Fill;
            this.ViewerBox.Location = new Point(394, 0);
            this.ViewerBox.Name = "ViewerBox";
            this.ViewerBox.Size = new Size(339, 335);
            this.ViewerBox.TabIndex = 100;
            this.ViewerBox.MouseDown += new MouseEventHandler(this.ViewerMouseDown);
            this.ViewerBox.MouseMove += new MouseEventHandler(this.ViewerMouseMoving);
            this.ViewerBox.MouseUp += new MouseEventHandler(this.ViewerMouseUp);
            this.ViewerBox.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.ViewerBox.APIVersion = new System.Version(3, 3, 0, 0);
            this.ViewerBox.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.ViewerBox.IsEventDriven = true;
            this.ViewerBox.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;

            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            this.applesToolStripMenuItem,
            this.leftVoltsToolStripMenuItem,
            this.rightVoltsToolStripMenuItem,
            this.supervoltsToolStripMenuItem,
            this.turnsToolStripMenuItem,
            this.groundtouchesToolStripMenuItem,
            this.gasOnToolStripMenuItem,
            this.gasOffToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new Size(157, 136);
            // 
            // applesToolStripMenuItem
            // 
            this.applesToolStripMenuItem.Checked = true;
            this.applesToolStripMenuItem.CheckOnClick = true;
            this.applesToolStripMenuItem.CheckState = CheckState.Checked;
            this.applesToolStripMenuItem.Name = "applesToolStripMenuItem";
            this.applesToolStripMenuItem.Size = new Size(156, 22);
            this.applesToolStripMenuItem.Text = "Apples";
            // 
            // leftVoltsToolStripMenuItem
            // 
            this.leftVoltsToolStripMenuItem.CheckOnClick = true;
            this.leftVoltsToolStripMenuItem.Name = "leftVoltsToolStripMenuItem";
            this.leftVoltsToolStripMenuItem.Size = new Size(156, 22);
            this.leftVoltsToolStripMenuItem.Text = "Left volts";
            // 
            // rightVoltsToolStripMenuItem
            // 
            this.rightVoltsToolStripMenuItem.CheckOnClick = true;
            this.rightVoltsToolStripMenuItem.Name = "rightVoltsToolStripMenuItem";
            this.rightVoltsToolStripMenuItem.Size = new Size(156, 22);
            this.rightVoltsToolStripMenuItem.Text = "Right volts";
            // 
            // supervoltsToolStripMenuItem
            // 
            this.supervoltsToolStripMenuItem.CheckOnClick = true;
            this.supervoltsToolStripMenuItem.Name = "supervoltsToolStripMenuItem";
            this.supervoltsToolStripMenuItem.Size = new Size(156, 22);
            this.supervoltsToolStripMenuItem.Text = "Supervolts";
            // 
            // turnsToolStripMenuItem
            // 
            this.turnsToolStripMenuItem.CheckOnClick = true;
            this.turnsToolStripMenuItem.Name = "turnsToolStripMenuItem";
            this.turnsToolStripMenuItem.Size = new Size(156, 22);
            this.turnsToolStripMenuItem.Text = "Turns";
            // 
            // groundtouchesToolStripMenuItem
            // 
            this.groundtouchesToolStripMenuItem.CheckOnClick = true;
            this.groundtouchesToolStripMenuItem.Name = "groundtouchesToolStripMenuItem";
            this.groundtouchesToolStripMenuItem.Size = new Size(156, 22);
            this.groundtouchesToolStripMenuItem.Text = "Groundtouches";
            // 
            // gasOnToolStripMenuItem
            // 
            this.gasOnToolStripMenuItem.CheckOnClick = true;
            this.gasOnToolStripMenuItem.Name = "gasOnToolStripMenuItem";
            this.gasOnToolStripMenuItem.Size = new Size(156, 22);
            this.gasOnToolStripMenuItem.Text = "Gas on (approx)";
            // 
            // gasOffToolStripMenuItem
            // 
            this.gasOffToolStripMenuItem.CheckOnClick = true;
            this.gasOffToolStripMenuItem.Name = "gasOffToolStripMenuItem";
            this.gasOffToolStripMenuItem.Size = new Size(156, 22);
            this.gasOffToolStripMenuItem.Text = "Gas off (approx)";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 1;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 0;
            // 
            // playbackSpeedBar
            // 
            this.playbackSpeedBar.BackColor = Color.White;
            this.playbackSpeedBar.LargeChange = 10;
            this.playbackSpeedBar.Location = new Point(0, 35);
            this.playbackSpeedBar.Minimum = -10;
            this.playbackSpeedBar.Name = "playbackSpeedBar";
            this.playbackSpeedBar.Size = new Size(166, 45);
            this.playbackSpeedBar.TabIndex = 126;
            this.toolTip1.SetToolTip(this.playbackSpeedBar, "Playback speed: 1x");
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Dock = DockStyle.Left;
            this.TabControl1.Location = new Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new Size(394, 335);
            this.TabControl1.TabIndex = 99;
            // 
            // TabPage1
            // 
            this.TabPage1.Controls.Add(this.button1);
            this.TabPage1.Controls.Add(this.playbackSpeedBar);
            this.TabPage1.Controls.Add(this.PlayList);
            this.TabPage1.Controls.Add(this.FullScreenButton);
            this.TabPage1.Controls.Add(this.SnapShotButton);
            this.TabPage1.Controls.Add(this.EventListBox);
            this.TabPage1.Controls.Add(this.SpeedLabel);
            this.TabPage1.Controls.Add(this.PlayButton);
            this.TabPage1.Controls.Add(this.StopButton);
            this.TabPage1.Controls.Add(this.ZoomFillButton);
            this.TabPage1.Controls.Add(this.TimeBox);
            this.TabPage1.Controls.Add(this.PrevFrameButton);
            this.TabPage1.Controls.Add(this.NextFrameButton);
            this.TabPage1.Controls.Add(this.timeBar);
            this.TabPage1.Controls.Add(this.CoordinateLabel);
            this.TabPage1.Location = new Point(4, 24);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new Padding(3);
            this.TabPage1.Size = new Size(386, 307);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Player";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new Point(173, 64);
            this.button1.Name = "button1";
            this.button1.Size = new Size(208, 41);
            this.button1.TabIndex = 127;
            this.button1.Text = "Click and hold to zoom in/out";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new MouseEventHandler(this.ZoomButtonMouseDown);
            this.button1.MouseMove += new MouseEventHandler(this.ZoomButtonMouseMove);
            this.button1.MouseUp += new MouseEventHandler(this.ZoomButtonMouseUp);
            // 
            // PlayList
            // 
            this.PlayList.AllColumns.Add(this.OlvColumn1);
            this.PlayList.AllColumns.Add(this.OlvColumn2);
            this.PlayList.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
                                                    | AnchorStyles.Left)));
            this.PlayList.CellEditUseWholeCell = false;
            this.PlayList.CheckBoxes = true;
            this.PlayList.Columns.AddRange(new ColumnHeader[] {
            this.OlvColumn1,
            this.OlvColumn2});
            this.PlayList.Cursor = Cursors.Default;
            this.PlayList.FullRowSelect = true;
            this.PlayList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.PlayList.HideSelection = false;
            this.PlayList.Location = new Point(0, 192);
            this.PlayList.Name = "PlayList";
            this.PlayList.ShowGroups = false;
            this.PlayList.ShowImagesOnSubItems = true;
            this.PlayList.Size = new Size(166, 108);
            this.PlayList.TabIndex = 100;
            this.PlayList.UseCompatibleStateImageBehavior = false;
            this.PlayList.UseOverlays = false;
            this.PlayList.View = View.Details;
            this.PlayList.SelectionChanged += new EventHandler(this.PlayListSelectionChanged);
            this.PlayList.ItemChecked += new ItemCheckedEventHandler(this.PlayListSelectionChanged);
            this.PlayList.MouseWheel += new MouseEventHandler(this.MouseWheelZoom);
            // 
            // OlvColumn1
            // 
            this.OlvColumn1.AspectName = "";
            this.OlvColumn1.Text = "Filename";
            this.OlvColumn1.Width = 110;
            // 
            // OlvColumn2
            // 
            this.OlvColumn2.AspectName = "";
            this.OlvColumn2.Text = "Player";
            this.OlvColumn2.Width = 50;
            // 
            // FullScreenButton
            // 
            this.FullScreenButton.Location = new Point(273, 35);
            this.FullScreenButton.Name = "FullScreenButton";
            this.FullScreenButton.Size = new Size(108, 25);
            this.FullScreenButton.TabIndex = 99;
            this.FullScreenButton.Text = "Full screen";
            this.FullScreenButton.UseVisualStyleBackColor = true;
            this.FullScreenButton.Click += new EventHandler(this.FullScreen);
            // 
            // SnapShotButton
            // 
            this.SnapShotButton.Location = new Point(173, 35);
            this.SnapShotButton.Name = "SnapShotButton";
            this.SnapShotButton.Size = new Size(93, 25);
            this.SnapShotButton.TabIndex = 98;
            this.SnapShotButton.Text = "Get snapshot";
            this.SnapShotButton.UseVisualStyleBackColor = true;
            this.SnapShotButton.Click += new EventHandler(this.SnapShotButtonClick);
            // 
            // EventListBox
            // 
            this.EventListBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
                                                        | AnchorStyles.Left)));
            this.EventListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.EventListBox.FormattingEnabled = true;
            this.EventListBox.IntegralHeight = false;
            this.EventListBox.ItemHeight = 15;
            this.EventListBox.Location = new Point(173, 192);
            this.EventListBox.Name = "EventListBox";
            this.EventListBox.Size = new Size(208, 107);
            this.EventListBox.TabIndex = 79;
            this.EventListBox.SelectedIndexChanged += new EventHandler(this.EventListBoxSelectedIndexChanged);
            this.EventListBox.MouseWheel += new MouseEventHandler(this.MouseWheelZoom);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new Point(172, 108);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new Size(42, 15);
            this.SpeedLabel.TabIndex = 96;
            this.SpeedLabel.Text = "Speed:";
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new Point(94, 7);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new Size(75, 25);
            this.PlayButton.TabIndex = 53;
            this.PlayButton.Text = "Play/Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Location = new Point(309, 7);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new Size(72, 25);
            this.StopButton.TabIndex = 55;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // ZoomFillButton
            // 
            this.ZoomFillButton.Location = new Point(0, 7);
            this.ZoomFillButton.Name = "ZoomFillButton";
            this.ZoomFillButton.Size = new Size(84, 25);
            this.ZoomFillButton.TabIndex = 56;
            this.ZoomFillButton.Text = "Zoom fill";
            this.ZoomFillButton.UseVisualStyleBackColor = true;
            // 
            // TimeBox
            // 
            this.TimeBox.Location = new Point(210, 9);
            this.TimeBox.Name = "TimeBox";
            this.TimeBox.Size = new Size(56, 23);
            this.TimeBox.TabIndex = 60;
            this.TimeBox.Text = "00:00,000";
            this.TimeBox.KeyDown += new KeyEventHandler(this.KeyHandler);
            // 
            // PrevFrameButton
            // 
            this.PrevFrameButton.Location = new Point(173, 7);
            this.PrevFrameButton.Name = "PrevFrameButton";
            this.PrevFrameButton.Size = new Size(29, 25);
            this.PrevFrameButton.TabIndex = 61;
            this.PrevFrameButton.Text = "<";
            this.PrevFrameButton.UseVisualStyleBackColor = true;
            // 
            // NextFrameButton
            // 
            this.NextFrameButton.Location = new Point(273, 7);
            this.NextFrameButton.Name = "NextFrameButton";
            this.NextFrameButton.Size = new Size(29, 25);
            this.NextFrameButton.TabIndex = 62;
            this.NextFrameButton.Text = ">";
            this.NextFrameButton.UseVisualStyleBackColor = true;
            // 
            // timeBar
            // 
            this.timeBar.BackColor = SystemColors.Window;
            this.timeBar.LargeChange = 10;
            this.timeBar.Location = new Point(0, 144);
            this.timeBar.Maximum = 1000;
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new Size(384, 45);
            this.timeBar.TabIndex = 54;
            this.timeBar.TickStyle = TickStyle.None;
            this.timeBar.Scroll += new EventHandler(this.PlayBarScroll);
            this.timeBar.MouseDown += new MouseEventHandler(this.Goto);
            this.timeBar.MouseMove += new MouseEventHandler(this.ShowTime);
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.AutoSize = true;
            this.CoordinateLabel.Cursor = Cursors.Hand;
            this.CoordinateLabel.Location = new Point(172, 130);
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new Size(111, 15);
            this.CoordinateLabel.TabIndex = 54;
            this.CoordinateLabel.Text = "Mouse coordinates:";
            this.CoordinateLabel.Click += new EventHandler(this.CoordinateLabelClick);
            this.CoordinateLabel.MouseDown += new MouseEventHandler(this.CoordinateLabelClick);
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.flowLayoutPanel);
            this.TabPage2.Location = new Point(4, 24);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new Padding(3);
            this.TabPage2.Size = new Size(386, 307);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Options";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // followAlsoWhenZooming
            // 
            this.followAlsoWhenZooming.AutoSize = true;
            this.followAlsoWhenZooming.Location = new Point(0, 38);
            this.followAlsoWhenZooming.Margin = new Padding(0);
            this.followAlsoWhenZooming.Name = "followAlsoWhenZooming";
            this.followAlsoWhenZooming.Size = new Size(138, 19);
            this.followAlsoWhenZooming.TabIndex = 56;
            this.followAlsoWhenZooming.Text = "...also when zooming";
            this.followAlsoWhenZooming.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(202, 65);
            this.label2.Name = "label2";
            this.label2.Size = new Size(13, 15);
            this.label2.TabIndex = 145;
            this.label2.Text = "x";
            // 
            // viewerSizeYBox
            // 
            this.viewerSizeYBox.BackColor = SystemColors.Window;
            this.viewerSizeYBox.DefaultValue = 0D;
            this.viewerSizeYBox.Location = new Point(221, 61);
            this.viewerSizeYBox.Name = "viewerSizeYBox";
            this.viewerSizeYBox.Size = new Size(49, 23);
            this.viewerSizeYBox.TabIndex = 66;
            this.viewerSizeYBox.Text = "1";
            this.viewerSizeYBox.KeyUp += new KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // viewerSizeXBox
            // 
            this.viewerSizeXBox.BackColor = SystemColors.Window;
            this.viewerSizeXBox.DefaultValue = 0D;
            this.viewerSizeXBox.Location = new Point(147, 61);
            this.viewerSizeXBox.Name = "viewerSizeXBox";
            this.viewerSizeXBox.Size = new Size(49, 23);
            this.viewerSizeXBox.TabIndex = 65;
            this.viewerSizeXBox.Text = "1";
            this.viewerSizeXBox.KeyUp += new KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // label1
            // 
            this.label1.Anchor = AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(74, 65);
            this.label1.Name = "label1";
            this.label1.Size = new Size(67, 15);
            this.label1.TabIndex = 142;
            this.label1.Text = "Viewer size:";
            // 
            // multiSpyBox
            // 
            this.multiSpyBox.AutoSize = true;
            this.multiSpyBox.Location = new Point(148, 76);
            this.multiSpyBox.Margin = new Padding(0);
            this.multiSpyBox.Name = "multiSpyBox";
            this.multiSpyBox.Size = new Size(75, 19);
            this.multiSpyBox.TabIndex = 61;
            this.multiSpyBox.Text = "Multi spy";
            this.multiSpyBox.UseVisualStyleBackColor = true;
            // 
            // HideStartObjectBox
            // 
            this.HideStartObjectBox.AutoSize = true;
            this.HideStartObjectBox.Location = new Point(0, 95);
            this.HideStartObjectBox.Margin = new Padding(0);
            this.HideStartObjectBox.Name = "HideStartObjectBox";
            this.HideStartObjectBox.Size = new Size(113, 19);
            this.HideStartObjectBox.TabIndex = 62;
            this.HideStartObjectBox.Text = "Hide start object";
            this.HideStartObjectBox.UseVisualStyleBackColor = true;
            // 
            // SelectNoPlayersBox
            // 
            this.SelectNoPlayersBox.AutoSize = true;
            this.SelectNoPlayersBox.Location = new Point(148, 57);
            this.SelectNoPlayersBox.Margin = new Padding(0);
            this.SelectNoPlayersBox.Name = "SelectNoPlayersBox";
            this.SelectNoPlayersBox.Size = new Size(170, 19);
            this.SelectNoPlayersBox.TabIndex = 59;
            this.SelectNoPlayersBox.Text = "Select no players by default";
            this.SelectNoPlayersBox.UseVisualStyleBackColor = true;
            // 
            // MouseClickZoomBox
            // 
            this.MouseClickZoomBox.BackColor = SystemColors.Window;
            this.MouseClickZoomBox.DefaultValue = 0D;
            this.MouseClickZoomBox.Location = new Point(147, 32);
            this.MouseClickZoomBox.Name = "MouseClickZoomBox";
            this.MouseClickZoomBox.Size = new Size(49, 23);
            this.MouseClickZoomBox.TabIndex = 64;
            this.MouseClickZoomBox.Text = "1";
            this.MouseClickZoomBox.TextChanged += new EventHandler(this.MouseClickZoomBoxTextChanged);
            // 
            // MouseWheelZoomBox
            // 
            this.MouseWheelZoomBox.BackColor = SystemColors.Window;
            this.MouseWheelZoomBox.DefaultValue = 0D;
            this.MouseWheelZoomBox.Location = new Point(147, 3);
            this.MouseWheelZoomBox.Name = "MouseWheelZoomBox";
            this.MouseWheelZoomBox.Size = new Size(49, 23);
            this.MouseWheelZoomBox.TabIndex = 63;
            this.MouseWheelZoomBox.Text = "1";
            this.MouseWheelZoomBox.TextChanged += new EventHandler(this.MouseWheelZoomBoxTextChanged);
            // 
            // PlayerFramesBox
            // 
            this.PlayerFramesBox.AutoSize = true;
            this.PlayerFramesBox.Location = new Point(0, 76);
            this.PlayerFramesBox.Margin = new Padding(0);
            this.PlayerFramesBox.Name = "PlayerFramesBox";
            this.PlayerFramesBox.Size = new Size(123, 19);
            this.PlayerFramesBox.TabIndex = 60;
            this.PlayerFramesBox.Text = "Player frames only";
            this.PlayerFramesBox.UseVisualStyleBackColor = true;
            // 
            // RenderingSettingsButton
            // 
            this.RenderingSettingsButton.Location = new Point(120, 30);
            this.RenderingSettingsButton.Name = "RenderingSettingsButton";
            this.RenderingSettingsButton.Size = new Size(115, 25);
            this.RenderingSettingsButton.TabIndex = 71;
            this.RenderingSettingsButton.Text = "Rendering settings";
            this.RenderingSettingsButton.UseVisualStyleBackColor = true;
            this.RenderingSettingsButton.Click += new EventHandler(this.RenderingSettingsButtonClick);
            // 
            // ActivePlayerPanel
            // 
            this.ActivePlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            this.ActivePlayerPanel.Cursor = Cursors.Hand;
            this.ActivePlayerPanel.Location = new Point(0, 0);
            this.ActivePlayerPanel.Name = "ActivePlayerPanel";
            this.ActivePlayerPanel.Size = new Size(20, 20);
            this.ActivePlayerPanel.TabIndex = 132;
            this.ActivePlayerPanel.Click += new EventHandler(this.ChangeRectColor);
            // 
            // InActivePlayerPanel
            // 
            this.InActivePlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            this.InActivePlayerPanel.Cursor = Cursors.Hand;
            this.InActivePlayerPanel.Location = new Point(0, 33);
            this.InActivePlayerPanel.Name = "InActivePlayerPanel";
            this.InActivePlayerPanel.Size = new Size(20, 20);
            this.InActivePlayerPanel.TabIndex = 133;
            this.InActivePlayerPanel.Click += new EventHandler(this.ChangeRectColor);
            // 
            // DrivingLinePanel
            // 
            this.DrivingLinePanel.BorderStyle = BorderStyle.FixedSingle;
            this.DrivingLinePanel.Cursor = Cursors.Hand;
            this.DrivingLinePanel.Location = new Point(120, 0);
            this.DrivingLinePanel.Name = "DrivingLinePanel";
            this.DrivingLinePanel.Size = new Size(20, 20);
            this.DrivingLinePanel.TabIndex = 134;
            this.DrivingLinePanel.Click += new EventHandler(this.ChangeRectColor);
            // 
            // DrivingLineLabel
            // 
            this.DrivingLineLabel.AutoSize = true;
            this.DrivingLineLabel.Cursor = Cursors.Default;
            this.DrivingLineLabel.Location = new Point(146, 3);
            this.DrivingLineLabel.Name = "DrivingLineLabel";
            this.DrivingLineLabel.Size = new Size(152, 15);
            this.DrivingLineLabel.TabIndex = 131;
            this.DrivingLineLabel.Text = "Current player\'s driving line";
            // 
            // ActivePLabel
            // 
            this.ActivePLabel.AutoSize = true;
            this.ActivePLabel.Cursor = Cursors.Default;
            this.ActivePLabel.Location = new Point(26, 3);
            this.ActivePLabel.Name = "ActivePLabel";
            this.ActivePLabel.Size = new Size(75, 15);
            this.ActivePLabel.TabIndex = 129;
            this.ActivePLabel.Text = "Active player";
            // 
            // InactivePLabel
            // 
            this.InactivePLabel.AutoSize = true;
            this.InactivePLabel.Cursor = Cursors.Default;
            this.InactivePLabel.Location = new Point(26, 36);
            this.InactivePLabel.Name = "InactivePLabel";
            this.InactivePLabel.Size = new Size(83, 15);
            this.InactivePLabel.TabIndex = 130;
            this.InactivePLabel.Text = "Inactive player";
            // 
            // Label5
            // 
            this.Label5.Anchor = AnchorStyles.Right;
            this.Label5.AutoSize = true;
            this.Label5.Location = new Point(10, 36);
            this.Label5.Name = "Label5";
            this.Label5.Size = new Size(131, 15);
            this.Label5.TabIndex = 123;
            this.Label5.Text = "Mouse click zoom step:";
            // 
            // Label4
            // 
            this.Label4.Anchor = AnchorStyles.Right;
            this.Label4.AutoSize = true;
            this.Label4.Location = new Point(3, 7);
            this.Label4.Name = "Label4";
            this.Label4.Size = new Size(138, 15);
            this.Label4.TabIndex = 121;
            this.Label4.Text = "Mouse wheel zoom step:";
            // 
            // TransparentInactiveBox
            // 
            this.TransparentInactiveBox.AutoSize = true;
            this.TransparentInactiveBox.Location = new Point(148, 38);
            this.TransparentInactiveBox.Margin = new Padding(0);
            this.TransparentInactiveBox.Name = "TransparentInactiveBox";
            this.TransparentInactiveBox.Size = new Size(171, 19);
            this.TransparentInactiveBox.TabIndex = 57;
            this.TransparentInactiveBox.Text = "Transparent inactive players";
            this.TransparentInactiveBox.UseVisualStyleBackColor = true;
            // 
            // PictBackGroundBox
            // 
            this.PictBackGroundBox.AutoSize = true;
            this.PictBackGroundBox.Location = new Point(0, 57);
            this.PictBackGroundBox.Margin = new Padding(0);
            this.PictBackGroundBox.Name = "PictBackGroundBox";
            this.PictBackGroundBox.Size = new Size(148, 19);
            this.PictBackGroundBox.TabIndex = 58;
            this.PictBackGroundBox.Text = "Pictures in background";
            this.PictBackGroundBox.UseVisualStyleBackColor = true;
            // 
            // LockedCamBox
            // 
            this.LockedCamBox.AutoSize = true;
            this.LockedCamBox.Location = new Point(148, 0);
            this.LockedCamBox.Margin = new Padding(0);
            this.LockedCamBox.Name = "LockedCamBox";
            this.LockedCamBox.Size = new Size(106, 19);
            this.LockedCamBox.TabIndex = 53;
            this.LockedCamBox.Text = "Locked camera";
            this.LockedCamBox.UseVisualStyleBackColor = true;
            // 
            // FollowDriverBox
            // 
            this.FollowDriverBox.AutoSize = true;
            this.FollowDriverBox.Location = new Point(0, 19);
            this.FollowDriverBox.Margin = new Padding(0);
            this.FollowDriverBox.Name = "FollowDriverBox";
            this.FollowDriverBox.Size = new Size(94, 19);
            this.FollowDriverBox.TabIndex = 54;
            this.FollowDriverBox.Text = "Follow driver";
            this.FollowDriverBox.UseVisualStyleBackColor = true;
            // 
            // ShowDriverPathBox
            // 
            this.ShowDriverPathBox.AutoSize = true;
            this.ShowDriverPathBox.Location = new Point(0, 0);
            this.ShowDriverPathBox.Margin = new Padding(0);
            this.ShowDriverPathBox.Name = "ShowDriverPathBox";
            this.ShowDriverPathBox.Size = new Size(115, 19);
            this.ShowDriverPathBox.TabIndex = 52;
            this.ShowDriverPathBox.Text = "Show driver path";
            this.ShowDriverPathBox.UseVisualStyleBackColor = true;
            // 
            // LoopPlayingBox
            // 
            this.LoopPlayingBox.AutoSize = true;
            this.LoopPlayingBox.Location = new Point(148, 19);
            this.LoopPlayingBox.Margin = new Padding(0);
            this.LoopPlayingBox.Name = "LoopPlayingBox";
            this.LoopPlayingBox.Size = new Size(95, 19);
            this.LoopPlayingBox.TabIndex = 55;
            this.LoopPlayingBox.Text = "Loop playing";
            this.LoopPlayingBox.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ShowDriverPathBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.followAlsoWhenZooming, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.LockedCamBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.FollowDriverBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LoopPlayingBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.HideStartObjectBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.multiSpyBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.TransparentInactiveBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.PictBackGroundBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.SelectNoPlayersBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.PlayerFramesBox, 0, 4);
            this.tableLayoutPanel1.Location = new Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.Size = new Size(319, 114);
            this.tableLayoutPanel1.TabIndex = 147;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.Label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.MouseWheelZoomBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.viewerSizeYBox, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.Label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.MouseClickZoomBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.viewerSizeXBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel2.Location = new Point(6, 123);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.Size = new Size(287, 116);
            this.tableLayoutPanel2.TabIndex = 148;
            //
            // optionsPanel3
            //
            this.optionsPanel3.AutoSize = true;
            this.optionsPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.optionsPanel3.Controls.Add(this.RenderingSettingsButton);
            this.optionsPanel3.Controls.Add(this.ActivePlayerPanel);
            this.optionsPanel3.Controls.Add(this.InActivePlayerPanel);
            this.optionsPanel3.Controls.Add(this.DrivingLinePanel);
            this.optionsPanel3.Controls.Add(this.DrivingLineLabel);
            this.optionsPanel3.Controls.Add(this.ActivePLabel);
            this.optionsPanel3.Controls.Add(this.InactivePLabel);
            this.optionsPanel3.Location = new Point(6, 244);
            this.optionsPanel3.Name = "optionsPanel3";
            this.optionsPanel3.Size = new Size(320, 60);
            this.optionsPanel3.TabIndex = 149;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel.Controls.Add(this.tableLayoutPanel2);
            this.flowLayoutPanel.Controls.Add(this.optionsPanel3);
            this.flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new Size(386, 307);
            this.flowLayoutPanel.WrapContents = false;
            // 
            // ReplayViewer
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(733, 335);
            this.Controls.Add(this.ViewerBox);
            this.Controls.Add(this.TabControl1);
            this.Font = new Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new Size(749, 371);
            this.Name = "ReplayViewerForm";
            this.Text = "Replay viewer";
            this.FormClosing += new FormClosingEventHandler(this.ViewerClosing);
            this.KeyDown += new KeyEventHandler(this.KeyHandler);
            this.MouseWheel += new MouseEventHandler(this.MouseWheelZoom);
            this.contextMenuStrip1.ResumeLayout(false);
            ((ISupportInitialize)(this.playbackSpeedBar)).EndInit();
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            ((ISupportInitialize)(this.PlayList)).EndInit();
            ((ISupportInitialize)(this.timeBar)).EndInit();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.optionsPanel3.ResumeLayout(false);
            this.optionsPanel3.PerformLayout();
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

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
        internal ObjectListView PlayList;
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
    }

}
