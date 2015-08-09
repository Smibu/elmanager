using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class ReplayViewer : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplayViewer));
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
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playbackSpeedBar)).BeginInit();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            this.TabPage2.SuspendLayout();
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
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(386, 309);
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
            this.FullScreenButton.Size = new System.Drawing.Size(108, 23);
            this.FullScreenButton.TabIndex = 99;
            this.FullScreenButton.Text = "Full screen";
            this.FullScreenButton.UseVisualStyleBackColor = true;
            this.FullScreenButton.Click += new System.EventHandler(this.FullScreen);
            // 
            // SnapShotButton
            // 
            this.SnapShotButton.Location = new System.Drawing.Point(173, 35);
            this.SnapShotButton.Name = "SnapShotButton";
            this.SnapShotButton.Size = new System.Drawing.Size(93, 23);
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
            this.EventListBox.Location = new System.Drawing.Point(173, 192);
            this.EventListBox.Name = "EventListBox";
            this.EventListBox.Size = new System.Drawing.Size(208, 108);
            this.EventListBox.TabIndex = 79;
            this.EventListBox.SelectedIndexChanged += new System.EventHandler(this.EventListBoxSelectedIndexChanged);
            this.EventListBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheelZoom);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new System.Drawing.Point(172, 108);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(41, 13);
            this.SpeedLabel.TabIndex = 96;
            this.SpeedLabel.Text = "Speed:";
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(94, 7);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(72, 22);
            this.PlayButton.TabIndex = 53;
            this.PlayButton.Text = "Play/Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(309, 7);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(72, 22);
            this.StopButton.TabIndex = 55;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // ZoomFillButton
            // 
            this.ZoomFillButton.Location = new System.Drawing.Point(0, 7);
            this.ZoomFillButton.Name = "ZoomFillButton";
            this.ZoomFillButton.Size = new System.Drawing.Size(84, 22);
            this.ZoomFillButton.TabIndex = 56;
            this.ZoomFillButton.Text = "Zoom fill";
            this.ZoomFillButton.UseVisualStyleBackColor = true;
            // 
            // TimeBox
            // 
            this.TimeBox.Location = new System.Drawing.Point(210, 9);
            this.TimeBox.Name = "TimeBox";
            this.TimeBox.Size = new System.Drawing.Size(56, 20);
            this.TimeBox.TabIndex = 60;
            this.TimeBox.Text = "00:00,000";
            this.TimeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandler);
            // 
            // PrevFrameButton
            // 
            this.PrevFrameButton.Location = new System.Drawing.Point(173, 7);
            this.PrevFrameButton.Name = "PrevFrameButton";
            this.PrevFrameButton.Size = new System.Drawing.Size(29, 22);
            this.PrevFrameButton.TabIndex = 61;
            this.PrevFrameButton.Text = "<";
            this.PrevFrameButton.UseVisualStyleBackColor = true;
            // 
            // NextFrameButton
            // 
            this.NextFrameButton.Location = new System.Drawing.Point(273, 7);
            this.NextFrameButton.Name = "NextFrameButton";
            this.NextFrameButton.Size = new System.Drawing.Size(29, 22);
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
            this.CoordinateLabel.Size = new System.Drawing.Size(100, 13);
            this.CoordinateLabel.TabIndex = 54;
            this.CoordinateLabel.Text = "Mouse coordinates:";
            this.CoordinateLabel.Click += new System.EventHandler(this.CoordinateLabelClick);
            this.CoordinateLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CoordinateLabelClick);
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.label2);
            this.TabPage2.Controls.Add(this.viewerSizeYBox);
            this.TabPage2.Controls.Add(this.viewerSizeXBox);
            this.TabPage2.Controls.Add(this.label1);
            this.TabPage2.Controls.Add(this.multiSpyBox);
            this.TabPage2.Controls.Add(this.HideStartObjectBox);
            this.TabPage2.Controls.Add(this.SelectNoPlayersBox);
            this.TabPage2.Controls.Add(this.MouseClickZoomBox);
            this.TabPage2.Controls.Add(this.MouseWheelZoomBox);
            this.TabPage2.Controls.Add(this.PlayerFramesBox);
            this.TabPage2.Controls.Add(this.RenderingSettingsButton);
            this.TabPage2.Controls.Add(this.ActivePlayerPanel);
            this.TabPage2.Controls.Add(this.InActivePlayerPanel);
            this.TabPage2.Controls.Add(this.DrivingLinePanel);
            this.TabPage2.Controls.Add(this.DrivingLineLabel);
            this.TabPage2.Controls.Add(this.ActivePLabel);
            this.TabPage2.Controls.Add(this.InactivePLabel);
            this.TabPage2.Controls.Add(this.ResolutionBox);
            this.TabPage2.Controls.Add(this.Label7);
            this.TabPage2.Controls.Add(this.Label5);
            this.TabPage2.Controls.Add(this.Label4);
            this.TabPage2.Controls.Add(this.TransparentInactiveBox);
            this.TabPage2.Controls.Add(this.PictBackGroundBox);
            this.TabPage2.Controls.Add(this.LockedCamBox);
            this.TabPage2.Controls.Add(this.FollowDriverBox);
            this.TabPage2.Controls.Add(this.ShowDriverPathBox);
            this.TabPage2.Controls.Add(this.LoopPlayingBox);
            this.TabPage2.Location = new System.Drawing.Point(4, 22);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(386, 309);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Options";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 145;
            this.label2.Text = "x";
            // 
            // viewerSizeYBox
            // 
            this.viewerSizeYBox.BackColor = System.Drawing.SystemColors.Window;
            this.viewerSizeYBox.DefaultValue = 0D;
            this.viewerSizeYBox.Location = new System.Drawing.Point(206, 167);
            this.viewerSizeYBox.Name = "viewerSizeYBox";
            this.viewerSizeYBox.Size = new System.Drawing.Size(49, 20);
            this.viewerSizeYBox.TabIndex = 144;
            this.viewerSizeYBox.Text = "1";
            this.viewerSizeYBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // viewerSizeXBox
            // 
            this.viewerSizeXBox.BackColor = System.Drawing.SystemColors.Window;
            this.viewerSizeXBox.DefaultValue = 0D;
            this.viewerSizeXBox.Location = new System.Drawing.Point(133, 167);
            this.viewerSizeXBox.Name = "viewerSizeXBox";
            this.viewerSizeXBox.Size = new System.Drawing.Size(49, 20);
            this.viewerSizeXBox.TabIndex = 143;
            this.viewerSizeXBox.Text = "1";
            this.viewerSizeXBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerSizeBoxKeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 142;
            this.label1.Text = "Viewer size:";
            // 
            // multiSpyBox
            // 
            this.multiSpyBox.AutoSize = true;
            this.multiSpyBox.Location = new System.Drawing.Point(147, 82);
            this.multiSpyBox.Name = "multiSpyBox";
            this.multiSpyBox.Size = new System.Drawing.Size(67, 17);
            this.multiSpyBox.TabIndex = 141;
            this.multiSpyBox.Text = "Multi spy";
            this.multiSpyBox.UseVisualStyleBackColor = true;
            // 
            // HideStartObjectBox
            // 
            this.HideStartObjectBox.AutoSize = true;
            this.HideStartObjectBox.Location = new System.Drawing.Point(6, 82);
            this.HideStartObjectBox.Name = "HideStartObjectBox";
            this.HideStartObjectBox.Size = new System.Drawing.Size(103, 17);
            this.HideStartObjectBox.TabIndex = 140;
            this.HideStartObjectBox.Text = "Hide start object";
            this.HideStartObjectBox.UseVisualStyleBackColor = true;
            // 
            // SelectNoPlayersBox
            // 
            this.SelectNoPlayersBox.AutoSize = true;
            this.SelectNoPlayersBox.Location = new System.Drawing.Point(147, 63);
            this.SelectNoPlayersBox.Name = "SelectNoPlayersBox";
            this.SelectNoPlayersBox.Size = new System.Drawing.Size(156, 17);
            this.SelectNoPlayersBox.TabIndex = 139;
            this.SelectNoPlayersBox.Text = "Select no players by default";
            this.SelectNoPlayersBox.UseVisualStyleBackColor = true;
            // 
            // MouseClickZoomBox
            // 
            this.MouseClickZoomBox.BackColor = System.Drawing.SystemColors.Window;
            this.MouseClickZoomBox.DefaultValue = 0D;
            this.MouseClickZoomBox.Location = new System.Drawing.Point(133, 141);
            this.MouseClickZoomBox.Name = "MouseClickZoomBox";
            this.MouseClickZoomBox.Size = new System.Drawing.Size(49, 20);
            this.MouseClickZoomBox.TabIndex = 138;
            this.MouseClickZoomBox.Text = "1";
            this.MouseClickZoomBox.TextChanged += new System.EventHandler(this.MouseClickZoomBoxTextChanged);
            // 
            // MouseWheelZoomBox
            // 
            this.MouseWheelZoomBox.BackColor = System.Drawing.SystemColors.Window;
            this.MouseWheelZoomBox.DefaultValue = 0D;
            this.MouseWheelZoomBox.Location = new System.Drawing.Point(133, 115);
            this.MouseWheelZoomBox.Name = "MouseWheelZoomBox";
            this.MouseWheelZoomBox.Size = new System.Drawing.Size(49, 20);
            this.MouseWheelZoomBox.TabIndex = 137;
            this.MouseWheelZoomBox.Text = "1";
            this.MouseWheelZoomBox.TextChanged += new System.EventHandler(this.MouseWheelZoomBoxTextChanged);
            // 
            // PlayerFramesBox
            // 
            this.PlayerFramesBox.AutoSize = true;
            this.PlayerFramesBox.Location = new System.Drawing.Point(6, 63);
            this.PlayerFramesBox.Name = "PlayerFramesBox";
            this.PlayerFramesBox.Size = new System.Drawing.Size(111, 17);
            this.PlayerFramesBox.TabIndex = 136;
            this.PlayerFramesBox.Text = "Player frames only";
            this.PlayerFramesBox.UseVisualStyleBackColor = true;
            // 
            // RenderingSettingsButton
            // 
            this.RenderingSettingsButton.Location = new System.Drawing.Point(271, 6);
            this.RenderingSettingsButton.Name = "RenderingSettingsButton";
            this.RenderingSettingsButton.Size = new System.Drawing.Size(112, 23);
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
            this.DrivingLineLabel.Size = new System.Drawing.Size(132, 13);
            this.DrivingLineLabel.TabIndex = 131;
            this.DrivingLineLabel.Text = "Current player\'s driving line";
            // 
            // ActivePLabel
            // 
            this.ActivePLabel.AutoSize = true;
            this.ActivePLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ActivePLabel.Location = new System.Drawing.Point(32, 247);
            this.ActivePLabel.Name = "ActivePLabel";
            this.ActivePLabel.Size = new System.Drawing.Size(68, 13);
            this.ActivePLabel.TabIndex = 129;
            this.ActivePLabel.Text = "Active player";
            // 
            // InactivePLabel
            // 
            this.InactivePLabel.AutoSize = true;
            this.InactivePLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.InactivePLabel.Location = new System.Drawing.Point(32, 280);
            this.InactivePLabel.Name = "InactivePLabel";
            this.InactivePLabel.Size = new System.Drawing.Size(76, 13);
            this.InactivePLabel.TabIndex = 130;
            this.InactivePLabel.Text = "Inactive player";
            // 
            // ResolutionBox
            // 
            this.ResolutionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResolutionBox.FormattingEnabled = true;
            this.ResolutionBox.Location = new System.Drawing.Point(133, 216);
            this.ResolutionBox.Name = "ResolutionBox";
            this.ResolutionBox.Size = new System.Drawing.Size(250, 21);
            this.ResolutionBox.TabIndex = 128;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(18, 219);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(109, 13);
            this.Label7.TabIndex = 127;
            this.Label7.Text = "Full screen resolution:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(9, 144);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(118, 13);
            this.Label5.TabIndex = 123;
            this.Label5.Text = "Mouse click zoom step:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(3, 118);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(124, 13);
            this.Label4.TabIndex = 121;
            this.Label4.Text = "Mouse wheel zoom step:";
            // 
            // TransparentInactiveBox
            // 
            this.TransparentInactiveBox.AutoSize = true;
            this.TransparentInactiveBox.Location = new System.Drawing.Point(147, 44);
            this.TransparentInactiveBox.Name = "TransparentInactiveBox";
            this.TransparentInactiveBox.Size = new System.Drawing.Size(159, 17);
            this.TransparentInactiveBox.TabIndex = 113;
            this.TransparentInactiveBox.Text = "Transparent inactive players";
            this.TransparentInactiveBox.UseVisualStyleBackColor = true;
            // 
            // PictBackGroundBox
            // 
            this.PictBackGroundBox.AutoSize = true;
            this.PictBackGroundBox.Location = new System.Drawing.Point(6, 44);
            this.PictBackGroundBox.Name = "PictBackGroundBox";
            this.PictBackGroundBox.Size = new System.Drawing.Size(135, 17);
            this.PictBackGroundBox.TabIndex = 112;
            this.PictBackGroundBox.Text = "Pictures in background";
            this.PictBackGroundBox.UseVisualStyleBackColor = true;
            // 
            // LockedCamBox
            // 
            this.LockedCamBox.AutoSize = true;
            this.LockedCamBox.Location = new System.Drawing.Point(147, 6);
            this.LockedCamBox.Name = "LockedCamBox";
            this.LockedCamBox.Size = new System.Drawing.Size(100, 17);
            this.LockedCamBox.TabIndex = 108;
            this.LockedCamBox.Text = "Locked camera";
            this.LockedCamBox.UseVisualStyleBackColor = true;
            // 
            // FollowDriverBox
            // 
            this.FollowDriverBox.AutoSize = true;
            this.FollowDriverBox.Location = new System.Drawing.Point(6, 25);
            this.FollowDriverBox.Name = "FollowDriverBox";
            this.FollowDriverBox.Size = new System.Drawing.Size(85, 17);
            this.FollowDriverBox.TabIndex = 54;
            this.FollowDriverBox.Text = "Follow driver";
            this.FollowDriverBox.UseVisualStyleBackColor = true;
            // 
            // ShowDriverPathBox
            // 
            this.ShowDriverPathBox.AutoSize = true;
            this.ShowDriverPathBox.Location = new System.Drawing.Point(6, 6);
            this.ShowDriverPathBox.Name = "ShowDriverPathBox";
            this.ShowDriverPathBox.Size = new System.Drawing.Size(106, 17);
            this.ShowDriverPathBox.TabIndex = 52;
            this.ShowDriverPathBox.Text = "Show driver path";
            this.ShowDriverPathBox.UseVisualStyleBackColor = true;
            // 
            // LoopPlayingBox
            // 
            this.LoopPlayingBox.AutoSize = true;
            this.LoopPlayingBox.Location = new System.Drawing.Point(147, 25);
            this.LoopPlayingBox.Name = "LoopPlayingBox";
            this.LoopPlayingBox.Size = new System.Drawing.Size(86, 17);
            this.LoopPlayingBox.TabIndex = 72;
            this.LoopPlayingBox.Text = "Loop playing";
            this.LoopPlayingBox.UseVisualStyleBackColor = true;
            // 
            // ReplayViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 335);
            this.Controls.Add(this.ViewerBox);
            this.Controls.Add(this.TabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
	}
	
}
