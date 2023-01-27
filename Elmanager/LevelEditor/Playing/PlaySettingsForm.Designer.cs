
namespace Elmanager.LevelEditor.Playing
{
    partial class PlaySettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaySettingsForm));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.loadButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.followDriverComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.gasButton = new System.Windows.Forms.Button();
            this.brakeButton = new System.Windows.Forms.Button();
            this.leftVoltButton = new System.Windows.Forms.Button();
            this.rightVoltButton = new System.Windows.Forms.Button();
            this.aloVoltButton = new System.Windows.Forms.Button();
            this.turnButton = new System.Windows.Forms.Button();
            this.dyingComboBox = new System.Windows.Forms.ComboBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.disableShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.fpsTextBox = new Elmanager.UI.IntTextBox();
            this.constantFpsCheckBox = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.brakeAliasButton = new System.Windows.Forms.Button();
            this.toggleFullscreenCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 806);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(762, 53);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(609, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(150, 46);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(453, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(150, 46);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.loadButton, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.followDriverComboBox, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.gasButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.brakeButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.leftVoltButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.rightVoltButton, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.aloVoltButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.turnButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.dyingComboBox, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.saveButton, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.disableShortcutsCheckBox, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.label14, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 12);
            this.tableLayoutPanel1.Controls.Add(this.label15, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.brakeAliasButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.toggleFullscreenCheckBox, 1, 14);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 15;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(762, 659);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // loadButton
            // 
            this.loadButton.AutoSize = true;
            this.loadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadButton.Location = new System.Drawing.Point(384, 479);
            this.loadButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(150, 42);
            this.loadButton.TabIndex = 20;
            this.loadButton.Text = "RShiftKey";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(313, 484);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 32);
            this.label13.TabIndex = 18;
            this.label13.Text = "Load";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(314, 436);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 32);
            this.label12.TabIndex = 17;
            this.label12.Text = "Save";
            // 
            // followDriverComboBox
            // 
            this.followDriverComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.followDriverComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.followDriverComboBox.FormattingEnabled = true;
            this.followDriverComboBox.Items.AddRange(new object[] {
            "When pressing a playing key",
            "Never"});
            this.followDriverComboBox.Location = new System.Drawing.Point(384, 385);
            this.followDriverComboBox.Name = "followDriverComboBox";
            this.followDriverComboBox.Size = new System.Drawing.Size(375, 40);
            this.followDriverComboBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(326, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gas";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(305, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Brake";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(277, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 32);
            this.label4.TabIndex = 3;
            this.label4.Text = "Left volt";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(261, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 32);
            this.label5.TabIndex = 4;
            this.label5.Text = "Right volt";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(289, 248);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 32);
            this.label6.TabIndex = 5;
            this.label6.Text = "Alovolt";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(315, 296);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 32);
            this.label7.TabIndex = 6;
            this.label7.Text = "Turn";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(234, 343);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 32);
            this.label8.TabIndex = 7;
            this.label8.Text = "When dying";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(227, 389);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(151, 32);
            this.label11.TabIndex = 8;
            this.label11.Text = "Follow driver";
            // 
            // gasButton
            // 
            this.gasButton.AutoSize = true;
            this.gasButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gasButton.Location = new System.Drawing.Point(384, 3);
            this.gasButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.gasButton.Name = "gasButton";
            this.gasButton.Size = new System.Drawing.Size(150, 42);
            this.gasButton.TabIndex = 9;
            this.gasButton.Text = "Up";
            this.gasButton.UseVisualStyleBackColor = true;
            this.gasButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // brakeButton
            // 
            this.brakeButton.AutoSize = true;
            this.brakeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.brakeButton.Location = new System.Drawing.Point(384, 51);
            this.brakeButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.brakeButton.Name = "brakeButton";
            this.brakeButton.Size = new System.Drawing.Size(150, 42);
            this.brakeButton.TabIndex = 10;
            this.brakeButton.Text = "Down";
            this.brakeButton.UseVisualStyleBackColor = true;
            this.brakeButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // leftVoltButton
            // 
            this.leftVoltButton.AutoSize = true;
            this.leftVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftVoltButton.Location = new System.Drawing.Point(384, 147);
            this.leftVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.leftVoltButton.Name = "leftVoltButton";
            this.leftVoltButton.Size = new System.Drawing.Size(150, 42);
            this.leftVoltButton.TabIndex = 11;
            this.leftVoltButton.Text = "Left";
            this.leftVoltButton.UseVisualStyleBackColor = true;
            this.leftVoltButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // rightVoltButton
            // 
            this.rightVoltButton.AutoSize = true;
            this.rightVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightVoltButton.Location = new System.Drawing.Point(384, 195);
            this.rightVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.rightVoltButton.Name = "rightVoltButton";
            this.rightVoltButton.Size = new System.Drawing.Size(150, 42);
            this.rightVoltButton.TabIndex = 12;
            this.rightVoltButton.Text = "Right";
            this.rightVoltButton.UseVisualStyleBackColor = true;
            this.rightVoltButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // aloVoltButton
            // 
            this.aloVoltButton.AutoSize = true;
            this.aloVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.aloVoltButton.Location = new System.Drawing.Point(384, 243);
            this.aloVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.aloVoltButton.Name = "aloVoltButton";
            this.aloVoltButton.Size = new System.Drawing.Size(150, 42);
            this.aloVoltButton.TabIndex = 13;
            this.aloVoltButton.Text = "Insert";
            this.aloVoltButton.UseVisualStyleBackColor = true;
            this.aloVoltButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // turnButton
            // 
            this.turnButton.AutoSize = true;
            this.turnButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.turnButton.Location = new System.Drawing.Point(384, 291);
            this.turnButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.turnButton.Name = "turnButton";
            this.turnButton.Size = new System.Drawing.Size(150, 42);
            this.turnButton.TabIndex = 14;
            this.turnButton.Text = "Space";
            this.turnButton.UseVisualStyleBackColor = true;
            this.turnButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // dyingComboBox
            // 
            this.dyingComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dyingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dyingComboBox.FormattingEnabled = true;
            this.dyingComboBox.Items.AddRange(new object[] {
            "Stop playing",
            "Pause playing",
            "Restart playing",
            "Be invulnerable"});
            this.dyingComboBox.Location = new System.Drawing.Point(384, 339);
            this.dyingComboBox.Name = "dyingComboBox";
            this.dyingComboBox.Size = new System.Drawing.Size(375, 40);
            this.dyingComboBox.TabIndex = 15;
            // 
            // saveButton
            // 
            this.saveButton.AutoSize = true;
            this.saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveButton.Location = new System.Drawing.Point(384, 431);
            this.saveButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 42);
            this.saveButton.TabIndex = 19;
            this.saveButton.Text = "LShiftKey";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // disableShortcutsCheckBox
            // 
            this.disableShortcutsCheckBox.AutoSize = true;
            this.disableShortcutsCheckBox.Location = new System.Drawing.Point(384, 527);
            this.disableShortcutsCheckBox.Name = "disableShortcutsCheckBox";
            this.disableShortcutsCheckBox.Size = new System.Drawing.Size(272, 36);
            this.disableShortcutsCheckBox.TabIndex = 21;
            this.disableShortcutsCheckBox.Text = "Disable shortcut keys";
            this.disableShortcutsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(243, 575);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(135, 32);
            this.label14.TabIndex = 22;
            this.label14.Text = "Physics FPS";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.fpsTextBox);
            this.flowLayoutPanel2.Controls.Add(this.constantFpsCheckBox);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(384, 569);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(303, 45);
            this.flowLayoutPanel2.TabIndex = 24;
            // 
            // fpsTextBox
            // 
            this.fpsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.fpsTextBox.DefaultValue = 1000;
            this.fpsTextBox.Location = new System.Drawing.Point(3, 3);
            this.fpsTextBox.Name = "fpsTextBox";
            this.fpsTextBox.Size = new System.Drawing.Size(150, 39);
            this.fpsTextBox.TabIndex = 23;
            this.fpsTextBox.Text = "1000";
            // 
            // constantFpsCheckBox
            // 
            this.constantFpsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.constantFpsCheckBox.AutoSize = true;
            this.constantFpsCheckBox.Location = new System.Drawing.Point(159, 4);
            this.constantFpsCheckBox.Name = "constantFpsCheckBox";
            this.constantFpsCheckBox.Size = new System.Drawing.Size(141, 36);
            this.constantFpsCheckBox.TabIndex = 22;
            this.constantFpsCheckBox.Text = "Constant";
            this.toolTip1.SetToolTip(this.constantFpsCheckBox, resources.GetString("constantFpsCheckBox.ToolTip"));
            this.constantFpsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(252, 104);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(126, 32);
            this.label15.TabIndex = 25;
            this.label15.Text = "Brake alias";
            // 
            // brakeAliasButton
            // 
            this.brakeAliasButton.AutoSize = true;
            this.brakeAliasButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.brakeAliasButton.Location = new System.Drawing.Point(384, 99);
            this.brakeAliasButton.MinimumSize = new System.Drawing.Size(150, 0);
            this.brakeAliasButton.Name = "brakeAliasButton";
            this.brakeAliasButton.Size = new System.Drawing.Size(150, 42);
            this.brakeAliasButton.TabIndex = 26;
            this.brakeAliasButton.Text = "X";
            this.brakeAliasButton.UseVisualStyleBackColor = true;
            this.brakeAliasButton.Click += new System.EventHandler(this.KeyButtonClick);
            // 
            // toggleFullscreenCheckBox
            // 
            this.toggleFullscreenCheckBox.AutoSize = true;
            this.toggleFullscreenCheckBox.Location = new System.Drawing.Point(384, 620);
            this.toggleFullscreenCheckBox.Name = "toggleFullscreenCheckBox";
            this.toggleFullscreenCheckBox.Size = new System.Drawing.Size(369, 36);
            this.toggleFullscreenCheckBox.TabIndex = 27;
            this.toggleFullscreenCheckBox.Text = "Toggle fullscreen on play/stop";
            this.toolTip1.SetToolTip(this.toggleFullscreenCheckBox, "Regardless of this option, you can use F11 to toggle fullscreen.");
            this.toggleFullscreenCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "label3";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 23);
            this.label9.TabIndex = 0;
            this.label9.Text = "label9";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 0;
            this.label10.Text = "label10";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 10;
            this.toolTip1.AutoPopDelay = 0;
            this.toolTip1.InitialDelay = 1;
            this.toolTip1.ReshowDelay = 2;
            // 
            // PlaySettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(762, 859);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.KeyPreview = true;
            this.Name = "PlaySettingsForm";
            this.Text = "Playing settings";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button gasButton;
        private System.Windows.Forms.Button brakeButton;
        private System.Windows.Forms.Button leftVoltButton;
        private System.Windows.Forms.Button rightVoltButton;
        private System.Windows.Forms.Button aloVoltButton;
        private System.Windows.Forms.Button turnButton;
        private System.Windows.Forms.ComboBox dyingComboBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox followDriverComboBox;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox disableShortcutsCheckBox;
        private System.Windows.Forms.Label label14;
        private UI.IntTextBox fpsTextBox;
        private System.Windows.Forms.CheckBox constantFpsCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button brakeAliasButton;
        private System.Windows.Forms.CheckBox toggleFullscreenCheckBox;
    }
}