using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    public partial class NewVersionForm : Form
    {
        public NewVersionForm()
        {
            InitializeComponent();
        }

        private void NewVersionForm_Load(object sender, EventArgs e)
        {
            downloadButton.Select();
        }

        private void DownloadButtonClick(object sender, EventArgs e)
        {
            downloadButton.Text = "Downloading...";
            downloadButton.Enabled = false;
            downloadButton.Refresh();
            Utils.DownloadAndOpenFile(Constants.ProgramUri, Application.StartupPath + "\\Elmanager.zip");
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Constants.ChangelogUri);
        }
    }
}