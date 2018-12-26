using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using My.Resources;

namespace Elmanager.Forms
{
    partial class MainForm
    {
        public MainForm()
        {
            InitializeComponent();
            versionLabel.Text = "Version: " + Global.BuildDate.ToLongDateString();
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
            LevelEditor le = new LevelEditor();
            Visible = false;
            le.ShowDialog();
            Close();
        }

        private void OpenReplayManager(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ReplayManager rm = new ReplayManager();
            Visible = false;
            rm.ShowDialog();
            Close();
        }

        private void StartUp(object sender, EventArgs e)
        {
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.oscarstours.ca/avis-de-deces/m-marck-antoine-simoneau#defunt");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://radimrehurek.com/");
        }

        private void levelManagerButton_Click(object sender, EventArgs e)
        {
            ComponentManager.LaunchLevelManager();
            Close();
        }
    }
}