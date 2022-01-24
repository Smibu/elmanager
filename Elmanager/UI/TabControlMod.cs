using System;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class TabControlMod : TabControl
{
    public KeyDownEventHandler KeyDownEvent;

    internal new event KeyDownEventHandler KeyDown
    {
        add => KeyDownEvent = (KeyDownEventHandler) Delegate.Combine(KeyDownEvent, value);
        remove => KeyDownEvent = (KeyDownEventHandler) Delegate.Remove(KeyDownEvent, value);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x100) //WM_KEYDOWN
        {
            var key = (Keys) m.WParam;
            KeyDownEvent?.Invoke(null, new KeyEventArgs(key));
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    internal delegate void KeyDownEventHandler(object sender, KeyEventArgs e);
}