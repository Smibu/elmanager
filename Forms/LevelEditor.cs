using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.CustomControls;
using Elmanager.EditorTools;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.Forms
{
    partial class LevelEditor
    {
        //TODO Tool interface should be improved
        private const string CoordinateFormat = "F3";
        private const string LevEditorName = "SLE";
        private const int MouseWheelStep = 20;
        private readonly List<Level> _history = new List<Level>();
        private bool AppleFilter = true;
        private bool FlowerFilter = true;
        private bool GrassFilter = true;
        private bool GroundFilter = true;
        private bool KillerFilter = true;
        private bool PictureFilter = true;
        private bool TextureFilter = true;
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

        internal LevelEditor(string levPath)
        {
            InitializeComponent();
            TryLoadLevel(levPath);
            Initialize();
        }

        private void TryLoadLevel(string levPath)
        {
            try
            {
                Lev = new Level();
                Lev.LoadFromPath(levPath);
                _fromScratch = false;
            }
            catch (LevelException ex)
            {
                Utils.ShowError("Error occurred while parsing level file: " + ex.Message, "Warning", MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                Utils.ShowError("Error occurred when loading level file " + levPath + ". Exception text: " +
                                ex.Message);
                SetBlankLevel();
            }
        }

        internal LevelEditor()
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
            get { return _modified; }
            set { SetModified(value); }
        }

        internal bool EffectiveAppleFilter
        {
            get { return AppleFilter && (ShowObjectFramesButton.Checked || (ShowObjectsButton.Checked && _pictureToolAvailable)); }
        }

        internal bool EffectiveKillerFilter
        {
            get { return KillerFilter && (ShowObjectFramesButton.Checked || (ShowObjectsButton.Checked && _pictureToolAvailable)); }
        }

        internal bool EffectiveFlowerFilter
        {
            get { return FlowerFilter && (ShowObjectFramesButton.Checked || (ShowObjectsButton.Checked && _pictureToolAvailable)); }
        }

        internal bool EffectiveGrassFilter
        {
            get { return GrassFilter && (ShowGrassEdgesButton.Checked); }
        }

        internal bool EffectiveGroundFilter
        {
            get { return GroundFilter && (ShowGroundEdgesButton.Checked || (ShowGroundButton.Checked && _pictureToolAvailable)); }
        }

        internal bool EffectiveTextureFilter
        {
            get { return TextureFilter && (ShowTextureFramesButton.Checked || (ShowTexturesButton.Checked && _pictureToolAvailable)); }
        }

        internal bool EffectivePictureFilter
        {
            get { return PictureFilter && (ShowPictureFramesButton.Checked || (ShowPicturesButton.Checked && _pictureToolAvailable)); }
        }

        private int SelectedElementCount
        {
            get
            {
                return _selectedObjectCount + _selectedPictureCount + _selectedVerticeCount +
                       _selectedTextureCount;
            }
        }

        private ToolBase ToolBase
        {
            get { return ((ToolBase) CurrentTool); }
        }

        internal void TransformMenuItemClick(object sender = null, EventArgs e = null)
        {
            if (!CurrentTool.Busy)
            {
                ChangeToDefaultCursor();
                CurrentTool.InActivate();
                CurrentTool = Tools[12];
                CurrentTool.Activate();
                
                // if not busy, there's nothing to transform
                if (!CurrentTool.Busy)
                {
                    CurrentTool = Tools[0];
                    CurrentTool.Activate();   
                }
            }
        }

        internal void SetModified(bool value, bool updateHistory = true)
        {
            _modified = value;
            SaveButton.Enabled = value;
            SaveToolStripMenuItem.Enabled = value;
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
                    if (z.Mark == Geometry.VectorMark.Selected)
                    {
                        hasSelectedVertices = true;
                        _selectedVerticeCount++;
                    }
                }
                if (hasSelectedVertices)
                    _selectedPolygonCount++;
            }
            foreach (Level.Object x in Lev.Objects)
                if (x.Position.Mark == Geometry.VectorMark.Selected)
                    _selectedObjectCount++;
            foreach (Level.Picture x in Lev.Pictures)
                if (x.Position.Mark == Geometry.VectorMark.Selected)
                    if (x.IsPicture)
                        _selectedPictureCount++;
                    else
                        _selectedTextureCount++;
            SelectionLabel.Text = "Selected " + _selectedVerticeCount + " vertices of " + _selectedPolygonCount +
                                  " polygons, " + _selectedObjectCount + " objects, " + _selectedPictureCount +
                                  " pictures, " + _selectedTextureCount + " textures.";
            MirrorLevelToolStripMenuItem.Enabled = SelectedElementCount >= 2;
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

        private void ChangeToDefaultCursor()
        {
            if (EditorControl.Cursor == Cursors.Hand)
                EditorControl.Cursor = Cursors.Default;
        }

        private void ChangeToolTo(int index)
        {
            CurrentTool.InActivate();
            CurrentTool = Tools[index];
            CurrentTool.Activate();
        }

        private void CheckForPictureLoss()
        {
            if (!Lev.AllPicturesFound)
            {
                topologyList.Text = "Warning";
                topologyList.DropDownItems.Add("Some pictures or textures could not be found in the LGR file. " +
                                               "You will lose these pictures if you save this level.");
                topologyList.ForeColor = Color.DarkOrange;
                topologyList.Font = new Font(topologyList.Font, FontStyle.Bold);
            }
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
                    items.Add("Level is too wide. Current width: " + Lev.Width + ", maximum width: " + Level.MaximumSize);
                if (Lev.TooTall)
                    items.Add("Level is too tall. Current height: " + Lev.Height + ", maximum height: " +
                              Level.MaximumSize);
                if (Lev.HasTooLargePolygons)
                    items.Add("There are polygons with too many vertices in the level.");
                if (Lev.HasTooManyObjects)
                    items.Add("There are too many objects in the level. Current: " + Lev.Objects.Count + ", maximum: " +
                              Level.MaximumObjectCount);
                if (Lev.HasTooManyPolygons)
                    items.Add("There are too many polygons in the level. Current: " + Lev.Polygons.Count + ", maximum: " +
                              Level.MaximumPolygonCount);
                if (Lev.HasTooManyVertices)
                    items.Add("There are too many vertices in the level. Current: " + Lev.VertexCount + ", maximum: " +
                              Level.MaximumVertexCount);
                if (Lev.HeadTouchesGround)
                    items.Add("The driver\'s head is touching ground.");

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
                CurrentTool.InActivate();
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
            var copiedObjects = new List<Level.Object>();
            var copiedTextures = new List<Level.Picture>();
            Vector.MarkDefault = Geometry.VectorMark.Selected;
            foreach (Polygon x in Lev.Polygons)
            {
                var copy = new Polygon();
                foreach (Vector z in x.Vertices)
                {
                    if (z.Mark == Geometry.VectorMark.Selected)
                    {
                        z.Mark = Geometry.VectorMark.None;
                        copy.Add(new Vector(z.X + Global.AppSettings.LevelEditor.RenderingSettings.GridSize,
                            z.Y + Global.AppSettings.LevelEditor.RenderingSettings.GridSize));
                    }
                }
                if (copy.Count > 2)
                {
                    copiedPolygons.Add(copy);
                    copy.IsGrass = x.IsGrass;
                    copy.UpdateDecomposition();
                }
            }
            foreach (Level.Object x in Lev.Objects)
            {
                if (x.Position.Mark == Geometry.VectorMark.Selected && x.Type != Level.ObjectType.Start)
                {
                    x.Position.Mark = Geometry.VectorMark.None;
                    copiedObjects.Add(
                        new Level.Object(
                            x.Position +
                            new Vector(Global.AppSettings.LevelEditor.RenderingSettings.GridSize,
                                Global.AppSettings.LevelEditor.RenderingSettings.GridSize), x.Type, x.AppleType,
                            x.AnimationNumber));
                }
            }
            foreach (Level.Picture x in Lev.Pictures)
            {
                if (x.Position.Mark == Geometry.VectorMark.Selected)
                {
                    Level.Picture copiedPicture = x.Clone();
                    copiedPicture.Position.X += Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
                    copiedPicture.Position.Y += Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
                    copiedTextures.Add(copiedPicture);
                    x.Position.Mark = Geometry.VectorMark.None;
                }
            }
            Vector.MarkDefault = Geometry.VectorMark.None;
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

            Action<Vector, Color> drawAction = Global.AppSettings.LevelEditor.RenderingSettings.UseCirclesForVertices ?
                    (Action<Vector, Color>)((pt, color) => Renderer.DrawPoint(pt, color))
                    : ((pt, color) => Renderer.DrawEquilateralTriangle(pt, Renderer.ZoomLevel * Global.AppSettings.LevelEditor.RenderingSettings.VertexSize, color));

            foreach (Polygon x in Lev.Polygons)
            {
                switch (x.Mark)
                {
                    case PolygonMark.Highlight:
                        if (Global.AppSettings.LevelEditor.UseHighlight)
                            if (x.IsGrass)
                                Renderer.DrawLineStrip(x, Global.AppSettings.LevelEditor.HighlightColor);
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
                        case Geometry.VectorMark.Selected:
                            drawAction(z, Global.AppSettings.LevelEditor.SelectionColor);
                            break;
                        case Geometry.VectorMark.Highlight:
                            if (Global.AppSettings.LevelEditor.UseHighlight)
                                drawAction(z, Global.AppSettings.LevelEditor.HighlightColor);
                            break;
                    }
                }
            }
            foreach (Level.Object t in Lev.Objects)
            {
                Vector z = t.Position;
                switch (z.Mark)
                {
                    case Geometry.VectorMark.Selected:
                        drawAction(z, Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case Geometry.VectorMark.Highlight:
                        if (Global.AppSettings.LevelEditor.UseHighlight)
                            drawAction(z, Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
            }
            foreach (Level.Picture t in Lev.Pictures)
            {
                Vector z = t.Position;
                switch (z.Mark)
                {
                    case Geometry.VectorMark.Selected:
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y + t.Height,
                            Global.AppSettings.LevelEditor.SelectionColor);
                        break;
                    case Geometry.VectorMark.Highlight:
                        Renderer.DrawRectangle(z.X, z.Y, z.X + t.Width, z.Y + t.Height,
                            Global.AppSettings.LevelEditor.HighlightColor);
                        break;
                }
            }
            foreach (Vector x in _errorPoints)
                Renderer.DrawSquare(x, Renderer.ZoomLevel/25, Color.Red);
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
                        if (x.Vertices[i].Mark == Geometry.VectorMark.Selected &&
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
                    if (Lev.Objects[i].Position.Mark == Geometry.VectorMark.Selected)
                    {
                        if (Lev.Objects[i].Type != Level.ObjectType.Start &&
                            (Lev.Objects[i].Type != Level.ObjectType.Flower || Lev.ExitObjectCount > 1))
                        {
                            Lev.Objects.RemoveAt(i);
                            anythingDeleted = true;
                        }
                    }
                }
                for (int i = Lev.Pictures.Count - 1; i >= 0; i--)
                {
                    Level.Picture x = Lev.Pictures[i];
                    if (x.Position.Mark == Geometry.VectorMark.Selected)
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
                Renderer.RedrawScene();
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
            GroundFilter = GroundPolygonsToolStripMenuItem.Checked;
            GrassFilter = GrassPolygonsToolStripMenuItem.Checked;
            AppleFilter = ApplesToolStripMenuItem.Checked;
            KillerFilter = KillersToolStripMenuItem.Checked;
            FlowerFilter = FlowersToolStripMenuItem.Checked;
            PictureFilter = PicturesToolStripMenuItem.Checked;
            TextureFilter = TexturesToolStripMenuItem.Checked;
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
                    mousePosNoTr.X*(Renderer.XMax - Renderer.XMin)/EditorControl.Width,
                Y =
                    Renderer.YMax -
                    mousePosNoTr.Y*(Renderer.YMax - Renderer.YMin)/EditorControl.Height
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
                    mousePosNoTr.X*(Renderer.XMax - Renderer.XMin)/EditorControl.Width,
                Y =
                    -Renderer.YMax +
                    mousePosNoTr.Y*(Renderer.YMax - Renderer.YMin)/EditorControl.Height
            };
            return mousePos;
        }

        private void HandleGrassMenu(object sender, EventArgs e)
        {
            ToolBase.NearestPolygon.IsGrass = GrassMenuItem.Checked;
            Modified = true;
            ToolBase.NearestPolygon.UpdateDecomposition();
            Renderer.RedrawScene();
        }

        private void HandleGravityMenu(object sender, EventArgs e)
        {
            Level.Object currApple = Lev.Objects[_selectedObjectIndex];
            Level.AppleTypes chosenAppleType;
            if (sender.Equals(GravityNoneMenuItem))
                chosenAppleType = Level.AppleTypes.Normal;
            else if (sender.Equals(GravityUpMenuItem))
                chosenAppleType = Level.AppleTypes.GravityUp;
            else if (sender.Equals(GravityDownMenuItem))
                chosenAppleType = Level.AppleTypes.GravityDown;
            else if (sender.Equals(GravityLeftMenuItem))
                chosenAppleType = Level.AppleTypes.GravityLeft;
            else
                chosenAppleType = Level.AppleTypes.GravityRight;

            if (currApple.Position.Mark == Geometry.VectorMark.Selected)
            {
                Lev.Objects.Where(
                    obj => obj.Position.Mark == Geometry.VectorMark.Selected && obj.Type == Level.ObjectType.Apple)
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
            CurrentTool.Activate();
            SetupEventHandlers();
            _savePath = Lev.Path;
            if (Global.LevelFiles == null)
                Global.LevelFiles = Utils.GetLevelFiles();
        }

        private void InitializeLevel()
        {
            _savePath = Lev.Path;
            UpdateLabels();
            UpdateButtons();
            Renderer.InitializeLevel(Lev);
            UpdateLgrFromLev();
            Renderer.UpdateSettings(Global.AppSettings.LevelEditor.RenderingSettings);
            UpdateLgrTools();
            Renderer.RedrawScene();
            ClearHistory();
            Modified = false;
            CurrentTool.InActivate();
            CurrentTool.Activate();

            topologyList.Text = string.Empty;
            ResetTopologyListStyle();

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
            switch (e.KeyCode)
            {
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
            }
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

        private void LevelDropped(object sender, DragEventArgs e)
        {
            string filePath = ((Array) (e.Data.GetData(DataFormats.FileDrop))).GetValue(0).ToString();
            OpenLevel(filePath);
        }

        private void LevelPropertiesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var levelProperties = new LevelPropertiesForm(Lev);
            levelProperties.ShowDialog();
        }

        private void LevelPropertyModified(object sender, EventArgs e)
        {
            _modified = true;
            SaveButton.Enabled = true;
            SaveToolStripMenuItem.Enabled = true;
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
        }

        private void LoadFromHistory()
        {
            Lev = _history[_historyIndex].Clone();
            Renderer.Lev = Lev;
            Lev.DecomposeGroundPolygons();
            Renderer.UpdateZoomFillBounds();
            UpdateUndoRedo();
            UpdateSelectionInfo();
            topologyList.DropDownItems.Clear();
            topologyList.Text = "";
            _errorPoints.Clear();
            Renderer.RedrawScene();
            SetModified(_savedIndex != _historyIndex, false);
        }

        private void Mirror(object sender, EventArgs e)
        {
            Lev.MirrorSelected();
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
                            if (Lev.Objects[nearestObjectIndex].Type == Level.ObjectType.Apple)
                            {
                                GravityNoneMenuItem.Visible = true;
                                GravityUpMenuItem.Visible = true;
                                GravityDownMenuItem.Visible = true;
                                GravityLeftMenuItem.Visible = true;
                                GravityRightMenuItem.Visible = true;
                                switch (Lev.Objects[nearestObjectIndex].AppleType)
                                {
                                    case Level.AppleTypes.Normal:
                                        UpdateGravityMenu(GravityNoneMenuItem);
                                        break;
                                    case Level.AppleTypes.GravityUp:
                                        UpdateGravityMenu(GravityUpMenuItem);
                                        break;
                                    case Level.AppleTypes.GravityDown:
                                        UpdateGravityMenu(GravityDownMenuItem);
                                        break;
                                    case Level.AppleTypes.GravityLeft:
                                        UpdateGravityMenu(GravityLeftMenuItem);
                                        break;
                                    case Level.AppleTypes.GravityRight:
                                        UpdateGravityMenu(GravityRightMenuItem);
                                        break;
                                }
                            }
                        }
                        if (nearestVertexIndex >= -1)
                        {
                            GrassMenuItem.Checked = ToolBase.NearestPolygon.IsGrass;
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
        }

        private void MouseLeaveEvent(object sender, EventArgs e)
        {
            CurrentTool.MouseOutOfEditor();
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
                    Renderer.CenterX = (Renderer.XMax + Renderer.XMin)/2 - (z.X - _moveStartPosition.X);
                    Renderer.CenterY = (Renderer.YMax + Renderer.YMin)/2 - (z.Y - _moveStartPosition.Y);
                }
                Renderer.RedrawScene();
            }
            CurrentTool.MouseMove(GetMouseCoordinatesFixed());
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            CurrentTool.MouseUp(e);
            _draggingScreen = false;
            _draggingGrid = false;
        }

        private void MouseWheelZoom2(long delta)
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
            LGRBox.Focus();
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
            ComponentManager.ShowConfiguration(2);
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
                    Utils.ShowError("You need to select LGR file from settings before you can use picture tool.", "Note", MessageBoxIcon.Information);
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
            var selectedPics = Lev.Pictures.Where(p => p.Position.Mark == Geometry.VectorMark.Selected).ToList();
            if (selectedPics.Count > 0)
            {
                PicForm.AllowMultiple = true;
                PicForm.SelectMultiple(selectedPics);
            }
            else
            {
                PicForm.AllowMultiple = false;
                selectedPics = new List<Level.Picture> {Lev.Pictures[_selectedPictureIndex]};
                PicForm.SelectTexture(Lev.Pictures[_selectedPictureIndex]);    
            }
            
            PicForm.ShowDialog();
            if (PicForm.OkButtonPressed)
            {
                foreach (var selected in selectedPics)
                {
                    var clipping = PicForm.MultipleClippingSelected ? selected.Clipping : PicForm.Clipping;
                    var distance = PicForm.MultipleDistanceSelected ? selected.Distance : PicForm.Distance;
                    var mask = PicForm.MultipleMaskSelected ? Renderer.DrawableImageFromName(selected.Name)
                                                            : Renderer.DrawableImageFromName(PicForm.Mask.Name);
                    var position = selected.Position;
                    var texture = PicForm.MultipleTexturesSelected ? Renderer.DrawableImageFromName(selected.TextureName)
                                                            : Renderer.DrawableImageFromName(PicForm.Texture.Name);
                    var picture = PicForm.MultiplePicturesSelected ? Renderer.DrawableImageFromName(selected.Name)
                                                            : Renderer.DrawableImageFromName(PicForm.Picture.Name);

                    if ((PicForm.TextureSelected && !PicForm.MultipleTexturesSelected))
                    {
                        if (selected.IsPicture)
                        {
                            // need to set proper mask; otherwise the mask name will be picture name
                            mask = Renderer.DrawableImageFromName(_editorLgr.ListedImages.Where(i => i.Type == Lgr.ImageType.Mask).First().Name);
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
            string levDir = Path.GetDirectoryName(Lev.Path);
            if (CurrLevDirExists())
            {
                if (_currLevDirFiles == null || _loadedLevFilesDir != levDir)
                {
                    UpdateCurrLevDirFiles();
                }
                if (_currLevDirFiles.Count > 0)
                {
                    if (Lev.Path == null)
                        OpenLevel(_currLevDirFiles[0]);
                    else
                    {
                        int i = GetCurrentLevelIndex();
                        if (sender.Equals(PreviousButton) || sender.Equals(previousLevelToolStripMenuItem))
                        {
                            i--;
                            if (i < 0)
                                i = _currLevDirFiles.Count - 1;
                        }
                        else
                        {
                            i++;
                            if (i >= _currLevDirFiles.Count)
                                i = 0;
                        }
                        OpenLevel(_currLevDirFiles[i]);
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
                foreach (string levelFile in Global.LevelFiles)
                {
                    string x = Path.GetFileNameWithoutExtension(levelFile);
                    if (x.StartsWith(filenameStart, StringComparison.OrdinalIgnoreCase))
                    {
                        int levelNumber;
                        bool isNum = int.TryParse(x.Substring(filenameStart.Length), out levelNumber);
                        if (isNum)
                            highestNumber = Math.Max(highestNumber, levelNumber);
                    }
                }
                try
                {
                    suggestion = filenameStart +
                                 (highestNumber + 1).ToString(Global.AppSettings.LevelEditor.NumberFormat);
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
                    Modified = false;
                    _savedIndex = _historyIndex;
                    _fromScratch = false;
                    if (!Global.LevelFiles.Contains(_savePath))
                    {
                        Global.LevelFiles.Add(_savePath);
                        UpdateCurrLevDirFiles();
                    }
                    UpdateLabels();
                    UpdateButtons();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Utils.ShowError("Error when saving level: " + ex.Message);
                }
            }
            CurrentTool.Activate();
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (var polygon in Lev.Polygons)
            {
                if ((polygon.IsGrass && GrassFilter) || (!polygon.IsGrass && GroundFilter))
                    polygon.MarkVectorsAs(Geometry.VectorMark.Selected);
            }
            foreach (var levelObject in Lev.Objects)
            {
                switch (levelObject.Type)
                {
                    case Level.ObjectType.Apple:
                        if (AppleFilter)
                            levelObject.Position.Select();
                        break;
                    case Level.ObjectType.Killer:
                        if (KillerFilter)
                            levelObject.Position.Select();
                        break;
                    case Level.ObjectType.Flower:
                        if (FlowerFilter)
                            levelObject.Position.Select();
                        break;
                }
            }

            foreach (var texture in Lev.Pictures)
            {
                if ((TextureFilter && !texture.IsPicture) || (PictureFilter && texture.IsPicture))
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
            LGRBox.KeyPress += LevelPropertyModified;
            GroundComboBox.SelectedIndexChanged += LevelPropertyModified;
            SkyComboBox.SelectedIndexChanged += LevelPropertyModified;
            TitleBox.KeyPress += LevelPropertyModified;
            ToolPanel.MouseWheel += MouseWheelZoom2;
            previousLevelToolStripMenuItem.Click += PrevNextButtonClick;
            nextLevelToolStripMenuItem.Click += PrevNextButtonClick;
            foreach (var x in ToolStrip2.Items)
            {
                var button = x as ToolStripButton;
                if (button != null)
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
            string filePath = ((Array) (e.Data.GetData(DataFormats.FileDrop))).GetValue(0).ToString();
            if (File.Exists(filePath))
                if (Path.GetExtension(filePath).CompareWith(Constants.LevExtension))
                    e.Effect = DragDropEffects.Copy;
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

        private void UpdateCurrLevDirFiles()
        {
            string levDir = Path.GetDirectoryName(Lev.Path);
            _currLevDirFiles = Directory.GetFiles(levDir, "*.lev", SearchOption.TopDirectoryOnly).ToList();
            _loadedLevFilesDir = levDir;
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
            }
            else
            {
                Text = Lev.FileNameWithoutExtension + " - " + LevEditorName;
                filenameBox.Text = Lev.FileNameWithoutExtension;
                filenameBox.Enabled = true;
            }
            TitleBox.Text = Lev.Title;
            LGRBox.Text = Lev.LgrFile;
            GroundComboBox.Text = Lev.GroundTextureName;
            SkyComboBox.Text = Lev.SkyTextureName;
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
                foreach (Lgr.LgrImage x in _editorLgr.LgrImages)
                {
                    if (x.Type == Lgr.ImageType.Texture && x.Name[0] != 'q')
                    {
                        SkyComboBox.Items.Add(x.Name);
                        GroundComboBox.Items.Add(x.Name);
                    }
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
            TitleBox.Width = Math.Max(width, 120);
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
                importFileDialog.FileNames.ToList().ForEach(file =>
                {
                    Level lev;
                    if (file.EndsWith(".lev", StringComparison.InvariantCultureIgnoreCase))
                    {
                        lev = new Level();
                        try
                        {
                            lev.LoadFromPath(file);
                        }
                        catch (LevelException exception)
                        {
                            Utils.ShowError(
                                string.Format("Imported level {0} with errors: {1}", file, exception.Message),
                                "Warning",
                                MessageBoxIcon.Exclamation);
                        }
                        lev.UpdateImages(Renderer.DrawableImages);
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
                    Lev.Import(lev);
                });
                Modified = true;
                Renderer.UpdateZoomFillBounds();
                Renderer.ZoomFill();
            }
        }

        private void saveAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveAsPictureDialog.ShowDialog() == DialogResult.OK)
                ElmaRenderer.GetSnapShot(EditorControl.Width, EditorControl.Height).Save(saveAsPictureDialog.FileName,
                    ImageFormat.Png);
        }

        private void ConvertClicked(object sender, EventArgs e)
        {
            var selectedVertices = Lev.Polygons.SelectMany(p => p.Vertices.Where(v => v.Mark == Geometry.VectorMark.Selected)).ToList();
            selectedVertices.AddRange(
                Lev.Objects.Where(v => v.Position.Mark == Geometry.VectorMark.Selected && v.Type != Level.ObjectType.Start).Select(o => o.Position));
            selectedVertices.AddRange(
                Lev.Pictures.Where(v => v.Position.Mark == Geometry.VectorMark.Selected).Select(p => p.Position));

            Action removeSelected = () =>
            {
                var first = Lev.Polygons.First().Clone();
                Lev.Polygons.ForEach(p => p.Vertices.RemoveAll(v => v.Mark == Geometry.VectorMark.Selected));
                Lev.Polygons.RemoveAll(p => p.Count < 3);
                if (Lev.Polygons.Count == 0)
                {
                    Lev.Polygons.Add(first);
                }

                Lev.Objects.RemoveAll(
                    o => o.Position.Mark == Geometry.VectorMark.Selected && o.Type != Level.ObjectType.Start);
                Lev.Pictures.RemoveAll(p => p.Position.Mark == Geometry.VectorMark.Selected);
            };
            var objType = Level.ObjectType.Apple;
            if (sender.Equals(applesConvertItem))
            {
                // default
            }
            else if (sender.Equals(killersConvertItem))
            {
                objType = Level.ObjectType.Killer;
            }
            else if (sender.Equals(flowersConvertItem))
            {
                objType = Level.ObjectType.Flower;
            }
            else
            {
                // handle picture
                PicForm.AllowMultiple = false;
                PicForm.ShowDialog();
                if (PicForm.OkButtonPressed)
                {
                    removeSelected();
                    foreach (var selectedVertex in selectedVertices)
                    {
                        if (PicForm.TextureSelected)
                        {
                            Lev.Pictures.Add(new Level.Picture(PicForm.Clipping, PicForm.Distance,
                                selectedVertex,
                                Renderer.DrawableImageFromName(PicForm.Texture.Name),
                                Renderer.DrawableImageFromName(PicForm.Mask.Name)));
                        }
                        else
                        {
                            Lev.Pictures.Add(new Level.Picture(Renderer.DrawableImageFromName(PicForm.Picture.Name),
                                selectedVertex, PicForm.Distance,
                                PicForm.Clipping));
                        }
                    }
                }
                Modified = true;
                return;
            }

            removeSelected();

            foreach (var selectedVertex in selectedVertices)
            {
                var obj = new Level.Object(selectedVertex, objType, Level.AppleTypes.Normal);
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
            if (MessageBox.Show("Are you sure you want to delete this level?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                File.Delete(Lev.Path);
                int levIndex = GetCurrentLevelIndex();
                _currLevDirFiles.RemoveAt(levIndex);
                if (levIndex < _currLevDirFiles.Count)
                {
                    OpenLevel(_currLevDirFiles[levIndex]);
                }
                else
                {
                    NewLevel();
                }
            }
        }

        private int GetCurrentLevelIndex()
        {
            return _currLevDirFiles.FindIndex(
                        path => string.Compare(path, Lev.Path, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteCurrentLevel();
        }

        private void filenameBox_TextChanged(object sender = null, EventArgs e = null)
        {
            bool showButtons = string.Compare(filenameBox.Text,
                Lev.FileNameWithoutExtension,
                StringComparison.InvariantCulture) != 0 && Lev.Path != null;
            filenameOkButton.Visible = showButtons;
            filenameCancelButton.Visible = showButtons;
        }

        private void filenameCancelButton_Click(object sender = null, EventArgs e = null)
        {
            filenameBox.Text = Lev.FileNameWithoutExtension;
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
                File.Move(Lev.Path, newPath);
                if (_currLevDirFiles != null)
                {
                    int index = GetCurrentLevelIndex();
                    _currLevDirFiles[index] = newPath;
                }
                Lev.Path = newPath;
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
    }
}