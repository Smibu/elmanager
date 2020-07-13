using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class ReplayViewer
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
                _renderer?.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.ViewerBox = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftVoltsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightVoltsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supervoltsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groundtouchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.playbackSpeedBar = new Elmanager.CustomControls.TrackBarMod();
            this.TabControl1 = new Elmanager.CustomControls.TabControlMod();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.PlayList = new BrightIdeasSoftware.ObjectListView();
            this.OlvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FullScreenButton = new System.Windows.Forms.Button();
            this.SnapShotButton = new System.Windows.Forms.Button();
            this.EventListBox = new System.Windows.Forms.ListBox();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.ZoomFillButton = new System.Windows.Forms.Button();
            this.TimeBox = new System.Windows.Forms.TextBox();
            this.PrevFrameButton = new System.Windows.Forms.Button();
            this.NextFrameButton = new System.Windows.Forms.Button();
            this.timeBar = new Elmanager.CustomControls.TrackBarMod();
            this.CoordinateLabel = new System.Windows.Forms.Label();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.followAlsoWhenZooming = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.viewerSizeYBox = new Elmanager.CustomControls.NumericTextBox();
            this.viewerSizeXBox = new Elmanager.CustomControls.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.multiSpyBox = new System.Windows.Forms.CheckBox();
            this.HideStartObjectBox = new System.Windows.Forms.CheckBox();
            this.SelectNoPlayersBox = new System.Windows.Forms.CheckBox();
            this.MouseClickZoomBox = new Elmanager.CustomControls.NumericTextBox();
            this.MouseWheelZoomBox = new Elmanager.CustomControls.NumericTextBox();
            this.PlayerFramesBox = new System.Windows.Forms.CheckBox();
            this.RenderingSettingsButton = new System.Windows.Forms.Button();
            this.ActivePlayerPanel = new System.Windows.Forms.Panel();
            this.InActivePlayerPanel = new System.Windows.Forms.Panel();
            this.DrivingLinePanel = new System.Windows.Forms.Panel();
            this.DrivingLineLabel = new System.Windows.Forms.Label();
            this.ActivePLabel = new System.Windows.Forms.Label();
            this.InactivePLabel = new System.Windows.Forms.Label();
            this.ResolutionBox = new Elmanager.CustomControls.ComboBoxMod();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.TransparentInactiveBox = new System.Windows.Forms.CheckBox();
            this.PictBackGroundBox = new System.Windows.Forms.CheckBox();
            this.LockedCamBox = new System.Windows.Forms.CheckBox();
            this.FollowDriverBox = new System.Windows.Forms.CheckBox();
            this.ShowDriverPathBox = new System.Windows.Forms.CheckBox();
            this.LoopPlayingBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playbackSpeedBar)).BeginInit();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            this.TabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.ViewerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewerBox.Location = new System.Drawing.Point(394, 0);
            this.ViewerBox.Name = "ViewerBox";
            this.ViewerBox.Size = new System.Drawing.Size(339, 335);
            this.ViewerBox.TabIndex = 100;
            this.ViewerBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseDown);
            this.ViewerBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseMoving);
            this.ViewerBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applesToolStripMenuItem,
            this.leftVoltsToolStripMenuItem,
            this.rightVoltsToolStripMenuItem,
            this.supervoltsToolStripMenuItem,
            this.turnsToolStripMenuItem,
            this.groundtouchesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 136);
            // 
            // applesToolStripMenuItem
            // 
            this.applesToolStripMenuItem.Checked = true;
            this.applesToolStripMenuItem.CheckOnClick = true;
            this.applesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.applesToolStripMenuItem.Name = "applesToolStripMenuItem";
            this.applesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.applesToolStripMenuItem.Text = "Apples";
            // 
            // leftVoltsToolStripMenuItem
            // 
            this.leftVoltsToolStripMenuItem.CheckOnClick = true;
            this.leftVoltsToolStripMenuItem.Name = "leftVoltsToolStripMenuItem";
            this.leftVoltsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.leftVoltsToolStripMenuItem.Text = "Left volts";
            // 
            // rightVoltsToolStripMenuItem
            // 
            this.rightVoltsToolStripMenuItem.CheckOnClick = true;
            this.rightVoltsToolStripMenuItem.Name = "rightVoltsToolStripMenuItem";
            this.rightVoltsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.rightVoltsToolStripMenuItem.Text = "Right volts";
            // 
            // supervoltsToolStripMenuItem
            // 
            this.supervoltsToolStripMenuItem.CheckOnClick = true;
            this.supervoltsToolStripMenuItem.Name = "supervoltsToolStripMenuItem";
            this.supervoltsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.supervoltsToolStripMenuItem.Text = "Supervolts";
            // 
            // turnsToolStripMenuItem
            // 
            this.turnsToolStripMenuItem.CheckOnClick = true;
            this.turnsToolStripMenuItem.Name = "turnsToolStripMenuItem";
            this.turnsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.turnsToolStripMenuItem.Text = "Turns";
            // 
            // groundtouchesToolStripMenuItem
            // 
            this.groundtouchesToolStripMenuItem.CheckOnClick = true;
            this.groundtouchesToolStripMenuItem.Name = "groundtouchesToolStripMenuItem";
            this.groundtouchesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.groundtouchesToolStripMenuItem.Text = "Groundtouches";
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
            this.playbackSpeedBar.BackColor = System.Drawing.Color.White;
            this.playbackSpeedBar.LargeChange = 10;
            this.playbackSpeedBar.Location = new System.Drawing.Point(0, 35);
            this.playbackSpeedBar.Minimum = -10;
            this.playbackSpeedBar.Name = "playbackSpeedBar";
            this.playbackSpeedBar.Size = new System.Drawing.Size(166, 45);
            this.playbackSpeedBar.TabIndex = 126;
            this.toolTip1.SetToolTip(this.playbackSpeedBar, "Playback speed: 1x");
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(394, 335);
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
            this.TabPage1.Location = new System.Drawing.Point(4, 24);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(386, 307);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Player";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(173, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(208, 41);
            this.button1.TabIndex = 127;
            this.button1.Text = "Click and hold to zoom in/out";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ZoomButtonMouseDown);
            this.button1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ZoomButtonMouseMove);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZoomButtonMouseUp);
            // 
            // PlayList
            // 
            this.PlayList.AllColumns.Add(this.OlvColumn1);
            this.PlayList.AllColumns.Add(this.OlvColumn2);
            this.PlayList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayList.CellEditUseWholeCell = false;
            this.PlayList.CheckBoxes = true;
            this.PlayList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OlvColumn1,
            this.OlvColumn2});
            this.PlayList.Cursor = System.Windows.Forms.Cursors.Default;
            this.PlayList.FullRowSelect = true;
            this.PlayList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PlayList.HideSelection = false;
            this.PlayList.Location = new System.Drawing.Point(0, 192);
            this.PlayList.Name = "PlayList";
            this.PlayList.ShowGroups = false;
            this.PlayList.ShowImagesOnSubItems = true;
            this.PlayList.Size = new System.Drawing.Size(166, 108);
            this.PlayList.TabIndex = 100;
            this.PlayList.UseCompatibleStateImageBehavior = false;
            this.PlayList.UseOverlays = false;
            this.PlayList.View = System.Windows.Forms.View.Details;
            this.PlayList.SelectionChanged += new System.EventHandler(this.PlayListSelectionChanged);
            this.PlayList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.PlayListSelectionChanged);
            this.PlayList.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheelZoom);
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
            this.FullScreenButton.Location = new System.Drawing.Point(273, 35);
            this.FullScreenButton.Name = "FullScreenButton";
            this.FullScreenButton.Size = new System.Drawing.Size(108, 25);
            this.FullScreenButton.TabIndex = 99;
            this.FullScreenButton.Text = "Full screen";
            this.FullScreenButton.UseVisualStyleBackColor = true;
            this.FullScreenButton.Click += new System.EventHandler(this.FullScreen);
            // 
            // SnapShotButton
            // 
            this.SnapShotButton.Location = new System.Drawing.Point(173, 35);
            this.SnapShotButton.Name = "SnapShotButton";
            this.SnapShotButton.Size = new System.Drawing.Size(93, 25);
            this.SnapShotButton.TabIndex = 98;
            this.SnapShotButton.Text = "Get snapshot";
            this.SnapShotButton.UseVisualStyleBackColor = true;
            this.SnapShotButton.Click += new System.EventHandler(this.SnapShotButtonClick);
            // 
            // EventListBox
            // 
            this.EventListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EventListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.EventListBox.FormattingEnabled = true;
            this.EventListBox.IntegralHeight = false;
            this.EventListBox.ItemHeight = 15;
            this.EventListBox.Location = new System.Drawing.Point(173, 192);
            this.EventListBox.Name = "EventListBox";
            this.EventListBox.Size = new System.Drawing.Size(208, 107);
            this.EventListBox.TabIndex = 79;
            this.EventListBox.SelectedIndexChanged += new System.EventHandler(this.EventListBoxSelectedIndexChanged);
            this.EventListBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheelZoom);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new System.Drawing.Point(172, 108);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(42, 15);
            this.SpeedLabel.TabIndex = 96;
            this.SpeedLabel.Text = "Speed:";
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(94, 7);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 25);
            this.PlayButton.TabIndex = 53;
            this.PlayButton.Text = "Play/Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(309, 7);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(72, 25);
            this.StopButton.TabIndex = 55;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // ZoomFillButton
            // 
            this.ZoomFillButton.Location = new System.Drawing.Point(0, 7);
            this.ZoomFillButton.Name = "ZoomFillButton";
            this.ZoomFillButton.Size = new System.Drawing.Size(84, 25);
            this.ZoomFillButton.TabIndex = 56;
            this.ZoomFillButton.Text = "Zoom fill";
            this.ZoomFillButton.UseVisualStyleBackColor = true;
            // 
            // TimeBox
            // 
            this.TimeBox.Location = new System.Drawing.Point(210, 9);
            this.TimeBox.Name = "TimeBox";
            this.TimeBox.Size = new System.Drawing.Size(56, 23);
            this.TimeBox.TabIndex = 60;
            this.TimeBox.Text = "00:00,000";
            this.TimeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandler);
            // 
            // PrevFrameButton
            // 
            this.PrevFrameButton.Location = new System.Drawing.Point(173, 7);
            this.PrevFrameButton.Name = "PrevFrameButton";
            this.PrevFrameButton.Size = new System.Drawing.Size(29, 25);
            this.PrevFrameButton.TabIndex = 61;
            this.PrevFrameButton.Text = "<";
            this.PrevFrameButton.UseVisualStyleBackColor = true;
            // 
            // NextFrameButton
            // 
            this.NextFrameButton.Location = new System.Drawing.Point(273, 7);
            this.NextFrameButton.Name = "NextFrameButton";
            this.NextFrameButton.Size = new System.Drawing.Size(29, 25);
            this.NextFrameButton.TabIndex = 62;
            this.NextFrameButton.Text = ">";
            this.NextFrameButton.UseVisualStyleBackColor = true;
            // 
            // timeBar
            // 
            this.timeBar.BackColor = System.Drawing.SystemColors.Window;
            this.timeBar.LargeChange = 10;
            this.timeBar.Location = new System.Drawing.Point(0, 144);
            this.timeBar.Maximum = 1000;
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(384, 45);
            this.timeBar.TabIndex = 54;
            this.timeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.timeBar.Scroll += new System.EventHandler(this.PlayBarScroll);
            this.timeBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Goto);
            this.timeBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ShowTime);
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.AutoSize = true;
            this.CoordinateLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CoordinateLabel.Location = new System.Drawing.Point(172, 130);
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new System.Drawing.Size(111, 15);
            this.CoordinateLabel.TabIndex = 54;
            this.CoordinateLabel.Text = "Mouse coordinates:";
            this.CoordinateLabel.Click += new System.EventHandler(this.CoordinateLabelClick);
            this.CoordinateLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CoordinateLabelClick);
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.tableLayoutPanel2);
            this.TabPage2.Controls.Add(this.tableLayoutPanel1);
            this.TabPage2.Controls.Add(this.RenderingSettingsButton);
            this.TabPage2.Controls.Add(this.ActivePlayerPanel);
            this.TabPage2.Controls.Add(this.InActivePlayerPanel);
            this.TabPage2.Controls.Add(this.DrivingLinePanel);
            this.TabPage2.Controls.Add(this.DrivingLineLabel);
            this.TabPage2.Controls.Add(this.ActivePLabel);
            this.TabPage2.Controls.Add(this.InactivePLabel);
            this.TabPage2.Location = new System.Drawing.Point(4, 24);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(386, 307);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Options";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // followAlsoWhenZooming
            // 
            this.followAlsoWhenZooming.AutoSize = true;
            this.followAlsoWhenZooming.Location = new System.Drawing.Point(0, 38);
            this.followAlsoWhenZooming.Margin = new System.Windows.Forms.Padding(0);
            this.followAlsoWhenZooming.Name = "followAlsoWhenZooming";
            this.followAlsoWhenZooming.Size = new System.Drawing.Size(138, 19);
            this.followAlsoWhenZooming.TabIndex = 146;
            this.followAlsoWhenZooming.Text = "...also when zooming";
            this.followAlsoWhenZooming.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 15);
            this.label2.TabIndex = 145;
            this.label2.Text = "x";
            // 
            // viewerSizeYBox
            // 
            this.viewerSizeYBox.BackColor = System.Drawing.SystemColors.Window;
            this.viewerSizeYBox.DefaultValue = 0D;
            this.viewerSizeYBox.Location = new System.Drawing.Point(221, 61);
            this.viewerSizeYBox.Name = "viewerSizeYBox";
            this.viewerSizeYBox.Size = new System.Drawing.Size(49, 23);
            this.viewerSizeYBox.TabIndex = 144;
            this.viewerSizeYBox.Text = "1";
            this.viewerSizeYBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // viewerSizeXBox
            // 
            this.viewerSizeXBox.BackColor = System.Drawing.SystemColors.Window;
            this.viewerSizeXBox.DefaultValue = 0D;
            this.viewerSizeXBox.Location = new System.Drawing.Point(147, 61);
            this.viewerSizeXBox.Name = "viewerSizeXBox";
            this.viewerSizeXBox.Size = new System.Drawing.Size(49, 23);
            this.viewerSizeXBox.TabIndex = 143;
            this.viewerSizeXBox.Text = "1";
            this.viewerSizeXBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 142;
            this.label1.Text = "Viewer size:";
            // 
            // multiSpyBox
            // 
            this.multiSpyBox.AutoSize = true;
            this.multiSpyBox.Location = new System.Drawing.Point(148, 76);
            this.multiSpyBox.Margin = new System.Windows.Forms.Padding(0);
            this.multiSpyBox.Name = "multiSpyBox";
            this.multiSpyBox.Size = new System.Drawing.Size(75, 19);
            this.multiSpyBox.TabIndex = 141;
            this.multiSpyBox.Text = "Multi spy";
            this.multiSpyBox.UseVisualStyleBackColor = true;
            // 
            // HideStartObjectBox
            // 
            this.HideStartObjectBox.AutoSize = true;
            this.HideStartObjectBox.Location = new System.Drawing.Point(0, 95);
            this.HideStartObjectBox.Margin = new System.Windows.Forms.Padding(0);
            this.HideStartObjectBox.Name = "HideStartObjectBox";
            this.HideStartObjectBox.Size = new System.Drawing.Size(113, 19);
            this.HideStartObjectBox.TabIndex = 140;
            this.HideStartObjectBox.Text = "Hide start object";
            this.HideStartObjectBox.UseVisualStyleBackColor = true;
            // 
            // SelectNoPlayersBox
            // 
            this.SelectNoPlayersBox.AutoSize = true;
            this.SelectNoPlayersBox.Location = new System.Drawing.Point(148, 57);
            this.SelectNoPlayersBox.Margin = new System.Windows.Forms.Padding(0);
            this.SelectNoPlayersBox.Name = "SelectNoPlayersBox";
            this.SelectNoPlayersBox.Size = new System.Drawing.Size(170, 19);
            this.SelectNoPlayersBox.TabIndex = 139;
            this.SelectNoPlayersBox.Text = "Select no players by default";
            this.SelectNoPlayersBox.UseVisualStyleBackColor = true;
            // 
            // MouseClickZoomBox
            // 
            this.MouseClickZoomBox.BackColor = System.Drawing.SystemColors.Window;
            this.MouseClickZoomBox.DefaultValue = 0D;
            this.MouseClickZoomBox.Location = new System.Drawing.Point(147, 32);
            this.MouseClickZoomBox.Name = "MouseClickZoomBox";
            this.MouseClickZoomBox.Size = new System.Drawing.Size(49, 23);
            this.MouseClickZoomBox.TabIndex = 138;
            this.MouseClickZoomBox.Text = "1";
            this.MouseClickZoomBox.TextChanged += new System.EventHandler(this.MouseClickZoomBoxTextChanged);
            // 
            // MouseWheelZoomBox
            // 
            this.MouseWheelZoomBox.BackColor = System.Drawing.SystemColors.Window;
            this.MouseWheelZoomBox.DefaultValue = 0D;
            this.MouseWheelZoomBox.Location = new System.Drawing.Point(147, 3);
            this.MouseWheelZoomBox.Name = "MouseWheelZoomBox";
            this.MouseWheelZoomBox.Size = new System.Drawing.Size(49, 23);
            this.MouseWheelZoomBox.TabIndex = 137;
            this.MouseWheelZoomBox.Text = "1";
            this.MouseWheelZoomBox.TextChanged += new System.EventHandler(this.MouseWheelZoomBoxTextChanged);
            // 
            // PlayerFramesBox
            // 
            this.PlayerFramesBox.AutoSize = true;
            this.PlayerFramesBox.Location = new System.Drawing.Point(0, 76);
            this.PlayerFramesBox.Margin = new System.Windows.Forms.Padding(0);
            this.PlayerFramesBox.Name = "PlayerFramesBox";
            this.PlayerFramesBox.Size = new System.Drawing.Size(123, 19);
            this.PlayerFramesBox.TabIndex = 136;
            this.PlayerFramesBox.Text = "Player frames only";
            this.PlayerFramesBox.UseVisualStyleBackColor = true;
            // 
            // RenderingSettingsButton
            // 
            this.RenderingSettingsButton.Location = new System.Drawing.Point(126, 274);
            this.RenderingSettingsButton.Name = "RenderingSettingsButton";
            this.RenderingSettingsButton.Size = new System.Drawing.Size(115, 25);
            this.RenderingSettingsButton.TabIndex = 135;
            this.RenderingSettingsButton.Text = "Rendering settings";
            this.RenderingSettingsButton.UseVisualStyleBackColor = true;
            this.RenderingSettingsButton.Click += new System.EventHandler(this.RenderingSettingsButtonClick);
            // 
            // ActivePlayerPanel
            // 
            this.ActivePlayerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ActivePlayerPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ActivePlayerPanel.Location = new System.Drawing.Point(6, 244);
            this.ActivePlayerPanel.Name = "ActivePlayerPanel";
            this.ActivePlayerPanel.Size = new System.Drawing.Size(20, 20);
            this.ActivePlayerPanel.TabIndex = 132;
            this.ActivePlayerPanel.Click += new System.EventHandler(this.ChangeRectColor);
            // 
            // InActivePlayerPanel
            // 
            this.InActivePlayerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InActivePlayerPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InActivePlayerPanel.Location = new System.Drawing.Point(6, 277);
            this.InActivePlayerPanel.Name = "InActivePlayerPanel";
            this.InActivePlayerPanel.Size = new System.Drawing.Size(20, 20);
            this.InActivePlayerPanel.TabIndex = 133;
            this.InActivePlayerPanel.Click += new System.EventHandler(this.ChangeRectColor);
            // 
            // DrivingLinePanel
            // 
            this.DrivingLinePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DrivingLinePanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DrivingLinePanel.Location = new System.Drawing.Point(126, 244);
            this.DrivingLinePanel.Name = "DrivingLinePanel";
            this.DrivingLinePanel.Size = new System.Drawing.Size(20, 20);
            this.DrivingLinePanel.TabIndex = 134;
            this.DrivingLinePanel.Click += new System.EventHandler(this.ChangeRectColor);
            // 
            // DrivingLineLabel
            // 
            this.DrivingLineLabel.AutoSize = true;
            this.DrivingLineLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DrivingLineLabel.Location = new System.Drawing.Point(152, 247);
            this.DrivingLineLabel.Name = "DrivingLineLabel";
            this.DrivingLineLabel.Size = new System.Drawing.Size(152, 15);
            this.DrivingLineLabel.TabIndex = 131;
            this.DrivingLineLabel.Text = "Current player\'s driving line";
            // 
            // ActivePLabel
            // 
            this.ActivePLabel.AutoSize = true;
            this.ActivePLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ActivePLabel.Location = new System.Drawing.Point(32, 247);
            this.ActivePLabel.Name = "ActivePLabel";
            this.ActivePLabel.Size = new System.Drawing.Size(75, 15);
            this.ActivePLabel.TabIndex = 129;
            this.ActivePLabel.Text = "Active player";
            // 
            // InactivePLabel
            // 
            this.InactivePLabel.AutoSize = true;
            this.InactivePLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.InactivePLabel.Location = new System.Drawing.Point(32, 280);
            this.InactivePLabel.Name = "InactivePLabel";
            this.InactivePLabel.Size = new System.Drawing.Size(83, 15);
            this.InactivePLabel.TabIndex = 130;
            this.InactivePLabel.Text = "Inactive player";
            // 
            // ResolutionBox
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.ResolutionBox, 4);
            this.ResolutionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResolutionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResolutionBox.FormattingEnabled = true;
            this.ResolutionBox.Location = new System.Drawing.Point(147, 90);
            this.ResolutionBox.Name = "ResolutionBox";
            this.ResolutionBox.Size = new System.Drawing.Size(137, 23);
            this.ResolutionBox.TabIndex = 128;
            // 
            // Label7
            // 
            this.Label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(19, 94);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(122, 15);
            this.Label7.TabIndex = 127;
            this.Label7.Text = "Full screen resolution:";
            // 
            // Label5
            // 
            this.Label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(10, 36);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(131, 15);
            this.Label5.TabIndex = 123;
            this.Label5.Text = "Mouse click zoom step:";
            // 
            // Label4
            // 
            this.Label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(3, 7);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(138, 15);
            this.Label4.TabIndex = 121;
            this.Label4.Text = "Mouse wheel zoom step:";
            // 
            // TransparentInactiveBox
            // 
            this.TransparentInactiveBox.AutoSize = true;
            this.TransparentInactiveBox.Location = new System.Drawing.Point(148, 38);
            this.TransparentInactiveBox.Margin = new System.Windows.Forms.Padding(0);
            this.TransparentInactiveBox.Name = "TransparentInactiveBox";
            this.TransparentInactiveBox.Size = new System.Drawing.Size(171, 19);
            this.TransparentInactiveBox.TabIndex = 113;
            this.TransparentInactiveBox.Text = "Transparent inactive players";
            this.TransparentInactiveBox.UseVisualStyleBackColor = true;
            // 
            // PictBackGroundBox
            // 
            this.PictBackGroundBox.AutoSize = true;
            this.PictBackGroundBox.Location = new System.Drawing.Point(0, 57);
            this.PictBackGroundBox.Margin = new System.Windows.Forms.Padding(0);
            this.PictBackGroundBox.Name = "PictBackGroundBox";
            this.PictBackGroundBox.Size = new System.Drawing.Size(148, 19);
            this.PictBackGroundBox.TabIndex = 112;
            this.PictBackGroundBox.Text = "Pictures in background";
            this.PictBackGroundBox.UseVisualStyleBackColor = true;
            // 
            // LockedCamBox
            // 
            this.LockedCamBox.AutoSize = true;
            this.LockedCamBox.Location = new System.Drawing.Point(148, 0);
            this.LockedCamBox.Margin = new System.Windows.Forms.Padding(0);
            this.LockedCamBox.Name = "LockedCamBox";
            this.LockedCamBox.Size = new System.Drawing.Size(106, 19);
            this.LockedCamBox.TabIndex = 108;
            this.LockedCamBox.Text = "Locked camera";
            this.LockedCamBox.UseVisualStyleBackColor = true;
            // 
            // FollowDriverBox
            // 
            this.FollowDriverBox.AutoSize = true;
            this.FollowDriverBox.Location = new System.Drawing.Point(0, 19);
            this.FollowDriverBox.Margin = new System.Windows.Forms.Padding(0);
            this.FollowDriverBox.Name = "FollowDriverBox";
            this.FollowDriverBox.Size = new System.Drawing.Size(94, 19);
            this.FollowDriverBox.TabIndex = 54;
            this.FollowDriverBox.Text = "Follow driver";
            this.FollowDriverBox.UseVisualStyleBackColor = true;
            // 
            // ShowDriverPathBox
            // 
            this.ShowDriverPathBox.AutoSize = true;
            this.ShowDriverPathBox.Location = new System.Drawing.Point(0, 0);
            this.ShowDriverPathBox.Margin = new System.Windows.Forms.Padding(0);
            this.ShowDriverPathBox.Name = "ShowDriverPathBox";
            this.ShowDriverPathBox.Size = new System.Drawing.Size(115, 19);
            this.ShowDriverPathBox.TabIndex = 52;
            this.ShowDriverPathBox.Text = "Show driver path";
            this.ShowDriverPathBox.UseVisualStyleBackColor = true;
            // 
            // LoopPlayingBox
            // 
            this.LoopPlayingBox.AutoSize = true;
            this.LoopPlayingBox.Location = new System.Drawing.Point(148, 19);
            this.LoopPlayingBox.Margin = new System.Windows.Forms.Padding(0);
            this.LoopPlayingBox.Name = "LoopPlayingBox";
            this.LoopPlayingBox.Size = new System.Drawing.Size(95, 19);
            this.LoopPlayingBox.TabIndex = 72;
            this.LoopPlayingBox.Text = "Loop playing";
            this.LoopPlayingBox.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(319, 114);
            this.tableLayoutPanel1.TabIndex = 147;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.Label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.MouseWheelZoomBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.viewerSizeYBox, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.Label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.MouseClickZoomBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.viewerSizeXBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.Label7, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.ResolutionBox, 1, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 123);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(287, 116);
            this.tableLayoutPanel2.TabIndex = 148;
            // 
            // ReplayViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(733, 335);
            this.Controls.Add(this.ViewerBox);
            this.Controls.Add(this.TabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(749, 371);
            this.Name = "ReplayViewer";
            this.Text = "Replay viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewerClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandler);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheelZoom);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.playbackSpeedBar)).EndInit();
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }
		internal System.Windows.Forms.Label CoordinateLabel;
		internal TrackBarMod timeBar;
		internal System.Windows.Forms.CheckBox LoopPlayingBox;
		internal System.Windows.Forms.Button NextFrameButton;
		internal System.Windows.Forms.Button PrevFrameButton;
		internal System.Windows.Forms.TextBox TimeBox;
		internal System.Windows.Forms.Button ZoomFillButton;
		internal System.Windows.Forms.Button StopButton;
		internal System.Windows.Forms.CheckBox ShowDriverPathBox;
		internal System.Windows.Forms.CheckBox FollowDriverBox;
		internal System.Windows.Forms.Button PlayButton;
		internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
		internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.ListBox EventListBox;
		internal System.Windows.Forms.Label SpeedLabel;
		internal TabControlMod TabControl1;
		internal System.Windows.Forms.TabPage TabPage1;
		internal System.Windows.Forms.TabPage TabPage2;
		internal System.Windows.Forms.Button SnapShotButton;
		internal System.Windows.Forms.Button FullScreenButton;
		internal System.Windows.Forms.CheckBox LockedCamBox;
		internal System.Windows.Forms.CheckBox TransparentInactiveBox;
		internal System.Windows.Forms.CheckBox PictBackGroundBox;
		internal System.Windows.Forms.Panel ViewerBox;
		internal ComboBoxMod ResolutionBox;
		internal System.Windows.Forms.Label Label7;
        internal TrackBarMod playbackSpeedBar;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.Panel ActivePlayerPanel;
		internal System.Windows.Forms.Panel InActivePlayerPanel;
		internal System.Windows.Forms.Panel DrivingLinePanel;
		internal System.Windows.Forms.Label DrivingLineLabel;
		internal System.Windows.Forms.Label ActivePLabel;
		internal System.Windows.Forms.Label InactivePLabel;
		internal System.Windows.Forms.Button RenderingSettingsButton;
		internal System.Windows.Forms.CheckBox PlayerFramesBox;
		internal Elmanager.CustomControls.NumericTextBox MouseClickZoomBox;
        internal Elmanager.CustomControls.NumericTextBox MouseWheelZoomBox;
		internal System.Windows.Forms.CheckBox SelectNoPlayersBox;
		internal BrightIdeasSoftware.ObjectListView PlayList;
		internal BrightIdeasSoftware.OLVColumn OlvColumn1;
		internal BrightIdeasSoftware.OLVColumn OlvColumn2;
		internal System.Windows.Forms.CheckBox HideStartObjectBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem applesToolStripMenuItem;
        private ToolStripMenuItem leftVoltsToolStripMenuItem;
        private ToolStripMenuItem rightVoltsToolStripMenuItem;
        private ToolStripMenuItem supervoltsToolStripMenuItem;
        private ToolStripMenuItem turnsToolStripMenuItem;
        private ToolStripMenuItem groundtouchesToolStripMenuItem;
        private System.ComponentModel.IContainer components;
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
    }
	
}
