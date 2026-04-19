using System.Runtime.InteropServices;

namespace Elmanager.UI;

internal static partial class NativeUtils
{
    [return: MarshalAs(UnmanagedType.Bool)]
    [LibraryImport("user32.dll")]
    public static partial bool IsChild(nint hWndParent, nint hWnd);

    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekMessage(
        out System.Windows.Forms.Message lpMsg,
        nint hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint wRemoveMsg);

    public static bool IsApplicationIdle() =>
        !PeekMessage(out _, nint.Zero, 0, 0, 0);

    public const int WmKeydown = 0x100;
    public const int WmKeyup = 0x101;
}
