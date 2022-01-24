using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Elmanager.IO;

internal static class KeyboardUtils
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(Keys key);

    public static bool IsKeyDown(Keys key)
    {
        return GetAsyncKeyState(key) < 0;
    }
}