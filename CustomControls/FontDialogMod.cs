using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    internal class FontDialogMod : FontDialog
    {
        private readonly IntPtr _applyCommand = new IntPtr(0x402);
        private const int WM_COMMAND = 0x0111;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            int low = unchecked((short) (long) wparam);
            int high = unchecked((short) ((long) wparam >> 16));
            switch (msg)
            {
                case WM_COMMAND:
                    switch ((int) wparam)
                    {
                        case 0x10472: // font size selected with listbox
                        case 0x50472: // font size changed with textbox
                        case 0x10470: // font face selected with listbox
                        case 0x10471: // font style selected with listbox
                        case 0x410:   // strikeout effect toggled
                        case 0x411:   // underline effect toggled

                            // simulates Apply button click
                            PostMessage(hWnd, WM_COMMAND, _applyCommand, IntPtr.Zero);
                            break;
                    }
                    break;
            }
            return base.HookProc(hWnd, msg, wparam, lparam);
        }
    }
}