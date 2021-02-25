using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Elmanager.Application;
using Elmanager.IO;
using Elmanager.Rec;
using Elmanager.ReplayViewer;
using Elmanager.Searching;
using Elmanager.UI;
using Elmanager.Utilities;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;
using StringUtils = Elmanager.Utilities.StringUtils;

namespace Elmanager.ReplayManager
{
    internal partial class ReplayManagerForm : FormMod, IManagerGui
    {
        private const string NoReplaysSelected = "No replays are selected!";
        private ObjectListView list;
        private bool _cellEditing;
        private readonly MaybeOpened<ReplayViewerForm> _currentViewer = new();
        private readonly Manager<Replay> _manager;
        private CancellationTokenSource _searchCancelToken;
        protected override Size DefaultSize => new(600, 400);

        internal ReplayManagerForm()
        {
            InitializeComponent();
            _manager = new Manager<Replay>(this);

            if (Global.AppSettings.ReplayManager.ShowTooltipInList)
            {
                TypedList.CellToolTipGetter = GetReplayToolTip;
                UiUtils.ConfigureTooltip(ObjectList.CellToolTip);
            }

            Global.AppSettings.ReplayManager.RestoreGui(this);
            DisplaySelectionInfo();
        }

        private TypedObjectListView<Replay> TypedList => _manager.TypedList;

        internal string Rename(Replay replay, string newName, bool allowSameName = true)
        {
            if (replay.FileName.ToLower() == newName.ToLower() + DirUtils.RecExtension && !allowSameName)
                return null;
            var T = string.Empty;
            var recDir = Path.GetDirectoryName(replay.Path);
            if (recDir == null)
            {
                return null;
            }

            if (File.Exists(Path.Combine(recDir, newName + DirUtils.RecExtension)))
            {
                var x = 'b';
                while (File.Exists(Path.Combine(recDir, newName + x + DirUtils.RecExtension)))
                    x++;
                T = $"{x}";
            }

            try
            {
                var fileName = newName + T;
                FileSystem.RenameFile(replay.Path, fileName + DirUtils.RecExtension);
                replay.FileName = fileName;
                ObjectList.RefreshObject(replay);
                return fileName;
            }
            catch (ArgumentException)
            {
                UiUtils.ShowError("Invalid file name!");
                return null;
            }
            catch (FileNotFoundException)
            {
                UiUtils.ShowError($"File {replay.Path} doesn't exist!");
                return null;
            }
        }

        private static bool IsDiffLevel(IList<Replay> replays)
        {
            var levFileName = replays[0].LevelFilename;
            return replays.Any(x => !StringUtils.CompareWith(x.LevelFilename, levFileName));
        }

