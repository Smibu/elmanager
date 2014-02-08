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
            IntPtr z = Marshal.AllocHGlobal(Resources.Arial_Rounded_MT.Length);
            Marshal.Copy(Resources.Arial_Rounded_MT, 0, z, Resources.Arial_Rounded_MT.Length);
            Global.ElmaFonts.AddMemoryFont(z, Resources.Arial_Rounded_MT.Length);
            Marshal.FreeHGlobal(z);
            Font elmaFont = new Font(Global.ElmaFonts.Families[0], 36, FontStyle.Bold);
            titleLabel.Font = elmaFont;
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:mawane@hotmail.com");
        }
    }
}