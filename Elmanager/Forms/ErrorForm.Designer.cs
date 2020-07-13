using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class ErrorForm
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
            this.Label16 = new System.Windows.Forms.Label();
            this.ErrorBox = new System.Windows.Forms.ListBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Location = new System.Drawing.Point(10, 7);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(352, 15);
            this.Label16.TabIndex = 43;
            this.Label16.Text = "The files below could not be loaded. They are probably corrupted.";
            // 
            // ErrorBox
            // 
            this.ErrorBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorBox.HorizontalScrollbar = true;
            this.ErrorBox.IntegralHeight = false;
            this.ErrorBox.ItemHeight = 15;
            this.ErrorBox.Location = new System.Drawing.Point(13, 25);
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.ErrorBox.Size = new System.Drawing.Size(392, 122);
            this.ErrorBox.TabIndex = 44;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteButton.AutoSize = true;
            this.DeleteButton.Location = new System.Drawing.Point(330, 259);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 25);
            this.DeleteButton.TabIndex = 45;
            this.DeleteButton.Text = "Delete all";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteFiles);
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(419, 296);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.Label16);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ErrorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		internal System.Windows.Forms.Label Label16;
		internal System.Windows.Forms.ListBox ErrorBox;
		internal System.Windows.Forms.Button DeleteButton;
		
	}
	
}
