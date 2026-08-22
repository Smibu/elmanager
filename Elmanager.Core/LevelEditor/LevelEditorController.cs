using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Playing;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using Envelope = NetTopologySuite.Geometries.Envelope;

namespace Elmanager.LevelEditor;

public class LevelEditorController<TEditorLev>(ILevelEditor levelEditor, TEditorLev lev) where TEditorLev : IEditorLev
{
    private readonly List<Level> _history = new();
    private TEditorLev _editorLev = lev;
    private int _historyIndex;
    private int _savedIndex;
    private bool _modified;
    private List<Vector> _errorPoints = new();
    private Vector? _savedStartPosition;
    private ToolBase.NearestVertexInfo? _grassInfo;

    private bool _lockMouseX;
    private bool _lockMouseY;
    private int _lockCoord;

    private int _selectedObjectCount;
    private int _selectedObjectIndex;
    private int _selectedPictureCount;
    private int _selectedPictureIndex;
    private int _selectedPolygonCount;
    private int _selectedVerticeCount;
    private int _selectedTextureCount;

    public Level Lev => _editorLev.Lev;
    public TEditorLev EditorLev => _editorLev;
    public bool Modified => _modified;
    public List<Vector> ErrorPoints => _errorPoints;
    public Vector? SavedStartPosition => _savedStartPosition;
    public ToolBase.NearestVertexInfo? GrassInfo { get => _grassInfo; set => _grassInfo = value; }
    public int SelectedObjectIndex { get => _selectedObjectIndex; set => _selectedObjectIndex = value; }
    public int SelectedPictureIndex { get => _selectedPictureIndex; set => _selectedPictureIndex = value; }
    public int SelectedPolygonCount => _selectedPolygonCount;

    public int SelectedElementCount => _selectedObjectCount + _selectedPictureCount + _selectedVerticeCount +
                                       _selectedTextureCount;

    public bool CanUndo => _historyIndex > 0;
    public bool CanRedo => _historyIndex < _history.Count - 1;
    private TexturizationOptions? _texturizationOpts;

    public void SetEditorLev(TEditorLev lev)
    {
        ClearHighlight();
        _editorLev = lev;
    }

    private void ClearHighlight()
    {
        levelEditor.CurrentHighlight = null;
        levelEditor.HighlightText = string.Empty;
    }

    private void ReplaceLevel(Level lev)
    {
        ClearHighlight();
        _editorLev = (TEditorLev)_editorLev.WithLev(lev);
    }

    public void SetNotModified()
    {
        _modified = false;
    }

    public void ClearSavedStartPosition()
    {
        _savedStartPosition = null;
    }

    public void SetModified(LevModification value, ElmaRenderer renderer, IEditorTool currentTool,
        PlayController playController, LevelEditorSettings settings, bool updateHistory = true)
    {
        var wasModified = value != LevModification.Nothing;
        _modified = wasModified || _modified;
        if (wasModified)
        {
            Lev.UpdateBounds();
            if (updateHistory)
                AddToHistory();
            if (settings.CheckTopologyDynamically)
                CheckTopology(currentTool);
        }

        if (value.HasFlag(LevModification.Ground) || value.HasFlag(LevModification.Apples) ||
            value.HasFlag(LevModification.Killers) || value.HasFlag(LevModification.Flowers))
        {
            playController.UpdateEngine(Lev);
        }
    }

