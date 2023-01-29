using System;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class TrackBarMod : TrackBar
{
    private MouseWheelEventHandler? _mouseWheelEvent;

    internal new event MouseWheelEventHandler MouseWheel
    {
        add => _mouseWheelEvent = (MouseWheelEventHandler)Delegate.Combine(_mouseWheelEvent, value);
        remove => _mouseWheelEvent = (MouseWheelEventHandler?)Delegate.Remove(_mouseWheelEvent, value);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x20A) //WM_MOUSEWHEEL
        {
            _mouseWheelEvent?.Invoke(m.WParam.ToInt32());
        }
        else
            base.WndProc(ref m);
    }

    internal delegate void MouseWheelEventHandler(int delta);
}