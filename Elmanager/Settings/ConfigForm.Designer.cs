using System.Windows.Forms;

namespace Elmanager.Settings
{
    internal partial class ConfigForm
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
		private System.ComponentModel.IContainer components = null;
		
		//The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		private void InitializeComponent()
			{
            this.components = new System.ComponentModel.Container();
            this.RecDirButton = new System.Windows.Forms.Button();
            this.LevDirButton = new System.Windows.Forms.Button();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.RecTextBox = new System.Windows.Forms.TextBox();
            this.LevTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DisableFrameBufferUsageCheckBox = new System.Windows.Forms.CheckBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.ElmaDirButton = new System.Windows.Forms.Button();
            this.LGRDirButton = new System.Windows.Forms.Button();
            this.LGRTextBox = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.CheckForUpdatesBox = new System.Windows.Forms.CheckBox();
            this.rmTab = new System.Windows.Forms.TabPage();
            this.showTooltipForReplaysCheckBox = new System.Windows.Forms.CheckBox();
            this.DeleteConfirmCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchRecSubDirsBox = new System.Windows.Forms.CheckBox();
            this.NitroBox = new System.Windows.Forms.CheckBox();
            this.SearchLevSubDirsBox = new System.Windows.Forms.CheckBox();
            this.ShowReplayListGridBox = new System.Windows.Forms.CheckBox();
            this.lmTab = new System.Windows.Forms.TabPage();
            this.lmShowTooltip = new System.Windows.Forms.CheckBox();
            this.lmConfirmDeletion = new System.Windows.Forms.CheckBox();
            this.lmSearchRecSubDirs = new System.Windows.Forms.CheckBox();
            this.lmSearchLevSubDirs = new System.Windows.Forms.CheckBox();
            this.lmShowGrid = new System.Windows.Forms.CheckBox();
            this.sleTab = new System.Windows.Forms.TabPage();
            this.alwaysSetDefaultsInPictureTool = new System.Windows.Forms.CheckBox();
            this.capturePicTextFromBordersCheckBox = new System.Windows.Forms.CheckBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.numberFormatBox = new System.Windows.Forms.TextBox();
            this.RenderingSettingsButton = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.crosshairPanel = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.SelectionPanel = new System.Windows.Forms.Panel();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.HighlightPanel = new System.Windows.Forms.Panel();
            this.SameAsFilenameBox = new System.Windows.Forms.CheckBox();
            this.DefaultTitleBox = new System.Windows.Forms.TextBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.baseFilenameBox = new System.Windows.Forms.TextBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.FilenameSuggestionBox = new System.Windows.Forms.CheckBox();
            this.HighlightBox = new System.Windows.Forms.CheckBox();
            this.DynamicCheckTopologyBox = new System.Windows.Forms.CheckBox();
            this.CheckTopologyWhenSavingBox = new System.Windows.Forms.CheckBox();
            this.CaptureRadiusBox = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.LevelTemplateBox = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.TabControl1.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.rmTab.SuspendLayout();
            this.lmTab.SuspendLayout();
            this.sleTab.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // RecDirButton
            // 
            this.RecDirButton.Location = new System.Drawing.Point(360, 3);
            this.RecDirButton.Name = "RecDirButton";
            this.RecDirButton.Size = new System.Drawing.Size(67, 25);
            this.RecDirButton.TabIndex = 0;
            this.RecDirButton.Text = "Browse...";
            this.RecDirButton.UseVisualStyleBackColor = true;
            this.RecDirButton.Click += new System.EventHandler(this.BrowseReplayFolder);
            // 
            // LevDirButton
            // 
            this.LevDirButton.Location = new System.Drawing.Point(360, 65);
            this.LevDirButton.Name = "LevDirButton";
            this.LevDirButton.Size = new System.Drawing.Size(67, 25);
            this.LevDirButton.TabIndex = 1;
            this.LevDirButton.Text = "Browse...";
            this.LevDirButton.UseVisualStyleBackColor = true;
            this.LevDirButton.Click += new System.EventHandler(this.BrowseLevelFolder);
            // 
            // RecTextBox
            // 
            this.RecTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RecTextBox.Location = new System.Drawing.Point(104, 4);
            this.RecTextBox.Name = "RecTextBox";
            this.RecTextBox.ReadOnly = true;
            this.RecTextBox.Size = new System.Drawing.Size(250, 23);
            this.RecTextBox.TabIndex = 44;
            // 
            // LevTextBox
            // 
            this.LevTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LevTextBox.Location = new System.Drawing.Point(104, 35);
            this.LevTextBox.Name = "LevTextBox";
            this.LevTextBox.ReadOnly = true;
            this.LevTextBox.Size = new System.Drawing.Size(250, 23);
            this.LevTextBox.TabIndex = 45;
            // 
            // Label2
            // 
            this.Label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(3, 8);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(95, 15);
            this.Label2.TabIndex = 46;
            this.Label2.Text = "Replay directory:";
            // 
            // Label3
            // 
            this.Label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(11, 39);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(87, 15);
            this.Label3.TabIndex = 47;
            this.Label3.Text = "Level directory:";
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.CheckFileExists = false;
            this.OpenFileDialog1.CheckPathExists = false;
            this.OpenFileDialog1.Filter = "Uncompressed replay databases|*.db|Compressed replay databases|*.cdb|All replay d" +
    "atabases (*.db), (*.cdb)|*.db;*.cdb";
            this.OpenFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.generalTab);
            this.TabControl1.Controls.Add(this.rmTab);
            this.TabControl1.Controls.Add(this.lmTab);
            this.TabControl1.Controls.Add(this.sleTab);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(639, 336);
            this.TabControl1.TabIndex = 55;
            // 
            // generalTab
            // 
            this.generalTab.BackColor = System.Drawing.Color.White;
            this.generalTab.Controls.Add(this.tableLayoutPanel1);
            this.generalTab.Controls.Add(this.groupBox2);
            this.generalTab.Controls.Add(this.resetButton);
            this.generalTab.Controls.Add(this.CheckForUpdatesBox);
            this.generalTab.Location = new System.Drawing.Point(4, 24);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(3);
            this.generalTab.Size = new System.Drawing.Size(631, 308);
            this.generalTab.TabIndex = 0;
            this.generalTab.Text = "General";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(8, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 47);
            this.groupBox2.TabIndex = 56;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Workarounds";
            // 
            // DisableFrameBufferUsageCheckBox
            // 
            this.DisableFrameBufferUsageCheckBox.AutoSize = true;
            this.DisableFrameBufferUsageCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisableFrameBufferUsageCheckBox.Location = new System.Drawing.Point(0, 0);
            this.DisableFrameBufferUsageCheckBox.Name = "DisableFrameBufferUsageCheckBox";
            this.DisableFrameBufferUsageCheckBox.Size = new System.Drawing.Size(194, 25);
            this.DisableFrameBufferUsageCheckBox.TabIndex = 55;
            this.DisableFrameBufferUsageCheckBox.Text = "Disable framebuffer usage";
            this.DisableFrameBufferUsageCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.resetButton.Location = new System.Drawing.Point(8, 274);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(161, 25);
            this.resetButton.TabIndex = 53;
            this.resetButton.Text = "Reset settings to default";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // ElmaDirButton
            // 
            this.ElmaDirButton.Location = new System.Drawing.Point(433, 34);
            this.ElmaDirButton.Name = "ElmaDirButton";
            this.ElmaDirButton.Size = new System.Drawing.Size(130, 25);
            this.ElmaDirButton.TabIndex = 52;
            this.ElmaDirButton.Text = "Get all from Elma dir";
            this.ElmaDirButton.UseVisualStyleBackColor = true;
            this.ElmaDirButton.Click += new System.EventHandler(this.BrowseForElmaDir);
            // 
            // LGRDirButton
            // 
            this.LGRDirButton.Location = new System.Drawing.Point(360, 34);
            this.LGRDirButton.Name = "LGRDirButton";
            this.LGRDirButton.Size = new System.Drawing.Size(67, 25);
            this.LGRDirButton.TabIndex = 49;
            this.LGRDirButton.Text = "Browse...";
            this.LGRDirButton.UseVisualStyleBackColor = true;
            this.LGRDirButton.Click += new System.EventHandler(this.BrowseLgrFolder);
            // 
            // LGRTextBox
            // 
            this.LGRTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LGRTextBox.Location = new System.Drawing.Point(104, 66);
            this.LGRTextBox.Name = "LGRTextBox";
            this.LGRTextBox.ReadOnly = true;
            this.LGRTextBox.Size = new System.Drawing.Size(250, 23);
            this.LGRTextBox.TabIndex = 50;
            // 
            // Label5
            // 
            this.Label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(17, 70);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(81, 15);
            this.Label5.TabIndex = 51;
            this.Label5.Text = "LGR directory:";
            // 
            // CheckForUpdatesBox
            // 
            this.CheckForUpdatesBox.AutoSize = true;
            this.CheckForUpdatesBox.Location = new System.Drawing.Point(8, 125);
            this.CheckForUpdatesBox.Name = "CheckForUpdatesBox";
            this.CheckForUpdatesBox.Size = new System.Drawing.Size(234, 19);
            this.CheckForUpdatesBox.TabIndex = 48;
            this.CheckForUpdatesBox.Text = "Check for Elmanager updates at startup";
            this.CheckForUpdatesBox.UseVisualStyleBackColor = true;
            // 
            // rmTab
            // 
            this.rmTab.Controls.Add(this.showTooltipForReplaysCheckBox);
            this.rmTab.Controls.Add(this.DeleteConfirmCheckBox);
            this.rmTab.Controls.Add(this.SearchRecSubDirsBox);
            this.rmTab.Controls.Add(this.NitroBox);
            this.rmTab.Controls.Add(this.SearchLevSubDirsBox);
            this.rmTab.Controls.Add(this.ShowReplayListGridBox);
            this.rmTab.Location = new System.Drawing.Point(4, 24);
            this.rmTab.Name = "rmTab";
            this.rmTab.Padding = new System.Windows.Forms.Padding(3);
            this.rmTab.Size = new System.Drawing.Size(631, 308);
            this.rmTab.TabIndex = 3;
            this.rmTab.Text = "Replay manager";
            this.rmTab.UseVisualStyleBackColor = true;
            // 
            // showTooltipForReplaysCheckBox
            // 
            this.showTooltipForReplaysCheckBox.AutoSize = true;
            this.showTooltipForReplaysCheckBox.Location = new System.Drawing.Point(8, 52);
            this.showTooltipForReplaysCheckBox.Name = "showTooltipForReplaysCheckBox";
            this.showTooltipForReplaysCheckBox.Size = new System.Drawing.Size(217, 19);
            this.showTooltipForReplaysCheckBox.TabIndex = 58;
            this.showTooltipForReplaysCheckBox.Text = "Show tooltip for replays in replay list";
            this.showTooltipForReplaysCheckBox.UseVisualStyleBackColor = true;
            // 
            // DeleteConfirmCheckBox
            // 
            this.DeleteConfirmCheckBox.AutoSize = true;
            this.DeleteConfirmCheckBox.Location = new System.Drawing.Point(258, 29);
            this.DeleteConfirmCheckBox.Name = "DeleteConfirmCheckBox";
            this.DeleteConfirmCheckBox.Size = new System.Drawing.Size(116, 19);
            this.DeleteConfirmCheckBox.TabIndex = 57;
            this.DeleteConfirmCheckBox.Text = "Confirm deletion";
            this.DeleteConfirmCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchRecSubDirsBox
            // 
            this.SearchRecSubDirsBox.AutoSize = true;
            this.SearchRecSubDirsBox.Location = new System.Drawing.Point(8, 6);
            this.SearchRecSubDirsBox.Name = "SearchRecSubDirsBox";
            this.SearchRecSubDirsBox.Size = new System.Drawing.Size(244, 19);
            this.SearchRecSubDirsBox.TabIndex = 55;
            this.SearchRecSubDirsBox.Text = "Search also subdirectories in replay folder";
            this.SearchRecSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // NitroBox
            // 
            this.NitroBox.AutoSize = true;
            this.NitroBox.Location = new System.Drawing.Point(258, 52);
            this.NitroBox.Name = "NitroBox";
            this.NitroBox.Size = new System.Drawing.Size(191, 19);
            this.NitroBox.TabIndex = 53;
            this.NitroBox.Text = "Treat Nitro replays as erroneous";
            this.NitroBox.UseVisualStyleBackColor = true;
            // 
            // SearchLevSubDirsBox
            // 
            this.SearchLevSubDirsBox.AutoSize = true;
            this.SearchLevSubDirsBox.Location = new System.Drawing.Point(258, 6);
            this.SearchLevSubDirsBox.Name = "SearchLevSubDirsBox";
            this.SearchLevSubDirsBox.Size = new System.Drawing.Size(236, 19);
            this.SearchLevSubDirsBox.TabIndex = 56;
            this.SearchLevSubDirsBox.Text = "Search also subdirectories in level folder";
            this.SearchLevSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // ShowReplayListGridBox
            // 
            this.ShowReplayListGridBox.AutoSize = true;
            this.ShowReplayListGridBox.Location = new System.Drawing.Point(8, 29);
            this.ShowReplayListGridBox.Name = "ShowReplayListGridBox";
            this.ShowReplayListGridBox.Size = new System.Drawing.Size(145, 19);
            this.ShowReplayListGridBox.TabIndex = 54;
            this.ShowReplayListGridBox.Text = "Show grid in replay list";
            this.ShowReplayListGridBox.UseVisualStyleBackColor = true;
            // 
            // lmTab
            // 
            this.lmTab.Controls.Add(this.lmShowTooltip);
            this.lmTab.Controls.Add(this.lmConfirmDeletion);
            this.lmTab.Controls.Add(this.lmSearchRecSubDirs);
            this.lmTab.Controls.Add(this.lmSearchLevSubDirs);
            this.lmTab.Controls.Add(this.lmShowGrid);
            this.lmTab.Location = new System.Drawing.Point(4, 24);
            this.lmTab.Name = "lmTab";
            this.lmTab.Padding = new System.Windows.Forms.Padding(3);
            this.lmTab.Size = new System.Drawing.Size(631, 308);
            this.lmTab.TabIndex = 5;
            this.lmTab.Text = "Level manager";
            this.lmTab.UseVisualStyleBackColor = true;
            // 
            // lmShowTooltip
            // 
            this.lmShowTooltip.AutoSize = true;
            this.lmShowTooltip.Location = new System.Drawing.Point(8, 52);
            this.lmShowTooltip.Name = "lmShowTooltip";
            this.lmShowTooltip.Size = new System.Drawing.Size(201, 19);
            this.lmShowTooltip.TabIndex = 65;
            this.lmShowTooltip.Text = "Show tooltip for levels in level list";
            this.lmShowTooltip.UseVisualStyleBackColor = true;
            // 
            // lmConfirmDeletion
            // 
            this.lmConfirmDeletion.AutoSize = true;
            this.lmConfirmDeletion.Location = new System.Drawing.Point(258, 29);
            this.lmConfirmDeletion.Name = "lmConfirmDeletion";
            this.lmConfirmDeletion.Size = new System.Drawing.Size(116, 19);
            this.lmConfirmDeletion.TabIndex = 64;
            this.lmConfirmDeletion.Text = "Confirm deletion";
            this.lmConfirmDeletion.UseVisualStyleBackColor = true;
            // 
            // lmSearchRecSubDirs
            // 
            this.lmSearchRecSubDirs.AutoSize = true;
            this.lmSearchRecSubDirs.Location = new System.Drawing.Point(8, 6);
            this.lmSearchRecSubDirs.Name = "lmSearchRecSubDirs";
            this.lmSearchRecSubDirs.Size = new System.Drawing.Size(244, 19);
            this.lmSearchRecSubDirs.TabIndex = 62;
            this.lmSearchRecSubDirs.Text = "Search also subdirectories in replay folder";
            this.lmSearchRecSubDirs.UseVisualStyleBackColor = true;
            // 
            // lmSearchLevSubDirs
            // 
            this.lmSearchLevSubDirs.AutoSize = true;
            this.lmSearchLevSubDirs.Location = new System.Drawing.Point(258, 6);
            this.lmSearchLevSubDirs.Name = "lmSearchLevSubDirs";
            this.lmSearchLevSubDirs.Size = new System.Drawing.Size(236, 19);
            this.lmSearchLevSubDirs.TabIndex = 63;
            this.lmSearchLevSubDirs.Text = "Search also subdirectories in level folder";
            this.lmSearchLevSubDirs.UseVisualStyleBackColor = true;
            // 
            // lmShowGrid
            // 
            this.lmShowGrid.AutoSize = true;
            this.lmShowGrid.Location = new System.Drawing.Point(8, 29);
            this.lmShowGrid.Name = "lmShowGrid";
            this.lmShowGrid.Size = new System.Drawing.Size(137, 19);
            this.lmShowGrid.TabIndex = 61;
            this.lmShowGrid.Text = "Show grid in level list";
            this.lmShowGrid.UseVisualStyleBackColor = true;
            // 
            // sleTab
            // 
            this.sleTab.Controls.Add(this.tableLayoutPanel3);
            this.sleTab.Controls.Add(this.tableLayoutPanel2);
            this.sleTab.Controls.Add(this.HighlightBox);
            this.sleTab.Controls.Add(this.alwaysSetDefaultsInPictureTool);
            this.sleTab.Controls.Add(this.capturePicTextFromBordersCheckBox);
            this.sleTab.Controls.Add(this.RenderingSettingsButton);
            this.sleTab.Controls.Add(this.GroupBox1);
            this.sleTab.Controls.Add(this.FilenameSuggestionBox);
            this.sleTab.Controls.Add(this.DynamicCheckTopologyBox);
            this.sleTab.Controls.Add(this.CheckTopologyWhenSavingBox);
            this.sleTab.Location = new System.Drawing.Point(4, 24);
            this.sleTab.Name = "sleTab";
            this.sleTab.Size = new System.Drawing.Size(631, 308);
            this.sleTab.TabIndex = 4;
            this.sleTab.Text = "SLE";
            this.sleTab.UseVisualStyleBackColor = true;
            // 
            // alwaysSetDefaultsInPictureTool
            // 
            this.alwaysSetDefaultsInPictureTool.AutoSize = true;
            this.alwaysSetDefaultsInPictureTool.Location = new System.Drawing.Point(200, 138);
            this.alwaysSetDefaultsInPictureTool.Name = "alwaysSetDefaultsInPictureTool";
            this.alwaysSetDefaultsInPictureTool.Size = new System.Drawing.Size(423, 19);
            this.alwaysSetDefaultsInPictureTool.TabIndex = 31;
            this.alwaysSetDefaultsInPictureTool.Text = "Always set distance and clipping to defaults when changing picture/texture";
            this.alwaysSetDefaultsInPictureTool.UseVisualStyleBackColor = true;
            // 
            // capturePicTextFromBordersCheckBox
            // 
            this.capturePicTextFromBordersCheckBox.AutoSize = true;
            this.capturePicTextFromBordersCheckBox.Location = new System.Drawing.Point(17, 92);
            this.capturePicTextFromBordersCheckBox.Name = "capturePicTextFromBordersCheckBox";
            this.capturePicTextFromBordersCheckBox.Size = new System.Drawing.Size(279, 19);
            this.capturePicTextFromBordersCheckBox.TabIndex = 30;
            this.capturePicTextFromBordersCheckBox.Text = "Capture pictures and textures from borders only";
            this.capturePicTextFromBordersCheckBox.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.browseButton.Location = new System.Drawing.Point(339, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 25);
            this.browseButton.TabIndex = 29;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // numberFormatBox
            // 
            this.numberFormatBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numberFormatBox.Location = new System.Drawing.Point(345, 3);
            this.numberFormatBox.Name = "numberFormatBox";
            this.numberFormatBox.Size = new System.Drawing.Size(100, 23);
            this.numberFormatBox.TabIndex = 28;
            // 
            // RenderingSettingsButton
            // 
            this.RenderingSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RenderingSettingsButton.Location = new System.Drawing.Point(229, 273);
            this.RenderingSettingsButton.Name = "RenderingSettingsButton";
            this.RenderingSettingsButton.Size = new System.Drawing.Size(115, 25);
            this.RenderingSettingsButton.TabIndex = 27;
            this.RenderingSettingsButton.Text = "Rendering settings";
            this.RenderingSettingsButton.UseVisualStyleBackColor = true;
            this.RenderingSettingsButton.Click += new System.EventHandler(this.RenderingSettingsButtonClick);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupBox1.Controls.Add(this.crosshairPanel);
            this.GroupBox1.Controls.Add(this.label9);
            this.GroupBox1.Controls.Add(this.SelectionPanel);
            this.GroupBox1.Controls.Add(this.Label15);
            this.GroupBox1.Controls.Add(this.Label17);
            this.GroupBox1.Controls.Add(this.HighlightPanel);
            this.GroupBox1.Location = new System.Drawing.Point(8, 220);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(207, 78);
            this.GroupBox1.TabIndex = 26;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Colors";
            // 
            // crosshairPanel
            // 
            this.crosshairPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crosshairPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crosshairPanel.Location = new System.Drawing.Point(6, 47);
            this.crosshairPanel.Name = "crosshairPanel";
            this.crosshairPanel.Size = new System.Drawing.Size(20, 20);
            this.crosshairPanel.TabIndex = 9;
            this.crosshairPanel.Click += new System.EventHandler(this.PanelClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 15);
            this.label9.TabIndex = 10;
            this.label9.Text = "Crosshair";
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectionPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectionPanel.Location = new System.Drawing.Point(6, 22);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.Size = new System.Drawing.Size(20, 20);
            this.SelectionPanel.TabIndex = 1;
            this.SelectionPanel.Click += new System.EventHandler(this.PanelClick);
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(32, 26);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(55, 15);
            this.Label15.TabIndex = 8;
            this.Label15.Text = "Selection";
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(142, 26);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(57, 15);
            this.Label17.TabIndex = 10;
            this.Label17.Text = "Highlight";
            // 
            // HighlightPanel
            // 
            this.HighlightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HighlightPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HighlightPanel.Location = new System.Drawing.Point(116, 22);
            this.HighlightPanel.Name = "HighlightPanel";
            this.HighlightPanel.Size = new System.Drawing.Size(20, 20);
            this.HighlightPanel.TabIndex = 2;
            this.HighlightPanel.Click += new System.EventHandler(this.PanelClick);
            // 
            // SameAsFilenameBox
            // 
            this.SameAsFilenameBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SameAsFilenameBox.AutoSize = true;
            this.SameAsFilenameBox.Location = new System.Drawing.Point(213, 34);
            this.SameAsFilenameBox.Name = "SameAsFilenameBox";
            this.SameAsFilenameBox.Size = new System.Drawing.Size(118, 19);
            this.SameAsFilenameBox.TabIndex = 19;
            this.SameAsFilenameBox.Text = "Same as filename";
            this.SameAsFilenameBox.UseVisualStyleBackColor = true;
            this.SameAsFilenameBox.CheckedChanged += new System.EventHandler(this.SameAsFilenameBoxCheckedChanged);
            // 
            // DefaultTitleBox
            // 
            this.DefaultTitleBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DefaultTitleBox.Location = new System.Drawing.Point(107, 32);
            this.DefaultTitleBox.MaxLength = 50;
            this.DefaultTitleBox.Name = "DefaultTitleBox";
            this.DefaultTitleBox.Size = new System.Drawing.Size(100, 23);
            this.DefaultTitleBox.TabIndex = 18;
            this.DefaultTitleBox.Text = "New level";
            // 
            // Label20
            // 
            this.Label20.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label20.AutoSize = true;
            this.Label20.Location = new System.Drawing.Point(3, 36);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(98, 15);
            this.Label20.TabIndex = 17;
            this.Label20.Text = "Default level title:";
            // 
            // baseFilenameBox
            // 
            this.baseFilenameBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.baseFilenameBox.Location = new System.Drawing.Point(107, 3);
            this.baseFilenameBox.Name = "baseFilenameBox";
            this.baseFilenameBox.Size = new System.Drawing.Size(100, 23);
            this.baseFilenameBox.TabIndex = 15;
            this.baseFilenameBox.Text = "MyLev";
            // 
            // Label19
            // 
            this.Label19.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label19.AutoSize = true;
            this.Label19.Location = new System.Drawing.Point(213, 7);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(126, 15);
            this.Label19.TabIndex = 14;
            this.Label19.Text = "Number format string:";
            // 
            // Label18
            // 
            this.Label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(21, 7);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(80, 15);
            this.Label18.TabIndex = 13;
            this.Label18.Text = "Basefilename:";
            // 
            // FilenameSuggestionBox
            // 
            this.FilenameSuggestionBox.AutoSize = true;
            this.FilenameSuggestionBox.Location = new System.Drawing.Point(17, 138);
            this.FilenameSuggestionBox.Name = "FilenameSuggestionBox";
            this.FilenameSuggestionBox.Size = new System.Drawing.Size(155, 19);
            this.FilenameSuggestionBox.TabIndex = 12;
            this.FilenameSuggestionBox.Text = "Use filename suggestion";
            this.FilenameSuggestionBox.UseVisualStyleBackColor = true;
            this.FilenameSuggestionBox.CheckedChanged += new System.EventHandler(this.FilenameSuggestionBoxCheckedChanged);
            // 
            // HighlightBox
            // 
            this.HighlightBox.AutoSize = true;
            this.HighlightBox.Location = new System.Drawing.Point(17, 69);
            this.HighlightBox.Name = "HighlightBox";
            this.HighlightBox.Size = new System.Drawing.Size(227, 19);
            this.HighlightBox.TabIndex = 10;
            this.HighlightBox.Text = "Highlight level elements under mouse";
            this.HighlightBox.UseVisualStyleBackColor = true;
            // 
            // DynamicCheckTopologyBox
            // 
            this.DynamicCheckTopologyBox.AutoSize = true;
            this.DynamicCheckTopologyBox.Location = new System.Drawing.Point(17, 115);
            this.DynamicCheckTopologyBox.Name = "DynamicCheckTopologyBox";
            this.DynamicCheckTopologyBox.Size = new System.Drawing.Size(177, 19);
            this.DynamicCheckTopologyBox.TabIndex = 7;
            this.DynamicCheckTopologyBox.Text = "Check topology dynamically";
            this.DynamicCheckTopologyBox.UseVisualStyleBackColor = true;
            // 
            // CheckTopologyWhenSavingBox
            // 
            this.CheckTopologyWhenSavingBox.AutoSize = true;
            this.CheckTopologyWhenSavingBox.Location = new System.Drawing.Point(200, 115);
            this.CheckTopologyWhenSavingBox.Name = "CheckTopologyWhenSavingBox";
            this.CheckTopologyWhenSavingBox.Size = new System.Drawing.Size(206, 19);
            this.CheckTopologyWhenSavingBox.TabIndex = 6;
            this.CheckTopologyWhenSavingBox.Text = "Check topology when saving level";
            this.CheckTopologyWhenSavingBox.UseVisualStyleBackColor = true;
            // 
            // CaptureRadiusBox
            // 
            this.CaptureRadiusBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CaptureRadiusBox.Location = new System.Drawing.Point(133, 34);
            this.CaptureRadiusBox.Name = "CaptureRadiusBox";
            this.CaptureRadiusBox.Size = new System.Drawing.Size(50, 23);
            this.CaptureRadiusBox.TabIndex = 5;
            this.CaptureRadiusBox.Text = "100";
            // 
            // Label8
            // 
            this.Label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(3, 38);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(124, 15);
            this.Label8.TabIndex = 4;
            this.Label8.Text = "Mouse capture radius:";
            // 
            // LevelTemplateBox
            // 
            this.LevelTemplateBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LevelTemplateBox.Location = new System.Drawing.Point(133, 4);
            this.LevelTemplateBox.Name = "LevelTemplateBox";
            this.LevelTemplateBox.Size = new System.Drawing.Size(200, 23);
            this.LevelTemplateBox.TabIndex = 2;
            this.LevelTemplateBox.Text = "50,50";
            this.toolTip1.SetToolTip(this.LevelTemplateBox, "width,height or browse for a level template");
            // 
            // Label6
            // 
            this.Label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(16, 8);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(111, 15);
            this.Label6.TabIndex = 0;
            this.Label6.Text = "New level template:";
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.FullOpen = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.Label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ElmaDirButton, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.RecTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.LGRDirButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.LevDirButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.LevTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LGRTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.RecDirButton, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 93);
            this.tableLayoutPanel1.TabIndex = 57;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DisableFrameBufferUsageCheckBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 25);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.CaptureRadiusBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.LevelTemplateBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.browseButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label8, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(11, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(417, 60);
            this.tableLayoutPanel2.TabIndex = 32;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.Label18, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.Label20, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.baseFilenameBox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.DefaultTitleBox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.Label19, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.numberFormatBox, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.SameAsFilenameBox, 2, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 163);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(448, 58);
            this.tableLayoutPanel3.TabIndex = 33;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(639, 336);
            this.Controls.Add(this.TabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Elmanager configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveSettings);
            this.TabControl1.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.generalTab.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.rmTab.ResumeLayout(false);
            this.rmTab.PerformLayout();
            this.lmTab.ResumeLayout(false);
            this.lmTab.PerformLayout();
            this.sleTab.ResumeLayout(false);
            this.sleTab.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }
		internal System.Windows.Forms.Button RecDirButton;
		internal System.Windows.Forms.Button LevDirButton;
		internal System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
		internal System.Windows.Forms.TextBox RecTextBox;
		internal System.Windows.Forms.TextBox LevTextBox;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
		internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage generalTab;
		internal System.Windows.Forms.TabPage rmTab;
		internal System.Windows.Forms.CheckBox DeleteConfirmCheckBox;
		internal System.Windows.Forms.CheckBox SearchRecSubDirsBox;
		internal System.Windows.Forms.CheckBox NitroBox;
		internal System.Windows.Forms.CheckBox SearchLevSubDirsBox;
		internal System.Windows.Forms.CheckBox ShowReplayListGridBox;
		internal System.Windows.Forms.TabPage sleTab;
		internal System.Windows.Forms.TextBox LevelTemplateBox;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.TextBox CaptureRadiusBox;
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.CheckBox CheckTopologyWhenSavingBox;
		internal System.Windows.Forms.CheckBox DynamicCheckTopologyBox;
		internal System.Windows.Forms.Panel HighlightPanel;
		internal System.Windows.Forms.Label Label17;
		internal System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Panel SelectionPanel;
		internal System.Windows.Forms.CheckBox HighlightBox;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
		internal System.Windows.Forms.TextBox baseFilenameBox;
		internal System.Windows.Forms.Label Label19;
		internal System.Windows.Forms.Label Label18;
		internal System.Windows.Forms.CheckBox FilenameSuggestionBox;
		internal System.Windows.Forms.CheckBox SameAsFilenameBox;
		internal System.Windows.Forms.TextBox DefaultTitleBox;
        internal System.Windows.Forms.Label Label20;
		internal System.Windows.Forms.CheckBox CheckForUpdatesBox;
		internal System.Windows.Forms.GroupBox GroupBox1;
		internal System.Windows.Forms.Button RenderingSettingsButton;
		internal System.Windows.Forms.Button LGRDirButton;
		internal System.Windows.Forms.TextBox LGRTextBox;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Button ElmaDirButton;
        private Button resetButton;
        private TextBox numberFormatBox;
        internal Panel crosshairPanel;
        internal Label label9;
        private Button browseButton;
        private ToolTip toolTip1;
        internal CheckBox capturePicTextFromBordersCheckBox;
        private GroupBox groupBox2;
        private CheckBox DisableFrameBufferUsageCheckBox;
        internal CheckBox showTooltipForReplaysCheckBox;
        internal CheckBox alwaysSetDefaultsInPictureTool;
        private TabPage lmTab;
        internal CheckBox lmShowTooltip;
        internal CheckBox lmConfirmDeletion;
        internal CheckBox lmSearchRecSubDirs;
        internal CheckBox lmSearchLevSubDirs;
        internal CheckBox lmShowGrid;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
    }
	
}
