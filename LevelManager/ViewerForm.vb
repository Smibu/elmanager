Public Class ViewerForm
    Dim RandomClass As New Random()
    Dim s2 As Single
    Dim TempVar As Single
    Declare Function GetAsyncKeyState Lib "user32" (ByVal vkey As Long) As Integer
    Sub ZoomInOrOut(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        With MainForm
            .NewCenter = New PointF() {PictureBox1.PointToClient(MousePosition)}
            .g.TransformPoints(Drawing2D.CoordinateSpace.World, Drawing2D.CoordinateSpace.Device, .NewCenter)
            If e.Button = Windows.Forms.MouseButtons.Left Then
                If BitConverter.GetBytes(GetAsyncKeyState(Keys.ControlKey))(3) And 128 Then
                    .ZoomRecting = True
                    .ZoomRectStartPoint = .NewCenter(0)
                    Exit Sub
                End If
                TempVar = 0.4F
            ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
                TempVar = 1 / 0.4F
            Else
                Exit Sub
            End If
            .Lastx = (.xmax - .xmin) / 2 * TempVar
            If .Lastx > 500 Or .Lastx < 0.001 Then Exit Sub
            If CheckBox3.Checked Then
                .NewCenter(0).X -= (.NewCenter(0).X - (.xmax + .xmin) / 2) * TempVar
                .NewCenter(0).Y -= (.NewCenter(0).Y - (.ymax + .ymin) / 2) * TempVar
            End If
            .EndZoom()
        End With
    End Sub
    Sub ResizeForm(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd, Me.Shown, CheckBox1.CheckedChanged, CheckBox2.CheckedChanged
        With MainForm
            Me.Height = Me.Width + 120
            GroupBox1.Location = New Point(1, Me.Height - 130)
            GroupBox1.Width = Me.Width - 18
            PictureBox1.Size = New System.Drawing.Size(Me.Width - 16, Me.Width - 16)
            PictureBox1.Image = New Bitmap(PictureBox1.Width, PictureBox1.Height)
            .g = Graphics.FromImage(PictureBox1.Image)
            If CheckBox2.Checked Then
                .g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Else
                .g.SmoothingMode = Drawing2D.SmoothingMode.None
            End If
            .ZoomFill()
            .DrawLevel()
        End With
    End Sub
    Sub ZoomFillButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZoomFillButton.MouseDown
        MainForm.ZoomFill()
        MainForm.DrawLevel()
    End Sub
    Sub ZoomRectEnding() Handles PictureBox1.MouseUp
        With MainForm
            If Not .ZoomRecting Then Exit Sub
            .ZoomRecting = False
            .NewCenter = New PointF() {PictureBox1.PointToClient(MousePosition)}
            If .UseDifferentMatrices Then
                .g.Transform = .LevelMatrix
            Else
                .g.Transform = .ZoomFillLevelMatrix
            End If
            .g.TransformPoints(Drawing2D.CoordinateSpace.World, Drawing2D.CoordinateSpace.Device, .NewCenter)
            .ZoomRectEndPoint = .NewCenter(0)
            If .ZoomRectStartPoint.X = .ZoomRectEndPoint.X And .ZoomRectStartPoint.Y = .ZoomRectEndPoint.Y Then
                MsgBox("Invalid zoom area!")
            Else
                If .ZoomRectStartPoint.X < .ZoomRectEndPoint.X Then
                    .xmin2 = .ZoomRectStartPoint.X
                    .xmax2 = .ZoomRectEndPoint.X
                Else
                    .xmax2 = .ZoomRectStartPoint.X
                    .xmin2 = .ZoomRectEndPoint.X
                End If
                If .ZoomRectStartPoint.Y < .ZoomRectEndPoint.Y Then
                    .ymin2 = .ZoomRectStartPoint.Y
                    .ymax2 = .ZoomRectEndPoint.Y
                Else
                    .ymax2 = .ZoomRectStartPoint.Y
                    .ymin2 = .ZoomRectEndPoint.Y
                End If
                .NewCenter(0).X = (.xmax2 + .xmin2) / 2 'New center point X
                .NewCenter(0).Y = (.ymax2 + .ymin2) / 2 'New center point Y
                If .xmax2 - .xmin2 > .ymax2 - .ymin2 Then
                    .Lastx = (.xmax2 - .xmin2) / 2
                Else
                    .Lastx = (.ymax2 - .ymin2) / 2
                End If
                If .Lastx > 500 Or .Lastx < 0.001 Then Exit Sub
                .EndZoom()
            End If
        End With
    End Sub
    Sub CloseViewer(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Visible = False
        My.Settings.FillPolygons = CheckBox1.Checked
        My.Settings.AntiAliasing = CheckBox2.Checked
    End Sub
    Sub SavePicButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SavePicButton.Click
        SaveFileDialog1.DefaultExt = "png"
        SaveFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png"
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then PictureBox1.Image.Save(SaveFileDialog1.FileName, Drawing.Imaging.ImageFormat.Png)
    End Sub
    Sub MirrorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MirrorButton.Click
        With MainForm
            .GetLevelProperties(.Level, False, False)
            If Not .IsAcrossLevel Then
                .Int = 130
            Else
                .Int = 100
            End If
            .NumPolygons = Math.Round(BitConverter.ToDouble(.Level, .Int) - 0.4643643)
            .Int += 8
            For i = 1 To .NumPolygons
                If Not .IsAcrossLevel Then .Int += 4
                .Int2 = BitConverter.ToUInt16(.Level, .Int)
                .Int += 4
                For j = 0 To .Int2 - 1
                    'Change x-coordinates here
                    Buffer.BlockCopy(BitConverter.GetBytes(.midx - (BitConverter.ToDouble(.Level, .Int + j * 16) - .midx)), 0, .Level, .Int + j * 16, 8)
                Next
                'The order of vertex coordinates for each polygon in memory must be reversed
                Dim TempBytes(15) As Byte
                Dim TempBytes2(15) As Byte
                For j = 0 To Math.Floor(.Int2 / 2) - 1
                    For k = 0 To 15
                        TempBytes(k) = .Level(.Int + j * 16 + k)
                        TempBytes2(k) = .Level(.Int + k + (.Int2 - 1) * 16 - j * 16)
                    Next
                    Buffer.BlockCopy(TempBytes, 0, .Level, .Int + (.Int2 - 1) * 16 - j * 16, 16)
                    Buffer.BlockCopy(TempBytes2, 0, .Level, .Int + j * 16, 16)
                Next
                .Int += .Int2 * 16
            Next
            .NumObjects = Math.Round(BitConverter.ToDouble(.Level, .Int) - 0.4643643)
            .Int += 8
            For i = 1 To .NumObjects
                'Int is in object's x-coordinate now
                If .Level(.Int + 16) = 4 Then
                    Buffer.BlockCopy(BitConverter.GetBytes(.midx - (BitConverter.ToDouble(.Level, .Int) + 0.85 - .midx) - 0.85), 0, .Level, .Int, 8)
                Else
                    Buffer.BlockCopy(BitConverter.GetBytes(.midx - (BitConverter.ToDouble(.Level, .Int) - .midx)), 0, .Level, .Int, 8)
                    If .Level(.Int + 16) = 2 Then 'Reverse gravity left & right apples
                        Select Case .Level(.Int + 20)
                            Case 3
                                .Level(.Int + 20) = 4
                            Case 4
                                .Level(.Int + 20) = 3
                        End Select
                    End If
                End If
                .Int += 20
                If Not .IsAcrossLevel Then .Int += 8
            Next
            If Not .IsAcrossLevel Then
                .NumPictures = Math.Round(BitConverter.ToDouble(.Level, .Int) - 0.2345672)
                .Int += 38
                If .NumPictures > 0 Then
                    For i = 1 To .NumPictures
                        'Int is in picture's x-coordinate
                        Buffer.BlockCopy(BitConverter.GetBytes(.midx - (BitConverter.ToDouble(.Level, .Int) - .midx)), 0, .Level, .Int, 8)
                        .Int += 54
                    Next
                End If
            End If
            .InitializeLevel()
            .ZoomFill()
            .DrawLevel()
        End With
    End Sub
    Sub SaveLevButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveLevButton.Click
        SaveFileDialog1.DefaultExt = "lev"
        SaveFileDialog1.Filter = "Elasto Mania level (*.lev)|*.lev"
        With MainForm
            .GetLevelProperties(.Level, False, False)
            For j = 0 To 687
                .Level(.Top10Offset + j) = My.Resources.emptytop10(j)
            Next
            Dim RandBytes(3) As Byte
            RandomClass.NextBytes(RandBytes)
            If .IsAcrossLevel Then
                Buffer.BlockCopy(RandBytes, 0, .Level, 5, 4)
            Else
                Buffer.BlockCopy(RandBytes, 0, .Level, 7, 4)
                Buffer.BlockCopy(RandBytes, 0, .Level, 5, 2)
                'Calculate integrities:
                Buffer.BlockCopy(BitConverter.GetBytes(.SUM), 0, .Level, 11, 8)
                Buffer.BlockCopy(BitConverter.GetBytes(11877 - .SUM + RandomClass.Next(0, 5871)), 0, .Level, 19, 8)
                Buffer.BlockCopy(BitConverter.GetBytes(11877 - .SUM + RandomClass.Next(0, 5871)), 0, .Level, 27, 8)
                Buffer.BlockCopy(BitConverter.GetBytes(12112 - .SUM + RandomClass.Next(0, 6102)), 0, .Level, 35, 8)
            End If
            SaveFileDialog1.InitialDirectory = .LevDirectory
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then System.IO.File.WriteAllBytes(SaveFileDialog1.FileName, .Level)
        End With
    End Sub
    Sub ShowCoordinates() Handles PictureBox1.MouseMove
        With MainForm
            .NewCenter = New PointF() {PictureBox1.PointToClient(MousePosition)}
            .g.TransformPoints(Drawing2D.CoordinateSpace.World, Drawing2D.CoordinateSpace.Device, .NewCenter)
            Label1.Text = "X: " & Math.Round(.NewCenter(0).X, 3) & " Y: " & Math.Round(.NewCenter(0).Y, 3)
        End With
    End Sub
    Sub MouseWheelZoom(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        With MainForm
            .NewCenter = New PointF() {PictureBox1.PointToClient(MousePosition)}
            .g.TransformPoints(Drawing2D.CoordinateSpace.World, Drawing2D.CoordinateSpace.Device, .NewCenter)
            If e.Delta > 0 Then
                TempVar = 0.8F
            Else
                TempVar = 1 / 0.8F
            End If
            s2 = (.xmax - .xmin) / 2 * TempVar
            If s2 > 500 Or s2 < 0.001 Then Exit Sub
            .Lastx = s2
            If CheckBox3.Checked Then
                .NewCenter(0).X -= (.NewCenter(0).X - (.xmax + .xmin) / 2) * TempVar
                .NewCenter(0).Y -= (.NewCenter(0).Y - (.ymax + .ymin) / 2) * TempVar
            End If
            .EndZoom()
        End With
    End Sub
End Class