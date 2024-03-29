﻿using System;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Properties;
using Elmanager.UI;

namespace Elmanager.Application;

internal partial class MainForm : FormMod
{
    public MainForm()
    {
        InitializeComponent();
        versionLabel.Text =
            $"Version: {Global.Version:dd MMMM yyyy}";
        linkLabel2.Links.Add(new LinkLabel.Link(0, 7) { Name = "License" });
        linkLabel2.Links.Add(new LinkLabel.Link(9, 9) { Name = "Libraries" });
    }

    private void ConfigButtonClick(object sender, EventArgs e)
    {
        ComponentManager.ShowConfiguration();
    }

    private void HomePageClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OsUtils.ShellExecute(homePageLabel.Text);
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
        OsUtils.ShellExecute("https://web.archive.org/web/20160121234513/https://www.oscarstours.ca/avis-de-deces/m-marck-antoine-simoneau");
    }

    private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        var txt = e.Link!.Name == "License" ? Resources.LICENSE : Resources.LICENSE_3RD_PARTY;

        using var f = new LicenseForm(e.Link.Name, txt);
        f.ShowDialog();
    }

    private void levelManagerButton_Click(object sender, EventArgs e)
    {
        ComponentManager.LaunchLevelManager();
        Close();
    }
}