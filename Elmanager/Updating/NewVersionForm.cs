using System;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.IO;
using Elmanager.UI;

namespace Elmanager.Updating
{
    internal partial class NewVersionForm : FormMod
    {
        private readonly UpdateInfo _updateInfo;

        public NewVersionForm(UpdateInfo info)
        {
            _updateInfo = info;
            InitializeComponent();
            versionInfoLabel.Text = $"Latest version: {info.Date.ToShortDateString()}. Your version: {Global.Version.ToShortDateString()}";
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
            NetUtils.DownloadAndOpenFile(_updateInfo.Link, System.Windows.Forms.Application.StartupPath + "\\Elmanager.zip");
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OsUtils.ShellExecute(UpdateChecker.ChangelogUri);
        }
    }
}