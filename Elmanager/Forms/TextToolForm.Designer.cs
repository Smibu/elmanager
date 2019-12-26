namespace Elmanager.Forms
{
    partial class TextToolForm
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fontButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.smoothnessBar = new System.Windows.Forms.TrackBar();
            this.lineHeightBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineHeightBar)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(12, 6);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox.Size = new System.Drawing.Size(332, 170);
            this.textBox.TabIndex = 0;
            this.textBox.WordWrap = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(188, 282);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(269, 282);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // fontButton
            // 
            this.fontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fontButton.Location = new System.Drawing.Point(12, 282);
            this.fontButton.Name = "fontButton";
            this.fontButton.Size = new System.Drawing.Size(75, 23);
            this.fontButton.TabIndex = 4;
            this.fontButton.Text = "Font...";
            this.fontButton.UseVisualStyleBackColor = true;
            this.fontButton.Click += new System.EventHandler(this.fontButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Smoothness:";
            // 
            // smoothnessBar
            // 
            this.smoothnessBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothnessBar.Location = new System.Drawing.Point(83, 233);
            this.smoothnessBar.Maximum = 100;
            this.smoothnessBar.Minimum = 30;
            this.smoothnessBar.Name = "smoothnessBar";
            this.smoothnessBar.Size = new System.Drawing.Size(261, 45);
            this.smoothnessBar.TabIndex = 7;
            this.smoothnessBar.Value = 38;
            // 
            // lineHeightBar
            // 
            this.lineHeightBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lineHeightBar.Location = new System.Drawing.Point(83, 182);
            this.lineHeightBar.Maximum = 1000;
            this.lineHeightBar.Minimum = 1;
            this.lineHeightBar.Name = "lineHeightBar";
            this.lineHeightBar.Size = new System.Drawing.Size(261, 45);
            this.lineHeightBar.TabIndex = 8;
            this.lineHeightBar.TickFrequency = 100;
            this.lineHeightBar.Value = 38;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 192);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Line height:";
            // 
            // TextToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(356, 317);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lineHeightBar);
            this.Controls.Add(this.smoothnessBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fontButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textBox);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(278, 227);
            this.Name = "TextToolForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Enter text";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Prompt_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineHeightBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button fontButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar smoothnessBar;
        private System.Windows.Forms.TrackBar lineHeightBar;
        private System.Windows.Forms.Label label1;
    }
}