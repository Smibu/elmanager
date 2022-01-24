using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class FontDialogMod : FontDialog
{
    private const int ApplyClick = 0x402;
    private readonly IntPtr _applyCommand = new(ApplyClick);
    public string FontStyleName { get; private set; }
    private const int WmCommand = 0x0111;

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetDlgItemText(IntPtr hDlg, int nIdDlgItem,
        [Out] StringBuilder lpString, int nMaxCount);

    public FontDialogMod()
    {
        FontStyleName = "";
    }

    protected override bool RunDialog(IntPtr hWndOwner)
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

    protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        switch (msg)
        {
            case WmCommand:
                switch ((int) wparam)
                {
                    case ApplyClick:
                        var sb = new StringBuilder();
                        GetDlgItemText(hWnd, 0x471, sb, 100);
                        FontStyleName = sb.ToString();
                        break;
                    case 0x10472: // font size selected with listbox
                    case 0x50472: // font size changed with textbox
                    case 0x10470: // font face selected with listbox
                    case 0x10471: // font style selected with listbox
                    case 0x410: // strikeout effect toggled
                    case 0x411: // underline effect toggled

                        // simulates Apply button click
                        PostMessage(hWnd, WmCommand, _applyCommand, IntPtr.Zero);
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
            return IntPtr.Zero;
        }
    }
}