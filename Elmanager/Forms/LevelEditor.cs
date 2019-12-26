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
using Elmanager.CustomControls;
using Elmanager.EditorTools;
using NetTopologySuite.Geometries;
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

namespace Elmanager.Forms
{
    partial class LevelEditor : FormMod
    {
        //TODO Tool interface should be improved
        private const string CoordinateFormat = "F3";
        private const string LevEditorName = "SLE";
        private const int MouseWheelStep = 20;
        private readonly List<Level> _history = new List<Level>();
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
        private Lgr _editorLgr;
        private List<Vector> _errorPoints = new List<Vector>();
        private int _historyIndex;
        private int _savedIndex;
        private string _loadedLevFilesDir;
        private int _lockCoord;
        private bool _lockMouseX;
        private bool _lockMouseY;
        private bool _modified;
        private bool _fromScratch;
        private Vector _moveStartPosition;
        private string _savePath;
        private int _selectedObjectCount;
        private int _selectedObjectIndex;
        private int _selectedPictureCount;
        private int _selectedPictureIndex;
        private int _selectedPolygonCount;
        private int _selectedTextureCount;
        private int _selectedVerticeCount;
        private bool _pictureToolAvailable;
        private bool _draggingGrid;
        private Vector _gridStartOffset;
        private bool _programmaticPropertyChange;
        private Vector _savedStartPosition;
        private float _dpiX;
        private float _dpiY;
        private Vector _contextMenuClickPosition;
        private SvgImportOptions _svgImportOptions = SvgImportOptions.Default;
        private bool _maybeOpenOnDrop;

        internal LevelEditor(string levPath)
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
            catch (LevelException ex)
            {
                Utils.ShowError("Error occurred while loading level file: " + ex.Message, "Warning",
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

        public LevelEditor()
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

        internal bool Modified
        {
            get => _modified;
            set => SetModified(value);
        }

        internal bool EffectiveAppleFilter => _appleFilter &&
                                              (ShowObjectFramesButton.Checked ||
                                               (ShowObjectsButton.Checked && _pictureToolAvailable));

        internal bool EffectiveKillerFilter => _killerFilter &&
                                               (ShowObjectFramesButton.Checked ||
                                                (ShowObjectsButton.Checked && _pictureToolAvailable));

        internal bool EffectiveFlowerFilter => _flowerFilter &&
                                               (ShowObjectFramesButton.Checked ||
                                                (ShowObjectsButton.Checked && _pictureToolAvailable));

        internal bool EffectiveGrassFilter => _grassFilter && (ShowGrassEdgesButton.Checked);

        internal bool EffectiveGroundFilter => _groundFilter &&
                                               (ShowGroundEdgesButton.Checked ||
                                                (ShowGroundButton.Checked && _pictureToolAvailable));

        internal bool EffectiveTextureFilter => _textureFilter &&
                                                (ShowTextureFramesButton.Checked ||
                                                 (ShowTexturesButton.Checked && _pictureToolAvailable));

        internal bool EffectivePictureFilter => _pictureFilter &&
                                                (ShowPictureFramesButton.Checked ||
                                                 (ShowPicturesButton.Checked && _pictureToolAvailable));

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

        internal void ActivateCurrentAndRedraw()
        {
            CurrentTool.Activate();
            Renderer.RedrawScene();
        }

        internal void InactivateCurrentAndRedraw()
        {
            CurrentTool.InActivate();
            Renderer.RedrawScene();
        }

        internal void SetModified(bool value, bool updateHistory = true)
        {
            _modified = value;
            EnableSaveButtons(value);
            if (value)
            {
                Lev.UpdateBounds();
                if (updateHistory)
                    AddToHistory();
                if (Global.AppSettings.LevelEditor.CheckTopologyDynamically)
                    CheckTopology();
                Renderer.UpdateZoomFillBounds();
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
            if (oldLgr != Global.AppSettings.LevelEditor.RenderingSettings.LgrFile)
                UpdateLgrTools();
            UpdateButtons();
            Renderer.RedrawScene();
        }

        private void AutoGrassButtonChanged(object sender, EventArgs e)
        {
            if (AutoGrassButton.Checked)
                ChangeToolTo(11);
        }

        private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_selectedObjectIndex >= 0)
            {
                var obj = Lev.Objects[_selectedObjectIndex];
                Lev.Objects.RemoveAt(_selectedObjectIndex);
                Lev.Objects.Add(obj);
            }
            else if (_selectedPictureIndex >= 0)
            {
                var obj = Lev.Pictures[_selectedPictureIndex];
                Lev.Pictures.RemoveAt(_selectedPictureIndex);
                Lev.Pictures.Insert(0, obj);
            }

            Modified = true;
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
            Renderer.RedrawScene();
        }

        private void ClearHistory()
        {
            _history.Clear();
            _history.Add(Lev.Clone());
            _historyIndex = 0;
            _savedIndex = -1;
            UpdateUndoRedo();
        }

        private void ConfirmClose(object sender, CancelEventArgs e)
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
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            var copiedPolygons = new List<Polygon>();
            var copiedObjects = new List<LevObject>();
            var copiedTextures = new List<Picture>();
            Vector.MarkDefault = VectorMark.Selected;
            var delta = Keyboard.IsKeyDown(Key.LeftShift)
                ? Global.AppSettings.LevelEditor.RenderingSettings.GridSize
                : Renderer.ZoomLevel * 0.1;
            foreach (Polygon x in Lev.Polygons)
            {
                var copy = new Polygon();
                foreach (Vector z in x.Vertices)
                {
                    if (z.Mark == VectorMark.Selected)
                    {
                        z.Mark = VectorMark.None;
                        copy.Add(new Vector(z.X + delta,
                            z.Y + delta));
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
                                delta), x.Type, x.AppleType,
                            x.AnimationNumber));
                }
            }

