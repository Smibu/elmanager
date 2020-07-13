using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class RenderingSettingsForm
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
            this.SettingsGrid = new System.Windows.Forms.PropertyGrid();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SettingsGrid
            // 
            this.SettingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsGrid.Location = new System.Drawing.Point(0, 0);
            this.SettingsGrid.Name = "SettingsGrid";
            this.SettingsGrid.Size = new System.Drawing.Size(426, 436);
            this.SettingsGrid.TabIndex = 145;
            this.SettingsGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SettingChanged);
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(351, 0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 146;
            this.CloseButton.Text = "Button1";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseSettings);
            // 
            // RenderingSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(426, 436);
            this.Controls.Add(this.SettingsGrid);
            this.Controls.Add(this.CloseButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "RenderingSettingsForm";
            this.Text = "Rendering settings";
            this.ResumeLayout(false);

        }
		internal System.Windows.Forms.PropertyGrid SettingsGrid;
		internal System.Windows.Forms.Button CloseButton;
	}
	
}
