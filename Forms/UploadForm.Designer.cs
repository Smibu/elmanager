namespace Elmanager.Forms
{
    partial class UploadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadForm));
            this.uploadLabel = new System.Windows.Forms.Label();
            this.urlBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // uploadLabel
            // 
            this.uploadLabel.AutoSize = true;
            this.uploadLabel.Location = new System.Drawing.Point(12, 9);
            this.uploadLabel.Name = "uploadLabel";
            this.uploadLabel.Size = new System.Drawing.Size(64, 13);
            this.uploadLabel.TabIndex = 0;
            this.uploadLabel.Text = "Uploading...";
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(15, 25);
            this.urlBox.Name = "urlBox";
            this.urlBox.ReadOnly = true;
            this.urlBox.Size = new System.Drawing.Size(281, 20);
            this.urlBox.TabIndex = 1;
            // 
            // UploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 105);
            this.Controls.Add(this.urlBox);
            this.Controls.Add(this.uploadLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UploadForm";
            this.Text = "File upload";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label uploadLabel;
        private System.Windows.Forms.TextBox urlBox;

    }
}