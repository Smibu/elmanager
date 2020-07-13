namespace Elmanager.CustomControls
{
    partial class TriSelect
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.OptionButton1 = new System.Windows.Forms.RadioButton();
            this.OptionButton2 = new System.Windows.Forms.RadioButton();
            this.OptionButton3 = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.OptionButton1);
            this.flowLayoutPanel1.Controls.Add(this.OptionButton2);
            this.flowLayoutPanel1.Controls.Add(this.OptionButton3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(233, 26);
            this.flowLayoutPanel1.TabIndex = 19;
            // 
            // OptionButton1
            // 
            this.OptionButton1.AutoSize = true;
            this.OptionButton1.Location = new System.Drawing.Point(3, 3);
            this.OptionButton1.Name = "OptionButton1";
            this.OptionButton1.Size = new System.Drawing.Size(71, 19);
            this.OptionButton1.TabIndex = 16;
            this.OptionButton1.Text = "Option 1";
            this.OptionButton1.UseVisualStyleBackColor = true;
            // 
            // OptionButton2
            // 
            this.OptionButton2.AutoSize = true;
            this.OptionButton2.Location = new System.Drawing.Point(80, 3);
            this.OptionButton2.Name = "OptionButton2";
            this.OptionButton2.Size = new System.Drawing.Size(71, 19);
            this.OptionButton2.TabIndex = 17;
            this.OptionButton2.Text = "Option 2";
            this.OptionButton2.UseVisualStyleBackColor = true;
            // 
            // OptionButton3
            // 
            this.OptionButton3.AutoSize = true;
            this.OptionButton3.Checked = true;
            this.OptionButton3.Location = new System.Drawing.Point(157, 3);
            this.OptionButton3.Name = "OptionButton3";
            this.OptionButton3.Size = new System.Drawing.Size(71, 19);
            this.OptionButton3.TabIndex = 18;
            this.OptionButton3.TabStop = true;
            this.OptionButton3.Text = "Option 3";
            this.OptionButton3.UseVisualStyleBackColor = true;
            // 
            // TriSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TriSelect";
            this.Size = new System.Drawing.Size(233, 26);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.RadioButton OptionButton3;
        internal System.Windows.Forms.RadioButton OptionButton2;
        internal System.Windows.Forms.RadioButton OptionButton1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
