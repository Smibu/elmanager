using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor.Playing;
using Elmanager.LevelEditor.Tools;
using Elmanager.Lgr;
using Elmanager.Physics;
using Elmanager.Properties;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Elmanager.Settings;
using Elmanager.UI;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using Envelope = NetTopologySuite.Geometries.Envelope;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;
using Polygon = Elmanager.Lev.Polygon;
using Timer = System.Timers.Timer;

namespace Elmanager.LevelEditor;

internal partial class LevelEditorForm : FormMod, IMessageFilter
{
    //TODO Tool interface should be improved
    private const string CoordinateFormat = "F3";
    private const string LevEditorName = "SLE";
    private const int MouseWheelStep = 20;
    private const bool Physics = false;
    private readonly List<Level> _history = new();
    private bool _appleFilter = true;
    private bool _flowerFilter = true;
    private bool _grassFilter = true;
    private bool _groundFilter = true;
    private bool _killerFilter = true;
    private bool _pictureFilter = true;
    private bool _textureFilter = true;
    internal IEditorTool CurrentTool = null!;
    internal Level Lev = null!;
    private ElmaFile? _file;
    internal ElmaRenderer Renderer = null!;
    internal IEditorTool[] Tools = null!;
    private List<string>? _currLevDirFiles;
    private bool _draggingScreen;
    private Lgr.Lgr? _editorLgr;
    private List<Vector> _errorPoints = new();
    private int _historyIndex;
    private int _savedIndex;
    private string? _loadedLevFilesDir;
    private int _lockCoord;
    private bool _lockMouseX;
    private bool _lockMouseY;
    private bool _modified;
    private Vector _moveStartPosition;
    private int _selectedObjectCount;
    private int _selectedObjectIndex;
    private int _selectedPictureCount;
    private int _selectedPictureIndex;
    private int _selectedPolygonCount;
    private int _selectedTextureCount;
    private int _selectedVerticeCount;
    private bool PictureToolAvailable => _editorLgr != null;
    private bool _draggingGrid;
    private Vector _gridStartOffset;
    private bool _programmaticPropertyChange;
    private Vector? _savedStartPosition;
    private float _dpiX;
    private float _dpiY;
    private Vector? _contextMenuClickPosition;
    private SvgImportOptions _svgImportOptions = SvgImportOptions.Default;
    private bool _maybeOpenOnDrop;
    private ZoomController _zoomCtrl = null!;
    private readonly SceneSettings _sceneSettings = new();
    private readonly TaskCompletionSource _tcs = new();
    private readonly FullScreenController _fullScreenController;

    internal LevelEditorForm(string levPath)
    {
        InitializeComponent();
        _fullScreenController = CreateFullScreenController();
        TryLoadLevel(levPath);
        PostInit();
    }

    private FullScreenController CreateFullScreenController() =>
        new(this, ViewerResized, new List<Control> { ToolPanel, MenuStrip1, ToolStripPanel1, StatusStrip1 });

    internal void SetLevel(ElmaFileObject<Level> lev)
    {
        SetExistingLev(lev);
        InitializeLevel();
    }

    private void TryLoadLevel(string levPath)
    {
        try
        {
            var lev = Level.FromPath(levPath);
            SetExistingLev(lev);
        }
        catch (BadFileException ex)
        {
            UiUtils.ShowError("Error occurred while loading level file: " + ex.Message, "Warning",
                MessageBoxIcon.Exclamation);
            SetBlankLevel();
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
            SetBlankLevel();
            ShowWarning($"The last opened level {levPath} was not found.");
        }
    }

    private void SetExistingLev(ElmaFileObject<Level> lev)
    {
        Lev = lev.Obj;
        _file = lev.File;
        if (Settings.EnableStartPositionFeature)
        {
            SaveStartPosition();
        }
    }

    public LevelEditorForm()
    {
        InitializeComponent();
        _fullScreenController = CreateFullScreenController();
        if (Settings.LastLevel != null)
        {
            TryLoadLevel(Settings.LastLevel);
        }
        else
            SetBlankLevel();
        PostInit();
    }

    private void PostInit()
    {
        System.Windows.Forms.Application.AddMessageFilter(this);
        EditorControl.HandleCreated += (_, _) =>
        {
            EditorControl.Context.SwapInterval = 0;
            Initialize();
            _tcs.SetResult();
        };
        Closed += (_, _) => System.Windows.Forms.Application.RemoveMessageFilter(this);
    }

    internal async Task WaitInit()
    {
        await _tcs.Task;
    }

    internal bool Modified => _modified;

    internal bool EffectiveAppleFilter => _appleFilter &&
                                          (ShowObjectFramesButton.Checked ||
                                           (ShowObjectsButton.Checked && PictureToolAvailable));

    internal bool EffectiveKillerFilter => _killerFilter &&
                                           (ShowObjectFramesButton.Checked ||
                                            (ShowObjectsButton.Checked && PictureToolAvailable));

    internal bool EffectiveFlowerFilter => _flowerFilter &&
                                           (ShowObjectFramesButton.Checked ||
                                            (ShowObjectsButton.Checked && PictureToolAvailable));

    internal bool EffectiveGrassFilter => _grassFilter &&
                                          (ShowGrassEdgesButton.Checked ||
                                           (showGrassButton.Checked && PictureToolAvailable));

    internal bool EffectiveGroundFilter => _groundFilter &&
                                           (ShowGroundEdgesButton.Checked ||
                                            (ShowGroundButton.Checked && PictureToolAvailable));

    internal bool EffectiveTextureFilter => _textureFilter &&
                                            (ShowTextureFramesButton.Checked ||
                                             (ShowTexturesButton.Checked && PictureToolAvailable));

    internal bool EffectivePictureFilter => _pictureFilter &&
                                            (ShowPictureFramesButton.Checked ||
                                             (ShowPicturesButton.Checked && PictureToolAvailable));

    private int SelectedElementCount => _selectedObjectCount + _selectedPictureCount + _selectedVerticeCount +
                                        _selectedTextureCount;

    private ToolBase ToolBase => ((ToolBase)CurrentTool);

    private List<string>? CurrLevDirFiles
    {
        get
        {
            UpdateCurrLevDirFiles();
            return _currLevDirFiles;
        }
    }

    internal ZoomController ZoomCtrl => _zoomCtrl;

    internal SceneSettings SceneSettings => _sceneSettings;

    internal PlayController PlayController { get; } = new() { Settings = Global.AppSettings.LevelEditor.PlayingSettings };
    public Lgr.Lgr? EditorLgr => _editorLgr;

    internal void TransformMenuItemClick(object? sender = null, EventArgs? e = null)
    {
        if (!CurrentTool.Busy)
        {
            ChangeToDefaultCursor();
            CurrentTool.InActivate();
            CurrentTool = Tools[12];
            ActivateCurrentAndRedraw();

            // if not busy, there's nothing to transform
            if (!CurrentTool.Busy)
            {
                CurrentTool = Tools[0];
                ActivateCurrentAndRedraw();
            }
        }
    }

    internal void RedrawScene(object? sender = null, EventArgs? e = null)
    {
        if (PlayController.PlayingOrPaused)
        {
            return;
        }

        DoRedrawScene();
    }

    private void ActivateCurrentAndRedraw()
    {
        CurrentTool.Activate();
        RedrawScene();
    }

    private void InactivateCurrentAndRedraw()
    {
        CurrentTool.InActivate();
        RedrawScene();
    }

    internal void SetModified(LevModification value, bool updateHistory = true)
    {
        var wasModified = value != LevModification.Nothing;
        _modified = wasModified || _modified;
        if (wasModified)
        {
            EnableSaveButtons(true);
        }
        if (wasModified)
        {
            Lev.UpdateBounds();
            if (updateHistory)
                AddToHistory();
            if (Settings.CheckTopologyDynamically)
                CheckTopology();
        }

        if (value.HasFlag(LevModification.Ground) || value.HasFlag(LevModification.Objects))
        {
            PlayController.UpdateEngine(Lev);
        }
    }

    private void EnableSaveButtons(bool value)
    {
        SaveButton.Enabled = value;
        SaveToolStripMenuItem.Enabled = value;
    }

