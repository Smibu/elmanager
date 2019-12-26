using System;
using System.Diagnostics;
using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    partial class MainForm : FormMod
    {
        public MainForm()
        {
            InitializeComponent();
            versionLabel.Text = $"Version: {Global.BuildDate.ToShortDateString()} ({ThisAssembly.GitCommitId.Substring(0, 8)})";
#if DEBUG
            versionLabel.Text += " [DEBUG BUILD]";
#endif
        }

        private void ConfigButtonClick(object sender, EventArgs e)
        {
            ComponentManager.ShowConfiguration();
        }

        private void HomePageClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(homePageLabel.Text);
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
            Process.Start(
                new ProcessStartInfo("http://www.oscarstours.ca/avis-de-deces/m-marck-antoine-simoneau#defunt")
                    {UseShellExecute = true});
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://radimrehurek.com/") {UseShellExecute = true});
        }

        private void levelManagerButton_Click(object sender, EventArgs e)
        {
            ComponentManager.LaunchLevelManager();
            Close();
        }
    }
}