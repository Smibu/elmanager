namespace Elmanager.Forms
{
    partial class SvgImportSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.evenOddRadioButton = new System.Windows.Forms.RadioButton();
            this.nonZeroRadioButton = new System.Windows.Forms.RadioButton();
            this.useOutlinedGeometryBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.smoothnessBar = new System.Windows.Forms.TrackBar();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.nonZeroRadioButton);
            this.groupBox1.Controls.Add(this.evenOddRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(646, 62);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fill rule";
            // 
            // evenOddRadioButton
            // 
            this.evenOddRadioButton.AutoSize = true;
            this.evenOddRadioButton.Checked = true;
            this.evenOddRadioButton.Location = new System.Drawing.Point(6, 21);
            this.evenOddRadioButton.Name = "evenOddRadioButton";
            this.evenOddRadioButton.Size = new System.Drawing.Size(96, 26);
            this.evenOddRadioButton.TabIndex = 0;
            this.evenOddRadioButton.TabStop = true;
            this.evenOddRadioButton.Text = "EvenOdd";
            this.evenOddRadioButton.UseVisualStyleBackColor = true;
            // 
            // nonZeroRadioButton
            // 
            this.nonZeroRadioButton.AutoSize = true;
            this.nonZeroRadioButton.Location = new System.Drawing.Point(108, 21);
            this.nonZeroRadioButton.Name = "nonZeroRadioButton";
            this.nonZeroRadioButton.Size = new System.Drawing.Size(92, 26);
            this.nonZeroRadioButton.TabIndex = 1;
            this.nonZeroRadioButton.Text = "NonZero";
            this.nonZeroRadioButton.UseVisualStyleBackColor = true;
            // 
            // useOutlinedGeometryBox
            // 
            this.useOutlinedGeometryBox.AutoSize = true;
            this.useOutlinedGeometryBox.Location = new System.Drawing.Point(12, 80);
            this.useOutlinedGeometryBox.Name = "useOutlinedGeometryBox";
            this.useOutlinedGeometryBox.Size = new System.Drawing.Size(175, 27);
            this.useOutlinedGeometryBox.TabIndex = 1;
            this.useOutlinedGeometryBox.Text = "Use outlined geometry";
            this.useOutlinedGeometryBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Smoothness:";
            // 
            // smoothnessBar
            // 
            this.smoothnessBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothnessBar.Location = new System.Drawing.Point(101, 103);
            this.smoothnessBar.Maximum = 100;
            this.smoothnessBar.Name = "smoothnessBar";
            this.smoothnessBar.Size = new System.Drawing.Size(557, 90);
            this.smoothnessBar.TabIndex = 3;
            this.smoothnessBar.Value = 38;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(502, 212);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(583, 212);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SvgImportSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 247);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.smoothnessBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.useOutlinedGeometryBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SvgImportSettingsForm";
            this.ShowIcon = false;
            this.Text = "SVG import settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton nonZeroRadioButton;
        private System.Windows.Forms.RadioButton evenOddRadioButton;
        private System.Windows.Forms.CheckBox useOutlinedGeometryBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar smoothnessBar;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}