            foreach (Picture x in Lev.Pictures)
            {
                if (x.Position.Mark == VectorMark.Selected)
                {
                    Picture copiedPicture = x.Clone();
                    copiedPicture.Position.X += delta;
                    copiedPicture.Position.Y += delta;
                    copiedTextures.Add(copiedPicture);
                    x.Position.Mark = VectorMark.None;
                }
            }

            Vector.MarkDefault = VectorMark.None;
            Lev.Polygons.AddRange(copiedPolygons);
            Lev.Objects.AddRange(copiedObjects);
            Lev.Pictures.AddRange(copiedTextures);
            if (copiedObjects.Count + copiedPolygons.Count + copiedTextures.Count > 0)
                Modified = true;
            Renderer.RedrawScene();
        }

        private bool CurrLevDirExists()
        {
            return Directory.Exists(Path.GetDirectoryName(Lev.Path));
        }

        private void CustomRendering()
        {
            CurrentTool.ExtraRendering();
            if (Global.AppSettings.LevelEditor.ShowCrossHair)
            {
                var mouse = GetMouseCoordinatesFixed();
                Renderer.DrawDashLine(Renderer.XMin, mouse.Y, Renderer.XMax,
                    mouse.Y, Global.AppSettings.LevelEditor.CrosshairColor);
                Renderer.DrawDashLine(mouse.X, -Renderer.YMax, mouse.X,
                    -Renderer.YMin, Global.AppSettings.LevelEditor.CrosshairColor);
            }

            Action<Vector, Color> drawAction = Global.AppSettings.LevelEditor.RenderingSettings.UseCirclesForVertices
                ? (Action<Vector, Color>) ((pt, color) => Renderer.DrawPoint(pt, color))
                : ((pt, color) => Renderer.DrawEquilateralTriangle(pt,
                    Renderer.ZoomLevel * Global.AppSettings.LevelEditor.RenderingSettings.VertexSize, color));

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
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y + t.Height,
                            Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case VectorMark.Highlight:
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y + t.Height,
                            Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
            }

            foreach (Vector x in _errorPoints)
                Renderer.DrawSquare(x, Renderer.ZoomLevel / 25, Color.Red);
            if ((object) _savedStartPosition != null)
            {
                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects)
                {
                    Renderer.DrawDummyPlayer(_savedStartPosition.X, -_savedStartPosition.Y, false, true);
                }

                if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectFrames)
                {
                    Renderer.DrawDummyPlayer(_savedStartPosition.X, -_savedStartPosition.Y, false, false);
                }
            }
        }

        private void CutButtonChanged(object sender, EventArgs e)
        {
            if (CutConnectButton.Checked)
                ChangeToolTo(10);
        }

        private void DeleteAllGrassToolStripMenuItemClick(object sender, EventArgs e)
        {
            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
            {
                Polygon x = Lev.Polygons[i];
                if (x.IsGrass)
                    Lev.Polygons.Remove(x);
            }

            Modified = true;
            Renderer.RedrawScene();
        }

        private void DeleteSelected(object sender, EventArgs e)
        {
            if (!CurrentTool.Busy)
            {
                bool anythingDeleted = false;
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
                            anythingDeleted = true;
                            polyModified = true;
                        }
                    }

                    if (x.Vertices.Count < 3)
                        Lev.Polygons.Remove(x);
                    else if (polyModified)
                        x.UpdateDecomposition();
                }

                for (int i = Lev.Objects.Count - 1; i >= 0; i--)
                {
                    if (Lev.Objects[i].Position.Mark == VectorMark.Selected)
                    {
                        if (Lev.Objects[i].Type != ObjectType.Start)
                        {
                            Lev.Objects.RemoveAt(i);
                            anythingDeleted = true;
                        }
                    }
                }

                for (int i = Lev.Pictures.Count - 1; i >= 0; i--)
                {
                    Picture x = Lev.Pictures[i];
                    if (x.Position.Mark == VectorMark.Selected)
                    {
                        Lev.Pictures.Remove(x);
                        anythingDeleted = true;
                    }
                }

                if (anythingDeleted)
                {
                    Modified = true;
                    UpdateSelectionInfo();
                }
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

            Renderer.RedrawScene();
            SelectionFilterToolStripMenuItem.ShowDropDown();
        }

        private void FrameButtonChanged(object sender, EventArgs e)
        {
            if (FrameButton.Checked)
                ChangeToolTo(8);
        }

        private Vector GetMouseCoordinates()
        {
            Point mousePosNoTr = EditorControl.PointToClient(MousePosition);
            var mousePos = new Vector
            {
                X =
                    Renderer.XMin +
                    mousePosNoTr.X * (Renderer.XMax - Renderer.XMin) / EditorControl.Width,
                Y =
                    Renderer.YMax -
                    mousePosNoTr.Y * (Renderer.YMax - Renderer.YMin) / EditorControl.Height
            };
            return mousePos;
        }

        private Vector GetMouseCoordinatesFixed()
        {
            Point mousePosNoTr = EditorControl.PointToClient(MousePosition);
            var mousePos = new Vector
            {
                X =
                    Renderer.XMin +
                    mousePosNoTr.X * (Renderer.XMax - Renderer.XMin) / EditorControl.Width,
                Y =
                    -Renderer.YMax +
                    mousePosNoTr.Y * (Renderer.YMax - Renderer.YMin) / EditorControl.Height
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

            polys.ForEach(p =>
            {
                p.IsGrass = !p.IsGrass;
                p.UpdateDecomposition();
            });
            Modified = true;
            Renderer.RedrawScene();
        }

        private void HandleGravityMenu(object sender, EventArgs e)
        {
            LevObject currApple = Lev.Objects[_selectedObjectIndex];
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

            Modified = true;
        }

        private void UpdateLgrFromLev()
        {
            if (Directory.Exists(Global.AppSettings.General.LgrDirectory))
            {
                var lgr = Path.Combine(Global.AppSettings.General.LgrDirectory, Lev.LgrFile + ".lgr");
                if (File.Exists(lgr))
                {
                    Global.AppSettings.LevelEditor.RenderingSettings.LgrFile = lgr;
                }
            }
        }

        private void Initialize()
        {
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
            UpdateLgrFromLev();
            Renderer = new ElmaRenderer(Lev, EditorControl, Global.AppSettings.LevelEditor.RenderingSettings);
            UpdateLgrTools();
            ClearHistory();
            UpdateLabels();
            Renderer.CustomRendering = CustomRendering;
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
            ActivateCurrentAndRedraw();
            SetupEventHandlers();
            _savePath = Lev.Path;
        }

        private void InitializeLevel()
        {
            _savePath = Lev.Path;
            Modified = false;
            UpdateLabels();
            UpdateButtons();
            Renderer.InitializeLevel(Lev);
            Renderer.ZoomFill();
            UpdateLgrFromLev();
            Renderer.UpdateSettings(Global.AppSettings.LevelEditor.RenderingSettings);
            topologyList.Text = string.Empty;
            topologyList.DropDownItems.Clear();
            ResetTopologyListStyle();
            UpdateLgrTools();
            ClearHistory();
            CurrentTool.InActivate();
            ActivateCurrentAndRedraw();
            _errorPoints.Clear();
        }

        private void KeyHandlerDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Add:
                    e = new KeyEventArgs(Constants.Increase);
                    break;
                case Keys.Subtract:
                    e = new KeyEventArgs(Constants.Decrease);
                    break;
            }

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
                    Utils.BeginArrowScroll(Renderer);
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
                case Keys.Enter:
                    wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.Intersection, Lev.Polygons);
                    break;
                case Keys.Oem2:
                    wasModified = PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons);
                    break;
            }

            if (wasModified)
            {
                Modified = true;
            }

            Renderer.RedrawScene();
        }

        private async void TexturizeSelection()
        {
            if (!_pictureToolAvailable)
            {
                Utils.ShowError("You need to select LGR file from settings before you can use texturize tool.", "Note",
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
                Renderer.DrawableImages.First(i => i.Type == Lgr.ImageType.Texture && i.Name == PicForm.Texture.Name);
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
                Utils.ShowError(e.Message);
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
                        new Vector(c.MinX - m.EmptyPixelXMargin, c.MinY - m.EmptyPixelYMargin), texture, m));
            Lev.Pictures.AddRange(pics);
            Modified = true;
        }

        private void SetModifiedAndRender()
        {
            Modified = true;
            Renderer.RedrawScene();
        }

        private void KeyHandlerUp(object sender, KeyEventArgs e)
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
                        Utils.ShowError("Default ground and sky is enabled, so you won\'t see this change in editor.",
                            "Warning", MessageBoxIcon.Exclamation);
                    Renderer.UpdateGroundAndSky(Global.AppSettings.LevelEditor.RenderingSettings.DefaultGroundAndSky);
                    Renderer.RedrawScene();
                }

                Modified = true;
            }
        }

        private void LoadFromHistory()
        {
            var oldPath = Lev.Path;
            Lev = _history[_historyIndex].Clone();
            Lev.Path = oldPath;
            Renderer.Lev = Lev;
            Lev.DecomposeGroundPolygons();
            Renderer.UpdateZoomFillBounds();
            UpdateUndoRedo();
            topologyList.DropDownItems.Clear();
            topologyList.Text = "";
            _errorPoints.Clear();
            Renderer.UpdateGroundAndSky(Global.AppSettings.LevelEditor.RenderingSettings.DefaultGroundAndSky);
            CurrentTool.InActivate();
            ActivateCurrentAndRedraw();
            UpdateLabels();
            SetModified(_savedIndex != _historyIndex, false);
        }

        private void MirrorHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lev.MirrorSelected(MirrorOption.Horizontal);
            Modified = true;
            Renderer.RedrawScene();
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            Vector p = GetMouseCoordinatesFixed();
            CurrentTool.MouseMove(p);
            int nearestVertexIndex = ToolBase.GetNearestVertexIndex(p);
            int nearestObjectIndex = ToolBase.GetNearestObjectIndex(p);
            int nearestPictureIndex = ToolBase.GetNearestPictureIndex(p);
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
                                    if ((object) _savedStartPosition != null)
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

                        EditorMenuStrip.Show(MousePosition);
                    }

                    break;
                case MouseButtons.Middle:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        _draggingGrid = true;
                        _gridStartOffset = Renderer.GridOffset;
                    }
                    else
                    {
                        _draggingScreen = true;
                    }

                    _moveStartPosition = GetMouseCoordinates();
                    break;
            }

            CurrentTool.MouseDown(e);
            Renderer.RedrawScene();
        }

        private void MouseLeaveEvent(object sender, EventArgs e)
        {
            CurrentTool.MouseOutOfEditor();
            Renderer.RedrawScene();
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
                    Renderer.GridOffset = _gridStartOffset + _moveStartPosition - z;
                }
                else
                {
                    Renderer.CenterX = (Renderer.XMax + Renderer.XMin) / 2 - (z.X - _moveStartPosition.X);
                    Renderer.CenterY = (Renderer.YMax + Renderer.YMin) / 2 - (z.Y - _moveStartPosition.Y);
                }
            }

            CurrentTool.MouseMove(GetMouseCoordinatesFixed());
            Renderer.RedrawScene();
            StatusStrip1.Refresh();
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            CurrentTool.MouseUp();
            _draggingScreen = false;
            _draggingGrid = false;
            Renderer.RedrawScene();
        }

        private void MouseWheelZoom(long delta)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                double currSize = Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
                double newSize = currSize + Math.Sign(delta) * Renderer.ZoomLevel / 50.0;
                if (newSize > 0)
                {
                    Global.AppSettings.LevelEditor.RenderingSettings.GridSize = newSize;
                    Renderer.SetGridSizeWithMouse(newSize, GetMouseCoordinates());
                }
            }
            else
            {
                Renderer.Zoom(GetMouseCoordinates(), delta > 0, 1 - MouseWheelStep / 100.0);
            }
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
                Renderer.RedrawScene();
            };
            rSettings.ShowDialog();
            AfterSettingsClosed(oldLgr);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenFileDialog1.InitialDirectory = Global.AppSettings.General.LevelDirectory;
            OpenFileDialog1.Multiselect = false;
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                OpenLevel(OpenFileDialog1.FileName);
        }

        private void PictureButtonChanged(object sender, EventArgs e)
        {
            if (PictureButton.Checked)
            {
                if (!_pictureToolAvailable)
                {
                    Utils.ShowError("You need to select LGR file from settings before you can use picture tool.",
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
                            mask = Renderer.DrawableImageFromName(_editorLgr.ListedImages
                                .Where(i => i.Type == Lgr.ImageType.Mask).First().Name);
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

                Modified = true;
                Renderer.RedrawScene();
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
                    Utils.ShowError("There are no levels in this directory!");
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
            for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
            {
                Polygon x = Lev.Polygons[i];
                if (!x.IsGrass)
                    Lev.Polygons.AddRange(((AutoGrassTool) (Tools[11])).AutoGrass(x));
            }

            Modified = true;
            Renderer.RedrawScene();
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
            Renderer.ZoomFill();
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
                    Utils.ShowError("Invalid format string!");
                }

                SaveFileDialog1.FileName = suggestion;
            }

            SaveFileDialog1.InitialDirectory = Global.AppSettings.General.LevelDirectory;
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _savePath = SaveFileDialog1.FileName;
                SaveLevel();
            }
        }

        private void SaveClicked(object sender = null, EventArgs e = null)
        {
            if (_savePath == null)
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
                    Lev.Save(_savePath);
                    _savedIndex = _historyIndex;
                    _fromScratch = false;
                    if (!Global.GetLevelFiles().Contains(_savePath))
                    {
                        Global.GetLevelFiles().Add(_savePath);
                        UpdateCurrLevDirFiles(force: true);
                    }

                    UpdateLabels();
                    UpdateButtons();
                    Modified = false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    Utils.ShowError("Error when saving level: " + ex.Message);
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

            Renderer.RedrawScene();
            UpdateSelectionInfo();
        }

        private void SelectButtonChanged(object sender, EventArgs e)
        {
            if (SelectButton.Checked)
                ChangeToolTo(0);
        }

        private void SendToBackToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_selectedObjectIndex >= 0)
            {
                var obj = Lev.Objects[_selectedObjectIndex];
                Lev.Objects.RemoveAt(_selectedObjectIndex);
                Lev.Objects.Insert(0, obj);
            }
            else if (_selectedPictureIndex >= 0)
            {
                var obj = Lev.Pictures[_selectedPictureIndex];
                Lev.Pictures.RemoveAt(_selectedPictureIndex);
                Lev.Pictures.Add(obj);
            }

            Modified = true;
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
            Renderer.RedrawScene();
        }

        private void SetupEventHandlers()
        {
            Resize += ViewerResized;
            EditorControl.Paint += Renderer.RedrawScene;
            ZoomFillButton.Click += Renderer.ZoomFill;
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
                if (files.All(filePath => File.Exists(filePath) && Constants.ImportableExtensions.Any(ext => Path.GetExtension(filePath).CompareWith(ext))))
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
                    .Where(s => s.EndsWith(Constants.LevExtension, StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(Constants.LebExtension, StringComparison.OrdinalIgnoreCase)).ToList();
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
            if (File.Exists(Global.AppSettings.LevelEditor.RenderingSettings.LgrFile) && Renderer.CurrentLgr != null)
            {
                _editorLgr = Renderer.CurrentLgr;
                _pictureToolAvailable = true;
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
                    image.Type == Lgr.ImageType.Texture))
                {
                    SkyComboBox.Items.Add(texture.Name);
                    GroundComboBox.Items.Add(texture.Name);
                }

                UpdateLabels();
            }
            else
            {
                _pictureToolAvailable = false;
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
                Renderer.ResetViewport(EditorControl.Width, EditorControl.Height);
                Renderer.RedrawScene();
            }
        }

        private void ZoomButtonChanged(object sender, EventArgs e)
        {
            if (ZoomButton.Checked)
                ChangeToolTo(5);
        }

        private void ZoomFillToolStripMenuItemClick(object sender, EventArgs e)
        {
            Renderer.ZoomFill();
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
            importFileDialog.InitialDirectory = Global.AppSettings.General.LevelDirectory;
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
                    catch (LevelException exception)
                    {
                        Utils.ShowError(
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
                    using (var converter = new FileSvgReader(settings))
                    {
                        var drawingGroup = converter.Read(file);
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
                            Utils.ShowError($"Failed to import SVG {file}. Invalid or animated SVGs are not supported.");
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
                }
                else
                {
                    try
                    {
                        lev = VectrastWrapper.LoadLevelFromImage(file);
                    }
                    catch (VectrastException ex)
                    {
                        Utils.ShowError(ex.Message);
                        return;
                    }
                }

                imported++;
                Lev.Import(lev);
            });
            if (imported > 0)
            {
                Modified = true;
                Renderer.UpdateZoomFillBounds();
                Renderer.ZoomFill();
            }
        }

        private void saveAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsPictureDialog.FileName = Lev.FileNameNoExt ?? "Untitled";
            if (saveAsPictureDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveAsPictureDialog.FileName.EndsWith(".png"))
                {
                    Renderer.SaveSnapShot(saveAsPictureDialog.FileName);
                }
                else if (saveAsPictureDialog.FileName.EndsWith(".svg"))
                {
                    var g = new SvgGraphics(Color.LightGray);
                    var pen = Pens.Black;
                    const int scale = 10;
                    var m = Matrix.CreateTranslation(-Lev.XMin + 1, -Lev.YMin + 1) * Matrix.CreateScaling(scale, scale);
                    var objOffset = new Vector(-0.4, -0.4);
                    const float oSize = (float)0.8 * scale;
                    Lev.Polygons.ForEach(p => g.DrawPolygon(pen, p
                        .ApplyTransformation(m)
                        .Vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray()));
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

                    var svgBody = g.WriteSVGString();
                    var width = (int) ((Lev.Width + 2) * scale);
                    var height = (int) ((Lev.Height + 2) * scale);
                    svgBody = svgBody.Replace("<svg ", $@"<svg width=""{width}"" height=""{height}"" ");
                    File.WriteAllText(saveAsPictureDialog.FileName, svgBody);
                }
                else
                {
                    Utils.ShowError("File type must be PNG or SVG.");
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

                Modified = true;
                return;
            }

            RemoveSelected();

            foreach (var selectedVertex in selectedVertices)
            {
                var obj = new LevObject(selectedVertex, objType, AppleType.Normal);
                Lev.Objects.Add(obj);
            }

            Modified = true;
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
                Utils.ShowError("The filename cannot be empty.");
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
                _savePath = newPath;
                UpdateLabels();
                filenameBox_TextChanged();
            }
            catch (ArgumentException)
            {
                Utils.ShowError("The filename is invalid.");
            }
            catch (IOException)
            {
                Utils.ShowError("A level with this name already exists.");
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
                SetModifiedAndRender();
            }
        }

        private void differenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PolyOpTool.PolyOpSelected(PolygonOperationType.Difference, Lev.Polygons))
            {
                SetModifiedAndRender();
            }
        }

        private void intersectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PolyOpTool.PolyOpSelected(PolygonOperationType.Intersection, Lev.Polygons))
            {
                SetModifiedAndRender();
            }
        }

        private void symmetricDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PolyOpTool.PolyOpSelected(PolygonOperationType.SymmetricDifference, Lev.Polygons))
            {
                SetModifiedAndRender();
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
            foreach (var o in Lev.Objects)
            {
                if (o.Type == ObjectType.Start)
                {
                    var oldPos = o.Position;
                    o.Position = _savedStartPosition.Clone();
                    if (oldPos != _savedStartPosition)
                    {
                        Modified = true;
                    }
                }
            }
        }

        private void MirrorVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lev.MirrorSelected(MirrorOption.Vertical);
            Modified = true;
            Renderer.RedrawScene();
        }

        private void MoveStartHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = Lev.Objects.Find(o => o.Type == ObjectType.Start);
            if (s != null && _contextMenuClickPosition != null)
            {
                s.Position = _contextMenuClickPosition;
                Modified = true;
            }
        }

        private void EditorMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _contextMenuClickPosition = GetMouseCoordinatesFixed();
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
    }
}