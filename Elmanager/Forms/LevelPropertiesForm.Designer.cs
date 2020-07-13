using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public sealed partial class LevelPropertiesForm : FormMod
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
            this.PropertiesLabel = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.OKButton = new System.Windows.Forms.Button();
            this.SinglePlayerTimesBox = new System.Windows.Forms.TextBox();
            this.MultiPlayerTimesBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // PropertiesLabel
            // 
            this.PropertiesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PropertiesLabel.Location = new System.Drawing.Point(12, 9);
            this.PropertiesLabel.Multiline = true;
            this.PropertiesLabel.Name = "PropertiesLabel";
            this.PropertiesLabel.ReadOnly = true;
            this.PropertiesLabel.Size = new System.Drawing.Size(138, 238);
            this.PropertiesLabel.TabIndex = 0;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(196, 9);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(103, 15);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "Singleplayer times";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(447, 9);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(99, 15);
            this.Label3.TabIndex = 3;
            this.Label3.Text = "Multiplayer times";
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.AutoSize = true;
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKButton.Location = new System.Drawing.Point(278, 222);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(87, 25);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // SinglePlayerTimesBox
            // 
            this.SinglePlayerTimesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SinglePlayerTimesBox.Font = new System.Drawing.Font("Lucida Console", 9F);
            this.SinglePlayerTimesBox.Location = new System.Drawing.Point(156, 29);
            this.SinglePlayerTimesBox.Multiline = true;
            this.SinglePlayerTimesBox.Name = "SinglePlayerTimesBox";
            this.SinglePlayerTimesBox.ReadOnly = true;
            this.SinglePlayerTimesBox.Size = new System.Drawing.Size(182, 189);
            this.SinglePlayerTimesBox.TabIndex = 6;
            // 
            // MultiPlayerTimesBox
            // 
            this.MultiPlayerTimesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.MultiPlayerTimesBox.Font = new System.Drawing.Font("Lucida Console", 9F);
            this.MultiPlayerTimesBox.Location = new System.Drawing.Point(367, 29);
            this.MultiPlayerTimesBox.Multiline = true;
            this.MultiPlayerTimesBox.Name = "MultiPlayerTimesBox";
            this.MultiPlayerTimesBox.ReadOnly = true;
            this.MultiPlayerTimesBox.Size = new System.Drawing.Size(258, 189);
            this.MultiPlayerTimesBox.TabIndex = 7;
            // 
            // LevelPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.OKButton;
            this.ClientSize = new System.Drawing.Size(642, 259);
            this.Controls.Add(this.MultiPlayerTimesBox);
            this.Controls.Add(this.SinglePlayerTimesBox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.PropertiesLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LevelPropertiesForm";
            this.Text = "Level properties";
            this.Shown += new System.EventHandler(this.LevelPropertiesForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.TextBox PropertiesLabel;
        internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Button OKButton;
        private TextBox SinglePlayerTimesBox;
        private TextBox MultiPlayerTimesBox;
	}
	
}
