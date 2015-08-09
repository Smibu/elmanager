namespace Elmanager.Forms
{
	public partial class ErrorForm : System.Windows.Forms.Form
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
                this.Label16 = new System.Windows.Forms.Label();
                this.ErrorBox = new System.Windows.Forms.ListBox();
                this.DeleteButton = new System.Windows.Forms.Button();
                this.SuspendLayout();
                // 
                // Label16
                // 
                this.Label16.Location = new System.Drawing.Point(36, 21);
                this.Label16.Name = "Label16";
                this.Label16.Size = new System.Drawing.Size(346, 45);
                this.Label16.TabIndex = 43;
                this.Label16.Text = "Replay manager has detected that the following replays are erroneous. However, if" +
                    " you are able to play these replays with original Elma, send them to Smibu.";
                // 
                // ErrorBox
                // 
                this.ErrorBox.FormattingEnabled = true;
                this.ErrorBox.HorizontalScrollbar = true;
                this.ErrorBox.Location = new System.Drawing.Point(36, 69);
                this.ErrorBox.Name = "ErrorBox";
                this.ErrorBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
                this.ErrorBox.Size = new System.Drawing.Size(346, 160);
                this.ErrorBox.TabIndex = 44;
                // 
                // DeleteButton
                // 
                this.DeleteButton.Location = new System.Drawing.Point(172, 235);
                this.DeleteButton.Name = "DeleteButton";
                this.DeleteButton.Size = new System.Drawing.Size(75, 23);
                this.DeleteButton.TabIndex = 45;
                this.DeleteButton.Text = "Delete all";
                this.DeleteButton.UseVisualStyleBackColor = true;
                this.DeleteButton.Click += new System.EventHandler(this.DeleteReplays);
                // 
                // ErrorForm
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(419, 267);
                this.Controls.Add(this.DeleteButton);
                this.Controls.Add(this.ErrorBox);
                this.Controls.Add(this.Label16);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
                this.MaximizeBox = false;
                this.Name = "ErrorForm";
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                this.Text = "Warning";
                this.ResumeLayout(false);

		}
		internal System.Windows.Forms.Label Label16;
		internal System.Windows.Forms.ListBox ErrorBox;
		internal System.Windows.Forms.Button DeleteButton;
		
	}
	
}
