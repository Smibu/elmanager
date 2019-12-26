using System.Drawing;
using System.Windows.Forms;
using My.Resources;

namespace Elmanager.CustomControls
{
    public class FormMod : Form
    {
        protected FormMod()
        {
            Icon = Resources.Elma;
            Font = new Font(new FontFamily("Microsoft Sans Serif"), 8f);
        }
    }
}
