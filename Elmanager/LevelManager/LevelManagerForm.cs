using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Elmanager.Application;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.Rec;
using Elmanager.ReplayViewer;
using Elmanager.Searching;
using Elmanager.UI;

namespace Elmanager.LevelManager
{
    internal partial class LevelManagerForm : FormMod, IManagerGui
    {
        private CancellationTokenSource _searchCancelToken;
        private readonly TypedObjectListView<Top10EntrySingle> _singleList;
        private readonly TypedObjectListView<Top10EntryMulti> _multiList;
        private readonly MaybeOpened<ReplayViewerForm> _viewer = new();
        private readonly MaybeOpened<LevelEditorForm> _editor = new();
        private Dictionary<string, List<Replay>> _recsByLevel = new();
        protected override Size DefaultSize => new(600, 400);
        private readonly Manager<Level> _manager;
        private TypedObjectListView<Level> TypedList => _manager.TypedList;

        public LevelManagerForm()
        {
            InitializeComponent();
            _manager = new Manager<Level>(this);
            _singleList = new TypedObjectListView<Top10EntrySingle>(singleTop10List);
            _multiList = new TypedObjectListView<Top10EntryMulti>(multiTop10List);
            UiUtils.ConfigureColumns<Level>(levelList);
            UiUtils.ConfigureColumns<Top10EntrySingle>(singleTop10List, true);
            UiUtils.ConfigureColumns<Top10EntryMulti>(multiTop10List, true);
            if (Global.AppSettings.LevelManager.ShowTooltipInList)
            {
                TypedList.CellToolTipGetter = GetLevelToolTip;
                UiUtils.ConfigureTooltip(ObjectList.CellToolTip);
            }

            Global.AppSettings.LevelManager.RestoreGui(this);
            DisplaySelectionInfo();
        }

        public bool Busy => false;

        public void DisplaySelectionInfo()
        {
            SelectedLevelsLabel.Text = $"{ObjectList.SelectedObjects.Count} of {ObjectList.Items.Count} levels";
        }

        public bool ConfirmDeletion()
        {
            return !Global.AppSettings.LevelManager.ConfirmDelete || UiUtils.Confirm("Delete the selected levels - are you sure?");
        }

        public void NotifyAboutModification()
        {
            UiUtils.ShowError("Some files changed!");
        }

        private static string GetLevelToolTip(OLVColumn column, Level lev)
        {
            return $"Gravity up: {lev.GetGravityAppleCount(AppleType.GravityUp)}\n" +
                   $"Gravity down: {lev.GetGravityAppleCount(AppleType.GravityDown)}\n" +
                   $"Gravity left: {lev.GetGravityAppleCount(AppleType.GravityLeft)}\n" +
                   $"Gravity right: {lev.GetGravityAppleCount(AppleType.GravityRight)}";
        }

