using System.Diagnostics;

namespace Elmanager.IO
{
    internal static class OsUtils
    {
        internal static void ShellExecute(string url)
        {
            Process.Start(
                new ProcessStartInfo(url)
                    {UseShellExecute = true});
        }
    }
}