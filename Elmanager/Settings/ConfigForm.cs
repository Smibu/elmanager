using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.LevelEditor;
using Elmanager.UI;
using Elmanager.Updating;
using SearchOption = System.IO.SearchOption;

namespace Elmanager.Settings;

internal partial class ConfigForm : FormMod
{
    private bool _levelDirectoryChanged;

    internal ConfigForm()
    {
        InitializeComponent();
        LevTextBox.Text = Global.AppSettings.General.LevelDirectory;
        RecTextBox.Text = Global.AppSettings.General.ReplayDirectory;
        LGRTextBox.Text = Global.AppSettings.General.LgrDirectory;

        NitroBox.Checked = Global.AppSettings.ReplayManager.NitroReplays;
        ShowReplayListGridBox.Checked = Global.AppSettings.ReplayManager.ShowGridInList;
        SearchLevSubDirsBox.Checked = Global.AppSettings.ReplayManager.LevDirSearchOption == SearchOption.AllDirectories;
        SearchRecSubDirsBox.Checked = Global.AppSettings.ReplayManager.RecDirSearchOption == SearchOption.AllDirectories;
        DeleteConfirmCheckBox.Checked = Global.AppSettings.ReplayManager.ConfirmDelete;
        showTooltipForReplaysCheckBox.Checked = Global.AppSettings.ReplayManager.ShowTooltipInList;

        lmShowGrid.Checked = Global.AppSettings.LevelManager.ShowGridInList;
        lmSearchLevSubDirs.Checked = Global.AppSettings.LevelManager.LevDirSearchOption == SearchOption.AllDirectories;
        lmSearchRecSubDirs.Checked = Global.AppSettings.LevelManager.RecDirSearchOption == SearchOption.AllDirectories;
        lmConfirmDeletion.Checked = Global.AppSettings.LevelManager.ConfirmDelete;
        lmShowTooltip.Checked = Global.AppSettings.LevelManager.ShowTooltipInList;

        LevelTemplateBox.Text = Global.AppSettings.LevelEditor.LevelTemplate;
        CaptureRadiusBox.Text = Global.AppSettings.LevelEditor.CaptureRadius.ToString();
        CheckTopologyWhenSavingBox.Checked = Global.AppSettings.LevelEditor.CheckTopologyWhenSaving;
        DynamicCheckTopologyBox.Checked = Global.AppSettings.LevelEditor.CheckTopologyDynamically;
        FilenameSuggestionBox.Checked = Global.AppSettings.LevelEditor.UseFilenameSuggestion;
        SameAsFilenameBox.Checked = Global.AppSettings.LevelEditor.UseFilenameForTitle;
        baseFilenameBox.Text = Global.AppSettings.LevelEditor.BaseFilename;
        numberFormatBox.Text = Global.AppSettings.LevelEditor.NumberFormat;
        DefaultTitleBox.Text = Global.AppSettings.LevelEditor.DefaultTitle;
        HighlightBox.Checked = Global.AppSettings.LevelEditor.UseHighlight;
        CheckForUpdatesBox.Checked = Global.AppSettings.General.CheckForUpdatesOnStartup;
        capturePicTextFromBordersCheckBox.Checked =
            Global.AppSettings.LevelEditor.CapturePicturesAndTexturesFromBordersOnly;
        if (Global.AppSettings.LevelEditor.RenderingSettings.DisableFrameBuffer &&
            Global.AppSettings.ReplayViewer.RenderingSettings.DisableFrameBuffer)
        {
            DisableFrameBufferUsageCheckBox.CheckState = CheckState.Checked;
        }
        else if (Global.AppSettings.LevelEditor.RenderingSettings.DisableFrameBuffer ||
                 Global.AppSettings.ReplayViewer.RenderingSettings.DisableFrameBuffer)
        {
            DisableFrameBufferUsageCheckBox.CheckState = CheckState.Indeterminate;
        }
        else
        {
            DisableFrameBufferUsageCheckBox.CheckState = CheckState.Unchecked;
        }

        alwaysSetDefaultsInPictureTool.Checked = Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool;
        startPositionFeatureCheckBox.Checked = Global.AppSettings.LevelEditor.EnableStartPositionFeature;
        FilenameSuggestionBoxCheckedChanged(null, null);
        SameAsFilenameBoxCheckedChanged(null, null);
    }

