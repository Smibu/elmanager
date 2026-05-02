using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Playing;
using Elmanager.LevelEditor.Tools;
using Elmanager.LevelEditor.Tools.Platform;
using Elmanager.Lgr;
using Elmanager.Physics;
using Elmanager.Properties;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Elmanager.Settings;
using Elmanager.UI;
using Elmanager.Utilities;
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;
using Timer = System.Timers.Timer;

namespace Elmanager.LevelEditor;

internal partial class LevelEditorForm : FormMod, IMessageFilter, ILevelEditor
{
    private const string CoordinateFormat = "F3";
    private const string LevEditorName = "SLE";
    private const bool Physics = true;
    private readonly LevelEditorController Controller;
    private IEditorTool CurrentTool = null!;
    internal Level Lev => Controller.Lev;
    private ElmaFile? LevFile => Controller.LevFile;
    internal ElmaRenderer Renderer = null!;
    private readonly WinFormsEditorTools Tools;
    private List<string>? _currLevDirFiles;
    private bool _draggingScreen;
    private string? _loadedLevFilesDir;

    private Vector _moveStartPosition;
    private bool IsLgrLoaded => EditorLgr != null;
    private bool _draggingGrid;
    private Vector _gridStartOffset;
    private bool _programmaticPropertyChange;
    private float _dpiX;
    private float _dpiY;
    private Vector? _contextMenuClickPosition;
    private SvgImportOptions _svgImportOptions = SvgImportOptions.Default;
    private bool _maybeOpenOnDrop;
    private ZoomController _zoomCtrl = null!;
    private readonly SceneSettings _sceneSettings = new();
    private readonly TaskCompletionSource _tcs = new();
    private readonly FullScreenController _fullScreenController;
    private LgrManager? _lgrManager;
    private static readonly string[] ImportableExtensions = { DirUtils.LevExtension, DirUtils.LebExtension, ".bmp", ".png", ".gif", ".tiff", ".exif", ".svg", ".svgz" };
    private readonly LevFileWatcher _levFileWatcher;
    private bool _hasFocus;
    public SelectionFilter SelectionFilter { get; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    private HighlightTarget? CurrentHighlight { get; set; }
    private IEditorCursorManager CursorManager { get; }
    private IKeyboardState KeyboardState { get; } = new WinFormsKeyboardState();
    private IPictureDialogService PictureDialogService { get; set; } = null!;
    private ICustomShapeService CustomShapeService { get; }
    private IProgressService ProgressService { get; set; } = null!;

    ElmaRenderer ILevelEditor.Renderer => Renderer;
    Level ILevelEditor.Lev => Lev;
    ZoomController ILevelEditor.ZoomCtrl => ZoomCtrl;
    SceneSettings ILevelEditor.SceneSettings => SceneSettings;
    ISelectionFilter ILevelEditor.SelectionFilter => SelectionFilter;
    PlayController ILevelEditor.PlayController => WinFormsPlayController;
    IEditorCursorManager ILevelEditor.CursorManager => CursorManager;
    IKeyboardState ILevelEditor.KeyboardState => KeyboardState;
    LevelEditorRenderingSettings ILevelEditor.RenderingSettings => Settings.RenderingSettings;
    IPictureDialogService ILevelEditor.PictureDialogService => PictureDialogService;
    ICustomShapeService ILevelEditor.CustomShapeService => CustomShapeService;
    IProgressService ILevelEditor.ProgressService => ProgressService;

    bool ILevelEditor.ObjectFramesVisible => ShowObjectFramesButton.Checked;
    bool ILevelEditor.ObjectsVisible => ShowObjectsButton.Checked;
    bool ILevelEditor.GrassEdgesVisible => ShowGrassEdgesButton.Checked;
    bool ILevelEditor.GrassVisible => showGrassButton.Checked;
    bool ILevelEditor.GroundEdgesVisible => ShowGroundEdgesButton.Checked;
    bool ILevelEditor.GroundVisible => ShowGroundButton.Checked;
    bool ILevelEditor.TextureFramesVisible => ShowTextureFramesButton.Checked;
    bool ILevelEditor.TexturesVisible => ShowTexturesButton.Checked;
    bool ILevelEditor.PictureFramesVisible => ShowPictureFramesButton.Checked;
    bool ILevelEditor.PicturesVisible => ShowPicturesButton.Checked;

    HighlightTarget? ILevelEditor.CurrentHighlight
    {
        get => CurrentHighlight;
        set => CurrentHighlight = value;
    }

    string ILevelEditor.HighlightText
    {
        get => HighlightLabel.Text;
        set => HighlightLabel.Text = value;
    }

    void ILevelEditor.ShowError(string message, string caption) => UiUtils.ShowError(message, caption);
    void ILevelEditor.SetModified(LevModification value) => SetModified(value);
    void ILevelEditor.PreserveSelection() => PreserveSelection();
    void ILevelEditor.UpdateSelectionInfo() => UpdateSelectionInfo();
    void ILevelEditor.RedrawScene() => RedrawScene();
    void ILevelEditor.ChangeToSelectionTool() => ChangeToSelectionTool();
    void ILevelEditor.TransformMenuItemClick() => TransformMenuItemClick();

    internal LevelEditorForm(string? levPath)
    {
        InitializeComponent();
        Controller = new LevelEditorController(this);
        CursorManager = new WinFormsCursorManager(EditorControl, this);
        CustomShapeService = new WinFormsCustomShapeService(this);
        InitializeInternalMenu();
        _levFileWatcher = new LevFileWatcher(this);
        SelectionFilter = new SelectionFilter(this);
        Tools = new WinFormsEditorTools(
            new SelectionTool(this),
            new VertexTool(this),
            new DrawTool(this),
            new ObjectTool(this),
            new PipeTool(this),
            new EllipseTool(this),
            new PolyOpTool(this),
            new FrameTool(this),
            new SmoothenTool(this),
            new CutConnectTool(this),
            new AutoGrassTool(this),
            new TransformTool(this),
            new PictureTool(this),
            new TextTool(this),
            new CustomShapeTool(this)
        );
        _fullScreenController = CreateFullScreenController();
        var lev = levPath != null
            ? TryLoadLevel(levPath)
            : Settings.LastLevel != null
                ? TryLoadLevel(Settings.LastLevel)
                : Controller.CreateBlankLevel();
        PostInit(lev);
    }

    private void InitializeInternalMenu()
    {
        for (var i = 0; i < Global.Internals.Count; i++)
        {
            var level = Global.Internals[i];
            var menu = i < 28 ? openInternalPart1ToolStripMenuItem : openInternalPart2ToolStripMenuItem;
            menu.DropDownItems.Add($"{i + 1}. {Level.InternalTitles[i]}", null,
                (_, _) => { InitializeLevelButPromptIfModified(new EditorLev(level, null)); });
        }
    }

    public void InitializeLevelButPromptIfModified(EditorLev level)
    {
        if (!PromptToSaveIfModified())
            return;
        InitializeLevel(level);
    }

    private FullScreenController CreateFullScreenController() =>
        new(this, ViewerResized, new List<Control> { ToolPanel, MenuStrip1, ToolStripPanel1, StatusStrip1 });

    internal void SetLevel(ElmaFileObject<Level> lev)
    {
        Controller.SaveStartPositionIfEnabled(lev);
        InitializeLevel(new EditorLev(lev.Obj, lev.File));
    }

    public EditorLev TryLoadLevel(string levPath)
    {
        try
        {
            var lev = Level.FromPath(levPath);
            return new EditorLev(lev.Obj, lev.File);
        }
        catch (BadFileException ex)
        {
            UiUtils.ShowError("Error occurred while loading level file: " + ex.Message, "Warning",
                MessageBoxIcon.Exclamation);
            return Controller.CreateBlankLevel();
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
            ShowWarning($"The level {levPath} was not found.");
            return Controller.CreateBlankLevel();
        }
    }

    public LevelEditorForm() : this(null)
    {
        SelectionFilter = new SelectionFilter(this);
        _levFileWatcher = new LevFileWatcher(this);
    }

    private void PostInit(EditorLev lev)
    {
        System.Windows.Forms.Application.AddMessageFilter(this);
        EditorControl.HandleCreated += (_, _) =>
        {
            EditorControl.Context!.SwapInterval = 0;
            Initialize(lev);
            _tcs.SetResult();
        };
        Closed += (_, _) => System.Windows.Forms.Application.RemoveMessageFilter(this);
    }

    internal async Task WaitInit()
    {
        await _tcs.Task;
    }

    internal bool Modified => Controller.Modified;

    private int SelectedElementCount => Controller.SelectedElementCount;

    private ToolBase ToolBase => ((ToolBase)CurrentTool);

    private List<string>? CurrLevDirFiles
    {
        get
        {
            UpdateCurrLevDirFiles();
            return _currLevDirFiles;
        }
    }

    private ZoomController ZoomCtrl => _zoomCtrl;

    private SceneSettings SceneSettings => _sceneSettings;

    private WinFormsPlayController WinFormsPlayController { get; } = new() { Settings = Global.AppSettings.LevelEditor.PlayingSettings };
    private Lgr.Lgr? EditorLgr => Renderer.OpenGlLgr?.CurrentLgr;

    private void TransformMenuItemClick(object? sender = null, EventArgs? e = null)
    {
        if (!CurrentTool.Busy)
        {
            ChangeToDefaultCursor();
            ChangeToolTo(Tools.TransformTool);

            // if not busy, there's nothing to transform
            if (!CurrentTool.Busy)
            {
                ChangeToolTo(Tools.SelectionTool);
            }
        }
    }

    private void RedrawScene(object? sender = null, EventArgs? e = null)
    {
        if (WinFormsPlayController.PlayingOrPaused)
        {
            return;
        }

        DoRedrawScene();
    }

    private void SetModified(LevModification value, bool updateHistory = true)
    {
        var wasModified = value != LevModification.Nothing;
        Controller.SetModified(value, Renderer, CurrentTool, WinFormsPlayController, Settings, updateHistory);
        if (wasModified)
        {
            EnableSaveButtons(true);
            if (Settings.CheckTopologyDynamically)
                CheckTopology();
        }
    }

    private void EnableSaveButtons(bool value)
    {
        SaveButton.Enabled = value;
        SaveToolStripMenuItem.Enabled = value;
    }

    private void UpdateSelectionInfo()
    {
        Controller.UpdateSelectionInfo();
        SelectionLabel.Text = Controller.GetSelectionText();
        MirrorHorizontallyToolStripMenuItem.Enabled = SelectedElementCount >= 2;
    }

    public void UpdateLevel(Level lev)
    {
        Controller.UpdateLevel(lev);
        SetModified(LevModification.All);
        Controller.MarkSaved();
        LoadFromHistory();
    }

    private void AfterSettingsClosed()
    {
        var r = Renderer.UpdateSettings(Lev, Settings.RenderingSettings, Global.AppSettings.General.LgrDirectory);
        if (r.LgrLoadException != null)
            UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + r.LgrLoadException.Message);
        UpdateLgrTools(r);
        UpdateLabels();
        UpdateButtons();
        RedrawScene();
    }

