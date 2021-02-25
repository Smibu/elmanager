using System.Windows.Forms;

namespace Elmanager.UI
{
    internal class NoPaintBgControl : Control
    {
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