    private static string GetDefaultLgrFile(IList<string> lgrFiles)
    {
        if (Directory.Exists(Global.AppSettings.General.LgrDirectory))
        {
            string defaultlgr = Global.AppSettings.General.LgrDirectory + "\\Default.lgr";
            return File.Exists(defaultlgr) ? defaultlgr : lgrFiles[0];
        }

        return string.Empty;
    }

    private void BrowseForElmaDir(object sender, EventArgs e)
    {
        FolderBrowserDialog1.Description = "Browse for Elasto Mania directory";
        if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            if (Directory.Exists(FolderBrowserDialog1.SelectedPath + "\\Lev"))
            {
                LevTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\Lev";
                Global.AppSettings.General.LevelDirectory = LevTextBox.Text;
                _levelDirectoryChanged = true;
            }

            if (Directory.Exists(FolderBrowserDialog1.SelectedPath + "\\Rec"))
            {
                RecTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\Rec";
                Global.AppSettings.General.ReplayDirectory = RecTextBox.Text;
            }

            if (Directory.Exists(FolderBrowserDialog1.SelectedPath + "\\Lgr"))
            {
                LGRTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\Lgr";
                Global.AppSettings.General.LgrDirectory = LGRTextBox.Text;
            }
        }
    }

    private void BrowseLevelFolder(object sender, EventArgs e)
    {
        if (Directory.Exists(LevTextBox.Text))
            FolderBrowserDialog1.SelectedPath = LevTextBox.Text;
        FolderBrowserDialog1.Description = "Browse for level directory";
        if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            LevTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\";
            Global.AppSettings.General.LevelDirectory = LevTextBox.Text;
            _levelDirectoryChanged = true;
        }
    }

    private void BrowseLgrFolder(object sender, EventArgs e)
    {
        if (Directory.Exists(LGRTextBox.Text))
            FolderBrowserDialog1.SelectedPath = LGRTextBox.Text;
        FolderBrowserDialog1.Description = "Browse for LGR directory";
        if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            LGRTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\";
            Global.AppSettings.General.LgrDirectory = LGRTextBox.Text;
        }
    }

    private void BrowseReplayFolder(object sender, EventArgs e)
    {
        if (Directory.Exists(RecTextBox.Text))
            FolderBrowserDialog1.SelectedPath = RecTextBox.Text;
        FolderBrowserDialog1.Description = "Browse for replay directory";
        if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            RecTextBox.Text = FolderBrowserDialog1.SelectedPath + "\\";
            Global.AppSettings.General.ReplayDirectory = RecTextBox.Text;
        }
    }

    private void FilenameSuggestionBoxCheckedChanged(object? sender, EventArgs? e)
    {
        baseFilenameBox.Enabled = FilenameSuggestionBox.Checked;
        numberFormatBox.Enabled = FilenameSuggestionBox.Checked;
    }

    private void PanelClick(object sender, EventArgs e)
    {
        Panel clickedPanel = (Panel)sender;
        ColorDialog1.Color = clickedPanel.BackColor;
        if (ColorDialog1.ShowDialog() == DialogResult.OK)
            clickedPanel.BackColor = ColorDialog1.Color;
    }

    private void RenderingSettingsButtonClick(object sender, EventArgs e)
    {
        RenderingSettingsForm rSettingsForm =
            new RenderingSettingsForm(Global.AppSettings.LevelEditor.RenderingSettings);
        rSettingsForm.ShowDialog();
    }

    private void ResetButtonClick(object sender, EventArgs e)
    {
        if (
            MessageBox.Show("Reset all settings to default - are you sure?", "Elmanager", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Global.AppSettings = new ElmanagerSettings();
            Close();
        }
    }

    private void SameAsFilenameBoxCheckedChanged(object? sender, EventArgs? e)
    {
        DefaultTitleBox.Enabled = !SameAsFilenameBox.Checked;
    }

    private void SaveSettings(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.UserClosing)
            return;
        Global.AppSettings.ReplayManager.NitroReplays = NitroBox.Checked;
        Global.AppSettings.ReplayManager.ShowGridInList = ShowReplayListGridBox.Checked;
        Global.AppSettings.ReplayManager.LevDirSearchOption = SearchLevSubDirsBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        Global.AppSettings.ReplayManager.RecDirSearchOption = SearchRecSubDirsBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        Global.AppSettings.ReplayManager.ConfirmDelete = DeleteConfirmCheckBox.Checked;
        Global.AppSettings.ReplayManager.ShowTooltipInList = showTooltipForReplaysCheckBox.Checked;

        Global.AppSettings.LevelManager.ShowGridInList = lmShowGrid.Checked;
        Global.AppSettings.LevelManager.LevDirSearchOption = lmSearchLevSubDirs.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        Global.AppSettings.LevelManager.RecDirSearchOption = lmSearchRecSubDirs.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        Global.AppSettings.LevelManager.ConfirmDelete = lmConfirmDeletion.Checked;
        Global.AppSettings.LevelManager.ShowTooltipInList = lmShowTooltip.Checked;

        try
        {
            LevelEditorSettings.TryGetTemplateLevel(LevelTemplateBox.Text);
            Global.AppSettings.LevelEditor.LevelTemplate = LevelTemplateBox.Text;
        }
        catch (SettingsException settingsException)
        {
            UiUtils.ShowError(settingsException.Message);
        }

        Global.AppSettings.LevelEditor.CheckTopologyWhenSaving = CheckTopologyWhenSavingBox.Checked;
        Global.AppSettings.LevelEditor.CheckTopologyDynamically = DynamicCheckTopologyBox.Checked;
        Global.AppSettings.LevelEditor.UseHighlight = HighlightBox.Checked;
        Global.AppSettings.LevelEditor.UseFilenameSuggestion = FilenameSuggestionBox.Checked;
        Global.AppSettings.LevelEditor.UseFilenameForTitle = SameAsFilenameBox.Checked;
        Global.AppSettings.LevelEditor.BaseFilename = baseFilenameBox.Text;
        Global.AppSettings.LevelEditor.NumberFormat = numberFormatBox.Text;
        Global.AppSettings.LevelEditor.DefaultTitle = DefaultTitleBox.Text;
        Global.AppSettings.General.CheckForUpdatesOnStartup = CheckForUpdatesBox.Checked;
        Global.AppSettings.LevelEditor.CapturePicturesAndTexturesFromBordersOnly =
            capturePicTextFromBordersCheckBox.Checked;
        if (DisableFrameBufferUsageCheckBox.CheckState != CheckState.Indeterminate)
        {
            Global.AppSettings.LevelEditor.RenderingSettings.DisableFrameBuffer =
                DisableFrameBufferUsageCheckBox.Checked;
            Global.AppSettings.ReplayViewer.RenderingSettings.DisableFrameBuffer =
                DisableFrameBufferUsageCheckBox.Checked;
        }

        Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool = alwaysSetDefaultsInPictureTool.Checked;
        Global.AppSettings.LevelEditor.EnableStartPositionFeature = startPositionFeatureCheckBox.Checked;
        try
        {
            Global.AppSettings.LevelEditor.CaptureRadius = double.Parse(CaptureRadiusBox.Text);
        }
        catch (FormatException)
        {
            UiUtils.ShowError("Capture radius value was in an incorrect format!");
        }

        if (_levelDirectoryChanged)
            Global.ResetLevelFiles();
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
        OpenFileDialog1.Filter = "Elasto Mania levels|*.lev";
        OpenFileDialog1.CheckFileExists = true;
        if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
        {
            LevelTemplateBox.Text = OpenFileDialog1.FileName;
        }
    }

    private async void checkForUpdatesButton_Click(object sender, EventArgs e)
    {
        checkForUpdatesButton.Enabled = false;
        checkForUpdatesButton.Text = "Checking...";
        updateInfoLabel.Visible = false;
        var hasUpdates = await UpdateChecker.CheckForUpdates();
        checkForUpdatesButton.Enabled = true;
        checkForUpdatesButton.Text = "Check now";
        if (!hasUpdates)
        {
            updateInfoLabel.Visible = true;
        }
    }
}