    private void AutoGrassButtonChanged(object? sender, EventArgs e)
    {
        if (AutoGrassButton.Checked)
            ChangeToolTo(Tools.AutoGrassTool);
    }

    private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
    {
        Controller.BringToFront();
    }

    public void ChangeToDefaultCursor()
    {
        EditorControl.Cursor = Cursors.Default;
    }

    private void ChangeToolTo(IEditorTool tool)
    {
        var mod = CurrentTool.InActivate();
        Renderer.UpdateBuffers(new LevEditState(Lev, TransientElements.Empty), mod);
        CurrentTool = tool;
        CurrentTool.Activate();
        UpdateToolHelp();
        RedrawScene();
    }

    private void ChangeToSelectionTool()
    {
        ChangeToolTo(Tools.SelectionTool);
    }

    private void ShowWarning(string text)
    {
        topologyList.Text = "Warning";
        topologyList.DropDownItems.Add(text);
        topologyList.ForeColor = Color.DarkOrange;
        topologyList.Font = new Font(topologyList.Font, FontStyle.Bold);
    }

    private void CheckTopology()
    {
        if (!CurrentTool.Busy)
        {
            topologyList.DropDownItems.Clear();
            ResetTopologyListStyle();
            topologyList.Text = "Checking topology...";
            ToolStrip2.Refresh();

            var errorItems = Controller.CheckTopologyErrors(CurrentTool);
            foreach (var item in errorItems)
                topologyList.DropDownItems.Add(item);

            // Determine if there are hard errors vs warnings
            var hasWarnings = errorItems.Any(i => i.StartsWith("Level has pictures that the LGR is missing"));
            var hasErrors = errorItems.Any(i => !i.StartsWith("Level has pictures that the LGR is missing"));

            var c = errorItems.Count;
            if (c == 0)
            {
                topologyList.Text = "No problems.";
                ResetTopologyListStyle();
            }
            else if (!hasErrors && hasWarnings)
            {
                topologyList.Text = "1 warning was found!";
                topologyList.ForeColor = Color.DarkOrange;
                topologyList.Font = new Font(topologyList.Font, FontStyle.Bold);
            }
            else
            {
                topologyList.Text = c > 1 ? c + " problems were found!" : "1 problem was found!";
                topologyList.ForeColor = Color.Red;
                topologyList.Font = new Font(topologyList.Font, FontStyle.Bold);
            }
        }
        else
            topologyList.Text = "Cannot check topology while editing is in progress!";
    }

    private void ResetTopologyListStyle()
    {
        topologyList.ForeColor = Color.Black;
        topologyList.Font = new Font(topologyList.Font, FontStyle.Regular);
    }

    private void CheckTopologyAndUpdate(object? sender = null, EventArgs? e = null)
    {
        CheckTopology();
        RedrawScene();
    }

    private void ClearHistory()
    {
        Controller.ClearHistory();
        UpdateUndoRedo();
    }

    private async void ConfirmClose(object sender, CancelEventArgs e)
    {
        if (!PromptToSaveIfModified())
            e.Cancel = true;

        if (WindowState == FormWindowState.Normal)
        {
            Settings.Size = Size;
        }

        Settings.WindowState = WindowState.ToSettingsWindowState();
        Settings.LastLevel = LevFile?.Path;
        if (WinFormsPlayController.PlayingOrPaused)
        {
            e.Cancel = true;
            await WinFormsPlayController.StopPlaying();
            Close();
        }
    }

    private void CopyMenuItemClick(object sender, EventArgs e)
    {
        Controller.CopySelected(_zoomCtrl);
    }

    private bool CurrLevDirExists() => LevFile?.FileInfo.Directory?.Exists ?? false;

    private void DoRedrawScene()
    {
        Controller.DrawEditorScene(Renderer, _zoomCtrl.Cam, _sceneSettings, Settings,
            WinFormsPlayController, CurrentTool, CurrentHighlight,
            EditorControl.Width, EditorControl.Height, GetMouseCoordinates);
    }

    public LevelEditorSettings Settings => Global.AppSettings.LevelEditor;

    private void CutButtonChanged(object? sender, EventArgs e)
    {
        if (CutConnectButton.Checked)
            ChangeToolTo(Tools.CutConnectTool);
    }

    private void DeleteAllGrassToolStripMenuItemClick(object? sender, EventArgs e)
    {
        Controller.DeleteAllGrass();
    }

    private void DeleteSelected(object? sender, EventArgs? e)
    {
        Controller.DeleteSelected(CurrentTool);
    }

    private void DrawButtonChanged(object? sender, EventArgs e)
    {
        if (DrawButton.Checked)
            ChangeToolTo(Tools.DrawTool);
    }

    private void EllipseButtonChanged(object? sender, EventArgs e)
    {
        if (EllipseButton.Checked)
            ChangeToolTo(Tools.EllipseTool);
    }

    private void CustomShapeButtonChanged(object? sender, EventArgs e)
    {
        if (CustomShapeButton.Checked)
            ChangeToolTo(Tools.CustomShapeTool);
    }

    private void ExitToolStripMenuItemClick(object? sender, EventArgs e)
    {
        Close();
    }

    private void FilterChanged(object? sender, EventArgs e)
    {
        SelectionFilter.GroundFilter = GroundPolygonsToolStripMenuItem.Checked;
        SelectionFilter.GrassFilter = GrassPolygonsToolStripMenuItem.Checked;
        SelectionFilter.AppleFilter = ApplesToolStripMenuItem.Checked;
        SelectionFilter.KillerFilter = KillersToolStripMenuItem.Checked;
        SelectionFilter.FlowerFilter = FlowersToolStripMenuItem.Checked;
        SelectionFilter.StartFilter = StartToolStripMenuItem.Checked;
        SelectionFilter.PictureFilter = PicturesToolStripMenuItem.Checked;
        SelectionFilter.TextureFilter = TexturesToolStripMenuItem.Checked;
        SelectionFilterToolStripMenuItem.ShowDropDown();
    }

    private void FrameButtonChanged(object? sender, EventArgs e)
    {
        if (FrameButton.Checked)
            ChangeToolTo(Tools.FrameTool);
    }

