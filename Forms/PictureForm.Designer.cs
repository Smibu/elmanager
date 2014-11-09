namespace Elmanager.Forms
{
	public partial class PictureForm : System.Windows.Forms.Form
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
            this.OKButton = new System.Windows.Forms.Button();
            this.CButton = new System.Windows.Forms.Button();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.PictureComboBox = new System.Windows.Forms.ComboBox();
            this.TextureComboBox = new System.Windows.Forms.ComboBox();
            this.TextureButton = new System.Windows.Forms.RadioButton();
            this.PictureButton = new System.Windows.Forms.RadioButton();
            this.Label1 = new System.Windows.Forms.Label();
            this.MaskComboBox = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.DistanceBox = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.ClippingComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OKButton.Location = new System.Drawing.Point(12, 159);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.ButtonClick);
            // 
            // CButton
            // 
            this.CButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CButton.Location = new System.Drawing.Point(93, 159);
            this.CButton.Name = "CButton";
            this.CButton.Size = new System.Drawing.Size(75, 23);
            this.CButton.TabIndex = 1;
            this.CButton.Text = "Cancel";
            this.CButton.UseVisualStyleBackColor = true;
            this.CButton.Click += new System.EventHandler(this.ButtonClick);
            // 
            // ImageBox
            // 
            this.ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBox.Location = new System.Drawing.Point(221, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(150, 170);
            this.ImageBox.TabIndex = 2;
            this.ImageBox.TabStop = false;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.PictureComboBox);
            this.Panel1.Controls.Add(this.TextureComboBox);
            this.Panel1.Controls.Add(this.TextureButton);
            this.Panel1.Controls.Add(this.PictureButton);
            this.Panel1.Location = new System.Drawing.Point(12, 12);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(201, 48);
            this.Panel1.TabIndex = 3;
            // 
            // PictureComboBox
            // 
            this.PictureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PictureComboBox.FormattingEnabled = true;
            this.PictureComboBox.Location = new System.Drawing.Point(70, 2);
            this.PictureComboBox.Name = "PictureComboBox";
            this.PictureComboBox.Size = new System.Drawing.Size(121, 21);
            this.PictureComboBox.Sorted = true;
            this.PictureComboBox.TabIndex = 5;
            // 
            // TextureComboBox
            // 
            this.TextureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextureComboBox.FormattingEnabled = true;
            this.TextureComboBox.Location = new System.Drawing.Point(70, 25);
            this.TextureComboBox.Name = "TextureComboBox";
            this.TextureComboBox.Size = new System.Drawing.Size(121, 21);
            this.TextureComboBox.Sorted = true;
            this.TextureComboBox.TabIndex = 4;
            // 
            // TextureButton
            // 
            this.TextureButton.AutoSize = true;
            this.TextureButton.Location = new System.Drawing.Point(3, 26);
            this.TextureButton.Name = "TextureButton";
            this.TextureButton.Size = new System.Drawing.Size(61, 17);
            this.TextureButton.TabIndex = 1;
            this.TextureButton.Text = "Texture";
            this.TextureButton.UseVisualStyleBackColor = true;
            // 
            // PictureButton
            // 
            this.PictureButton.AutoSize = true;
            this.PictureButton.Checked = true;
            this.PictureButton.Location = new System.Drawing.Point(3, 3);
            this.PictureButton.Name = "PictureButton";
            this.PictureButton.Size = new System.Drawing.Size(58, 17);
            this.PictureButton.TabIndex = 0;
            this.PictureButton.TabStop = true;
            this.PictureButton.Text = "Picture";
            this.PictureButton.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(40, 73);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(33, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Mask";
            // 
            // MaskComboBox
            // 
            this.MaskComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MaskComboBox.FormattingEnabled = true;
            this.MaskComboBox.Location = new System.Drawing.Point(82, 70);
            this.MaskComboBox.Name = "MaskComboBox";
            this.MaskComboBox.Size = new System.Drawing.Size(121, 21);
            this.MaskComboBox.Sorted = true;
            this.MaskComboBox.TabIndex = 6;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(24, 100);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(49, 13);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "Distance";
            // 
            // DistanceBox
            // 
            this.DistanceBox.Location = new System.Drawing.Point(82, 97);
            this.DistanceBox.MaxLength = 20;
            this.DistanceBox.Name = "DistanceBox";
            this.DistanceBox.Size = new System.Drawing.Size(121, 20);
            this.DistanceBox.TabIndex = 8;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(29, 126);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(44, 13);
            this.Label3.TabIndex = 9;
            this.Label3.Text = "Clipping";
            // 
            // ClippingComboBox
            // 
            this.ClippingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ClippingComboBox.FormattingEnabled = true;
            this.ClippingComboBox.Location = new System.Drawing.Point(82, 123);
            this.ClippingComboBox.Name = "ClippingComboBox";
            this.ClippingComboBox.Size = new System.Drawing.Size(121, 21);
            this.ClippingComboBox.TabIndex = 10;
            // 
            // PictureForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CButton;
            this.ClientSize = new System.Drawing.Size(383, 194);
            this.Controls.Add(this.ClippingComboBox);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.DistanceBox);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.MaskComboBox);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.CButton);
            this.Controls.Add(this.OKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PictureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Picture/texture properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreventClose);
            this.VisibleChanged += new System.EventHandler(this.WhenShown);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		internal System.Windows.Forms.Button OKButton;
		internal System.Windows.Forms.Button CButton;
		internal System.Windows.Forms.PictureBox ImageBox;
		internal System.Windows.Forms.Panel Panel1;
		internal System.Windows.Forms.ComboBox PictureComboBox;
		internal System.Windows.Forms.ComboBox TextureComboBox;
		internal System.Windows.Forms.RadioButton TextureButton;
		internal System.Windows.Forms.RadioButton PictureButton;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.ComboBox MaskComboBox;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.TextBox DistanceBox;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.ComboBox ClippingComboBox;
	}
	
}
