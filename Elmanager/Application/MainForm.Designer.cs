﻿
namespace Elmanager.Application
{
    internal partial class MainForm
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
            this.rmButton = new System.Windows.Forms.Button();
            this.levelEditorButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.byLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.homePageLabel = new System.Windows.Forms.LinkLabel();
            this.configButton = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.levelManagerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rmButton
            // 
            this.rmButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rmButton.AutoSize = true;
            this.rmButton.Location = new System.Drawing.Point(174, 292);
            this.rmButton.Margin = new System.Windows.Forms.Padding(6);
            this.rmButton.Name = "rmButton";
            this.rmButton.Size = new System.Drawing.Size(276, 50);
            this.rmButton.TabIndex = 1;
            this.rmButton.Text = "Replay manager";
            this.rmButton.UseVisualStyleBackColor = true;
            this.rmButton.Click += new System.EventHandler(this.OpenReplayManager);
            // 
            // levelEditorButton
            // 
            this.levelEditorButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.levelEditorButton.AutoSize = true;
            this.levelEditorButton.Location = new System.Drawing.Point(174, 408);
            this.levelEditorButton.Margin = new System.Windows.Forms.Padding(6);
            this.levelEditorButton.Name = "levelEditorButton";
            this.levelEditorButton.Size = new System.Drawing.Size(276, 50);
            this.levelEditorButton.TabIndex = 3;
            this.levelEditorButton.Text = "SLE";
            this.levelEditorButton.UseVisualStyleBackColor = true;
            this.levelEditorButton.Click += new System.EventHandler(this.OpenLevelEditor);
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.titleLabel.Location = new System.Drawing.Point(24, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(576, 110);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Elmanager";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // byLabel
            // 
            this.byLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.byLabel.AutoSize = true;
            this.byLabel.Location = new System.Drawing.Point(256, 110);
            this.byLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.byLabel.Name = "byLabel";
            this.byLabel.Size = new System.Drawing.Size(115, 32);
            this.byLabel.TabIndex = 4;
            this.byLabel.Text = "by Smibu";
            this.byLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.Location = new System.Drawing.Point(24, 181);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(576, 32);
            this.versionLabel.TabIndex = 5;
            this.versionLabel.Text = "Version";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // homePageLabel
            // 
            this.homePageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.homePageLabel.Location = new System.Drawing.Point(24, 232);
            this.homePageLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.homePageLabel.Name = "homePageLabel";
            this.homePageLabel.Size = new System.Drawing.Size(576, 32);
            this.homePageLabel.TabIndex = 7;
            this.homePageLabel.TabStop = true;
            this.homePageLabel.Text = "https://m-l.fi/Elma/";
            this.homePageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.homePageLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HomePageClicked);
            // 
            // configButton
            // 
            this.configButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configButton.AutoSize = true;
            this.configButton.Location = new System.Drawing.Point(174, 466);
            this.configButton.Margin = new System.Windows.Forms.Padding(6);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(276, 50);
            this.configButton.TabIndex = 4;
            this.configButton.Text = "Configuration";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.ConfigButtonClick);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(41, 6);
            this.linkLabel1.Location = new System.Drawing.Point(64, 548);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(518, 38);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Testing && SLE toolbar graphics: Mawane (R.I.P.)";
            this.toolTip1.SetToolTip(this.linkLabel1, "https://web.archive.org/web/20160121234513/https://www.oscarstours.ca/avis-de-dec" +
        "es/m-marck-antoine-simoneau");
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1LinkClicked);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.InitialDelay = 1;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.UseFading = false;
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(224, 582);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(196, 32);
            this.linkLabel2.TabIndex = 6;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "License (libraries)";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // levelManagerButton
            // 
            this.levelManagerButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.levelManagerButton.AutoSize = true;
            this.levelManagerButton.Location = new System.Drawing.Point(174, 350);
            this.levelManagerButton.Margin = new System.Windows.Forms.Padding(6);
            this.levelManagerButton.Name = "levelManagerButton";
            this.levelManagerButton.Size = new System.Drawing.Size(276, 50);
            this.levelManagerButton.TabIndex = 2;
            this.levelManagerButton.Text = "Level manager";
            this.levelManagerButton.UseVisualStyleBackColor = true;
            this.levelManagerButton.Click += new System.EventHandler(this.levelManagerButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 642);
            this.Controls.Add(this.levelManagerButton);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.configButton);
            this.Controls.Add(this.homePageLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.byLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.levelEditorButton);
            this.Controls.Add(this.rmButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Elmanager";
            this.Load += new System.EventHandler(this.StartUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		internal System.Windows.Forms.Button rmButton;
		internal System.Windows.Forms.Button levelEditorButton;
		internal System.Windows.Forms.Label titleLabel;
		internal System.Windows.Forms.Label byLabel;
		internal System.Windows.Forms.Label versionLabel;
		internal System.Windows.Forms.LinkLabel homePageLabel;
        internal System.Windows.Forms.Button configButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        internal System.Windows.Forms.Button levelManagerButton;
    }
	
}