        private static string GetReplayToolTip(OLVColumn col, Replay x)
        {
            var rep = x;
            var toolTipStr = $"Replay path: {rep.Path}";
            if (rep.LevelExists && !rep.IsInternal)
                toolTipStr += $"\r\nLevel path: {rep.LevelPath}";
            toolTipStr += $"\r\n\r\n---Player 1---\r\n{rep.Player1.GetPlayerInfoStr()}";
            if (rep.IsMulti)
                toolTipStr += $"\r\n\r\n---Player 2---\r\n{rep.Player2.GetPlayerInfoStr()}";
            return toolTipStr;
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (!e.Cancel)
            {
                var x = (Replay) e.RowObject;
                var newName = Rename(x, e.Control.Text, false);
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
            if (ObjectList.SelectedObjects.Count <= 1)
            {
                UiUtils.ShowError("At least two replays must be selected!");
                return;
            }

            if (!IsDiffLevelOrMulti())
            {
                var cf = new CompareForm(GetSelectedReplays());
                cf.Show();
            }
            else
                UiUtils.ShowError(
                    "The selected replays must have the same level and they must be singleplayer replays!");
        }

        private void DeleteReplays(object sender, EventArgs e)
        {
            _manager.DeleteItems();
        }

        private void DisplayConfiguration(object sender, EventArgs e)
        {
            ComponentManager.ShowConfiguration("rm");
            ObjectList.GridLines = Global.AppSettings.ReplayManager.ShowGridInList;
        }

        public bool Busy => _cellEditing;

        public bool ConfirmDeletion()
        {
            return !Global.AppSettings.ReplayManager.ConfirmDelete || UiUtils.Confirm("Delete the selected replays - are you sure?");
        }

        public void NotifyAboutModification()
        {
            UiUtils.ShowError("Some files changed!");
        }

        public void DisplaySelectionInfo()
        {
            SelectedReplaysLabel.Text = $"{ObjectList.SelectedObjects.Count} of {ObjectList.Items.Count} replays, total time: ";
            SelectedReplaysLabel.Text +=
                GetTotalTimeOfSelected().ToTimeString(Global.AppSettings.ReplayManager.NumDecimals);
        }

        private void DuplicateFilenameSearch(object sender, EventArgs e)
        {
            if (!DirUtils.LevRecDirectoriesExist())
            {
                UiUtils.ShowError(DirUtils.LevOrRecDirNotFound);
                return;
            }

            var allFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                SearchOption.AllDirectories);
            var replayFiles = SearchUtils.FilterByRegex(allFiles, "");
            var dupes = replayFiles.GroupBy(x => Path.GetFileName(x)?.ToLower()).Where(g => g.Count() > 1)
                .SelectMany(g => g).Select(x => new Replay(x));
            ObjectList.SetObjects(dupes);
            DisplaySelectionInfo();
        }

        private void DuplicateReplaySearch(object sender, EventArgs e)
        {
            if (!DirUtils.LevRecDirectoriesExist())
            {
                UiUtils.ShowError(DirUtils.LevOrRecDirNotFound);
                return;
            }

            var allFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                Global.AppSettings.ReplayManager.RecDirSearchOption);
            var replayFiles = SearchUtils.FilterByRegex(allFiles, "");
            var sameLengths = replayFiles.GroupBy(x => FileSystem.GetFileInfo(x).Length).Where(g => g.Count() > 1)
                .SelectMany(g => g);
            using (var md5 = MD5.Create())
            {
                var dupes = sameLengths.GroupBy(x => Convert.ToBase64String(md5.ComputeHash(File.ReadAllBytes(x))))
                    .Where(g => g.Count() > 1).SelectMany(g => g).Select(x => new Replay(x)).ToList();
                ObjectList.SetObjects(dupes);
            }

            DisplaySelectionInfo();
        }

        private List<Replay> GetSelectedReplays()
        {
            return TypedList.SelectedObjects.ToList();
        }

        private Replay GetSelectedReplay()
        {
            return TypedList.SelectedObjects[0];
        }

        private byte[][] GetSelectedReplaysToArray()
        {
            var replays = new byte[ObjectList.SelectedObjects.Count][];
            for (var i = 0; i < ObjectList.SelectedObjects.Count; i++)
                replays[i] = File.ReadAllBytes(TypedList.SelectedObjects[i].Path);
            return replays;
        }

        private double GetTotalTimeOfSelected()
        {
            double totalTime = 0;
            if (Global.AppSettings.ReplayManager.Decimal3Shown)
                totalTime += TypedList.SelectedObjects.Sum(x => x.Time);
            else
                totalTime += TypedList.SelectedObjects.Sum(x => Math.Floor(x.Time * 100) / 100);
            return totalTime;
        }

        private void InvertSelection(object sender, EventArgs e)
        {
            foreach (ListViewItem x in ObjectList.Items)
                x.Selected = !x.Selected;
        }

        private bool IsDiffLevelOrMulti()
        {
            var firstReplay = GetSelectedReplay();
            return
                TypedList.SelectedObjects
                    .Any(x => x.IsMulti || !x.LevelFilename.CompareWith(firstReplay.LevelFilename));
        }

        private void MergeReplays(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count != 2)
            {
                UiUtils.ShowError("Two replays must be selected!");
                return;
            }

