using System.Runtime.InteropServices;

namespace Elmanager.UI;

internal static partial class NativeUtils
{
    [return: MarshalAs(UnmanagedType.Bool)]
    [LibraryImport("user32.dll")]
    public static partial bool IsChild(nint hWndParent, nint hWnd);

    public const int WmKeydown = 0x100;
    public const int WmKeyup = 0x101;
}
