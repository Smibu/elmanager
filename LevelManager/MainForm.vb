Public Class MainForm
    Declare Function GetAsyncKeyState Lib "user32" (ByVal vkey As Long) As Integer
    Public Level As Byte()
    Dim Replay As Byte()
    'Dim Top10(687) As Byte
    'Dim State As Byte()
    'Dim DecipheredState As Byte()
    Dim TempBytes, TempBytes2, TempBytes3 As Byte()
    Public Int, Int2, Int3 As Integer 'Used to store temp stuff
    Dim OldSelect As Integer = 0
    Dim ListBoxStrings As String() = {"Ground polygons: ", "Grass polygons: ", "Ground vertices: ", "Grass vertices: ", "Apples: ", "Gravity up apples: ", "Gravity down apples: ", "Gravity left apples: ", "Gravity right apples: ", "Killers: ", "Flowers: ", "Pictures: ", "Textures: ", "Times in singleplayer top 10: ", "Times in multiplayer top 10: "}
    Dim ChangeEventDisabled As Boolean
    Dim LevelFiles As String()
    Dim ReplayFiles As String()
    Dim ReplayLevelFiles As String()
    Dim LevelFileNames As String()
    Dim RecDirectory As String
    Public LevDirectory As String
    Dim SearchPattern As String
    Dim Dates As Date()
    Dim RealSearchInProgress As Boolean = False
    Dim DuplicateSearch As Boolean = False
    Dim MoveClicked, SingleListClicked As Boolean
    Public Top10Offset As Integer
    Dim LevelLengths As Integer()
    'Public ErrorLevTop10Offsets As Integer()
    Dim LevelTimes(1)() As Integer
    Dim SingleTotalTimeShown As Boolean
    Dim TotalTime As Integer
    'Level properties:
    Public IsAcrossLevel As Boolean
    Dim LevelFileName As String
    Dim LevelTitle As String
    Dim LevelLGRName As String
    Dim LevelGroundTexture As String
    Dim LevelSkyTexture As String
    Dim SingleTop10Names() As String
    Dim MultiTop10Names() As String
    Dim SingleTop10Times() As Integer
    Dim MultiTop10Times() As Integer
    Dim LevelAmounts(14) As Integer 'Number of ground/grass polygons, vertices, apples etc...
    Dim PictureNames As String()
    Dim TextureNames As String()
    Dim InvalidTop10 As Boolean
    Public PSUM, OSUM, PICSUM, SUM As Double
    Dim WrongLevelLength As Boolean
    'Temporary variables:
    Public NumPolygons As Integer
    Dim IsGrassPoly As Boolean
    Public NumObjects As Integer
    Public NumPictures As Integer
    Dim SingleTop10HiddenNames() As String
    Dim SignedAreas As Single()
    'Search parametres:
    Dim SearchTitle As String
    Dim SearchLGRs As String()
    Dim SearchGroundTextures As String()
    Dim SearchSkyTextures As String()
    Dim SearchAmountsLow As Integer()
    Dim SearchAmountsHigh As Integer()
    Dim SearchSingleTop10Names As String()
    Dim SearchMultiTop10Names As String()
    Dim SearchSingleBestTime(1) As Integer 'Lower and upper bound
    Dim SearchMultiBestTime(1) As Integer 'Lower and upper bound
    'Temporary variables:
    Dim TempStrings As String()
    Dim TempString As String
    Dim AmountsSatisfied As Boolean
    Dim ReplayExists As Boolean
    Dim Found1, Found2, Found3 As Boolean
    'Level viewer variables:
    Dim PlayingLevelPolygons As PointF()()
    Public x, y, xmin2, ymin2, xmax2, ymax2, Lastx, ZoomFillxmax, ZoomFillxmin, ZoomFillymax, ZoomFillymin, xmax, xmin, ymin, ymax, midx, midy, s As Single
    Dim PolygonDrawingOrder As Integer()
    Dim PlayingLevelObjectTypes As Integer()
    Dim GroundPolygons, NumVertice, MaxClipping As Integer
    Dim PlayingLevelObjects As RectangleF()
    Dim PolygonClippings As Integer()
    Public IsInside, UseDifferentMatrices, ZoomRecting As Boolean
    Public ChangeMatrices As Boolean = False
    Public ZoomLevel As Single
    Public NewCenter As PointF()
    Public ZoomFillLevelMatrix As Drawing2D.Matrix
    Public LevelMatrix As Drawing2D.Matrix
    Public g As Graphics
    Dim BlackPen As New Pen(Color.Black, 1)
    Dim RedPen As New Pen(Color.Red, 1)
    Dim WhitePen As New Pen(Color.White, 1)
    Dim GreenPen As New Pen(Color.Green, 1)
    Dim YellowPen As New Pen(Color.Yellow, 1)
    Dim Remainder As Integer
    Public ZoomRectStartPoint, ZoomRectEndPoint As PointF
    Sub StartProgram() Handles Me.Load
        My.Forms.m_MainForm = Me
        DeleteToRecycleBinToolStripMenuItem.Checked = My.Settings.DelToRecycle
        SingleTotalTimeShown = My.Settings.ShowSingleTotal
    End Sub
    Sub SaveSettings() Handles Me.FormClosing
        My.Settings.ReplayDirectory = BrowseForReplayFolderToolStripMenuItem.Text
        My.Settings.LevelDirectory = BrowseForLevelFolderToolStripMenuItem.Text
        My.Settings.DelToRecycle = DeleteToRecycleBinToolStripMenuItem.Checked
        My.Settings.ShowSingleTotal = SingleTotalTimeShown
        My.Settings.Save()
    End Sub
    Sub ResetBounds(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Shown, ResetButton.Click
        SearchAmountsLow = New Integer() {1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        SearchAmountsHigh = New Integer() {10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10, 10}
        ChangeEventDisabled = True
        If sender.Equals(ResetButton) Then
            For i = 0 To 14
                ListBox2.Items.Item(i) = ListBoxStrings(i) & SearchAmountsLow(i).ToString & "-" & SearchAmountsHigh(i).ToString
            Next
        End If
        ListBox2.SelectedIndex = 0
        OldSelect = 0
        TextBox8.Text = "1"
        TextBox9.Text = "10000"
        ChangeEventDisabled = False
    End Sub
    Sub SetupLevelPropertiesBox() Handles Me.Shown
        For i = 0 To 12
            PropertiesList.Items.Item(i) = (ListBoxStrings(i))
        Next
    End Sub
    Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim State, DecipheredState As Byte()
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            State = System.IO.File.ReadAllBytes(OpenFileDialog1.FileName)
            DecipheredState = State
            Dim Remainder As Integer
            For i = 0 To State.Length - 1
                Math.DivRem(i - 67758, 4, Remainder)
                If i = 0 Or i = 4 Or i = 61924 Or i = 67724 Or i = 67728 Or i = 67743 Or (i > 67757 And Remainder = 0 And i < 67794) Or i = 67822 Or (i > 67853 And Remainder = 0 And i < 67870) Or i = 67886 Then
                    Int = 23
                    Int2 = 9782
                End If
                DecipheredState(i) = State(i) Xor BitConverter.GetBytes(Int)(0)
                Dim b1 As Byte = BitConverter.GetBytes(Int)(0)
                Dim b2 As Byte = BitConverter.GetBytes(Int)(1)
                If (BitConverter.GetBytes(Int)(1) And 128) > 0 Then
                    Int = BitConverter.ToInt32(New Byte() {b1, b2, 255, 255}, 0)
                Else
                    Int = BitConverter.ToInt32(New Byte() {b1, b2, 0, 0}, 0)
                End If
                Math.DivRem(Int, 3391, Remainder)
                Int2 = Int2 + Remainder * 3391
                Int = Int2 * 31 + 3391
            Next
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                System.IO.File.WriteAllBytes(SaveFileDialog1.FileName, DecipheredState)
                Application.Exit()
            End If
        End If
    End Sub
    Public Function TimeString(ByVal T As Integer) As String 'Input: time in centiseconds
        Dim Minutes, Hours As Integer
        TimeString = ""
        Hours = CSng(Math.Floor(T / 360000))
        If Hours > 0 Then
            TimeString = Hours & ":"
            T -= 360000 * Hours
        End If
        Minutes = CSng(Math.Floor(T / 6000))
        If Minutes < 10 Then
            TimeString &= "0"
        End If
        TimeString &= Minutes & ":"
        If T - Minutes * 6000 < 1000 Then
            TimeString &= "0"
        End If
        If T - Minutes * 6000 < 100 Then
            TimeString &= "0"
        Else
            TimeString &= Math.Floor((T - Minutes * 6000) / 100)
        End If
        TimeString &= ":"
        If Math.Round((T / 100 - Math.Floor(T / 100)) * 100) < 10 Then
            TimeString &= "0"
        End If
        TimeString &= Math.Round((T / 100 - Math.Floor(T / 100)) * 100)
    End Function
    Function StringToTime(ByVal S As String) As Integer 'Of the form 00:00:00, output: centiseconds
        Return Val(Microsoft.VisualBasic.Left(S, 2)) * 60 * 100 + Val(Microsoft.VisualBasic.Mid(S, 4, 2)) * 100 + Val(Microsoft.VisualBasic.Right(S, 2))
    End Function
    Sub ReadBytesTillZero(ByVal File As Byte(), ByVal Offset As Integer, ByVal MaxAmount As Integer)
        Array.Resize(TempBytes, 0)
        For i = 0 To MaxAmount - 1
            If File(Offset + i) = 0 Then Exit Sub
            Array.Resize(TempBytes, i + 1)
            TempBytes(i) = File(Offset + i)
        Next
    End Sub
    Sub GetLevelProperties(ByVal Level As Byte(), ByVal Decipher As Boolean, ByVal ShortCheck As Boolean)
        Array.Clear(LevelAmounts, 0, 15)
        IsAcrossLevel = (Level(3) = 48)
        If IsAcrossLevel Then
            Int = 41
        Else
            Int = 43
        End If
        ReadBytesTillZero(Level, Int, 51)
        LevelTitle = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
        If Not IsAcrossLevel Then
            ReadBytesTillZero(Level, 94, 16)
            LevelLGRName = LCase(System.Text.ASCIIEncoding.ASCII.GetString(TempBytes))
            ReadBytesTillZero(Level, 110, 10)
            LevelGroundTexture = LCase(System.Text.ASCIIEncoding.ASCII.GetString(TempBytes))
            ReadBytesTillZero(Level, 120, 10)
            LevelSkyTexture = LCase(System.Text.ASCIIEncoding.ASCII.GetString(TempBytes))
            Int = 130
        Else
            Int = 100
        End If
        NumPolygons = Math.Round(BitConverter.ToDouble(Level, Int) - 0.4643643)
        Int += 8
        For i = 1 To NumPolygons
            If Not IsAcrossLevel Then
                IsGrassPoly = (Level(Int) = 1)
                If IsGrassPoly Then
                    LevelAmounts(1) += 1
                Else
                    LevelAmounts(0) += 1
                End If
                Int += 4
            Else
                LevelAmounts(0) += 1
            End If
            Int2 = BitConverter.ToUInt16(Level, Int)
            If IsAcrossLevel Then
                LevelAmounts(2) += Int2
            Else
                If IsGrassPoly Then
                    LevelAmounts(3) += Int2
                Else
                    LevelAmounts(2) += Int2
                End If
            End If
            Int += Int2 * 16 + 4
        Next
        NumObjects = Math.Round(BitConverter.ToDouble(Level, Int) - 0.4643643)
        Int += 8
        For i = 1 To NumObjects
            Int += 16
            Select Case Level(Int)
                Case 1
                    LevelAmounts(10) += 1
                Case 2
                    If IsAcrossLevel Then
                        LevelAmounts(4) += 1
                    Else
                        LevelAmounts(4 + Level(Int + 4)) += 1
                    End If
                Case 3
                    LevelAmounts(9) += 1
            End Select
            Int += 4
            If Not IsAcrossLevel Then Int += 8
        Next
        If Not IsAcrossLevel Then
            NumPictures = Math.Round(BitConverter.ToDouble(Level, Int) - 0.2345672)
            Int += 8
            If NumPictures > 0 Then
                For i = 1 To NumPictures
                    ReadBytesTillZero(Level, Int, 10)
                    If TempBytes.Length > 0 Then
                        LevelAmounts(11) += 1
                    Else
                        LevelAmounts(12) += 1
                    End If
                    Int += 54
                Next
            End If
        End If
        Int += 4
        Top10Offset = Int
        If ShortCheck Then Exit Sub
        InvalidTop10 = False
        Try
            If Decipher Then
                For i = 0 To 687
                    Level(Int + i) = Level(Int + i) Xor My.Resources.emptytop10(i)
                    'Top10(i) = Level(Int + i)
                Next
            End If
            Int2 = Level(Int)
            LevelAmounts(13) = Int2
            Array.Resize(SingleTop10Times, Int2)
            Array.Resize(SingleTop10Names, Int2)
            If Int2 > 0 Then
                For i = 0 To Int2 - 1
                    SingleTop10Times(i) = BitConverter.ToUInt32(Level, Int + 4 + 4 * i)
                    ReadBytesTillZero(Level, Int + 44 + 15 * i, 8)
                    SingleTop10Names(i) = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
                Next
            End If
            Int2 = Level(Int + 344)
            LevelAmounts(14) = Int2
            Array.Resize(MultiTop10Times, Int2)
            Array.Resize(MultiTop10Names, Int2 * 2)
            If Int2 > 0 Then
                For i = 0 To Int2 - 1
                    MultiTop10Times(i) = BitConverter.ToUInt32(Level, Int + 4 + 4 * i + 344)
                    ReadBytesTillZero(Level, Int + 44 + 15 * i + 344, 8)
                    MultiTop10Names(i * 2) = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
                    ReadBytesTillZero(Level, Int + 194 + 15 * i + 344, 8)
                    MultiTop10Names(i * 2 + 1) = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
                Next
            End If
            WrongLevelLength = Level.Length > Top10Offset + 688 + 4
        Catch ex As Exception
            InvalidTop10 = True
            Array.Resize(SingleTop10Times, 0)
            Array.Resize(SingleTop10Names, 0)
            Array.Resize(MultiTop10Times, 0)
            Array.Resize(MultiTop10Names, 0)
            LevelAmounts(13) = 0
            LevelAmounts(14) = 0
        End Try
    End Sub
    Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        If ChangeEventDisabled Then Exit Sub
        SearchAmountsLow(OldSelect) = Val(TextBox8.Text)
        SearchAmountsHigh(OldSelect) = Val(TextBox9.Text)
        ChangeEventDisabled = True
        ListBox2.Items.Item(OldSelect) = ListBoxStrings(OldSelect) & SearchAmountsLow(OldSelect).ToString & "-" & SearchAmountsHigh(OldSelect).ToString
        OldSelect = ListBox2.SelectedIndex
        ChangeEventDisabled = False
        TextBox8.Text = SearchAmountsLow(ListBox2.SelectedIndex).ToString
        TextBox9.Text = SearchAmountsHigh(ListBox2.SelectedIndex).ToString
    End Sub
    Sub GetStringsBetweenCommas(ByVal S As String)
        Array.Resize(TempStrings, 0)
        Dim Last As Integer = 0
        For i = 0 To S.Length - 1
            If S(i) = "," Or i = S.Length - 1 Then
                If i = S.Length - 1 Then
                    Int2 = 1
                Else
                    Int2 = 0
                End If
                Array.Resize(TempStrings, TempStrings.Length + 1)
                TempStrings(TempStrings.Length - 1) = Microsoft.VisualBasic.Mid(S, Last + 1, i - Last + Int2)
                Last = i + 1
            End If
        Next
    End Sub
    Sub SearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchButton.MouseDown
        If DuplicateSearch Then Exit Sub
        If RealSearchInProgress Then
            RealSearchInProgress = False
            Exit Sub
        End If
        RecDirectory = BrowseForReplayFolderToolStripMenuItem.Text & "\"
        LevDirectory = BrowseForLevelFolderToolStripMenuItem.Text & "\"
        If System.IO.Directory.Exists(RecDirectory) And System.IO.Directory.Exists(LevDirectory) Then
            ListBox2_SelectedIndexChanged(sender, e)
            DisableButtons(sender, e)
            GroupBox1.Enabled = False
            ViewerForm.Close()
            SearchButton.Text = "Stop"
            SearchPattern = TextBox1.Text & ".lev"
            Label1.Text = "Initializing search..."
            Application.DoEvents()
            LevelList.Items.Clear()
            ErrorForm.ErrorList.Items.Clear()
            ErrorForm.WrongLengthList.Items.Clear()
            'Array.Resize(ErrorLevTop10Offsets, 0)
            Array.Resize(LevelTimes(0), 0)
            Array.Resize(LevelTimes(1), 0)
            If CheckBox7.Checked Then
                LevelFiles = System.IO.Directory.GetFiles(LevDirectory, SearchPattern, IO.SearchOption.AllDirectories)
            Else
                LevelFiles = System.IO.Directory.GetFiles(LevDirectory, SearchPattern, IO.SearchOption.TopDirectoryOnly)
            End If
            If CheckBox8.Checked Then
                ReplayFiles = System.IO.Directory.GetFiles(RecDirectory, "*.rec", IO.SearchOption.AllDirectories)
            Else
                ReplayFiles = System.IO.Directory.GetFiles(RecDirectory, "*.rec", IO.SearchOption.TopDirectoryOnly)
            End If
            'Remove the pathnames from each element of LevelFiles and convert them to lowercase
            Array.Resize(LevelFileNames, LevelFiles.Length)
            Array.Resize(ReplayLevelFiles, 0)
            For i = 0 To LevelFiles.Length - 1
                LevelFileNames(i) = System.IO.Path.GetFileName(LevelFiles(i))
                LevelFiles(i) = Microsoft.VisualBasic.Mid(LevelFiles(i), Len(LevDirectory) + 1)
            Next
            'Go through replay files and take level file names from them
            For i = 0 To ReplayFiles.Length - 1
                Replay = System.IO.File.ReadAllBytes(ReplayFiles(i))
                Try
                    ReadBytesTillZero(Replay, 20, 12)
                    TempString = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
                    Int2 = Array.IndexOf(ReplayLevelFiles, TempString)
                    If Int2 < 0 Then
                        Array.Resize(ReplayLevelFiles, ReplayLevelFiles.Length + 1)
                        ReplayLevelFiles(ReplayLevelFiles.Length - 1) = TempString
                    End If
                Catch ex As Exception
                End Try
            Next
            If Not AlphabetButton.Checked Then
                Array.Resize(Dates, LevelFiles.Length)
                For i = 0 To LevelFiles.Length - 1
                    If DateModifiedButton.Checked Then
                        Dates(i) = System.IO.File.GetLastWriteTime(LevelFiles(i))
                    Else
                        Dates(i) = System.IO.File.GetCreationTime(LevelFiles(i))
                    End If
                Next
                Array.Sort(Dates, LevelFiles)
            End If
            'Get the search parametres:
            SearchTitle = LCase(TextBox2.Text)
            GetStringsBetweenCommas(LCase(TextBox3.Text))
            SearchLGRs = TempStrings
            GetStringsBetweenCommas(LCase(TextBox4.Text))
            SearchGroundTextures = TempStrings
            GetStringsBetweenCommas(LCase(TextBox5.Text))
            SearchSkyTextures = TempStrings
            GetStringsBetweenCommas(LCase(TextBox6.Text))
            SearchSingleTop10Names = TempStrings
            GetStringsBetweenCommas(LCase(TextBox7.Text))
            SearchMultiTop10Names = TempStrings
            SearchSingleBestTime(0) = StringToTime(MaskedTextBox1.Text)
            SearchSingleBestTime(1) = StringToTime(MaskedTextBox2.Text)
            SearchMultiBestTime(0) = StringToTime(MaskedTextBox3.Text)
            SearchMultiBestTime(1) = StringToTime(MaskedTextBox4.Text)
            RealSearchInProgress = True
            For i = 0 To LevelFiles.Length - 1
                Application.DoEvents()
                Try
                    TempBytes = System.IO.File.ReadAllBytes(LevDirectory & LevelFiles(i))
                    GetLevelProperties(TempBytes, True, False)
                    If WrongLevelLength Then
                        ErrorForm.WrongLengthList.Items.Add(LevelFiles(i))
                        'Array.Resize(ErrorLevTop10Offsets, ErrorLevTop10Offsets.Length + 1)
                        'ErrorLevTop10Offsets(ErrorLevTop10Offsets.Length - 1) = Top10Offset
                    End If
                    If (BothLevelsButton.Checked Or (ElmaButton.Checked And Not IsAcrossLevel) Or (AcrossButton.Checked And IsAcrossLevel)) _
                    And ((ExactCheckBox.Checked And LCase(LevelTitle) = SearchTitle) Or (Not ExactCheckBox.Checked And LCase(LevelTitle).Contains(SearchTitle))) Then
                        For j = 0 To 14
                            If Not (SearchAmountsLow(j) <= LevelAmounts(j) And SearchAmountsHigh(j) >= LevelAmounts(j)) Then
                                GoTo NotAdded
                            End If
                        Next
                        If LevelAmounts(13) > 0 Then
                            If Not (SearchSingleBestTime(0) <= SingleTop10Times(0) And SearchSingleBestTime(1) >= SingleTop10Times(0)) Then
                                GoTo NotAdded
                            End If
                        End If
                        If LevelAmounts(14) > 0 Then
                            If Not (SearchMultiBestTime(0) <= MultiTop10Times(0) And SearchMultiBestTime(1) >= MultiTop10Times(0)) Then
                                GoTo NotAdded
                            End If
                        End If
                        Found1 = False
                        Found2 = False
                        Found3 = False
                        For j = 0 To SearchLGRs.Length - 1
                            If LevelLGRName = SearchLGRs(j) Then
                                Found1 = True
                                Exit For
                            End If
                        Next
                        For j = 0 To SearchGroundTextures.Length - 1
                            If LevelGroundTexture = SearchGroundTextures(j) Then
                                Found2 = True
                                Exit For
                            End If
                        Next
                        For j = 0 To SearchSkyTextures.Length - 1
                            If LevelSkyTexture = SearchSkyTextures(j) Then
                                Found3 = True
                                Exit For
                            End If
                        Next
                        If ((Found1 Xor CheckBox3.Checked) Or SearchLGRs.Length = 0) And ((Found2 Xor CheckBox4.Checked) Or SearchGroundTextures.Length = 0) And ((Found3 Xor CheckBox1.Checked) Or SearchSkyTextures.Length = 0) Then
                            Found1 = False
                            Found2 = False
                            Found3 = False
                            If SearchSingleTop10Names.Length = 0 Then
                                Found1 = True
                            Else
                                For j = 0 To SearchSingleTop10Names.Length - 1
                                    Found1 = False
                                    For k = 0 To SingleTop10Names.Length - 1
                                        If LCase(SingleTop10Names(k)) = SearchSingleTop10Names(j) Then
                                            Found1 = True
                                        End If
                                    Next
                                    If Not Found1 Then Exit For
                                Next
                            End If
                            If SearchMultiTop10Names.Length = 0 Then
                                Found2 = True
                            Else
                                For j = 0 To SearchMultiTop10Names.Length - 1
                                    Found2 = False
                                    For k = 0 To MultiTop10Names.Length - 1
                                        If LCase(MultiTop10Names(k)) = SearchMultiTop10Names(j) Then
                                            Found2 = True
                                        End If
                                    Next
                                    If Not Found2 Then Exit For
                                Next
                            End If
                            If (Found1 Xor CheckBox6.Checked) And (Found2 Xor CheckBox5.Checked) Then
                                If Not BothReplaysButton.Checked Then
                                    For j = 0 To ReplayLevelFiles.Length - 1
                                        If LevelFileNames(i) = ReplayLevelFiles(j) Then
                                            Found3 = True
                                            Exit For
                                        End If
                                    Next
                                End If
                                If (Found3 And ReplaysButton.Checked) Or (Not Found3 And NoReplaysButton.Checked) Or BothReplaysButton.Checked Then
                                    AddLevelToList(LevelFiles(i))
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    ErrorForm.ErrorList.Items.Add(LevelFiles(i))
                End Try
NotAdded:
                If Not RealSearchInProgress Then
                    Exit For
                End If
                Label1.Text = "Searching... " & Math.Round(i / LevelFiles.Length * 100) & "%"
            Next
            RealSearchInProgress = False
            GroupBox1.Enabled = True
            SearchButton.Text = "Search"
            Label1.Text = "Levels found: " & LevelList.Items.Count
            If ErrorForm.ErrorList.Items.Count > 0 Or ErrorForm.WrongLengthList.Items.Count > 0 Then
                ErrorForm.Label3.Text = "Found: " & ErrorForm.ErrorList.Items.Count
                ErrorForm.Label4.Text = "Found: " & ErrorForm.WrongLengthList.Items.Count
                ErrorForm.Show()
                ErrorForm.Select()
            End If
        Else
            MsgBox("Level or replay directory doesn't exist!")
        End If
    End Sub
    Sub AddLevelToList(ByVal L As String)
        LevelList.Items.Add(L)
        Array.Resize(LevelTimes(0), LevelTimes(0).Length + 1)
        If SingleTop10Times.Length > 0 Then
            LevelTimes(0)(LevelTimes(0).Length - 1) = SingleTop10Times(0)
        Else
            LevelTimes(0)(LevelTimes(0).Length - 1) = 0
        End If
        Array.Resize(LevelTimes(1), LevelTimes(1).Length + 1)
        If MultiTop10Times.Length > 0 Then
            LevelTimes(1)(LevelTimes(1).Length - 1) = MultiTop10Times(0)
        Else
            LevelTimes(1)(LevelTimes(1).Length - 1) = 0
        End If
    End Sub
    Sub BrowseForLevelFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseForLevelFolderToolStripMenuItem.Click
        If BrowseForLevelFolderToolStripMenuItem.Text <> My.Settings.Properties.Item("LevelDirectory").DefaultValue.ToString Then FolderBrowserDialog1.SelectedPath = BrowseForLevelFolderToolStripMenuItem.Text
        FolderBrowserDialog1.Description = My.Settings.Properties.Item("LevelDirectory").DefaultValue.ToString
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            BrowseForLevelFolderToolStripMenuItem.Text = FolderBrowserDialog1.SelectedPath
            LevelList.Items.Clear()
            'ErrorForm.ErrorBox.Items.Clear()
            DisableButtons(sender, e)
        End If
    End Sub
    Sub ResetSettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetSettingsToolStripMenuItem.Click
        If MsgBox("Are you sure you want to reset the settings? Pressing Yes will close Level manager.", MsgBoxStyle.YesNo) = Windows.Forms.DialogResult.Yes Then
            BrowseForReplayFolderToolStripMenuItem.Text = My.Settings.Properties.Item("ReplayDirectory").DefaultValue
            BrowseForLevelFolderToolStripMenuItem.Text = My.Settings.Properties.Item("LevelDirectory").DefaultValue()
            DeleteToRecycleBinToolStripMenuItem.Checked = My.Settings.Properties.Item("DelToRecycle").DefaultValue
            Application.Exit()
        End If
    End Sub
    Sub SelectAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectAllToolStripMenuItem.Click
        ChangeEventDisabled = True
        For i = 0 To LevelList.Items.Count - 1
            LevelList.SetSelected(i, True)
        Next
        ChangeEventDisabled = False
        LevelList_SelectedIndexChanged(sender, e) 'For updating total time label
    End Sub
    Sub InvertSelectionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InvertSelectionToolStripMenuItem.Click
        ChangeEventDisabled = True
        For i = 0 To LevelList.Items.Count - 1
            LevelList.SetSelected(i, Not LevelList.GetSelected(i))
        Next
        ChangeEventDisabled = False
        LevelList_SelectedIndexChanged(sender, e) 'For updating total time label
    End Sub
    Sub BrowseForReplayFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseForReplayFolderToolStripMenuItem.Click
        If BrowseForReplayFolderToolStripMenuItem.Text <> My.Settings.Properties.Item("ReplayDirectory").DefaultValue.ToString Then FolderBrowserDialog1.SelectedPath = BrowseForReplayFolderToolStripMenuItem.Text
        FolderBrowserDialog1.Description = My.Settings.Properties.Item("ReplayDirectory").DefaultValue.ToString
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            BrowseForReplayFolderToolStripMenuItem.Text = FolderBrowserDialog1.SelectedPath
            LevelList.Items.Clear()
            'ErrorForm.ErrorBox.Items.Clear()
            DisableButtons(sender, e)
        End If
    End Sub
    Sub DisableButtons(ByVal sender As System.Object, ByVal e As System.EventArgs)
        LGRLabel.Visible = False
        GroundLabel.Visible = False
        SkyLabel.Visible = False
        AcrossLevLabel.Visible = False
        TitleLabel.Visible = False
        ViewerButton.Enabled = False
        DeleteButton.Enabled = False
        RemoveFromListToolStripMenuItem.Enabled = False
        CopyToToolStripMenuItem.Enabled = False
        MoveToToolStripMenuItem.Enabled = False
        ClearTop10ToolStripMenuItem.Enabled = False
        Label14.Visible = False
        SingleTop10List.Items.Clear()
        MultiTop10List.Items.Clear()
        SetupLevelPropertiesBox()
    End Sub
    Sub LevelList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LevelList.SelectedIndexChanged
        If ChangeEventDisabled Then Exit Sub
        If LevelList.SelectedItems.Count > 0 Then
            ViewerButton.Enabled = True
            DeleteButton.Enabled = True
            RemoveFromListToolStripMenuItem.Enabled = True
            CopyToToolStripMenuItem.Enabled = True
            MoveToToolStripMenuItem.Enabled = True
            ClearTop10ToolStripMenuItem.Enabled = True
            Level = System.IO.File.ReadAllBytes(LevDirectory & LevelList.SelectedItem)
            GetLevelProperties(Level, True, False)
            TitleLabel.Visible = True
            TitleLabel.Text = "Title: " & LevelTitle
            If IsAcrossLevel Then
                LGRLabel.Visible = False
                GroundLabel.Visible = False
                SkyLabel.Visible = False
                AcrossLevLabel.Visible = True
            Else
                LGRLabel.Text = "LGR: " & LevelLGRName
                GroundLabel.Text = "Ground texture: " & LevelGroundTexture
                SkyLabel.Text = "Sky texture: " & LevelSkyTexture
                LGRLabel.Visible = True
                GroundLabel.Visible = True
                SkyLabel.Visible = True
                AcrossLevLabel.Visible = False
            End If
            For i = 0 To 12
                PropertiesList.Items.Item(i) = (ListBoxStrings(i)) & LevelAmounts(i)
            Next
            SingleTop10List.Items.Clear()
            For i = 0 To SingleTop10Times.Length - 1
                SingleTop10List.Items.Add(i + 1 & ". " & SingleTop10Names(i) & " " & TimeString(SingleTop10Times(i)))
            Next
            MultiTop10List.Items.Clear()
            For i = 0 To MultiTop10Times.Length - 1
                MultiTop10List.Items.Add(i + 1 & ". " & MultiTop10Names(i * 2) & " & " & MultiTop10Names(i * 2 + 1) & " " & TimeString(MultiTop10Times(i)))
            Next
            If InvalidTop10 Then
                SingleTop10List.Items.Add("Top10 part is invalid!")
            End If
            InitializeLevel()
            ZoomFill()
            ViewerForm.Text = "Level viewer - " & LevelList.SelectedItem.ToString
            If ViewerForm.Created = True Then
                DrawLevel()
            End If
            Label14.Visible = True
            Label14_Click(sender, e)
        Else
            DisableButtons(sender, e)
        End If
    End Sub
    Sub Top10List_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SingleTop10List.MouseDown, MultiTop10List.MouseDown
        If LevelList.SelectedIndex >= 0 Then
            Level = System.IO.File.ReadAllBytes(LevDirectory & LevelList.SelectedItem)
            GetLevelProperties(Level, True, False)
            RemoveSelectedTimesToolStripMenuItem.Enabled = (sender.SelectedItems.Count > 0 And Not InvalidTop10)
            SingleListClicked = sender.Equals(SingleTop10List)
        Else
            RemoveSelectedTimesToolStripMenuItem.Enabled = False
        End If
    End Sub
    Sub RemoveReplays(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteButton.Click, RemoveFromListToolStripMenuItem.Click, DeleteToolStripMenuItem.Click
        ChangeEventDisabled = True
        Int2 = LevelList.SelectedIndex
        Do While LevelList.SelectedItems.Count > 0
            RemoveFromTimes()
            If Not sender.Equals(RemoveFromListToolStripMenuItem) Then
                Dim RecycleOpt As Microsoft.VisualBasic.FileIO.RecycleOption
                If DeleteToRecycleBinToolStripMenuItem.Checked Then
                    RecycleOpt = FileIO.RecycleOption.SendToRecycleBin
                Else
                    RecycleOpt = FileIO.RecycleOption.DeletePermanently
                End If
                My.Computer.FileSystem.DeleteFile(LevDirectory & LevelList.SelectedItems(0).ToString, FileIO.UIOption.OnlyErrorDialogs, RecycleOpt)
            End If
            LevelList.Items.RemoveAt(LevelList.SelectedIndices(0))
        Loop
        EndRemove(sender, e)
    End Sub
    Sub RemoveFromTimes()
        For j = 0 To 1
            For i = LevelList.SelectedIndices(0) To UBound(LevelTimes(j)) - 1
                LevelTimes(j)(i) = LevelTimes(j)(i + 1)
            Next
            ReDim Preserve LevelTimes(j)(UBound(LevelTimes(j)) - 1)
        Next
    End Sub
    Sub EndRemove(ByVal sender As System.Object, ByVal e As System.EventArgs) 'Updates replay count and selects new replay in the list after removal
        Label1.Text = "Levels found: " & LevelList.Items.Count()
        ChangeEventDisabled = False
        If LevelList.Items.Count = 0 Then
            DisableButtons(sender, e)
        ElseIf Int2 = 0 Then
            LevelList.SelectedIndex = 0
        Else
            LevelList.SelectedIndex = Int2 - 1
        End If
    End Sub
    Sub MoveToClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToToolStripMenuItem.Click
        MoveClicked = True
        FolderBrowserDialog1.Description = "Move to..."
        MoveOrCopy(sender, e)
    End Sub
    Sub CopyToClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToToolStripMenuItem.Click
        MoveClicked = False
        FolderBrowserDialog1.Description = "Copy to..."
        MoveOrCopy(sender, e)
    End Sub
    Sub MoveOrCopy(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If System.IO.Directory.Exists(My.Settings.MoveToPath) Then FolderBrowserDialog1.SelectedPath = My.Settings.MoveToPath
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            My.Settings.MoveToPath = FolderBrowserDialog1.SelectedPath
            ChangeEventDisabled = True
            Int2 = LevelList.SelectedIndex
            Do While LevelList.SelectedItems.Count > 0
                Dim i As Integer
                For i = Len(LevelList.SelectedItems(0)) To 1 Step -1
                    If Microsoft.VisualBasic.Mid(LevelList.SelectedItems(0).ToString, i, 1) = "\" Then
                        Exit For
                    End If
                Next
                LevelFileName = Microsoft.VisualBasic.Right(LevelList.SelectedItems(0).ToString, Len(LevelList.SelectedItems(0)) - i)
                If Not System.IO.File.Exists(FolderBrowserDialog1.SelectedPath & "\" & LevelFileName) Then
                    If MoveClicked Then
                        My.Computer.FileSystem.MoveFile(LevDirectory & LevelList.SelectedItems(0).ToString, FolderBrowserDialog1.SelectedPath & "\" & LevelFileName)
                    Else
                        My.Computer.FileSystem.CopyFile(LevDirectory & LevelList.SelectedItems(0).ToString, FolderBrowserDialog1.SelectedPath & "\" & LevelFileName)
                    End If
                Else
                    MsgBox("File " & LevelFileName & " already exists in the destination directory!")
                End If
                'RemoveFromTimes()
                LevelList.Items.RemoveAt(LevelList.SelectedIndices(0))
            Loop
            EndRemove(sender, e)
        End If
    End Sub
    Sub RemoveSelectedTimesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveSelectedTimesToolStripMenuItem.Click
        If MsgBox("Are you sure you want to delete the selected times?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            GetLevelProperties(Level, False, False)
            Array.Resize(SingleTop10HiddenNames, LevelAmounts(13))
            For i = 0 To LevelAmounts(13) - 1
                ReadBytesTillZero(Level, Top10Offset + 194 + 15 * i, 8)
                SingleTop10HiddenNames(i) = System.Text.ASCIIEncoding.ASCII.GetString(TempBytes)
            Next
            If SingleListClicked Then
                Do While SingleTop10List.SelectedItems.Count > 0
                    For j = SingleTop10List.SelectedIndices(0) To UBound(SingleTop10Names) - 1
                        SingleTop10Names(j) = SingleTop10Names(j + 1)
                        SingleTop10HiddenNames(j) = SingleTop10HiddenNames(j + 1)
                        SingleTop10Times(j) = SingleTop10Times(j + 1)
                    Next
                    ReDim Preserve SingleTop10Names(UBound(SingleTop10Names) - 1)
                    ReDim Preserve SingleTop10HiddenNames(UBound(SingleTop10HiddenNames) - 1)
                    ReDim Preserve SingleTop10Times(UBound(SingleTop10Times) - 1)
                    SingleTop10List.Items.RemoveAt(SingleTop10List.SelectedIndices(0))
                Loop
                Level(Top10Offset) = SingleTop10List.Items.Count
                Array.Clear(Level, Top10Offset + 1, 343)
                If SingleTop10List.Items.Count > 0 Then
                    For i = 0 To SingleTop10Times.Length - 1
                        TempBytes = BitConverter.GetBytes(SingleTop10Times(i))
                        For j = 0 To 3
                            Level(Top10Offset + 4 + i * 4 + j) = TempBytes(j)
                        Next
                    Next
                    For i = 0 To SingleTop10Names.Length - 1
                        TempBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(SingleTop10Names(i))
                        For j = 0 To TempBytes.Length - 1
                            Level(Top10Offset + 44 + i * 15 + j) = TempBytes(j)
                        Next
                    Next
                    For i = 0 To SingleTop10HiddenNames.Length - 1
                        TempBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(SingleTop10HiddenNames(i))
                        For j = 0 To TempBytes.Length - 1
                            Level(Top10Offset + 194 + i * 15 + j) = TempBytes(j)
                        Next
                    Next
                End If
                If SingleTop10Times.Length > 0 Then
                    LevelTimes(0)(LevelList.SelectedIndex) = SingleTop10Times(0)
                Else
                    LevelTimes(0)(LevelList.SelectedIndex) = 0
                End If
            Else
                Do While MultiTop10List.SelectedItems.Count > 0
                    For j = MultiTop10List.SelectedIndices(0) To UBound(MultiTop10Times) - 1
                        MultiTop10Names(j * 2) = MultiTop10Names(j * 2 + 2)
                        MultiTop10Names(j * 2 + 1) = MultiTop10Names(j * 2 + 3)
                        MultiTop10Times(j) = MultiTop10Times(j + 1)
                    Next
                    ReDim Preserve MultiTop10Names(UBound(MultiTop10Names) - 2)
                    ReDim Preserve MultiTop10Times(UBound(MultiTop10Times) - 1)
                    MultiTop10List.Items.RemoveAt(MultiTop10List.SelectedIndices(0))
                Loop
                Level(Top10Offset + 344) = MultiTop10List.Items.Count
                Array.Clear(Level, Top10Offset + 344 + 1, 343)
                If MultiTop10List.Items.Count > 0 Then
                    For i = 0 To MultiTop10Times.Length - 1
                        TempBytes = BitConverter.GetBytes(MultiTop10Times(i))
                        For j = 0 To 3
                            Level(Top10Offset + 344 + 4 + i * 4 + j) = TempBytes(j)
                        Next
                    Next
                    For i = 0 To MultiTop10Names.Length / 2 - 1
                        TempBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(MultiTop10Names(i * 2))
                        For j = 0 To TempBytes.Length - 1
                            Level(Top10Offset + 344 + 44 + i * 15 + j) = TempBytes(j)
                        Next
                        TempBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(MultiTop10Names(i * 2 + 1))
                        For j = 0 To TempBytes.Length - 1
                            Level(Top10Offset + 344 + 194 + i * 15 + j) = TempBytes(j)
                        Next
                    Next
                End If
                If MultiTop10Times.Length > 0 Then
                    LevelTimes(1)(LevelList.SelectedIndex) = MultiTop10Times(0)
                Else
                    LevelTimes(1)(LevelList.SelectedIndex) = 0
                End If
                End If
                'Encrypt top10 and save file
                For i = 0 To 687
                    Level(Top10Offset + i) = Level(Top10Offset + i) Xor My.Resources.emptytop10(i)
                Next
                System.IO.File.WriteAllBytes(LevDirectory & LevelList.SelectedItem.ToString, Level)
                'System.IO.File.WriteAllBytes(LevDirectory & "testijuu.lev", Level)
                LevelList_SelectedIndexChanged(sender, e)
            End If
    End Sub
    Sub ClearTop10ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearTop10ToolStripMenuItem.Click
        If MsgBox("Clear top10 from selected levels - are you sure?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            For i = 0 To LevelList.SelectedItems.Count - 1
                GetLevelProperties(Level, False, False)
                If Not InvalidTop10 Then
                    For j = 0 To 687
                        Level(Top10Offset + j) = My.Resources.emptytop10(j)
                    Next
                    System.IO.File.WriteAllBytes(LevDirectory & LevelList.SelectedItems(i), Level)
                End If
            Next
            LevelList_SelectedIndexChanged(sender, e)
        End If
    End Sub
    Sub DuplicateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DuplicateButton.MouseDown
        If RealSearchInProgress Then Exit Sub
        If DuplicateSearch Then
            DuplicateSearch = False
            Exit Sub
        End If
        RecDirectory = BrowseForReplayFolderToolStripMenuItem.Text & "\"
        LevDirectory = BrowseForLevelFolderToolStripMenuItem.Text & "\"
        If System.IO.Directory.Exists(LevDirectory) Then
            Dim k As Integer
            DisableButtons(sender, e)
            Label1.Text = "Initializing search..."
            GroupBox1.Enabled = False
            SearchPattern = TextBox1.Text & ".lev"
            DuplicateButton.Text = "Stop"
            LevelList.Items.Clear()
            Me.Refresh()
            ViewerForm.Close()
            For i = 0 To 1
                Array.Resize(LevelTimes(i), 0)
            Next
            If CheckBox7.Checked Then
                LevelFiles = System.IO.Directory.GetFiles(LevDirectory, SearchPattern, IO.SearchOption.AllDirectories)
            Else
                LevelFiles = System.IO.Directory.GetFiles(LevDirectory, SearchPattern, IO.SearchOption.TopDirectoryOnly)
            End If
            Array.Resize(LevelFileNames, LevelFiles.Length)
            Array.Resize(ReplayLevelFiles, 0)
            For i = 0 To LevelFiles.Length - 1
                LevelFileNames(i) = System.IO.Path.GetFileName(LevelFiles(i))
                LevelFiles(i) = Microsoft.VisualBasic.Mid(LevelFiles(i), Len(LevDirectory) + 1)
            Next
            If Not AlphabetButton.Checked Then
                Array.Resize(Dates, LevelFiles.Length)
                For i = 0 To LevelFiles.Length - 1
                    If DateModifiedButton.Checked Then
                        Dates(i) = System.IO.File.GetLastWriteTime(LevelFiles(i))
                    Else
                        Dates(i) = System.IO.File.GetCreationTime(LevelFiles(i))
                    End If
                Next
                Array.Sort(Dates, LevelFiles)
            End If
            DuplicateSearch = True
            'Get lengths of the level files
            Array.Resize(LevelLengths, LevelFiles.Length)
            For i = 0 To LevelFiles.Length - 1
                LevelLengths(i) = New System.IO.FileInfo(LevDirectory & LevelFiles(i)).Length
            Next
            For i = 0 To LevelFiles.Length - 2
                For j = i + 1 To LevelFiles.Length - 1
                    Application.DoEvents()
                    If LevelLengths(i) = LevelLengths(j) Then
                        TempBytes2 = System.IO.File.ReadAllBytes(LevDirectory & LevelFiles(i))
                        TempBytes3 = System.IO.File.ReadAllBytes(LevDirectory & LevelFiles(j))
                        For k = 130 To LevelLengths(i) - 693 'Ignore top10 part and level header
                            If TempBytes2(k) <> TempBytes3(k) Then
                                Exit For
                            End If
                        Next
                        If k = LevelLengths(i) - 692 Then
                            If Not LevelList.Items.Contains(LevelFiles(i)) Then
                                GetLevelProperties(TempBytes2, True, False)
                                AddLevelToList(LevelFiles(i))
                            End If
                            If Not LevelList.Items.Contains(LevelFiles(j)) Then
                                GetLevelProperties(TempBytes3, True, False)
                                AddLevelToList(LevelFiles(j))
                            End If
                        End If
                    End If
                    If Not DuplicateSearch Then Exit For
                Next
                Label1.Text = "Searching... " & Math.Round(i / LevelFiles.Length * 100) & "%"
                If Not DuplicateSearch Then Exit For
            Next
            DuplicateSearch = False
            GroupBox1.Enabled = True
            Label1.Text = "Levels found: " & LevelList.Items.Count
            DuplicateButton.Text = "Duplicate level search"
        Else
            MsgBox("Level or replay directory doesn't exist!")
        End If
    End Sub
    Sub ViewerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewerButton.Click
        ViewerForm.Show()
        ViewerForm.Select()
        ViewerForm.CheckBox1.Checked = My.Settings.FillPolygons
        ViewerForm.CheckBox2.Checked = My.Settings.AntiAliasing
    End Sub
    Sub InitializeLevel()
        Dim j As Integer
        If IsAcrossLevel Then
            NumPolygons = CInt(Math.Round(BitConverter.ToDouble(Level, 100) - 0.4643643))
            Int = 108
        Else
            NumPolygons = CInt(Math.Round(BitConverter.ToDouble(Level, 130) - 0.4643643))
            Int = 138
        End If
        GroundPolygons = NumPolygons
        Dim d As Integer = 0
        PSUM = 0
        For i = 0 To NumPolygons - 1
NewTry:
            If IsAcrossLevel Then
                NumVertice = Level(Int) + 256 * Level(Int + 1)
                Int += 4
            Else
                NumVertice = Level(Int + 4) + 256 * Level(Int + 5)
                If Level(Int) = 1 Then
                    For j = 0 To NumVertice - 1
                        PSUM += Level(Int + 8 + j * 16) + Level(Int + 16 + j * 16)
                    Next
                    Int = Int + 8 + 16 * NumVertice
                    GroundPolygons -= 1
                    i += 1
                    If i < NumPolygons Then
                        GoTo NewTry
                    Else
                        Exit For
                    End If
                End If
                Int += 8
            End If
            ReDim Preserve PlayingLevelPolygons(d)
            Array.Resize(PlayingLevelPolygons(d), 0)
            For j = 0 To NumVertice - 1
                x = CSng(BitConverter.ToDouble(Level, Int))
                y = CSng(BitConverter.ToDouble(Level, Int + 8))
                PSUM += BitConverter.ToDouble(Level, Int) + BitConverter.ToDouble(Level, Int + 8)
                If x < xmin2 Or (d = 0 And j = 0) Then xmin2 = x
                If x > xmax2 Or (d = 0 And j = 0) Then xmax2 = x
                If y < ymin2 Or (d = 0 And j = 0) Then ymin2 = y
                If y > ymax2 Or (d = 0 And j = 0) Then ymax2 = y
                Array.Resize(PlayingLevelPolygons(d), PlayingLevelPolygons(d).Length + 1)
                PlayingLevelPolygons(d)(PlayingLevelPolygons(d).Length - 1) = New PointF(x, y)
                Int += 16
            Next
            d += 1
        Next
        x = (xmax2 + xmin2) / 2
        y = (ymax2 + ymin2) / 2
        If xmax2 - xmin2 < ymax2 - ymin2 Then
            Lastx = (ymax2 - ymin2) / 2 * 1.01F
        Else
            Lastx = (xmax2 - xmin2) / 2 * 1.01F
        End If
        ZoomFillxmin = x - Lastx
        ZoomFillxmax = x + Lastx
        ZoomFillymin = y - Lastx
        ZoomFillymax = y + Lastx
        'Initialize objects
        NumObjects = CInt(Math.Round(BitConverter.ToDouble(Level, Int) - 0.4643643))
        Array.Resize(PlayingLevelObjects, NumObjects)
        Array.Resize(PlayingLevelObjectTypes, NumObjects)
        Int += 8
        OSUM = 0
        For i = 0 To NumObjects - 1
            PlayingLevelObjects(i) = New RectangleF(CSng(BitConverter.ToDouble(Level, Int) - 0.4), CSng(BitConverter.ToDouble(Level, Int + 8) - 0.4), 0.8, 0.8)
            PlayingLevelObjectTypes(i) = Level(Int + 16)
            OSUM += BitConverter.ToDouble(Level, Int) + BitConverter.ToDouble(Level, Int + 8) + Level(Int + 16)
            If IsAcrossLevel Then
                Int += 20
            Else
                Int += 28
            End If
        Next
        'Initialize polygon drawing order for drawing filled polygons
        Array.Resize(PolygonClippings, GroundPolygons)
        Array.Resize(PolygonDrawingOrder, 0)
        MaxClipping = 0
        For i = 0 To GroundPolygons - 1
            PolygonClippings(i) = 0
            For j = 0 To GroundPolygons - 1
                If i <> j Then
                    PointInPolygon(PlayingLevelPolygons(j), PlayingLevelPolygons(i)(0))
                    If IsInside Then
                        PolygonClippings(i) += 1
                    End If
                End If
            Next
            If PolygonClippings(i) > MaxClipping Then MaxClipping = PolygonClippings(i)
        Next
        For i = 0 To MaxClipping
            For j = 0 To GroundPolygons - 1
                If PolygonClippings(j) = i Then
                    Array.Resize(PolygonDrawingOrder, PolygonDrawingOrder.Length + 1)
                    PolygonDrawingOrder(PolygonDrawingOrder.Length - 1) = j
                End If
            Next
        Next
        If Not IsAcrossLevel Then
            PICSUM = 0
            Int += 38
            For i = 0 To NumPictures - 1
                PICSUM += BitConverter.ToDouble(Level, Int + i * 54) + BitConverter.ToDouble(Level, Int + i * 54 + 8)
            Next
            SUM = (PSUM + OSUM + PICSUM) * 3247.764325643
        End If
        'Calculate signed areas for fun...
        'Array.Resize(SignedAreas, PlayingLevelPolygons.Length)
        'For i = 0 To GroundPolygons - 1
        '    SignedAreas(i) = 0
        '    For j = 0 To PlayingLevelPolygons(i).Length - 2
        '        SignedAreas(i) += PlayingLevelPolygons(i)(j).X * PlayingLevelPolygons(i)(j + 1).Y - PlayingLevelPolygons(i)(j + 1).X * PlayingLevelPolygons(i)(j).Y
        '    Next
        '    SignedAreas(i) += PlayingLevelPolygons(i)(j).X * PlayingLevelPolygons(i)(0).Y - PlayingLevelPolygons(i)(0).X * PlayingLevelPolygons(i)(j).Y
        '    SignedAreas(i) /= 2
        'Next
    End Sub
    Sub PointInPolygon(ByVal Polygon As PointF(), ByVal P As PointF)
        IsInside = False
        Dim k As Single
        For i = 0 To Polygon.Length - 2
            If Not (Polygon(i).Y <= P.Y Xor Polygon(i + 1).Y > P.Y) Then
                If Polygon(i).X <= P.X And Polygon(i + 1).X <= P.X Then
                    IsInside = Not IsInside
                ElseIf Not (Polygon(i).X <= P.X Xor Polygon(i + 1).X > P.X) Then
                    k = (Polygon(i + 1).Y - Polygon(i).Y) / (Polygon(i + 1).X - Polygon(i).X) 'Division is never zero at this point
                    If Not (P.Y < k * (P.X - Polygon(i).X) + Polygon(i).Y Xor k > 0) Then IsInside = Not IsInside
                End If
            End If
        Next
        'Last edge
        If Not (Polygon(0).Y <= P.Y Xor Polygon(Polygon.Length - 1).Y > P.Y) Then
            If Polygon(0).X <= P.X And Polygon(Polygon.Length - 1).X <= P.X Then
                IsInside = Not IsInside
            ElseIf Not (Polygon(0).X <= P.X Xor Polygon(Polygon.Length - 1).X > P.X) Then
                k = (Polygon(Polygon.Length - 1).Y - Polygon(0).Y) / (Polygon(Polygon.Length - 1).X - Polygon(0).X)
                If Not (P.Y < k * (P.X - Polygon(0).X) + Polygon(0).Y Xor k > 0) Then IsInside = Not IsInside
            End If
        End If
    End Sub
    Sub EndZoom()
        ZoomLevel = Lastx
        xmin = NewCenter(0).X - ZoomLevel
        xmax = NewCenter(0).X + ZoomLevel
        ymin = NewCenter(0).Y - ZoomLevel
        ymax = NewCenter(0).Y + ZoomLevel
        CalcTransforms()
        UseDifferentMatrices = False
        ZoomFillLevelMatrix = New Drawing2D.Matrix(s, 0, 0, s, -xmin * s, ViewerForm.PictureBox1.Height - ymax * s)
        DrawLevel()
    End Sub
    Sub CalcTransforms()
        s = ViewerForm.PictureBox1.Width / (xmax - xmin)
        Lastx = 1 / s
        BlackPen.ScaleTransform(Lastx, Lastx)
        WhitePen.ScaleTransform(Lastx, Lastx)
        RedPen.ScaleTransform(Lastx, Lastx)
        GreenPen.ScaleTransform(Lastx, Lastx)
        YellowPen.ScaleTransform(Lastx, Lastx)
    End Sub
    Sub ZoomFill()
        xmin = ZoomFillxmin
        xmax = ZoomFillxmax
        ymin = ZoomFillymin
        ymax = ZoomFillymax
        ZoomLevel = (xmax - xmin) / 2
        CalcTransforms()
        UseDifferentMatrices = False
        ZoomFillLevelMatrix = New Drawing2D.Matrix(s, 0, 0, s, -xmin * s, ViewerForm.PictureBox1.Height - ymax * s)
    End Sub
    Sub DrawLevel()
        g.Transform = ZoomFillLevelMatrix
        If ViewerForm.CheckBox1.Checked Then
            g.Clear(Color.Black)
            For k = 0 To PlayingLevelPolygons.Length - 1
                Math.DivRem(PolygonClippings(PolygonDrawingOrder(k)), 2, Remainder)
                If Remainder = 0 Then
                    g.FillPolygon(Brushes.LightGray, PlayingLevelPolygons(PolygonDrawingOrder(k)))
                Else
                    g.FillPolygon(Brushes.Black, PlayingLevelPolygons(PolygonDrawingOrder(k)))
                End If
            Next
        Else
            g.Clear(Color.LightGray)
            For k = 0 To PlayingLevelPolygons.Length - 1
                g.DrawPolygon(BlackPen, PlayingLevelPolygons(k))
            Next
        End If
        For k = 0 To PlayingLevelObjects.Length - 1
            Select Case PlayingLevelObjectTypes(k)
                Case 1
                    g.DrawEllipse(WhitePen, PlayingLevelObjects(k))
                Case 2
                    g.DrawEllipse(RedPen, PlayingLevelObjects(k))
                Case 3
                    g.DrawEllipse(BlackPen, PlayingLevelObjects(k))
                Case 4
                    g.DrawEllipse(GreenPen, PlayingLevelObjects(k))
            End Select
        Next
        ViewerForm.PictureBox1.Refresh()
    End Sub
    Sub CopyToClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToClipboardToolStripMenuItem.Click
        Dim strList(0) As String
        If SingleListClicked Then
            Array.Resize(strList, SingleTop10List.Items.Count)
            SingleTop10List.Items.CopyTo(strList, 0)
        Else
            Array.Resize(strList, MultiTop10List.Items.Count)
            MultiTop10List.Items.CopyTo(strList, 0)
        End If
        Clipboard.SetDataObject(String.Join(vbCrLf, strList))
    End Sub
    Sub Label14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label14.MouseDown
        If sender.Equals(Label14) Then
            SingleTotalTimeShown = Not SingleTotalTimeShown
        End If
        TotalTime = 0
        If SingleTotalTimeShown Then
            For i = 0 To LevelList.SelectedItems.Count - 1
                TotalTime += LevelTimes(0)(LevelList.SelectedIndices(i))
            Next
            Label14.Text = "Single total time of selected levels: " & TimeString(TotalTime)
        Else
            For i = 0 To LevelList.SelectedItems.Count - 1
                TotalTime += LevelTimes(1)(LevelList.SelectedIndices(i))
            Next
            Label14.Text = "Multi total time of selected levels: " & TimeString(TotalTime)
        End If
    End Sub
End Class