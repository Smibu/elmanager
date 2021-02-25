namespace Elmanager.Updating
{
    partial class NewVersionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.downloadButton = new System.Windows.Forms.Button();
            this.notYetButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.versionInfoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // downloadButton
            // 
            this.downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadButton.AutoSize = true;
            this.downloadButton.Location = new System.Drawing.Point(14, 50);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(108, 25);
            this.downloadButton.TabIndex = 2;
            this.downloadButton.Text = "Download";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.DownloadButtonClick);
            // 
            // notYetButton
            // 
            this.notYetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.notYetButton.AutoSize = true;
            this.notYetButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.notYetButton.Location = new System.Drawing.Point(128, 50);
            this.notYetButton.Name = "notYetButton";
            this.notYetButton.Size = new System.Drawing.Size(108, 25);
            this.notYetButton.TabIndex = 3;
            this.notYetButton.Text = "Not yet";
            this.notYetButton.UseVisualStyleBackColor = true;
            this.notYetButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(360, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Note: Elmanager will automatically exit if you choose to download.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(39, 50);
            this.linkLabel1.Location = new System.Drawing.Point(11, 9);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(285, 21);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "New version of Elmanager is available! What\'s new?";
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // versionInfoLabel
            // 
            this.versionInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.versionInfoLabel.AutoSize = true;
            this.versionInfoLabel.Location = new System.Drawing.Point(11, 30);
            this.versionInfoLabel.Name = "versionInfoLabel";
            this.versionInfoLabel.Size = new System.Drawing.Size(82, 15);
            this.versionInfoLabel.TabIndex = 6;
            this.versionInfoLabel.Text = "Latest version:";
            // 
            // NewVersionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.notYetButton;
            this.ClientSize = new System.Drawing.Size(379, 100);
            this.Controls.Add(this.versionInfoLabel);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.notYetButton);
            this.Controls.Add(this.downloadButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "NewVersionForm";
            this.Text = "New version available";
            this.Load += new System.EventHandler(this.NewVersionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button notYetButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label versionInfoLabel;
    }
}