    private Vector GetMouseCoordinates()
    {
        var mousePosNoTr = Invoke(() => EditorControl.PointToClient(MousePosition));
        var bounds = _zoomCtrl.Cam.GetBounds(Renderer.AspectRatio);
        return LevelEditorController.ScreenToWorld(mousePosNoTr.X, mousePosNoTr.Y,
            EditorControl.Width, EditorControl.Height, bounds);
    }

    private void HandleGrassMenu(object sender, EventArgs e)
    {
        Controller.ToggleGrass();
    }

    private void HandleGravityMenu(object sender, EventArgs e)
    {
        AppleType chosenAppleType;
        if (sender.Equals(GravityNoneMenuItem))
            chosenAppleType = AppleType.Normal;
        else if (sender.Equals(GravityUpMenuItem))
            chosenAppleType = AppleType.GravityUp;
        else if (sender.Equals(GravityDownMenuItem))
            chosenAppleType = AppleType.GravityDown;
        else if (sender.Equals(GravityLeftMenuItem))
            chosenAppleType = AppleType.GravityLeft;
        else
            chosenAppleType = AppleType.GravityRight;

        Controller.HandleGravity(chosenAppleType, WinFormsPlayController);
    }

    private void Initialize(EditorLev lev)
    {
        if (!Physics)
#pragma warning disable CS0162
        {
            playButton.Visible = false;
            stopButton.Visible = false;
            settingsButton.Visible = false;
        }
#pragma warning restore CS0162
        WinFormsPlayController.PlayingPaused += () => Invoke(SetNotPlaying);
        var graphics = CreateGraphics();
        _dpiX = graphics.DpiX / 96;
        _dpiY = graphics.DpiY / 96;
        var dpiXint = (int)_dpiX;
        var dpiYint = (int)_dpiY;
        ToolStrip1.ImageScalingSize = new Size(32 * dpiXint, 32 * dpiYint);
        ToolStrip2.ImageScalingSize = new Size(32 * dpiXint, 32 * dpiYint);
        MenuStrip1.ImageScalingSize = new Size(16 * dpiXint, 16 * dpiYint);
        EditorMenuStrip.ImageScalingSize = new Size(16 * dpiXint, 16 * dpiYint);
        graphics.Dispose();
        SelectionLabel.Width *= dpiXint;
        CoordinateLabel.Width *= dpiXint;
        BestTimeLabel.Width *= dpiXint;
        filenameBox.Width *= dpiXint;
        TitleBox.Width *= dpiXint;
        LGRBox.Width *= dpiXint;
        GroundComboBox.Width *= dpiXint;
        SkyComboBox.Width *= dpiXint;
        WindowState = Settings.WindowState.ToFormWindowState();
        SelectButton.Select();
        UpdateButtons();
        Size = Settings.Size;
        Renderer = new ElmaRenderer(new GlControlContext(EditorControl), Settings.RenderingSettings);
        PictureDialogService = new WinFormsPictureDialogService(Renderer);
        ProgressService = new WinFormsProgressService(this);
        CurrentTool = Tools.SelectionTool;
        SetupEventHandlers();
        InitializeLevel(lev);
    }

    private async void InitializeLevel(EditorLev lev)
    {
        Controller.SetEditorLev(lev);
        if (lev.File is not null)
        {
            var elmaFileObject = new ElmaFileObject<Level>(lev.File, lev.Lev);
            Controller.SaveStartPositionIfEnabled(elmaFileObject);
            _levFileWatcher.StoreLevDiskSnapshot(elmaFileObject);
        }
        else
        {
            _levFileWatcher.ClearLevDiskSnapshot();
        }
        await WinFormsPlayController.NotifyLevelChanged();
        PlayTimeLabel.Text = "";
        _zoomCtrl = new ZoomController(new ElmaCamera(), Lev, () => RedrawScene());
        SetNotModified();
        var r = Renderer.UpdateSettings(Lev, Settings.RenderingSettings, Global.AppSettings.General.LgrDirectory);
        if (r.LgrLoadException != null)
            UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + r.LgrLoadException.Message);
        Lev.UpdateBounds();
        ZoomFill();
        topologyList.Text = string.Empty;
        topologyList.DropDownItems.Clear();
        ResetTopologyListStyle();
        UpdateLgrTools(r);
        UpdateLabels();
        ClearHistory();
        UpdatePrevNextButtons();
        ChangeToolTo(CurrentTool);
        Controller.ErrorPoints.Clear();
    }

    private async void KeyHandlerDown(object? sender, KeyEventArgs e)
    {
        e = e.KeyCode switch
        {
            Keys.Add => new KeyEventArgs(KeyUtils.Increase),
            Keys.Subtract => new KeyEventArgs(KeyUtils.Decrease),
            _ => e
        };

        var mod = CurrentTool.KeyDown(InputAdapter.ToEditorKeyEventArgs(e));
        UpdateRendererBuffers(mod);
        UpdateToolHelp();
        var wasModified = false;
        switch (e.KeyCode)
        {
            case Keys.Oem5:
                await Controller.TexturizeSelection();
                break;
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
                if (!WinFormsPlayController.PlayingOrPaused)
                {
                    WinFormsArrowScroll.BeginArrowScroll(() => RedrawScene(), _zoomCtrl);
                }
                break;
            case Keys.Z:
                if (!Controller.LockMouseX)
                {
                    Controller.SetLockMouseX(true, MousePosition.X);
                }

                break;
            case Keys.X:
                if (!Controller.LockMouseY)
                {
                    Controller.SetLockMouseY(true, MousePosition.Y);
                }

                break;
            case Keys.Delete:
                DeleteSelected(null, null);
                break;
            case Keys.Oemcomma:
                wasModified = Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Union, Lev.Polygons);
                break;
            case Keys.OemPeriod:
                wasModified = Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, Lev.Polygons);
                break;
#pragma warning disable CS0162
            case Keys.Enter when Physics:
                WinFormsPlayController.UpdateInputKeys();
                playButton_Click(null, null);
                break;