        private (Regex titleRe, Regex lgrRe, Regex gtRe, Regex stRe, Regex spnRe, Regex mpnRe)? GetRegexes()
        {
            try
            {
                var titleRe = new Regex(titleBox.Text);
                var lgrRe = new Regex(lgrBox.Text);
                var gtRe = new Regex(groundTextureBox.Text);
                var stRe = new Regex(skyTextureBox.Text);
                var spnRe = new Regex(singleplayerNickBox.Text);
                var mpnRe = new Regex(multiplayerNickBox.Text);
                return (titleRe, lgrRe, gtRe, stRe, spnRe, mpnRe);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            if (_searchCancelToken != null)
            {
                _searchCancelToken.Cancel();
                _searchCancelToken = null;
                return;
            }

            if (!DirUtils.LevDirectoryExists())
            {
                UiUtils.ShowError(DirUtils.LevDirNotFound);
                return;
            }
            var res = GetRegexes();
            if (!res.HasValue)
            {
                UiUtils.ShowError("Some of the search parameters have invalid syntax (regex).");
                return;
            }

            var allFiles = Directory.GetFiles(Global.AppSettings.General.LevelDirectory, "*.lev",
                Global.AppSettings.LevelManager.LevDirSearchOption);
            var recFiles = Directory.GetFiles(Global.AppSettings.General.ReplayDirectory, "*.rec",
                Global.AppSettings.LevelManager.RecDirSearchOption);
            var levFiles = SearchUtils.FilterByRegex(allFiles, SearchPattern).ToList();
            searchProgressBar.Maximum = recFiles.Length;
            var progressHandler = new Progress<(int progress, string statusText)>(value =>
            {
                var (progressValue, status) = value;
                searchProgressBar.Value = progressValue;
                statusLabel.Text = status;
            });
            var progress = (IProgress<(int progress, string statusText)>) progressHandler;

            var errorFiles = new List<string>();
            _recsByLevel = new Dictionary<string, List<Replay>>();
            _searchCancelToken = new CancellationTokenSource();
            var token = _searchCancelToken.Token;
            var foundLevs = new List<Level>();
            var clickedButton = (Button)sender;
            var oldButtonText = clickedButton.Text;
            clickedButton.Text = "Stop";
            try
            {
                await Task.Run(() =>
                {
                    var iter = 0;
                    foreach (var f in recFiles)
                    {
                        iter++;
                        Replay r;
                        try
                        {
                            r = new Replay(f);
                        }
                        catch (BadFileException)
                        {
                            continue;
                        }
                        var key = r.LevelFilename.ToLower() + r.LevId;
                        _recsByLevel.TryGetValue(key, out var list);
                        if (list == null)
                        {
                            list = new List<Replay>();
                            _recsByLevel[key] = list;
                        }

                        list.Add(r);
                        if (iter % 10 == 0)
                            progress.Report((iter, $"Processing replay files first: {iter} of {recFiles.Length}"));
                        token.ThrowIfCancellationRequested();
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                Finalize();
                return;
            }

            searchProgressBar.Maximum = levFiles.Count;
            var (titleRe, lgrRe, gtRe, stRe, spnRe, mpnRe) = res.Value;
            var searchParams = new LevelSearchParameters
            {
                AcrossLev = SearchParameters.GetBoolOptionFromTriSelect(elmaAcrossSelect),
                Date = new Range<DateTime>(minDateTime.Value, maxDateTime.Value),
                Size = new Range<int>((int) minFileSizeBox.Value * 1024, (int) maxFileSizeBox.Value * 1024),
                Title = titleRe,
                Lgr = lgrRe,
                GroundTexture = gtRe,
                SkyTexture = stRe,
                SinglePlayerNick = spnRe,
                MultiPlayerNick = mpnRe,
                SinglePlayerBestTime = SearchUtils.GetTimeRange(minSingleBestTimeBox.Text, maxSingleBestTimeBox.Text),
                MultiPlayerBestTime = SearchUtils.GetTimeRange(minMultiBestTimeBox.Text, maxMultiBestTimeBox.Text),
                GroundPolygons = Range<int>.FromNumericBoxes(minGroundPolygonsBox, maxGroundPolygonsBox),
                GroundVertices = Range<int>.FromNumericBoxes(minGroundVerticesBox, maxGroundVerticesBox),
                GrassPolygons = Range<int>.FromNumericBoxes(minGrassPolygonsBox, maxGrassPolygonsBox),
                GrassVertices = Range<int>.FromNumericBoxes(minGrassVerticesBox, maxGrassVerticesBox),
                SingleTop10Times = Range<int>.FromNumericBoxes(minSingleplayerTimesBox, maxSingleplayerTimesBox),
                MultiTop10Times = Range<int>.FromNumericBoxes(minMultiplayerTimesBox, maxMultiplayerTimesBox),
                Killers = Range<int>.FromNumericBoxes(minKillersBox, maxKillersBox),
                Flowers = Range<int>.FromNumericBoxes(minFlowersBox, maxFlowersBox),
                Pictures = Range<int>.FromNumericBoxes(minPicturesBox, maxPicturesBox),
                Textures = Range<int>.FromNumericBoxes(minTexturesBox, maxTexturesBox),
                Apples = Range<int>.FromNumericBoxes(minApplesBox, maxApplesBox),
                GravApples = new Dictionary<AppleType, Range<int>>
                {
                    {AppleType.Normal, Range<int>.FromNumericBoxes(minNormApplesBox, maxNormApplesBox)},
                    {AppleType.GravityUp, Range<int>.FromNumericBoxes(minGravUpApplesBox, maxGravUpApplesBox)},
                    {AppleType.GravityDown, Range<int>.FromNumericBoxes(minGravDownApplesBox, maxGravDownApplesBox)},
                    {AppleType.GravityLeft, Range<int>.FromNumericBoxes(minGravLeftApplesBox, maxGravLeftApplesBox)},
                    {AppleType.GravityRight, Range<int>.FromNumericBoxes(minGravRightApplesBox, maxGravRightApplesBox)}
                }
            };
            
            try
            {
                await Task.Run(() =>
                {
                    var iter = 0;
                    foreach (var levFile in levFiles)
                    {
                        iter++;
                        Level srp;
                        try
                        {
                            srp = Level.FromPath(levFile);
                        }
                        catch (BadFileException)
                        {
                            errorFiles.Add(levFile);
                            continue;
                        }

                        if (searchParams.Matches(srp))
                        {
                            foundLevs.Add(srp);
                        }

                        if (iter % 10 == 0)
                            progress.Report((iter, $"Found {foundLevs.Count} levels so far"));
                        token.ThrowIfCancellationRequested();
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
            }

            Finalize();

            void Finalize()
            {
                foreach (var lev in foundLevs)
                {
                    var key = lev.FileName.ToLower() + lev.Identifier;
                    _recsByLevel.TryGetValue(key, out var recs);
                    if (recs != null)
                    {
                        lev.Replays = recs.Count;
                    }
                }

                ObjectList.SetObjects(foundLevs);
                statusLabel.Text = "Ready";
                searchProgressBar.Value = 0;
                _searchCancelToken = null;
                configButton.Enabled = true;
                clickedButton.Text = oldButtonText;
                DisplaySelectionInfo();
                if (errorFiles.Count > 0)
                {
                    using var ef = new ErrorForm(errorFiles);
                    ef.ShowDialog();
                }
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            ComponentManager.ShowConfiguration("lm");
            ObjectList.GridLines = Global.AppSettings.LevelManager.ShowGridInList;
        }

        public Form Form => this;

        public ObjectListView ObjectList => levelList;

        public string SearchPattern
        {
            get => levelFilenameBox.Text;
            set => levelFilenameBox.Text = value;
        }

        public string EmptySelectionError => "No levels are selected.";

        private void LevelManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.AppSettings.LevelManager.SaveGui(this);
        }

        private void autoresizeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectList.AutoResizeColumns();
        }

        private void levelList_SelectionChanged(object sender, EventArgs e)
        {
            RefreshTop10Lists();
            var v = _viewer.ExistingInstance;
            if (v != null && CurrentLevelHasReplays)
            {
                UpdateViewer(v);
            }

            var editor = _editor.ExistingInstance;
            if (editor != null && TypedList.SelectedObject != null && !editor.Modified)
            {
                editor.SetLevel(TypedList.SelectedObject);
            }
        }

        private void RefreshTop10Lists()
        {
            SelectedLevelsLabel.Text = $"{levelList.SelectedObjects.Count} of {levelList.Items.Count} levels";
            if (TypedList.SelectedObject == null)
            {
                singleTop10List.ClearObjects();
                multiTop10List.ClearObjects();
                return;
            }

            _singleList.Objects = TypedList.SelectedObject.Top10.SinglePlayer;
            _multiList.Objects = TypedList.SelectedObject.Top10.MultiPlayer;
            singleTop10List.AutoResizeColumns();
            multiTop10List.AutoResizeColumns();
        }

        private void removeSelectedTimesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lev = TypedList.SelectedObject;
            if (lev == null)
            {
                return;
            }

            if (UiUtils.Confirm("Delete the selected times - are you sure?"))
            {
                var list = GetSourceControl(sender);
                if (list.Equals(singleTop10List))
                {
                    lev.Top10.SinglePlayer.RemoveAll(entry => _singleList.SelectedObjects.Contains(entry));
                }
                else if (list.Equals(multiTop10List))
                {
                    lev.Top10.MultiPlayer.RemoveAll(entry => _multiList.SelectedObjects.Contains(entry));
                }

                SaveLevAndRefresh(lev);
            }
        }

        private void SaveLevAndRefresh(Level lev)
        {
            lev.Save(lev.Path, false);
            RefreshTop10Lists();
            ObjectList.RefreshObject(lev);
        }

        private static Control GetSourceControl(object sender)
        {
            return ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl;
        }

        private void openViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenViewer();
        }

        private void OpenViewer()
        {
            if (!CurrentLevelHasReplays)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            UpdateViewer(_viewer.Instance);
            Cursor = Cursors.Default;
        }

        private async void UpdateViewer(ReplayViewerForm v)
        {
            if (CurrentLevelHasReplays)
            {
                v.Show();
                await v.WaitInit();
                v.SetReplays(GetReplays(TypedList.SelectedObject));
            }
        }

        private bool CurrentLevelHasReplays =>
            TypedList.SelectedObject != null && TypedList.SelectedObject.Replays > 0;

        private List<Replay> GetReplays(Level lev)
        {
            _recsByLevel.TryGetValue(lev.FileName.ToLower() + lev.Identifier, out var list);
            return list ?? new List<Replay>();
        }

        private void levelList_DoubleClick(object sender, EventArgs e)
        {
            OpenViewer();
        }

        private async void OpenInLevelEditor(object sender, EventArgs e)
        {
            if (TypedList.SelectedObject != null)
            {
                Cursor = Cursors.WaitCursor;
                _editor.Instance.Show();
                await _editor.Instance.WaitInit();
                _editor.Instance.SetLevel(TypedList.SelectedObject);
                Cursor = Cursors.Default;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelList.SelectAll();
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem x in ObjectList.Items)
                x.Selected = !x.Selected;
        }

        private void clearTop10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lev = TypedList.SelectedObject;
            if (lev == null)
            {
                return;
            }

            if (UiUtils.Confirm("Delete all times - are you sure?"))
            {
                lev.Top10.SinglePlayer.Clear();
                lev.Top10.MultiPlayer.Clear();
                SaveLevAndRefresh(lev);
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lev = TypedList.SelectedObject;
            if (lev == null)
            {
                return;
            }
            var list = GetSourceControl(sender);
            if (list.Equals(singleTop10List))
            {
                Clipboard.SetText(lev.Top10.GetSinglePlayerString());
            }
            else if (list.Equals(multiTop10List))
            {
                Clipboard.SetText(lev.Top10.GetMultiPlayerString());
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _manager.DeleteItems();
        }
    }
}