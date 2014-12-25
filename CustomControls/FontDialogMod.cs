using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    internal class FontDialogMod : FontDialog
    {

        private readonly IntPtr _applyCommand = new IntPtr(0x402);
        public string FontStyleName { get; private set; }
        private const int WM_COMMAND = 0x0111;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetDlgItemText(IntPtr hDlg, int nIDDlgItem,
           [Out] StringBuilder lpString, int nMaxCount);

        public FontDialogMod()
        {
            FontStyleName = "";
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            int low = unchecked((short) (long) wparam);
            int high = unchecked((short) ((long) wparam >> 16));
            switch (msg)
            {
                case WM_COMMAND:
                    switch ((int) wparam)
                    {
                        case 0x402: 
                            var sb = new StringBuilder();
                            GetDlgItemText(hWnd, 0x471, sb, 100);
                            FontStyleName = sb.ToString();
                            break;
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