using System;
using System.Runtime.InteropServices;

namespace Elmanager.UI;

internal static class NativeUtils
{
    [DllImport("user32.dll")]
    public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

    public const int WmKeydown = 0x100;
    public const int WmKeyup = 0x101;
}
