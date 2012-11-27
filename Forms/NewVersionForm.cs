using System;
using System.Net;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    public partial class NewVersionForm : Form
    {
        public NewVersionForm()
        {
            InitializeComponent();
        }

        public void LoadChangelog()
        {
            var wc = new WebClient();
            string changelog = wc.DownloadString(Constants.ChangelogUri).Replace("\n", "\r\n");
            textBox1.Text = changelog;
        }

        private void NewVersionForm_Load(object sender, EventArgs e)
        {
            LoadChangelog();
            downloadButton.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Utils.DownloadAndOpenFile(Constants.ProgramUri, Application.StartupPath + "\\Elmanager.zip");
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}