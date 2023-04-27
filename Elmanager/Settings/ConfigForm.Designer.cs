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
            components = new System.ComponentModel.Container();
            RecDirButton = new Button();
            LevDirButton = new Button();
            FolderBrowserDialog1 = new FolderBrowserDialog();
            RecTextBox = new TextBox();
            LevTextBox = new TextBox();
            Label2 = new Label();
            Label3 = new Label();
            OpenFileDialog1 = new OpenFileDialog();
            TabControl1 = new TabControl();
            generalTab = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            Label5 = new Label();
            ElmaDirButton = new Button();
            LGRDirButton = new Button();
            LGRTextBox = new TextBox();
            groupBox2 = new GroupBox();
            panel1 = new Panel();
            DisableFrameBufferUsageCheckBox = new CheckBox();
            resetButton = new Button();
            CheckForUpdatesBox = new CheckBox();
            rmTab = new TabPage();
            showTooltipForReplaysCheckBox = new CheckBox();
            DeleteConfirmCheckBox = new CheckBox();
            SearchRecSubDirsBox = new CheckBox();
            NitroBox = new CheckBox();
            SearchLevSubDirsBox = new CheckBox();
            ShowReplayListGridBox = new CheckBox();
            lmTab = new TabPage();
            lmShowTooltip = new CheckBox();
            lmConfirmDeletion = new CheckBox();
            lmSearchRecSubDirs = new CheckBox();
            lmSearchLevSubDirs = new CheckBox();
            lmShowGrid = new CheckBox();
            sleTab = new TabPage();
            startPositionFeatureCheckBox = new CheckBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            Label18 = new Label();
            Label20 = new Label();
            baseFilenameBox = new TextBox();
            DefaultTitleBox = new TextBox();
            Label19 = new Label();
            numberFormatBox = new TextBox();
            SameAsFilenameBox = new CheckBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            CaptureRadiusBox = new TextBox();
            LevelTemplateBox = new TextBox();
            browseButton = new Button();
            Label6 = new Label();
            Label8 = new Label();
            HighlightBox = new CheckBox();
            alwaysSetDefaultsInPictureTool = new CheckBox();
            capturePicTextFromBordersCheckBox = new CheckBox();
            RenderingSettingsButton = new Button();
            GroupBox1 = new GroupBox();
            crosshairPanel = new Panel();
            label9 = new Label();
            SelectionPanel = new Panel();
            Label15 = new Label();
            Label17 = new Label();
            HighlightPanel = new Panel();
            FilenameSuggestionBox = new CheckBox();
            DynamicCheckTopologyBox = new CheckBox();
            CheckTopologyWhenSavingBox = new CheckBox();
            ColorDialog1 = new ColorDialog();
            toolTip1 = new ToolTip(components);
            TabControl1.SuspendLayout();
            generalTab.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            rmTab.SuspendLayout();
            lmTab.SuspendLayout();
            sleTab.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            GroupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // RecDirButton
            // 
            RecDirButton.Location = new System.Drawing.Point(716, 6);
            RecDirButton.Margin = new Padding(6);
            RecDirButton.Name = "RecDirButton";
            RecDirButton.Size = new System.Drawing.Size(134, 50);
            RecDirButton.TabIndex = 0;
            RecDirButton.Text = "Browse...";
            RecDirButton.UseVisualStyleBackColor = true;
            RecDirButton.Click += BrowseReplayFolder;
            // 
            // LevDirButton
            // 
            LevDirButton.Location = new System.Drawing.Point(716, 68);
            LevDirButton.Margin = new Padding(6);
            LevDirButton.Name = "LevDirButton";
            LevDirButton.Size = new System.Drawing.Size(134, 50);
            LevDirButton.TabIndex = 1;
            LevDirButton.Text = "Browse...";
            LevDirButton.UseVisualStyleBackColor = true;
            LevDirButton.Click += BrowseLevelFolder;
            // 
            // RecTextBox
            // 
            RecTextBox.Anchor = AnchorStyles.None;
            RecTextBox.Location = new System.Drawing.Point(208, 11);
            RecTextBox.Margin = new Padding(6);
            RecTextBox.Name = "RecTextBox";
            RecTextBox.ReadOnly = true;
            RecTextBox.Size = new System.Drawing.Size(496, 39);
            RecTextBox.TabIndex = 44;
            // 
            // LevTextBox
            // 
            LevTextBox.Anchor = AnchorStyles.None;
            LevTextBox.Location = new System.Drawing.Point(208, 73);
            LevTextBox.Margin = new Padding(6);
            LevTextBox.Name = "LevTextBox";
            LevTextBox.ReadOnly = true;
            LevTextBox.Size = new System.Drawing.Size(496, 39);
            LevTextBox.TabIndex = 45;
            // 
            // Label2
            // 
            Label2.Anchor = AnchorStyles.Right;
            Label2.AutoSize = true;
            Label2.Location = new System.Drawing.Point(6, 15);
            Label2.Margin = new Padding(6, 0, 6, 0);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(190, 32);
            Label2.TabIndex = 46;
            Label2.Text = "Replay directory:";
            // 
            // Label3
            // 
            Label3.Anchor = AnchorStyles.Right;
            Label3.AutoSize = true;
            Label3.Location = new System.Drawing.Point(21, 77);
            Label3.Margin = new Padding(6, 0, 6, 0);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(175, 32);
            Label3.TabIndex = 47;
            Label3.Text = "Level directory:";
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.CheckFileExists = false;
            OpenFileDialog1.CheckPathExists = false;
            OpenFileDialog1.Filter = "Uncompressed replay databases|*.db|Compressed replay databases|*.cdb|All replay databases (*.db), (*.cdb)|*.db;*.cdb";
            OpenFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // TabControl1
            // 
            TabControl1.Controls.Add(generalTab);
            TabControl1.Controls.Add(rmTab);
            TabControl1.Controls.Add(lmTab);
            TabControl1.Controls.Add(sleTab);
            TabControl1.Dock = DockStyle.Fill;
            TabControl1.Location = new System.Drawing.Point(0, 0);
            TabControl1.Margin = new Padding(6);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new System.Drawing.Size(1171, 849);
            TabControl1.TabIndex = 55;
            // 
            // generalTab
            // 
            generalTab.BackColor = System.Drawing.Color.White;
            generalTab.Controls.Add(tableLayoutPanel1);
            generalTab.Controls.Add(groupBox2);
            generalTab.Controls.Add(resetButton);
            generalTab.Controls.Add(CheckForUpdatesBox);
            generalTab.Location = new System.Drawing.Point(8, 46);
            generalTab.Margin = new Padding(6);
            generalTab.Name = "generalTab";
            generalTab.Padding = new Padding(6);
            generalTab.Size = new System.Drawing.Size(1155, 795);
            generalTab.TabIndex = 0;
            generalTab.Text = "General";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(Label2, 0, 0);
            tableLayoutPanel1.Controls.Add(Label3, 0, 1);
            tableLayoutPanel1.Controls.Add(Label5, 0, 2);
            tableLayoutPanel1.Controls.Add(ElmaDirButton, 3, 1);
            tableLayoutPanel1.Controls.Add(RecTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(LGRDirButton, 2, 2);
            tableLayoutPanel1.Controls.Add(LevDirButton, 2, 1);
            tableLayoutPanel1.Controls.Add(LevTextBox, 1, 1);
            tableLayoutPanel1.Controls.Add(LGRTextBox, 1, 2);
            tableLayoutPanel1.Controls.Add(RecDirButton, 2, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            tableLayoutPanel1.Margin = new Padding(6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(1128, 186);
            tableLayoutPanel1.TabIndex = 57;
            // 
            // Label5
            // 
            Label5.Anchor = AnchorStyles.Right;
            Label5.AutoSize = true;
            Label5.Location = new System.Drawing.Point(36, 139);
            Label5.Margin = new Padding(6, 0, 6, 0);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(160, 32);
            Label5.TabIndex = 51;
            Label5.Text = "LGR directory:";
            // 
            // ElmaDirButton
            // 
            ElmaDirButton.Location = new System.Drawing.Point(862, 68);
            ElmaDirButton.Margin = new Padding(6);
            ElmaDirButton.Name = "ElmaDirButton";
            ElmaDirButton.Size = new System.Drawing.Size(260, 50);
            ElmaDirButton.TabIndex = 52;
            ElmaDirButton.Text = "Get all from Elma dir";
            ElmaDirButton.UseVisualStyleBackColor = true;
            ElmaDirButton.Click += BrowseForElmaDir;
            // 
            // LGRDirButton
            // 
            LGRDirButton.Location = new System.Drawing.Point(716, 130);
            LGRDirButton.Margin = new Padding(6);
            LGRDirButton.Name = "LGRDirButton";
            LGRDirButton.Size = new System.Drawing.Size(134, 50);
            LGRDirButton.TabIndex = 49;
            LGRDirButton.Text = "Browse...";
            LGRDirButton.UseVisualStyleBackColor = true;
            LGRDirButton.Click += BrowseLgrFolder;
            // 
            // LGRTextBox
            // 
            LGRTextBox.Anchor = AnchorStyles.None;
            LGRTextBox.Location = new System.Drawing.Point(208, 135);
            LGRTextBox.Margin = new Padding(6);
            LGRTextBox.Name = "LGRTextBox";
            LGRTextBox.ReadOnly = true;
            LGRTextBox.Size = new System.Drawing.Size(496, 39);
            LGRTextBox.TabIndex = 50;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panel1);
            groupBox2.Location = new System.Drawing.Point(16, 326);
            groupBox2.Margin = new Padding(6);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(6);
            groupBox2.Size = new System.Drawing.Size(400, 94);
            groupBox2.TabIndex = 56;
            groupBox2.TabStop = false;
            groupBox2.Text = "Workarounds";
            // 
            // panel1
            // 
            panel1.Controls.Add(DisableFrameBufferUsageCheckBox);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(6, 38);
            panel1.Margin = new Padding(6);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(388, 50);
            panel1.TabIndex = 0;
            // 
            // DisableFrameBufferUsageCheckBox
            // 
            DisableFrameBufferUsageCheckBox.AutoSize = true;
            DisableFrameBufferUsageCheckBox.Dock = DockStyle.Fill;
            DisableFrameBufferUsageCheckBox.Location = new System.Drawing.Point(0, 0);
            DisableFrameBufferUsageCheckBox.Margin = new Padding(6);
            DisableFrameBufferUsageCheckBox.Name = "DisableFrameBufferUsageCheckBox";
            DisableFrameBufferUsageCheckBox.Size = new System.Drawing.Size(388, 50);
            DisableFrameBufferUsageCheckBox.TabIndex = 55;
            DisableFrameBufferUsageCheckBox.Text = "Disable framebuffer usage";
            DisableFrameBufferUsageCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetButton
            // 
            resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            resetButton.Location = new System.Drawing.Point(16, 725);
            resetButton.Margin = new Padding(6);
            resetButton.Name = "resetButton";
            resetButton.Size = new System.Drawing.Size(322, 50);
            resetButton.TabIndex = 53;
            resetButton.Text = "Reset settings to default";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += ResetButtonClick;
            // 
            // CheckForUpdatesBox
            // 
            CheckForUpdatesBox.AutoSize = true;
            CheckForUpdatesBox.Location = new System.Drawing.Point(16, 250);
            CheckForUpdatesBox.Margin = new Padding(6);
            CheckForUpdatesBox.Name = "CheckForUpdatesBox";
            CheckForUpdatesBox.Size = new System.Drawing.Size(467, 36);
            CheckForUpdatesBox.TabIndex = 48;
            CheckForUpdatesBox.Text = "Check for Elmanager updates at startup";
            CheckForUpdatesBox.UseVisualStyleBackColor = true;
            // 
            // rmTab
            // 
            rmTab.Controls.Add(showTooltipForReplaysCheckBox);
            rmTab.Controls.Add(DeleteConfirmCheckBox);
            rmTab.Controls.Add(SearchRecSubDirsBox);
            rmTab.Controls.Add(NitroBox);
            rmTab.Controls.Add(SearchLevSubDirsBox);
            rmTab.Controls.Add(ShowReplayListGridBox);
            rmTab.Location = new System.Drawing.Point(8, 46);
            rmTab.Margin = new Padding(6);
            rmTab.Name = "rmTab";
            rmTab.Padding = new Padding(6);
            rmTab.Size = new System.Drawing.Size(1155, 795);
            rmTab.TabIndex = 3;
            rmTab.Text = "Replay manager";
            rmTab.UseVisualStyleBackColor = true;
            // 
            // showTooltipForReplaysCheckBox
            // 
            showTooltipForReplaysCheckBox.AutoSize = true;
            showTooltipForReplaysCheckBox.Location = new System.Drawing.Point(16, 104);
            showTooltipForReplaysCheckBox.Margin = new Padding(6);
            showTooltipForReplaysCheckBox.Name = "showTooltipForReplaysCheckBox";
            showTooltipForReplaysCheckBox.Size = new System.Drawing.Size(436, 36);
            showTooltipForReplaysCheckBox.TabIndex = 58;
            showTooltipForReplaysCheckBox.Text = "Show tooltip for replays in replay list";
            showTooltipForReplaysCheckBox.UseVisualStyleBackColor = true;
            // 
            // DeleteConfirmCheckBox
            // 
            DeleteConfirmCheckBox.AutoSize = true;
            DeleteConfirmCheckBox.Location = new System.Drawing.Point(516, 58);
            DeleteConfirmCheckBox.Margin = new Padding(6);
            DeleteConfirmCheckBox.Name = "DeleteConfirmCheckBox";
            DeleteConfirmCheckBox.Size = new System.Drawing.Size(227, 36);
            DeleteConfirmCheckBox.TabIndex = 57;
            DeleteConfirmCheckBox.Text = "Confirm deletion";
            DeleteConfirmCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchRecSubDirsBox
            // 
            SearchRecSubDirsBox.AutoSize = true;
            SearchRecSubDirsBox.Location = new System.Drawing.Point(16, 12);
            SearchRecSubDirsBox.Margin = new Padding(6);
            SearchRecSubDirsBox.Name = "SearchRecSubDirsBox";
            SearchRecSubDirsBox.Size = new System.Drawing.Size(491, 36);
            SearchRecSubDirsBox.TabIndex = 55;
            SearchRecSubDirsBox.Text = "Search also subdirectories in replay folder";
            SearchRecSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // NitroBox
            // 
            NitroBox.AutoSize = true;
            NitroBox.Location = new System.Drawing.Point(516, 104);
            NitroBox.Margin = new Padding(6);
            NitroBox.Name = "NitroBox";
            NitroBox.Size = new System.Drawing.Size(385, 36);
            NitroBox.TabIndex = 53;
            NitroBox.Text = "Treat Nitro replays as erroneous";
            NitroBox.UseVisualStyleBackColor = true;
            // 
            // SearchLevSubDirsBox
            // 
            SearchLevSubDirsBox.AutoSize = true;
            SearchLevSubDirsBox.Location = new System.Drawing.Point(516, 12);
            SearchLevSubDirsBox.Margin = new Padding(6);
            SearchLevSubDirsBox.Name = "SearchLevSubDirsBox";
            SearchLevSubDirsBox.Size = new System.Drawing.Size(476, 36);
            SearchLevSubDirsBox.TabIndex = 56;
            SearchLevSubDirsBox.Text = "Search also subdirectories in level folder";
            SearchLevSubDirsBox.UseVisualStyleBackColor = true;
            // 
            // ShowReplayListGridBox
            // 
            ShowReplayListGridBox.AutoSize = true;
            ShowReplayListGridBox.Location = new System.Drawing.Point(16, 58);
            ShowReplayListGridBox.Margin = new Padding(6);
            ShowReplayListGridBox.Name = "ShowReplayListGridBox";
            ShowReplayListGridBox.Size = new System.Drawing.Size(289, 36);
            ShowReplayListGridBox.TabIndex = 54;
            ShowReplayListGridBox.Text = "Show grid in replay list";
            ShowReplayListGridBox.UseVisualStyleBackColor = true;
            // 
            // lmTab
            // 
            lmTab.Controls.Add(lmShowTooltip);
            lmTab.Controls.Add(lmConfirmDeletion);
            lmTab.Controls.Add(lmSearchRecSubDirs);
            lmTab.Controls.Add(lmSearchLevSubDirs);
            lmTab.Controls.Add(lmShowGrid);
            lmTab.Location = new System.Drawing.Point(8, 46);
            lmTab.Margin = new Padding(6);
            lmTab.Name = "lmTab";
            lmTab.Padding = new Padding(6);
            lmTab.Size = new System.Drawing.Size(1155, 795);
            lmTab.TabIndex = 5;
            lmTab.Text = "Level manager";
            lmTab.UseVisualStyleBackColor = true;
            // 
            // lmShowTooltip
            // 
            lmShowTooltip.AutoSize = true;
            lmShowTooltip.Location = new System.Drawing.Point(16, 104);
            lmShowTooltip.Margin = new Padding(6);
            lmShowTooltip.Name = "lmShowTooltip";
            lmShowTooltip.Size = new System.Drawing.Size(406, 36);
            lmShowTooltip.TabIndex = 65;
            lmShowTooltip.Text = "Show tooltip for levels in level list";
            lmShowTooltip.UseVisualStyleBackColor = true;
            // 
            // lmConfirmDeletion
            // 
            lmConfirmDeletion.AutoSize = true;
            lmConfirmDeletion.Location = new System.Drawing.Point(516, 58);
            lmConfirmDeletion.Margin = new Padding(6);
            lmConfirmDeletion.Name = "lmConfirmDeletion";
            lmConfirmDeletion.Size = new System.Drawing.Size(227, 36);
            lmConfirmDeletion.TabIndex = 64;
            lmConfirmDeletion.Text = "Confirm deletion";
            lmConfirmDeletion.UseVisualStyleBackColor = true;
            // 
            // lmSearchRecSubDirs
            // 
            lmSearchRecSubDirs.AutoSize = true;
            lmSearchRecSubDirs.Location = new System.Drawing.Point(16, 12);
            lmSearchRecSubDirs.Margin = new Padding(6);
            lmSearchRecSubDirs.Name = "lmSearchRecSubDirs";
            lmSearchRecSubDirs.Size = new System.Drawing.Size(491, 36);
            lmSearchRecSubDirs.TabIndex = 62;
            lmSearchRecSubDirs.Text = "Search also subdirectories in replay folder";
            lmSearchRecSubDirs.UseVisualStyleBackColor = true;
            // 
            // lmSearchLevSubDirs
            // 
            lmSearchLevSubDirs.AutoSize = true;
            lmSearchLevSubDirs.Location = new System.Drawing.Point(516, 12);
            lmSearchLevSubDirs.Margin = new Padding(6);
            lmSearchLevSubDirs.Name = "lmSearchLevSubDirs";
            lmSearchLevSubDirs.Size = new System.Drawing.Size(476, 36);
            lmSearchLevSubDirs.TabIndex = 63;
            lmSearchLevSubDirs.Text = "Search also subdirectories in level folder";
            lmSearchLevSubDirs.UseVisualStyleBackColor = true;
            // 
            // lmShowGrid
            // 
            lmShowGrid.AutoSize = true;
            lmShowGrid.Location = new System.Drawing.Point(16, 58);
            lmShowGrid.Margin = new Padding(6);
            lmShowGrid.Name = "lmShowGrid";
            lmShowGrid.Size = new System.Drawing.Size(274, 36);
            lmShowGrid.TabIndex = 61;
            lmShowGrid.Text = "Show grid in level list";
            lmShowGrid.UseVisualStyleBackColor = true;
            // 
            // sleTab
            // 
            sleTab.Controls.Add(startPositionFeatureCheckBox);
            sleTab.Controls.Add(tableLayoutPanel3);
            sleTab.Controls.Add(tableLayoutPanel2);
            sleTab.Controls.Add(HighlightBox);
            sleTab.Controls.Add(alwaysSetDefaultsInPictureTool);
            sleTab.Controls.Add(capturePicTextFromBordersCheckBox);
            sleTab.Controls.Add(RenderingSettingsButton);
            sleTab.Controls.Add(GroupBox1);
            sleTab.Controls.Add(FilenameSuggestionBox);
            sleTab.Controls.Add(DynamicCheckTopologyBox);
            sleTab.Controls.Add(CheckTopologyWhenSavingBox);
            sleTab.Location = new System.Drawing.Point(8, 46);
            sleTab.Margin = new Padding(6);
            sleTab.Name = "sleTab";
            sleTab.Size = new System.Drawing.Size(1155, 795);
            sleTab.TabIndex = 4;
            sleTab.Text = "SLE";
            sleTab.UseVisualStyleBackColor = true;
            // 
            // startPositionFeatureCheckBox
            // 
            startPositionFeatureCheckBox.AutoSize = true;
            startPositionFeatureCheckBox.Location = new System.Drawing.Point(22, 371);
            startPositionFeatureCheckBox.Margin = new Padding(6);
            startPositionFeatureCheckBox.Name = "startPositionFeatureCheckBox";
            startPositionFeatureCheckBox.Size = new System.Drawing.Size(509, 36);
            startPositionFeatureCheckBox.TabIndex = 34;
            startPositionFeatureCheckBox.Text = "Enable \"Save/Restore start position\" feature";
            startPositionFeatureCheckBox.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.AutoSize = true;
            tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel3.ColumnCount = 4;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.Controls.Add(Label18, 0, 0);
            tableLayoutPanel3.Controls.Add(Label20, 0, 1);
            tableLayoutPanel3.Controls.Add(baseFilenameBox, 1, 0);
            tableLayoutPanel3.Controls.Add(DefaultTitleBox, 1, 1);
            tableLayoutPanel3.Controls.Add(Label19, 2, 0);
            tableLayoutPanel3.Controls.Add(numberFormatBox, 3, 0);
            tableLayoutPanel3.Controls.Add(SameAsFilenameBox, 2, 1);
            tableLayoutPanel3.Location = new System.Drawing.Point(16, 503);
            tableLayoutPanel3.Margin = new Padding(6);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(894, 102);
            tableLayoutPanel3.TabIndex = 33;
            // 
            // Label18
            // 
            Label18.Anchor = AnchorStyles.Right;
            Label18.AutoSize = true;
            Label18.Location = new System.Drawing.Point(47, 9);
            Label18.Margin = new Padding(6, 0, 6, 0);
            Label18.Name = "Label18";
            Label18.Size = new System.Drawing.Size(161, 32);
            Label18.TabIndex = 13;
            Label18.Text = "Basefilename:";
            // 
            // Label20
            // 
            Label20.Anchor = AnchorStyles.Right;
            Label20.AutoSize = true;
            Label20.Location = new System.Drawing.Point(6, 60);
            Label20.Margin = new Padding(6, 0, 6, 0);
            Label20.Name = "Label20";
            Label20.Size = new System.Drawing.Size(202, 32);
            Label20.TabIndex = 17;
            Label20.Text = "Default level title:";
            // 
            // baseFilenameBox
            // 
            baseFilenameBox.Anchor = AnchorStyles.None;
            baseFilenameBox.Location = new System.Drawing.Point(220, 6);
            baseFilenameBox.Margin = new Padding(6);
            baseFilenameBox.Name = "baseFilenameBox";
            baseFilenameBox.Size = new System.Drawing.Size(196, 39);
            baseFilenameBox.TabIndex = 15;
            baseFilenameBox.Text = "MyLev";
            // 
            // DefaultTitleBox
            // 
            DefaultTitleBox.Anchor = AnchorStyles.None;
            DefaultTitleBox.Location = new System.Drawing.Point(220, 57);
            DefaultTitleBox.Margin = new Padding(6);
            DefaultTitleBox.MaxLength = 50;
            DefaultTitleBox.Name = "DefaultTitleBox";
            DefaultTitleBox.Size = new System.Drawing.Size(196, 39);
            DefaultTitleBox.TabIndex = 18;
            DefaultTitleBox.Text = "New level";
            // 
            // Label19
            // 
            Label19.Anchor = AnchorStyles.Right;
            Label19.AutoSize = true;
            Label19.Location = new System.Drawing.Point(428, 9);
            Label19.Margin = new Padding(6, 0, 6, 0);
            Label19.Name = "Label19";
            Label19.Size = new System.Drawing.Size(252, 32);
            Label19.TabIndex = 14;
            Label19.Text = "Number format string:";
            // 
            // numberFormatBox
            // 
            numberFormatBox.Anchor = AnchorStyles.None;
            numberFormatBox.Location = new System.Drawing.Point(692, 6);
            numberFormatBox.Margin = new Padding(6);
            numberFormatBox.Name = "numberFormatBox";
            numberFormatBox.Size = new System.Drawing.Size(196, 39);
            numberFormatBox.TabIndex = 28;
            // 
            // SameAsFilenameBox
            // 
            SameAsFilenameBox.Anchor = AnchorStyles.Left;
            SameAsFilenameBox.AutoSize = true;
            SameAsFilenameBox.Location = new System.Drawing.Point(428, 58);
            SameAsFilenameBox.Margin = new Padding(6);
            SameAsFilenameBox.Name = "SameAsFilenameBox";
            SameAsFilenameBox.Size = new System.Drawing.Size(234, 36);
            SameAsFilenameBox.TabIndex = 19;
            SameAsFilenameBox.Text = "Same as filename";
            SameAsFilenameBox.UseVisualStyleBackColor = true;
            SameAsFilenameBox.CheckedChanged += SameAsFilenameBoxCheckedChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(CaptureRadiusBox, 1, 1);
            tableLayoutPanel2.Controls.Add(LevelTemplateBox, 1, 0);
            tableLayoutPanel2.Controls.Add(browseButton, 2, 0);
            tableLayoutPanel2.Controls.Add(Label6, 0, 0);
            tableLayoutPanel2.Controls.Add(Label8, 0, 1);
            tableLayoutPanel2.Location = new System.Drawing.Point(22, 6);
            tableLayoutPanel2.Margin = new Padding(6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(832, 113);
            tableLayoutPanel2.TabIndex = 32;
            // 
            // CaptureRadiusBox
            // 
            CaptureRadiusBox.Anchor = AnchorStyles.Left;
            CaptureRadiusBox.Location = new System.Drawing.Point(268, 68);
            CaptureRadiusBox.Margin = new Padding(6);
            CaptureRadiusBox.Name = "CaptureRadiusBox";
            CaptureRadiusBox.Size = new System.Drawing.Size(96, 39);
            CaptureRadiusBox.TabIndex = 5;
            CaptureRadiusBox.Text = "100";
            // 
            // LevelTemplateBox
            // 
            LevelTemplateBox.Anchor = AnchorStyles.None;
            LevelTemplateBox.Location = new System.Drawing.Point(268, 11);
            LevelTemplateBox.Margin = new Padding(6);
            LevelTemplateBox.Name = "LevelTemplateBox";
            LevelTemplateBox.Size = new System.Drawing.Size(396, 39);
            LevelTemplateBox.TabIndex = 2;
            LevelTemplateBox.Text = "50,50";
            toolTip1.SetToolTip(LevelTemplateBox, "width,height or browse for a level template");
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Left;
            browseButton.Location = new System.Drawing.Point(676, 6);
            browseButton.Margin = new Padding(6);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(150, 50);
            browseButton.TabIndex = 29;
            browseButton.Text = "Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += browseButton_Click;
            // 
            // Label6
            // 
            Label6.Anchor = AnchorStyles.Right;
            Label6.AutoSize = true;
            Label6.Location = new System.Drawing.Point(30, 15);
            Label6.Margin = new Padding(6, 0, 6, 0);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(226, 32);
            Label6.TabIndex = 0;
            Label6.Text = "New level template:";
            // 
            // Label8
            // 
            Label8.Anchor = AnchorStyles.Right;
            Label8.AutoSize = true;
            Label8.Location = new System.Drawing.Point(6, 71);
            Label8.Margin = new Padding(6, 0, 6, 0);
            Label8.Name = "Label8";
            Label8.Size = new System.Drawing.Size(250, 32);
            Label8.TabIndex = 4;
            Label8.Text = "Mouse capture radius:";
            // 
            // HighlightBox
            // 
            HighlightBox.AutoSize = true;
            HighlightBox.Location = new System.Drawing.Point(22, 131);
            HighlightBox.Margin = new Padding(6);
            HighlightBox.Name = "HighlightBox";
            HighlightBox.Size = new System.Drawing.Size(456, 36);
            HighlightBox.TabIndex = 10;
            HighlightBox.Text = "Highlight level elements under mouse";
            HighlightBox.UseVisualStyleBackColor = true;
            // 
            // alwaysSetDefaultsInPictureTool
            // 
            alwaysSetDefaultsInPictureTool.AutoSize = true;
            alwaysSetDefaultsInPictureTool.Location = new System.Drawing.Point(22, 323);
            alwaysSetDefaultsInPictureTool.Margin = new Padding(6);
            alwaysSetDefaultsInPictureTool.Name = "alwaysSetDefaultsInPictureTool";
            alwaysSetDefaultsInPictureTool.Size = new System.Drawing.Size(847, 36);
            alwaysSetDefaultsInPictureTool.TabIndex = 31;
            alwaysSetDefaultsInPictureTool.Text = "Always set distance and clipping to defaults when changing picture/texture";
            alwaysSetDefaultsInPictureTool.UseVisualStyleBackColor = true;
            // 
            // capturePicTextFromBordersCheckBox
            // 
            capturePicTextFromBordersCheckBox.AutoSize = true;
            capturePicTextFromBordersCheckBox.Location = new System.Drawing.Point(22, 179);
            capturePicTextFromBordersCheckBox.Margin = new Padding(6);
            capturePicTextFromBordersCheckBox.Name = "capturePicTextFromBordersCheckBox";
            capturePicTextFromBordersCheckBox.Size = new System.Drawing.Size(559, 36);
            capturePicTextFromBordersCheckBox.TabIndex = 30;
            capturePicTextFromBordersCheckBox.Text = "Capture pictures and textures from borders only";
            capturePicTextFromBordersCheckBox.UseVisualStyleBackColor = true;
            // 
            // RenderingSettingsButton
            // 
            RenderingSettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RenderingSettingsButton.Location = new System.Drawing.Point(458, 723);
            RenderingSettingsButton.Margin = new Padding(6);
            RenderingSettingsButton.Name = "RenderingSettingsButton";
            RenderingSettingsButton.Size = new System.Drawing.Size(230, 50);
            RenderingSettingsButton.TabIndex = 27;
            RenderingSettingsButton.Text = "Rendering settings";
            RenderingSettingsButton.UseVisualStyleBackColor = true;
            RenderingSettingsButton.Click += RenderingSettingsButtonClick;
            // 
            // GroupBox1
            // 
            GroupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            GroupBox1.Controls.Add(crosshairPanel);
            GroupBox1.Controls.Add(label9);
            GroupBox1.Controls.Add(SelectionPanel);
            GroupBox1.Controls.Add(Label15);
            GroupBox1.Controls.Add(Label17);
            GroupBox1.Controls.Add(HighlightPanel);
            GroupBox1.Location = new System.Drawing.Point(16, 617);
            GroupBox1.Margin = new Padding(6);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Padding = new Padding(6);
            GroupBox1.Size = new System.Drawing.Size(414, 156);
            GroupBox1.TabIndex = 26;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Colors";
            // 
            // crosshairPanel
            // 
            crosshairPanel.BorderStyle = BorderStyle.FixedSingle;
            crosshairPanel.Cursor = Cursors.Hand;
            crosshairPanel.Location = new System.Drawing.Point(12, 94);
            crosshairPanel.Margin = new Padding(6);
            crosshairPanel.Name = "crosshairPanel";
            crosshairPanel.Size = new System.Drawing.Size(38, 38);
            crosshairPanel.TabIndex = 9;
            crosshairPanel.Click += PanelClick;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(64, 102);
            label9.Margin = new Padding(6, 0, 6, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(111, 32);
            label9.TabIndex = 10;
            label9.Text = "Crosshair";
            // 
            // SelectionPanel
            // 
            SelectionPanel.BorderStyle = BorderStyle.FixedSingle;
            SelectionPanel.Cursor = Cursors.Hand;
            SelectionPanel.Location = new System.Drawing.Point(12, 44);
            SelectionPanel.Margin = new Padding(6);
            SelectionPanel.Name = "SelectionPanel";
            SelectionPanel.Size = new System.Drawing.Size(38, 38);
            SelectionPanel.TabIndex = 1;
            SelectionPanel.Click += PanelClick;
            // 
            // Label15
            // 
            Label15.AutoSize = true;
            Label15.Location = new System.Drawing.Point(64, 52);
            Label15.Margin = new Padding(6, 0, 6, 0);
            Label15.Name = "Label15";
            Label15.Size = new System.Drawing.Size(112, 32);
            Label15.TabIndex = 8;
            Label15.Text = "Selection";
            // 
            // Label17
            // 
            Label17.AutoSize = true;
            Label17.Location = new System.Drawing.Point(284, 52);
            Label17.Margin = new Padding(6, 0, 6, 0);
            Label17.Name = "Label17";
            Label17.Size = new System.Drawing.Size(113, 32);
            Label17.TabIndex = 10;
            Label17.Text = "Highlight";
            // 
            // HighlightPanel
            // 
            HighlightPanel.BorderStyle = BorderStyle.FixedSingle;
            HighlightPanel.Cursor = Cursors.Hand;
            HighlightPanel.Location = new System.Drawing.Point(232, 44);
            HighlightPanel.Margin = new Padding(6);
            HighlightPanel.Name = "HighlightPanel";
            HighlightPanel.Size = new System.Drawing.Size(38, 38);
            HighlightPanel.TabIndex = 2;
            HighlightPanel.Click += PanelClick;
            // 
            // FilenameSuggestionBox
            // 
            FilenameSuggestionBox.AutoSize = true;
            FilenameSuggestionBox.Location = new System.Drawing.Point(22, 419);
            FilenameSuggestionBox.Margin = new Padding(6);
            FilenameSuggestionBox.Name = "FilenameSuggestionBox";
            FilenameSuggestionBox.Size = new System.Drawing.Size(309, 36);
            FilenameSuggestionBox.TabIndex = 12;
            FilenameSuggestionBox.Text = "Use filename suggestion";
            FilenameSuggestionBox.UseVisualStyleBackColor = true;
            FilenameSuggestionBox.CheckedChanged += FilenameSuggestionBoxCheckedChanged;
            // 
            // DynamicCheckTopologyBox
            // 
            DynamicCheckTopologyBox.AutoSize = true;
            DynamicCheckTopologyBox.Location = new System.Drawing.Point(22, 227);
            DynamicCheckTopologyBox.Margin = new Padding(6);
            DynamicCheckTopologyBox.Name = "DynamicCheckTopologyBox";
            DynamicCheckTopologyBox.Size = new System.Drawing.Size(347, 36);
            DynamicCheckTopologyBox.TabIndex = 7;
            DynamicCheckTopologyBox.Text = "Check topology dynamically";
            DynamicCheckTopologyBox.UseVisualStyleBackColor = true;
            // 
            // CheckTopologyWhenSavingBox
            // 
            CheckTopologyWhenSavingBox.AutoSize = true;
            CheckTopologyWhenSavingBox.Location = new System.Drawing.Point(22, 275);
            CheckTopologyWhenSavingBox.Margin = new Padding(6);
            CheckTopologyWhenSavingBox.Name = "CheckTopologyWhenSavingBox";
            CheckTopologyWhenSavingBox.Size = new System.Drawing.Size(411, 36);
            CheckTopologyWhenSavingBox.TabIndex = 6;
            CheckTopologyWhenSavingBox.Text = "Check topology when saving level";
            CheckTopologyWhenSavingBox.UseVisualStyleBackColor = true;
            // 
            // ColorDialog1
            // 
            ColorDialog1.FullOpen = true;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1171, 849);
            Controls.Add(TabControl1);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(6);
            MaximizeBox = false;
            Name = "ConfigForm";
            Text = "Elmanager configuration";
            FormClosing += SaveSettings;
            TabControl1.ResumeLayout(false);
            generalTab.ResumeLayout(false);
            generalTab.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            groupBox2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            rmTab.ResumeLayout(false);
            rmTab.PerformLayout();
            lmTab.ResumeLayout(false);
            lmTab.PerformLayout();
            sleTab.ResumeLayout(false);
            sleTab.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            ResumeLayout(false);
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
        internal CheckBox startPositionFeatureCheckBox;
    }

}
