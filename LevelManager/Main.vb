Imports System.IO

Module Main
    Sub Main()
        Dim p As New ProcessStartInfo("") With {
            .FileName = Path.Combine(Application.StartupPath, "Elmanager.exe"),
            .Arguments = "/levelmanager"
        }
        Process.Start(p)
    End Sub
End Module
