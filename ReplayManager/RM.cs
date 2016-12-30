using System.Diagnostics;

namespace RM
{
    internal static class Launch
    {
        public static void Main()
        {
            var p = new ProcessStartInfo()
            {
                FileName = "Elmanager.exe",
                Arguments = "/replaymanager"
            };
            Process.Start(p);
        }
    }
}
