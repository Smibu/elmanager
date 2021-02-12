using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    internal class NoPaintBgControl : Control
    {
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