            if (IsDiffLevelOrMulti())
            {
                UiUtils.ShowError(
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
                UiUtils.ShowError(ex.Message);
                return;
            }

            var newReplay = new byte[replays[0].Length + replays[1].Length];
            Buffer.BlockCopy(replays[0], 0, newReplay, 0, replays[0].Length);
            Buffer.BlockCopy(replays[1], 0, newReplay, replays[0].Length, replays[1].Length);
            // Set the two bytes in newReplay to tell it's multiplayer replay
            newReplay[8] = 1;
            newReplay[replays[0].Length + 8] = 1;
            SaveFileDialog1.DefaultExt = "rec";
            SaveFileDialog1.Filter = "Elasto Mania replay (*.rec)|*.rec";
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(SaveFileDialog1.FileName, newReplay);
        }

        private void MoveOrCopy(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count == 0)
            {
                UiUtils.ShowError(NoReplaysSelected);
                return;
            }

            var move = sender.Equals(MoveToToolStripMenuItem);
            FolderBrowserDialog1.Description = move ? "Move to..." : "Copy to...";
            if (FolderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            foreach (var replay in TypedList.SelectedObjects)
            {
                try
                {
                    var dest = Path.Combine(FolderBrowserDialog1.SelectedPath, replay.FileName);
                    if (move)
                    {
                        FileSystem.MoveFile(replay.Path, dest, true);
                        replay.Path = dest;
                    }
                    else
                        FileSystem.CopyFile(replay.Path, dest, true);
                }
                catch (FileNotFoundException ex)
                {
                    UiUtils.ShowError(ex.Message);
                }
            }
        }

        private void OpenLevelMenuItemClick(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count <= 0)
            {
                UiUtils.ShowError("No replay is selected!");
                return;
            }

            var selectedReplay = GetSelectedReplay();
            if (selectedReplay.IsInternal)
            {
                UiUtils.ShowError("Cannot open level file from internal replays!");
                return;
            }

            var levelFile = selectedReplay.LevelFilename;

            if (!selectedReplay.LevelExists)
            {
                UiUtils.ShowError("Level file doesn\'t exist!");
                return;
            }

