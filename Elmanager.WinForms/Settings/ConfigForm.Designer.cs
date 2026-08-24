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
            TabControl1 = new TabControl();
            generalTab = new TabPage();
            flowLayoutPanel1 = new FlowLayoutPanel();
            CheckForUpdatesBox = new CheckBox();
            checkForUpdatesButton = new Button();
            updateInfoLabel = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            Label5 = new Label();
            ElmaDirButton = new Button();
            LGRDirButton = new Button();
            LGRTextBox = new TextBox();
            groupBox2 = new GroupBox();
            panel1 = new Panel();
            DisableFrameBufferUsageCheckBox = new CheckBox();
            resetButton = new Button();
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
            TabControl1.SuspendLayout();
            generalTab.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            rmTab.SuspendLayout();
            lmTab.SuspendLayout();
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
            // TabControl1
            // 
            TabControl1.Controls.Add(generalTab);
            TabControl1.Controls.Add(rmTab);
            TabControl1.Controls.Add(lmTab);
            TabControl1.Dock = DockStyle.Fill;
            TabControl1.Location = new System.Drawing.Point(0, 0);
            TabControl1.Margin = new Padding(6);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new System.Drawing.Size(1171, 777);
            TabControl1.TabIndex = 55;
            // 
            // generalTab
            // 
            generalTab.BackColor = System.Drawing.Color.White;
            generalTab.Controls.Add(flowLayoutPanel1);
            generalTab.Controls.Add(tableLayoutPanel1);
            generalTab.Controls.Add(groupBox2);
            generalTab.Controls.Add(resetButton);
            generalTab.Location = new System.Drawing.Point(8, 46);
            generalTab.Margin = new Padding(6);
            generalTab.Name = "generalTab";
            generalTab.Padding = new Padding(6);
            generalTab.Size = new System.Drawing.Size(1155, 795);
            generalTab.TabIndex = 0;
            generalTab.Text = "General";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(CheckForUpdatesBox);
            flowLayoutPanel1.Controls.Add(checkForUpdatesButton);
            flowLayoutPanel1.Controls.Add(updateInfoLabel);
            flowLayoutPanel1.Location = new System.Drawing.Point(16, 236);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(1124, 56);
            flowLayoutPanel1.TabIndex = 58;
            // 
            // CheckForUpdatesBox
            // 
            CheckForUpdatesBox.AutoSize = true;
            CheckForUpdatesBox.Location = new System.Drawing.Point(6, 10);
            CheckForUpdatesBox.Margin = new Padding(6, 10, 6, 6);
            CheckForUpdatesBox.Name = "CheckForUpdatesBox";
            CheckForUpdatesBox.Size = new System.Drawing.Size(467, 36);
            CheckForUpdatesBox.TabIndex = 48;
            CheckForUpdatesBox.Text = "Check for Elmanager updates at startup";
            CheckForUpdatesBox.UseVisualStyleBackColor = true;
            // 
            // checkForUpdatesButton
            // 
            checkForUpdatesButton.AutoSize = true;
            checkForUpdatesButton.Location = new System.Drawing.Point(482, 3);
            checkForUpdatesButton.Name = "checkForUpdatesButton";
            checkForUpdatesButton.Size = new System.Drawing.Size(141, 50);
            checkForUpdatesButton.TabIndex = 49;
            checkForUpdatesButton.Text = "Check now";
            checkForUpdatesButton.UseVisualStyleBackColor = true;
            checkForUpdatesButton.Click += checkForUpdatesButton_Click;
            // 
            // updateInfoLabel
            // 
            updateInfoLabel.AutoSize = true;
            updateInfoLabel.Location = new System.Drawing.Point(629, 13);
            updateInfoLabel.Margin = new Padding(3, 13, 3, 0);
            updateInfoLabel.Name = "updateInfoLabel";
            updateInfoLabel.Size = new System.Drawing.Size(273, 32);
            updateInfoLabel.TabIndex = 50;
            updateInfoLabel.Text = "Elmanager is up-to-date";
            updateInfoLabel.Visible = false;
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
            // ConfigForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1171, 777);
            Controls.Add(TabControl1);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(6);
            MaximizeBox = false;
            Name = "ConfigForm";
            Text = "Elmanager configuration";
            FormClosing += SaveSettings;
            TabControl1.ResumeLayout(false);
            generalTab.ResumeLayout(false);
            generalTab.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            groupBox2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            rmTab.ResumeLayout(false);
            rmTab.PerformLayout();
            lmTab.ResumeLayout(false);
            lmTab.PerformLayout();
            ResumeLayout(false);
        }

        internal System.Windows.Forms.Button RecDirButton;
        internal System.Windows.Forms.Button LevDirButton;
        internal System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
        internal System.Windows.Forms.TextBox RecTextBox;
        internal System.Windows.Forms.TextBox LevTextBox;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage generalTab;
        internal System.Windows.Forms.TabPage rmTab;
        internal System.Windows.Forms.CheckBox DeleteConfirmCheckBox;
        internal System.Windows.Forms.CheckBox SearchRecSubDirsBox;
        internal System.Windows.Forms.CheckBox NitroBox;
        internal System.Windows.Forms.CheckBox SearchLevSubDirsBox;
        internal System.Windows.Forms.CheckBox ShowReplayListGridBox;
        internal System.Windows.Forms.CheckBox CheckForUpdatesBox;
        internal System.Windows.Forms.Button LGRDirButton;
        internal System.Windows.Forms.TextBox LGRTextBox;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Button ElmaDirButton;
        private Button resetButton;
        private GroupBox groupBox2;
        private CheckBox DisableFrameBufferUsageCheckBox;
        internal CheckBox showTooltipForReplaysCheckBox;
        private TabPage lmTab;
        internal CheckBox lmShowTooltip;
        internal CheckBox lmConfirmDeletion;
        internal CheckBox lmSearchRecSubDirs;
        internal CheckBox lmSearchLevSubDirs;
        internal CheckBox lmShowGrid;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button checkForUpdatesButton;
        private Label updateInfoLabel;
    }

}
