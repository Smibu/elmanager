using System.Collections.Generic;
using System.Windows.Forms;

namespace Elmanager
{
    internal class AppContext : ApplicationContext
    {
        private readonly List<Form> _forms = new();

        public void AddAndShow(Form form)
        {
            _forms.Add(form);
            form.FormClosed += FormClosed;
            form.Show();
        }

        private void FormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            _forms.Remove((Form)sender);
            if (_forms.Count == 0)
            {
                Global.AppSettings.Save();
                ExitThread();
            }
        }
    }
}