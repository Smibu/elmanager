using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Elmanager.UI;

internal partial class FontDialogMod : FontDialog
{
    private const int ApplyClick = 0x402;
    private readonly nint _applyCommand = new(ApplyClick);
    public string FontStyleName { get; private set; }
    private const int WmCommand = 0x0111;

    [return: MarshalAs(UnmanagedType.Bool)]
    [LibraryImport("user32.dll")]
    private static partial bool PostMessageW(nint hWnd, int wMsg, nint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint GetDlgItemTextW(nint hDlg, int nIdDlgItem,
        [Out] char[] lpString, int nMaxCount);

    public FontDialogMod()
    {
        FontStyleName = "";
    }

    protected override bool RunDialog(nint hWndOwner)
    {
        while (true)
        {
            try
            {
                return base.RunDialog(hWndOwner);
            }
            catch (ArgumentException)
            {
                UiUtils.ShowError("Sorry, this font does not work. Pick some other font.");
            }
        }
    }

    protected override nint HookProc(nint hWnd, int msg, nint wparam, nint lparam)
    {
        switch (msg)
        {
            case WmCommand:
                switch ((int)wparam)
                {
                    case ApplyClick:
                        var buffer = new char[100];
                        GetDlgItemTextW(hWnd, 0x471, buffer, buffer.Length);
                        FontStyleName = new string(buffer);
                        break;
                    case 0x10472: // font size selected with listbox
                    case 0x50472: // font size changed with textbox
                    case 0x10470: // font face selected with listbox
                    case 0x10471: // font style selected with listbox
                    case 0x410: // strikeout effect toggled
                    case 0x411: // underline effect toggled

                        // simulates Apply button click
                        PostMessageW(hWnd, WmCommand, _applyCommand, nint.Zero);
                        break;
                }

                break;
        }

        try
        {
            return base.HookProc(hWnd, msg, wparam, lparam);
        }
        catch (ArgumentException)
        {
            return nint.Zero;
        }
    }
}