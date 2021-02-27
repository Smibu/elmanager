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
using NetTopologySuite.Geometries;
using OpenTK.Graphics.OpenGL;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SvgNet.SvgGdi;
using Color = System.Drawing.Color;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using Envelope = NetTopologySuite.Geometries.Envelope;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;
using Polygon = Elmanager.Lev.Polygon;
using Timer = System.Timers.Timer;

namespace Elmanager.LevelEditor
{
    internal partial class LevelEditorForm : FormMod
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
        internal IEditorTool CurrentTool;
        internal Level Lev;
        internal PictureForm PicForm;
        internal ElmaRenderer Renderer;
        internal IEditorTool[] Tools;
        private List<string> _currLevDirFiles;
        private bool _draggingScreen;
        private Lgr.Lgr _editorLgr;
        private List<Vector> _errorPoints = new();
        private int _historyIndex;
        private int _savedIndex;
        private string _loadedLevFilesDir;
        private int _lockCoord;
        private bool _lockMouseX;
        private bool _lockMouseY;
        private bool _modified;
        private bool _fromScratch;
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
        private ElmaCamera _camera;
        private ZoomController _zoomCtrl;
        private readonly SceneSettings _sceneSettings = new();

        internal LevelEditorForm(string levPath)
        {
            InitializeComponent();
            TryLoadLevel(levPath);
            Initialize();
        }

        internal void SetLevel(Level lev)
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

        private void SetExistingLev(Level lev)
        {
            Lev = lev;
            _fromScratch = false;
            SaveStartPosition();
        }

        public LevelEditorForm()
        {
            InitializeComponent();
            if (Global.AppSettings.LevelEditor.LastLevel != null)
            {
                TryLoadLevel(Global.AppSettings.LevelEditor.LastLevel);
            }
            else
                SetBlankLevel();

            Initialize();
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

        internal bool EffectiveGrassFilter => _grassFilter && (ShowGrassEdgesButton.Checked);

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

        private ToolBase ToolBase => ((ToolBase) CurrentTool);

        private List<string> CurrLevDirFiles
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

        internal void TransformMenuItemClick(object sender = null, EventArgs e = null)
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

        internal void RedrawScene(object sender = null, EventArgs e = null)
        {
            if (PlayController.PlayingOrPaused)
            {
                return;
            }

            DoRedrawScene();
        }

        internal void ActivateCurrentAndRedraw()
        {
            CurrentTool.Activate();
            RedrawScene();
        }

        internal void InactivateCurrentAndRedraw()
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
                if (Global.AppSettings.LevelEditor.CheckTopologyDynamically)
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
            foreach (Picture x in Lev.Pictures)
                if (x.Position.Mark == VectorMark.Selected)
                    if (x.IsPicture)
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

        private void AfterSettingsClosed(string oldLgr)
        {
            Renderer.UpdateSettings(Global.AppSettings.LevelEditor.RenderingSettings);
            UpdateLgrTools();
            UpdateButtons();
            RedrawScene();
        }

        private void AutoGrassButtonChanged(object sender, EventArgs e)
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
                var obj = Lev.Pictures[_selectedPictureIndex];
                Lev.Pictures.RemoveAt(_selectedPictureIndex);
                Lev.Pictures.Insert(0, obj);
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
                const string text = "Some pictures or textures could not be found in the LGR file. " +
                                    "You will lose these pictures if you save this level.";
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

        private void CheckTopologyAndUpdate(object sender = null, EventArgs e = null)
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
                Global.AppSettings.LevelEditor.Size = Size;
            }

            Global.AppSettings.LevelEditor.WindowState = WindowState;
            Global.AppSettings.LevelEditor.LastLevel = Lev.Path;
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
            var copiedTextures = new List<Picture>();
            Vector.MarkDefault = VectorMark.Selected;
            var delta = Keyboard.IsKeyDown(Key.LeftShift)
                ? Global.AppSettings.LevelEditor.RenderingSettings.GridSize
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