            var file = Global.GetLevelFiles().FirstOrDefault(t => Path.GetFileName(t).CompareWith(levelFile));
            if (file != null)
            {
                OsUtils.ShellExecute(file);
            }
        }

        private void OpenViewer(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count <= 0)
            {
                if (!sender.Equals(ObjectList))
                    UiUtils.ShowError(NoReplaysSelected);
                return;
            }

            var rps = GetSelectedReplays();
            if (IsDiffLevel(rps))
            {
                UiUtils.ShowError("The replays must have the same level!");
                return;
            }

            if (!rps[0].LevelExists)
            {
                UiUtils.ShowError("Level for the replays was not found!");
                return;
            }

            Cursor = Cursors.WaitCursor;
            _currentViewer.Instance.SetReplays(rps);
            _currentViewer.Instance.Show();

            Cursor = Cursors.Default;
        }

        private void RemoveReplays(object sender = null, EventArgs e = null)
        {
            _manager.RemoveReplays();
        }

        private void Rename(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count != 1)
            {
                UiUtils.ShowError("One replay must be selected!");
                return;
            }

            ObjectList.CellEditActivation = ObjectListView.CellEditActivateMode.F2Only;
            ObjectList.EditSubItem(ObjectList.SelectedItem,
                ObjectList.AllColumns.FindIndex(c => c.AspectName == nameof(Replay.FileNameNoExt)));
            ObjectList.CellEditActivation = ObjectListView.CellEditActivateMode.None;
        }

        private void RenamePattern(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count <= 0)
            {
                UiUtils.ShowError(NoReplaysSelected);
                return;
            }

            var rForm = new RenameForm(TypedList.SelectedObjects, this)
                {Location = new Point(MousePosition.X, MousePosition.Y)};
            rForm.ShowDialog();
        }

        private void ReplaylistSelectionChanged(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count > 0 && _currentViewer.ExistingInstance != null)
            {
                Cursor = Cursors.WaitCursor;
                var selectedRp = GetSelectedReplay();
                if (selectedRp.LevelExists)
                {
                    _currentViewer.ExistingInstance.SetReplays(selectedRp);
                }

                Cursor = Cursors.Default;
            }

            DisplaySelectionInfo();
        }

        private void ResetFields(object sender, EventArgs e)
        {
            for (var i = 1; i <= 12; i++)
            {
                TabControl1.TabPages[1].Controls[$"TextBox{i}"].Text = "0";
                TabControl1.TabPages[1].Controls[$"TextBox{i + 12}"].Text = "10000";
            }
        }

        private void ResizeControls(object sender, EventArgs e)
        {
            if (ToolStripProgressBar1.ProgressBar != null)
                ToolStripProgressBar1.ProgressBar.Width = (int) (Width * 0.6);
        }

        private void SaveListToTextFile(object sender, EventArgs e)
        {
            if (ObjectList.SelectedObjects.Count <= 0)
            {
                UiUtils.ShowError(NoReplaysSelected);
                return;
            }

            SaveFileDialog1.DefaultExt = "txt";
            SaveFileDialog1.Filter = "Text document (*.txt)|*.txt";
            if (SaveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            statusLabel.Text = "Saving... ";
            StatusStrip1.Refresh();
            const int columnWidth = 2;
            var file = new StreamWriter(SaveFileDialog1.FileName);
            file.WriteLine($"{ObjectList.SelectedObjects.Count} replays");
            file.WriteLine();
            // Find out order of columns
            var cols = ObjectList.AllColumns.Select((x, i) => (x.DisplayIndex, i)).ToList();
            cols.Sort((x, y) => x.DisplayIndex.CompareTo(y.DisplayIndex));
            var columnOrder = cols.Select(x => x.i).ToList();
            // Get maximum text length for each column
            var maxColTextLength = new int[columnOrder.Count];
            for (var i = 0; i < columnOrder.Count; i++)
            {
                maxColTextLength[i] = 0;
                foreach (ListViewItem j in ObjectList.SelectedItems)
                    maxColTextLength[i] = Math.Max(maxColTextLength[i], j.SubItems[columnOrder[i]].Text.Length);
            }

            for (var i = 0; i < columnOrder.Count; i++)
                if (maxColTextLength[i] > 0)
                    file.Write($" - {ObjectList.Columns[columnOrder[i]].Text}");
            file.Write(" - ");
            file.WriteLine();
            ToolStripProgressBar1.Value = 0;
            ToolStripProgressBar1.Maximum = ObjectList.SelectedObjects.Count;
            foreach (ListViewItem i in ObjectList.SelectedItems)
            {
                for (var j = 0; j < columnOrder.Count; j++)
                {
                    if (maxColTextLength[j] <= 0) continue;
                    file.Write(i.SubItems[columnOrder[j]].Text);
                    for (var k = 0; k < maxColTextLength[j] - i.SubItems[columnOrder[j]].Text.Length; k++)
                        file.Write(' ');
                    if (j < columnOrder.Count - 1)
                        for (var k = 0; k < columnWidth; k++)
                            file.Write(' ');
                }

                file.WriteLine();
                ToolStripProgressBar1.Value++;
            }

            file.WriteLine();
            file.Write("Total time: ");
            var totalTime = GetTotalTimeOfSelected();
            file.Write(totalTime.ToTimeString(Global.AppSettings.ReplayManager.NumDecimals));

            statusLabel.Text = "Ready";
            ToolStripProgressBar1.Value = 0;
            file.Close();
        }

        private void SaveSettings(object sender, FormClosingEventArgs e)
        {
            Global.AppSettings.ReplayManager.SaveGui(this);
        }

        private void SelectAll(object sender, EventArgs e)
        {
            ObjectList.SelectAll();
        }

        private async void ToggleSearch(object sender, EventArgs e)
        {
            if (_searchCancelToken != null)
            {
                _searchCancelToken.Cancel();
                _searchCancelToken = null;
                return;
            }

            var levelPattern = LevPatternBox.Text;
            var errorFiles = new List<string>();

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

            var searchOnlyMissingLev = clickedButton.Equals(ReplaysWithoutLevFileButton);
            var searchOnlyWrongLev = clickedButton.Equals(ReplaysIncorrectLevButton);

            if (!DirUtils.RecDirectoryExists())
            {
                UiUtils.ShowError(DirUtils.RecDirNotFound);
                return;
            }

            var p1Apples = Range<int>.FromNumericBoxes(TextBox1, TextBox14);
            var p2Apples = Range<int>.FromNumericBoxes(TextBox3, TextBox16);
            var p1Turns = Range<int>.FromNumericBoxes(TextBox2, TextBox23);
            var p2Turns = Range<int>.FromNumericBoxes(TextBox10, TextBox15);
            var p1Gt = Range<int>.FromNumericBoxes(TextBox11, TextBox24);
            var p2Gt = Range<int>.FromNumericBoxes(TextBox12, TextBox13);
            var p1Sv = Range<int>.FromNumericBoxes(TextBox4, TextBox21);
            var p2Sv = Range<int>.FromNumericBoxes(TextBox9, TextBox22);
            var p1Rv = Range<int>.FromNumericBoxes(TextBox6, TextBox19);
            var p2Rv = Range<int>.FromNumericBoxes(TextBox7, TextBox20);
            var p1Lv = Range<int>.FromNumericBoxes(TextBox8, TextBox17);
            var p2Lv = Range<int>.FromNumericBoxes(TextBox5, TextBox18);
            Range<double> time;
            try
            {
                time = SearchUtils.GetTimeRange(TimeMinBox.Text, TimeMaxBox.Text);
            }
            catch (Exception)
            {
                UiUtils.ShowError("Time bound is in wrong format. Make sure it is 9 characters long.");
                return;
            }

            var size = new Range<int>((int) minFileSizeBox.Value * 1024, (int) maxFileSizeBox.Value * 1024);

            Regex levFilenameMatcher;
            try
            {
                levFilenameMatcher = new Regex(levelPattern, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                levFilenameMatcher = new Regex(string.Empty);
            }

            var searchParams = new ReplaySearchParameters
            {
                InternalRec = SearchParameters.GetBoolOptionFromTriSelect(intExtSelect),
                AcrossLev = SearchParameters.GetBoolOptionFromTriSelect(elmaAcrossSelect),
                Date = new Range<DateTime>(minDateTime.Value, maxDateTime.Value),
                Finished = SearchParameters.GetBoolOptionFromTriSelect(finishedSelect),
                LevExists = BoolOption.Dontcare,
                WrongLev = BoolOption.Dontcare,
                LevFilenameMatcher = levFilenameMatcher,
                MultiPlayer = SearchParameters.GetBoolOptionFromTriSelect(singleMultiSelect),
                Size = size,
                Time = time,
                P1Bounds =
                    new ReplaySearchParameters.PlayerBounds
                    {
                        Apples = p1Apples,
                        GroundTouches = p1Gt,
                        LeftVolts = p1Lv,
                        RightVolts = p1Rv,
                        SuperVolts = p1Sv,
                        Turns = p1Turns
                    },
                P2Bounds =
                    new ReplaySearchParameters.PlayerBounds
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
                searchParams.LevExists = BoolOption.False;
            }

            if (searchOnlyWrongLev)
            {
                searchParams.ResetOptions();
                searchParams.WrongLev = BoolOption.True;
            }

            var allFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                Global.AppSettings.ReplayManager.RecDirSearchOption);
            var replayFiles = searchOnlyMissingLev || searchOnlyWrongLev
                ? allFiles.ToList()
                : SearchUtils.FilterByRegex(allFiles, SearchPattern).ToList();
            ObjectList.ClearObjects();
            var oldButtonText = clickedButton.Text;
            _searchCancelToken = new CancellationTokenSource();
            DuplicateButton.Enabled = false;
            DuplicateFilenameButton.Enabled = false;
            ConfigButton.Enabled = false;
            clickedButton.Text = "Stop";
            var foundReplays = new List<Replay>();
            ToolStripProgressBar1.Maximum = replayFiles.Count;

            var recsByLevel = new Dictionary<string, List<Replay>>();
            var token = _searchCancelToken.Token;
            var progressHandler = new Progress<(int progress, int levels, int recs, int phase)>(value =>
            {
                var (progressValue, levels, recs, phase) = value;
                ToolStripProgressBar1.Value = progressValue;
                if (searchForAllReplays)
                {
                    statusLabel.Text = $"Found {recs} replays so far";
                }
                else
                {
                    statusLabel.Text = phase == 1 ? $"Phase 1: {levels} levels" : $"Phase 2: {recs} replays";
                }
            });
            var progress = (IProgress<(int progress, int levels, int recs, int phase)>) progressHandler;
            var iter = 0;
            try
            {
                await Task.Run(() =>
                {
                    foreach (var replayFile in replayFiles)
                    {
                        iter++;
                        Replay srp;
                        try
                        {
                            srp = new Replay(replayFile);
                        }
                        catch (BadFileException)
                        {
                            errorFiles.Add(replayFile);
                            continue;
                        }

                        if (searchParams.Matches(srp))
                        {
                            if (!searchForAllReplays)
                            {
                                var key = srp.LevelFilename.ToLower() + srp.IsMulti;
                                recsByLevel.TryGetValue(key, out var recs);
                                if (recs == null)
                                {
                                    recs = new List<Replay>();
                                    recsByLevel[key] = recs;
                                }

                                recs.Add(srp);
                            }
                            else
                                foundReplays.Add(srp);
                        }

                        if (iter % 10 == 0)
                            progress.Report((iter, recsByLevel.Count, foundReplays.Count, 1));
                        token.ThrowIfCancellationRequested();
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                Finalize();
                return;
            }

            if (!searchForAllReplays)
            {
                ToolStripProgressBar1.Maximum = recsByLevel.Count;
                iter = 0;
                try
                {
                    await Task.Run(() =>
                    {
                        foreach (var rbl in recsByLevel)
                        {
                            iter++;
                            var matchedReplay = rbl.Value[0];
                            foreach (var z in rbl.Value.Where(z => !z.WrongLevelVersion))
                            {
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
                                             (z.Player1.Apples < matchedReplay.Player1.Apples ||
                                              matchedReplay.Time > z.Time))
                                        matchedReplay = z;
                                }
                            }

                            if (searchForFastestReplays || rbl.Value.Count > 1)
                                foundReplays.Add(matchedReplay);
                            if (iter % 10 == 0)
                                progress.Report((iter, recsByLevel.Count, foundReplays.Count, 2));
                            token.ThrowIfCancellationRequested();
                        }
                    }, token);
                }
                catch (OperationCanceledException)
                {
                }
            }

            Finalize();

            void Finalize()
            {
                ObjectList.SetObjects(foundReplays);
                statusLabel.Text = "Ready";
                ToolStripProgressBar1.Value = 0;
                _searchCancelToken = null;
                DuplicateButton.Enabled = true;
                DuplicateFilenameButton.Enabled = true;
                ConfigButton.Enabled = true;
                clickedButton.Text = oldButtonText;
                DisplaySelectionInfo();
                if (errorFiles.Count > 0)
                {
                    using var ef = new ErrorForm(errorFiles);
                    ef.ShowDialog();
                }
            }
        }

        public Form Form => this;

        public ObjectListView ObjectList => list;

        public string SearchPattern
        {
            get => PatternBox.Text;
            set => PatternBox.Text = value;
        }

        public string EmptySelectionError => "No replays are selected.";

        private void autoresizeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectList.AutoResizeColumns();
        }
    }
}