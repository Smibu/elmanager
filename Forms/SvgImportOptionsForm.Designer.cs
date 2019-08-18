namespace Elmanager.Forms
{
    partial class SvgImportOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvgImportOptionsForm));
            this.fillRuleGroupBox = new System.Windows.Forms.GroupBox();
            this.nonZeroRadioButton = new System.Windows.Forms.RadioButton();
            this.evenOddRadioButton = new System.Windows.Forms.RadioButton();
            this.useOutlinedGeometryBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.smoothnessBar = new System.Windows.Forms.TrackBar();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.neverWidenClosedPathsBox = new System.Windows.Forms.CheckBox();
            this.fillRuleGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // fillRuleGroupBox
            // 
            this.fillRuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fillRuleGroupBox.Controls.Add(this.nonZeroRadioButton);
            this.fillRuleGroupBox.Controls.Add(this.evenOddRadioButton);
            this.fillRuleGroupBox.Location = new System.Drawing.Point(165, 11);
            this.fillRuleGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.fillRuleGroupBox.Name = "fillRuleGroupBox";
            this.fillRuleGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.fillRuleGroupBox.Size = new System.Drawing.Size(326, 46);
            this.fillRuleGroupBox.TabIndex = 0;
            this.fillRuleGroupBox.TabStop = false;
            this.fillRuleGroupBox.Text = "Fill rule";
            // 
            // nonZeroRadioButton
            // 
            this.nonZeroRadioButton.AutoSize = true;
            this.nonZeroRadioButton.Location = new System.Drawing.Point(78, 16);
            this.nonZeroRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.nonZeroRadioButton.Name = "nonZeroRadioButton";
            this.nonZeroRadioButton.Size = new System.Drawing.Size(67, 17);
            this.nonZeroRadioButton.TabIndex = 1;
            this.nonZeroRadioButton.Text = "NonZero";
            this.nonZeroRadioButton.UseVisualStyleBackColor = true;
            // 
            // evenOddRadioButton
            // 
            this.evenOddRadioButton.AutoSize = true;
            this.evenOddRadioButton.Checked = true;
            this.evenOddRadioButton.Location = new System.Drawing.Point(4, 16);
            this.evenOddRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.evenOddRadioButton.Name = "evenOddRadioButton";
            this.evenOddRadioButton.Size = new System.Drawing.Size(70, 17);
            this.evenOddRadioButton.TabIndex = 0;
            this.evenOddRadioButton.TabStop = true;
            this.evenOddRadioButton.Text = "EvenOdd";
            this.evenOddRadioButton.UseVisualStyleBackColor = true;
            // 
            // useOutlinedGeometryBox
            // 
            this.useOutlinedGeometryBox.AutoSize = true;
            this.useOutlinedGeometryBox.Location = new System.Drawing.Point(11, 28);
            this.useOutlinedGeometryBox.Margin = new System.Windows.Forms.Padding(2);
            this.useOutlinedGeometryBox.Name = "useOutlinedGeometryBox";
            this.useOutlinedGeometryBox.Size = new System.Drawing.Size(131, 17);
            this.useOutlinedGeometryBox.TabIndex = 1;
            this.useOutlinedGeometryBox.Text = "Use outlined geometry";
            this.useOutlinedGeometryBox.UseVisualStyleBackColor = true;
            this.useOutlinedGeometryBox.CheckedChanged += new System.EventHandler(this.UseOutlinedGeometryBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 104);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Smoothness:";
            // 
            // smoothnessBar
            // 
            this.smoothnessBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothnessBar.Location = new System.Drawing.Point(79, 90);
            this.smoothnessBar.Margin = new System.Windows.Forms.Padding(2);
            this.smoothnessBar.Maximum = 100;
            this.smoothnessBar.Name = "smoothnessBar";
            this.smoothnessBar.Size = new System.Drawing.Size(418, 45);
            this.smoothnessBar.TabIndex = 3;
            this.smoothnessBar.Value = 38;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(341, 167);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(73, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(418, 167);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(73, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // neverWidenClosedPathsBox
            // 
            this.neverWidenClosedPathsBox.AutoSize = true;
            this.neverWidenClosedPathsBox.Location = new System.Drawing.Point(11, 69);
            this.neverWidenClosedPathsBox.Margin = new System.Windows.Forms.Padding(2);
            this.neverWidenClosedPathsBox.Name = "neverWidenClosedPathsBox";
            this.neverWidenClosedPathsBox.Size = new System.Drawing.Size(149, 17);
            this.neverWidenClosedPathsBox.TabIndex = 6;
            this.neverWidenClosedPathsBox.Text = "Never widen closed paths";
            this.neverWidenClosedPathsBox.UseVisualStyleBackColor = true;
            // 
            // SvgImportOptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(502, 201);
            this.Controls.Add(this.neverWidenClosedPathsBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.smoothnessBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.useOutlinedGeometryBox);
            this.Controls.Add(this.fillRuleGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SvgImportOptionsForm";
            this.ShowIcon = false;
            this.Text = "SVG import options";
            this.fillRuleGroupBox.ResumeLayout(false);
            this.fillRuleGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smoothnessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox fillRuleGroupBox;
        private System.Windows.Forms.RadioButton nonZeroRadioButton;
        private System.Windows.Forms.RadioButton evenOddRadioButton;
        private System.Windows.Forms.CheckBox useOutlinedGeometryBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar smoothnessBar;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox neverWidenClosedPathsBox;
    }
}