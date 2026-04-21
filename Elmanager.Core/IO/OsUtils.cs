using System.Diagnostics;

namespace Elmanager.IO;

public static class OsUtils
{
    public static void ShellExecute(string url)
    {
        Process.Start(
            new ProcessStartInfo(url)
            { UseShellExecute = true });
    }
}