#pragma warning restore CS0162
            case Keys.Oem2:
                wasModified = Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons);
                break;
            case Keys.Escape:
                if (!WinFormsPlayController.PlayingOrPaused)
                {
                    _fullScreenController.Restore();
                }

                stopButton_Click(null, null);
                break;
            case Keys.F11:
                _fullScreenController.Toggle();
                break;
        }

        if (wasModified)
        {
            SetModified(LevModification.Ground);
        }

        RedrawScene();
    }

    private void UpdateToolHelp()
    {
        InfoLabel.Text = CurrentTool.GetHelp();
    }

    private void SetModifiedAndRender(LevModification value)
    {
        SetModified(value);
        RedrawScene();
    }

    private void KeyHandlerUp(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Z:
                Controller.SetLockMouseX(false);
                break;
            case Keys.X:
                Controller.SetLockMouseY(false);
                break;
        }
    }

    private void ItemsDropped(object sender, DragEventArgs e)
    {
        var data = e.Data?.GetData(DataFormats.FileDrop);
        // BeginInvoke is required for Wine
        BeginInvoke(() =>
        {
            if (data is string[] files)
            {
                if (ShouldOpenOnDrop())
                {
                    OpenLevel(files[0]);
                }
                else
                {
                    ImportFiles(files);
                }
            }
        });
    }

    private void LevelPropertiesToolStripMenuItemClick(object? sender, EventArgs e)
    {
        var levelProperties = new LevelPropertiesForm(Lev, LevFile);
        levelProperties.ShowDialog();
    }

    private void LevelPropertyModified(object? sender, EventArgs e)
    {
        if (!_programmaticPropertyChange)
        {
            var wasModified = Lev.Title != TitleBox.Text || !Lev.LgrFile.EqualsIgnoreCase(SelectedLgrFilename) ||
                              Lev.GroundTextureName != SelectedGround.Name ||
                              Lev.SkyTextureName != SelectedSky.Name;
            Lev.Title = TitleBox.Text;
            Lev.LgrFile = SelectedLgrFilename;
            if (sender is not null)
            {
                if (sender.Equals(SkyComboBox) || sender.Equals(GroundComboBox) || sender.Equals(LGRBox))
                {
                    if (sender.Equals(GroundComboBox))
                        Lev.GroundTextureName = SelectedGround.Name;
                    if (sender.Equals(SkyComboBox))
                        Lev.SkyTextureName = SelectedSky.Name;
                    if (Settings.RenderingSettings.DefaultGroundAndSky)
                        UiUtils.ShowError("Default ground and sky is enabled, so you won\'t see this change in editor.",
                            "Warning", MessageBoxIcon.Exclamation);
                    var r = Renderer.UpdateSettings(Lev, Settings.RenderingSettings, Global.AppSettings.General.LgrDirectory);
                    if (r.LgrLoadException != null)
                        UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + r.LgrLoadException.Message);
                    UpdateLgrTools(r);
                    UpdateLabels();
                    RedrawScene();
                }
            }

            if (wasModified)
            {
                SetModified(LevModification.Start);
            }
        }
    }

    private TextureEntry SelectedGround => (GroundComboBox.SelectedItem as TextureEntry)!;
    private TextureEntry SelectedSky => (SkyComboBox.SelectedItem as TextureEntry)!;

    private string SelectedLgrFilename => LGRBox.SelectedItem is LgrEntry l ? l.Filename : "";

    private void LoadFromHistory()
    {
        Controller.LoadFromHistory(_zoomCtrl, Settings.RenderingSettings);
        UpdateUndoRedo();
        topologyList.DropDownItems.Clear();
        topologyList.Text = "";
        var r = Renderer.UpdateSettings(Lev, Settings.RenderingSettings, Global.AppSettings.General.LgrDirectory);
        if (r.LgrLoadException != null)
            UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + r.LgrLoadException.Message);
        UpdateLgrTools(r);
        UpdateLabels();
        ChangeToolTo(CurrentTool);
        if (!Controller.IsSaved)
        {
            SetModified(LevModification.All, false);
        }
        else
        {
            SetNotModified();
        }
    }

    private void SetNotModified()
    {
        EnableSaveButtons(false);
        Controller.SetNotModified();
    }

    private void MirrorHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Controller.MirrorSelected(MirrorOption.Horizontal);
    }

    private void MouseDownEvent(object sender, MouseEventArgs e)
    {
        Vector p = GetMouseCoordinates();
        CurrentTool.MouseMove(p);
        var info = ToolBase.GetNearestVertexInfo(p);
        int nearestObjectIndex = ToolBase.GetNearestObjectIndex(p);
        int nearestPictureIndex = ToolBase.GetNearestPictureIndex(p);
        var player = WinFormsPlayController.GetNearestDriverBodyPart(p, ToolBase.CaptureRadiusScaled);
        WinFormsPlayController.FollowDriver = false;
        switch (e.Button)
        {
            case MouseButtons.Right:
                if (!CurrentTool.Busy)
                {
                    CopyMenuItem.Visible = false;
                    DeleteMenuItem.Visible = false;
                    GravityNoneMenuItem.Visible = false;
                    GravityUpMenuItem.Visible = false;
                    GravityDownMenuItem.Visible = false;
                    GravityLeftMenuItem.Visible = false;
                    GravityRightMenuItem.Visible = false;
                    GrassMenuItem.Visible = false;
                    PicturePropertiesMenuItem.Visible = false;
                    TransformMenuItem.Visible = false;
                    bringToFrontToolStripMenuItem.Visible = false;
                    sendToBackToolStripMenuItem.Visible = false;
                    convertToToolStripMenuItem.Visible = false;
                    saveStartPositionToolStripMenuItem.Visible = false;
                    restoreStartPositionToolStripMenuItem.Visible = false;
                    createCustomShapeMenuItem.Visible = false;
                    ChangeToDefaultCursor();
                    if (SelectedElementCount > 0)
                    {
                        CopyMenuItem.Visible = true;
                        DeleteMenuItem.Visible = true;
                        convertToToolStripMenuItem.Visible = true;
                        picturesConvertItem.Visible = IsLgrLoaded;
                    }

                    TransformMenuItem.Visible = SelectedElementCount > 1;
                    Controller.SelectedObjectIndex = nearestObjectIndex;
                    if (nearestObjectIndex >= 0)
                    {
                        bringToFrontToolStripMenuItem.Visible = true;
                        sendToBackToolStripMenuItem.Visible = true;
                        switch (Lev.Objects[nearestObjectIndex].Type)
                        {
                            case ObjectType.Apple:
                                GravityNoneMenuItem.Visible = true;
                                GravityUpMenuItem.Visible = true;
                                GravityDownMenuItem.Visible = true;
                                GravityLeftMenuItem.Visible = true;
                                GravityRightMenuItem.Visible = true;
                                switch (Lev.Objects[nearestObjectIndex].AppleType)
                                {
                                    case AppleType.Normal:
                                        UpdateGravityMenu(GravityNoneMenuItem);
                                        break;
                                    case AppleType.GravityUp:
                                        UpdateGravityMenu(GravityUpMenuItem);
                                        break;
                                    case AppleType.GravityDown:
                                        UpdateGravityMenu(GravityDownMenuItem);
                                        break;
                                    case AppleType.GravityLeft:
                                        UpdateGravityMenu(GravityLeftMenuItem);
                                        break;
                                    case AppleType.GravityRight:
                                        UpdateGravityMenu(GravityRightMenuItem);
                                        break;
                                }

                                break;
                            case ObjectType.Flower:
                                break;
                            case ObjectType.Killer:
                                break;
                            case ObjectType.Start when Settings.EnableStartPositionFeature:
                                saveStartPositionToolStripMenuItem.Visible = true;
                                if (Controller.SavedStartPosition != null)
                                {
                                    restoreStartPositionToolStripMenuItem.Visible = true;
                                }

                                break;
                        }
                    }

                    if (info is not null)
                    {
                        GrassMenuItem.Visible = true;
                        Controller.GrassInfo = info;
                        if (info.Polygon.IsGrass)
                        {
                            bringToFrontToolStripMenuItem.Visible = true;
                            sendToBackToolStripMenuItem.Visible = true;
                        }
                    }

                    Controller.SelectedPictureIndex = nearestPictureIndex;
                    if (nearestPictureIndex >= 0)
                    {
                        PicturePropertiesMenuItem.Visible = true;
                        bringToFrontToolStripMenuItem.Visible = true;
                        sendToBackToolStripMenuItem.Visible = true;
                    }

                    if (Controller.SelectedPolygonCount > 0)
                    {
                        bool allGrassSelected = Lev.Polygons.Where(pol => pol.Vertices.Any(v => v.Mark == VectorMark.Selected)).All(pol => pol.IsGrass);
                        createCustomShapeMenuItem.Visible = !allGrassSelected;
                    }

                    if (player != null)
                    {
                        if (nearestObjectIndex < 0)
                        {
                            GravityUpMenuItem.Visible = true;
                            GravityDownMenuItem.Visible = true;
                            GravityLeftMenuItem.Visible = true;
                            GravityRightMenuItem.Visible = true;
                            switch (WinFormsPlayController.Driver!.GravityDirection)
                            {
                                case GravityDirection.Up:
                                    UpdateGravityMenu(GravityUpMenuItem);
                                    break;
                                case GravityDirection.Down:
                                    UpdateGravityMenu(GravityDownMenuItem);
                                    break;
                                case GravityDirection.Left:
                                    UpdateGravityMenu(GravityLeftMenuItem);
                                    break;
                                case GravityDirection.Right:
                                    UpdateGravityMenu(GravityRightMenuItem);
                                    break;
                            }
                        }
                    }

                    EditorMenuStrip.Show(MousePosition);
                }

                break;
            case MouseButtons.Middle:
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    _draggingGrid = true;
                    _gridStartOffset = _sceneSettings.GridOffset;
                }
                else
                {
                    _draggingScreen = true;
                }

                _moveStartPosition = GetMouseCoordinates();
                break;
        }

        var mod = CurrentTool.MouseDown(InputAdapter.ToEditorMouseEventArgs(e));
        UpdateRendererBuffers(mod);
        UpdateToolHelp();
        RedrawScene();
    }

    private void MouseLeaveEvent(object sender, EventArgs e)
    {
        _hasFocus = false;
        var mod = CurrentTool.MouseOutOfEditor();
        UpdateRendererBuffers(mod);
        UpdateToolHelp();
        RedrawScene();
    }

    private void UpdateRendererBuffers(LevVisualChange mod)
    {
        Renderer.UpdateBuffers(new LevEditState(Lev, CurrentTool.GetTransientElements(_hasFocus)), mod);
    }

    private void MouseMoveEvent(object sender, MouseEventArgs e)
    {
        _hasFocus = true;
        if (Controller.LockMouseX)
            Cursor.Position = new Point(Controller.LockCoord, MousePosition.Y);
        else if (Controller.LockMouseY)
            Cursor.Position = new Point(MousePosition.X, Controller.LockCoord);
        ShowCoordinates();
        if (_draggingScreen || _draggingGrid)
        {
            Vector z = GetMouseCoordinates();
            Controller.HandleDragMove(z, _moveStartPosition, _draggingGrid,
                _sceneSettings, _gridStartOffset, Settings.LockGrid, _zoomCtrl);
        }

        var mod = CurrentTool.MouseMove(GetMouseCoordinates());
        UpdateRendererBuffers(mod);
        UpdateToolHelp();
        RedrawScene();
        StatusStrip1.Refresh();
    }

    private void MouseUpEvent(object sender, MouseEventArgs e)
    {
        CurrentTool.MouseUp();
        UpdateToolHelp();
        _draggingScreen = false;
        _draggingGrid = false;
        RedrawScene();
    }

    private void MouseWheelZoom(long delta)
    {
        Controller.MouseWheelZoom(delta, GetMouseCoordinates(), _zoomCtrl, _sceneSettings, Settings, Renderer, Global.AppSettings.General.LgrDirectory);
        UpdateZoomLabel();
        RedrawScene();
    }

    private void MoveFocus(object sender, EventArgs e)
    {
        ToolPanel.Focus();
    }

    private void NewLevel(object? sender = null, EventArgs? e = null)
    {
        if (!PromptToSaveIfModified())
            return;
        var lev = Controller.CreateBlankLevel();
        InitializeLevel(lev);
    }

    private void ObjectButtonChanged(object? sender, EventArgs e)
    {
        if (ObjectButton.Checked)
            ChangeToolTo(Tools.ObjectTool);
    }

    private void OpenConfig(object sender, EventArgs e)
    {
        ComponentManager.ShowConfiguration("sle");
        AfterSettingsClosed();
        if (!Settings.EnableStartPositionFeature)
        {
            Controller.ClearSavedStartPosition();
        }
    }

    private void OpenLevel(string path)
    {
        if (!PromptToSaveIfModified())
            return;
        var lev = TryLoadLevel(path);
        InitializeLevel(lev);
    }

    private void OpenRenderingSettings(object sender, EventArgs e)
    {
        var rSettings = new RenderingSettingsForm(Settings.RenderingSettings);
        rSettings.Changed += x =>
        {
            var r = Renderer.UpdateSettings(Lev, x, Global.AppSettings.General.LgrDirectory);
            if (r.LgrLoadException != null)
                UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + r.LgrLoadException.Message);
            RedrawScene();
        };
        rSettings.ShowDialog();
        AfterSettingsClosed();
    }

    private void OpenToolStripMenuItemClick(object? sender, EventArgs e)
    {
        OpenFileDialog1.InitialDirectory = GetInitialDir();
        OpenFileDialog1.Multiselect = false;
        if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            OpenLevel(OpenFileDialog1.FileName);
    }

    private void PictureButtonChanged(object? sender, EventArgs e)
    {
        if (PictureButton.Checked)
        {
            if (!IsLgrLoaded)
            {
                UiUtils.ShowError("You need to set LGR directory from settings before you can use picture tool.",
                    "Note", MessageBoxIcon.Information);
                SelectButton.Checked = true;
            }
            else
            {
                ChangeToolTo(Tools.PictureTool);
            }
        }
    }

    private void PicturePropertiesToolStripMenuItemClick(object sender, EventArgs e)
    {
        Controller.ShowPictureProperties(Settings.AlwaysSetDefaultsInPictureTool);
    }

    private void PipeButtonChanged(object? sender, EventArgs e)
    {
        if (PipeButton.Checked)
            ChangeToolTo(Tools.PipeTool);
    }

    private void PolyOpButtonChanged(object? sender, EventArgs e)
    {
        if (PolyOpButton.Checked)
            ChangeToolTo(Tools.PolyOpTool);
    }

    private void PrevNextButtonClick(object? sender, EventArgs e)
    {
        if (CurrLevDirFiles?.Count > 0)
        {
            if (LevFile is null)
                OpenLevel(CurrLevDirFiles[0]);
            else
            {
                int i = GetCurrentLevelIndex(LevFile, CurrLevDirFiles);
                if (PreviousButton.Equals(sender) || previousLevelToolStripMenuItem.Equals(sender))
                {
                    i--;
                    if (i < 0)
                        i = CurrLevDirFiles.Count - 1;
                }
                else
                {
                    i++;
                    if (i >= CurrLevDirFiles.Count)
                        i = 0;
                }

                OpenLevel(CurrLevDirFiles[i]);
            }
        }
    }

    private bool PromptToSaveIfModified()
    {
        if (Modified)
        {
            switch (
                MessageBox.Show("Level has been modified. Do you want to save changes?", LevEditorName,
                    MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    SaveClicked();
                    break;
                case DialogResult.Cancel:
                    return false;
            }
        }

        return true;
    }

    private void QuickGrassToolStripMenuItemClick(object sender, EventArgs e)
    {
        Controller.QuickGrass(Tools.AutoGrassTool);
    }

    private void Redo(object sender, EventArgs e)
    {
        if (Controller.CanRedo && !CurrentTool.Busy)
        {
            Controller.Redo();
            LoadFromHistory();
        }
    }

    private void RefreshOnOpen(object sender, EventArgs e)
    {
        ViewerResized();
        ZoomFill();
    }

    private void SaveAs(object? sender = null, EventArgs? e = null)
    {
        string suggestion = string.Empty;
        if (Settings.UseFilenameSuggestion)
        {
            try
            {
                suggestion = Controller.GetFilenameSuggestion(
                    Global.GetLevelFiles(), Settings.BaseFilename, Settings.NumberFormat);
            }
            catch (FormatException)
            {
                UiUtils.ShowError("Invalid format string!");
            }

            SaveFileDialog1.FileName = suggestion;
        }

        SaveFileDialog1.InitialDirectory = GetInitialDir();
        if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
        {
            SaveLevel(SaveFileDialog1.FileName);
        }
    }

    private string? GetInitialDir()
    {
        return LevFile is not null ? LevFile.FileInfo.DirectoryName! : Global.AppSettings.General.LevelDirectory;
    }

    private void SaveClicked(object? sender = null, EventArgs? e = null)
    {
        if (LevFile is null)
            SaveAs();
        else
            SaveLevel(LevFile.Path);
    }

    private void SaveLevel(string path)
    {
        Lev.Title = TitleBox.Text;
        Lev.LgrFile = SelectedLgrFilename;
        Lev.GroundTextureName = SelectedGround.Name;
        Lev.SkyTextureName = SelectedSky.Name;
        if (Lev.GroundTextureName == "")
            Lev.GroundTextureName = "ground";
        if (Lev.SkyTextureName == "")
            Lev.SkyTextureName = "sky";
        if (Lev.Top10.IsEmpty ||
            MessageBox.Show("This level has times in top 10. Do you still want to save the level?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            if (Settings.CheckTopologyWhenSaving)
                CheckTopologyAndUpdate();
            if (Settings.UseFilenameForTitle && LevFile is null)
            {
                Lev.Title = Path.GetFileNameWithoutExtension(SaveFileDialog1.FileName);
            }

            try
            {
                Controller.UpdateEditorLevFile(_levFileWatcher.WithoutEvents(() => Lev.Save(path)));
                Controller.MarkSaved();
                _levFileWatcher.StoreLevDiskSnapshot(new ElmaFileObject<Level>(Controller.LevFile!, Controller.Lev));
                if (!Global.GetLevelFiles().Contains(path))
                {
                    Global.GetLevelFiles().Add(path);
                    UpdateCurrLevDirFiles(force: true);
                }

                UpdateLabels();
                UpdatePrevNextButtons();
                SetNotModified();
            }
            catch (UnauthorizedAccessException ex)
            {
                UiUtils.ShowError("Error when saving level: " + ex.Message);
            }
        }
    }

    private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
    {
        Controller.SelectAll();
    }

    private void SelectButtonChanged(object? sender, EventArgs e)
    {
        if (SelectButton.Checked)
            ChangeToolTo(Tools.SelectionTool);
    }

    private void SendToBackToolStripMenuItemClick(object? sender, EventArgs e)
    {
        Controller.SendToBack();
    }

    private void SetAllFilters(object? sender, EventArgs e)
    {
        foreach (ToolStripMenuItem x in SelectionFilterToolStripMenuItem.DropDownItems)
            if (x.CheckOnClick)
                x.Checked = EnableAllToolStripMenuItem.Equals(sender);
    }

    private void SettingChanged(object? sender, EventArgs e)
    {
        var settings = Settings.RenderingSettings;
        settings.ShowGrass = showGrassButton.Checked;
        settings.ShowGrassEdges = ShowGrassEdgesButton.Checked;
        settings.ShowGroundEdges = ShowGroundEdgesButton.Checked;
        settings.ShowGrid = ShowGridButton.Checked;
        settings.ShowObjectFrames = ShowObjectFramesButton.Checked;
        settings.ShowObjects = ShowObjectsButton.Checked;
        settings.ShowGround = ShowGroundButton.Checked;
        settings.ShowPictureFrames = ShowPictureFramesButton.Checked;
        settings.ShowPictures = ShowPicturesButton.Checked;
        settings.ShowTextureFrames = ShowTextureFramesButton.Checked;
        settings.ShowTextures = ShowTexturesButton.Checked;
        settings.ShowVertices = ShowVerticesButton.Checked;
        settings.GroundTextureEnabled = ShowGroundTextureButton.Checked;
        settings.SkyTextureEnabled = ShowSkyTextureButton.Checked;
        settings.ZoomTextures = ZoomTexturesButton.Checked;
        settings.ShowGravityAppleArrows = ShowGravityAppleArrowsButton.Checked;
        Settings.SnapToGrid = snapToGridButton.Checked;
        Settings.LockGrid = lockGridButton.Checked;
        Settings.ShowCrossHair = showCrossHairButton.Checked;
        var updateResult = Renderer.UpdateSettings(Lev, settings, Global.AppSettings.General.LgrDirectory);
        if (updateResult.LgrLoadException != null)
            UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + updateResult.LgrLoadException.Message);
        RedrawScene();
    }

    private void SetupEventHandlers()
    {
        Resize += ViewerResized;
        EditorControl.Paint += RedrawScene;
        ZoomFillButton.Click += (_, _) => ZoomFill();
        ObjectButton.CheckedChanged += ObjectButtonChanged;
        VertexButton.CheckedChanged += VertexButtonChanged;
        PipeButton.CheckedChanged += PipeButtonChanged;
        EllipseButton.CheckedChanged += EllipseButtonChanged;
        PolyOpButton.CheckedChanged += PolyOpButtonChanged;
        DrawButton.CheckedChanged += DrawButtonChanged;
        FrameButton.CheckedChanged += FrameButtonChanged;
        SelectButton.CheckedChanged += SelectButtonChanged;
        SmoothenButton.CheckedChanged += SmoothenButtonChanged;
        CutConnectButton.CheckedChanged += CutButtonChanged;
        AutoGrassButton.CheckedChanged += AutoGrassButtonChanged;
        PictureButton.CheckedChanged += PictureButtonChanged;
        CustomShapeButton.CheckedChanged += CustomShapeButtonChanged;
        LGRBox.SelectedIndexChanged += LevelPropertyModified;
        GroundComboBox.SelectedIndexChanged += LevelPropertyModified;
        SkyComboBox.SelectedIndexChanged += LevelPropertyModified;
        TitleBox.TextChanged += LevelPropertyModified;
        ToolPanel.MouseWheel += MouseWheelZoom; // Windows 8.1 and earlier
        EditorControl.MouseWheel +=
            MouseWheelZoom; // Windows 10 with the option "Scroll inactive windows when I hover over them" enabled
        previousLevelToolStripMenuItem.Click += PrevNextButtonClick;
        nextLevelToolStripMenuItem.Click += PrevNextButtonClick;
        foreach (var x in ToolStrip2.Items)
        {
            if (x is ToolStripButton button)
            {
                button.CheckedChanged += SettingChanged;
            }
        }

        foreach (RadioButtonMod x in ToolPanel.Controls)
        {
            x.KeyDown += KeyHandlerDown;
            x.KeyUp += KeyHandlerUp;
        }
    }

    private void MouseWheelZoom(object? sender, MouseEventArgs e)
    {
        MouseWheelZoom(e.Delta);
    }

    private void ShowCoordinates()
    {
        Vector x = GetMouseCoordinates();
        CoordinateLabel.Text = "Mouse X: " + x.X.ToString(CoordinateFormat) + " Y: " +
                               x.Y.ToString(CoordinateFormat);
    }

    private void SmoothenButtonChanged(object? sender, EventArgs e)
    {
        if (SmoothenButton.Checked)
            ChangeToolTo(Tools.SmoothenTool);
    }

    private void StartingDrop(object? sender, DragEventArgs e)
    {
        var data = e.Data?.GetData(DataFormats.FileDrop);
        if (data is string[] files)
        {
            if (files.All(filePath => File.Exists(filePath) && ImportableExtensions.Any(ext => Path.GetExtension(filePath).EqualsIgnoreCase(ext))))
            {
                e.Effect = DragDropEffects.Copy;
            }

            _maybeOpenOnDrop = files.Length == 1 && Path.GetExtension(files[0]) == ".lev";
        }
    }

    private void Undo(object sender, EventArgs e)
    {
        if (Controller.CanUndo && !CurrentTool.Busy)
        {
            Controller.Undo();
            LoadFromHistory();
        }
    }

    private void UpdateButtons()
    {
        var settings = Settings.RenderingSettings;
        showGrassButton.Checked = settings.ShowGrass;
        ShowGrassEdgesButton.Checked = settings.ShowGrassEdges;
        ShowGroundEdgesButton.Checked = settings.ShowGroundEdges;
        ShowGridButton.Checked = settings.ShowGrid;
        ShowObjectFramesButton.Checked = settings.ShowObjectFrames;
        ShowObjectsButton.Checked = settings.ShowObjects;
        ShowGroundButton.Checked = settings.ShowGround;
        ShowPictureFramesButton.Checked = settings.ShowPictureFrames;
        ShowPicturesButton.Checked = settings.ShowPictures;
        ShowTextureFramesButton.Checked = settings.ShowTextureFrames;
        ShowTexturesButton.Checked = settings.ShowTextures;
        ShowVerticesButton.Checked = settings.ShowVertices;
        ShowGroundTextureButton.Checked = settings.GroundTextureEnabled;
        ShowSkyTextureButton.Checked = settings.SkyTextureEnabled;
        ZoomTexturesButton.Checked = settings.ZoomTextures;
        ShowGravityAppleArrowsButton.Checked = settings.ShowGravityAppleArrows;
        snapToGridButton.Checked = Settings.SnapToGrid;
        lockGridButton.Checked = Settings.LockGrid;
        showCrossHairButton.Checked = Settings.ShowCrossHair;
    }

    private void UpdatePrevNextButtons()
    {
        PreviousButton.Enabled = CurrLevDirExists();
        NextButton.Enabled = PreviousButton.Enabled;
        previousLevelToolStripMenuItem.Enabled = PreviousButton.Enabled;
        nextLevelToolStripMenuItem.Enabled = PreviousButton.Enabled;
    }

    private void UpdateCurrLevDirFiles(bool force = false)
    {
        string? levDir = LevFile?.FileInfo.DirectoryName;
        if (levDir == null)
        {
            return;
        }

        if (force || _currLevDirFiles == null || _loadedLevFilesDir != levDir)
        {
            _currLevDirFiles = Directory.GetFiles(levDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(DirUtils.LevExtension, StringComparison.OrdinalIgnoreCase) ||
                            s.EndsWith(DirUtils.LebExtension, StringComparison.OrdinalIgnoreCase)).ToList();
            _loadedLevFilesDir = levDir;
        }
    }

    private void UpdateGravityMenu(object sender)
    {
        foreach (ToolStripMenuItem x in EditorMenuStrip.Items)
            x.Checked = sender.Equals(x);
    }

    private void UpdateLabels()
    {
        if (LevFile is null)
        {
            this.SetTitleWithVersion("New - " + LevEditorName);
            filenameBox.Text = string.Empty;
            filenameBox.Enabled = false;
            deleteButton.Enabled = false;
            deleteLevMenuItem.Enabled = false;
            EnableSaveButtons(true);
        }
        else
        {
            this.SetTitleWithVersion(LevFile.FileNameNoExt + " - " + LevEditorName);
            filenameBox.Text = LevFile.FileNameNoExt;
            filenameBox.Enabled = true;
            deleteButton.Enabled = true;
            deleteLevMenuItem.Enabled = true;
        }

        _programmaticPropertyChange = true;
        TitleBox.Text = Lev.Title;
        LGRBox.Items.Clear();
        if (_lgrManager?.LgrFolderPath != Global.AppSettings.General.LgrDirectory && Global.AppSettings.General.LgrDirectory != null)
        {
            _lgrManager = new LgrManager(Global.AppSettings.General.LgrDirectory, Resources.Lgrs);
        }
        else if (Global.AppSettings.General.LgrDirectory == null)
        {
            _lgrManager = null;
        }
        if (_lgrManager != null)
        {
            var lgrEntries = _lgrManager.GetLgrs().ToArray();
            LGRBox.Items.AddRange(lgrEntries);
            var found = lgrEntries.FirstOrDefault(e => e.Filename.EqualsIgnoreCase(Lev.LgrFile));
            if (found is not null)
            {
                LGRBox.SelectedItem = found;
            }
            else
            {
                LGRBox.Items.Add(new LgrEntry(Lev.LgrFile, null));
                LGRBox.SelectedIndex = LGRBox.Items.Count - 1;
            }
        }
        else
        {
            LGRBox.Items.Add(new LgrEntry(Lev.LgrFile, null));
            LGRBox.SelectedIndex = 0;
        }

        UiUtils.SetDropDownWidth(LGRBox);

        var foundGround = GroundComboBox.Items.Cast<TextureEntry>()
            .FirstOrDefault(t => t.Name == Lev.GroundTextureName);
        if (foundGround is null)
        {
            foundGround = new TextureEntry(Lev.GroundTextureName, IsLgrLoaded);
            GroundComboBox.Items.Add(foundGround);
        }

        var foundSky = SkyComboBox.Items.Cast<TextureEntry>().FirstOrDefault(t => t.Name == Lev.SkyTextureName);
        if (foundSky is null)
        {
            foundSky = new TextureEntry(Lev.SkyTextureName, IsLgrLoaded);
            SkyComboBox.Items.Add(foundSky);
        }

        UiUtils.SetDropDownWidth(GroundComboBox);
        UiUtils.SetDropDownWidth(SkyComboBox);

        GroundComboBox.SelectedItem = foundGround;
        SkyComboBox.SelectedItem = foundSky;
        _programmaticPropertyChange = false;
        BestTimeLabel.Text = "Best time: " + Lev.Top10.GetSinglePlayerString(0);
        UpdateSelectionInfo();
    }

    private void UpdateLgrTools(RendererSettingsChangeResult result)
    {
        if (!result.LgrUpdated)
        {
            return;
        }
        if (EditorLgr != null)
        {
            PicturePropertiesMenuItem.Enabled = true;
            SkyComboBox.Enabled = true;
            GroundComboBox.Enabled = true;
            SkyComboBox.Items.Clear();
            GroundComboBox.Items.Clear();
            var names = EditorLgr.ListedImagesExcludingSpecial.Where(image =>
                image.Type == ImageType.Texture).Select(image => new TextureEntry(image.Name, false)).OrderBy(x => x.Name).ToArray();
            SkyComboBox.Items.AddRange(names);
            GroundComboBox.Items.AddRange(names);
        }
        else
        {
            PicturePropertiesMenuItem.Enabled = false;
            SkyComboBox.Enabled = false;
            GroundComboBox.Enabled = false;
        }
    }

    public void UpdateUndoRedo()
    {
        UndoButton.Enabled = Controller.CanUndo;
        RedoButton.Enabled = Controller.CanRedo;
        UndoToolStripMenuItem.Enabled = UndoButton.Enabled;
        RedoToolStripMenuItem.Enabled = RedoButton.Enabled;
    }

    private void VertexButtonChanged(object? sender, EventArgs e)
    {
        if (VertexButton.Checked)
            ChangeToolTo(Tools.VertexTool);
    }

    private void ViewerResized(object? sender = null, EventArgs? e = null)
    {
        if (EditorControl.Width > 0 && EditorControl.Height > 0)
        {
            if (WinFormsPlayController.PlayingOrPaused)
            {
                WinFormsPlayController.ResetViewPortRequested = (EditorControl.Width, EditorControl.Height);
            }
            else
            {
                ResetViewPort();
                RedrawScene();
            }
        }
    }

    private void ResetViewPort()
    {
        Renderer.ResetViewport(EditorControl.Width, EditorControl.Height);
    }

    private void ZoomFillToolStripMenuItemClick(object? sender, EventArgs e)
    {
        ZoomFill();
    }

    private void ZoomFill()
    {
        _zoomCtrl.ZoomFill(Settings.RenderingSettings, Renderer.AspectRatio);
        UpdateZoomLabel();
    }

    private void UpdateZoomLabel()
    {
        zoomLabel.Text = $"Zoom: {_zoomCtrl.ZoomLevel:F3}";
    }

    private void TitleBoxTextChanged(object? sender, EventArgs e)
    {
        int width = TextRenderer.MeasureText(TitleBox.Text, TitleBox.Font).Width;
        TitleBox.Width = Math.Max(width + 5, 120 * (int)_dpiX);
        TitleBox.BackColor = Regex.IsMatch(TitleBox.Text, "[^a-zA-Z0-9!\"%&/()=?`^*-_,.;:<>\\[\\]+# ]")
            ? Color.Red
            : Color.White;
    }

    private void PreserveSelection()
    {
        Controller.PreserveSelection();
    }

    private void importLevelsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        importFileDialog.InitialDirectory = GetInitialDir();
        if (importFileDialog.ShowDialog() == DialogResult.OK)
        {
            ImportFiles(importFileDialog.FileNames);
        }
    }

    private void ImportFiles(string[] files)
    {
        int imported = 0;
        files.ToList().ForEach(file =>
        {
            Level lev;
            if (file.EndsWith(".lev"))
            {
                try
                {
                    lev = Level.FromPath(file).Obj;
                }
                catch (BadFileException exception)
                {
                    UiUtils.ShowError(
                        $"Imported level {file} with errors: {exception.Message}",
                        "Warning",
                        MessageBoxIcon.Exclamation);
                    lev = new Level();
                }

                lev.UpdateImages(Renderer.OpenGlLgr?.DrawableImages ?? new Dictionary<string, DrawableImage>());
            }
            else if (file.EndsWith(".svg") || file.EndsWith(".svgz"))
            {
                var result = SvgImportOptionsForm.ShowDefault(_svgImportOptions, file);
                if (result is not { } newOpts)
                {
                    return;
                }

                _svgImportOptions = newOpts;
                using var fileStream = File.OpenText(file);
                try
                {
                    lev = SvgImporter.FromStream(fileStream, newOpts);
                }
                catch (PolygonException)
                {
                    UiUtils.ShowError($"Failed to import SVG {file}. Invalid or animated SVGs are not supported.");
                    return;
                }
            }
            else
            {
                try
                {
                    lev = BitmapImporter.FromPath(file);
                }
                catch (ImportException ex)
                {
                    UiUtils.ShowError(ex.Message);
                    return;
                }
            }

            imported++;
            Lev.Import(lev);
            Lev.UpdateGrass(Settings.RenderingSettings.GrassZoom);
        });
        if (imported > 0)
        {
            SetModified(LevModification.All);
            ZoomFill();
        }
    }

    private void saveAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
    {
        saveAsPictureDialog.FileName = LevFile?.FileNameNoExt ?? "Untitled";
        if (saveAsPictureDialog.ShowDialog() == DialogResult.OK)
        {
            if (saveAsPictureDialog.FileName.EndsWith(".png"))
            {
                Renderer.SaveSnapShot(saveAsPictureDialog.FileName, _zoomCtrl, _sceneSettings, Settings.RenderingSettings);
            }
            else if (saveAsPictureDialog.FileName.EndsWith(".svg"))
            {
                SvgExporter.ExportAsSvg(Lev, Settings.RenderingSettings,
                    saveAsPictureDialog.FileName);
            }
            else
            {
                UiUtils.ShowError("File type must be PNG or SVG.");
            }
        }
    }

    private void ConvertClicked(object sender, EventArgs e)
    {
        ObjectType? objType = null;
        if (sender.Equals(applesConvertItem))
            objType = ObjectType.Apple;
        else if (sender.Equals(killersConvertItem))
            objType = ObjectType.Killer;
        else if (sender.Equals(flowersConvertItem))
            objType = ObjectType.Flower;

        Controller.ConvertSelected(objType);
    }

    private void TextButton_CheckedChanged(object sender, EventArgs e)
    {
        if (TextButton.Checked)
        {
            ChangeToolTo(Tools.TextTool);
        }
    }

    private void deleteLevMenuItem_Click(object sender, EventArgs e)
    {
        DeleteCurrentLevel();
    }

    private void DeleteCurrentLevel()
    {
        if (LevFile is null)
        {
            return;
        }

        if (MessageBox.Show("Are you sure you want to delete this level?", "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
        {
            UpdateCurrLevDirFiles();
            File.Delete(LevFile.Path);

            int levIndex = GetCurrentLevelIndex(LevFile, CurrLevDirFiles!);
            CurrLevDirFiles!.RemoveAt(levIndex);
            if (levIndex < CurrLevDirFiles.Count)
            {
                OpenLevel(CurrLevDirFiles[levIndex]);
            }
            else
            {
                NewLevel();
            }
        }
    }

    private int GetCurrentLevelIndex(ElmaFile file, List<string> files)
    {
        return files.FindIndex(
            path => string.Compare(path, file.Path, StringComparison.OrdinalIgnoreCase) == 0);
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
        DeleteCurrentLevel();
    }

    private void filenameBox_TextChanged(object? sender = null, EventArgs? e = null)
    {
        bool showButtons = LevFile is not null && string.Compare(filenameBox.Text,
            LevFile.FileNameNoExt,
            StringComparison.InvariantCulture) != 0;
        filenameOkButton.Visible = showButtons;
        filenameCancelButton.Visible = showButtons;
    }

    private void filenameCancelButton_Click(object? sender = null, EventArgs? e = null)
    {
        filenameBox.Text = LevFile?.FileNameNoExt;
    }

    private void filenameOkButton_Click(object? sender = null, EventArgs? e = null)
    {
        if (filenameBox.Text == string.Empty)
        {
            UiUtils.ShowError("The filename cannot be empty.");
            return;
        }

        try
        {
            var newPath = Path.Combine(LevFile!.FileInfo.DirectoryName!, filenameBox.Text + ".lev");
            UpdateCurrLevDirFiles();
            File.Move(LevFile.Path, newPath);
            if (CurrLevDirFiles != null)
            {
                int index = GetCurrentLevelIndex(LevFile, CurrLevDirFiles);
                CurrLevDirFiles[index] = newPath;
            }

            Controller.UpdateEditorLevFile(new ElmaFile(newPath));
            UpdateLabels();
            filenameBox_TextChanged();
        }
        catch (ArgumentException)
        {
            UiUtils.ShowError("The filename is invalid.");
        }
        catch (IOException)
        {
            UiUtils.ShowError("A level with this name already exists.");
        }
    }

    private void filenameBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (LevFile is null)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Enter:
                filenameOkButton_Click();
                e.Handled = e.SuppressKeyPress = true;
                break;
            case Keys.Escape:
                filenameCancelButton_Click();
                e.Handled = e.SuppressKeyPress = true;
                break;
        }
    }

    private void unionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Union, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void differenceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void intersectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Intersection, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void symmetricDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Tools.PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private async void texturizeMenuItem_Click(object sender, EventArgs e)
    {
        await Controller.TexturizeSelection();
    }

    private void SaveStartPosition_Click(object sender, EventArgs e)
    {
        Controller.SaveStartPosition(Lev);
    }

    private void restoreStartPositionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var mod = Controller.RestoreStartPosition();
        if (mod != LevModification.Nothing)
        {
            SetModified(mod);
        }
    }

    private void MirrorVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Controller.MirrorSelected(MirrorOption.Vertical);
    }

    private void MoveStartHereToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_contextMenuClickPosition is { } p)
        {
            var mod = Controller.MoveStartHere(p);
            if (mod != LevModification.Nothing)
                SetModified(mod);
        }
    }

    private void EditorMenuStrip_Opening(object sender, CancelEventArgs e)
    {
        _contextMenuClickPosition = GetMouseCoordinates();
    }

    private void EditorControl_DragLeave(object sender, EventArgs e)
    {
        UpdateToolHelp();
    }

    private bool ShouldOpenOnDrop()
    {
        return _maybeOpenOnDrop && EditorControl.PointToClient(MousePosition).X < EditorControl.Width / 2;
    }

    private void EditorControl_DragOver(object sender, DragEventArgs e)
    {
        if (_maybeOpenOnDrop)
        {
            InfoLabel.Text = "Left side: open, right side: import. Current: ";
            InfoLabel.Text += ShouldOpenOnDrop() ? "open" : "import";
        }
    }

    private async void playButton_Click(object? sender, EventArgs? e)
    {
        if (WinFormsPlayController.Paused)
        {
            WinFormsPlayController.PlayState = PlayState.Playing;
            SetToPlaying();
            return;
        }
        if (WinFormsPlayController.Playing)
        {
            WinFormsPlayController.PlayState = PlayState.Paused;
            SetNotPlaying();
            return;
        }

        if (Settings.PlayingSettings.ToggleFullscreen)
        {
            _fullScreenController.FullScreen();
        }
        SetToPlaying();
        var t = new Timer(100);
        var updateTime = new Action(() =>
        {
            PlayTimeLabel.Text = WinFormsPlayController.Driver!.CurrentTime.ToSeconds().ToTimeString(1);
        });
        t.Elapsed += (_, _) =>
        {
            Invoke(updateTime);
        };
        t.Start();
        WinFormsArrowScroll.AllowScroll = false;
        var oldZoom = _zoomCtrl.ZoomLevel;

        if (Settings.PlayingSettings.FollowDriverOption == FollowDriverOption.WhenPressingKey)
        {
            _zoomCtrl.ZoomLevel = Settings.PlayingSettings.PlayZoomLevel;
        }

        await WinFormsPlayController.BeginLoop(Lev, _sceneSettings, Renderer, _zoomCtrl, DoRedrawScene);

        if (Settings.PlayingSettings.FollowDriverOption == FollowDriverOption.WhenPressingKey)
        {
            Settings.PlayingSettings.PlayZoomLevel = _zoomCtrl.ZoomLevel;
            _zoomCtrl.ZoomLevel = oldZoom;
        }

        t.Stop();
        PlayTimeLabel.Text = WinFormsPlayController.Driver!.CurrentTime.ToSeconds().ToTimeString();
        if (WinFormsPlayController.Driver.Condition == DriverCondition.Finished)
        {
            PlayTimeLabel.Text += " F";
        }
        RedrawScene();
        SetNotPlaying();
        stopButton.Enabled = false;
        if (Settings.PlayingSettings.ToggleFullscreen)
        {
            _fullScreenController.Restore();
        }
    }

    private void SetNotPlaying()
    {
        playButton.Image = Resources.Play;
        playButton.ToolTipText = "Play";
    }

    private void SetToPlaying()
    {
        playButton.Image = Resources.Pause;
        playButton.ToolTipText = "Pause";
        stopButton.Enabled = true;
    }

    private async void stopButton_Click(object? sender, EventArgs? e)
    {
        if (WinFormsPlayController.PlayingOrPaused)
        {
            await WinFormsPlayController.StopPlaying();
        }

        if (Settings.PlayingSettings.ToggleFullscreen)
        {
            _fullScreenController.Restore();
        }
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        var f = new PlaySettingsForm(WinFormsPlayController.Settings);
        var result = f.ShowDialog();
        if (result == DialogResult.OK)
        {
            WinFormsPlayController.Settings = f.Settings;
            Settings.PlayingSettings = f.Settings;
        }
    }

    public bool PreFilterMessage(ref Message m)
    {
        if (WinFormsPlayController.Playing)
        {
            // The message filter is global, so the control that is receiving the message
            // might be in a different form (e.g. level manager). Hence the need for IsChild check.
            if (m.Msg is NativeUtils.WmKeydown or NativeUtils.WmKeyup && NativeUtils.IsChild(Handle, m.HWnd))
            {
                WinFormsPlayController.UpdateInputKeys();
                var key = (Keys)m.WParam;
                if (WinFormsPlayController.Settings.DisableShortcuts)
                {
                    if (m.Msg == NativeUtils.WmKeydown)
                    {
                        switch (key)
                        {
                            case Keys.Enter:
                                playButton_Click(null, null);
                                break;
                            case Keys.Escape:
                                stopButton_Click(null, null);
                                break;
                            case Keys.F11:
                                _fullScreenController.Toggle();
                                break;
                        }
                    }
                    return true;
                }
            }
        }

        return false;
    }


    private void deselectGroundPolygonsToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectPolygonsWith(p => !p.IsGrass); RedrawScene(); }

    private void deselectGrassPolygonsToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectPolygonsWith(p => p.IsGrass); RedrawScene(); }

    private void deselectApplesToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectObjectsWith(o => o.Type == ObjectType.Apple); RedrawScene(); }

    private void deselectKillersToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectObjectsWith(o => o.Type == ObjectType.Killer); RedrawScene(); }

    private void deselectFlowersToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectObjectsWith(o => o.Type == ObjectType.Flower); RedrawScene(); }

    private void deselectPicturesToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectGraphicElementsWith(ge => ge is GraphicElement.Picture); RedrawScene(); }

    private void deselectTexturesToolStripMenuItem_Click(object sender, EventArgs e) { Controller.DeselectGraphicElementsWith(ge => ge is GraphicElement.Texture); RedrawScene(); }

    private void zoomLabel_Click(object sender, EventArgs e)
    {
        if (ZoomForm.GetValue(_zoomCtrl.ZoomLevel) is { } newZoom)
        {
            _zoomCtrl.ZoomLevel = newZoom;
            UpdateZoomLabel();
            RedrawScene();
        }
    }

    private void fixSelfIntersectionsMenuItem_Click(object sender, EventArgs e)
    {
        if (Tools.PolyOpTool.FixSelfIntersections(Lev.Polygons))
        {
            SetModified(LevModification.Ground);
            RedrawScene();
        }
    }

    private void createCustomShapeMenuItem_Click(object sender, EventArgs e)
    {
        Tools.CustomShapeTool.SaveShape();
    }
}
