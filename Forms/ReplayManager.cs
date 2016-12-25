using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Elmanager.CustomControls;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace Elmanager.Forms
{
    partial class ReplayManager
    {
        private const string NoReplaysSelected = "No replays are selected!";
        private readonly int[] _boolIndices = {2, 3, 4, 8};
        private List<string> _allLevelFiles;
        private bool _cellEditing;
        private ReplayViewer _currentViewer;
        private string _oldText;
        private bool _searchInProgress;

        internal ReplayManager()
        {
            InitializeComponent();
            MemberInfo[] replayMembers = typeof (Replay).GetMembers(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i <= 8; i++)
            {
                RList.GetColumn(i).AspectName = replayMembers[i + 6].Name;
                RList.GetColumn(i).Text =
                    ((DescriptionAttribute)
                     replayMembers[i + 6].GetCustomAttributes(typeof (DescriptionAttribute), false)[0]).Description;
            }
            RList.GetColumn(0).AspectToStringConverter = GetFileNameWithoutExtension;
            RList.GetColumn(5).AspectToStringConverter =
                Utils.GetPossiblyInternal;
            RList.GetColumn(7).AspectToStringConverter = Utils.ToTimeString;
            foreach (int i in _boolIndices)
                RList.GetColumn(i).AspectToStringConverter = Utils.BoolToString;
            RList.GetColumn(6).AspectToStringConverter = Utils.SizeToString;
            if (Global.AppSettings.ReplayManager.ShowTooltipForReplays)
            {
                RList.CellToolTipGetter = ShowReplayToolTip;
                RList.CellToolTip.IsBalloon = true;
                RList.CellToolTip.AutoPopDelay = 30000;
                RList.CellToolTip.InitialDelay = 1000;
                RList.CellToolTip.ReshowDelay = 0;
                RList.CellToolTip.Title = "Details";
            }
            if (Global.AppSettings.ReplayManager.ReplayListState != null)
            {
                RList.RestoreState(Global.AppSettings.ReplayManager.ReplayListState);
            }
            PatternBox.Text = Global.AppSettings.ReplayManager.SearchPattern;
            Location = Global.AppSettings.ReplayManager.Location;
            Size = Global.AppSettings.ReplayManager.Size;
            RList.GridLines = Global.AppSettings.ReplayManager.ShowGridInReplayList;
            if (Directory.Exists(Global.AppSettings.ReplayManager.MoveToPath))
                FolderBrowserDialog1.SelectedPath = Global.AppSettings.ReplayManager.MoveToPath;
            WindowState = Global.AppSettings.ReplayManager.WindowState;
            DisplaySelectionInfo();
        }

        internal string Rename(Replay replay, string newName, bool allowSameName = true)
        {
            if (replay.FileName.ToLower() == newName.ToLower() + Constants.RecExtension && !allowSameName)
                return null;
            string T = string.Empty;
            string recDir = Path.GetDirectoryName(replay.Path) + "\\";
            if (File.Exists(recDir + newName + Constants.RecExtension))
            {
                int x = 98;
                while (File.Exists(recDir + newName + char.ConvertFromUtf32(x) + Constants.RecExtension))
                    x++;
                T = char.ConvertFromUtf32(x);
            }
            try
            {
                string fileName = newName + T;
                FileSystem.RenameFile(replay.Path, fileName + Constants.RecExtension);
                replay.FileName = fileName;
                RList.RefreshObject(replay);
                return fileName;
            }
            catch (ArgumentException)
            {
                Utils.ShowError("Invalid file name!");
                return null;
            }
            catch (FileNotFoundException)
            {
                Utils.ShowError("File " + replay.Path + " doesn\'t exist!");
                return null;
            }
        }

        private static string GetFileNameWithoutExtension(object path)
        {
            return Path.GetFileNameWithoutExtension((string) path);
        }

        private static string[] GetMatches(IEnumerable<string> replays, string pattern)
        {
            Regex replayFileMatcher;
            try
            {
                replayFileMatcher = new Regex(pattern, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                replayFileMatcher = new Regex(string.Empty, RegexOptions.IgnoreCase);
            }
            return replays.Where(x => replayFileMatcher.IsMatch(Path.GetFileNameWithoutExtension(x))).ToArray();
        }

        private static RSearchOption GetSearchParamFromOption(TriSelect select)
        {
            switch (select.SelectedOption)
            {
                case 0:
                    return RSearchOption.True;
                case 1:
                    return RSearchOption.False;
                default:
                    return RSearchOption.Dontcare;
            }
        }

        private static bool IsDiffLevel(IList<Replay> replays)
        {
            string levFileName = replays[0].LevelFilename;
            return replays.Any(x => !x.LevelFilename.CompareWith(levFileName));
        }

        private static string ShowReplayToolTip(OLVColumn col, object x)
        {
            var rep = (Replay) x;
            string toolTipStr = "Replay path: " + rep.Path;
            if (rep.LevelExists && !rep.IsInternal)
                toolTipStr += "\r\n" + "Level path: " + rep.LevelPath;
            toolTipStr += "\r\n" + "\r\n" + "---Player 1---" + "\r\n" + Utils.GetPlayerInfoStr(rep.Player1);
            if (rep.IsMulti)
                toolTipStr += "\r\n" + "\r\n" + "---Player 2---" + "\r\n" + Utils.GetPlayerInfoStr(rep.Player2);
            return toolTipStr;
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (!e.Cancel)
            {
                var x = (Replay) e.RowObject;
                string newName = Rename(x, e.Control.Text, false);
                if (newName != null)
                    e.Control.Text = newName;
            }
            _cellEditing = false;
        }

        private void CellEditStarting(object sender, CellEditEventArgs e)
        {
            e.Control.Text = Path.GetFileNameWithoutExtension(e.Control.Text);
            _cellEditing = true;
        }

        private void ChangeTotalTimeDisplay(object sender, MouseEventArgs e)
        {
            Global.AppSettings.ReplayManager.Decimal3Shown = !Global.AppSettings.ReplayManager.Decimal3Shown;
            DisplaySelectionInfo();
        }

        private void Compare(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count <= 1)
            {
                Utils.ShowError("At least two replays must be selected!");
                return;
            }
            if (!IsDiffLevelOrMulti())
            {
                var cf = new CompareForm(GetPropertiesOfSelected());
                cf.Show();
            }
            else
                Utils.ShowError(
                    "The selected replays must have the same level and they must be singleplayer replays!");
        }

        private void DeleteReplays(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count == 0)
            {
                Utils.ShowError(NoReplaysSelected);
                return;
            }
            if (!_cellEditing &&
                (!Global.AppSettings.ReplayManager.ConfirmDelete ||
                 MessageBox.Show("Are you sure you want to delete the selected replays?", "Warning",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                                 0, false) == DialogResult.Yes))
            {
                RecycleOption recycleOpt = Global.AppSettings.ReplayManager.DelToRecycle
                                               ? RecycleOption.SendToRecycleBin
                                               : RecycleOption.DeletePermanently;
                ToolStripProgressBar1.Maximum = RList.SelectedObjects.Count;
                ToolStripProgressBar1.Value = 0;
                foreach (Replay x in RList.SelectedObjects)
                {
                    try
                    {
                        statusLabel.Text = "Deleting " + x.FileName;
                        ToolStripProgressBar1.PerformStep();
                        StatusStrip1.Refresh();
                        FileSystem.DeleteFile(x.Path, UIOption.OnlyErrorDialogs, recycleOpt);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
                RemoveReplays();
                statusLabel.Text = "Ready";
                ToolStripProgressBar1.Value = 0;
            }
        }

        private void DisplayConfiguration(object sender, EventArgs e)
        {
            string oldLevDir = Global.AppSettings.General.LevelDirectory;
            ComponentManager.ShowConfiguration();
            RList.GridLines = Global.AppSettings.ReplayManager.ShowGridInReplayList;
            if (oldLevDir != Global.AppSettings.General.LevelDirectory)
            {
                _allLevelFiles = null;
            }
        }

        private void DisplaySelectionInfo()
        {
            SelectedReplaysLabel.Text = RList.SelectedObjects.Count + " of " + RList.Items.Count +
                                        " replays, total time: ";
            if (Global.AppSettings.ReplayManager.Decimal3Shown)
                SelectedReplaysLabel.Text += GetTotalTimeOfSelected().ToTimeString();
            else
                SelectedReplaysLabel.Text += GetTotalTimeOfSelected().ToTimeString(2);
        }

        private void DuplicateFilenameSearch(object sender, EventArgs e)
        {
            if (Utils.LevRecDirectoriesExist())
            {
                string[] replayFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                                                          SearchOption.AllDirectories);
                replayFiles = GetMatches(replayFiles, "");
                var replayFilesNoPaths = new string[replayFiles.Length];
                RList.ClearObjects();
                statusLabel.Text = "Searching...";
                Refresh();
                for (int i = 0; i < replayFiles.Length; i++)
                    replayFilesNoPaths[i] = Path.GetFileName(replayFiles[i]).ToLower();
                for (int i = 0; i < replayFiles.Length - 1; i++)
                {
                    for (int j = i + 1; j < replayFiles.Length; j++)
                    {
                        if (replayFilesNoPaths[i] != replayFilesNoPaths[j]) continue;
                        var dupRp = new Replay(replayFiles[i]);
                        if (!ReplayListContains(dupRp))
                            RList.AddObject(dupRp);
                        dupRp = new Replay(replayFiles[j]);
                        if (!ReplayListContains(dupRp))
                            RList.AddObject(dupRp);
                    }
                }
                statusLabel.Text = "Ready";
                DisplaySelectionInfo();
            }
            else
                Utils.ShowError(Constants.LevOrRecDirNotFound);
        }

        private void DuplicateReplaySearch(object sender, EventArgs e)
        {
            var replayLengths = new int[] {};
            if (Utils.LevRecDirectoriesExist())
            {
                RList.ClearObjects();
                string[] replayFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                                                          Global.AppSettings.ReplayManager.SearchRecSubDirs
                                                              ? SearchOption.AllDirectories
                                                              : SearchOption.TopDirectoryOnly);
                replayFiles = GetMatches(replayFiles, "");
                statusLabel.Text = "Searching...";
                Refresh();
                Array.Resize(ref replayLengths, replayFiles.Length);
                for (int i = 0; i < replayFiles.Length; i++)
                    replayLengths[i] = (int) (FileSystem.GetFileInfo(replayFiles[i]).Length);
                for (int i = 0; i < replayFiles.Length - 1; i++)
                {
                    for (int j = i + 1; j < replayFiles.Length; j++)
                    {
                        if (replayLengths[i] != replayLengths[j]) continue;
                        bool isDifferent = false;
                        byte[] replay = File.ReadAllBytes(replayFiles[i]);
                        byte[] replayToCompare = File.ReadAllBytes(replayFiles[j]);
                        for (int k = 0; k < replayLengths[i]; k++)
                        {
                            if (replay[k] == replayToCompare[k]) continue;
                            isDifferent = true;
                            break;
                        }
                        if (isDifferent) continue;
                        var dupRp = new Replay(replayFiles[i]);
                        if (!ReplayListContains(dupRp))
                            RList.AddObject(dupRp);
                        dupRp = new Replay(replayFiles[j]);
                        if (!ReplayListContains(dupRp))
                            RList.AddObject(dupRp);
                    }
                }
                DisplaySelectionInfo();
                statusLabel.Text = "Ready";
            }
            else
                Utils.ShowError(Constants.LevOrRecDirNotFound);
        }

        private Replay[] GetPropertiesOfSelected()
        {
            var replaysProperties = new Replay[RList.SelectedObjects.Count];
            for (int i = 0; i < RList.SelectedObjects.Count; i++)
                replaysProperties[i] = (Replay) (RList.SelectedObjects[i]);
            return replaysProperties;
        }

        private Replay GetSelectedReplay()
        {
            return (Replay) (RList.SelectedObjects[0]);
        }

        private byte[][] GetSelectedReplaysToArray()
        {
            var replays = new byte[RList.SelectedObjects.Count][];
            for (int i = 0; i < RList.SelectedObjects.Count; i++)
                replays[i] = File.ReadAllBytes(((Replay) (RList.SelectedObjects[i])).Path);
            return replays;
        }

        private double GetTotalTimeOfSelected()
        {
            double totalTime = 0;
            if (Global.AppSettings.ReplayManager.Decimal3Shown)
                totalTime += RList.SelectedObjects.Cast<Replay>().Sum(x => x.Time);
            else
                totalTime += RList.SelectedObjects.Cast<Replay>().Sum(x => Math.Floor(x.Time * 100) / 100);
            return totalTime;
        }

        private void InvertSelection(object sender, EventArgs e)
        {
            foreach (ListViewItem x in RList.Items)
                x.Selected = !x.Selected;
        }

        private bool IsDiffLevelOrMulti()
        {
            var firstReplay = GetSelectedReplay();
            return
                RList.SelectedObjects.Cast<Replay>().Any(x => x.IsMulti || !x.LevelFilename.CompareWith(firstReplay.LevelFilename));
        }

        private void K10XnetToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                string replayFile = GetSelectedReplay().Path;
                UploadForm u = new UploadForm();
                u.UploadTok10X(replayFile);
            }
            else
                Utils.ShowError(NoReplaysSelected);
        }

        private void MergeReplays(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count == 2)
            {
                if (IsDiffLevelOrMulti())
                {
                    Utils.ShowError(
                        "The selected replays must have the same level and they must be singleplayer replays.");
                    return;
                }
                byte[][] replays;
                try
                {
                    replays = GetSelectedReplaysToArray();
                }
                catch (FileNotFoundException ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
                var newReplay = new byte[replays[0].Length + replays[1].Length];
                Buffer.BlockCopy(replays[0], 0, newReplay, 0, replays[0].Length);
                Buffer.BlockCopy(replays[1], 0, newReplay, replays[0].Length, replays[1].Length);
                //Set the two bytes in NewReplay to tell it's multiplayer replay
                newReplay[8] = 1;
                newReplay[replays[0].Length + 8] = 1;
                SaveFileDialog1.DefaultExt = "rec";
                SaveFileDialog1.Filter = "Elasto Mania replay (*.rec)|*.rec";
                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(SaveFileDialog1.FileName, newReplay);
            }
            else
                Utils.ShowError("Two replays must be selected!");
        }

        private void MoveOrCopy(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count == 0)
            {
                Utils.ShowError(NoReplaysSelected);
                return;
            }
            bool move = sender.Equals(MoveToToolStripMenuItem);
            FolderBrowserDialog1.Description = move ? "Move to..." : "Copy to...";
            if (FolderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            foreach (Replay replay in RList.SelectedObjects)
            {
                try
                {
                    if (move)
                    {
                        FileSystem.MoveFile(replay.Path, FolderBrowserDialog1.SelectedPath + "\\" + replay.FileName,
                                            true);
                        replay.Path = FolderBrowserDialog1.SelectedPath + "\\" + replay.FileName;
                    }
                    else
                        FileSystem.CopyFile(replay.Path, FolderBrowserDialog1.SelectedPath + "\\" + replay.FileName,
                                            true);
                }
                catch (FileNotFoundException ex)
                {
                    Utils.ShowError(ex.Message);
                }
            }
        }

        private void OpenLevelMenuItemClick(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                var selectedReplay = GetSelectedReplay();
                if (!selectedReplay.IsInternal)
                {
                    string levelFile = selectedReplay.LevelFilename;
                    if (_allLevelFiles == null)
                    {
                        _allLevelFiles = Utils.GetLevelFiles(true);
                    }
                    if (!selectedReplay.LevelExists)
                    {
                        Utils.ShowError("Level file doesn\'t exist!");
                        return;
                    }
                    int i;
                    for (i = 0; i < _allLevelFiles.Count(); i++)
                        if (Path.GetFileName(_allLevelFiles[i]).CompareWith(levelFile))
                            break;
                    Process.Start(_allLevelFiles[i]);
                }
                else
                {
                    Utils.ShowError("Cannot open level file from internal replays!");
                }
            }
            else
                Utils.ShowError("No replay is selected!");
        }

        private void OpenViewer(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                var rps = new List<Replay>();
                try
                {
                    rps.AddRange(RList.SelectedObjects.Cast<Replay>());
                }
                catch (FileNotFoundException ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
                if (!IsDiffLevel(rps))
                {
                    if (rps[0].LevelExists)
                    {
                        Cursor = Cursors.WaitCursor;
                        rps.ForEach(x => x.InitializeFrameData());
                        try
                        {
                            if (_currentViewer != null && !_currentViewer.IsDisposed)
                                _currentViewer.SetReplays(rps);
                            else
                            {
                                _currentViewer = new ReplayViewer(rps);
                                _currentViewer.Show();
                            }
                        }
                        catch (FileFormatException ex)
                        {
                            Utils.ShowError("An error occurred when loading replay viewer. Exception text:" + "\r\n" +
                                                ex.Message);
                        }
                        Cursor = Cursors.Default;
                    }
                    else
                        Utils.ShowError("Level for the replays was not found!");
                }
                else
                    Utils.ShowError("The replays must have the same level!");
            }
            else if (!sender.Equals(RList))
                Utils.ShowError(NoReplaysSelected);
        }

        private void RemoveReplays(object sender=null, EventArgs e=null)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                if (!_cellEditing)
                {
                    int index = RList.SelectedIndices[0];
                    RList.RemoveObjects(RList.SelectedObjects);
                    if (RList.Items.Count > 0)
                    {
                        if (index >= RList.Items.Count)
                        {
                            index--;
                        }
                        RList.Items[index].Selected = true;
                    }
                    DisplaySelectionInfo();
                }
            }
            else
                Utils.ShowError(NoReplaysSelected);
        }

        private void Rename(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count == 1)
            {
                RList.CellEditActivation = ObjectListView.CellEditActivateMode.F2Only;
                RList.EditSubItem(RList.SelectedItem, 0);
                RList.CellEditActivation = ObjectListView.CellEditActivateMode.None;
            }
            else
                Utils.ShowError("One replay must be selected!");
        }

        private void RenamePattern(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                var rForm = new RenameForm(RList.SelectedObjects, this)
                                {Location = new Point(MousePosition.X, MousePosition.Y)};
                rForm.ShowDialog();
            }
            else
                Utils.ShowError(NoReplaysSelected);
        }

        private bool ReplayListContains(Replay rp)
        {
            return RList.Objects != null && RList.Objects.Cast<Replay>().Any(x => x.Path == rp.Path);
        }

        private void ReplaylistSelectionChanged(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                if (_currentViewer != null && !_currentViewer.IsDisposed)
                {
                    Cursor = Cursors.WaitCursor;
                    var selectedRp = GetSelectedReplay();
                    selectedRp.InitializeFrameData();
                    if (selectedRp.LevelExists)
                    {
                        _currentViewer.SetReplays(selectedRp);
                    }
                    Cursor = Cursors.Default;
                }
            }

            DisplaySelectionInfo();
        }

        private void ResetFields(object sender, EventArgs e)
        {
            for (int i = 1; i <= 12; i++)
            {
                TabControl1.TabPages[1].Controls["TextBox" + i].Text = "0";
                TabControl1.TabPages[1].Controls["TextBox" + (i + 12)].Text = "10000";
            }
        }

        private void ResizeControls(object sender, EventArgs e)
        {
            ToolStripProgressBar1.ProgressBar.Width = (int) (Width * 0.6);
        }

        private void SaveListToTextFile(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                SaveFileDialog1.DefaultExt = "txt";
                SaveFileDialog1.Filter = "Text document (*.txt)|*.txt";
                if (SaveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                statusLabel.Text = "Saving... ";
                StatusStrip1.Refresh();
                const int columnWidth = 2;
                StreamWriter file = new StreamWriter(SaveFileDialog1.FileName);
                file.WriteLine(RList.SelectedObjects.Count + " replays");
                file.WriteLine();
                //Find out order of columns
                var columnOrder = new int[RList.Columns.Count];
                for (int i = 0; i < columnOrder.Length; i++)
                {
                    for (int j = 0; j < columnOrder.Length; j++)
                    {
                        if (RList.Columns[j].DisplayIndex != i) continue;
                        columnOrder[i] = j;
                        break;
                    }
                }
                //Get maximum text length for each column
                var maxColTextLength = new int[columnOrder.Length];
                for (int i = 0; i < columnOrder.Length; i++)
                {
                    maxColTextLength[i] = 0;
                    foreach (ListViewItem j in RList.SelectedItems)
                        if (maxColTextLength[i] < j.SubItems[columnOrder[i]].Text.Length)
                            maxColTextLength[i] = j.SubItems[columnOrder[i]].Text.Length;
                }
                for (int i = 0; i < columnOrder.Length; i++)
                    if (maxColTextLength[i] > 0)
                        file.Write(" - " + RList.Columns[columnOrder[i]].Text);
                file.Write(" - ");
                file.WriteLine();
                ToolStripProgressBar1.Value = 0;
                ToolStripProgressBar1.Maximum = RList.SelectedObjects.Count;
                foreach (ListViewItem i in RList.SelectedItems)
                {
                    for (int j = 0; j < columnOrder.Length; j++)
                    {
                        if (maxColTextLength[j] <= 0) continue;
                        file.Write(i.SubItems[columnOrder[j]].Text);
                        for (int k = 0; k < maxColTextLength[j] - i.SubItems[columnOrder[j]].Text.Length; k++)
                            file.Write(' ');
                        if (j < columnOrder.Length - 1)
                            for (int k = 0; k < columnWidth; k++)
                                file.Write(' ');
                    }
                    file.WriteLine();
                    ToolStripProgressBar1.Value++;
                }
                file.WriteLine();
                file.Write("Total time: ");
                double totalTime = GetTotalTimeOfSelected();
                file.Write(totalTime.ToTimeString().Substring(0,
                                                              totalTime.ToTimeString().Length -
                                                              Utils.BooleanToInteger(
                                                                  !Global.AppSettings.ReplayManager.Decimal3Shown)));

                statusLabel.Text = "Ready";
                ToolStripProgressBar1.Value = 0;
                file.Close();
            }
            else
                Utils.ShowError(NoReplaysSelected);
        }

        private void SaveSettings(object sender, FormClosingEventArgs e)
        {
            Global.AppSettings.ReplayManager.SearchPattern = PatternBox.Text;
            Global.AppSettings.ReplayManager.Location = Location;
            Global.AppSettings.ReplayManager.Size = Size;
            Global.AppSettings.ReplayManager.WindowState = WindowState;
            Global.AppSettings.ReplayManager.ReplayListState = RList.SaveState();
        }

        private void SelectAll(object sender, EventArgs e)
        {
            RList.SelectAll();
        }

        private void ToggleSearch(object sender, EventArgs e)
        {
            if (_searchInProgress)
            {
                _searchInProgress = false;
                return;
            }
            string searchPattern = PatternBox.Text;
            string levelPattern = LevPatternBox.Text;
            var replayFiles = new string[] {};
            var errorFiles = new List<string>();
            bool nonExistantReplaysFound = false;
            List<Replay> replayDataBaseMatches = null;

            bool searchForFastestReplays;
            bool searchForAllReplays;
            var clickedButton = (Button) sender;
            if (clickedButton.Equals(SearchButton))
            {
                searchForAllReplays = fastestSlowestSelect.SelectedOption == 2;
                searchForFastestReplays = fastestSlowestSelect.SelectedOption == 0;
            }
            else
            {
                searchForAllReplays = true;
                searchForFastestReplays = false;
            }
            bool searchOnlyMissingLev = clickedButton.Equals(ReplaysWithoutLevFileButton);
            bool searchOnlyWrongLev = clickedButton.Equals(ReplaysIncorrectLevButton);

            if (!Utils.RecDirectoryExists())
            {
                Utils.ShowError(Constants.RecDirNotFound);
                return;
            }
            var p1Apples = new Bound<int>(TextBox1.ValueAsInt, TextBox14.ValueAsInt);
            var p2Apples = new Bound<int>(TextBox3.ValueAsInt, TextBox16.ValueAsInt);
            var p1Turns = new Bound<int>(TextBox2.ValueAsInt, TextBox23.ValueAsInt);
            var p2Turns = new Bound<int>(TextBox10.ValueAsInt, TextBox15.ValueAsInt);
            var p1Gt = new Bound<int>(TextBox11.ValueAsInt, TextBox24.ValueAsInt);
            var p2Gt = new Bound<int>(TextBox12.ValueAsInt, TextBox13.ValueAsInt);
            var p1Sv = new Bound<int>(TextBox4.ValueAsInt, TextBox21.ValueAsInt);
            var p2Sv = new Bound<int>(TextBox9.ValueAsInt, TextBox22.ValueAsInt);
            var p1Rv = new Bound<int>(TextBox6.ValueAsInt, TextBox19.ValueAsInt);
            var p2Rv = new Bound<int>(TextBox7.ValueAsInt, TextBox20.ValueAsInt);
            var p1Lv = new Bound<int>(TextBox8.ValueAsInt, TextBox17.ValueAsInt);
            var p2Lv = new Bound<int>(TextBox5.ValueAsInt, TextBox18.ValueAsInt);
            Bound<double> time;
            try
            {
                time = new Bound<double>(Utils.StringToTime(TimeMinBox.Text),
                                         Utils.StringToTime(TimeMaxBox.Text));
            }
            catch (Exception)
            {
                Utils.ShowError("Time bound is in wrong format. Make sure it is 9 characters long.");
                return;
            }

            Bound<int> size = new Bound<int>((int) minFileSizeBox.Value * 1024, (int) maxFileSizeBox.Value * 1024);

            Regex levFilenameMatcher;
            try
            {
                levFilenameMatcher = new Regex(levelPattern, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                levFilenameMatcher = new Regex(String.Empty, RegexOptions.IgnoreCase);
            }
            var searchParams = new SearchParameters
                                   {
                                       InternalRec = GetSearchParamFromOption(intExtSelect),
                                       AcrossLev = GetSearchParamFromOption(elmaAcrossSelect),
                                       Date = new Bound<DateTime>(minDateTime.Value, maxDateTime.Value),
                                       Finished = GetSearchParamFromOption(finishedSelect),
                                       LevExists = RSearchOption.Dontcare,
                                       WrongLev = RSearchOption.Dontcare,
                                       LevFilenameMatcher = levFilenameMatcher,
                                       MultiPlayer = GetSearchParamFromOption(singleMultiSelect),
                                       Size = size,
                                       Time = time,
                                       P1Bounds =
                                           new SearchParameters.PlayerBounds
                                               {
                                                   Apples = p1Apples,
                                                   GroundTouches = p1Gt,
                                                   LeftVolts = p1Lv,
                                                   RightVolts = p1Rv,
                                                   SuperVolts = p1Sv,
                                                   Turns = p1Turns
                                               },
                                       P2Bounds =
                                           new SearchParameters.PlayerBounds
                                               {
                                                   Apples = p2Apples,
                                                   GroundTouches = p2Gt,
                                                   LeftVolts = p2Lv,
                                                   RightVolts = p2Rv,
                                                   SuperVolts = p2Sv,
                                                   Turns = p2Turns
                                               }
                                   };
            if (searchOnlyMissingLev)
            {
                searchParams.ResetOptions();
                searchParams.LevExists = RSearchOption.False;
            }
            if (searchOnlyWrongLev)
            {
                searchParams.ResetOptions();
                searchParams.WrongLev = RSearchOption.True;
            }
            int i;
            int replayCount;
            if (!Global.AppSettings.ReplayManager.UseDataBase)
            {
                replayFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                                                 Global.AppSettings.ReplayManager.SearchRecSubDirs
                                                     ? SearchOption.AllDirectories
                                                     : SearchOption.TopDirectoryOnly);
                if (!(searchOnlyMissingLev || searchOnlyWrongLev))
                    replayFiles = GetMatches(replayFiles, searchPattern);
                replayCount = replayFiles.Count();
            }
            else
            {
                if (Global.ReplayDataBase == null)
                {
                    statusLabel.Text = "Loading database...";
                    Refresh();
                    if (!Utils.LoadDataBase())
                        return;
                }
                replayDataBaseMatches = new List<Replay>();
                Regex replayFileMatcher;
                try
                {
                    replayFileMatcher = new Regex(PatternBox.Text, RegexOptions.IgnoreCase);
                }
                catch (Exception)
                {
                    replayFileMatcher = new Regex(string.Empty, RegexOptions.IgnoreCase);
                }
                for (i = 0; i < Global.ReplayDataBase.Count; i++)
                    if (replayFileMatcher.IsMatch(Path.GetFileNameWithoutExtension(Global.ReplayDataBase[i].FileName)))
                        replayDataBaseMatches.Add(Global.ReplayDataBase[i]);
                replayCount = replayDataBaseMatches.Count;
            }
            RList.ClearObjects();
            string oldButtonText = clickedButton.Text;
            _searchInProgress = true;
            DuplicateButton.Enabled = false;
            DuplicateFilenameButton.Enabled = false;
            ConfigButton.Enabled = false;
            clickedButton.Text = "Stop";
            var foundReplays = new List<Replay>();
            ToolStripProgressBar1.Maximum = replayCount;
            var recsByLevel = new List<ReplaysByLevel>();
            for (i = 0; i < replayCount; i++)
            {
                Replay srp;
                try
                {
                    if (Global.AppSettings.ReplayManager.UseDataBase)
                    {
                        srp = replayDataBaseMatches[i];
                        if (!File.Exists(srp.Path))
                            nonExistantReplaysFound = true;
                    }
                    else
                        srp = new Replay(replayFiles[i]);
                }
                catch (ArgumentException)
                {
                    errorFiles.Add(replayFiles[i]);
                    continue;
                }
                if (srp.IsNitro)
                {
                    if (Global.AppSettings.ReplayManager.NitroReplays)
                        errorFiles.Add(replayFiles[i]);
                    continue;
                }

                if (searchParams.Matches(srp))
                {
                    if (!searchForAllReplays)
                    {
                        bool match = false;
                        for (int j = 0; j < recsByLevel.Count; j++)
                        {
                            if (!recsByLevel[j].Level.CompareWith(srp.LevelFilename))
                                continue;
                            if (recsByLevel[j].Replays[0].IsMulti != srp.IsMulti)
                                continue;
                            recsByLevel[j].Replays.Add(srp);
                            match = true;
                            break;
                        }
                        if (!match)
                            recsByLevel.Add(new ReplaysByLevel(srp));
                    }
                    else
                        foundReplays.Add(srp);
                }

                ToolStripProgressBar1.Value = i;
                StatusStrip1.Refresh();
                if (!searchForAllReplays)
                    statusLabel.Text = "Phase 1: " + recsByLevel.Count + " levels";
                else
                    statusLabel.Text = "Found " + foundReplays.Count + " replays so far";
                Application.DoEvents();
                if (!_searchInProgress)
                    break;
            }
            if (!searchForAllReplays)
            {
                ToolStripProgressBar1.Maximum = recsByLevel.Count;
                for (i = 0; i < recsByLevel.Count; i++)
                {
                    Replay matchedReplay = recsByLevel[i].Replays[0];
                    foreach (Replay z in recsByLevel[i].Replays)
                    {
                        if (z.WrongLevelVersion)
                            continue;
                        if (searchForFastestReplays)
                        {
                            if (!matchedReplay.Finished && z.Finished)
                                matchedReplay = z;
                            else if (matchedReplay.Finished && z.Finished && z.Time < matchedReplay.Time)
                                matchedReplay = z;
                            else if (!matchedReplay.Finished && !z.Finished &&
                                     z.Player1.Apples > matchedReplay.Player1.Apples)
                                matchedReplay = z;
                        }
                        else
                        {
                            if (matchedReplay.Finished && !z.Finished)
                                matchedReplay = z;
                            else if (matchedReplay.Finished && z.Finished && z.Time > matchedReplay.Time)
                                matchedReplay = z;
                            else if (!matchedReplay.Finished && !z.Finished &&
                                     (z.Player1.Apples < matchedReplay.Player1.Apples || matchedReplay.Time > z.Time))
                                matchedReplay = z;
                        }
                    }
                    if (searchForFastestReplays || recsByLevel[i].Replays.Count > 1)
                        foundReplays.Add(matchedReplay);
                    statusLabel.Text = "Phase 2: " + foundReplays.Count + " replays";
                    ToolStripProgressBar1.Value = i;
                    Application.DoEvents();
                    if (!_searchInProgress)
                        break;
                }
            }
            RList.SetObjects(foundReplays);
            _oldText = "Ready";
            statusLabel.Text = _oldText;
            ToolStripProgressBar1.Value = 0;
            _searchInProgress = false;
            DuplicateButton.Enabled = true;
            DuplicateFilenameButton.Enabled = true;
            ConfigButton.Enabled = true;
            clickedButton.Text = oldButtonText;
            DisplaySelectionInfo();
            if (errorFiles.Count > 0)
            {
                var ef = new ErrorForm(errorFiles);
                ef.ShowDialog();
            }
            if (nonExistantReplaysFound && Global.AppSettings.ReplayManager.WarnAboutOldDb)
                Utils.ShowError(
                    "The replay database contains replays that have been deleted or moved. It is recommended to update the database.",
                    "Warning", MessageBoxIcon.Exclamation);
        }

        private void UploadToZworqyToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (RList.SelectedObjects.Count > 0)
            {
                string replayFile = GetSelectedReplay().Path;
                UploadForm u = new UploadForm();
                u.UploadToZworqy(replayFile);
            }
            else
                Utils.ShowError(NoReplaysSelected);
        }

        private struct ReplaysByLevel
        {
            internal readonly string Level;
            internal readonly List<Replay> Replays;

            internal ReplaysByLevel(Replay rep)
            {
                Level = rep.LevelFilename;
                Replays = new List<Replay> {rep};
            }
        }
    }
}