using System;
using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    internal class PanelMod : Panel
    {
        private MouseWheelEventHandler _mouseWheelEvent;

        internal new event MouseWheelEventHandler MouseWheel
        {
            add { _mouseWheelEvent = (MouseWheelEventHandler) Delegate.Combine(_mouseWheelEvent, value); }
            remove { _mouseWheelEvent = (MouseWheelEventHandler) Delegate.Remove(_mouseWheelEvent, value); }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x20A) //WM_MOUSEWHEEL
            {
                if (_mouseWheelEvent != null)
                {
                    long val = (long) m.WParam;
                    if (val > int.MaxValue) return;
                    _mouseWheelEvent((int) val);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        internal delegate void MouseWheelEventHandler(long delta);
    }
}