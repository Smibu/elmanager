<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ViewerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.CheckBox2 = New System.Windows.Forms.CheckBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.CheckBox3 = New System.Windows.Forms.CheckBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.SaveLevButton = New System.Windows.Forms.Button
        Me.MirrorButton = New System.Windows.Forms.Button
        Me.SavePicButton = New System.Windows.Forms.Button
        Me.ZoomFillButton = New System.Windows.Forms.Button
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(285, 231)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(6, 18)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(83, 17)
        Me.CheckBox1.TabIndex = 1
        Me.CheckBox1.Text = "Fill polygons"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(95, 18)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(79, 17)
        Me.CheckBox2.TabIndex = 2
        Me.CheckBox2.Text = "Antialiasing"
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckBox3)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.SaveLevButton)
        Me.GroupBox1.Controls.Add(Me.MirrorButton)
        Me.GroupBox1.Controls.Add(Me.SavePicButton)
        Me.GroupBox1.Controls.Add(Me.ZoomFillButton)
        Me.GroupBox1.Controls.Add(Me.CheckBox2)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 237)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(278, 92)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Checked = Global.LevelManager.My.MySettings.Default.SmartZoom
        Me.CheckBox3.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.LevelManager.My.MySettings.Default, "SmartZoom", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.CheckBox3.Location = New System.Drawing.Point(184, 65)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(81, 17)
        Me.CheckBox3.TabIndex = 8
        Me.CheckBox3.Text = "Smart zoom"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 66)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(10, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = " "
        '
        'SaveLevButton
        '
        Me.SaveLevButton.Location = New System.Drawing.Point(184, 41)
        Me.SaveLevButton.Name = "SaveLevButton"
        Me.SaveLevButton.Size = New System.Drawing.Size(83, 22)
        Me.SaveLevButton.TabIndex = 6
        Me.SaveLevButton.Text = "Save level"
        Me.SaveLevButton.UseVisualStyleBackColor = True
        '
        'MirrorButton
        '
        Me.MirrorButton.Location = New System.Drawing.Point(95, 41)
        Me.MirrorButton.Name = "MirrorButton"
        Me.MirrorButton.Size = New System.Drawing.Size(73, 22)
        Me.MirrorButton.TabIndex = 5
        Me.MirrorButton.Text = "Mirror level"
        Me.MirrorButton.UseVisualStyleBackColor = True
        '
        'SavePicButton
        '
        Me.SavePicButton.Location = New System.Drawing.Point(184, 14)
        Me.SavePicButton.Name = "SavePicButton"
        Me.SavePicButton.Size = New System.Drawing.Size(83, 22)
        Me.SavePicButton.TabIndex = 4
        Me.SavePicButton.Text = "Save picture"
        Me.SavePicButton.UseVisualStyleBackColor = True
        '
        'ZoomFillButton
        '
        Me.ZoomFillButton.Location = New System.Drawing.Point(6, 41)
        Me.ZoomFillButton.Name = "ZoomFillButton"
        Me.ZoomFillButton.Size = New System.Drawing.Size(70, 22)
        Me.ZoomFillButton.TabIndex = 3
        Me.ZoomFillButton.Text = "Zoom fill"
        Me.ZoomFillButton.UseVisualStyleBackColor = True
        '
        'ViewerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = Global.LevelManager.My.MySettings.Default.ViewerSize
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("ClientSize", Global.LevelManager.My.MySettings.Default, "ViewerSize", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(288, 292)
        Me.Name = "ViewerForm"
        Me.ShowIcon = False
        Me.Text = "Level viewer"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ZoomFillButton As System.Windows.Forms.Button
    Friend WithEvents SavePicButton As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents SaveLevButton As System.Windows.Forms.Button
    Friend WithEvents MirrorButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
End Class