    internal void UpdateSelectionInfo()
    {
        _selectedVerticeCount = 0;
        _selectedPolygonCount = 0;
        _selectedObjectCount = 0;
        _selectedPictureCount = 0;
        _selectedTextureCount = 0;
        foreach (Polygon x in Lev.Polygons)
        {
            bool hasSelectedVertices = false;
            foreach (Vector z in x.Vertices)
            {
                if (z.Mark == VectorMark.Selected)
                {
                    hasSelectedVertices = true;
                    _selectedVerticeCount++;
                }
            }

            if (hasSelectedVertices)
                _selectedPolygonCount++;
        }

        foreach (LevObject x in Lev.Objects)
            if (x.Position.Mark == VectorMark.Selected)
                _selectedObjectCount++;
        foreach (GraphicElement x in Lev.GraphicElements)
            if (x.Position.Mark == VectorMark.Selected)
                if (x is GraphicElement.Picture)
                    _selectedPictureCount++;
                else
                    _selectedTextureCount++;
        SelectionLabel.Text = "Selected " + _selectedVerticeCount + " vertices of " + _selectedPolygonCount +
                              " polygons, " + _selectedObjectCount + " objects, " + _selectedPictureCount +
                              " pictures, " + _selectedTextureCount + " textures.";
        MirrorHorizontallyToolStripMenuItem.Enabled = SelectedElementCount >= 2;
    }

    private void AddToHistory()
    {
        if (_historyIndex < _history.Count - 1)
        {
            _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);
            _historyIndex = _history.Count - 1;
        }

        _history.Add(Lev.Clone());
        _historyIndex++;
        if (_historyIndex <= _savedIndex)
        {
            _savedIndex = -1;
        }

