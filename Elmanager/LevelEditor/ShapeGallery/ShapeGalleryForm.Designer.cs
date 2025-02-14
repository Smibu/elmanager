using System.Windows.Forms;

namespace Elmanager.LevelEditor.ShapeGallery
{
    partial class ShapeGalleryForm
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

            // Dispose of images in the FlowLayoutPanel
            foreach (Control control in flowLayoutPanelShapes.Controls)
            {
                if (control is CustomShapeControl customShapeControl)
                {
                    customShapeControl.Dispose();
                }
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
            flowLayoutPanelShapes = new FlowLayoutPanel();
            buttonCancel = new Button();
            buttonOk = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            comboBoxSubfolders = new ComboBox();
            panel2 = new Panel();
            resetValuesButton = new Button();
            mirrorComboBox = new ComboBox();
            mirrorLabel = new Label();
            scalingNumericUpDown = new NumericUpDown();
            rotationNumericUpDown = new NumericUpDown();
            rotationLabel = new Label();
            scalingLabel = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            vScrollBar1 = new VScrollBar();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scalingNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rotationNumericUpDown).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanelShapes
            // 
            flowLayoutPanelShapes.Dock = DockStyle.Fill;
            flowLayoutPanelShapes.Location = new System.Drawing.Point(3, 3);
            flowLayoutPanelShapes.Margin = new Padding(3, 3, 0, 3);
            flowLayoutPanelShapes.Name = "flowLayoutPanelShapes";
            flowLayoutPanelShapes.Size = new System.Drawing.Size(568, 542);
            flowLayoutPanelShapes.TabIndex = 0;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Location = new System.Drawing.Point(510, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonOk
            // 
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Location = new System.Drawing.Point(429, 4);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new System.Drawing.Size(75, 23);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += ButtonOk_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 2);
            tableLayoutPanel1.Controls.Add(panel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanel1.Size = new System.Drawing.Size(594, 646);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.Controls.Add(comboBoxSubfolders);
            panel1.Controls.Add(buttonOk);
            panel1.Controls.Add(buttonCancel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(3, 613);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(588, 30);
            panel1.TabIndex = 1;
            // 
            // comboBoxSubfolders
            // 
            comboBoxSubfolders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            comboBoxSubfolders.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSubfolders.FormattingEnabled = true;
            comboBoxSubfolders.Location = new System.Drawing.Point(3, 4);
            comboBoxSubfolders.Name = "comboBoxSubfolders";
            comboBoxSubfolders.Size = new System.Drawing.Size(366, 23);
            comboBoxSubfolders.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Controls.Add(resetValuesButton);
            panel2.Controls.Add(mirrorComboBox);
            panel2.Controls.Add(mirrorLabel);
            panel2.Controls.Add(scalingNumericUpDown);
            panel2.Controls.Add(rotationNumericUpDown);
            panel2.Controls.Add(rotationLabel);
            panel2.Controls.Add(scalingLabel);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(3, 557);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(588, 50);
            panel2.TabIndex = 2;
            // 
            // resetValuesButton
            // 
            resetValuesButton.Location = new System.Drawing.Point(294, 23);
            resetValuesButton.Name = "resetValuesButton";
            resetValuesButton.Size = new System.Drawing.Size(75, 25);
            resetValuesButton.TabIndex = 8;
            resetValuesButton.Text = "Reset";
            resetValuesButton.UseVisualStyleBackColor = true;
            resetValuesButton.Click += resetValuesButton_Click;
            // 
            // mirrorComboBox
            // 
            mirrorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mirrorComboBox.FormattingEnabled = true;
            mirrorComboBox.Location = new System.Drawing.Point(197, 24);
            mirrorComboBox.Name = "mirrorComboBox";
            mirrorComboBox.Size = new System.Drawing.Size(91, 23);
            mirrorComboBox.TabIndex = 7;
            // 
            // mirrorLabel
            // 
            mirrorLabel.Location = new System.Drawing.Point(197, 0);
            mirrorLabel.Name = "mirrorLabel";
            mirrorLabel.Size = new System.Drawing.Size(91, 23);
            mirrorLabel.TabIndex = 6;
            mirrorLabel.Text = "Mirror Option:";
            mirrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scalingNumericUpDown
            // 
            scalingNumericUpDown.DecimalPlaces = 3;
            scalingNumericUpDown.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            scalingNumericUpDown.Location = new System.Drawing.Point(3, 24);
            scalingNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            scalingNumericUpDown.Name = "scalingNumericUpDown";
            scalingNumericUpDown.Size = new System.Drawing.Size(91, 23);
            scalingNumericUpDown.TabIndex = 5;
            scalingNumericUpDown.TextAlign = HorizontalAlignment.Right;
            scalingNumericUpDown.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // rotationNumericUpDown
            // 
            rotationNumericUpDown.DecimalPlaces = 3;
            rotationNumericUpDown.Location = new System.Drawing.Point(100, 24);
            rotationNumericUpDown.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            rotationNumericUpDown.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            rotationNumericUpDown.Name = "rotationNumericUpDown";
            rotationNumericUpDown.Size = new System.Drawing.Size(91, 23);
            rotationNumericUpDown.TabIndex = 4;
            rotationNumericUpDown.TextAlign = HorizontalAlignment.Right;
            // 
            // rotationLabel
            // 
            rotationLabel.Location = new System.Drawing.Point(100, 0);
            rotationLabel.Name = "rotationLabel";
            rotationLabel.Size = new System.Drawing.Size(91, 23);
            rotationLabel.TabIndex = 3;
            rotationLabel.Text = "Rotation Angle:";
            rotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scalingLabel
            // 
            scalingLabel.Location = new System.Drawing.Point(3, 0);
            scalingLabel.Name = "scalingLabel";
            scalingLabel.Size = new System.Drawing.Size(91, 23);
            scalingLabel.TabIndex = 0;
            scalingLabel.Text = "Scaling Factor:";
            scalingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 17F));
            tableLayoutPanel2.Controls.Add(flowLayoutPanelShapes, 0, 0);
            tableLayoutPanel2.Controls.Add(vScrollBar1, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(588, 548);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            vScrollBar1.Location = new System.Drawing.Point(571, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new System.Drawing.Size(17, 548);
            vScrollBar1.TabIndex = 1;
            // 
            // ShapeGalleryForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(594, 646);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(610, 300);
            Name = "ShapeGalleryForm";
            Text = "ShapeGallery";
            Load += ShapeGalleryForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scalingNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)rotationNumericUpDown).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelShapes;
        private Button buttonCancel;
        private Button buttonOk;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private ComboBox comboBoxSubfolders;
        private Panel panel2;
        private Label scalingLabel;
        private NumericUpDown scalingNumericUpDown;
        private NumericUpDown rotationNumericUpDown;
        private Label rotationLabel;
        private ComboBox mirrorComboBox;
        private Label mirrorLabel;
        private Button resetValuesButton;
        private TableLayoutPanel tableLayoutPanel2;
        private VScrollBar vScrollBar1;
    }
}