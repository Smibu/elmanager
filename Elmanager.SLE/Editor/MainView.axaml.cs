using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaDialogs.Views;
using DialogHostAvalonia;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Playing;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Elmanager.SLE.Dialogs;
using Elmanager.SLE.Editor.Tools;
using Elmanager.SLE.LgrUtil;
using Elmanager.SLE.Platform;
using SleEditorTools = Elmanager.SLE.Editor.Tools.EditorTools;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.SLE.Editor;

public partial class MainView : UserControl, ILevelEditor
{
    private static readonly IBrush TopologyOkBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0xAF, 0x4A));

    private static readonly HttpClient Http = new();
    private readonly LevelEditorController<SleEditorLev> _controller;
    private readonly AvaloniaCursorManager _cursorManager;
    private readonly FullscreenController _fullscreenController;
    private readonly AvaloniaGameLoopRunner _gameLoopRunner;
    private readonly Lock _levelModificationLock = new();
    private readonly BookmarkLgrCache _lgrCache;
    private readonly PlayController _playController;
    private readonly SceneSettings _sceneSettings = new();
    private readonly ZoomController _zoomCtrl;
    private HighlightTarget? _currentHighlight;
    private IEditorTool _currentTool;
    private bool _draggingGrid;
    private bool _draggingScreen;
    private Vector _gridStartOffset;
    private bool _hasFocus;
    private Vector _lastMouseCoords;
    private Vector _moveStartPosition;
    private LevVisualChange? _pendingModification;
    private bool _pendingSettingsUpdate;
    private bool _pendingZoomFill;
    private bool _programmaticLgrChange;
    private bool _programmaticTextureChange;
    private ElmaRenderer? _renderer;
    private int _topologyCheckVersion;

    public MainView()
    {
        InitializeComponent();
        ExitSeparator.IsVisible = ExitMenuItem.IsVisible = !OperatingSystem.IsBrowser();
        ExceptionNotificationList.ItemsSource = _exceptionNotifications;
        _fullscreenController = new FullscreenController(
            this,
            SetFullscreenUi,
            OnFullscreenDismissed,
            ex => LogException(ex, "Could not change fullscreen mode."));
        LgrBox.AddHandler(PointerReleasedEvent, OnLgrPointerReleased, RoutingStrategies.Tunnel);
        LgrBox.AddHandler(KeyDownEvent, OnLgrKeyDownBeforeOpen, RoutingStrategies.Tunnel);
        AddHandler(KeyDownEvent, OnPlayingKeyDown, RoutingStrategies.Tunnel, true);
        AddHandler(KeyUpEvent, OnPlayingKeyUp, RoutingStrategies.Tunnel, true);
        Settings = LevelEditorSettings.Load();
        ApplyToolbarIconSize();
        _lgrCache = new BookmarkLgrCache(() => Top.StorageProvider);
        _cursorManager = new AvaloniaCursorManager(this, () => RootDialogHost.IsOpen);
        _gameLoopRunner = new AvaloniaGameLoopRunner(RedrawScene);
        _playController = new PlayController(_gameLoopRunner);
        _playController.PlayingPaused += OnPlayingPaused;
        _controller = new LevelEditorController<SleEditorLev>(
            this,
            CreateNewEditorLevel(Level.FromDimensions(50, 50)));
        SyncRenderingSettingsToUi();
        _selectionFilter = new SelectionFilter(this);
        _tools = new SleEditorTools(
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
            new CustomShapeTool(this)
        );
        var textTool = new TextTool(this);
        _currentTool = _tools.SelectionTool;
        _buttonToolMap = new Dictionary<ToggleButton, IEditorTool>
        {
            { SelectButton, _tools.SelectionTool },
            { VertexButton, _tools.VertexTool },
            { DrawButton, _tools.DrawTool },
            { ObjectButton, _tools.ObjectTool },
            { PipeButton, _tools.PipeTool },
            { EllipseButton, _tools.EllipseTool },
            { PolyOpButton, _tools.PolyOpTool },
            { FrameButton, _tools.FrameTool },
            { SmoothenButton, _tools.SmoothenTool },
            { CutConnectButton, _tools.CutConnectTool },
            { AutoGrassButton, _tools.AutoGrassTool },
            { PictureButton, _tools.PictureTool },
            { TextButton, textTool },
            { CustomShapeButton, _tools.CustomShapeTool }
        };
        _zoomCtrl = new ZoomController(new ElmaCamera(), () => { });

        InitializeInternalMenu();
    }

    private TopLevel Top => TopLevel.GetTopLevel(this)!;

    Level ILevelEditor.Lev => _controller.Lev;
    public ElmaRenderer Renderer => _renderer!;
    ZoomController ILevelEditor.ZoomCtrl => _zoomCtrl;
    SceneSettings ILevelEditor.SceneSettings => _sceneSettings;
    IEditorCursorManager ILevelEditor.CursorManager => _cursorManager;
    PlayController ILevelEditor.PlayController => _playController;
    IPictureDialogService ILevelEditor.PictureDialogService => new AvaloniaPictureDialogService(Renderer);

    ICustomShapeService ILevelEditor.CustomShapeService => new AvaloniaCustomShapeService(
        () => Settings,
        () => TopLevel.GetTopLevel(this)?.StorageProvider,
        ShowError);

    IProgressService ILevelEditor.ProgressService => new AvaloniaProgressService();

    HighlightTarget? ILevelEditor.CurrentHighlight
    {
        get => _currentHighlight;
        set => _currentHighlight = value;
    }

    string ILevelEditor.HighlightText
    {
        get => HighlightLabel.Text ?? "";
        set => HighlightLabel.Text = value;
    }

    public void ShowError(string message, string caption)
    {
        SingleActionDialog dialog = new() { Message = message, ButtonText = "OK" };
        _ = dialog.ShowAsync();
    }

    void ILevelEditor.SetModified(LevModification value) => SetModified(value);
    void ILevelEditor.SignalVisualChange(LevVisualChange value) => SetPendingModification(value);
    void ILevelEditor.SignalRenderingSettingsChange() => _pendingSettingsUpdate = true;
    void ILevelEditor.RedrawScene() => RedrawScene();
    void ILevelEditor.TransformMenuItemClick() => TransformSelection();
    void ILevelEditor.UpdateUndoRedo() => UpdateUndoRedo();

    private async void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Focus();
        RegisterHotkeys();
        await TryRestoreLastLevel();
        await ShowBrowserWarningIfNeeded();
    }

    private async Task ShowBrowserWarningIfNeeded()
    {
        if (!OperatingSystem.IsBrowser() ||
            Settings.NonChromiumWarningShown ||
            BrowserInterop.IsChromiumBrowser())
        {
            return;
        }

        await new BrowserWarningDialog().ShowAsync();
        Settings.NonChromiumWarningShown = true;
        await Settings.Save();
    }

    private void OnDialogOpened(object? sender, DialogOpenedEventArgs e) => Cursor = Cursor.Default;

    private void OnDialogClosing(object? sender, DialogClosingEventArgs e) => Focus();

    private void UpdateAppTitle()
    {
        var filename = _controller.EditorLev.StorageFile?.Name;
        var levelName = filename != null ? Path.GetFileNameWithoutExtension(filename) : "New";
        var unsavedMarker = Settings.SaveState == LevSaveState.Unsaved ? "*" : "";
        var title = App.TitleWithVersion($"{levelName}{unsavedMarker} - SLE");

        if (OperatingSystem.IsBrowser())
        {
            BrowserInterop.SetDocumentTitle(title);
        }
        else if (Top is Window window)
        {
            window.Title = title;
        }
    }

    private void TransformSelection()
    {
        if (_currentTool.Busy)
        {
            return;
        }

        ChangeToolTo(_tools.TransformTool);

        if (!_currentTool.Busy)
        {
            ChangeToolTo(_tools.SelectionTool);
        }
    }

    private void SetModified(LevModification value)
    {
        SetPendingModification((LevVisualChange)value);
        _controller.SetModified(value, _renderer!, _currentTool, _playController, Settings);
        if (value != LevModification.Nothing)
        {
            Settings.SaveState = LevSaveState.Unsaved;
            UpdateSaveButtons();
            UpdateAppTitle();
            _ = Autosave();
            _ = Settings.Save();
        }
    }

    private void SetModifiedAndRender(LevModification value)
    {
        SetModified(value);
        RedrawScene();
    }

    private void UpdateUndoRedo()
    {
        UndoButton.IsEnabled = _controller.CanUndo;
        RedoButton.IsEnabled = _controller.CanRedo;
        UndoMenuItem.IsEnabled = _controller.CanUndo;
        RedoMenuItem.IsEnabled = _controller.CanRedo;
    }

    private void UpdateRendererBuffers(LevVisualChange mod) =>
        _renderer?.UpdateBuffers(new LevEditState(_controller.Lev, _currentTool.GetTransientElements(_hasFocus)), mod);

    private void SetPendingModification(LevVisualChange mod)
    {
        if (mod != LevVisualChange.Nothing)
        {
            if (_pendingModification != null)
            {
                _pendingModification |= mod;
            }
            else
            {
                _pendingModification = mod;
            }
        }
    }

    private async void OnZoomLabelClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new ZoomDialog(_zoomCtrl.ZoomLevel);
        var result = await dialog.ShowAsync();
        if (result.HasValue)
        {
            _zoomCtrl.ZoomLevel = result.Value;
            ZoomLabel.Content = $"Zoom: {_zoomCtrl.ZoomLevel:F3}";
            RedrawScene();
        }
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        if (Top is Window window)
        {
            window.Close();
        }
    }

    private async void OnNewClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!await ConfirmDiscardUnsavedChanges())
            {
                return;
            }

            var lev = await CreateBlankLevel();
            await InitializeLevel(lev, LevSaveState.New);
            await Settings.Save();
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not create a new level.");
        }
    }

    private void OnQuickGrassClick(object? sender, RoutedEventArgs e) => _controller.QuickGrass(_tools.AutoGrassTool);

    private void OnDeleteAllGrassClick(object? sender, RoutedEventArgs e) => _controller.DeleteAllGrass();

    private void OnUndoClick(object? sender, RoutedEventArgs e)
    {
        if (_controller.CanUndo && !_currentTool.Busy)
        {
            _controller.Undo();
            LoadFromHistory();
        }
    }

    private void OnRedoClick(object? sender, RoutedEventArgs e)
    {
        if (_controller.CanRedo && !_currentTool.Busy)
        {
            _controller.Redo();
            LoadFromHistory();
        }
    }

    private void OnZoomFillClick(object? sender, RoutedEventArgs e)
    {
        ZoomFill(_renderer!.AspectRatio);
        RedrawScene();
    }

    private async void OnCheckTopologyClick(object? sender, RoutedEventArgs e)
    {
        var checkVersion = ++_topologyCheckVersion;
        var topologyErrors = _controller.CheckTopologyErrors(_currentTool);
        TopologyErrorList.ItemsSource = topologyErrors.Count == 0
            ? new[] { "No problems." }
            : topologyErrors;
        var hasErrors = topologyErrors.Exists(msg =>
            !msg.StartsWith("Level has pictures that the LGR is missing"));
        TopologyCheckmark.Stroke = hasErrors
            ? Brushes.Red
            : topologyErrors.Count > 0
                ? Brushes.DarkOrange
                : TopologyOkBrush;
        FlyoutBase.ShowAttachedFlyout(CheckTopologyButton);
        RedrawScene();

        if (topologyErrors.Count == 0)
        {
            await Task.Delay(500);
            if (checkVersion == _topologyCheckVersion)
            {
                FlyoutBase.GetAttachedFlyout(CheckTopologyButton)?.Hide();
            }
        }
    }

    private void OnSelectAllClick(object? sender, RoutedEventArgs e)
    {
        _controller.SelectAll();
        RedrawScene();
    }

    private void OnCopyClick(object? sender, RoutedEventArgs e) => _controller.CopySelected(_zoomCtrl);

    private async void OnSaveShapeClick(object? sender, RoutedEventArgs e) => await _tools.CustomShapeTool.SaveShape();

    private void OnMirrorHClick(object? sender, RoutedEventArgs e) =>
        _controller.MirrorSelected(MirrorOption.Horizontal);

    private void OnMirrorVClick(object? sender, RoutedEventArgs e) => _controller.MirrorSelected(MirrorOption.Vertical);

    private void OnDeleteSelectedClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeleteSelected(_currentTool);
        RedrawScene();
    }

    private void OnUnionClick(object? sender, RoutedEventArgs e)
    {
        if (_tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Union, _controller.Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void OnDifferenceClick(object? sender, RoutedEventArgs e)
    {
        if (_tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, _controller.Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void OnIntersectionClick(object? sender, RoutedEventArgs e)
    {
        if (_tools.PolyOpTool.PolyOpSelected(PolygonOperationType.Intersection, _controller.Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void OnSymDiffClick(object? sender, RoutedEventArgs e)
    {
        if (_tools.PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, _controller.Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void OnFixSelfIntClick(object? sender, RoutedEventArgs e)
    {
        if (_tools.PolyOpTool.FixSelfIntersections(_controller.Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private async void OnTexturizeClick(object? sender, RoutedEventArgs e)
    {
        await _controller.TexturizeSelection();
        RedrawScene();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        UpdateVisualSize();
        return size;
    }

    private void ZoomFill(double aspectRatio)
    {
        _controller.Lev.UpdateBounds();
        _zoomCtrl.ZoomFill(Settings.RenderingSettings, aspectRatio, _controller.Lev);
        Dispatcher.UIThread.Post(() => { ZoomLabel.Content = $"Zoom: {_zoomCtrl.ZoomLevel:F3}"; },
            DispatcherPriority.Background);
    }

    private void LoadFromHistory()
    {
        _controller.LoadFromHistory(Settings.RenderingSettings);
        PopulateTextureBoxes(
            _lgrCache.TryGetLoaded(_controller.Lev.LgrFile.ToLower()) ??
            _lgrCache.TryGetLoaded("default"));
        _pendingSettingsUpdate = true;
        UpdateUndoRedo();
        ChangeToolTo(_currentTool);
    }
}