        UpdateUndoRedo();
    }

    private void AfterSettingsClosed()
    {
        Renderer.UpdateSettings(Settings.RenderingSettings);
        UpdateLgrTools();
        UpdateButtons();
        RedrawScene();
    }

    private void AutoGrassButtonChanged(object? sender, EventArgs e)
    {
        if (AutoGrassButton.Checked)
            ChangeToolTo(11);
    }

    private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
    {
        var mod = LevModification.Nothing;
        if (_selectedObjectIndex >= 0)
        {
            var obj = Lev.Objects[_selectedObjectIndex];
            Lev.Objects.RemoveAt(_selectedObjectIndex);
            Lev.Objects.Add(obj);
            mod = LevModification.Objects;
        }
        else if (_selectedPictureIndex >= 0)
        {
            var obj = Lev.GraphicElements[_selectedPictureIndex];
            Lev.GraphicElements.RemoveAt(_selectedPictureIndex);
            Lev.GraphicElements.Insert(0, obj);
            mod = LevModification.Decorations;
        }

        SetModified(mod);
    }

    public void ChangeToDefaultCursor()
    {
        EditorControl.Cursor = Cursors.Default;
    }

    private void ChangeToolTo(int index)
    {
        CurrentTool.InActivate();
        CurrentTool = Tools[index];
        ActivateCurrentAndRedraw();
    }

    private void CheckForPictureLoss()
    {
        if (!Lev.AllPicturesFound)
        {
            var text = "Level has pictures that the LGR is missing: " +
                       string.Join(", ", Lev.MissingElements.Select(m => $"{m.Item2}")) +
                       ". These pictures are lost if you save this level.";
            ShowWarning(text);
        }
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
        var items = topologyList.DropDownItems;
        if (!CurrentTool.Busy)
        {
            items.Clear();
            ResetTopologyListStyle();
            topologyList.Text = "Checking topology...";
            ToolStrip2.Refresh();
            _errorPoints.Clear();
            if (Lev.TooWide)
                items.Add(
                    "Level is too wide. Current width: " + Lev.Width + ", maximum width: " + Level.MaximumSize);
            if (Lev.TooTall)
                items.Add("Level is too tall. Current height: " + Lev.Height + ", maximum height: " +
                          Level.MaximumSize);
            if (Lev.HasTooLargePolygons)
                items.Add("There are polygons with too many vertices in the level.");
            if (Lev.HasTooManyObjects)
                items.Add("There are too many objects in the level. Current: " + Lev.Objects.Count + ", maximum: " +
                          Level.MaximumObjectCount);
            if (Lev.HasTooFewObjects)
                items.Add("There must be at least one object in the level (in addition to the start object).");
            if (Lev.HasTooManyPolygons)
                items.Add("There are too many polygons in the level. Current: " + Lev.Polygons.Count +
                          ", maximum: " +
                          Level.MaximumPolygonCount);
            if (Lev.HasTooManyVertices)
                items.Add("There are too many ground vertices in the level. Current: " + Lev.GroundVertexCount +
                          ", maximum: " +
                          Level.MaximumGroundVertexCount);
            if (Lev.HasTooManyPictures)
                items.Add("There are too many pictures and textures in the level. Current: " +
                          Lev.PictureTextureCount + ", maximum: " +
                          Level.MaximumPictureTextureCount);
            if (Lev.HeadTouchesGround)
                items.Add("The driver\'s head is touching ground.");
            if (Lev.WheelLiesOnEdge)
                items.Add("The driver\'s wheel is lying on an edge.");
            if (Lev.HasTexturesOutOfBounds)
                items.Add("Some textures are too far outside of the level polygons.");

            _errorPoints = Lev.GetIntersectionPoints();
            if (_errorPoints.Count > 0)
                items.Add("There are intersections in the level.");

            var errObjs = Lev.GetApplesAndFlowersInsideGround();
            if (errObjs.Count > 0)
            {
                foreach (var errObj in errObjs)
                    _errorPoints.Add(errObj.Position);
                items.Add("Some apples and/or flowers are inside ground.");
            }

            var shortEdges = Lev.GetTooShortEdges();
            if (shortEdges.Count > 0)
            {
                _errorPoints.AddRange(shortEdges);
                items.Add("Some polygon edges are too short.");
            }

            var c = items.Count;
            if (c == 0)
            {
                topologyList.Text = "No problems.";
                ResetTopologyListStyle();
            }
            else
            {
                if (c > 1)
                {
                    topologyList.Text = c + " problems were found!";
                }
                else
                {
                    topologyList.Text = "1 problem was found!";
                }

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
        _history.Clear();
        _history.Add(Lev.Clone());
        _historyIndex = 0;
        _savedIndex = -1;
        UpdateUndoRedo();
    }

    private async void ConfirmClose(object sender, CancelEventArgs e)
    {
        if (!PromptToSaveIfModified())
            e.Cancel = true;
        else
        {
            InactivateCurrentAndRedraw();
        }

        if (WindowState == FormWindowState.Normal)
        {
            Settings.Size = Size;
        }

        Settings.WindowState = WindowState;
        Settings.LastLevel = _file?.Path;
        if (PlayController.PlayingOrPaused)
        {
            e.Cancel = true;
            await PlayController.StopPlaying();
            Close();
        }
    }

    private void CopyMenuItemClick(object sender, EventArgs e)
    {
        var copiedPolygons = new List<Polygon>();
        var copiedObjects = new List<LevObject>();
        var copiedTextures = new List<GraphicElement>();
        Vector.MarkDefault = VectorMark.Selected;
        var delta = Keyboard.IsKeyDown(Key.LeftShift)
            ? Settings.RenderingSettings.GridSize
            : _zoomCtrl.Cam.ZoomLevel * 0.1;
        foreach (Polygon x in Lev.Polygons)
        {
            var copy = new Polygon();
            for (var index = 0; index < x.Vertices.Count; index++)
            {
                Vector z = x.Vertices[index];
                if (z.Mark == VectorMark.Selected)
                {
                    x.Vertices[index] = new Vector(z.X, z.Y, VectorMark.None);
                    copy.Add(new Vector(z.X + delta,
                        z.Y - delta));
                }
            }

            if (copy.Vertices.Count > 2)
            {
                copiedPolygons.Add(copy);
                copy.IsGrass = x.IsGrass;
                copy.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, Settings.RenderingSettings.GrassZoom);
            }
        }

        foreach (LevObject x in Lev.Objects)
        {
            if (x.Mark == VectorMark.Selected && x.Type != ObjectType.Start)
            {
                x.Mark = VectorMark.None;
                copiedObjects.Add(
                    new LevObject(
                        x.Position +
                        new Vector(delta,
                            -delta), x.Type, x.AppleType,
                        x.AnimationNumber));
            }
        }

        foreach (GraphicElement x in Lev.GraphicElements)
        {
            if (x.Position.Mark == VectorMark.Selected)
            {
                var copiedGraphicElement = x with { Position = new Vector(x.X + delta, x.Y - delta) };
                copiedTextures.Add(copiedGraphicElement);
                x.Mark = VectorMark.None;
            }
        }

        Vector.MarkDefault = VectorMark.None;
        Lev.Polygons.AddRange(copiedPolygons);
        Lev.Objects.AddRange(copiedObjects);
        Lev.GraphicElements.AddRange(copiedTextures);
        var mod = LevModification.Nothing;
        if (copiedObjects.Count > 0)
        {
            mod |= LevModification.Objects;
        }
        if (copiedPolygons.Count > 0)
        {
            mod |= LevModification.Ground;
        }
        if (copiedTextures.Count > 0)
        {
            mod |= LevModification.Decorations;
        }
        SetModified(mod);
        RedrawScene();
    }

    private bool CurrLevDirExists() => _file?.FileInfo.Directory?.Exists ?? false;

    private void DoRedrawScene()
    {
        _sceneSettings.AdditionalPolys = CurrentTool.GetExtraPolygons();
        var jf = PlayController.Playing && PlayController.FollowDriver ? _zoomCtrl.Cam.FixJitter() : new Vector();
        Renderer.DrawScene(_zoomCtrl.Cam, _sceneSettings);

        var drawAction = GetVertexDrawAction();

        if (Settings.ShowCrossHair)
        {
            var mouse = GetMouseCoordinates();
            Renderer.DrawDashLine(_zoomCtrl.Cam.XMin, mouse.Y, _zoomCtrl.Cam.XMax,
                mouse.Y, Settings.CrosshairColor);
            Renderer.DrawDashLine(mouse.X, _zoomCtrl.Cam.YMin, mouse.X,
                _zoomCtrl.Cam.YMax, Settings.CrosshairColor);
        }

        foreach (Polygon x in Lev.Polygons)
        {
            switch (x.Mark)
            {
                case PolygonMark.Highlight:
                    if (Settings.UseHighlight)
                        if (x.IsGrass)
                        {
                            Renderer.DrawGrassPolygon(x, Settings.HighlightColor,
                                Settings.RenderingSettings.ShowInactiveGrassEdges);
                        }
                        else
                            Renderer.DrawPolygon(x, Settings.HighlightColor);

                    break;
                case PolygonMark.Selected:
                    Renderer.DrawPolygon(x, Color.Red);
                    break;
                case PolygonMark.Erroneous:
                    Renderer.DrawPolygon(x, Color.Red);
                    break;
            }

            foreach (Vector z in x.Vertices)
            {
                switch (z.Mark)
                {
                    case VectorMark.Selected:
                        drawAction(z, Settings.SelectionColor);
                        break;
                    case VectorMark.Highlight:
                        if (Settings.UseHighlight)
                            drawAction(z, Settings.HighlightColor);
                        break;
                }
            }
        }

        foreach (LevObject t in Lev.Objects)
        {
            Vector z = t.Position;
            switch (z.Mark)
            {
                case VectorMark.Selected:
                    drawAction(z, Settings.SelectionColor);
                    break;
                case VectorMark.Highlight:
                    if (Settings.UseHighlight)
                        drawAction(z, Settings.HighlightColor);
                    break;
            }
        }

        foreach (GraphicElement t in Lev.GraphicElements)
        {
            Vector z = t.Position;
            switch (z.Mark)
            {
                case VectorMark.Selected:
                    Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y - t.Height,
                        Settings.SelectionColor);
                    break;
                case VectorMark.Highlight:
                    Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y - t.Height,
                        Settings.HighlightColor);
                    break;
            }
        }

        foreach (Vector x in _errorPoints)
            Renderer.DrawSquare(x, _zoomCtrl.Cam.ZoomLevel / 25, Color.Red);
        if (_savedStartPosition is { } p)
        {
            if (Settings.RenderingSettings.ShowObjects)
            {
                Renderer.DrawDummyPlayer(p.X, p.Y, _sceneSettings, new PlayerRenderOpts(Color.Green, false, true, true));
            }

            if (Settings.RenderingSettings.ShowObjectFrames)
            {
                Renderer.DrawDummyPlayer(p.X, p.Y, _sceneSettings, new PlayerRenderOpts(Color.Green, false, false, true));
            }
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
        }

        if (PlayController.PlayingOrPaused)
        {
            GL.Translate(-jf.X, -jf.Y, 0);
            var driver = PlayController.Driver!;
            if (Settings.RenderingSettings.ShowObjects && Renderer.OpenGlLgr != null)
            {
                Renderer.DrawPlayer(driver.GetState(), PlayController.RenderOptsLgr, _sceneSettings);
            }
            else if (Settings.RenderingSettings.ShowObjectFrames)
            {
                Renderer.DrawPlayer(driver.GetState(), PlayController.RenderOptsFrame, _sceneSettings);
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            switch (PlayController.PlayerSelection)
            {
                case VectorMark.Selected:
                    drawAction(driver.Body.Location, Settings.SelectionColor);
                    break;
                case VectorMark.Highlight:
                    drawAction(driver.Body.Location, Settings.HighlightColor);
                    break;
            }
            GL.Translate(jf.X, jf.Y, 0);
        }

        CurrentTool.ExtraRendering();

        Renderer.Swap();
    }

    internal LevelEditorSettings Settings => Global.AppSettings.LevelEditor;

    private Action<Vector, Color> GetVertexDrawAction()
    {
        var drawAction = Settings.RenderingSettings.UseCirclesForVertices
            ? (Action<Vector, Color>)((pt, color) => Renderer.DrawPoint(pt, color))
            : ((pt, color) => Renderer.DrawEquilateralTriangle(pt,
                _zoomCtrl.Cam.ZoomLevel * Settings.RenderingSettings.VertexSize, color));
        return drawAction;
    }

    private void CutButtonChanged(object? sender, EventArgs e)
    {
        if (CutConnectButton.Checked)
            ChangeToolTo(10);
    }

    private void DeleteAllGrassToolStripMenuItemClick(object? sender, EventArgs e)
    {
        var mod = LevModification.Nothing;
        for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
        {
            Polygon x = Lev.Polygons[i];
            if (x.IsGrass)
            {
                mod = LevModification.Decorations;
                Lev.Polygons.Remove(x);
            }
        }

        SetModified(mod);
        RedrawScene();
    }

    private void DeleteSelected(object? sender, EventArgs? e)
    {
        if (!CurrentTool.Busy)
        {
            var mod = LevModification.Nothing;
            for (int j = Lev.Polygons.Count - 1; j >= 0; j--)
            {
                bool polyModified = false;
                Polygon x = Lev.Polygons[j];
                for (int i = x.Vertices.Count - 1; i >= 0; i--)
                {
                    if (x.Vertices[i].Mark == VectorMark.Selected &&
                        (Lev.Polygons.Count > 1 || x.Vertices.Count > 3))
                    {
                        x.Vertices.RemoveAt(i);
                        if (x.IsGrass)
                        {
                            mod |= LevModification.Decorations;
                        }
                        else
                        {
                            mod |= LevModification.Ground;
                        }
                        polyModified = true;
                    }
                }

                if (x.Vertices.Count < 3)
                    Lev.Polygons.Remove(x);
                else if (polyModified)
                    x.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, Settings.RenderingSettings.GrassZoom);
            }

            var deletedApples = new HashSet<int>();
            for (int i = Lev.Objects.Count - 1; i >= 0; i--)
            {
                if (Lev.Objects[i].Position.Mark == VectorMark.Selected)
                {
                    if (Lev.Objects[i].Type == ObjectType.Start)
                        continue;
                    mod |= LevModification.Objects;
                    if (Lev.Objects[i].Type == ObjectType.Apple)
                    {
                        deletedApples.Add(i);
                    }
                    Lev.Objects.RemoveAt(i);
                }
            }

            for (int i = Lev.GraphicElements.Count - 1; i >= 0; i--)
            {
                GraphicElement x = Lev.GraphicElements[i];
                if (x.Position.Mark == VectorMark.Selected)
                {
                    Lev.GraphicElements.Remove(x);
                    mod |= LevModification.Decorations;
                }
            }

            PlayController.NotifyDeletedApples(deletedApples);
            SetModified(mod);
            UpdateSelectionInfo();
        }
    }

    private void DrawButtonChanged(object? sender, EventArgs e)
    {
        if (DrawButton.Checked)
            ChangeToolTo(2);
    }

    private void EllipseButtonChanged(object? sender, EventArgs e)
    {
        if (EllipseButton.Checked)
            ChangeToolTo(6);
    }

    private void ExitToolStripMenuItemClick(object? sender, EventArgs e)
    {
        Close();
    }

    private void FilterChanged(object? sender, EventArgs e)
    {
        _groundFilter = GroundPolygonsToolStripMenuItem.Checked;
        _grassFilter = GrassPolygonsToolStripMenuItem.Checked;
        _appleFilter = ApplesToolStripMenuItem.Checked;
        _killerFilter = KillersToolStripMenuItem.Checked;
        _flowerFilter = FlowersToolStripMenuItem.Checked;
        _pictureFilter = PicturesToolStripMenuItem.Checked;
        _textureFilter = TexturesToolStripMenuItem.Checked;
        SelectionFilterToolStripMenuItem.ShowDropDown();
    }

    private void FrameButtonChanged(object? sender, EventArgs e)
    {
        if (FrameButton.Checked)
            ChangeToolTo(8);
    }

    private Vector GetMouseCoordinates()
    {
        var mousePosNoTr = Invoke(() => EditorControl.PointToClient(MousePosition));
        var mousePos = new Vector
        {
            X =
                _zoomCtrl.Cam.XMin +
                mousePosNoTr.X * (_zoomCtrl.Cam.XMax - _zoomCtrl.Cam.XMin) / EditorControl.Width,
            Y =
                _zoomCtrl.Cam.YMax -
                mousePosNoTr.Y * (_zoomCtrl.Cam.YMax - _zoomCtrl.Cam.YMin) / EditorControl.Height
        };
        return mousePos;
    }

    private void HandleGrassMenu(object sender, EventArgs e)
    {
        var polys = Lev.Polygons.GetSelectedPolygons(includeGrass: true).ToList();
        if (!polys.Any() && _grassInfo is not null)
        {
            polys.Add(_grassInfo.Polygon);
        }

        var mod = LevModification.Nothing;
        polys.ForEach(p =>
        {
            p.IsGrass = !p.IsGrass;
            if (!p.IsGrass)
            {
                mod |= LevModification.Ground;
            }
            else
            {
                mod |= LevModification.Decorations;
            }
            p.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, Settings.RenderingSettings.GrassZoom);
        });
        SetModified(mod);
        RedrawScene();
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

        if (_selectedObjectIndex >= 0)
        {
            var currApple = Lev.Objects[_selectedObjectIndex];
            if (currApple.Position.Mark == VectorMark.Selected)
            {
                Lev.Objects.Where(
                        obj => obj.Position.Mark == VectorMark.Selected && obj.Type == ObjectType.Apple)
                    .ToList()
                    .ForEach(apple => apple.AppleType = chosenAppleType);
            }
            else
            {
                currApple.AppleType = chosenAppleType;
            }
            SetModified(LevModification.Objects);
        }
        else
        {
            PlayController.UpdateGravity(chosenAppleType);
        }
    }

    private void Initialize()
    {
        if (!Physics)
#pragma warning disable CS0162
        {
            playButton.Visible = false;
            stopButton.Visible = false;
            settingsButton.Visible = false;
        }
#pragma warning restore CS0162
        PlayController.PlayingPaused += () => Invoke(SetNotPlaying);
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
        WindowState = Settings.WindowState;
        SelectButton.Select();
        UpdateButtons();
        Size = Settings.Size;
        Renderer = new ElmaRenderer(EditorControl, Settings.RenderingSettings);

        Tools = new IEditorTool[]
        {
            new SelectionTool(this),
            new VertexTool(this),
            new DrawTool(this),
            new ObjectTool(this),
            new PipeTool(this),
            new ZoomTool(this),
            new EllipseTool(this),
            new PolyOpTool(this),
            new FrameTool(this),
            new SmoothenTool(this),
            new CutConnectTool(this),
            new AutoGrassTool(this),
            new TransformTool(this),
            new PictureTool(this),
            new TextTool(this)
        };
        CurrentTool = Tools[0];
        SetupEventHandlers();
        InitializeLevel();
    }

    private async void InitializeLevel()
    {
        await PlayController.NotifyLevelChanged();
        PlayTimeLabel.Text = "";
        _zoomCtrl = new ZoomController(new ElmaCamera(), Lev, Settings.RenderingSettings, () => RedrawScene());
        SetNotModified();
        Renderer.InitializeLevel(Lev);
        Renderer.UpdateSettings(Settings.RenderingSettings);
        Lev.UpdateBounds();
        _zoomCtrl.ZoomFill();
        topologyList.Text = string.Empty;
        topologyList.DropDownItems.Clear();
        ResetTopologyListStyle();
        UpdateLgrTools();
        UpdateLabels();
        ClearHistory();
        UpdatePrevNextButtons();
        CurrentTool.InActivate();
        ActivateCurrentAndRedraw();
        _errorPoints.Clear();
    }

    private void KeyHandlerDown(object? sender, KeyEventArgs e)
    {
        e = e.KeyCode switch
        {
            Keys.Add => new KeyEventArgs(KeyUtils.Increase),
            Keys.Subtract => new KeyEventArgs(KeyUtils.Decrease),
            _ => e
        };

        CurrentTool.KeyDown(e);
        var wasModified = false;
        switch (e.KeyCode)
        {
            case Keys.Oem5:
                TexturizeSelection();
                break;
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
                if (!PlayController.PlayingOrPaused)
                {
                    CameraUtils.BeginArrowScroll(() => RedrawScene(), _zoomCtrl);
                }
                break;
            case Keys.C:
                if (!_lockMouseX)
                {
                    _lockMouseX = true;
                    _lockCoord = MousePosition.X;
                }

                break;
            case Keys.X:
                if (!_lockMouseY)
                {
                    _lockMouseY = true;
                    _lockCoord = MousePosition.Y;
                }

                break;
            case Keys.Delete:
                DeleteSelected(null, null);
                break;
            case Keys.Oemcomma:
                wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.Union, Lev.Polygons);
                break;
            case Keys.OemPeriod:
                wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, Lev.Polygons);
                break;
