Public Class ErrorForm
    Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Do While ErrorList.Items.Count > 0
            Dim RecycleOpt As Microsoft.VisualBasic.FileIO.RecycleOption
            If MainForm.DeleteToRecycleBinToolStripMenuItem.Checked Then
                RecycleOpt = FileIO.RecycleOption.SendToRecycleBin
            Else
                RecycleOpt = FileIO.RecycleOption.DeletePermanently
            End If
            My.Computer.FileSystem.DeleteFile(MainForm.LevDirectory & ErrorList.Items(0).ToString, FileIO.UIOption.OnlyErrorDialogs, RecycleOpt)
            ErrorList.Items.RemoveAt(0)
        Loop
    End Sub
    Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        With MainForm
            For i = 0 To WrongLengthList.Items.Count - 1
                Dim TempBytes As Byte() = System.IO.File.ReadAllBytes(.LevDirectory & WrongLengthList.Items(i))
                .GetLevelProperties(TempBytes, False, True)
                Array.Resize(TempBytes, .Top10Offset + 688 + 4)
                System.IO.File.WriteAllBytes(.LevDirectory & WrongLengthList.Items(i), TempBytes)
                Label4.Text = "Fixing... " & Math.Round((i + 1) / WrongLengthList.Items.Count * 100) & "%"
                Label4.Refresh()
            Next
            WrongLengthList.Items.Clear()
            Label4.Text = "Fixing... Done!"
        End With
    End Sub
End Class