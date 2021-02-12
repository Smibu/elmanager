using System;
using System.Windows.Forms;
using Elmanager.CustomControls;
using Elmanager.Properties;

namespace Elmanager.Forms
{
    internal partial class MainForm : FormMod
    {
        public MainForm()
        {
            InitializeComponent();
            versionLabel.Text =
                $"Version: {Global.BuildDate.ToShortDateString()} ({ThisAssembly.GitCommitId.Substring(0, 8)})";
            var conf = "Debug";
            if (ThisAssembly.AssemblyConfiguration == conf)
            {
                versionLabel.Text += " [DEBUG BUILD]";
            }
            linkLabel2.Links.Add(new LinkLabel.Link(0, 7) {Name = "License"});
            linkLabel2.Links.Add(new LinkLabel.Link(9, 9) {Name = "Libraries"});
        }

        private void ConfigButtonClick(object sender, EventArgs e)
        {
            ComponentManager.ShowConfiguration();
        }

        private void HomePageClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.ShellExecute(homePageLabel.Text);
        }

        private void OpenLevelEditor(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ComponentManager.LaunchLevelEditor();
            Close();
        }

        private void OpenReplayManager(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ComponentManager.LaunchReplayManager();
            Close();
        }

        private void StartUp(object sender, EventArgs e)
        {
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.ShellExecute("http://www.oscarstours.ca/avis-de-deces/m-marck-antoine-simoneau#defunt");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var txt = e.Link.Name == "License" ? Resources.LICENSE : Resources.LICENSE_3RD_PARTY;

            using var f = new LicenseForm(e.Link.Name, txt);
            f.ShowDialog();
        }

        private void levelManagerButton_Click(object sender, EventArgs e)
        {
            ComponentManager.LaunchLevelManager();
            Close();
        }
    }
}