#pragma warning disable CS0162
            case Keys.Enter when Physics:
                PlayController.UpdateInputKeys();
                playButton_Click(null, null);
                break;
#pragma warning restore CS0162
            case Keys.Oem2:
                wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons);
                break;
            case Keys.Escape:
                if (!PlayController.PlayingOrPaused)
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

    private async void TexturizeSelection()
    {
        if (Renderer.OpenGlLgr is null)
        {
            UiUtils.ShowError("You need to select LGR file from settings before you can use texturize tool.", "Note",
                MessageBoxIcon.Information);
            return;
        }

        var selected = Lev.Polygons.GetSelectedPolygonsAsMultiPolygon();
        if (selected.IsEmpty)
        {
            return;
        }

        var picForm = new PictureForm(Renderer.OpenGlLgr.CurrentLgr, null)
        {
            AutoTextureMode = true,
            AllowMultiple = false,
            SetDefaultsAutomatically = true
        };
        if (_texturizationOpts is { })
        {
            picForm.TexturizationOptions = _texturizationOpts;
        }
        picForm.SetDefaultDistanceAndClipping();
        picForm.ShowDialog();
        if (picForm.Selection is not ImageSelection.TextureSelection sel)
        {
            return;
        }

        var opts = picForm.TexturizationOptions;
        _texturizationOpts = opts;

        var masks = opts.SelectedMasks.Select(x => Renderer.OpenGlLgr.DrawableImageFromName(Renderer.OpenGlLgr.CurrentLgr.ImageFromName(x)!)).ToList();
        var texture = Renderer.OpenGlLgr.DrawableImageFromName(sel.Txt);
        var rects = masks
            .Select(i => new Envelope(0, i.WidthMinusMargin, 0, i.HeightMinusMargin));

        var src = new CancellationTokenSource();

        var progress = new Progress<double>();
        var task = Task.Factory.StartNew(() => selected.FindCovering(rects, src.Token, progress,
            iterations: opts.Iterations,
            minRectCover: opts.MinCoverPercentage / 100).ToList(), src.Token);

        var progressForm = new ProgressDialog(task, src, progress);
        BeginInvoke(() => { progressForm.ShowDialog(); });
        List<Envelope> covering;
        try
        {
            covering = await task;
        }
        catch (PolygonException e)
        {
            UiUtils.ShowError(e.Message);
            return;
        }
        catch (OperationCanceledException)
        {
            return;
        }

        var selmasks =
            covering.Select(env =>
                masks.First(m => Math.Abs(m.WidthMinusMargin * m.HeightMinusMargin - env.Area) < 0.001));
        var pics = selmasks.Zip(covering,
            (m, c) =>
                GraphicElement.Text(sel.Clipping!.Value, sel.Distance!.Value,
                    new Vector(c.MinX - m.EmptyPixelXMargin, c.MaxY + m.EmptyPixelYMargin), texture, m));
        Lev.GraphicElements.AddRange(pics);
        SetModified(LevModification.Decorations);
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
            case Keys.C:
                _lockMouseX = false;
                break;
            case Keys.X:
                _lockMouseY = false;
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
        var levelProperties = new LevelPropertiesForm(Lev, _file);
        levelProperties.ShowDialog();
    }

    private void LevelPropertyModified(object? sender, EventArgs e)
    {
        if (!_programmaticPropertyChange)
        {
            Lev.Title = TitleBox.Text;
            Lev.LgrFile = LGRBox.Text;
            if (sender is not null && (sender.Equals(SkyComboBox) || sender.Equals(GroundComboBox)))
            {
                if (sender.Equals(GroundComboBox))
                    Lev.GroundTextureName = GroundComboBox.SelectedItem.ToString() ?? "ground";
                if (sender.Equals(SkyComboBox))
                    Lev.SkyTextureName = SkyComboBox.SelectedItem.ToString() ?? "sky";
                if (Settings.RenderingSettings.DefaultGroundAndSky)
                    UiUtils.ShowError("Default ground and sky is enabled, so you won\'t see this change in editor.",
                        "Warning", MessageBoxIcon.Exclamation);
                Renderer.OpenGlLgr?.UpdateGroundAndSky(Lev, Settings.RenderingSettings.DefaultGroundAndSky);
                RedrawScene();
            }

            SetModified(LevModification.Decorations);
        }
    }

    private void LoadFromHistory()
    {
        Lev = _history[_historyIndex].Clone();
        Renderer.Lev = Lev;
        _zoomCtrl.Lev = Lev;
        Lev.UpdateAllPolygons(Settings.RenderingSettings.GrassZoom);
        UpdateUndoRedo();
        topologyList.DropDownItems.Clear();
        topologyList.Text = "";
        _errorPoints.Clear();
        Renderer.OpenGlLgr?.UpdateGroundAndSky(Lev, Settings.RenderingSettings.DefaultGroundAndSky);
        CurrentTool.InActivate();
        ActivateCurrentAndRedraw();
        UpdateLabels();
        if (_savedIndex != _historyIndex)
        {
            SetModified(LevModification.Ground | LevModification.Objects | LevModification.Decorations, false);
        }
        else
        {
            SetNotModified();
        }
    }

    private void SetNotModified()
    {
        EnableSaveButtons(false);
        _modified = false;
    }

    private void MirrorHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Lev.MirrorSelected(MirrorOption.Horizontal);
        Lev.UpdateAllPolygons(Settings.RenderingSettings.GrassZoom);
        SetModified(LevModification.Objects | LevModification.Ground | LevModification.Decorations);
        RedrawScene();
    }

    private void MouseDownEvent(object sender, MouseEventArgs e)
    {
        Vector p = GetMouseCoordinates();
        CurrentTool.MouseMove(p);
        var info = ToolBase.GetNearestVertexInfo(p);
        int nearestObjectIndex = ToolBase.GetNearestObjectIndex(p);
        int nearestPictureIndex = ToolBase.GetNearestPictureIndex(p);
        var player = PlayController.GetNearestDriverBodyPart(p, ToolBase.CaptureRadiusScaled);
        PlayController.FollowDriver = false;
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
                    ChangeToDefaultCursor();
                    if (SelectedElementCount > 0)
                    {
                        CopyMenuItem.Visible = true;
                        DeleteMenuItem.Visible = true;
                        convertToToolStripMenuItem.Visible = true;
                        picturesConvertItem.Visible = _editorLgr != null;
                    }

                    TransformMenuItem.Visible = SelectedElementCount > 1;
                    _selectedObjectIndex = nearestObjectIndex;
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
                                if (_savedStartPosition != null)
                                {
                                    restoreStartPositionToolStripMenuItem.Visible = true;
                                }

                                break;
                        }
                    }

                    if (info is not null)
                    {
                        GrassMenuItem.Visible = true;
                        _grassInfo = info;
                    }

                    _selectedPictureIndex = nearestPictureIndex;
                    if (nearestPictureIndex >= 0)
                    {
                        PicturePropertiesMenuItem.Visible = true;
                        bringToFrontToolStripMenuItem.Visible = true;
                        sendToBackToolStripMenuItem.Visible = true;
                    }

                    if (player != null)
                    {
                        if (nearestObjectIndex < 0)
                        {
                            GravityUpMenuItem.Visible = true;
                            GravityDownMenuItem.Visible = true;
                            GravityLeftMenuItem.Visible = true;
                            GravityRightMenuItem.Visible = true;
                            switch (PlayController.Driver!.GravityDirection)
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

        CurrentTool.MouseDown(e);
        RedrawScene();
    }

    private void MouseLeaveEvent(object sender, EventArgs e)
    {
        CurrentTool.MouseOutOfEditor();
        RedrawScene();
    }

    private void MouseMoveEvent(object sender, MouseEventArgs e)
    {
        if (_lockMouseX)
            Cursor.Position = new Point(_lockCoord, MousePosition.Y);
        else if (_lockMouseY)
            Cursor.Position = new Point(MousePosition.X, _lockCoord);
        ShowCoordinates();
        if (_draggingScreen || _draggingGrid)
        {
            Vector z = GetMouseCoordinates();
            if (_draggingGrid)
            {
                _sceneSettings.GridOffset = _gridStartOffset + _moveStartPosition - z;
            }
            else
            {
                _zoomCtrl.CenterX = (_zoomCtrl.Cam.XMax + _zoomCtrl.Cam.XMin) / 2 - (z.X - _moveStartPosition.X);
                _zoomCtrl.CenterY = (_zoomCtrl.Cam.YMax + _zoomCtrl.Cam.YMin) / 2 - (z.Y - _moveStartPosition.Y);
            }
        }

        CurrentTool.MouseMove(GetMouseCoordinates());
        RedrawScene();
        StatusStrip1.Refresh();
    }

    private void MouseUpEvent(object sender, MouseEventArgs e)
    {
        CurrentTool.MouseUp();
        _draggingScreen = false;
        _draggingGrid = false;
        RedrawScene();
    }

    private void MouseWheelZoom(long delta)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            double currSize = Settings.RenderingSettings.GridSize;
            double newSize = currSize + Math.Sign(delta) * _zoomCtrl.Cam.ZoomLevel / 50.0;
            if (newSize > 0)
            {
                SetGridSizeWithMouse(newSize, GetMouseCoordinates());
            }
        }
        else
        {
            _zoomCtrl.Zoom(GetMouseCoordinates(), delta > 0, 1 - MouseWheelStep / 100.0);
        }
    }

    private static double GetGridMouseRatio(double size, double offset, double min, double mouse)
    {
        var dist = mouse - ElmaRenderer.GetFirstGridLine(size, offset, min);
        return (dist % size) / size;
    }

    private void SetGridSizeWithMouse(double newSize, Vector mouseCoords)
    {
        var settings = Settings.RenderingSettings;
        var gx = _sceneSettings.GridOffset.X;
        _sceneSettings.GridOffset.X = (gx + ElmaRenderer.GetFirstGridLine(newSize, gx, _zoomCtrl.Cam.XMin)
            - mouseCoords.X + GetGridMouseRatio(settings.GridSize, gx, _zoomCtrl.Cam.XMin, mouseCoords.X) *
            newSize) % newSize;
        var gy = _sceneSettings.GridOffset.Y;
        _sceneSettings.GridOffset.Y = (gy + ElmaRenderer.GetFirstGridLine(newSize, gy, _zoomCtrl.Cam.YMin)
            - mouseCoords.Y + GetGridMouseRatio(settings.GridSize, gy, _zoomCtrl.Cam.YMin, mouseCoords.Y) *
            newSize) % newSize;
        settings.GridSize = newSize;
        Renderer.UpdateSettings(settings);
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
        SetBlankLevel();
        InitializeLevel();
    }

    private void ObjectButtonChanged(object? sender, EventArgs e)
    {
        if (ObjectButton.Checked)
            ChangeToolTo(3);
    }

    private void OpenConfig(object sender, EventArgs e)
    {
        ComponentManager.ShowConfiguration("sle");
        AfterSettingsClosed();
        if (!Settings.EnableStartPositionFeature)
        {
            _savedStartPosition = null;
        }
    }

    private void OpenLevel(string path)
    {
        if (!PromptToSaveIfModified())
            return;
        TryLoadLevel(path);
        InitializeLevel();
    }

    private void OpenRenderingSettings(object sender, EventArgs e)
    {
        var rSettings = new RenderingSettingsForm(Settings.RenderingSettings);
        rSettings.Changed += x =>
        {
            Renderer.UpdateSettings(x);
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
            if (!PictureToolAvailable)
            {
                UiUtils.ShowError("You need to select LGR file from settings before you can use picture tool.",
                    "Note", MessageBoxIcon.Information);
                SelectButton.Checked = true;
            }
            else
            {
                ChangeToolTo(13);
            }
        }
    }

    private void PicturePropertiesToolStripMenuItemClick(object sender, EventArgs e)
    {
        if (Renderer.OpenGlLgr is null)
        {
            return;
        }
        var selectedElems = Lev.GraphicElements.Where(p => p.Position.Mark == VectorMark.Selected).ToList();
        var picForm = new PictureForm(Renderer.OpenGlLgr.CurrentLgr, null)
        {
            SetDefaultsAutomatically = Settings.AlwaysSetDefaultsInPictureTool
        };
        if (selectedElems.Count > 0)
        {
            picForm.AllowMultiple = true;
            picForm.SelectMultiple(selectedElems);
        }
        else
        {
            picForm.AllowMultiple = false;
            var selectedElem = Lev.GraphicElements[_selectedPictureIndex];
            picForm.SelectElement(selectedElem);
            selectedElems = new List<GraphicElement> { selectedElem };
        }

        picForm.AutoTextureMode = false;
        picForm.ShowDialog();
        if (picForm.Selection is not { } sel) return;
        Lev.GraphicElements = Lev.GraphicElements.Select(curr =>
        {
            if (selectedElems.Find(s => ReferenceEquals(s, curr)) is null)
            {
                return curr;
            }

            var clipping = sel.Clipping ?? curr.Clipping;
            var distance = sel.Distance ?? curr.Distance;
            var position = curr.Position;

            return sel switch
            {
                ImageSelection.MixedSelection => curr with { Distance = distance, Clipping = clipping },
                ImageSelection.PictureSelection(var pic, _, _) => GraphicElement.Pic(
                    Renderer.OpenGlLgr.DrawableImageFromName(pic), position, distance, clipping),
                ImageSelection.TextureSelection(var txt, var mask, _, _) => GraphicElement.Text(clipping, distance,
                    position,
                    Renderer.OpenGlLgr.DrawableImageFromName(txt),
                    Renderer.OpenGlLgr.DrawableImageFromName(mask)),
                ImageSelection.TextureSelectionMultipleMasks(var txt, _, _) when
                    curr is GraphicElement.Texture t =>
                    GraphicElement.Text(clipping, distance, position, Renderer.OpenGlLgr.DrawableImageFromName(txt), t.MaskInfo),
                ImageSelection.TextureSelectionMultipleMasks(var txt, _, _) when curr is GraphicElement.Picture
                    =>
                    GraphicElement.Text(clipping, distance, position, Renderer.OpenGlLgr.DrawableImageFromName(txt),
                        Renderer.OpenGlLgr.DrawableImageFromName(_editorLgr!.LgrImages.Values.First(i => i.Type == ImageType.Mask))),
                ImageSelection.TextureSelectionMultipleTextures(var mask, _, _) when
                    curr is GraphicElement.Texture t => GraphicElement.Text(clipping,
                        distance, position, t.TextureInfo, Renderer.OpenGlLgr.DrawableImageFromName(mask)),
                ImageSelection.TextureSelectionMultipleTextures when
                    curr is GraphicElement.Picture => curr with { Distance = distance, Clipping = clipping },
                _ => throw new ArgumentOutOfRangeException(nameof(sel))
            };
        }).ToList();

        SetModified(LevModification.Decorations);
        RedrawScene();
    }

    private void PipeButtonChanged(object? sender, EventArgs e)
    {
        if (PipeButton.Checked)
            ChangeToolTo(4);
    }

    private void PolyOpButtonChanged(object? sender, EventArgs e)
    {
        if (PolyOpButton.Checked)
            ChangeToolTo(7);
    }

    private void PrevNextButtonClick(object? sender, EventArgs e)
    {
        if (CurrLevDirFiles?.Count > 0)
        {
            if (_file is null)
                OpenLevel(CurrLevDirFiles[0]);
            else
            {
                int i = GetCurrentLevelIndex(_file, CurrLevDirFiles);
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
        var autoGrassTool = (AutoGrassTool)Tools[11];
        var grassPolys = Lev.Polygons.Where(x => !x.IsGrass)
            .SelectMany(autoGrassTool.AutoGrass).ToList();
        Lev.Polygons.AddRange(grassPolys);
        SetModified(grassPolys.Count > 0 ? LevModification.Decorations : LevModification.Nothing);
        RedrawScene();
    }

    private void Redo(object sender, EventArgs e)
    {
        if (_historyIndex < _history.Count - 1 && !CurrentTool.Busy)
        {
            _historyIndex++;
            LoadFromHistory();
        }
    }

    private void RefreshOnOpen(object sender, EventArgs e)
    {
        ViewerResized();
        _zoomCtrl.ZoomFill();
    }

    private void SaveAs(object? sender = null, EventArgs? e = null)
    {
        string suggestion = string.Empty;
        if (Settings.UseFilenameSuggestion)
        {
            var filenameStart = Settings.BaseFilename;
            int highestNumber = 0;
            int lowestNumber = int.MaxValue;
            foreach (string levelFile in Global.GetLevelFiles())
            {
                string x = Path.GetFileNameWithoutExtension(levelFile);
                if (x.StartsWith(filenameStart, StringComparison.OrdinalIgnoreCase))
                {
                    bool isNum = int.TryParse(x.Substring(filenameStart.Length), out var levelNumber);
                    if (isNum)
                    {
                        highestNumber = Math.Max(highestNumber, levelNumber);
                        lowestNumber = Math.Min(lowestNumber, levelNumber);
                    }
                }
            }

            try
            {
                int newNumber;
                if (highestNumber == 0 || lowestNumber <= 1)
                {
                    newNumber = highestNumber + 1;
                }
                else
                {
                    newNumber = lowestNumber - 1;
                }

                suggestion = filenameStart + newNumber.ToString(Settings.NumberFormat);
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

    private string GetInitialDir()
    {
        return _file is not null ? _file.FileInfo.DirectoryName! : Global.AppSettings.General.LevelDirectory;
    }

    private void SaveClicked(object? sender = null, EventArgs? e = null)
    {
        if (_file is null)
            SaveAs();
        else
            SaveLevel(_file.Path);
    }

    private void SaveLevel(string path)
    {
        Lev.Title = TitleBox.Text;
        Lev.LgrFile = LGRBox.Text;
        Lev.GroundTextureName = GroundComboBox.Text;
        Lev.SkyTextureName = SkyComboBox.Text;
        if (Lev.GroundTextureName == "")
            Lev.GroundTextureName = "ground";
        if (Lev.SkyTextureName == "")
            Lev.SkyTextureName = "sky";
        CurrentTool.InActivate();
        if (Lev.Top10.IsEmpty ||
            MessageBox.Show("This level has times in top 10. Do you still want to save the level?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            if (Settings.CheckTopologyWhenSaving)
                CheckTopologyAndUpdate();
            if (Settings.UseFilenameForTitle && _file is null)
            {
                Lev.Title = Path.GetFileNameWithoutExtension(SaveFileDialog1.FileName);
            }

            try
            {
                _file = Lev.Save(path);
                _savedIndex = _historyIndex;
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

        ActivateCurrentAndRedraw();
    }

    private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
    {
        foreach (var polygon in Lev.Polygons)
        {
            if ((polygon.IsGrass && EffectiveGrassFilter) || (!polygon.IsGrass && EffectiveGroundFilter))
                polygon.MarkVectorsAs(VectorMark.Selected);
        }

        foreach (var levelObject in Lev.Objects)
        {
            switch (levelObject.Type)
            {
                case ObjectType.Apple:
                    if (EffectiveAppleFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
                case ObjectType.Killer:
                    if (EffectiveKillerFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
                case ObjectType.Flower:
                    if (EffectiveFlowerFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
            }
        }

        foreach (var ge in Lev.GraphicElements)
        {
            if ((EffectiveTextureFilter && ge is GraphicElement.Texture) ||
                (EffectivePictureFilter && ge is GraphicElement.Picture))
                ge.Mark = VectorMark.Selected;
        }

        RedrawScene();
        UpdateSelectionInfo();
    }

    private void SelectButtonChanged(object? sender, EventArgs e)
    {
        if (SelectButton.Checked)
            ChangeToolTo(0);
    }

    private void SendToBackToolStripMenuItemClick(object? sender, EventArgs e)
    {
        var mod = LevModification.Nothing;
        if (_selectedObjectIndex >= 0)
        {
            var obj = Lev.Objects[_selectedObjectIndex];
            Lev.Objects.RemoveAt(_selectedObjectIndex);
            Lev.Objects.Insert(0, obj);
            mod |= LevModification.Objects;
        }
        else if (_selectedPictureIndex >= 0)
        {
            var obj = Lev.GraphicElements[_selectedPictureIndex];
            Lev.GraphicElements.RemoveAt(_selectedPictureIndex);
            Lev.GraphicElements.Add(obj);
            mod |= LevModification.Decorations;
        }

        SetModified(mod);
    }

    private void SetAllFilters(object? sender, EventArgs e)
    {
        foreach (ToolStripMenuItem x in SelectionFilterToolStripMenuItem.DropDownItems)
            if (x.CheckOnClick)
                x.Checked = EnableAllToolStripMenuItem.Equals(sender);
    }

    private void SetDefaultLevelTitle()
    {
        if (!Settings.UseFilenameForTitle)
            Lev.Title = Settings.DefaultTitle;
    }

    private void SetBlankLevel()
    {
        Lev = Settings.GetTemplateLevel();
        SetDefaultLevelTitle();
        _file = null;
        _savedStartPosition = null;
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
        Settings.ShowCrossHair = showCrossHairButton.Checked;
        Renderer.UpdateSettings(settings);
        RedrawScene();
    }

    private void SetupEventHandlers()
    {
        Resize += ViewerResized;
        EditorControl.Paint += RedrawScene;
        ZoomFillButton.Click += (_, _) => _zoomCtrl.ZoomFill();
        ObjectButton.CheckedChanged += ObjectButtonChanged;
        VertexButton.CheckedChanged += VertexButtonChanged;
        PipeButton.CheckedChanged += PipeButtonChanged;
        EllipseButton.CheckedChanged += EllipseButtonChanged;
        PolyOpButton.CheckedChanged += PolyOpButtonChanged;
        DrawButton.CheckedChanged += DrawButtonChanged;
        FrameButton.CheckedChanged += FrameButtonChanged;
        ZoomButton.CheckedChanged += ZoomButtonChanged;
        SelectButton.CheckedChanged += SelectButtonChanged;
        SmoothenButton.CheckedChanged += SmoothenButtonChanged;
        CutConnectButton.CheckedChanged += CutButtonChanged;
        AutoGrassButton.CheckedChanged += AutoGrassButtonChanged;
        PictureButton.CheckedChanged += PictureButtonChanged;
        LGRBox.TextChanged += LevelPropertyModified;
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
            ChangeToolTo(9);
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
        if (_historyIndex > 0 && !CurrentTool.Busy)
        {
            _historyIndex--;
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
        string? levDir = _file?.FileInfo.DirectoryName;
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
        if (_file is null)
        {
            Text = "New - " + LevEditorName;
            filenameBox.Text = string.Empty;
            filenameBox.Enabled = false;
            deleteButton.Enabled = false;
            deleteLevMenuItem.Enabled = false;
            EnableSaveButtons(true);
        }
        else
        {
            Text = _file.FileNameNoExt + " - " + LevEditorName;
            filenameBox.Text = _file.FileNameNoExt;
            filenameBox.Enabled = true;
            deleteButton.Enabled = true;
            deleteLevMenuItem.Enabled = true;
        }

        _programmaticPropertyChange = true;
        TitleBox.Text = Lev.Title;
        LGRBox.Text = Lev.LgrFile;
        GroundComboBox.Text = Lev.GroundTextureName;
        SkyComboBox.Text = Lev.SkyTextureName;
        _programmaticPropertyChange = false;
        BestTimeLabel.Text = "Best time: " + Lev.Top10.GetSinglePlayerString(0);
        UpdateSelectionInfo();
    }

    private void UpdateLgrTools()
    {
        if (Renderer.OpenGlLgr != null && _editorLgr?.Path != Renderer.OpenGlLgr.CurrentLgr.Path)
        {
            _editorLgr = Renderer.OpenGlLgr.CurrentLgr;
            PicturePropertiesMenuItem.Enabled = true;
            SkyComboBox.Enabled = true;
            GroundComboBox.Enabled = true;
            SkyComboBox.Items.Clear();
            GroundComboBox.Items.Clear();
            var names = _editorLgr.ListedImagesExcludingSpecial.Where(image =>
                image.Type == ImageType.Texture).Select(image => image.Name).ToArray();
            SkyComboBox.Items.AddRange(names);
            GroundComboBox.Items.AddRange(names);
        }
        else if (!PictureToolAvailable)
        {
            PicturePropertiesMenuItem.Enabled = false;
            SkyComboBox.Enabled = false;
            GroundComboBox.Enabled = false;
        }

        CheckForPictureLoss();
    }

    private void UpdateUndoRedo()
    {
        UndoButton.Enabled = _historyIndex > 0;
        RedoButton.Enabled = _historyIndex < _history.Count - 1;
        UndoToolStripMenuItem.Enabled = UndoButton.Enabled;
        RedoToolStripMenuItem.Enabled = RedoButton.Enabled;
    }

    private void VertexButtonChanged(object? sender, EventArgs e)
    {
        if (VertexButton.Checked)
            ChangeToolTo(1);
    }

    private void ViewerResized(object? sender = null, EventArgs? e = null)
    {
        if (EditorControl.Width > 0 && EditorControl.Height > 0)
        {
            if (PlayController.PlayingOrPaused)
            {
                PlayController.ResetViewPortRequested = (EditorControl.Width, EditorControl.Height);
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

    private void ZoomButtonChanged(object? sender, EventArgs e)
    {
        if (ZoomButton.Checked)
            ChangeToolTo(5);
    }

    private void ZoomFillToolStripMenuItemClick(object? sender, EventArgs e)
    {
        _zoomCtrl.ZoomFill();
    }

    private void TitleBoxTextChanged(object? sender, EventArgs e)
    {
        int width = TextRenderer.MeasureText(TitleBox.Text, TitleBox.Font).Width;
        TitleBox.Width = Math.Max(width + 5, 120 * (int)_dpiX);
        TitleBox.BackColor = Regex.IsMatch(TitleBox.Text, "[^a-zA-Z0-9!\"%&/()=?`^*-_,.;:<>\\[\\]+# ]")
            ? Color.Red
            : Color.White;
    }

    public void PreserveSelection()
    {
        _history[_historyIndex] = Lev.Clone();
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

                if (Renderer.OpenGlLgr != null)
                {
                    lev.UpdateImages(Renderer.OpenGlLgr.DrawableImages);
                }
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
            Lev.UpdateAllPolygons(Settings.RenderingSettings.GrassZoom);
        });
        if (imported > 0)
        {
            SetModified(LevModification.Objects | LevModification.Ground | LevModification.Decorations);
            _zoomCtrl.ZoomFill();
        }
    }

    private void saveAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
    {
        saveAsPictureDialog.FileName = _file?.FileNameNoExt ?? "Untitled";
        if (saveAsPictureDialog.ShowDialog() == DialogResult.OK)
        {
            if (saveAsPictureDialog.FileName.EndsWith(".png"))
            {
                Renderer.SaveSnapShot(saveAsPictureDialog.FileName, _zoomCtrl, _sceneSettings);
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
        var selectedVertices = Lev.Polygons
            .SelectMany(p => p.Vertices.Where(v => v.Mark == VectorMark.Selected)).ToList();
        selectedVertices.AddRange(
            Lev.Objects.Where(v =>
                    v.Position.Mark == VectorMark.Selected && v.Type != ObjectType.Start)
                .Select(o => o.Position));
        selectedVertices.AddRange(
            Lev.GraphicElements.Where(v => v.Position.Mark == VectorMark.Selected).Select(p => p.Position));

        void RemoveSelected()
        {
            var first = Lev.Polygons.First().Clone();
            Lev.Polygons.ForEach(p => p.Vertices.RemoveAll(v => v.Mark == VectorMark.Selected));
            Lev.Polygons.RemoveAll(p => p.Vertices.Count < 3);
            if (Lev.Polygons.Count == 0)
            {
                Lev.Polygons.Add(first);
            }

            Lev.Objects.RemoveAll(o =>
                o.Position.Mark == VectorMark.Selected && o.Type != ObjectType.Start);
            Lev.GraphicElements.RemoveAll(p => p.Position.Mark == VectorMark.Selected);
        }

        var objType = ObjectType.Apple;
        if (sender.Equals(applesConvertItem))
        {
            // default
        }
        else if (sender.Equals(killersConvertItem))
        {
            objType = ObjectType.Killer;
        }
        else if (sender.Equals(flowersConvertItem))
        {
            objType = ObjectType.Flower;
        }
        else
        {
            // handle picture
            var picForm = new PictureForm(Renderer.OpenGlLgr!.CurrentLgr, null)
            {
                AllowMultiple = false,
                AutoTextureMode = false,
                SetDefaultsAutomatically = true
            };
            picForm.SetDefaultDistanceAndClipping();
            picForm.ShowDialog();
            if (picForm.Selection is { } sel)
            {
                RemoveSelected();
                var clipping = sel.Clipping!.Value;
                var distance = sel.Distance!.Value;
                foreach (var selectedVertex in selectedVertices)
                {
                    GraphicElement g = picForm.Selection switch
                    {
                        ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance, selectedVertex,
                            Renderer.OpenGlLgr.DrawableImageFromName(t.Txt), Renderer.OpenGlLgr.DrawableImageFromName(t.Mask)),
                        ImageSelection.PictureSelection p => GraphicElement.Pic(
                            Renderer.OpenGlLgr.DrawableImageFromName(p.Pic), selectedVertex, distance, clipping),
                        _ => throw new Exception("Unexpected")
                    };
                    Lev.GraphicElements.Add(g);
                }
            }

            SetModified(LevModification.Decorations);
            return;
        }

        RemoveSelected();

        foreach (var selectedVertex in selectedVertices)
        {
            var obj = new LevObject(selectedVertex, objType, AppleType.Normal);
            Lev.Objects.Add(obj);
        }

        SetModified(LevModification.Decorations | LevModification.Ground | LevModification.Objects);
    }

    private void TextButton_CheckedChanged(object sender, EventArgs e)
    {
        if (TextButton.Checked)
        {
            ChangeToolTo(14);
        }
    }

    private void deleteLevMenuItem_Click(object sender, EventArgs e)
    {
        DeleteCurrentLevel();
    }

    private void DeleteCurrentLevel()
    {
        if (_file is null)
        {
            return;
        }

        if (MessageBox.Show("Are you sure you want to delete this level?", "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
        {
            UpdateCurrLevDirFiles();
            File.Delete(_file.Path);

            int levIndex = GetCurrentLevelIndex(_file, CurrLevDirFiles!);
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
        bool showButtons = _file is not null && string.Compare(filenameBox.Text,
            _file.FileNameNoExt,
            StringComparison.InvariantCulture) != 0;
        filenameOkButton.Visible = showButtons;
        filenameCancelButton.Visible = showButtons;
    }

    private void filenameCancelButton_Click(object? sender = null, EventArgs? e = null)
    {
        filenameBox.Text = _file?.FileNameNoExt;
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
            var newPath = Path.Combine(_file!.FileInfo.DirectoryName!, filenameBox.Text + ".lev");
            UpdateCurrLevDirFiles();
            File.Move(_file.Path, newPath);
            if (CurrLevDirFiles != null)
            {
                int index = GetCurrentLevelIndex(_file, CurrLevDirFiles);
                CurrLevDirFiles[index] = newPath;
            }

            _file = new ElmaFile(newPath);
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
        if (_file is null)
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
        if (PolyOpTool.PolyOpSelected(PolygonOperationType.Union, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void differenceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void intersectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PolyOpTool.PolyOpSelected(PolygonOperationType.Intersection, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void symmetricDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons))
        {
            SetModifiedAndRender(LevModification.Ground);
        }
    }

    private void texturizeMenuItem_Click(object sender, EventArgs e)
    {
        TexturizeSelection();
    }

    private void SaveStartPosition(object? sender = null, EventArgs? e = null)
    {
        foreach (var o in Lev.Objects)
        {
            if (o.Type == ObjectType.Start)
            {
                _savedStartPosition = o.Position.Clone();
            }
        }
    }

    private void restoreStartPositionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_savedStartPosition is not { } p)
        {
            return;
        }
        foreach (var o in Lev.Objects)
        {
            if (o.Type == ObjectType.Start)
            {
                var oldPos = o.Position;
                o.Position = p.Clone();
                if (!Equals(oldPos, _savedStartPosition))
                {
                    SetModified(LevModification.Objects);
                }
            }
        }
    }

    private void MirrorVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Lev.MirrorSelected(MirrorOption.Vertical);
        Lev.UpdateAllPolygons(Settings.RenderingSettings.GrassZoom);
        SetModified(LevModification.Decorations | LevModification.Ground | LevModification.Objects);
        RedrawScene();
    }

    private void MoveStartHereToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var s = Lev.Objects.Find(o => o.Type == ObjectType.Start);
        if (s != null && _contextMenuClickPosition is { } p)
        {
            s.Position = p;
            SetModified(LevModification.Objects);
        }
    }

    private void EditorMenuStrip_Opening(object sender, CancelEventArgs e)
    {
        _contextMenuClickPosition = GetMouseCoordinates();
    }

    private void EditorControl_DragLeave(object sender, EventArgs e)
    {
        CurrentTool.UpdateHelp();
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
        if (PlayController.Paused)
        {
            PlayController.PlayState = PlayState.Playing;
            SetToPlaying();
            return;
        }
        if (PlayController.Playing)
        {
            PlayController.PlayState = PlayState.Paused;
            SetNotPlaying();
            return;
        }

        if (Settings.PlayingSettings.ToggleFullscreen)
        {
            _fullScreenController.FullScreen();
        }
        SetToPlaying();
        var t = new Timer(25);
        var updateTime = new Action(() =>
        {
            PlayTimeLabel.Text = PlayController.Driver!.CurrentTime.ToSeconds().ToTimeString();
        });
        t.Elapsed += (_, _) =>
        {
            Invoke(updateTime);
        };
        t.Start();
        CameraUtils.AllowScroll = false;
        var oldZoom = _zoomCtrl.ZoomLevel;

        if (Settings.PlayingSettings.FollowDriverOption == FollowDriverOption.WhenPressingKey)
        {
            _zoomCtrl.ZoomLevel = Settings.PlayingSettings.PlayZoomLevel;
        }

        await PlayController.BeginLoop(Lev, _sceneSettings, Renderer, _zoomCtrl, DoRedrawScene);

        if (Settings.PlayingSettings.FollowDriverOption == FollowDriverOption.WhenPressingKey)
        {
            Settings.PlayingSettings.PlayZoomLevel = _zoomCtrl.ZoomLevel;
            _zoomCtrl.ZoomLevel = oldZoom;
        }

        t.Stop();
        PlayTimeLabel.Text = PlayController.Driver!.CurrentTime.ToSeconds().ToTimeString();
        if (PlayController.Driver.Condition == DriverCondition.Finished)
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
        if (PlayController.PlayingOrPaused)
        {
            await PlayController.StopPlaying();
        }

        if (Settings.PlayingSettings.ToggleFullscreen)
        {
            _fullScreenController.Restore();
        }
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        var f = new PlaySettingsForm(PlayController.Settings);
        var result = f.ShowDialog();
        if (result == DialogResult.OK)
        {
            PlayController.Settings = f.Settings;
            Settings.PlayingSettings = f.Settings;
        }
    }

    private static readonly string[] ImportableExtensions = { DirUtils.LevExtension, DirUtils.LebExtension, ".bmp", ".png", ".gif", ".tiff", ".exif", ".svg", ".svgz" };
    private ToolBase.NearestVertexInfo? _grassInfo;
    private TexturizationOptions? _texturizationOpts;

    public bool PreFilterMessage(ref Message m)
    {
        if (PlayController.Playing)
        {
            // The message filter is global, so the control that is receiving the message
            // might be in a different form (e.g. level manager). Hence the need for IsChild check.
            if (m.Msg is NativeUtils.WmKeydown or NativeUtils.WmKeyup && NativeUtils.IsChild(Handle, m.HWnd))
            {
                PlayController.UpdateInputKeys();
                var key = (Keys)m.WParam;
                if (PlayController.Settings.DisableShortcuts)
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

    private void DeselectPolygonsWith(Func<Polygon, bool> cond)
    {
        foreach (var polygon in Lev.Polygons.Where(cond))
        {
            polygon.Vertices = polygon.Vertices.Select(v => v with { Mark = VectorMark.None }).ToList();
        }

        RedrawScene();
    }

    private void DeselectObjectsWith(Func<LevObject, bool> cond)
    {
        foreach (var obj in Lev.Objects.Where(cond))
        {
            obj.Mark = VectorMark.None;
        }

        RedrawScene();
    }

    private void DeselectGraphicElementsWith(Func<GraphicElement, bool> cond)
    {
        foreach (var elem in Lev.GraphicElements.Where(cond))
        {
            elem.Mark = VectorMark.None;
        }

        RedrawScene();
    }

    private void deselectGroundPolygonsToolStripMenuItem_Click(object sender, EventArgs e) => DeselectPolygonsWith(p => !p.IsGrass);

    private void deselectGrassPolygonsToolStripMenuItem_Click(object sender, EventArgs e) => DeselectPolygonsWith(p => p.IsGrass);

    private void deselectApplesToolStripMenuItem_Click(object sender, EventArgs e) => DeselectObjectsWith(o => o.Type == ObjectType.Apple);

    private void deselectKillersToolStripMenuItem_Click(object sender, EventArgs e) => DeselectObjectsWith(o => o.Type == ObjectType.Killer);

    private void deselectFlowersToolStripMenuItem_Click(object sender, EventArgs e) => DeselectObjectsWith(o => o.Type == ObjectType.Flower);

    private void deselectPicturesToolStripMenuItem_Click(object sender, EventArgs e) => DeselectGraphicElementsWith(ge => ge is GraphicElement.Picture);

    private void deselectTexturesToolStripMenuItem_Click(object sender, EventArgs e) => DeselectGraphicElementsWith(ge => ge is GraphicElement.Texture);
}
