using System;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class RadioButtonMod : RadioButton
{
    private KeyDownEventHandler? _keyDownEvent;

    internal new event KeyDownEventHandler KeyDown
    {
        add => _keyDownEvent = (KeyDownEventHandler)Delegate.Combine(_keyDownEvent, value);
        remove => _keyDownEvent = (KeyDownEventHandler?)Delegate.Remove(_keyDownEvent, value);
    }

    protected override bool IsInputKey(Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
                _keyDownEvent?.Invoke(null, new KeyEventArgs(keyData));
                return true;
            case Keys.Tab:
                return true;
        }

        return base.IsInputKey(keyData);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeUtils.WmKeydown)
        {
            var pressedKey = (Keys)m.WParam;
            if (pressedKey != Keys.Up && pressedKey != Keys.Down && pressedKey != Keys.Left &&
                pressedKey != Keys.Right)
            {
                _keyDownEvent?.Invoke(null, new KeyEventArgs(pressedKey));
            }

            return;
        }

        base.WndProc(ref m);
    }

    internal delegate void KeyDownEventHandler(object? sender, KeyEventArgs e);
}