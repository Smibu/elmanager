<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
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
        Me.components = New System.ComponentModel.Container
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BrowseForLevelFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.BrowseForReplayFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteToRecycleBinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ResetSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.InvertSelectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ClearTop10ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RemoveFromListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CopyToToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveToToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LevelList = New System.Windows.Forms.ListBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.DeleteButton = New System.Windows.Forms.Button
        Me.SearchButton = New System.Windows.Forms.Button
        Me.DuplicateButton = New System.Windows.Forms.Button
        Me.ViewerButton = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.CheckBox8 = New System.Windows.Forms.CheckBox
        Me.CheckBox7 = New System.Windows.Forms.CheckBox
        Me.GroupBox5 = New System.Windows.Forms.GroupBox
        Me.DateModifiedButton = New System.Windows.Forms.RadioButton
        Me.DateCreatedButton = New System.Windows.Forms.RadioButton
        Me.AlphabetButton = New System.Windows.Forms.RadioButton
        Me.MaskedTextBox4 = New System.Windows.Forms.MaskedTextBox
        Me.MaskedTextBox3 = New System.Windows.Forms.MaskedTextBox
        Me.MaskedTextBox2 = New System.Windows.Forms.MaskedTextBox
        Me.MaskedTextBox1 = New System.Windows.Forms.MaskedTextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.ResetButton = New System.Windows.Forms.Button
        Me.CheckBox6 = New System.Windows.Forms.CheckBox
        Me.CheckBox5 = New System.Windows.Forms.CheckBox
        Me.TextBox7 = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.TextBox6 = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.ListBox2 = New System.Windows.Forms.ListBox
        Me.TextBox9 = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.TextBox8 = New System.Windows.Forms.TextBox
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.TextBox5 = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.CheckBox4 = New System.Windows.Forms.CheckBox
        Me.CheckBox3 = New System.Windows.Forms.CheckBox
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.ExactCheckBox = New System.Windows.Forms.CheckBox
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.BothReplaysButton = New System.Windows.Forms.RadioButton
        Me.NoReplaysButton = New System.Windows.Forms.RadioButton
        Me.ReplaysButton = New System.Windows.Forms.RadioButton
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.BothLevelsButton = New System.Windows.Forms.RadioButton
        Me.AcrossButton = New System.Windows.Forms.RadioButton
        Me.ElmaButton = New System.Windows.Forms.RadioButton
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.MultiTop10List = New System.Windows.Forms.ListBox
        Me.ContextMenuStrip3 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RemoveSelectedTimesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CopyToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SingleTop10List = New System.Windows.Forms.ListBox
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.AcrossLevLabel = New System.Windows.Forms.Label
        Me.SkyLabel = New System.Windows.Forms.Label
        Me.GroundLabel = New System.Windows.Forms.Label
        Me.LGRLabel = New System.Windows.Forms.Label
        Me.PropertiesList = New System.Windows.Forms.ListBox
        Me.TitleLabel = New System.Windows.Forms.Label
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.Label14 = New System.Windows.Forms.Label
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.Button1 = New System.Windows.Forms.Button
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.ContextMenuStrip3.SuspendLayout()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BrowseForLevelFolderToolStripMenuItem, Me.BrowseForReplayFolderToolStripMenuItem, Me.DeleteToRecycleBinToolStripMenuItem, Me.ResetSettingsToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(200, 92)
        '
        'BrowseForLevelFolderToolStripMenuItem
        '
        Me.BrowseForLevelFolderToolStripMenuItem.Name = "BrowseForLevelFolderToolStripMenuItem"
        Me.BrowseForLevelFolderToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.BrowseForLevelFolderToolStripMenuItem.Text = Global.LevelManager.My.MySettings.Default.LevelDirectory
        '
        'BrowseForReplayFolderToolStripMenuItem
        '
        Me.BrowseForReplayFolderToolStripMenuItem.Name = "BrowseForReplayFolderToolStripMenuItem"
        Me.BrowseForReplayFolderToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.BrowseForReplayFolderToolStripMenuItem.Text = Global.LevelManager.My.MySettings.Default.ReplayDirectory
        '
        'DeleteToRecycleBinToolStripMenuItem
        '
        Me.DeleteToRecycleBinToolStripMenuItem.Checked = Global.LevelManager.My.MySettings.Default.DelToRecycle
        Me.DeleteToRecycleBinToolStripMenuItem.CheckOnClick = True
        Me.DeleteToRecycleBinToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.DeleteToRecycleBinToolStripMenuItem.Name = "DeleteToRecycleBinToolStripMenuItem"
        Me.DeleteToRecycleBinToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.DeleteToRecycleBinToolStripMenuItem.Text = "Delete to Recycle bin"
        '
        'ResetSettingsToolStripMenuItem
        '
        Me.ResetSettingsToolStripMenuItem.Name = "ResetSettingsToolStripMenuItem"
        Me.ResetSettingsToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.ResetSettingsToolStripMenuItem.Text = "Reset settings"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectAllToolStripMenuItem, Me.InvertSelectionToolStripMenuItem, Me.ClearTop10ToolStripMenuItem, Me.RemoveFromListToolStripMenuItem, Me.CopyToToolStripMenuItem, Me.MoveToToolStripMenuItem, Me.DeleteToolStripMenuItem})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        Me.ContextMenuStrip2.Size = New System.Drawing.Size(206, 158)
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select all"
        '
        'InvertSelectionToolStripMenuItem
        '
        Me.InvertSelectionToolStripMenuItem.Name = "InvertSelectionToolStripMenuItem"
        Me.InvertSelectionToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.InvertSelectionToolStripMenuItem.Text = "Invert selection"
        '
        'ClearTop10ToolStripMenuItem
        '
        Me.ClearTop10ToolStripMenuItem.Enabled = False
        Me.ClearTop10ToolStripMenuItem.Name = "ClearTop10ToolStripMenuItem"
        Me.ClearTop10ToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.ClearTop10ToolStripMenuItem.Text = "Clear Top 10"
        '
        'RemoveFromListToolStripMenuItem
        '
        Me.RemoveFromListToolStripMenuItem.Enabled = False
        Me.RemoveFromListToolStripMenuItem.Name = "RemoveFromListToolStripMenuItem"
        Me.RemoveFromListToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.RemoveFromListToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.RemoveFromListToolStripMenuItem.Text = "Remove from list"
        '
        'CopyToToolStripMenuItem
        '
        Me.CopyToToolStripMenuItem.Enabled = False
        Me.CopyToToolStripMenuItem.Name = "CopyToToolStripMenuItem"
        Me.CopyToToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.CopyToToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.CopyToToolStripMenuItem.Text = "Copy to..."
        '
        'MoveToToolStripMenuItem
        '
        Me.MoveToToolStripMenuItem.Enabled = False
        Me.MoveToToolStripMenuItem.Name = "MoveToToolStripMenuItem"
        Me.MoveToToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.MoveToToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.MoveToToolStripMenuItem.Text = "Move to..."
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.DeleteToolStripMenuItem.Text = "Delete"
        Me.DeleteToolStripMenuItem.Visible = False
        '
        'LevelList
        '
        Me.LevelList.ContextMenuStrip = Me.ContextMenuStrip2
        Me.LevelList.FormattingEnabled = True
        Me.LevelList.Location = New System.Drawing.Point(12, 12)
        Me.LevelList.Name = "LevelList"
        Me.LevelList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.LevelList.Size = New System.Drawing.Size(357, 108)
        Me.LevelList.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 123)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(10, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = " "
        '
        'DeleteButton
        '
        Me.DeleteButton.Enabled = False
        Me.DeleteButton.Location = New System.Drawing.Point(264, 128)
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.Size = New System.Drawing.Size(105, 22)
        Me.DeleteButton.TabIndex = 8
        Me.DeleteButton.Text = "Delete level(s)"
        Me.DeleteButton.UseVisualStyleBackColor = True
        '
        'SearchButton
        '
        Me.SearchButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SearchButton.Location = New System.Drawing.Point(12, 156)
        Me.SearchButton.Name = "SearchButton"
        Me.SearchButton.Size = New System.Drawing.Size(93, 22)
        Me.SearchButton.TabIndex = 9
        Me.SearchButton.Text = "Search"
        Me.SearchButton.UseVisualStyleBackColor = True
        '
        'DuplicateButton
        '
        Me.DuplicateButton.Location = New System.Drawing.Point(123, 156)
        Me.DuplicateButton.Name = "DuplicateButton"
        Me.DuplicateButton.Size = New System.Drawing.Size(123, 22)
        Me.DuplicateButton.TabIndex = 10
        Me.DuplicateButton.Text = "Duplicate level search"
        Me.DuplicateButton.UseVisualStyleBackColor = True
        '
        'ViewerButton
        '
        Me.ViewerButton.Enabled = False
        Me.ViewerButton.Location = New System.Drawing.Point(264, 156)
        Me.ViewerButton.Name = "ViewerButton"
        Me.ViewerButton.Size = New System.Drawing.Size(105, 22)
        Me.ViewerButton.TabIndex = 11
        Me.ViewerButton.Text = "Open level viewer"
        Me.ViewerButton.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Controls.Add(Me.CheckBox8)
        Me.GroupBox1.Controls.Add(Me.CheckBox7)
        Me.GroupBox1.Controls.Add(Me.GroupBox5)
        Me.GroupBox1.Controls.Add(Me.MaskedTextBox4)
        Me.GroupBox1.Controls.Add(Me.MaskedTextBox3)
        Me.GroupBox1.Controls.Add(Me.MaskedTextBox2)
        Me.GroupBox1.Controls.Add(Me.MaskedTextBox1)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.ResetButton)
        Me.GroupBox1.Controls.Add(Me.CheckBox6)
        Me.GroupBox1.Controls.Add(Me.CheckBox5)
        Me.GroupBox1.Controls.Add(Me.TextBox7)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.TextBox6)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.ListBox2)
        Me.GroupBox1.Controls.Add(Me.TextBox9)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.TextBox8)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Controls.Add(Me.TextBox5)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.CheckBox4)
        Me.GroupBox1.Controls.Add(Me.CheckBox3)
        Me.GroupBox1.Controls.Add(Me.TextBox4)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.TextBox3)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.ExactCheckBox)
        Me.GroupBox1.Controls.Add(Me.TextBox2)
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.TextBox1)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(375, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(362, 532)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Search options"
        '
        'CheckBox8
        '
        Me.CheckBox8.AutoSize = True
        Me.CheckBox8.Location = New System.Drawing.Point(140, 36)
        Me.CheckBox8.Name = "CheckBox8"
        Me.CheckBox8.Size = New System.Drawing.Size(213, 17)
        Me.CheckBox8.TabIndex = 44
        Me.CheckBox8.Text = "Search subdirectories in replay directory"
        Me.CheckBox8.UseVisualStyleBackColor = True
        '
        'CheckBox7
        '
        Me.CheckBox7.AutoSize = True
        Me.CheckBox7.Location = New System.Drawing.Point(140, 13)
        Me.CheckBox7.Name = "CheckBox7"
        Me.CheckBox7.Size = New System.Drawing.Size(207, 17)
        Me.CheckBox7.TabIndex = 43
        Me.CheckBox7.Text = "Search subdirectories in level directory"
        Me.CheckBox7.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.DateModifiedButton)
        Me.GroupBox5.Controls.Add(Me.DateCreatedButton)
        Me.GroupBox5.Controls.Add(Me.AlphabetButton)
        Me.GroupBox5.Location = New System.Drawing.Point(9, 482)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(347, 41)
        Me.GroupBox5.TabIndex = 42
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Sort level list by:"
        '
        'DateModifiedButton
        '
        Me.DateModifiedButton.AutoSize = True
        Me.DateModifiedButton.Location = New System.Drawing.Point(219, 15)
        Me.DateModifiedButton.Name = "DateModifiedButton"
        Me.DateModifiedButton.Size = New System.Drawing.Size(90, 17)
        Me.DateModifiedButton.TabIndex = 2
        Me.DateModifiedButton.Text = "Date modified"
        Me.DateModifiedButton.UseVisualStyleBackColor = True
        '
        'DateCreatedButton
        '
        Me.DateCreatedButton.AutoSize = True
        Me.DateCreatedButton.Location = New System.Drawing.Point(102, 15)
        Me.DateCreatedButton.Name = "DateCreatedButton"
        Me.DateCreatedButton.Size = New System.Drawing.Size(87, 17)
        Me.DateCreatedButton.TabIndex = 1
        Me.DateCreatedButton.Text = "Date created"
        Me.DateCreatedButton.UseVisualStyleBackColor = True
        '
        'AlphabetButton
        '
        Me.AlphabetButton.AutoSize = True
        Me.AlphabetButton.Checked = True
        Me.AlphabetButton.Location = New System.Drawing.Point(10, 15)
        Me.AlphabetButton.Name = "AlphabetButton"
        Me.AlphabetButton.Size = New System.Drawing.Size(67, 17)
        Me.AlphabetButton.TabIndex = 0
        Me.AlphabetButton.TabStop = True
        Me.AlphabetButton.Text = "Alphabet"
        Me.AlphabetButton.UseVisualStyleBackColor = True
        '
        'MaskedTextBox4
        '
        Me.MaskedTextBox4.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
        Me.MaskedTextBox4.Location = New System.Drawing.Point(228, 444)
        Me.MaskedTextBox4.Mask = "00:00:00"
        Me.MaskedTextBox4.Name = "MaskedTextBox4"
        Me.MaskedTextBox4.Size = New System.Drawing.Size(51, 20)
        Me.MaskedTextBox4.TabIndex = 41
        Me.MaskedTextBox4.Text = "600000"
        '
        'MaskedTextBox3
        '
        Me.MaskedTextBox3.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
        Me.MaskedTextBox3.Location = New System.Drawing.Point(140, 444)
        Me.MaskedTextBox3.Mask = "00:00:00"
        Me.MaskedTextBox3.Name = "MaskedTextBox3"
        Me.MaskedTextBox3.Size = New System.Drawing.Size(51, 20)
        Me.MaskedTextBox3.TabIndex = 40
        Me.MaskedTextBox3.Text = "000000"
        '
        'MaskedTextBox2
        '
        Me.MaskedTextBox2.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
        Me.MaskedTextBox2.Location = New System.Drawing.Point(228, 418)
        Me.MaskedTextBox2.Mask = "00:00:00"
        Me.MaskedTextBox2.Name = "MaskedTextBox2"
        Me.MaskedTextBox2.Size = New System.Drawing.Size(51, 20)
        Me.MaskedTextBox2.TabIndex = 39
        Me.MaskedTextBox2.Text = "600000"
        '
        'MaskedTextBox1
        '
        Me.MaskedTextBox1.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
        Me.MaskedTextBox1.Location = New System.Drawing.Point(140, 418)
        Me.MaskedTextBox1.Mask = "00:00:00"
        Me.MaskedTextBox1.Name = "MaskedTextBox1"
        Me.MaskedTextBox1.Size = New System.Drawing.Size(51, 20)
        Me.MaskedTextBox1.TabIndex = 15
        Me.MaskedTextBox1.Text = "000000"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(201, 447)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(16, 13)
        Me.Label13.TabIndex = 38
        Me.Label13.Text = "to"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(201, 421)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(16, 13)
        Me.Label12.TabIndex = 37
        Me.Label12.Text = "to"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(29, 447)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(105, 13)
        Me.Label11.TabIndex = 14
        Me.Label11.Text = "Multiplayer best time:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(22, 421)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(112, 13)
        Me.Label9.TabIndex = 34
        Me.Label9.Text = "Singleplayer best time:"
        '
        'ResetButton
        '
        Me.ResetButton.Location = New System.Drawing.Point(264, 338)
        Me.ResetButton.Name = "ResetButton"
        Me.ResetButton.Size = New System.Drawing.Size(92, 22)
        Me.ResetButton.TabIndex = 33
        Me.ResetButton.Text = "Reset defaults"
        Me.ResetButton.UseVisualStyleBackColor = True
        '
        'CheckBox6
        '
        Me.CheckBox6.AutoSize = True
        Me.CheckBox6.Location = New System.Drawing.Point(284, 368)
        Me.CheckBox6.Name = "CheckBox6"
        Me.CheckBox6.Size = New System.Drawing.Size(72, 17)
        Me.CheckBox6.TabIndex = 32
        Me.CheckBox6.Text = "Not these"
        Me.CheckBox6.UseVisualStyleBackColor = True
        '
        'CheckBox5
        '
        Me.CheckBox5.AutoSize = True
        Me.CheckBox5.Location = New System.Drawing.Point(284, 394)
        Me.CheckBox5.Name = "CheckBox5"
        Me.CheckBox5.Size = New System.Drawing.Size(72, 17)
        Me.CheckBox5.TabIndex = 31
        Me.CheckBox5.Text = "Not these"
        Me.CheckBox5.UseVisualStyleBackColor = True
        '
        'TextBox7
        '
        Me.TextBox7.Location = New System.Drawing.Point(140, 392)
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.Size = New System.Drawing.Size(139, 20)
        Me.TextBox7.TabIndex = 30
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(13, 395)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(121, 13)
        Me.Label8.TabIndex = 29
        Me.Label8.Text = "Multiplayer top 10 nicks:"
        '
        'TextBox6
        '
        Me.TextBox6.Location = New System.Drawing.Point(140, 366)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(139, 20)
        Me.TextBox6.TabIndex = 28
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 369)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(128, 13)
        Me.Label7.TabIndex = 27
        Me.Label7.Text = "Singleplayer top 10 nicks:"
        '
        'ListBox2
        '
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.Items.AddRange(New Object() {"Ground polygons: 1-10000", "Grass polygons: 0-10000", "Ground vertices: 3-10000", "Grass vertices: 0-10000", "Apples: 0-10000", "Gravity up apples: 0-10000", "Gravity down apples: 0-10000", "Gravity left apples: 0-10000", "Gravity right apples: 0-10000", "Killers: 0-10000", "Flowers: 0-10000", "Pictures: 0-10000", "Textures: 0-10000", "Times in singleplayer top 10: 0-10", "Times in multiplayer top 10: 0-10"})
        Me.ListBox2.Location = New System.Drawing.Point(9, 252)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.Size = New System.Drawing.Size(208, 108)
        Me.ListBox2.TabIndex = 26
        '
        'TextBox9
        '
        Me.TextBox9.Location = New System.Drawing.Point(309, 252)
        Me.TextBox9.Name = "TextBox9"
        Me.TextBox9.Size = New System.Drawing.Size(47, 20)
        Me.TextBox9.TabIndex = 22
        Me.TextBox9.Text = "10000"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(282, 255)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(16, 13)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "to"
        '
        'TextBox8
        '
        Me.TextBox8.Location = New System.Drawing.Point(225, 252)
        Me.TextBox8.Name = "TextBox8"
        Me.TextBox8.Size = New System.Drawing.Size(47, 20)
        Me.TextBox8.TabIndex = 20
        Me.TextBox8.Text = "1"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(285, 228)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(72, 17)
        Me.CheckBox1.TabIndex = 16
        Me.CheckBox1.Text = "Not these"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(144, 226)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(135, 20)
        Me.TextBox5.TabIndex = 15
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(23, 229)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(115, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Name(s) of sky texture:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(108, 151)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(30, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "Title:"
        '
        'CheckBox4
        '
        Me.CheckBox4.AutoSize = True
        Me.CheckBox4.Location = New System.Drawing.Point(285, 202)
        Me.CheckBox4.Name = "CheckBox4"
        Me.CheckBox4.Size = New System.Drawing.Size(72, 17)
        Me.CheckBox4.TabIndex = 13
        Me.CheckBox4.Text = "Not these"
        Me.CheckBox4.UseVisualStyleBackColor = True
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Location = New System.Drawing.Point(285, 176)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(72, 17)
        Me.CheckBox3.TabIndex = 12
        Me.CheckBox3.Text = "Not these"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(144, 200)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(135, 20)
        Me.TextBox4.TabIndex = 11
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 203)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(132, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Name(s) of ground texture:"
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(144, 174)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(135, 20)
        Me.TextBox3.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(52, 177)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(86, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Name(s) of LGR:"
        '
        'ExactCheckBox
        '
        Me.ExactCheckBox.AutoSize = True
        Me.ExactCheckBox.Location = New System.Drawing.Point(285, 150)
        Me.ExactCheckBox.Name = "ExactCheckBox"
        Me.ExactCheckBox.Size = New System.Drawing.Size(53, 17)
        Me.ExactCheckBox.TabIndex = 7
        Me.ExactCheckBox.Text = "Exact"
        Me.ExactCheckBox.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(144, 148)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(135, 20)
        Me.TextBox2.TabIndex = 5
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.BothReplaysButton)
        Me.GroupBox3.Controls.Add(Me.NoReplaysButton)
        Me.GroupBox3.Controls.Add(Me.ReplaysButton)
        Me.GroupBox3.Location = New System.Drawing.Point(9, 99)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(347, 41)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        '
        'BothReplaysButton
        '
        Me.BothReplaysButton.AutoSize = True
        Me.BothReplaysButton.Checked = True
        Me.BothReplaysButton.Location = New System.Drawing.Point(183, 15)
        Me.BothReplaysButton.Name = "BothReplaysButton"
        Me.BothReplaysButton.Size = New System.Drawing.Size(47, 17)
        Me.BothReplaysButton.TabIndex = 2
        Me.BothReplaysButton.TabStop = True
        Me.BothReplaysButton.Text = "Both"
        Me.BothReplaysButton.UseVisualStyleBackColor = True
        '
        'NoReplaysButton
        '
        Me.NoReplaysButton.AutoSize = True
        Me.NoReplaysButton.Location = New System.Drawing.Point(90, 15)
        Me.NoReplaysButton.Name = "NoReplaysButton"
        Me.NoReplaysButton.Size = New System.Drawing.Size(75, 17)
        Me.NoReplaysButton.TabIndex = 1
        Me.NoReplaysButton.TabStop = True
        Me.NoReplaysButton.Text = "No replays"
        Me.NoReplaysButton.UseVisualStyleBackColor = True
        '
        'ReplaysButton
        '
        Me.ReplaysButton.AutoSize = True
        Me.ReplaysButton.Location = New System.Drawing.Point(6, 15)
        Me.ReplaysButton.Name = "ReplaysButton"
        Me.ReplaysButton.Size = New System.Drawing.Size(63, 17)
        Me.ReplaysButton.TabIndex = 0
        Me.ReplaysButton.TabStop = True
        Me.ReplaysButton.Text = "Replays"
        Me.ReplaysButton.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BothLevelsButton)
        Me.GroupBox2.Controls.Add(Me.AcrossButton)
        Me.GroupBox2.Controls.Add(Me.ElmaButton)
        Me.GroupBox2.Location = New System.Drawing.Point(9, 52)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(347, 41)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        '
        'BothLevelsButton
        '
        Me.BothLevelsButton.AutoSize = True
        Me.BothLevelsButton.Checked = True
        Me.BothLevelsButton.Location = New System.Drawing.Point(183, 15)
        Me.BothLevelsButton.Name = "BothLevelsButton"
        Me.BothLevelsButton.Size = New System.Drawing.Size(47, 17)
        Me.BothLevelsButton.TabIndex = 2
        Me.BothLevelsButton.TabStop = True
        Me.BothLevelsButton.Text = "Both"
        Me.BothLevelsButton.UseVisualStyleBackColor = True
        '
        'AcrossButton
        '
        Me.AcrossButton.AutoSize = True
        Me.AcrossButton.Location = New System.Drawing.Point(90, 15)
        Me.AcrossButton.Name = "AcrossButton"
        Me.AcrossButton.Size = New System.Drawing.Size(87, 17)
        Me.AcrossButton.TabIndex = 1
        Me.AcrossButton.Text = "Across levels"
        Me.AcrossButton.UseVisualStyleBackColor = True
        '
        'ElmaButton
        '
        Me.ElmaButton.AutoSize = True
        Me.ElmaButton.Location = New System.Drawing.Point(6, 15)
        Me.ElmaButton.Name = "ElmaButton"
        Me.ElmaButton.Size = New System.Drawing.Size(78, 17)
        Me.ElmaButton.TabIndex = 0
        Me.ElmaButton.Text = "Elma levels"
        Me.ElmaButton.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.LevelManager.My.MySettings.Default, "SearchPattern", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TextBox1.Location = New System.Drawing.Point(85, 26)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(53, 20)
        Me.TextBox1.TabIndex = 1
        Me.TextBox1.Text = Global.LevelManager.My.MySettings.Default.SearchPattern
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Search pattern:"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.MultiTop10List)
        Me.GroupBox4.Controls.Add(Me.SingleTop10List)
        Me.GroupBox4.Controls.Add(Me.Label20)
        Me.GroupBox4.Controls.Add(Me.Label19)
        Me.GroupBox4.Controls.Add(Me.AcrossLevLabel)
        Me.GroupBox4.Controls.Add(Me.SkyLabel)
        Me.GroupBox4.Controls.Add(Me.GroundLabel)
        Me.GroupBox4.Controls.Add(Me.LGRLabel)
        Me.GroupBox4.Controls.Add(Me.PropertiesList)
        Me.GroupBox4.Controls.Add(Me.TitleLabel)
        Me.GroupBox4.Location = New System.Drawing.Point(12, 179)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(357, 358)
        Me.GroupBox4.TabIndex = 14
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Level properties"
        '
        'MultiTop10List
        '
        Me.MultiTop10List.ContextMenuStrip = Me.ContextMenuStrip3
        Me.MultiTop10List.FormattingEnabled = True
        Me.MultiTop10List.Location = New System.Drawing.Point(182, 215)
        Me.MultiTop10List.Name = "MultiTop10List"
        Me.MultiTop10List.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.MultiTop10List.Size = New System.Drawing.Size(164, 134)
        Me.MultiTop10List.TabIndex = 9
        '
        'ContextMenuStrip3
        '
        Me.ContextMenuStrip3.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveSelectedTimesToolStripMenuItem, Me.CopyToClipboardToolStripMenuItem})
        Me.ContextMenuStrip3.Name = "ContextMenuStrip3"
        Me.ContextMenuStrip3.Size = New System.Drawing.Size(196, 48)
        '
        'RemoveSelectedTimesToolStripMenuItem
        '
        Me.RemoveSelectedTimesToolStripMenuItem.Enabled = False
        Me.RemoveSelectedTimesToolStripMenuItem.Name = "RemoveSelectedTimesToolStripMenuItem"
        Me.RemoveSelectedTimesToolStripMenuItem.Size = New System.Drawing.Size(195, 22)
        Me.RemoveSelectedTimesToolStripMenuItem.Text = "Remove selected times"
        '
        'CopyToClipboardToolStripMenuItem
        '
        Me.CopyToClipboardToolStripMenuItem.Name = "CopyToClipboardToolStripMenuItem"
        Me.CopyToClipboardToolStripMenuItem.Size = New System.Drawing.Size(195, 22)
        Me.CopyToClipboardToolStripMenuItem.Text = "Copy list to clipboard"
        '
        'SingleTop10List
        '
        Me.SingleTop10List.ContextMenuStrip = Me.ContextMenuStrip3
        Me.SingleTop10List.FormattingEnabled = True
        Me.SingleTop10List.Location = New System.Drawing.Point(9, 215)
        Me.SingleTop10List.Name = "SingleTop10List"
        Me.SingleTop10List.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.SingleTop10List.Size = New System.Drawing.Size(164, 134)
        Me.SingleTop10List.TabIndex = 8
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(179, 195)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(93, 13)
        Me.Label20.TabIndex = 7
        Me.Label20.Text = "Multiplayer top 10:"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(6, 195)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(100, 13)
        Me.Label19.TabIndex = 6
        Me.Label19.Text = "Singleplayer top 10:"
        '
        'AcrossLevLabel
        '
        Me.AcrossLevLabel.AutoSize = True
        Me.AcrossLevLabel.Location = New System.Drawing.Point(179, 124)
        Me.AcrossLevLabel.Name = "AcrossLevLabel"
        Me.AcrossLevLabel.Size = New System.Drawing.Size(64, 13)
        Me.AcrossLevLabel.TabIndex = 5
        Me.AcrossLevLabel.Text = "Across level"
        Me.AcrossLevLabel.Visible = False
        '
        'SkyLabel
        '
        Me.SkyLabel.AutoSize = True
        Me.SkyLabel.Location = New System.Drawing.Point(179, 102)
        Me.SkyLabel.Name = "SkyLabel"
        Me.SkyLabel.Size = New System.Drawing.Size(63, 13)
        Me.SkyLabel.TabIndex = 4
        Me.SkyLabel.Text = "Sky texture:"
        Me.SkyLabel.Visible = False
        '
        'GroundLabel
        '
        Me.GroundLabel.AutoSize = True
        Me.GroundLabel.Location = New System.Drawing.Point(179, 85)
        Me.GroundLabel.Name = "GroundLabel"
        Me.GroundLabel.Size = New System.Drawing.Size(80, 13)
        Me.GroundLabel.TabIndex = 3
        Me.GroundLabel.Text = "Ground texture:"
        Me.GroundLabel.Visible = False
        '
        'LGRLabel
        '
        Me.LGRLabel.AutoSize = True
        Me.LGRLabel.Location = New System.Drawing.Point(179, 68)
        Me.LGRLabel.Name = "LGRLabel"
        Me.LGRLabel.Size = New System.Drawing.Size(32, 13)
        Me.LGRLabel.TabIndex = 2
        Me.LGRLabel.Text = "LGR:"
        Me.LGRLabel.Visible = False
        '
        'PropertiesList
        '
        Me.PropertiesList.FormattingEnabled = True
        Me.PropertiesList.Items.AddRange(New Object() {"", "", "", "", "", "", "", "", "", "", "", "", ""})
        Me.PropertiesList.Location = New System.Drawing.Point(9, 19)
        Me.PropertiesList.Name = "PropertiesList"
        Me.PropertiesList.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.PropertiesList.Size = New System.Drawing.Size(164, 173)
        Me.PropertiesList.TabIndex = 1
        '
        'TitleLabel
        '
        Me.TitleLabel.Location = New System.Drawing.Point(179, 19)
        Me.TitleLabel.Name = "TitleLabel"
        Me.TitleLabel.Size = New System.Drawing.Size(167, 49)
        Me.TitleLabel.TabIndex = 0
        Me.TitleLabel.Text = "Title:"
        Me.TitleLabel.Visible = False
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(9, 140)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(169, 13)
        Me.Label14.TabIndex = 15
        Me.Label14.Text = "Single total time of selected levels:"
        Me.Label14.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(240, 298)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(98, 24)
        Me.Button1.TabIndex = 45
        Me.Button1.Text = "DecipherState"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(745, 544)
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.LevelList)
        Me.Controls.Add(Me.ViewerButton)
        Me.Controls.Add(Me.SearchButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.DeleteButton)
        Me.Controls.Add(Me.DuplicateButton)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.LevelManager.My.MySettings.Default, "MyLocation", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.ShowIcon = False
        Me.Text = "Level manager for Elasto Mania by Smibu"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.ContextMenuStrip3.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents BrowseForLevelFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BrowseForReplayFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteToRecycleBinToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetSettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip2 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ClearTop10ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveFromListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyToToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveToToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LevelList As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DeleteButton As System.Windows.Forms.Button
    Friend WithEvents SearchButton As System.Windows.Forms.Button
    Friend WithEvents DuplicateButton As System.Windows.Forms.Button
    Friend WithEvents ViewerButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents NoReplaysButton As System.Windows.Forms.RadioButton
    Friend WithEvents ReplaysButton As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents BothLevelsButton As System.Windows.Forms.RadioButton
    Friend WithEvents AcrossButton As System.Windows.Forms.RadioButton
    Friend WithEvents ElmaButton As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents BothReplaysButton As System.Windows.Forms.RadioButton
    Friend WithEvents ExactCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CheckBox4 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TextBox9 As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TextBox8 As System.Windows.Forms.TextBox
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents TextBox7 As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents CheckBox6 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox5 As System.Windows.Forms.CheckBox
    Friend WithEvents ResetButton As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents PropertiesList As System.Windows.Forms.ListBox
    Friend WithEvents TitleLabel As System.Windows.Forms.Label
    Friend WithEvents SkyLabel As System.Windows.Forms.Label
    Friend WithEvents GroundLabel As System.Windows.Forms.Label
    Friend WithEvents LGRLabel As System.Windows.Forms.Label
    Friend WithEvents AcrossLevLabel As System.Windows.Forms.Label
    Friend WithEvents MaskedTextBox1 As System.Windows.Forms.MaskedTextBox
    Friend WithEvents MaskedTextBox4 As System.Windows.Forms.MaskedTextBox
    Friend WithEvents MaskedTextBox3 As System.Windows.Forms.MaskedTextBox
    Friend WithEvents MaskedTextBox2 As System.Windows.Forms.MaskedTextBox
    Friend WithEvents MultiTop10List As System.Windows.Forms.ListBox
    Friend WithEvents SingleTop10List As System.Windows.Forms.ListBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents SelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvertSelectionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents DateModifiedButton As System.Windows.Forms.RadioButton
    Friend WithEvents DateCreatedButton As System.Windows.Forms.RadioButton
    Friend WithEvents AlphabetButton As System.Windows.Forms.RadioButton
    Friend WithEvents CheckBox8 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox7 As System.Windows.Forms.CheckBox
    Friend WithEvents ContextMenuStrip3 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents RemoveSelectedTimesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Button1 As System.Windows.Forms.Button

End Class
