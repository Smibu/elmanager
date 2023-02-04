using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Elmanager.IO;

internal static partial class KeyboardUtils
{
    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(Keys key);

    public static bool IsKeyDown(Keys key)
    {
        return GetAsyncKeyState(key) < 0;
    }
}