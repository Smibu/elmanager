using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class ConfigForm : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.RecDirButton = new System.Windows.Forms.Button();
            this.LevDirButton = new System.Windows.Forms.Button();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.CheckBox6 = new System.Windows.Forms.CheckBox();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.RecTextBox = new System.Windows.Forms.TextBox();
            this.LevTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.DBTextBox = new System.Windows.Forms.TextBox();
            this.SetPathButton = new System.Windows.Forms.Button();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.LoadButton = new System.Windows.Forms.Button();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.GeneralPage = new System.Windows.Forms.TabPage();
            this.generateNativeImageButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.ElmaDirButton = new System.Windows.Forms.Button();
            this.LGRDirButton = new System.Windows.Forms.Button();
            this.LGRTextBox = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.CheckForUpdatesBox = new System.Windows.Forms.CheckBox();
            this.RMPage = new System.Windows.Forms.TabPage();
            this.DeleteConfirmCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchRecSubDirsBox = new System.Windows.Forms.CheckBox();
            this.DeleteRecycleCheckBox = new System.Windows.Forms.CheckBox();
            this.NitroBox = new System.Windows.Forms.CheckBox();
            this.SearchLevSubDirsBox = new System.Windows.Forms.CheckBox();
            this.ShowReplayListGridBox = new System.Windows.Forms.CheckBox();
            this.LevelEditorPage = new System.Windows.Forms.TabPage();
            this.RenderingSettingsButton = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.SelectionPanel = new System.Windows.Forms.Panel();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.HighlightPanel = new System.Windows.Forms.Panel();
            this.AutograssThicknessBox = new System.Windows.Forms.TextBox();
            this.Label23 = new System.Windows.Forms.Label();
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
            this.InitialHeightBox = new System.Windows.Forms.TextBox();
            this.InitialWidthBox = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.DatabasePage = new System.Windows.Forms.TabPage();
            this.Label1 = new System.Windows.Forms.Label();
            this.WarnAboutOldDBBox = new System.Windows.Forms.CheckBox();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.numberFormatBox = new System.Windows.Forms.TextBox();
            this.TabControl1.SuspendLayout();
            this.GeneralPage.SuspendLayout();
            this.RMPage.SuspendLayout();
            this.LevelEditorPage.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.DatabasePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // RecDirButton
            // 
            this.RecDirButton.Location = new System.Drawing.Point(361, 14);
            this.RecDirButton.Name = "RecDirButton";
            this.RecDirButton.Size = new System.Drawing.Size(67, 23);
            this.RecDirButton.TabIndex = 0;
            this.RecDirButton.Text = "Browse...";
            this.RecDirButton.UseVisualStyleBackColor = true;
            this.RecDirButton.Click += new System.EventHandler(this.BrowseReplayFolder);
            // 
            // LevDirButton
            // 
            this.LevDirButton.Location = new System.Drawing.Point(361, 40);
            this.LevDirButton.Name = "LevDirButton";
            this.LevDirButton.Size = new System.Drawing.Size(67, 23);
            this.LevDirButton.TabIndex = 1;
            this.LevDirButton.Text = "Browse...";
            this.LevDirButton.UseVisualStyleBackColor = true;
            this.LevDirButton.Click += new System.EventHandler(this.BrowseLevelFolder);
            // 
            // CheckBox6
            // 
            this.CheckBox6.AutoSize = true;
            this.CheckBox6.Enabled = false;
            this.CheckBox6.Location = new System.Drawing.Point(360, 17);
            this.CheckBox6.Name = "CheckBox6";
            this.CheckBox6.Size = new System.Drawing.Size(123, 17);
            this.CheckBox6.TabIndex = 9;
            this.CheckBox6.Text = "Use replay database";
            this.CheckBox6.UseVisualStyleBackColor = true;
            // 
            // GenerateButton
            // 
            this.GenerateButton.Location = new System.Drawing.Point(9, 51);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(150, 23);
            this.GenerateButton.TabIndex = 10;
            this.GenerateButton.Text = "Generate/update database";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateDataBaseClick);
            // 
            // RecTextBox
            // 
            this.RecTextBox.Location = new System.Drawing.Point(105, 16);
            this.RecTextBox.Name = "RecTextBox";
            this.RecTextBox.ReadOnly = true;
            this.RecTextBox.Size = new System.Drawing.Size(250, 20);
            this.RecTextBox.TabIndex = 44;
            // 
            // LevTextBox
            // 
            this.LevTextBox.Location = new System.Drawing.Point(105, 42);
            this.LevTextBox.Name = "LevTextBox";
            this.LevTextBox.ReadOnly = true;
            this.LevTextBox.Size = new System.Drawing.Size(250, 20);
            this.LevTextBox.TabIndex = 45;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(13, 19);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(86, 13);
            this.Label2.TabIndex = 46;
            this.Label2.Text = "Replay directory:";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(20, 45);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(79, 13);
            this.Label3.TabIndex = 47;
            this.Label3.Text = "Level directory:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(6, 18);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(106, 13);
            this.Label4.TabIndex = 48;
            this.Label4.Text = "Replay database file:";
            // 
            // DBTextBox
            // 
            this.DBTextBox.Location = new System.Drawing.Point(118, 15);
            this.DBTextBox.Name = "DBTextBox";
            this.DBTextBox.ReadOnly = true;
            this.DBTextBox.Size = new System.Drawing.Size(163, 20);
            this.DBTextBox.TabIndex = 49;
            // 
            // SetPathButton
            // 
            this.SetPathButton.Location = new System.Drawing.Point(287, 13);
            this.SetPathButton.Name = "SetPathButton";
            this.SetPathButton.Size = new System.Drawing.Size(67, 23);
            this.SetPathButton.TabIndex = 50;
            this.SetPathButton.Text = "Set path...";
            this.SetPathButton.UseVisualStyleBackColor = true;
            this.SetPathButton.Click += new System.EventHandler(this.SetDbPath);
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.CheckFileExists = false;
            this.OpenFileDialog1.CheckPathExists = false;
            this.OpenFileDialog1.Filter = "Uncompressed replay databases|*.db|Compressed replay databases|*.cdb|All replay d" +
    "atabases (*.db), (*.cdb)|*.db;*.cdb";
            this.OpenFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(165, 51);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(150, 23);
            this.LoadButton.TabIndex = 53;
            this.LoadButton.Text = "Load selected database";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButtonClick);
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.GeneralPage);
            this.TabControl1.Controls.Add(this.RMPage);
            this.TabControl1.Controls.Add(this.LevelEditorPage);
            this.TabControl1.Controls.Add(this.DatabasePage);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(604, 297);
            this.TabControl1.TabIndex = 55;
            // 
            // GeneralPage
            // 
            this.GeneralPage.BackColor = System.Drawing.Color.White;
            this.GeneralPage.Controls.Add(this.generateNativeImageButton);
            this.GeneralPage.Controls.Add(this.resetButton);
            this.GeneralPage.Controls.Add(this.ElmaDirButton);
            this.GeneralPage.Controls.Add(this.LGRDirButton);
            this.GeneralPage.Controls.Add(this.LGRTextBox);
            this.GeneralPage.Controls.Add(this.Label5);
            this.GeneralPage.Controls.Add(this.CheckForUpdatesBox);
            this.GeneralPage.Controls.Add(this.RecTextBox);
            this.GeneralPage.Controls.Add(this.RecDirButton);
            this.GeneralPage.Controls.Add(this.LevDirButton);
            this.GeneralPage.Controls.Add(this.LevTextBox);
            this.GeneralPage.Controls.Add(this.Label2);
            this.GeneralPage.Controls.Add(this.Label3);
            this.GeneralPage.Location = new System.Drawing.Point(4, 22);
            this.GeneralPage.Name = "GeneralPage";
            this.GeneralPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralPage.Size = new System.Drawing.Size(596, 271);
            this.GeneralPage.TabIndex = 0;
            this.GeneralPage.Text = "General";
            // 
            // generateNativeImageButton
            // 
            this.generateNativeImageButton.Location = new System.Drawing.Point(8, 157);
            this.generateNativeImageButton.Name = "generateNativeImageButton";
            this.generateNativeImageButton.Size = new System.Drawing.Size(143, 23);
            this.generateNativeImageButton.TabIndex = 54;
            this.generateNativeImageButton.Text = "Put Elmanager in cache";
            this.generateNativeImageButton.UseVisualStyleBackColor = true;
            this.generateNativeImageButton.Click += new System.EventHandler(this.GenerateNativeImage);
            // 
            // resetButton
            // 
            this.resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resetButton.Location = new System.Drawing.Point(8, 237);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(161, 23);
            this.resetButton.TabIndex = 53;
            this.resetButton.Text = "Reset settings to default";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // ElmaDirButton
            // 
            this.ElmaDirButton.Location = new System.Drawing.Point(457, 40);
            this.ElmaDirButton.Name = "ElmaDirButton";
            this.ElmaDirButton.Size = new System.Drawing.Size(118, 23);
            this.ElmaDirButton.TabIndex = 52;
            this.ElmaDirButton.Text = "Get all from Elma dir";
            this.ElmaDirButton.UseVisualStyleBackColor = true;
            this.ElmaDirButton.Click += new System.EventHandler(this.BrowseForElmaDir);
            // 
            // LGRDirButton
            // 
            this.LGRDirButton.Location = new System.Drawing.Point(361, 66);
            this.LGRDirButton.Name = "LGRDirButton";
            this.LGRDirButton.Size = new System.Drawing.Size(67, 23);
            this.LGRDirButton.TabIndex = 49;
            this.LGRDirButton.Text = "Browse...";
            this.LGRDirButton.UseVisualStyleBackColor = true;
            this.LGRDirButton.Click += new System.EventHandler(this.BrowseLgrFolder);
            // 
            // LGRTextBox
            // 
            this.LGRTextBox.Location = new System.Drawing.Point(105, 68);
            this.LGRTextBox.Name = "LGRTextBox";
            this.LGRTextBox.ReadOnly = true;
            this.LGRTextBox.Size = new System.Drawing.Size(250, 20);
            this.LGRTextBox.TabIndex = 50;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(24, 71);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(75, 13);
            this.Label5.TabIndex = 51;
            this.Label5.Text = "LGR directory:";
            // 
            // CheckForUpdatesBox
            // 
            this.CheckForUpdatesBox.AutoSize = true;
            this.CheckForUpdatesBox.Location = new System.Drawing.Point(8, 125);
            this.CheckForUpdatesBox.Name = "CheckForUpdatesBox";
            this.CheckForUpdatesBox.Size = new System.Drawing.Size(213, 17);
            this.CheckForUpdatesBox.TabIndex = 48;
            this.CheckForUpdatesBox.Text = "Check for Elmanager updates at startup";
            this.CheckForUpdatesBox.UseVisualStyleBackColor = true;
            // 
            // RMPage
            // 
            this.RMPage.Controls.Add(this.DeleteConfirmCheckBox);
            this.RMPage.Controls.Add(this.SearchRecSubDirsBox);
            this.RMPage.Controls.Add(this.DeleteRecycleCheckBox);
            this.RMPage.Controls.Add(this.NitroBox);
            this.RMPage.Controls.Add(this.SearchLevSubDirsBox);
            this.RMPage.Controls.Add(this.ShowReplayListGridBox);
            this.RMPage.Location = new System.Drawing.Point(4, 22);
            this.RMPage.Name = "RMPage";
            this.RMPage.Size = new System.Drawing.Size(596, 271);
            this.RMPage.TabIndex = 3;
            this.RMPage.Text = "Replay manager";
            this.RMPage.UseVisualStyleBackColor = true;
            // 
            // DeleteConfirmCheckBox
            // 
            this.DeleteConfirmCheckBox.AutoSize = true;
            this.DeleteConfirmCheckBox.Location = new System.Drawing.Point(250, 36);
            this.DeleteConfirmCheckBox.Name = "DeleteConfirmCheckBox";
            this.DeleteConfirmCheckBox.Size = new System.Drawing.Size(101, 17);
            this.DeleteConfirmCheckBox.TabIndex = 57;
            this.DeleteConfirmCheckBox.Text = "Confirm deletion";
            this.DeleteConfirmCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchRecSubDirsBox
            // 
            this.SearchRecSubDirsBox.AutoSize = true;
            this.SearchRecSubDirsBox.Location = new System.Drawing.Point(8, 13);
            this.SearchRecSubDirsBox.Name = "SearchRecSubDirsBox";
            this.SearchRecSubDirsBox.Size = new System.Drawing.Size(221, 17);
            this.SearchRecSubDirsBox.TabIndex = 55;
            this.SearchRecSubDirsBox.Text = "Search also subdirectories in replay folder";
            this.SearchRecSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // DeleteRecycleCheckBox
            // 
            this.DeleteRecycleCheckBox.AutoSize = true;
            this.DeleteRecycleCheckBox.Location = new System.Drawing.Point(8, 36);
            this.DeleteRecycleCheckBox.Name = "DeleteRecycleCheckBox";
            this.DeleteRecycleCheckBox.Size = new System.Drawing.Size(128, 17);
            this.DeleteRecycleCheckBox.TabIndex = 52;
            this.DeleteRecycleCheckBox.Text = "Delete to Recycle bin";
            this.DeleteRecycleCheckBox.UseVisualStyleBackColor = true;
            // 
            // NitroBox
            // 
            this.NitroBox.AutoSize = true;
            this.NitroBox.Location = new System.Drawing.Point(8, 59);
            this.NitroBox.Name = "NitroBox";
            this.NitroBox.Size = new System.Drawing.Size(176, 17);
            this.NitroBox.TabIndex = 53;
            this.NitroBox.Text = "Treat Nitro replays as erroneous";
            this.NitroBox.UseVisualStyleBackColor = true;
            // 
            // SearchLevSubDirsBox
            // 
            this.SearchLevSubDirsBox.AutoSize = true;
            this.SearchLevSubDirsBox.Location = new System.Drawing.Point(250, 13);
            this.SearchLevSubDirsBox.Name = "SearchLevSubDirsBox";
            this.SearchLevSubDirsBox.Size = new System.Drawing.Size(215, 17);
            this.SearchLevSubDirsBox.TabIndex = 56;
            this.SearchLevSubDirsBox.Text = "Search also subdirectories in level folder";
            this.SearchLevSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // ShowReplayListGridBox
            // 
            this.ShowReplayListGridBox.AutoSize = true;
            this.ShowReplayListGridBox.Location = new System.Drawing.Point(250, 59);
            this.ShowReplayListGridBox.Name = "ShowReplayListGridBox";
            this.ShowReplayListGridBox.Size = new System.Drawing.Size(130, 17);
            this.ShowReplayListGridBox.TabIndex = 54;
            this.ShowReplayListGridBox.Text = "Show grid in replay list";
            this.ShowReplayListGridBox.UseVisualStyleBackColor = true;
            // 
            // LevelEditorPage
            // 
            this.LevelEditorPage.Controls.Add(this.numberFormatBox);
            this.LevelEditorPage.Controls.Add(this.RenderingSettingsButton);
            this.LevelEditorPage.Controls.Add(this.GroupBox1);
            this.LevelEditorPage.Controls.Add(this.AutograssThicknessBox);
            this.LevelEditorPage.Controls.Add(this.Label23);
            this.LevelEditorPage.Controls.Add(this.SameAsFilenameBox);
            this.LevelEditorPage.Controls.Add(this.DefaultTitleBox);
            this.LevelEditorPage.Controls.Add(this.Label20);
            this.LevelEditorPage.Controls.Add(this.baseFilenameBox);
            this.LevelEditorPage.Controls.Add(this.Label19);
            this.LevelEditorPage.Controls.Add(this.Label18);
            this.LevelEditorPage.Controls.Add(this.FilenameSuggestionBox);
            this.LevelEditorPage.Controls.Add(this.HighlightBox);
            this.LevelEditorPage.Controls.Add(this.DynamicCheckTopologyBox);
            this.LevelEditorPage.Controls.Add(this.CheckTopologyWhenSavingBox);
            this.LevelEditorPage.Controls.Add(this.CaptureRadiusBox);
            this.LevelEditorPage.Controls.Add(this.Label8);
            this.LevelEditorPage.Controls.Add(this.InitialHeightBox);
            this.LevelEditorPage.Controls.Add(this.InitialWidthBox);
            this.LevelEditorPage.Controls.Add(this.Label7);
            this.LevelEditorPage.Controls.Add(this.Label6);
            this.LevelEditorPage.Location = new System.Drawing.Point(4, 22);
            this.LevelEditorPage.Name = "LevelEditorPage";
            this.LevelEditorPage.Size = new System.Drawing.Size(596, 271);
            this.LevelEditorPage.TabIndex = 4;
            this.LevelEditorPage.Text = "SLE";
            this.LevelEditorPage.UseVisualStyleBackColor = true;
            // 
            // RenderingSettingsButton
            // 
            this.RenderingSettingsButton.Location = new System.Drawing.Point(235, 225);
            this.RenderingSettingsButton.Name = "RenderingSettingsButton";
            this.RenderingSettingsButton.Size = new System.Drawing.Size(114, 23);
            this.RenderingSettingsButton.TabIndex = 27;
            this.RenderingSettingsButton.Text = "Rendering settings";
            this.RenderingSettingsButton.UseVisualStyleBackColor = true;
            this.RenderingSettingsButton.Click += new System.EventHandler(this.RenderingSettingsButtonClick);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.SelectionPanel);
            this.GroupBox1.Controls.Add(this.Label15);
            this.GroupBox1.Controls.Add(this.Label17);
            this.GroupBox1.Controls.Add(this.HighlightPanel);
            this.GroupBox1.Location = new System.Drawing.Point(11, 207);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(207, 52);
            this.GroupBox1.TabIndex = 26;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Colors";
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectionPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectionPanel.Location = new System.Drawing.Point(12, 19);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.Size = new System.Drawing.Size(20, 20);
            this.SelectionPanel.TabIndex = 1;
            this.SelectionPanel.Click += new System.EventHandler(this.PanelClick);
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(38, 23);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(51, 13);
            this.Label15.TabIndex = 8;
            this.Label15.Text = "Selection";
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(148, 23);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(48, 13);
            this.Label17.TabIndex = 10;
            this.Label17.Text = "Highlight";
            // 
            // HighlightPanel
            // 
            this.HighlightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HighlightPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HighlightPanel.Location = new System.Drawing.Point(122, 19);
            this.HighlightPanel.Name = "HighlightPanel";
            this.HighlightPanel.Size = new System.Drawing.Size(20, 20);
            this.HighlightPanel.TabIndex = 2;
            this.HighlightPanel.Click += new System.EventHandler(this.PanelClick);
            // 
            // AutograssThicknessBox
            // 
            this.AutograssThicknessBox.Location = new System.Drawing.Point(120, 172);
            this.AutograssThicknessBox.Name = "AutograssThicknessBox";
            this.AutograssThicknessBox.Size = new System.Drawing.Size(100, 20);
            this.AutograssThicknessBox.TabIndex = 25;
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.Location = new System.Drawing.Point(9, 175);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(105, 13);
            this.Label23.TabIndex = 24;
            this.Label23.Text = "Autograss thickness:";
            // 
            // SameAsFilenameBox
            // 
            this.SameAsFilenameBox.AutoSize = true;
            this.SameAsFilenameBox.Location = new System.Drawing.Point(227, 148);
            this.SameAsFilenameBox.Name = "SameAsFilenameBox";
            this.SameAsFilenameBox.Size = new System.Drawing.Size(109, 17);
            this.SameAsFilenameBox.TabIndex = 19;
            this.SameAsFilenameBox.Text = "Same as filename";
            this.SameAsFilenameBox.UseVisualStyleBackColor = true;
            this.SameAsFilenameBox.CheckedChanged += new System.EventHandler(this.SameAsFilenameBoxCheckedChanged);
            // 
            // DefaultTitleBox
            // 
            this.DefaultTitleBox.Location = new System.Drawing.Point(120, 146);
            this.DefaultTitleBox.MaxLength = 50;
            this.DefaultTitleBox.Name = "DefaultTitleBox";
            this.DefaultTitleBox.Size = new System.Drawing.Size(100, 20);
            this.DefaultTitleBox.TabIndex = 18;
            this.DefaultTitleBox.Text = "New level";
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Location = new System.Drawing.Point(26, 149);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(88, 13);
            this.Label20.TabIndex = 17;
            this.Label20.Text = "Default level title:";
            // 
            // baseFilenameBox
            // 
            this.baseFilenameBox.Location = new System.Drawing.Point(120, 120);
            this.baseFilenameBox.Name = "baseFilenameBox";
            this.baseFilenameBox.Size = new System.Drawing.Size(100, 20);
            this.baseFilenameBox.TabIndex = 15;
            this.baseFilenameBox.Text = "MyLev";
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Location = new System.Drawing.Point(226, 123);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(107, 13);
            this.Label19.TabIndex = 14;
            this.Label19.Text = "Number format string:";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(41, 123);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(73, 13);
            this.Label18.TabIndex = 13;
            this.Label18.Text = "Basefilename:";
            // 
            // FilenameSuggestionBox
            // 
            this.FilenameSuggestionBox.AutoSize = true;
            this.FilenameSuggestionBox.Location = new System.Drawing.Point(11, 97);
            this.FilenameSuggestionBox.Name = "FilenameSuggestionBox";
            this.FilenameSuggestionBox.Size = new System.Drawing.Size(141, 17);
            this.FilenameSuggestionBox.TabIndex = 12;
            this.FilenameSuggestionBox.Text = "Use filename suggestion";
            this.FilenameSuggestionBox.UseVisualStyleBackColor = true;
            this.FilenameSuggestionBox.CheckedChanged += new System.EventHandler(this.FilenameSuggestionBoxCheckedChanged);
            // 
            // HighlightBox
            // 
            this.HighlightBox.AutoSize = true;
            this.HighlightBox.Location = new System.Drawing.Point(179, 63);
            this.HighlightBox.Name = "HighlightBox";
            this.HighlightBox.Size = new System.Drawing.Size(201, 17);
            this.HighlightBox.TabIndex = 10;
            this.HighlightBox.Text = "Highlight level elements under mouse";
            this.HighlightBox.UseVisualStyleBackColor = true;
            // 
            // DynamicCheckTopologyBox
            // 
            this.DynamicCheckTopologyBox.AutoSize = true;
            this.DynamicCheckTopologyBox.Location = new System.Drawing.Point(179, 37);
            this.DynamicCheckTopologyBox.Name = "DynamicCheckTopologyBox";
            this.DynamicCheckTopologyBox.Size = new System.Drawing.Size(157, 17);
            this.DynamicCheckTopologyBox.TabIndex = 7;
            this.DynamicCheckTopologyBox.Text = "Check topology dynamically";
            this.DynamicCheckTopologyBox.UseVisualStyleBackColor = true;
            // 
            // CheckTopologyWhenSavingBox
            // 
            this.CheckTopologyWhenSavingBox.AutoSize = true;
            this.CheckTopologyWhenSavingBox.Location = new System.Drawing.Point(179, 11);
            this.CheckTopologyWhenSavingBox.Name = "CheckTopologyWhenSavingBox";
            this.CheckTopologyWhenSavingBox.Size = new System.Drawing.Size(188, 17);
            this.CheckTopologyWhenSavingBox.TabIndex = 6;
            this.CheckTopologyWhenSavingBox.Text = "Check topology when saving level";
            this.CheckTopologyWhenSavingBox.UseVisualStyleBackColor = true;
            // 
            // CaptureRadiusBox
            // 
            this.CaptureRadiusBox.Location = new System.Drawing.Point(120, 61);
            this.CaptureRadiusBox.Name = "CaptureRadiusBox";
            this.CaptureRadiusBox.Size = new System.Drawing.Size(53, 20);
            this.CaptureRadiusBox.TabIndex = 5;
            this.CaptureRadiusBox.Text = "100";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(36, 64);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(78, 13);
            this.Label8.TabIndex = 4;
            this.Label8.Text = "Capture radius:";
            // 
            // InitialHeightBox
            // 
            this.InitialHeightBox.Location = new System.Drawing.Point(120, 35);
            this.InitialHeightBox.Name = "InitialHeightBox";
            this.InitialHeightBox.Size = new System.Drawing.Size(53, 20);
            this.InitialHeightBox.TabIndex = 3;
            this.InitialHeightBox.Text = "50";
            // 
            // InitialWidthBox
            // 
            this.InitialWidthBox.Location = new System.Drawing.Point(120, 9);
            this.InitialWidthBox.Name = "InitialWidthBox";
            this.InitialWidthBox.Size = new System.Drawing.Size(53, 20);
            this.InitialWidthBox.TabIndex = 2;
            this.InitialWidthBox.Text = "50";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(8, 38);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(106, 13);
            this.Label7.TabIndex = 1;
            this.Label7.Text = "Initial polygon height:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(12, 12);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(102, 13);
            this.Label6.TabIndex = 0;
            this.Label6.Text = "Initial polygon width:";
            // 
            // DatabasePage
            // 
            this.DatabasePage.BackColor = System.Drawing.Color.White;
            this.DatabasePage.Controls.Add(this.Label1);
            this.DatabasePage.Controls.Add(this.WarnAboutOldDBBox);
            this.DatabasePage.Controls.Add(this.Label4);
            this.DatabasePage.Controls.Add(this.CheckBox6);
            this.DatabasePage.Controls.Add(this.LoadButton);
            this.DatabasePage.Controls.Add(this.GenerateButton);
            this.DatabasePage.Controls.Add(this.SetPathButton);
            this.DatabasePage.Controls.Add(this.DBTextBox);
            this.DatabasePage.Location = new System.Drawing.Point(4, 22);
            this.DatabasePage.Name = "DatabasePage";
            this.DatabasePage.Padding = new System.Windows.Forms.Padding(3);
            this.DatabasePage.Size = new System.Drawing.Size(596, 271);
            this.DatabasePage.TabIndex = 1;
            this.DatabasePage.Text = "Databases";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(8, 77);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(10, 13);
            this.Label1.TabIndex = 56;
            this.Label1.Text = " ";
            // 
            // WarnAboutOldDBBox
            // 
            this.WarnAboutOldDBBox.AutoSize = true;
            this.WarnAboutOldDBBox.Checked = true;
            this.WarnAboutOldDBBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarnAboutOldDBBox.Location = new System.Drawing.Point(9, 98);
            this.WarnAboutOldDBBox.Name = "WarnAboutOldDBBox";
            this.WarnAboutOldDBBox.Size = new System.Drawing.Size(146, 17);
            this.WarnAboutOldDBBox.TabIndex = 54;
            this.WarnAboutOldDBBox.Text = "Warn about old database";
            this.WarnAboutOldDBBox.UseVisualStyleBackColor = true;
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.FullOpen = true;
            // 
            // numberFormatBox
            // 
            this.numberFormatBox.Location = new System.Drawing.Point(339, 120);
            this.numberFormatBox.Name = "numberFormatBox";
            this.numberFormatBox.Size = new System.Drawing.Size(100, 20);
            this.numberFormatBox.TabIndex = 28;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 297);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Elmanager configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveSettings);
            this.TabControl1.ResumeLayout(false);
            this.GeneralPage.ResumeLayout(false);
            this.GeneralPage.PerformLayout();
            this.RMPage.ResumeLayout(false);
            this.RMPage.PerformLayout();
            this.LevelEditorPage.ResumeLayout(false);
            this.LevelEditorPage.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.DatabasePage.ResumeLayout(false);
            this.DatabasePage.PerformLayout();
            this.ResumeLayout(false);

		}
		internal System.Windows.Forms.Button RecDirButton;
		internal System.Windows.Forms.Button LevDirButton;
		internal System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
		internal System.Windows.Forms.CheckBox CheckBox6;
        internal System.Windows.Forms.Button GenerateButton;
		internal System.Windows.Forms.TextBox RecTextBox;
		internal System.Windows.Forms.TextBox LevTextBox;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.TextBox DBTextBox;
		internal System.Windows.Forms.Button SetPathButton;
		internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
		internal System.Windows.Forms.Button LoadButton;
		internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage GeneralPage;
		internal System.Windows.Forms.TabPage DatabasePage;
		internal System.Windows.Forms.CheckBox WarnAboutOldDBBox;
		internal System.Windows.Forms.TabPage RMPage;
		internal System.Windows.Forms.CheckBox DeleteConfirmCheckBox;
		internal System.Windows.Forms.CheckBox SearchRecSubDirsBox;
		internal System.Windows.Forms.CheckBox DeleteRecycleCheckBox;
		internal System.Windows.Forms.CheckBox NitroBox;
		internal System.Windows.Forms.CheckBox SearchLevSubDirsBox;
		internal System.Windows.Forms.CheckBox ShowReplayListGridBox;
		internal System.Windows.Forms.TabPage LevelEditorPage;
		internal System.Windows.Forms.TextBox InitialHeightBox;
		internal System.Windows.Forms.TextBox InitialWidthBox;
		internal System.Windows.Forms.Label Label7;
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
		internal System.Windows.Forms.TextBox AutograssThicknessBox;
		internal System.Windows.Forms.Label Label23;
		internal System.Windows.Forms.CheckBox CheckForUpdatesBox;
		internal System.Windows.Forms.GroupBox GroupBox1;
		internal System.Windows.Forms.Button RenderingSettingsButton;
		internal System.Windows.Forms.Button LGRDirButton;
		internal System.Windows.Forms.TextBox LGRTextBox;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Button ElmaDirButton;
        private Button resetButton;
        private Button generateNativeImageButton;
        internal Label Label1;
        private TextBox numberFormatBox;
	}
	
}
