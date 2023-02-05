using System;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class TabControlMod : TabControl
{
    private KeyDownEventHandler? _keyDownEvent;

    internal new event KeyDownEventHandler KeyDown
    {
        add => _keyDownEvent = (KeyDownEventHandler)Delegate.Combine(_keyDownEvent, value);
        remove => _keyDownEvent = (KeyDownEventHandler?)Delegate.Remove(_keyDownEvent, value);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeUtils.WmKeydown)
        {
            var key = (Keys)m.WParam;
            _keyDownEvent?.Invoke(null, new KeyEventArgs(key));
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    internal delegate void KeyDownEventHandler(object? sender, KeyEventArgs e);
}