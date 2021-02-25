
namespace Elmanager.ReplayManager
{
    internal partial class CompareForm
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
            this.ResultsBox = new System.Windows.Forms.ListBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.LevelLabel = new System.Windows.Forms.Label();
            this.AppleButton = new System.Windows.Forms.RadioButton();
            this.GTButton = new System.Windows.Forms.RadioButton();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.CompareButton = new System.Windows.Forms.Button();
            this.CRBox = new System.Windows.Forms.ListBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.CPListBox = new System.Windows.Forms.CheckedListBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GroupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ResultsBox
            // 
            this.ResultsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultsBox.FormattingEnabled = true;
            this.ResultsBox.ItemHeight = 15;
            this.ResultsBox.Location = new System.Drawing.Point(11, 246);
            this.ResultsBox.Name = "ResultsBox";
            this.ResultsBox.Size = new System.Drawing.Size(315, 214);
            this.ResultsBox.TabIndex = 51;
            this.ResultsBox.Visible = false;
            // 
            // Label19
            // 
            this.Label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label19.AutoSize = true;
            this.Label19.Location = new System.Drawing.Point(8, 228);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(148, 15);
            this.Label19.TabIndex = 50;
            this.Label19.Text = "Replay comparison results:";
            this.Label19.Visible = false;
            // 
            // LevelLabel
            // 
            this.LevelLabel.AutoSize = true;
            this.LevelLabel.Location = new System.Drawing.Point(9, 9);
            this.LevelLabel.Name = "LevelLabel";
            this.LevelLabel.Size = new System.Drawing.Size(37, 15);
            this.LevelLabel.TabIndex = 52;
            this.LevelLabel.Text = "Level:";
            // 
            // AppleButton
            // 
            this.AppleButton.AutoSize = true;
            this.AppleButton.Checked = true;
            this.AppleButton.Location = new System.Drawing.Point(3, 3);
            this.AppleButton.Name = "AppleButton";
            this.AppleButton.Size = new System.Drawing.Size(56, 19);
            this.AppleButton.TabIndex = 54;
            this.AppleButton.TabStop = true;
            this.AppleButton.Text = "Apple";
            this.AppleButton.UseVisualStyleBackColor = true;
            // 
            // GTButton
            // 
            this.GTButton.AutoSize = true;
            this.GTButton.Location = new System.Drawing.Point(65, 3);
            this.GTButton.Name = "GTButton";
            this.GTButton.Size = new System.Drawing.Size(96, 19);
            this.GTButton.TabIndex = 55;
            this.GTButton.Text = "Groundtouch";
            this.GTButton.UseVisualStyleBackColor = true;
            this.GTButton.CheckedChanged += new System.EventHandler(this.RadioButton2CheckedChanged);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.panel1);
            this.GroupBox1.Location = new System.Drawing.Point(12, 34);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(199, 52);
            this.GroupBox1.TabIndex = 56;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Type of checkpoints";
            // 
            // CompareButton
            // 
            this.CompareButton.AutoSize = true;
            this.CompareButton.Location = new System.Drawing.Point(217, 53);
            this.CompareButton.Name = "CompareButton";
            this.CompareButton.Size = new System.Drawing.Size(85, 25);
            this.CompareButton.TabIndex = 57;
            this.CompareButton.Text = "Compare";
            this.CompareButton.UseVisualStyleBackColor = true;
            this.CompareButton.Click += new System.EventHandler(this.Compare);
            // 
            // CRBox
            // 
            this.CRBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CRBox.FormattingEnabled = true;
            this.CRBox.IntegralHeight = false;
            this.CRBox.ItemHeight = 15;
            this.CRBox.Location = new System.Drawing.Point(11, 107);
            this.CRBox.Name = "CRBox";
            this.CRBox.Size = new System.Drawing.Size(315, 111);
            this.CRBox.TabIndex = 58;
            this.CRBox.SelectedIndexChanged += new System.EventHandler(this.CrBoxSelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(9, 89);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(129, 15);
            this.Label2.TabIndex = 59;
            this.Label2.Text = "Replays in comparison:";
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(329, 89);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(175, 15);
            this.Label3.TabIndex = 60;
            this.Label3.Text = "Checkpoints for selected replay:";
            this.Label3.Visible = false;
            // 
            // CPListBox
            // 
            this.CPListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CPListBox.CheckOnClick = true;
            this.CPListBox.FormattingEnabled = true;
            this.CPListBox.IntegralHeight = false;
            this.CPListBox.Location = new System.Drawing.Point(332, 107);
            this.CPListBox.Name = "CPListBox";
            this.CPListBox.Size = new System.Drawing.Size(168, 353);
            this.CPListBox.TabIndex = 61;
            this.CPListBox.Visible = false;
            this.CPListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CpListBoxItemCheck);
            // 
            // Label4
            // 
            this.Label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(8, 465);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(93, 15);
            this.Label4.TabIndex = 63;
            this.Label4.Text = "Combined time:";
            this.Label4.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.GTButton);
            this.panel1.Controls.Add(this.AppleButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(193, 30);
            this.panel1.TabIndex = 64;
            // 
            // CompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(509, 489);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.CPListBox);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.CRBox);
            this.Controls.Add(this.CompareButton);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.LevelLabel);
            this.Controls.Add(this.ResultsBox);
            this.Controls.Add(this.Label19);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(525, 525);
            this.Name = "CompareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Replay comparison";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		internal System.Windows.Forms.ListBox ResultsBox;
		internal System.Windows.Forms.Label Label19;
		internal System.Windows.Forms.Label LevelLabel;
		internal System.Windows.Forms.RadioButton AppleButton;
		internal System.Windows.Forms.RadioButton GTButton;
		internal System.Windows.Forms.GroupBox GroupBox1;
		internal System.Windows.Forms.Button CompareButton;
		internal System.Windows.Forms.ListBox CRBox;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.CheckedListBox CPListBox;
		internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Panel panel1;
    }
	
}