    public void UpdateSelectionInfo()
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
                if (x is GraphicElement.Picture or GraphicElement.MissingPicture)
                    _selectedPictureCount++;
                else
                    _selectedTextureCount++;
    }

    public string GetSelectionText()
    {
        return "Selected " + _selectedVerticeCount + " vertices of " + _selectedPolygonCount +
               " polygons, " + _selectedObjectCount + " objects, " + _selectedPictureCount +
               " pictures, " + _selectedTextureCount + " textures.";
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
        levelEditor.UpdateUndoRedo();
    }

    public void ClearHistory()
    {
        _history.Clear();
        _history.Add(Lev.Clone());
        _historyIndex = 0;
        _savedIndex = -1;
    }

    public bool Undo()
    {
        if (_historyIndex > 0)
        {
            _historyIndex--;
            return true;
        }

        return false;
    }

    public bool Redo()
    {
        if (_historyIndex < _history.Count - 1)
        {
            _historyIndex++;
            return true;
        }

        return false;
    }

    public void LoadFromHistory(LevelEditorRenderingSettings renderingSettings)
    {
        ReplaceLevel(_history[_historyIndex].Clone());
        Lev.UpdateGrass(renderingSettings.GrassZoom);
        _errorPoints.Clear();
        if (_savedIndex == _historyIndex)
        {
            _modified = false;
        }
    }

    public void MarkSaved()
    {
        _savedIndex = _historyIndex;
    }

    public bool IsSaved => _savedIndex == _historyIndex;

    public void PreserveSelection()
    {
        _history[_historyIndex] = Lev.Clone();
    }

    public void UpdateLevel(Level lev)
    {
        ReplaceLevel(lev);
        _savedIndex = _historyIndex;
    }

    public void DeleteSelected(IEditorTool currentTool)
    {
        var editor = levelEditor;
        if (currentTool.Busy)
        {
            return;
        }
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
                    mod |= x.IsGrass ? LevModification.Grass : LevModification.Ground;
                    polyModified = true;
                }
            }

            if (x.Vertices.Count < 3)
                Lev.Polygons.Remove(x);
            else if (polyModified)
                x.UpdateGrassSlopeInfo(Lev.GroundBounds, editor.Settings.RenderingSettings.GrassZoom);
        }

        var deletedApples = new HashSet<int>();
        for (int i = Lev.Objects.Count - 1; i >= 0; i--)
        {
            if (Lev.Objects[i].Position.Mark == VectorMark.Selected)
            {
                if (Lev.Objects[i].Type == ObjectType.Start)
                    continue;
                mod |= Lev.Objects[i].Type switch
                {
                    ObjectType.Apple => LevModification.Apples,
                    ObjectType.Killer => LevModification.Killers,
                    ObjectType.Flower => LevModification.Flowers,
                    _ => LevModification.Nothing
                };
                if (Lev.Objects[i].Type == ObjectType.Apple)
                    deletedApples.Add(i);
                Lev.Objects.RemoveAt(i);
            }
        }

        for (int i = Lev.GraphicElements.Count - 1; i >= 0; i--)
        {
            GraphicElement x = Lev.GraphicElements[i];
            if (x.Position.Mark == VectorMark.Selected)
            {
                Lev.GraphicElements.Remove(x);
                mod |= x is GraphicElement.Picture or GraphicElement.MissingPicture
                    ? LevModification.Pictures
                    : LevModification.Textures;
            }
        }

        editor.PlayController.NotifyDeletedApples(deletedApples);

        editor.SetModified(mod);
        editor.UpdateSelectionInfo();
    }

    public void CopySelected(ZoomController zoomCtrl)
    {
        var editor = levelEditor;
        var delta = editor.KeyboardState.IsKeyDown(ModifierKey.LeftShift)
            ? editor.Settings.RenderingSettings.GridSize
            : zoomCtrl.Cam.ZoomLevel * 0.1;
        var copiedPolygons = new List<Polygon>();
        var copiedObjects = new List<LevObject>();
        var copiedTextures = new List<GraphicElement>();
        Vector.MarkDefault = VectorMark.Selected;
        foreach (Polygon x in Lev.Polygons)
        {
            var copy = new Polygon();
            for (var index = 0; index < x.Vertices.Count; index++)
            {
                Vector z = x.Vertices[index];
                if (z.Mark == VectorMark.Selected)
                {
                    x.Vertices[index] = new Vector(z.X, z.Y, VectorMark.None);
                    copy.Add(new Vector(z.X + delta, z.Y - delta));
                }
            }

            if (copy.Vertices.Count > 2)
            {
                copiedPolygons.Add(copy);
                copy.IsGrass = x.IsGrass;
                copy.UpdateGrassSlopeInfo(Lev.GroundBounds, editor.Settings.RenderingSettings.GrassZoom);
            }
        }

        foreach (LevObject x in Lev.Objects)
        {
            if (x.Mark == VectorMark.Selected && x.Type != ObjectType.Start)
            {
                x.Mark = VectorMark.None;
                copiedObjects.Add(
                    new LevObject(
                        x.Position + new Vector(delta, -delta), x.Type, x.AppleType,
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
        foreach (var obj in copiedObjects)
        {
            mod |= obj.Type switch
            {
                ObjectType.Apple => LevModification.Apples,
                ObjectType.Killer => LevModification.Killers,
                ObjectType.Flower => LevModification.Flowers,
                _ => LevModification.Nothing
            };
        }

        if (copiedPolygons.Count > 0)
            mod |= LevModification.Ground;
        foreach (var tex in copiedTextures)
            mod |= tex is GraphicElement.Picture or GraphicElement.MissingPicture
                ? LevModification.Pictures
                : LevModification.Textures;
        editor.SetModified(mod);
        editor.RedrawScene();
    }

    public List<string> CheckTopologyErrors(IEditorTool currentTool)
    {
        var items = new List<string>();
        if (currentTool.Busy)
            return items;

        _errorPoints.Clear();
        if (Lev.TooWide)
            items.Add("Level is too wide. Current width: " + Lev.Width + ", maximum width: " + Level.MaximumSize);
        if (Lev.TooTall)
            items.Add("Level is too tall. Current height: " + Lev.Height + ", maximum height: " + Level.MaximumSize);
        if (Lev.HasTooLargePolygons)
            items.Add("There are polygons with too many vertices in the level.");
        if (Lev.HasTooManyObjects)
            items.Add("There are too many objects in the level. Current: " + Lev.Objects.Count + ", maximum: " +
                      Level.MaximumObjectCount);
        if (Lev.HasTooFewObjects)
            items.Add("There must be at least one object in the level (in addition to the start object).");
        if (Lev.HasTooManyPolygons)
            items.Add("There are too many polygons in the level. Current: " + Lev.Polygons.Count + ", maximum: " +
                      Level.MaximumPolygonCount);
        if (Lev.HasTooManyVertices)
            items.Add("There are too many ground vertices in the level. Current: " + Lev.GroundVertexCount +
                      ", maximum: " + Level.MaximumGroundVertexCount);
        if (Lev.HasTooManyPictures)
            items.Add("There are too many pictures and textures in the level. Current: " +
                      Lev.PictureTextureCount + ", maximum: " + Level.MaximumPictureTextureCount);
        if (Lev.HeadTouchesGround)
            items.Add("The driver's head is touching ground.");
        if (Lev.WheelLiesOnEdge)
            items.Add("The driver's wheel is lying on an edge.");
        if (Lev.HasTexturesOutOfBounds)
            items.Add("Some textures are too far outside of the level polygons.");

        _errorPoints = Lev.GetIntersectionPoints();
        if (_errorPoints.Count > 0)
            items.Add("There are intersections in the level.");

        var errorPositions = Lev.GetApplesAndFlowersInsideGround();
        if (errorPositions.Count > 0)
        {
            _errorPoints.AddRange(errorPositions);
            items.Add("Some apples and/or flowers are inside ground.");
        }

        var shortEdges = Lev.GetTooShortEdges();
        if (shortEdges.Count > 0)
        {
            _errorPoints.AddRange(shortEdges);
            items.Add("Some polygon edges are too short.");
        }

        var missingNames = Lev.GraphicElements.Select(e => e switch
        {
            GraphicElement.MissingPicture missingPicture => missingPicture.Name,
            GraphicElement.MissingTexture missingTexture => missingTexture.TextureName,
            _ => null
        }).Where(name => name != null).Distinct().ToList();
        if (missingNames.Any())
        {
            items.Add($"Level has pictures that the LGR is missing: {string.Join(", ", missingNames)}");
        }

        return items;
    }

    private void CheckTopology(IEditorTool currentTool)
    {
        CheckTopologyErrors(currentTool);
    }

    public void BringToFront()
    {
        var editor = levelEditor;
        var mod = LevModification.Nothing;
        if (_selectedObjectIndex >= 0)
        {
            var obj = Lev.Objects[_selectedObjectIndex];
            Lev.Objects.RemoveAt(_selectedObjectIndex);
            Lev.Objects.Add(obj);
            mod = obj.Type switch
            {
                ObjectType.Apple => LevModification.Apples,
                ObjectType.Killer => LevModification.Killers,
                ObjectType.Flower => LevModification.Flowers,
                _ => LevModification.Nothing
            };
        }
        else if (_selectedPictureIndex >= 0)
        {
            var obj = Lev.GraphicElements[_selectedPictureIndex];
            Lev.GraphicElements.RemoveAt(_selectedPictureIndex);
            Lev.GraphicElements.Insert(0, obj);
            mod = obj is GraphicElement.Picture or GraphicElement.MissingPicture
                ? LevModification.Pictures
                : LevModification.Textures;
        }
        else if (_grassInfo is not null)
        {
            Lev.Polygons.Remove(_grassInfo.Polygon);
            Lev.Polygons.Insert(0, _grassInfo.Polygon);
        }

        editor.SetModified(mod);
    }

    public void SendToBack()
    {
        var editor = levelEditor;
        var mod = LevModification.Nothing;
        if (_selectedObjectIndex >= 0)
        {
            var obj = Lev.Objects[_selectedObjectIndex];
            Lev.Objects.RemoveAt(_selectedObjectIndex);
            Lev.Objects.Insert(0, obj);
            mod |= obj.Type switch
            {
                ObjectType.Apple => LevModification.Apples,
                ObjectType.Killer => LevModification.Killers,
                ObjectType.Flower => LevModification.Flowers,
                _ => LevModification.Nothing
            };
        }
        else if (_selectedPictureIndex >= 0)
        {
            var obj = Lev.GraphicElements[_selectedPictureIndex];
            Lev.GraphicElements.RemoveAt(_selectedPictureIndex);
            Lev.GraphicElements.Add(obj);
            mod |= obj is GraphicElement.Picture or GraphicElement.MissingPicture
                ? LevModification.Pictures
                : LevModification.Textures;
        }
        else if (_grassInfo is not null)
        {
            Lev.Polygons.Remove(_grassInfo.Polygon);
            Lev.Polygons.Add(_grassInfo.Polygon);
        }

        editor.SetModified(mod);
    }

    public void ToggleGrass()
    {
        var editor = levelEditor;
        var polys = GetPolygonsForGrassToggle();
        var mod = LevModification.Nothing;
        polys.ForEach(p =>
        {
            p.IsGrass = !p.IsGrass;
            mod |= LevModification.Ground | LevModification.Grass;
            p.UpdateGrassSlopeInfo(Lev.GroundBounds, editor.Settings.RenderingSettings.GrassZoom);
        });
        editor.SetModified(mod);
        editor.RedrawScene();
    }

    public bool WouldToggleGrassLeaveNoGroundPolygons()
    {
        int groundPolygonCount = Lev.GroundPolygonCount;
        foreach (var polygon in GetPolygonsForGrassToggle())
            groundPolygonCount += polygon.IsGrass ? 1 : -1;

        return groundPolygonCount <= 0;
    }

    private List<Polygon> GetPolygonsForGrassToggle()
    {
        var polys = new List<Polygon>();
        var selectedPolygons = Lev.Polygons.GetSelectedPolygons(includeGrass: true).ToList();
        if (_grassInfo is not null)
        {
            if (selectedPolygons.Contains(_grassInfo.Polygon))
            {
                polys.AddRange(selectedPolygons);
            }
            else
            {
                polys.Add(_grassInfo.Polygon);
            }
        }
        else
        {
            polys.AddRange(selectedPolygons);
        }

        return polys;
    }

    public void HandleGravity(AppleType chosenAppleType, PlayController playController)
    {
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
        }
        else
        {
            playController.UpdateGravity(chosenAppleType);
        }
        if (SelectedObjectIndex >= 0)
        {
            levelEditor.SetModified(LevModification.Apples);
        }
    }

    public void SelectAll()
    {
        var editor = levelEditor;
        var filter = editor.SelectionFilter;
        foreach (var polygon in Lev.Polygons)
        {
            if ((polygon.IsGrass && filter.EffectiveGrassFilter) ||
                (!polygon.IsGrass && filter.EffectiveGroundFilter))
                polygon.MarkVectorsAs(VectorMark.Selected);
        }

        foreach (var levelObject in Lev.Objects)
        {
            switch (levelObject.Type)
            {
                case ObjectType.Apple:
                    if (filter.EffectiveAppleFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
                case ObjectType.Killer:
                    if (filter.EffectiveKillerFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
                case ObjectType.Flower:
                    if (filter.EffectiveFlowerFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
                case ObjectType.Start:
                    if (filter.EffectiveStartFilter)
                        levelObject.Mark = VectorMark.Selected;
                    break;
            }
        }

        foreach (var ge in Lev.GraphicElements)
        {
            if ((filter.EffectiveTextureFilter && ge is GraphicElement.Texture) ||
                (filter.EffectivePictureFilter && ge is GraphicElement.Picture))
                ge.Mark = VectorMark.Selected;
        }

        editor.RedrawScene();
        editor.UpdateSelectionInfo();
    }

    public void MirrorSelected(MirrorOption option)
    {
        var editor = levelEditor;
        Lev.MirrorSelected(option);
        Lev.UpdateGrass(editor.Settings.RenderingSettings.GrassZoom);
        editor.SetModified(LevModification.All);
        editor.RedrawScene();
    }

    public void SaveStartPosition(Level level)
    {
        foreach (var o in level.Objects)
        {
            if (o.Type == ObjectType.Start)
            {
                _savedStartPosition = o.Position.Clone();
            }
        }
    }

    public LevModification RestoreStartPosition()
    {
        if (_savedStartPosition is not { } p)
            return LevModification.Nothing;

        var mod = LevModification.Nothing;
        foreach (var o in Lev.Objects)
        {
            if (o.Type == ObjectType.Start)
            {
                var oldPos = o.Position;
                o.Position = p.Clone();
                if (!Equals(oldPos, _savedStartPosition))
                {
                    mod = LevModification.Objects;
                }
            }
        }

        return mod;
    }

    public void SaveStartPositionIfEnabled(ElmaFileObject<Level> lev)
    {
        if (levelEditor.Settings.EnableStartPositionFeature)
        {
            SaveStartPosition(lev.Obj);
        }
    }

    public void DeleteAllGrass()
    {
        var editor = levelEditor;
        var mod = LevModification.Nothing;
        for (int i = Lev.Polygons.Count - 1; i >= 0; i--)
        {
            Polygon x = Lev.Polygons[i];
            if (x.IsGrass)
            {
                mod = LevModification.Grass;
                Lev.Polygons.Remove(x);
            }
        }

        editor.SetModified(mod);
        editor.RedrawScene();
    }

    public void QuickGrass(AutoGrassTool autoGrassTool)
    {
        var editor = levelEditor;
        var grassPolys = Lev.Polygons.Where(x => !x.IsGrass)
            .SelectMany(autoGrassTool.AutoGrass).ToList();
        Lev.Polygons.AddRange(grassPolys);
        var mod = grassPolys.Count > 0 ? LevModification.Grass : LevModification.Nothing;
        editor.SetModified(mod);
        editor.RedrawScene();
    }

    public void DeselectPolygonsWith(Func<Polygon, bool> cond)
    {
        foreach (var polygon in Lev.Polygons.Where(cond))
        {
            polygon.Vertices = polygon.Vertices.Select(v => v with { Mark = VectorMark.None }).ToList();
        }
    }

    public void DeselectObjectsWith(Func<LevObject, bool> cond)
    {
        foreach (var obj in Lev.Objects.Where(cond))
        {
            obj.Mark = VectorMark.None;
        }
    }

    public void DeselectGraphicElementsWith(Func<GraphicElement, bool> cond)
    {
        foreach (var elem in Lev.GraphicElements.Where(cond))
        {
            elem.Mark = VectorMark.None;
        }
    }

    public LevModification MoveStartHere(Vector position)
    {
        var s = Lev.Objects.Find(o => o.Type == ObjectType.Start);
        if (s != null)
        {
            s.Position = position;
            return LevModification.Start;
        }

        return LevModification.Nothing;
    }

    public static Vector ScreenToWorld(double screenX, double screenY, double viewWidth, double viewHeight, Bounds bounds)
    {
        return new Vector
        {
            X = bounds.XMin + screenX * bounds.XSize / viewWidth,
            Y = bounds.YMax - screenY * bounds.YSize / viewHeight
        };
    }

    public bool LockMouseX => _lockMouseX;
    public bool LockMouseY => _lockMouseY;
    public int LockCoord => _lockCoord;

    public void SetLockMouseX(bool value, int coord = 0)
    {
        _lockMouseX = value;
        if (value) _lockCoord = coord;
    }

    public void SetLockMouseY(bool value, int coord = 0)
    {
        _lockMouseY = value;
        if (value) _lockCoord = coord;
    }

    private const int MouseWheelStep = 20;

    public void MouseWheelZoom(long delta, Vector mouseCoords, ZoomController zoomCtrl,
        SceneSettings sceneSettings, LevelEditorSettings settings, ElmaRenderer renderer)
    {
        if (!settings.LockGrid && levelEditor.KeyboardState.IsKeyDown(ModifierKey.LeftCtrl))
        {
            double currSize = settings.RenderingSettings.GridSize;
            double newSize = currSize + Math.Sign(delta) * zoomCtrl.Cam.ZoomLevel / 50.0;
            if (newSize > 0)
            {
                SetGridSizeWithMouse(newSize, mouseCoords, zoomCtrl, sceneSettings, settings, renderer);
            }
        }
        else
        {
            zoomCtrl.Zoom(mouseCoords, delta > 0, 1 - MouseWheelStep / 100.0, settings.RenderingSettings);
        }
    }

    private void SetGridSizeWithMouse(double newSize, Vector mouseCoords, ZoomController zoomCtrl,
        SceneSettings sceneSettings, LevelEditorSettings settings, ElmaRenderer renderer)
    {
        var renderSettings = settings.RenderingSettings;
        var bounds = zoomCtrl.Cam.GetBounds(renderer.AspectRatio);
        SetGridSizeAtMouse(newSize, mouseCoords, sceneSettings, renderSettings, bounds);
        levelEditor.SignalRenderingSettingsChange();
    }

    private static double GetGridMouseRatio(double size, double offset, double min, double mouse)
    {
        var dist = mouse - ElmaRenderer.GetFirstGridLine(size, offset, min);
        return (dist % size) / size;
    }

    private static void SetGridSizeAtMouse(double newSize, Vector mouseCoords, SceneSettings sceneSettings,
        LevelEditorRenderingSettings settings, Bounds bounds)
    {
        var gx = sceneSettings.GridOffset.X;
        sceneSettings.GridOffset.X = (gx + ElmaRenderer.GetFirstGridLine(newSize, gx, bounds.XMin)
            - mouseCoords.X + GetGridMouseRatio(settings.GridSize, gx, bounds.XMin, mouseCoords.X) *
            newSize) % newSize;
        var gy = sceneSettings.GridOffset.Y;
        sceneSettings.GridOffset.Y = (gy + ElmaRenderer.GetFirstGridLine(newSize, gy, bounds.YMin)
            - mouseCoords.Y + GetGridMouseRatio(settings.GridSize, gy, bounds.YMin, mouseCoords.Y) *
            newSize) % newSize;
        settings.GridSize = newSize;
    }

    public void HandleDragMove(Vector currentMouse, Vector moveStartPosition, bool draggingGrid,
        SceneSettings sceneSettings, Vector gridStartOffset, bool lockGrid, ZoomController zoomCtrl)
    {
        if (!lockGrid && draggingGrid)
        {
            sceneSettings.GridOffset = gridStartOffset + moveStartPosition - currentMouse;
        }
        else
        {
            zoomCtrl.CenterX = zoomCtrl.CenterX - (currentMouse.X - moveStartPosition.X);
            zoomCtrl.CenterY = zoomCtrl.CenterY - (currentMouse.Y - moveStartPosition.Y);
        }
    }

    public string GetFilenameSuggestion(IEnumerable<string> levelFiles, string baseFilename, string numberFormat)
    {
        int highestNumber = 0;
        int lowestNumber = int.MaxValue;
        foreach (string levelFile in levelFiles)
        {
            string x = System.IO.Path.GetFileNameWithoutExtension(levelFile);
            if (x.StartsWith(baseFilename, StringComparison.OrdinalIgnoreCase))
            {
                bool isNum = int.TryParse(x.Substring(baseFilename.Length), out var levelNumber);
                if (isNum)
                {
                    highestNumber = Math.Max(highestNumber, levelNumber);
                    lowestNumber = Math.Min(lowestNumber, levelNumber);
                }
            }
        }

        int newNumber;
        if (highestNumber == 0 || lowestNumber <= 1)
        {
            newNumber = highestNumber + 1;
        }
        else
        {
            newNumber = lowestNumber - 1;
        }

        return baseFilename + newNumber.ToString(numberFormat);
    }

    private List<Vector> GetSelectedVertices()
    {
        var selectedVertices = Lev.Polygons
            .SelectMany(p => p.Vertices.Where(v => v.Mark == VectorMark.Selected)).ToList();
        selectedVertices.AddRange(
            Lev.Objects.Where(v =>
                    v.Position.Mark == VectorMark.Selected && v.Type != ObjectType.Start)
                .Select(o => o.Position));
        selectedVertices.AddRange(
            Lev.GraphicElements.Where(v => v.Position.Mark == VectorMark.Selected).Select(p => p.Position));
        return selectedVertices;
    }

    private void RemoveSelected()
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

    private void ConvertSelectedToObjects(List<Vector> selectedVertices, ObjectType objType)
    {
        RemoveSelected();
        foreach (var selectedVertex in selectedVertices)
        {
            var obj = new LevObject(selectedVertex, objType, AppleType.Normal);
            Lev.Objects.Add(obj);
        }
    }

    private void ApplyPictureProperties(ImageSelection sel, List<GraphicElement> selectedElems, OpenGlLgr openGlLgr)
    {
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
                    openGlLgr.DrawableImageFromLgrImage(pic), position, distance, clipping),
                ImageSelection.TextureSelection(var txt, var mask, _, _) => GraphicElement.Text(clipping, distance,
                    position,
                    openGlLgr.DrawableImageFromLgrImage(txt),
                    openGlLgr.DrawableImageFromLgrImage(mask)),
                ImageSelection.TextureSelectionMultipleMasks(var txt, _, _) when
                    curr is GraphicElement.Texture t =>
                    GraphicElement.Text(clipping, distance, position, openGlLgr.DrawableImageFromLgrImage(txt), t.MaskInfo),
                ImageSelection.TextureSelectionMultipleMasks(var txt, _, _) when curr is GraphicElement.Picture
                    =>
                    GraphicElement.Text(clipping, distance, position, openGlLgr.DrawableImageFromLgrImage(txt),
                        openGlLgr.DrawableImageFromLgrImage(openGlLgr.CurrentLgr.LgrImages.Values.First(i => i.Type == Lgr.ImageType.Mask))),
                ImageSelection.TextureSelectionMultipleTextures(var mask, _, _) when
                    curr is GraphicElement.Texture t => GraphicElement.Text(clipping,
                        distance, position, t.TextureInfo, openGlLgr.DrawableImageFromLgrImage(mask)),
                ImageSelection.TextureSelectionMultipleTextures when
                    curr is GraphicElement.Picture => curr with { Distance = distance, Clipping = clipping },
                _ => throw new ArgumentOutOfRangeException(nameof(sel))
            };
        }).ToList();
    }

    public void DrawEditorScene(ElmaRenderer renderer, ElmaCamera cam, SceneSettings sceneSettings,
        LevelEditorSettings settings, PlayController playController,
        IEditorTool currentTool, HighlightTarget? currentHighlight,
        int viewWidth, int viewHeight, Func<Vector> getMouseCoordinates)
    {
        var jf = playController.Playing && playController.FollowDriver
            ? cam.FixJitter(viewWidth, viewHeight)
            : new Vector();
        renderer.DrawScene(cam, 0, sceneSettings);

        if (settings.ShowCrossHair)
        {
            var bounds = cam.GetBounds(renderer.AspectRatio);
            var mouse = getMouseCoordinates();
            renderer.DrawDashLine(bounds.XMin, mouse.Y, bounds.XMax,
                mouse.Y, settings.RenderingSettings.CrosshairColor);
            renderer.DrawDashLine(mouse.X, bounds.YMin, mouse.X,
                bounds.YMax, settings.RenderingSettings.CrosshairColor);
        }

        foreach (Polygon x in Lev.Polygons)
        {
            switch (x.Mark)
            {
                case PolygonMark.Selected:
                case PolygonMark.Erroneous:
                    renderer.DrawPolygon(x, System.Drawing.Color.Red);
                    break;
            }

            foreach (Vector z in x.Vertices)
            {
                if (z.Mark == VectorMark.Selected)
                    renderer.AddSelectionPoint(z);
            }
        }

        foreach (LevObject t in Lev.Objects)
        {
            if (t.Position.Mark == VectorMark.Selected)
                renderer.AddSelectionPoint(t.Position);

            if (t.Type == ObjectType.Start)
            {
                renderer.DrawDummyPlayer(t.X, t.Y, new PlayerRenderOpts(System.Drawing.Color.Green, false, settings.RenderingSettings.ShowObjects, false), settings.RenderingSettings);
            }
        }

        foreach (GraphicElement t in Lev.GraphicElements)
        {
            if (t.Position.Mark == VectorMark.Selected)
            {
                var p1 = new Vector(t.Position.X, t.Position.Y);
                var p2 = new Vector(t.Position.X + t.Width, t.Position.Y);
                var p3 = new Vector(t.Position.X + t.Width, t.Position.Y - t.Height);
                var p4 = new Vector(t.Position.X, t.Position.Y - t.Height);
                renderer.AddSelectionLineLoop([p1, p2, p3, p4]);
            }
        }

        foreach (Vector x in _errorPoints)
            renderer.DrawSquare(x, cam.ZoomLevel / 25, System.Drawing.Color.Red);
        if (_savedStartPosition is { } p)
        {
            if (settings.RenderingSettings.ShowObjects)
            {
                renderer.DrawDummyPlayer(p.X, p.Y, new PlayerRenderOpts(System.Drawing.Color.Green, false, true, true), settings.RenderingSettings);
            }

            if (settings.RenderingSettings.ShowObjectFrames)
            {
                renderer.DrawDummyPlayer(p.X, p.Y, new PlayerRenderOpts(System.Drawing.Color.Green, false, false, true), settings.RenderingSettings);
            }
        }

        if (playController.PlayingOrPaused)
        {
            cam.CenterX += jf.X;
            cam.CenterY += jf.Y;
            renderer.SetCamera(cam);
            var driver = playController.Driver!;
            if (settings.RenderingSettings.ShowObjects && renderer.OpenGlLgr != null)
            {
                renderer.DrawPlayer(driver.GetState(), playController.RenderOptsLgr, settings.RenderingSettings);
            }
            else if (settings.RenderingSettings.ShowObjectFrames)
            {
                renderer.DrawPlayer(driver.GetState(), playController.RenderOptsFrame, settings.RenderingSettings);
            }

            if (playController.PlayerSelection == VectorMark.Selected)
                renderer.AddSelectionPoint(driver.Body.Location);
        }

        if (currentHighlight is { } hl)
        {
            switch (hl)
            {
                case HighlightTarget.PolygonTarget pt:
                    if (pt.Polygon.IsGrass)
                        renderer.DrawGrassPolygon(pt.Polygon, settings.RenderingSettings.HighlightColor, settings.RenderingSettings);
                    else
                        renderer.DrawPolygon(pt.Polygon, settings.RenderingSettings.HighlightColor);
                    break;
                case HighlightTarget.VertexTarget vt:
                    renderer.DrawPoint(vt.Polygon.Vertices[vt.VertexIndex], settings.RenderingSettings.HighlightColor);
                    break;
                case HighlightTarget.ObjectTarget ot:
                    renderer.DrawPoint(Lev.Objects[ot.ObjectIndex].Position, settings.RenderingSettings.HighlightColor);
                    break;
                case HighlightTarget.GraphicElementTarget gt:
                    renderer.DrawGraphicElementFrame(Lev.GraphicElements[gt.GraphicElementIndex], settings.RenderingSettings, settings.RenderingSettings.HighlightColor);
                    break;
                case HighlightTarget.PlayerTarget when playController.PlayingOrPaused:
                    renderer.DrawPoint(playController.Driver!.Body.Location, settings.RenderingSettings.HighlightColor);
                    break;
            }
        }

        currentTool.ExtraRendering();

        renderer.DrawSelection(settings.RenderingSettings.SelectionColor);

        renderer.Swap();
    }

    public async Task ConvertSelected(ObjectType? objType)
    {
        var editor = levelEditor;
        var selectedVertices = GetSelectedVertices();

        if (objType is { } type)
        {
            ConvertSelectedToObjects(selectedVertices, type);
            editor.SetModified(LevModification.All);
            return;
        }

        // Convert to picture/texture
        var sel = await editor.PictureDialogService.ShowConvertToPictureDialog(editor.Renderer.OpenGlLgr?.CurrentLgr);
        if (sel is { } && editor.Renderer.OpenGlLgr is not null)
        {
            RemoveSelected();
            var clipping = sel.Clipping!.Value;
            var distance = sel.Distance!.Value;
            foreach (var selectedVertex in selectedVertices)
            {
                GraphicElement g = sel switch
                {
                    ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance, selectedVertex,
                        editor.Renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Txt),
                        editor.Renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Mask)),
                    ImageSelection.PictureSelection p => GraphicElement.Pic(
                        editor.Renderer.OpenGlLgr.DrawableImageFromLgrImage(p.Pic), selectedVertex, distance, clipping),
                    _ => throw new Exception("Unexpected")
                };
                Lev.GraphicElements.Add(g);
            }
        }

        editor.SetModified(LevModification.Pictures | LevModification.Textures | LevModification.Ground);
    }

    public async Task ShowPictureProperties(bool alwaysSetDefaults)
    {
        var editor = levelEditor;
        if (editor.Renderer.OpenGlLgr is null)
        {
            return;
        }

        var selectedElems = Lev.GraphicElements.Where(p => p.Position.Mark == VectorMark.Selected).ToList();
        if (selectedElems.Count == 0)
        {
            var selectedElem = Lev.GraphicElements[_selectedPictureIndex];
            selectedElems = new List<GraphicElement> { selectedElem };
        }

        var sel = await editor.PictureDialogService.ShowPicturePropertiesDialog(
            editor.Renderer.OpenGlLgr.CurrentLgr, selectedElems, alwaysSetDefaults);
        if (sel is null) return;

        ApplyPictureProperties(sel, selectedElems, editor.Renderer.OpenGlLgr);
        editor.SetModified(LevModification.Pictures | LevModification.Textures);
        editor.RedrawScene();
    }

    public async Task TexturizeSelection()
    {
        var editor = levelEditor;
        var existingOptions = _texturizationOpts;
        if (editor.Renderer.OpenGlLgr is null)
        {
            editor.ShowError("You need to set LGR directory from settings before you can use texturize tool.", "Note");
            return;
        }

        var selected = Lev.Polygons.GetSelectedPolygonsAsMultiPolygon();
        if (selected.IsEmpty)
        {
            return;
        }

        var result = await editor.PictureDialogService.ShowTexturizeDialog(
            editor.Renderer.OpenGlLgr.CurrentLgr, existingOptions);
        if (result is not { } dialogResult)
        {
            return;
        }

        var sel = dialogResult.Selection;
        var opts = dialogResult.Options;
        _texturizationOpts = opts;

        var masks = opts.SelectedMasks
            .Select(x => editor.Renderer.OpenGlLgr.DrawableImageFromLgrImage(
                editor.Renderer.OpenGlLgr.CurrentLgr.ImageFromName(x)!)).ToList();
        var texture = editor.Renderer.OpenGlLgr.DrawableImageFromLgrImage(sel.Txt);
        var rects = masks
            .Select(i => new Envelope(0, i.WidthMinusMargin, 0, i.HeightMinusMargin));

        List<Envelope>? covering;
        try
        {
            covering = await editor.ProgressService.RunWithProgress((progress, token) =>
                selected.FindCovering(rects, token, progress,
                    iterations: opts.Iterations,
                    minRectCover: opts.MinCoverPercentage / 100));
        }
        catch (PolygonException e)
        {
            editor.ShowError(e.Message);
            return;
        }

        if (covering is null) return;

        var selmasks =
            covering.Select(env =>
                masks.First(m => Math.Abs(m.WidthMinusMargin * m.HeightMinusMargin - env.Area) < 0.001));
        var pics = selmasks.Zip(covering,
            (m, c) =>
                GraphicElement.Text(sel.Clipping!.Value, sel.Distance!.Value,
                    new Vector(c.MinX - m.EmptyPixelXMargin, c.MaxY + m.EmptyPixelYMargin), texture, m));
        Lev.GraphicElements.AddRange(pics);
        editor.SetModified(LevModification.Textures);
    }
}
