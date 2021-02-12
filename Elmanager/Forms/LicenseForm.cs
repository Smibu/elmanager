using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    internal sealed partial class LicenseForm : FormMod
    {
        public LicenseForm()
        {
            InitializeComponent();
        }

        public LicenseForm(string title, string text) : this()
        {
            Text = title;
            textBox1.Text = text;
            textBox1.Select(0, 0);
        }
    }
}
