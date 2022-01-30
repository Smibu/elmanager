using System;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class RadioButtonMod : RadioButton
{
    public KeyDownEventHandler KeyDownEvent;

    internal new event KeyDownEventHandler KeyDown
    {
        add => KeyDownEvent = (KeyDownEventHandler) Delegate.Combine(KeyDownEvent, value);
        remove => KeyDownEvent = (KeyDownEventHandler) Delegate.Remove(KeyDownEvent, value);
    }

    protected override bool IsInputKey(Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
                KeyDownEvent?.Invoke(null, new KeyEventArgs(keyData));
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
            var pressedKey = (Keys) m.WParam;
            if (pressedKey != Keys.Up && pressedKey != Keys.Down && pressedKey != Keys.Left &&
                pressedKey != Keys.Right)
            {
                KeyDownEvent?.Invoke(null, new KeyEventArgs(pressedKey));
            }

            return;
        }

        base.WndProc(ref m);
    }

    internal delegate void KeyDownEventHandler(object sender, KeyEventArgs e);
}