                if (copy.Count > 2)
                {
                    copiedPolygons.Add(copy);
                    copy.IsGrass = x.IsGrass;
                    copy.UpdateDecomposition();
                }
            }

            foreach (LevObject x in Lev.Objects)
            {
                if (x.Position.Mark == VectorMark.Selected && x.Type != ObjectType.Start)
                {
                    x.Position.Mark = VectorMark.None;
                    copiedObjects.Add(
                        new LevObject(
                            x.Position +
                            new Vector(delta,
                                -delta), x.Type, x.AppleType,
                            x.AnimationNumber));
                }
            }

            foreach (Picture x in Lev.Pictures)
            {
                if (x.Position.Mark == VectorMark.Selected)
                {
                    Picture copiedPicture = x.Clone();
                    copiedPicture.Position.X += delta;
                    copiedPicture.Position.Y -= delta;
                    copiedTextures.Add(copiedPicture);
                    x.Position.Mark = VectorMark.None;
                }
            }

            Vector.MarkDefault = VectorMark.None;
            Lev.Polygons.AddRange(copiedPolygons);
            Lev.Objects.AddRange(copiedObjects);
            Lev.Pictures.AddRange(copiedTextures);
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

        private bool CurrLevDirExists()
        {
            return Directory.Exists(Path.GetDirectoryName(Lev.Path));
        }

        private void DoRedrawScene()
        {
            _sceneSettings.AdditionalPolys = CurrentTool.GetExtraPolygons();
            var jf = PlayController.Playing && PlayController.FollowDriver ? _zoomCtrl.Cam.FixJitter() : new Vector();
            Renderer.DrawScene(_zoomCtrl.Cam, _sceneSettings);

            var drawAction = GetVertexDrawAction();

            if (Global.AppSettings.LevelEditor.ShowCrossHair)
            {
                var mouse = GetMouseCoordinates();
                Renderer.DrawDashLine(_zoomCtrl.Cam.XMin, mouse.Y, _zoomCtrl.Cam.XMax,
                    mouse.Y, Global.AppSettings.LevelEditor.CrosshairColor);
                Renderer.DrawDashLine(mouse.X, _zoomCtrl.Cam.YMin, mouse.X,
                    _zoomCtrl.Cam.YMax, Global.AppSettings.LevelEditor.CrosshairColor);
            }

            foreach (Polygon x in Lev.Polygons)
            {
                switch (x.Mark)
                {
                    case PolygonMark.Highlight:
                        if (Global.AppSettings.LevelEditor.UseHighlight)
                            if (x.IsGrass)
                            {
                                Renderer.DrawLineStrip(x, Global.AppSettings.LevelEditor.HighlightColor);
                                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowInactiveGrassEdges)
                                {
                                    Renderer.DrawDashLine(x.Vertices.First(), x.Vertices.Last(),
                                        Global.AppSettings.LevelEditor.HighlightColor);
                                }
                            }
                            else
                                Renderer.DrawPolygon(x, Global.AppSettings.LevelEditor.HighlightColor);

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
                            drawAction(z, Global.AppSettings.LevelEditor.SelectionColor);
                            break;
                        case VectorMark.Highlight:
                            if (Global.AppSettings.LevelEditor.UseHighlight)
                                drawAction(z, Global.AppSettings.LevelEditor.HighlightColor);
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
                        drawAction(z, Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case VectorMark.Highlight:
                        if (Global.AppSettings.LevelEditor.UseHighlight)
                            drawAction(z, Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
            }

            foreach (Picture t in Lev.Pictures)
            {
                Vector z = t.Position;
                switch (z.Mark)
                {
                    case VectorMark.Selected:
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y - t.Height,
                            Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case VectorMark.Highlight:
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y - t.Height,
                            Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
            }

            foreach (Vector x in _errorPoints)
                Renderer.DrawSquare(x, _zoomCtrl.Cam.ZoomLevel / 25, Color.Red);
            if (_savedStartPosition is { } p)
            {
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects)
                {
                    Renderer.DrawDummyPlayer(p.X, p.Y, _sceneSettings, new PlayerRenderOpts(Color.Green, false, true, true));
                }

                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectFrames)
                {
                    Renderer.DrawDummyPlayer(p.X, p.Y, _sceneSettings, new PlayerRenderOpts(Color.Green, false, false, true));
                }
            }

            if (PlayController.PlayingOrPaused)
            {
                GL.Translate(-jf.X, -jf.Y, 0);
                var driver = PlayController.Driver;
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects && Renderer.LgrGraphicsLoaded)
                {
                    Renderer.DrawPlayer(driver.GetState(), PlayController.RenderOptsLgr, _sceneSettings);
                }
                else if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectFrames)
                {
                    Renderer.DrawPlayer(driver.GetState(), PlayController.RenderOptsFrame, _sceneSettings);
                }

                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.DepthTest);
                switch (PlayController.PlayerSelection)
                {
                    case VectorMark.Selected:
                        drawAction(driver.Body.Location, Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case VectorMark.Highlight:
                        drawAction(driver.Body.Location, Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
                GL.Translate(jf.X, jf.Y, 0);
            }

            CurrentTool.ExtraRendering();

            Renderer.Swap();
        }

        private Action<Vector, Color> GetVertexDrawAction()
        {
            var drawAction = Global.AppSettings.LevelEditor.RenderingSettings.UseCirclesForVertices
                ? (Action<Vector, Color>) ((pt, color) => Renderer.DrawPoint(pt, color))
                : ((pt, color) => Renderer.DrawEquilateralTriangle(pt,
                    _zoomCtrl.Cam.ZoomLevel * Global.AppSettings.LevelEditor.RenderingSettings.VertexSize, color));
            return drawAction;
        }

        private void CutButtonChanged(object sender, EventArgs e)
        {
            if (CutConnectButton.Checked)
                ChangeToolTo(10);
        }

        private void DeleteAllGrassToolStripMenuItemClick(object sender, EventArgs e)
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

        private void DeleteSelected(object sender, EventArgs e)
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
                        x.UpdateDecomposition();
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

                for (int i = Lev.Pictures.Count - 1; i >= 0; i--)
                {
                    Picture x = Lev.Pictures[i];
                    if (x.Position.Mark == VectorMark.Selected)
                    {
                        Lev.Pictures.Remove(x);
                        mod |= LevModification.Decorations;
                    }
                }

                PlayController.NotifyDeletedApples(deletedApples);
                SetModified(mod);
                UpdateSelectionInfo();
            }
        }

        private void DrawButtonChanged(object sender, EventArgs e)
        {
            if (DrawButton.Checked)
                ChangeToolTo(2);
        }

        private void EllipseButtonChanged(object sender, EventArgs e)
        {
            if (EllipseButton.Checked)
                ChangeToolTo(6);
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            _groundFilter = GroundPolygonsToolStripMenuItem.Checked;
            _grassFilter = GrassPolygonsToolStripMenuItem.Checked;
            _appleFilter = ApplesToolStripMenuItem.Checked;
            _killerFilter = KillersToolStripMenuItem.Checked;
            _flowerFilter = FlowersToolStripMenuItem.Checked;
            _pictureFilter = PicturesToolStripMenuItem.Checked;
            _textureFilter = TexturesToolStripMenuItem.Checked;
            foreach (var polygon in Lev.Polygons)
            {
                if (polygon.IsGrass && !_grassFilter)
                {
                    polygon.MarkVectorsAs(VectorMark.None);
                }
                else if (!polygon.IsGrass && !_groundFilter)
                {
                    polygon.MarkVectorsAs(VectorMark.None);
                }
            }

            foreach (var o in Lev.Objects)
            {
                switch (o.Type)
                {
                    case ObjectType.Flower:
                        if (!_flowerFilter)
                        {
                            o.Position.Mark = VectorMark.None;
                        }

                        break;
                    case ObjectType.Apple:
                        if (!_appleFilter)
                        {
                            o.Position.Mark = VectorMark.None;
                        }

                        break;
                    case ObjectType.Killer:
                        if (!_killerFilter)
                        {
                            o.Position.Mark = VectorMark.None;
                        }

                        break;
                    case ObjectType.Start:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var picture in Lev.Pictures)
            {
                if (picture.IsPicture && !_pictureFilter)
                {
                    picture.Position.Mark = VectorMark.None;
                }
                else if (!picture.IsPicture && !_textureFilter)
                {
                    picture.Position.Mark = VectorMark.None;
                }
            }

            RedrawScene();
            SelectionFilterToolStripMenuItem.ShowDropDown();
        }

        private void FrameButtonChanged(object sender, EventArgs e)
        {
            if (FrameButton.Checked)
                ChangeToolTo(8);
        }

        private Vector GetMouseCoordinates()
        {
            var mousePosNoTr = (Point) Invoke(new Func<Point>(() => EditorControl.PointToClient(MousePosition)));
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
            if (!polys.Any())
            {
                polys.Add(ToolBase.NearestPolygon);
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
                p.UpdateDecomposition();
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

        private void UpdateLgrFromLev()
        {
            if (Directory.Exists(Global.AppSettings.General.LgrDirectory))
            {
                var lgr = Path.Combine(Global.AppSettings.General.LgrDirectory, Lev.LgrFile.ToLower() + ".lgr");
                if (File.Exists(lgr))
                {
                    Global.AppSettings.LevelEditor.RenderingSettings.LgrFile = lgr;
                }
            }
        }

        private void Initialize()
        {
            if (!Physics)
            {
                playButton.Visible = false;
                stopButton.Visible = false;
                settingsButton.Visible = false;
            }
            PlayController.PlayingPaused += () => Invoke(new Action(SetNotPlaying));
            var graphics = CreateGraphics();
            _dpiX = graphics.DpiX / 96;
            _dpiY = graphics.DpiY / 96;
            var dpiXint = (int) _dpiX;
            var dpiYint = (int) _dpiY;
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
            WindowState = Global.AppSettings.LevelEditor.WindowState;
            SelectButton.Select();
            UpdateButtons();
            Size = Global.AppSettings.LevelEditor.Size;
            Renderer = new ElmaRenderer(EditorControl, Global.AppSettings.LevelEditor.RenderingSettings);
            
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
            await PlayController.StopPlaying();
            PlayTimeLabel.Text = "";
            _camera = new ElmaCamera();
            _zoomCtrl = new ZoomController(_camera, Lev, Global.AppSettings.LevelEditor.RenderingSettings, () => RedrawScene());
            SetNotModified();
            Renderer.InitializeLevel(Lev);
            _zoomCtrl.ZoomFill();
            UpdateLgrFromLev();
            Renderer.UpdateSettings(Global.AppSettings.LevelEditor.RenderingSettings);
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

        private void KeyHandlerDown(object sender, KeyEventArgs e)
        {
            PlayController.UpdateInputKeys();
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
                case Keys.Enter when Physics:
                    playButton_Click(null, null);
                    break;
                case Keys.Oem2:
                    wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons);
                    break;
                case Keys.Escape:
                    stopButton_Click(null, null);
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
            if (!PictureToolAvailable)
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

            PicForm.AutoTextureMode = true;
            PicForm.AllowMultiple = false;
            PicForm.SetDefaultsAutomatically = true;
            PicForm.SetDefaultDistanceAndClipping();
            PicForm.ShowDialog();
            if (!PicForm.OkButtonPressed)
            {
                return;
            }

            var masks = PicForm.SelectedMasks.Select(x => Renderer.DrawableImageFromName(x.Name)).ToList();
            var texture =
                Renderer.DrawableImages.First(i => i.Type == ImageType.Texture && i.Name == PicForm.Texture.Name);
            var rects = masks
                .Select(i => new Envelope(0, i.WidthMinusMargin, 0, i.HeightMinusMargin));

            var src = new CancellationTokenSource();

            var progress = new Progress<double>();
            var task = Task.Factory.StartNew(() => selected.FindCovering(rects, src.Token, progress,
                iterations: PicForm.IterationCount,
                minRectCover: PicForm.MinCoverPercentage / 100).ToList(), src.Token);

            var progressForm = new ProgressDialog(task, src, progress);
            BeginInvoke(new Action(() => { progressForm.ShowDialog(); }));
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
                    new Picture(PicForm.Clipping, PicForm.Distance,
                        new Vector(c.MinX - m.EmptyPixelXMargin, c.MaxY + m.EmptyPixelYMargin), texture, m));
            Lev.Pictures.AddRange(pics);
            SetModified(LevModification.Decorations);
        }

        private void SetModifiedAndRender(LevModification value)
        {
            SetModified(value);
            RedrawScene();
        }

        private void KeyHandlerUp(object sender, KeyEventArgs e)
        {
            PlayController.UpdateInputKeys();
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
            var data = e.Data.GetData(DataFormats.FileDrop);
            // BeginInvoke is required for Wine
            BeginInvoke(new Action(() =>
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
            }));
        }

        private void LevelPropertiesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var levelProperties = new LevelPropertiesForm(Lev);
            levelProperties.ShowDialog();
        }

        private void LevelPropertyModified(object sender, EventArgs e)
        {
            if (!_programmaticPropertyChange)
            {
                Lev.Title = TitleBox.Text;
                Lev.LgrFile = LGRBox.Text;
                if (sender.Equals(SkyComboBox) || sender.Equals(GroundComboBox))
                {
                    if (sender.Equals(GroundComboBox))
                        Lev.GroundTextureName = GroundComboBox.SelectedItem.ToString();
                    if (sender.Equals(SkyComboBox))
                        Lev.SkyTextureName = SkyComboBox.SelectedItem.ToString();
                    if (Global.AppSettings.LevelEditor.RenderingSettings.DefaultGroundAndSky)
                        UiUtils.ShowError("Default ground and sky is enabled, so you won\'t see this change in editor.",
                            "Warning", MessageBoxIcon.Exclamation);
                    Renderer.UpdateGroundAndSky(Global.AppSettings.LevelEditor.RenderingSettings.DefaultGroundAndSky);
                    RedrawScene();
                }

                SetModified(LevModification.Decorations);
            }
        }

        private void LoadFromHistory()
        {
            var oldPath = Lev.Path;
            Lev = _history[_historyIndex].Clone();
            Lev.Path = oldPath;
            Renderer.Lev = Lev;
            _zoomCtrl.Lev = Lev;
            Lev.DecomposeGroundPolygons();
            UpdateUndoRedo();
            topologyList.DropDownItems.Clear();
            topologyList.Text = "";
            _errorPoints.Clear();
            Renderer.UpdateGroundAndSky(Global.AppSettings.LevelEditor.RenderingSettings.DefaultGroundAndSky);
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
            SetModified(LevModification.Objects | LevModification.Ground | LevModification.Decorations);
            RedrawScene();
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            Vector p = GetMouseCoordinates();
            CurrentTool.MouseMove(p);
            int nearestVertexIndex = ToolBase.GetNearestVertexIndex(p);
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
                                case ObjectType.Start:
                                    saveStartPositionToolStripMenuItem.Visible = true;
                                    if (_savedStartPosition != null)
                                    {
                                        restoreStartPositionToolStripMenuItem.Visible = true;
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        if (nearestVertexIndex >= -1)
                        {
                            GrassMenuItem.Visible = true;
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
                                switch (PlayController.Driver.GravityDirection)
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
                double currSize = Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
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
            var settings = Global.AppSettings.LevelEditor.RenderingSettings;
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

        private void NewLevel(object sender = null, EventArgs e = null)
        {
            if (!PromptToSaveIfModified())
                return;
            SetBlankLevel();
            InitializeLevel();
        }

        private void ObjectButtonChanged(object sender, EventArgs e)
        {
            if (ObjectButton.Checked)
                ChangeToolTo(3);
        }

        private void OpenConfig(object sender, EventArgs e)
        {
            string oldLgr = Global.AppSettings.LevelEditor.RenderingSettings.LgrFile;
            ComponentManager.ShowConfiguration("sle");
            AfterSettingsClosed(oldLgr);
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
            string oldLgr = Global.AppSettings.LevelEditor.RenderingSettings.LgrFile;
            var rSettings = new RenderingSettingsForm(Global.AppSettings.LevelEditor.RenderingSettings);
            rSettings.Changed += x =>
            {
                Renderer.UpdateSettings(x);
                RedrawScene();
            };
            rSettings.ShowDialog();
            AfterSettingsClosed(oldLgr);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenFileDialog1.InitialDirectory = GetInitialDir();
            OpenFileDialog1.Multiselect = false;
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                OpenLevel(OpenFileDialog1.FileName);
        }

        private void PictureButtonChanged(object sender, EventArgs e)
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
            var selectedPics = Lev.Pictures.Where(p => p.Position.Mark == VectorMark.Selected).ToList();
            PicForm.SetDefaultsAutomatically = Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool;
            if (selectedPics.Count > 0)
            {
                PicForm.AllowMultiple = true;
                PicForm.SelectMultiple(selectedPics);
            }
            else
            {
                PicForm.AllowMultiple = false;
                selectedPics = new List<Picture> {Lev.Pictures[_selectedPictureIndex]};
                PicForm.SelectElement(Lev.Pictures[_selectedPictureIndex]);
            }

            PicForm.AutoTextureMode = false;
            PicForm.ShowDialog();
            if (PicForm.OkButtonPressed)
            {
                foreach (var selected in selectedPics)
                {
                    var clipping = PicForm.MultipleClippingSelected ? selected.Clipping : PicForm.Clipping;
                    var distance = PicForm.MultipleDistanceSelected ? selected.Distance : PicForm.Distance;
                    var mask = PicForm.MultipleMaskSelected
                        ? Renderer.DrawableImageFromName(selected.Name)
                        : Renderer.DrawableImageFromName(PicForm.Mask.Name);
                    var position = selected.Position;
                    var texture = PicForm.MultipleTexturesSelected
                        ? Renderer.DrawableImageFromName(selected.TextureName)
                        : Renderer.DrawableImageFromName(PicForm.Texture.Name);
                    var picture = PicForm.MultiplePicturesSelected
                        ? Renderer.DrawableImageFromName(selected.Name)
                        : Renderer.DrawableImageFromName(PicForm.Picture.Name);

                    if ((PicForm.TextureSelected && !PicForm.MultipleTexturesSelected))
                    {
                        if (selected.IsPicture)
                        {
                            // need to set proper mask; otherwise the mask name will be picture name
                            mask = Renderer.DrawableImageFromName(_editorLgr.ListedImages.First(i => i.Type == ImageType.Mask).Name);
                        }

                        selected.SetTexture(clipping, distance, position, texture,
                            mask);
                    }
                    else if ((!PicForm.TextureSelected && !PicForm.MultiplePicturesSelected))
                    {
                        selected.SetPicture(picture, position,
                            distance,
                            clipping);
                    }
                    else
                    {
                        if (selected.IsPicture)
                        {
                            selected.SetPicture(picture, position,
                                distance,
                                clipping);
                        }
                        else
                        {
                            selected.SetTexture(clipping, distance, position, texture,
                                mask);
                        }
                    }
                }

                SetModified(LevModification.Decorations);
                RedrawScene();
            }
        }

        private void PipeButtonChanged(object sender, EventArgs e)
        {
            if (PipeButton.Checked)
                ChangeToolTo(4);
        }

        private void PolyOpButtonChanged(object sender, EventArgs e)
        {
            if (PolyOpButton.Checked)
                ChangeToolTo(7);
        }

        private void PrevNextButtonClick(object sender, EventArgs e)
        {
            if (CurrLevDirExists())
            {
                if (CurrLevDirFiles.Count > 0)
                {
                    if (Lev.Path == null)
                        OpenLevel(CurrLevDirFiles[0]);
                    else
                    {
                        int i = GetCurrentLevelIndex();
                        if (sender.Equals(PreviousButton) || sender.Equals(previousLevelToolStripMenuItem))
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
                else
                    UiUtils.ShowError("There are no levels in this directory!");
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
            var mod = LevModification.Nothing;
            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
            {
                Polygon x = Lev.Polygons[i];
                if (!x.IsGrass)
                {
                    var autoGrass = ((AutoGrassTool) (Tools[11])).AutoGrass(x);
                    Lev.Polygons.AddRange(autoGrass);
                    if (autoGrass.Count > 0)
                    {
                        mod = LevModification.Decorations;
                    }
                }
            }

            SetModified(mod);
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

        private void SaveAs(object sender = null, EventArgs e = null)
        {
            string suggestion = string.Empty;
            if (Global.AppSettings.LevelEditor.UseFilenameSuggestion)
            {
                var filenameStart = Global.AppSettings.LevelEditor.BaseFilename;
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

                    suggestion = filenameStart + newNumber.ToString(Global.AppSettings.LevelEditor.NumberFormat);
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
                Lev.Path = SaveFileDialog1.FileName;
                SaveLevel();
            }
        }

        private string GetInitialDir()
        {
            if (Lev.Path != null)
            {
                return Path.GetDirectoryName(Lev.Path);
            }
            return Global.AppSettings.General.LevelDirectory;
        }

        private void SaveClicked(object sender = null, EventArgs e = null)
        {
            if (Lev.Path == null)
                SaveAs();
            else
                SaveLevel();
        }

        private void SaveLevel()
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
                if (Global.AppSettings.LevelEditor.CheckTopologyWhenSaving)
                    CheckTopologyAndUpdate();
                if (Global.AppSettings.LevelEditor.UseFilenameForTitle && _fromScratch)
                {
                    Lev.Title = Path.GetFileNameWithoutExtension(SaveFileDialog1.FileName);
                }

                try
                {
                    Lev.Save(Lev.Path);
                    _savedIndex = _historyIndex;
                    _fromScratch = false;
                    if (!Global.GetLevelFiles().Contains(Lev.Path))
                    {
                        Global.GetLevelFiles().Add(Lev.Path);
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
                if ((polygon.IsGrass && _grassFilter) || (!polygon.IsGrass && _groundFilter))
                    polygon.MarkVectorsAs(VectorMark.Selected);
            }

            foreach (var levelObject in Lev.Objects)
            {
                switch (levelObject.Type)
                {
                    case ObjectType.Apple:
                        if (_appleFilter)
                            levelObject.Position.Select();
                        break;
                    case ObjectType.Killer:
                        if (_killerFilter)
                            levelObject.Position.Select();
                        break;
                    case ObjectType.Flower:
                        if (_flowerFilter)
                            levelObject.Position.Select();
                        break;
                }
            }

            foreach (var texture in Lev.Pictures)
            {
                if ((_textureFilter && !texture.IsPicture) || (_pictureFilter && texture.IsPicture))
                    texture.Position.Select();
            }

            RedrawScene();
            UpdateSelectionInfo();
        }

        private void SelectButtonChanged(object sender, EventArgs e)
        {
            if (SelectButton.Checked)
                ChangeToolTo(0);
        }

        private void SendToBackToolStripMenuItemClick(object sender, EventArgs e)
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
                var obj = Lev.Pictures[_selectedPictureIndex];
                Lev.Pictures.RemoveAt(_selectedPictureIndex);
                Lev.Pictures.Add(obj);
                mod |= LevModification.Decorations;
            }

            SetModified(mod);
        }

        private void SetAllFilters(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem x in SelectionFilterToolStripMenuItem.DropDownItems)
                if (x.CheckOnClick)
                    x.Checked = sender.Equals(EnableAllToolStripMenuItem);
        }

        private void SetDefaultLevelTitle()
        {
            if (!Global.AppSettings.LevelEditor.UseFilenameForTitle)
                Lev.Title = Global.AppSettings.LevelEditor.DefaultTitle;
        }

        private void SetBlankLevel()
        {
            Lev = Global.AppSettings.LevelEditor.GetTemplateLevel();
            SetDefaultLevelTitle();
            _fromScratch = true;
            _savedStartPosition = null;
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            var settings = Global.AppSettings.LevelEditor.RenderingSettings;
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
            Global.AppSettings.LevelEditor.SnapToGrid = snapToGridButton.Checked;
            Global.AppSettings.LevelEditor.ShowCrossHair = showCrossHairButton.Checked;
            Renderer.UpdateSettings(settings);
            RedrawScene();
        }

        private void SetupEventHandlers()
        {
            Resize += ViewerResized;
            EditorControl.Paint += RedrawScene;
            ZoomFillButton.Click += (s, e) => _zoomCtrl.ZoomFill();
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

        private void MouseWheelZoom(object sender, MouseEventArgs e)
        {
            MouseWheelZoom(e.Delta);
        }

        private void ShowCoordinates()
        {
            Vector x = GetMouseCoordinates();
            CoordinateLabel.Text = "Mouse X: " + x.X.ToString(CoordinateFormat) + " Y: " +
                                   x.Y.ToString(CoordinateFormat);
        }

        private void SmoothenButtonChanged(object sender, EventArgs e)
        {
            if (SmoothenButton.Checked)
                ChangeToolTo(9);
        }

        private void StartingDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data is string[] files)
            {
                if (files.All(filePath => File.Exists(filePath) && ImportableExtensions.Any(ext => Path.GetExtension(filePath).CompareWith(ext))))
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
            var settings = Global.AppSettings.LevelEditor.RenderingSettings;
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
            snapToGridButton.Checked = Global.AppSettings.LevelEditor.SnapToGrid;
            showCrossHairButton.Checked = Global.AppSettings.LevelEditor.ShowCrossHair;
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
            string levDir = Path.GetDirectoryName(Lev.Path);
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
            if (Lev.Path == null)
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
                Text = Lev.FileNameNoExt + " - " + LevEditorName;
                filenameBox.Text = Lev.FileNameNoExt;
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
            if (Renderer.CurrentLgr != null && _editorLgr?.Path != Renderer.CurrentLgr.Path)
            {
                _editorLgr = Renderer.CurrentLgr;
                PicturePropertiesMenuItem.Enabled = true;
                SkyComboBox.Enabled = true;
                GroundComboBox.Enabled = true;
                if (PicForm != null)
                    PicForm.UpdateLgr(_editorLgr);
                else
                    PicForm = new PictureForm(_editorLgr);
                SkyComboBox.Items.Clear();
                GroundComboBox.Items.Clear();
                foreach (var texture in _editorLgr.ListedImagesExcludingSpecial.Where(image =>
                    image.Type == ImageType.Texture))
                {
                    SkyComboBox.Items.Add(texture.Name);
                    GroundComboBox.Items.Add(texture.Name);
                }
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

        private void VertexButtonChanged(object sender, EventArgs e)
        {
            if (VertexButton.Checked)
                ChangeToolTo(1);
        }

        private void ViewerResized(object sender = null, EventArgs e = null)
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

        private void ZoomButtonChanged(object sender, EventArgs e)
        {
            if (ZoomButton.Checked)
                ChangeToolTo(5);
        }

        private void ZoomFillToolStripMenuItemClick(object sender, EventArgs e)
        {
            _zoomCtrl.ZoomFill();
        }

        private void TitleBoxTextChanged(object sender, EventArgs e)
        {
            int width = TextRenderer.MeasureText(TitleBox.Text, TitleBox.Font).Width;
            TitleBox.Width = Math.Max(width + 5, 120 * (int) _dpiX);
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
                        lev = Level.FromPath(file);
                    }
                    catch (BadFileException exception)
                    {
                        UiUtils.ShowError(
                            $"Imported level {file} with errors: {exception.Message}",
                            "Warning",
                            MessageBoxIcon.Exclamation);
                        lev = new Level();
                    }

                    lev.UpdateImages(Renderer.DrawableImages);
                }
                else if (file.EndsWith(".svg") || file.EndsWith(".svgz"))
                {
                    var result = SvgImportOptionsForm.ShowDefault(_svgImportOptions, file);
                    if (!result.HasValue)
                    {
                        return;
                    }

                    var newOpts = result.Value;
                    _svgImportOptions = newOpts;
                    var settings = new WpfDrawingSettings
                        {IncludeRuntime = false, TextAsGeometry = true, IgnoreRootViewbox = true};
                    using var converter = new FileSvgReader(settings);
                    var drawingGroup = converter.Read(new Uri(file));
                    List<Polygon> polys;
                    try
                    {
                        (polys, _) = TextTool.BuildPolygons(
                            TextTool.CreateGeometry(drawingGroup, newOpts),
                            new Vector(),
                            newOpts.Smoothness,
                            newOpts.UseOutlinedGeometry);
                    }
                    catch (PolygonException)
                    {
                        UiUtils.ShowError($"Failed to import SVG {file}. Invalid or animated SVGs are not supported.");
                        return;
                    }

                    try
                    {
                        TextTool.FinalizePolygons(polys);
                    }
                    catch (TopologyException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }

                    var m = Matrix.CreateScaling(1 / 10.0, 1 / 10.0);
                    polys = polys.Select(p => p.ApplyTransformation(m)).ToList();
                    lev = new Level();
                    lev.Polygons.AddRange(polys);
                }
                else
                {
                    try
                    {
                        lev = VectrastWrapper.LoadLevelFromImage(file);
                    }
                    catch (VectrastException ex)
                    {
                        UiUtils.ShowError(ex.Message);
                        return;
                    }
                }

                imported++;
                Lev.Import(lev);
            });
            if (imported > 0)
            {
                SetModified(LevModification.Objects | LevModification.Ground | LevModification.Decorations);
                _zoomCtrl.ZoomFill();
            }
        }

        private void saveAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsPictureDialog.FileName = Lev.FileNameNoExt ?? "Untitled";
            if (saveAsPictureDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveAsPictureDialog.FileName.EndsWith(".png"))
                {
                    Renderer.SaveSnapShot(saveAsPictureDialog.FileName, _zoomCtrl, _sceneSettings);
                }
                else if (saveAsPictureDialog.FileName.EndsWith(".svg"))
                {
                    var g = new SvgGraphics(Color.LightGray);
                    var blackPen = Pens.Black;
                    var greenPen = Pens.Green;
                    const int scale = 10;
                    var m = Matrix.CreateTranslation(-Lev.XMin + 1, -Lev.YMin + 1) * Matrix.CreateScaling(scale, scale);
                    var objOffset = new Vector(-0.4, -0.4);
                    const float oSize = (float)0.8 * scale;
                    Lev.Polygons.ForEach(p =>
                    {
                        if (p.IsGrass && Global.AppSettings.LevelEditor.RenderingSettings.ShowGrassOrEdges)
                        {
                            g.DrawLines(greenPen, p
                                .ApplyTransformation(m)
                                .Vertices.Select(v => new PointF((float) v.X, (float) v.Y)).ToArray());
                        }
                        else if (!p.IsGrass && Global.AppSettings.LevelEditor.RenderingSettings.ShowGroundOrEdges)
                        {
                            g.DrawPolygon(blackPen, p
                                .ApplyTransformation(m)
                                .Vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
                        }
                    });
                    if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectsOrFrames)
                    {
                        Lev.Objects.ForEach(o =>
                        {
                            var pos = (o.Position + objOffset) * m;
                            switch (o.Type)
                            {
                                case ObjectType.Flower:
                                    g.DrawEllipse(Pens.White, (float)pos.X, (float)pos.Y, oSize, oSize);
                                    break;
                                case ObjectType.Apple:
                                    g.DrawEllipse(Pens.Red, (float)pos.X, (float)pos.Y, oSize, oSize);
                                    break;
                                case ObjectType.Killer:
                                    g.DrawEllipse(Pens.Brown, (float)pos.X, (float)pos.Y, oSize, oSize);
                                    break;
                                case ObjectType.Start:
                                    g.DrawEllipse(Pens.Blue, (float)pos.X, (float)pos.Y, oSize, oSize);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });
                    }

                    var svgBody = g.WriteSVGString();
                    var width = (int) ((Lev.Width + 2) * scale);
                    var height = (int) ((Lev.Height + 2) * scale);
                    svgBody = svgBody.Replace("<svg ", $@"<svg width=""{width}"" height=""{height}"" ");
                    File.WriteAllText(saveAsPictureDialog.FileName, svgBody);
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
                Lev.Pictures.Where(v => v.Position.Mark == VectorMark.Selected).Select(p => p.Position));

            void RemoveSelected()
            {
                var first = Lev.Polygons.First().Clone();
                Lev.Polygons.ForEach(p => p.Vertices.RemoveAll(v => v.Mark == VectorMark.Selected));
                Lev.Polygons.RemoveAll(p => p.Count < 3);
                if (Lev.Polygons.Count == 0)
                {
                    Lev.Polygons.Add(first);
                }

                Lev.Objects.RemoveAll(o =>
                    o.Position.Mark == VectorMark.Selected && o.Type != ObjectType.Start);
                Lev.Pictures.RemoveAll(p => p.Position.Mark == VectorMark.Selected);
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
                PicForm.AllowMultiple = false;
                PicForm.AutoTextureMode = false;
                PicForm.SetDefaultsAutomatically = true;
                PicForm.SetDefaultDistanceAndClipping();
                PicForm.ShowDialog();
                if (PicForm.OkButtonPressed)
                {
                    RemoveSelected();
                    foreach (var selectedVertex in selectedVertices)
                    {
                        if (PicForm.TextureSelected)
                        {
                            Lev.Pictures.Add(new Picture(PicForm.Clipping, PicForm.Distance,
                                selectedVertex,
                                Renderer.DrawableImageFromName(PicForm.Texture.Name),
                                Renderer.DrawableImageFromName(PicForm.Mask.Name)));
                        }
                        else
                        {
                            Lev.Pictures.Add(new Picture(Renderer.DrawableImageFromName(PicForm.Picture.Name),
                                selectedVertex, PicForm.Distance,
                                PicForm.Clipping));
                        }
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
            if (Lev.Path == null)
            {
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this level?", "Confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UpdateCurrLevDirFiles();
                File.Delete(Lev.Path);

                int levIndex = GetCurrentLevelIndex();
                CurrLevDirFiles.RemoveAt(levIndex);
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

        private int GetCurrentLevelIndex()
        {
            return CurrLevDirFiles.FindIndex(
                path => string.Compare(path, Lev.Path, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteCurrentLevel();
        }

        private void filenameBox_TextChanged(object sender = null, EventArgs e = null)
        {
            bool showButtons = string.Compare(filenameBox.Text,
                                   Lev.FileNameNoExt,
                                   StringComparison.InvariantCulture) != 0 && Lev.Path != null;
            filenameOkButton.Visible = showButtons;
            filenameCancelButton.Visible = showButtons;
        }

        private void filenameCancelButton_Click(object sender = null, EventArgs e = null)
        {
            filenameBox.Text = Lev.FileNameNoExt;
        }

        private void filenameOkButton_Click(object sender = null, EventArgs e = null)
        {
            if (filenameBox.Text == string.Empty)
            {
                UiUtils.ShowError("The filename cannot be empty.");
                return;
            }

            try
            {
                var newPath = Path.Combine(Path.GetDirectoryName(Lev.Path), filenameBox.Text + ".lev");
                UpdateCurrLevDirFiles();
                File.Move(Lev.Path, newPath);
                if (CurrLevDirFiles != null)
                {
                    int index = GetCurrentLevelIndex();
                    CurrLevDirFiles[index] = newPath;
                }

                Lev.Path = newPath;
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
            if (Lev.Path == null)
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

        private void SaveStartPosition(object sender = null, EventArgs e = null)
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

        private async void playButton_Click(object sender, EventArgs e)
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

            SetToPlaying();
            var t = new Timer(25);
            var updateTime = new Action(() =>
            {
                PlayTimeLabel.Text = PlayController.Driver.CurrentTime.ToSeconds().ToTimeString(3);
            });
            t.Elapsed += (_, _) =>
            {
                Invoke(updateTime);
            };
            t.Start();
            CameraUtils.AllowScroll = false;
            await PlayController.BeginLoop(Lev, _sceneSettings, Renderer, _zoomCtrl, DoRedrawScene);
            t.Stop();
            PlayTimeLabel.Text = PlayController.Driver.CurrentTime.ToSeconds().ToTimeString(3);
            if (PlayController.Driver.Condition == DriverCondition.Finished)
            {
                PlayTimeLabel.Text += " F";
            }
            RedrawScene();
            SetNotPlaying();
            stopButton.Enabled = false;
        }

        private void SetNotPlaying()
        {
            playButton.SvgData = Resources.Play;
            playButton.ToolTipText = "Play";
        }

        private void SetToPlaying()
        {
            playButton.SvgData = Resources.Pause;
            playButton.ToolTipText = "Pause";
            stopButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (PlayController.PlayingOrPaused)
            {
                PlayController.PlayingStopRequested = true;
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            var f = new PlaySettingsForm(PlayController.Settings);
            var result = f.ShowDialog();
            if (result == DialogResult.OK)
            {
                PlayController.Settings = f.Settings;
                Global.AppSettings.LevelEditor.PlayingSettings = f.Settings;
            }
        }

        private static readonly string[] ImportableExtensions = {DirUtils.LevExtension, DirUtils.LebExtension, ".bmp", ".png", ".gif", ".tiff", ".exif", ".svg", ".svgz" };
    }
}