using System.Diagnostics;

namespace SLE
{
    internal static class Sle
    {
        public static void Main()
        {
            var p = new ProcessStartInfo()
            {
                FileName = "Elmanager.exe",
                Arguments = "/leveleditor"
            };
            Process.Start(p);
        }
    }
}
