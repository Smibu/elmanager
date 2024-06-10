
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaySettingsForm));
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            cancelButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            loadButton = new System.Windows.Forms.Button();
            label13 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            followDriverComboBox = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            gasButton = new System.Windows.Forms.Button();
            brakeButton = new System.Windows.Forms.Button();
            leftVoltButton = new System.Windows.Forms.Button();
            rightVoltButton = new System.Windows.Forms.Button();
            aloVoltButton = new System.Windows.Forms.Button();
            turnButton = new System.Windows.Forms.Button();
            dyingComboBox = new System.Windows.Forms.ComboBox();
            saveButton = new System.Windows.Forms.Button();
            disableShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            label14 = new System.Windows.Forms.Label();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            fpsTextBox = new UI.IntTextBox();
            constantFpsCheckBox = new System.Windows.Forms.CheckBox();
            label15 = new System.Windows.Forms.Label();
            brakeAliasButton = new System.Windows.Forms.Button();
            toggleFullscreenCheckBox = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(cancelButton);
            flowLayoutPanel1.Controls.Add(okButton);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 806);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(762, 53);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(609, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(150, 46);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButtonClick;
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(453, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(150, 46);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButtonClick;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(loadButton, 1, 10);
            tableLayoutPanel1.Controls.Add(label13, 0, 10);
            tableLayoutPanel1.Controls.Add(label12, 0, 9);
            tableLayoutPanel1.Controls.Add(followDriverComboBox, 1, 8);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(label7, 0, 6);
            tableLayoutPanel1.Controls.Add(label8, 0, 7);
            tableLayoutPanel1.Controls.Add(label11, 0, 8);
            tableLayoutPanel1.Controls.Add(gasButton, 1, 0);
            tableLayoutPanel1.Controls.Add(brakeButton, 1, 1);
            tableLayoutPanel1.Controls.Add(leftVoltButton, 1, 3);
            tableLayoutPanel1.Controls.Add(rightVoltButton, 1, 4);
            tableLayoutPanel1.Controls.Add(aloVoltButton, 1, 5);
            tableLayoutPanel1.Controls.Add(turnButton, 1, 6);
            tableLayoutPanel1.Controls.Add(dyingComboBox, 1, 7);
            tableLayoutPanel1.Controls.Add(saveButton, 1, 9);
            tableLayoutPanel1.Controls.Add(disableShortcutsCheckBox, 1, 11);
            tableLayoutPanel1.Controls.Add(label14, 0, 12);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 1, 12);
            tableLayoutPanel1.Controls.Add(label15, 0, 2);
            tableLayoutPanel1.Controls.Add(brakeAliasButton, 1, 2);
            tableLayoutPanel1.Controls.Add(toggleFullscreenCheckBox, 1, 14);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 15;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(762, 806);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // loadButton
            // 
            loadButton.AutoSize = true;
            loadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            loadButton.Location = new System.Drawing.Point(384, 479);
            loadButton.MinimumSize = new System.Drawing.Size(150, 0);
            loadButton.Name = "loadButton";
            loadButton.Size = new System.Drawing.Size(150, 42);
            loadButton.TabIndex = 20;
            loadButton.Text = "RShiftKey";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += KeyButtonClick;
            // 
            // label13
            // 
            label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(313, 484);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(65, 32);
            label13.TabIndex = 18;
            label13.Text = "Load";
            // 
            // label12
            // 
            label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(314, 436);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(64, 32);
            label12.TabIndex = 17;
            label12.Text = "Save";
            // 
            // followDriverComboBox
            // 
            followDriverComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            followDriverComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            followDriverComboBox.FormattingEnabled = true;
            followDriverComboBox.Items.AddRange(new object[] { "When pressing a playing key", "Never" });
            followDriverComboBox.Location = new System.Drawing.Point(384, 385);
            followDriverComboBox.Name = "followDriverComboBox";
            followDriverComboBox.Size = new System.Drawing.Size(375, 40);
            followDriverComboBox.TabIndex = 16;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(326, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 32);
            label1.TabIndex = 0;
            label1.Text = "Gas";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(305, 56);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(73, 32);
            label2.TabIndex = 1;
            label2.Text = "Brake";
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(277, 152);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(101, 32);
            label4.TabIndex = 3;
            label4.Text = "Left volt";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(261, 200);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(117, 32);
            label5.TabIndex = 4;
            label5.Text = "Right volt";
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(289, 248);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(89, 32);
            label6.TabIndex = 5;
            label6.Text = "Alovolt";
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(315, 296);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(63, 32);
            label7.TabIndex = 6;
            label7.Text = "Turn";
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(234, 343);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(144, 32);
            label8.TabIndex = 7;
            label8.Text = "When dying";
            // 
            // label11
            // 
            label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(227, 389);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(151, 32);
            label11.TabIndex = 8;
            label11.Text = "Follow driver";
            // 
            // gasButton
            // 
            gasButton.AutoSize = true;
            gasButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            gasButton.Location = new System.Drawing.Point(384, 3);
            gasButton.MinimumSize = new System.Drawing.Size(150, 0);
            gasButton.Name = "gasButton";
            gasButton.Size = new System.Drawing.Size(150, 42);
            gasButton.TabIndex = 9;
            gasButton.Text = "Up";
            gasButton.UseVisualStyleBackColor = true;
            gasButton.Click += KeyButtonClick;
            // 
            // brakeButton
            // 
            brakeButton.AutoSize = true;
            brakeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            brakeButton.Location = new System.Drawing.Point(384, 51);
            brakeButton.MinimumSize = new System.Drawing.Size(150, 0);
            brakeButton.Name = "brakeButton";
            brakeButton.Size = new System.Drawing.Size(150, 42);
            brakeButton.TabIndex = 10;
            brakeButton.Text = "Down";
            brakeButton.UseVisualStyleBackColor = true;
            brakeButton.Click += KeyButtonClick;
            // 
            // leftVoltButton
            // 
            leftVoltButton.AutoSize = true;
            leftVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            leftVoltButton.Location = new System.Drawing.Point(384, 147);
            leftVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            leftVoltButton.Name = "leftVoltButton";
            leftVoltButton.Size = new System.Drawing.Size(150, 42);
            leftVoltButton.TabIndex = 11;
            leftVoltButton.Text = "Left";
            leftVoltButton.UseVisualStyleBackColor = true;
            leftVoltButton.Click += KeyButtonClick;
            // 
            // rightVoltButton
            // 
            rightVoltButton.AutoSize = true;
            rightVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            rightVoltButton.Location = new System.Drawing.Point(384, 195);
            rightVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            rightVoltButton.Name = "rightVoltButton";
            rightVoltButton.Size = new System.Drawing.Size(150, 42);
            rightVoltButton.TabIndex = 12;
            rightVoltButton.Text = "Right";
            rightVoltButton.UseVisualStyleBackColor = true;
            rightVoltButton.Click += KeyButtonClick;
            // 
            // aloVoltButton
            // 
            aloVoltButton.AutoSize = true;
            aloVoltButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            aloVoltButton.Location = new System.Drawing.Point(384, 243);
            aloVoltButton.MinimumSize = new System.Drawing.Size(150, 0);
            aloVoltButton.Name = "aloVoltButton";
            aloVoltButton.Size = new System.Drawing.Size(150, 42);
            aloVoltButton.TabIndex = 13;
            aloVoltButton.Text = "Insert";
            aloVoltButton.UseVisualStyleBackColor = true;
            aloVoltButton.Click += KeyButtonClick;
            // 
            // turnButton
            // 
            turnButton.AutoSize = true;
            turnButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            turnButton.Location = new System.Drawing.Point(384, 291);
            turnButton.MinimumSize = new System.Drawing.Size(150, 0);
            turnButton.Name = "turnButton";
            turnButton.Size = new System.Drawing.Size(150, 42);
            turnButton.TabIndex = 14;
            turnButton.Text = "Space";
            turnButton.UseVisualStyleBackColor = true;
            turnButton.Click += KeyButtonClick;
            // 
            // dyingComboBox
            // 
            dyingComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            dyingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            dyingComboBox.FormattingEnabled = true;
            dyingComboBox.Items.AddRange(new object[] { "Stop playing", "Pause playing", "Restart playing", "Be invulnerable" });
            dyingComboBox.Location = new System.Drawing.Point(384, 339);
            dyingComboBox.Name = "dyingComboBox";
            dyingComboBox.Size = new System.Drawing.Size(375, 40);
            dyingComboBox.TabIndex = 15;
            // 
            // saveButton
            // 
            saveButton.AutoSize = true;
            saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            saveButton.Location = new System.Drawing.Point(384, 431);
            saveButton.MinimumSize = new System.Drawing.Size(150, 0);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(150, 42);
            saveButton.TabIndex = 19;
            saveButton.Text = "LShiftKey";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += KeyButtonClick;
            // 
            // disableShortcutsCheckBox
            // 
            disableShortcutsCheckBox.AutoSize = true;
            disableShortcutsCheckBox.Location = new System.Drawing.Point(384, 527);
            disableShortcutsCheckBox.Name = "disableShortcutsCheckBox";
            disableShortcutsCheckBox.Size = new System.Drawing.Size(272, 36);
            disableShortcutsCheckBox.TabIndex = 21;
            disableShortcutsCheckBox.Text = "Disable shortcut keys";
            disableShortcutsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(243, 575);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(135, 32);
            label14.TabIndex = 22;
            label14.Text = "Physics FPS";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(fpsTextBox);
            flowLayoutPanel2.Controls.Add(constantFpsCheckBox);
            flowLayoutPanel2.Location = new System.Drawing.Point(384, 569);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(303, 45);
            flowLayoutPanel2.TabIndex = 24;
            // 
            // fpsTextBox
            // 
            fpsTextBox.BackColor = System.Drawing.SystemColors.Window;
            fpsTextBox.DefaultValue = 1000;
            fpsTextBox.Location = new System.Drawing.Point(3, 3);
            fpsTextBox.Name = "fpsTextBox";
            fpsTextBox.Size = new System.Drawing.Size(150, 39);
            fpsTextBox.TabIndex = 23;
            fpsTextBox.Text = "1000";
            // 
            // constantFpsCheckBox
            // 
            constantFpsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            constantFpsCheckBox.AutoSize = true;
            constantFpsCheckBox.Location = new System.Drawing.Point(159, 4);
            constantFpsCheckBox.Name = "constantFpsCheckBox";
            constantFpsCheckBox.Size = new System.Drawing.Size(141, 36);
            constantFpsCheckBox.TabIndex = 22;
            constantFpsCheckBox.Text = "Constant";
            toolTip1.SetToolTip(constantFpsCheckBox, resources.GetString("constantFpsCheckBox.ToolTip"));
            constantFpsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(252, 104);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(126, 32);
            label15.TabIndex = 25;
            label15.Text = "Brake alias";
            // 
            // brakeAliasButton
            // 
            brakeAliasButton.AutoSize = true;
            brakeAliasButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            brakeAliasButton.Location = new System.Drawing.Point(384, 99);
            brakeAliasButton.MinimumSize = new System.Drawing.Size(150, 0);
            brakeAliasButton.Name = "brakeAliasButton";
            brakeAliasButton.Size = new System.Drawing.Size(150, 42);
            brakeAliasButton.TabIndex = 26;
            brakeAliasButton.Text = "X";
            brakeAliasButton.UseVisualStyleBackColor = true;
            brakeAliasButton.Click += KeyButtonClick;
            // 
            // toggleFullscreenCheckBox
            // 
            toggleFullscreenCheckBox.AutoSize = true;
            toggleFullscreenCheckBox.Location = new System.Drawing.Point(384, 620);
            toggleFullscreenCheckBox.Name = "toggleFullscreenCheckBox";
            toggleFullscreenCheckBox.Size = new System.Drawing.Size(369, 36);
            toggleFullscreenCheckBox.TabIndex = 27;
            toggleFullscreenCheckBox.Text = "Toggle fullscreen on play/stop";
            toolTip1.SetToolTip(toggleFullscreenCheckBox, "Regardless of this option, you can use F11 to toggle fullscreen.");
            toggleFullscreenCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(0, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(100, 23);
            label3.TabIndex = 0;
            label3.Text = "label3";
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(0, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(100, 23);
            label9.TabIndex = 0;
            label9.Text = "label9";
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(0, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(100, 23);
            label10.TabIndex = 0;
            label10.Text = "label10";
            // 
            // toolTip1
            // 
            toolTip1.AutomaticDelay = 10;
            toolTip1.AutoPopDelay = 0;
            toolTip1.InitialDelay = 1;
            toolTip1.ReshowDelay = 2;
            // 
            // PlaySettingsForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(762, 859);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(flowLayoutPanel1);
            KeyPreview = true;
            Name = "PlaySettingsForm";
            Text = "Playing settings";
            flowLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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