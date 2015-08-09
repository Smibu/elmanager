using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace Elmanager.Forms
{
    partial class ConfigForm
    {
        private bool _levelDirectoryChanged;

        internal ConfigForm()
        {
            InitializeComponent();
            Utils.AddShieldToButton(generateNativeImageButton);
            LevTextBox.Text = Global.AppSettings.General.LevelDirectory;
            RecTextBox.Text = Global.AppSettings.General.ReplayDirectory;
            LGRTextBox.Text = Global.AppSettings.General.LgrDirectory;
            DeleteRecycleCheckBox.Checked = Global.AppSettings.ReplayManager.DelToRecycle;
            WarnAboutOldDBBox.Checked = Global.AppSettings.ReplayManager.WarnAboutOldDb;
            NitroBox.Checked = Global.AppSettings.ReplayManager.NitroReplays;
            ShowReplayListGridBox.Checked = Global.AppSettings.ReplayManager.ShowGridInReplayList;
            CheckBox6.Checked = Global.AppSettings.ReplayManager.UseDataBase;
            SearchLevSubDirsBox.Checked = Global.AppSettings.ReplayManager.SearchLevSubDirs;
            SearchRecSubDirsBox.Checked = Global.AppSettings.ReplayManager.SearchRecSubDirs;
            DeleteConfirmCheckBox.Checked = Global.AppSettings.ReplayManager.ConfirmDelete;
            DBTextBox.Text = Global.AppSettings.ReplayManager.DbFile;
            LevelTemplateBox.Text = Global.AppSettings.LevelEditor.LevelTemplate ?? "50,50";
            CaptureRadiusBox.Text = Global.AppSettings.LevelEditor.CaptureRadius.ToString();
            CheckTopologyWhenSavingBox.Checked = Global.AppSettings.LevelEditor.CheckTopologyWhenSaving;
            DynamicCheckTopologyBox.Checked = Global.AppSettings.LevelEditor.CheckTopologyDynamically;
            CheckBox6.Enabled = File.Exists(Global.AppSettings.ReplayManager.DbFile);
            LoadButton.Enabled = File.Exists(Global.AppSettings.ReplayManager.DbFile);
            FilenameSuggestionBox.Checked = Global.AppSettings.LevelEditor.UseFilenameSuggestion;
            SameAsFilenameBox.Checked = Global.AppSettings.LevelEditor.UseFilenameForTitle;
            baseFilenameBox.Text = Global.AppSettings.LevelEditor.BaseFilename;
            numberFormatBox.Text = Global.AppSettings.LevelEditor.NumberFormat;
            DefaultTitleBox.Text = Global.AppSettings.LevelEditor.DefaultTitle;
            HighlightBox.Checked = Global.AppSettings.LevelEditor.UseHighlight;
            CheckForUpdatesBox.Checked = Global.AppSettings.General.CheckForUpdatesOnStartup;
            HighlightPanel.BackColor = Global.AppSettings.LevelEditor.HighlightColor;
            SelectionPanel.BackColor = Global.AppSettings.LevelEditor.SelectionColor;
            crosshairPanel.BackColor = Global.AppSettings.LevelEditor.CrosshairColor;
            capturePicTextFromBordersCheckBox.Checked =
                Global.AppSettings.LevelEditor.CapturePicturesAndTexturesFromBordersOnly;
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

        private static void UpdateLgrDirsIfEmpty()
        {
            if (Directory.Exists(Global.AppSettings.General.LgrDirectory))
            {
                string[] lgrFiles = Directory.GetFiles(Global.AppSettings.General.LgrDirectory, "*.lgr",
                                                       SearchOption.AllDirectories);
                if (lgrFiles.Count() > 0)
                {
                    if (Global.AppSettings.LevelEditor.RenderingSettings.LgrFile == string.Empty)
                        Global.AppSettings.LevelEditor.RenderingSettings.LgrFile = GetDefaultLgrFile(lgrFiles);
                    if (Global.AppSettings.ReplayViewer.RenderingSettings.LgrFile == string.Empty)
                        Global.AppSettings.ReplayViewer.RenderingSettings.LgrFile = GetDefaultLgrFile(lgrFiles);
                }
            }
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
                    UpdateLgrDirsIfEmpty();
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
                UpdateLgrDirsIfEmpty();
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

        private void FilenameSuggestionBoxCheckedChanged(object sender, EventArgs e)
        {
            baseFilenameBox.Enabled = FilenameSuggestionBox.Checked;
            numberFormatBox.Enabled = FilenameSuggestionBox.Checked;
        }

        private void GenerateDataBase()
        {
            int totalSizeOfReplays = 0;
            if (DBTextBox.Text.Length == 0)
            {
                Utils.ShowError("Database file is not specified!");
                return;
            }
            if (Utils.LevRecDirectoriesExist())
            {
                Global.LevelFiles = Utils.GetLevelFiles(SearchLevSubDirsBox.Checked);
                string[] replayFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.*",
                                                          Global.AppSettings.ReplayManager.SearchRecSubDirs
                                                              ? SearchOption.AllDirectories
                                                              : SearchOption.TopDirectoryOnly);
                int replayCount = replayFiles.Length - 1;
                Global.ReplayDataBase = new List<Replay>();
                for (int i = 0; i <= replayCount; i++)
                {
                    Replay srp;
                    try
                    {
                        srp = new Replay(replayFiles[i]);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (srp.IsNitro)
                        continue;
                    Global.ReplayDataBase.Add(srp);
                    Label1.Text = "Gathering replay files... " + Math.Round(i/(double) replayCount*100.0) + "%";
                    Application.DoEvents();
                    totalSizeOfReplays += (int) (FileSystem.GetFileInfo(replayFiles[i]).Length);
                }
                BinaryFormatter binFormatter = new BinaryFormatter();
                MemoryStream fs = new MemoryStream();
                byte[] serializedData;
                Label1.Text = "Generating database...";
                Label1.Refresh();
                try
                {
                    fs.Capacity = totalSizeOfReplays/10;
                    binFormatter.Serialize(fs, Global.ReplayDataBase);
                    serializedData = fs.ToArray();
                }
                finally
                {
                    fs.Close();
                }
                File.WriteAllBytes(DBTextBox.Text, serializedData);
                CheckBox6.Enabled = File.Exists(DBTextBox.Text);
                Label1.Text = "Done!";
            }
            else
                Utils.ShowError(Constants.LevOrRecDirNotFound);
        }

        private void GenerateDataBaseClick(object sender, EventArgs e)
        {
            GenerateDataBase();
            //label9.Text = "Generating database...";
            //var x = new Action(GenerateDataBase);
            //x.BeginInvoke(z => label9.Text = "Done!", null);
        }

        private void GenerateNativeImage(object sender, EventArgs e)
        {
            Process p = new Process
                            {
                                StartInfo =
                                    {
                                        FileName = RuntimeEnvironment.GetRuntimeDirectory() + "ngen.exe",
                                        Arguments = "install \"" + Application.ExecutablePath + "\"",
                                        Verb = "runas"
                                    }
                            };
            try
            {
                p.Start();
            }
            catch (Win32Exception)
            {
            }
        }

        private void LoadButtonClick(object sender, EventArgs e)
        {
            Global.AppSettings.ReplayManager.DbFile = DBTextBox.Text;
            Label1.Text = "Loading database...";
            Refresh();
            Label1.Text = Utils.LoadDataBase() ? "Done!" : "Failed to load database!";
        }

        private void PanelClick(object sender, EventArgs e)
        {
            Panel clickedPanel = (Panel) sender;
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

        private void SameAsFilenameBoxCheckedChanged(object sender, EventArgs e)
        {
            DefaultTitleBox.Enabled = !SameAsFilenameBox.Checked;
        }

        private void SaveSettings(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            Global.AppSettings.ReplayManager.DelToRecycle = DeleteRecycleCheckBox.Checked;
            Global.AppSettings.ReplayManager.NitroReplays = NitroBox.Checked;
            Global.AppSettings.ReplayManager.ShowGridInReplayList = ShowReplayListGridBox.Checked;
            Global.AppSettings.ReplayManager.UseDataBase = CheckBox6.Checked;
            Global.AppSettings.ReplayManager.SearchLevSubDirs = SearchLevSubDirsBox.Checked;
            Global.AppSettings.ReplayManager.SearchRecSubDirs = SearchRecSubDirsBox.Checked;
            Global.AppSettings.ReplayManager.DbFile = DBTextBox.Text;
            Global.AppSettings.ReplayManager.ConfirmDelete = DeleteConfirmCheckBox.Checked;
            Global.AppSettings.ReplayManager.WarnAboutOldDb = WarnAboutOldDBBox.Checked;

            try
            {
                ElmanagerSettings.LevelEditorSettings.TryGetTemplateLevel(LevelTemplateBox.Text);
                Global.AppSettings.LevelEditor.LevelTemplate = LevelTemplateBox.Text;
            }
            catch (SettingsException settingsException)
            {
                Utils.ShowError(settingsException.Message);
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
            Global.AppSettings.LevelEditor.HighlightColor = HighlightPanel.BackColor;
            Global.AppSettings.LevelEditor.SelectionColor = SelectionPanel.BackColor;
            Global.AppSettings.LevelEditor.CrosshairColor = crosshairPanel.BackColor;
            Global.AppSettings.LevelEditor.CapturePicturesAndTexturesFromBordersOnly =
                capturePicTextFromBordersCheckBox.Checked;
            try
            {
                Global.AppSettings.LevelEditor.CaptureRadius = double.Parse(CaptureRadiusBox.Text);
            }
            catch (FormatException)
            {
                Utils.ShowError("Capture radius value was in an incorrect format!");
            }
            if (_levelDirectoryChanged)
                Global.LevelFiles = Utils.GetLevelFiles();
        }

        private void SetDbPath(object sender, EventArgs e)
        {
            if (DBTextBox.Text.Length > 0)
                OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(DBTextBox.Text);
            OpenFileDialog1.CheckFileExists = false;
            OpenFileDialog1.Filter = "Replay databases|*.rdb";
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBTextBox.Text = OpenFileDialog1.FileName;
                CheckBox6.Enabled = File.Exists(DBTextBox.Text);
                LoadButton.Enabled = File.Exists(DBTextBox.Text);
                if (!CheckBox6.Enabled)
                    CheckBox6.Checked = false;
            }
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
    }
}