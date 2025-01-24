namespace Elmanager.LevelEditor.ShapeGallery
{
    partial class CustomShapeControl
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

            if (picShapeImage.Image != null)
            {
                picShapeImage.Image.Dispose();
                picShapeImage.Image = null;
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
            picShapeImage = new System.Windows.Forms.PictureBox();
            lblShapeName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)picShapeImage).BeginInit();
            SuspendLayout();
            // 
            // picShapeImage
            // 
            picShapeImage.BackColor = System.Drawing.Color.LightGray;
            picShapeImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            picShapeImage.Location = new System.Drawing.Point(1, 1);
            picShapeImage.Name = "picShapeImage";
            picShapeImage.Size = new System.Drawing.Size(128, 128);
            picShapeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            picShapeImage.TabIndex = 0;
            picShapeImage.TabStop = false;
            // 
            // lblShapeName
            // 
            lblShapeName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblShapeName.Location = new System.Drawing.Point(1, 130);
            lblShapeName.Name = "lblShapeName";
            lblShapeName.Size = new System.Drawing.Size(128, 41);
            lblShapeName.TabIndex = 1;
            lblShapeName.Text = "Shape Name Goes Here";
            lblShapeName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CustomShapeControl
            // 
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(lblShapeName);
            Controls.Add(picShapeImage);
            Name = "CustomShapeControl";
            Size = new System.Drawing.Size(132, 174);
            MouseDown += OnMouseDown;
            ((System.ComponentModel.ISupportInitialize)picShapeImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox picShapeImage;
        private System.Windows.Forms.Label lblShapeName;
    }
}
