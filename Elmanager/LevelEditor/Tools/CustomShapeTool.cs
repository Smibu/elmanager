using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;
using Elmanager.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace Elmanager.LevelEditor.Tools;

internal class CustomShapeTool : ToolBase, IEditorTool
{
    private ShapeSelection? _shapeSelection;
    private string? _lastUsedShapeFolder;

    // Mouse Interaction
    private Vector _initialMousePosition = new();
    private bool _hasFocus;

    // Transformation Properties
    private double _scalingFactor = 1.0;
    private double _rotationAngle = 0.0;
    private ShapeMirrorOption _selectedMirrorOption = ShapeMirrorOption.None;
    private PlacementAnchor _anchor = PlacementAnchor.Center;

    internal CustomShapeTool(LevelEditorForm editorForm) : base(editorForm)
    {
    }

    public void ExtraRendering()
    {
        if (_shapeSelection == null)
        {
            return;
        }

        var settings = Global.AppSettings.LevelEditor.RenderingSettings;
        foreach (var polygon in _shapeSelection.Shape.Obj.Level.Polygons)
        {
            if (polygon.IsGrass)
            {
                if (settings.ShowGrassEdges)
                {
                    var color = polygon.SlopeInfo?.HasError ?? false ? Color.Red : settings.GrassEdgeColor;
                    Renderer.DrawGrassPolygon(polygon, color, settings.ShowInactiveGrassEdges, settings);
                }
            }
        }
    }

    public TransientElements GetTransientElements()
    {
        if (!_hasFocus || _shapeSelection == null)
        {
            return TransientElements.Empty;
        }

        Level level = _shapeSelection.Shape.Obj.Level;
        return new TransientElements(level.Polygons, level.Objects,level.GraphicElements);
    }

    private void OpenDialog()
    {
        // Validate that Shapes exist
        string shapesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sle_shapes");
        if (!Directory.Exists(shapesDirectory))
        {
            UiUtils.ShowError("The 'sle_shapes' folder does not exist.\nSelect + right-click in editor to save selection as a new shape.", 
                "Shapes directory not found", MessageBoxIcon.Exclamation);
            return;
        }

        // Check each subdirectory for .lev files
        var subdirectories = Directory.GetDirectories(shapesDirectory);
        bool hasLevFiles = false;

        foreach (var subdirectory in subdirectories)
        {
            if (Directory.GetFiles(subdirectory, "*.lev", SearchOption.TopDirectoryOnly).Length > 0)
            {
                hasLevFiles = true;
                break;
            }
        }

        if (!hasLevFiles)
        {
            UiUtils.ShowError("No .lev files found in any subdirectory of 'sle_shapes'.\nSelect + right-click in editor to save selection as a new shape.",
                "No shapes found", MessageBoxIcon.Information);
            return;
        }

        ElmaFileObject<SleShape>? shape = ShapeSelectionForm.ShowForm(LevEditor.EditorControl, LevEditor.Renderer, _shapeSelection?.Shape.File.Path);
        if (shape != null)
        {
            _shapeSelection = new ShapeSelection(shape, new SleShape(shape.Obj.Level.Clone()));
            ApplyTransformations(CurrentPos);
        }
    }

    private void ApplyTransformations(Vector mousePosition)
    {
        if (_shapeSelection == null)
        {
            return;
        }

        var scalingMatrix = Matrix.CreateScaling(_scalingFactor, _scalingFactor);
        var rotationMatrix = Matrix.Identity;
        rotationMatrix.Rotate(_rotationAngle);
        var mirrorMatrix = _selectedMirrorOption switch
        {
            ShapeMirrorOption.Horizontal => Matrix.CreateScaling(-1.0, 1.0),
            ShapeMirrorOption.Vertical => Matrix.CreateScaling(1.0, -1.0),
            ShapeMirrorOption.Both => Matrix.CreateScaling(-1.0, -1.0),
            _ => Matrix.Identity
        };
        var transformationMatrix = scalingMatrix * rotationMatrix * mirrorMatrix;

        Level level = _shapeSelection.Shape.Obj.Level;
        Level originalLevel = _shapeSelection.Original.Level;

        // Center shape around mouse position
        Vector center = new Vector((level.Bounds.XMin + level.Bounds.XMax) / 2, (level.Bounds.YMin + level.Bounds.YMax) / 2);
        var min = new Vector(level.Bounds.XMin, level.Bounds.YMin);
        var max = new Vector(level.Bounds.XMax, level.Bounds.YMax);

        // Scale min / max before calculating anchor offset
        min *= scalingMatrix;
        max *= scalingMatrix;
        Vector anchorOffset = GetAnchorOffset(min, max);

        var translationMatrix = Matrix.CreateTranslation(mousePosition.X - center.X + anchorOffset.X, mousePosition.Y - center.Y + anchorOffset.Y);
        transformationMatrix = transformationMatrix * translationMatrix;

        level.Polygons = originalLevel.Polygons.Select(p => p.ApplyTransformation(transformationMatrix)).ToList();
        level.Polygons.ForEach(polygon => polygon.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom));

        level.Objects = originalLevel.Objects.Select(o =>
        {
            var newObj = o.Clone();
            newObj.Position = newObj.Position.Transform(transformationMatrix);
            return newObj;
        }).ToList();

        level.GraphicElements = originalLevel.GraphicElements.Select(ge =>
        {
            var newGe = ge with { Position = new Vector(ge.X, ge.Y) };
            newGe.Position = newGe.Position.Transform(transformationMatrix);
            return newGe;
        }).ToList();
    }

    public void Activate() => UpdateHelp();

    public void InActivate()
    {
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        if (mouseData.Button == MouseButtons.Left)
        {
            HandleLeftMouseDown();
        }
        else if (mouseData.Button == MouseButtons.Right)
        {
            _initialMousePosition = CurrentPos;
            OpenDialog();
        }
    }

    private void HandleLeftMouseDown()
    {
        if (_shapeSelection != null)
        {
            _initialMousePosition = CurrentPos;
            InsertShapeIntoLevel(CurrentPos - _initialMousePosition);
            _hasFocus = false;
            ApplyTransformations(CurrentPos);
            _hasFocus = true;
        }
        else
        {
            _initialMousePosition = CurrentPos;
            OpenDialog();
        }
    }

    public void MouseMove(Vector p)
    {
        _hasFocus = true;
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        if (_shapeSelection != null)
        {
            ApplyTransformations(CurrentPos);
            _initialMousePosition = CurrentPos;
        }
    }

    public void MouseUp() { }

    public void MouseOutOfEditor() => _hasFocus = false;

    public void KeyDown(KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.D0:
                _scalingFactor = 1.0;
                _rotationAngle = 0.0;
                _selectedMirrorOption = ShapeMirrorOption.None;
                break;
            case Keys.D1:
                _anchor = PlacementAnchor.Center;
                break;
            case Keys.D2:
                _anchor = PlacementAnchor.TopLeft;
                break;
            case Keys.D3:
                _anchor = PlacementAnchor.TopRight;
                break;
            case Keys.D4:
                _anchor = PlacementAnchor.BottomLeft;
                break;
            case Keys.D5:
                _anchor = PlacementAnchor.BottomRight;
                break;
            case Keys.Oemplus:
            case Keys.Add:
                _scalingFactor += 0.1;
                break;
            case Keys.OemMinus:
            case Keys.Subtract:
                _scalingFactor = Math.Max(0.1, _scalingFactor - 0.1);
                break;
            case Keys.D6:
                _selectedMirrorOption = _selectedMirrorOption switch
                {
                    ShapeMirrorOption.None => ShapeMirrorOption.Horizontal,
                    ShapeMirrorOption.Horizontal => ShapeMirrorOption.Vertical,
                    ShapeMirrorOption.Vertical => ShapeMirrorOption.Both,
                    ShapeMirrorOption.Both => ShapeMirrorOption.None,
                    _ => ShapeMirrorOption.None
                };
                break;
            case Keys.D7:
                _rotationAngle -= 5.0; // Rotate left by 5 degrees
                break;
            case Keys.D8:
                _rotationAngle = 0.0; // Reset rotation
                break;
            case Keys.D9:
                _rotationAngle += 5.0; // Rotate right by 5 degrees
                break;
        }

        ApplyTransformations(CurrentPos);
    }

    public void KeyUp(KeyEventArgs e) { }

    private Vector GetAnchorOffset(Vector min, Vector max) =>
        _anchor switch
        {
            PlacementAnchor.Center => new Vector(0, 0),
            PlacementAnchor.TopLeft => new Vector(min.X - max.X, max.Y - min.Y) / 2,
            PlacementAnchor.TopRight => new Vector(max.X - min.X, max.Y - min.Y) / 2,
            PlacementAnchor.BottomLeft => new Vector(min.X - max.X, min.Y - max.Y) / 2,
            PlacementAnchor.BottomRight => new Vector(max.X - min.X, min.Y - max.Y) / 2,
            _ => throw new ArgumentOutOfRangeException()
        };

    private void TranslateShape(Vector translation)
    {
        if (_shapeSelection == null)
        {
            return;
        }

        Level level = _shapeSelection.Shape.Obj.Level;
        level.Polygons.ForEach(polygon => polygon.Move(translation));
        level.Objects.ForEach(obj => obj.Position += translation);
        level.GraphicElements.ForEach(graphicElement => graphicElement.Position += translation);
    }

    private void InsertShapeIntoLevel(Vector position)
    {
        if (_shapeSelection == null)
        {
            return;
        }

        TranslateShape(position);

        Level level = _shapeSelection.Shape.Obj.Level;

        Lev.Polygons.AddRange(level.Polygons);
        Lev.Objects.AddRange(level.Objects);
        Lev.GraphicElements.AddRange(level.GraphicElements);
        Lev.UpdateAllPolygons(Global.AppSettings.LevelEditor.RenderingSettings.GrassZoom);
        LevEditor.SetModified(LevModification.Ground | LevModification.Decorations | LevModification.Objects);
    }

    public void SaveShape()
    {
        var selectedPolygons = LevEditor.Lev.Polygons.Where(p => p.Vertices.Any(v => v.Mark == VectorMark.Selected)).ToList();
        var selectedObjects = LevEditor.Lev.Objects.Where(o => o.Position.Mark == VectorMark.Selected && o.Type != ObjectType.Start).ToList();
        var selectedGraphicElements = LevEditor.Lev.GraphicElements.Where(t => t.Position.Mark == VectorMark.Selected).ToList();

        if (selectedPolygons.Count == 0 && selectedObjects.Count == 0 && selectedGraphicElements.Count == 0)
        {
            return;
        }

        bool allGrassSelected = selectedPolygons.All(pol => pol.IsGrass);
        if (allGrassSelected)
        {
            MessageBox.Show(@"All selected polygons are grass. Custom shapes require at least 1 ground polygon!",
                    @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string shapesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sle_shapes");

        if (!Directory.Exists(shapesDirectory))
        {
            try
            {
                Directory.CreateDirectory(shapesDirectory);
            }
            catch (Exception ex)
            {
                UiUtils.ShowError("Error creating directory: " + shapesDirectory + "\n\n" + ex.Message, "Error", MessageBoxIcon.Error);
                return;
            }
        }

        if (Directory.GetDirectories(shapesDirectory).Length == 0)
        {
            string uncategorizedDirName = Path.Combine(shapesDirectory, "Uncategorized");

            try
            {
                Directory.CreateDirectory(uncategorizedDirName);
            }
            catch (Exception ex)
            {
                UiUtils.ShowError("Error creating directory: " + uncategorizedDirName + "\n\n" + ex.Message, "Error", MessageBoxIcon.Error);
                return;
            }

            _lastUsedShapeFolder = uncategorizedDirName;
        }

        if (_lastUsedShapeFolder != null && !Directory.Exists(_lastUsedShapeFolder))
        {
            _lastUsedShapeFolder = null;
        }

        LevEditor.SaveShapeDialog.FileName = "Type Shape Title Here";
        LevEditor.SaveShapeDialog.InitialDirectory = _lastUsedShapeFolder ?? shapesDirectory;

        var result = LevEditor.SaveShapeDialog.ShowDialog();

        if (result != DialogResult.OK)
        {
            return;
        }

        string fullShapesDirectory = Path.GetFullPath(shapesDirectory);
        string fullFilePath = Path.GetFullPath(LevEditor.SaveShapeDialog.FileName);

        if (!fullFilePath.StartsWith(fullShapesDirectory, StringComparison.OrdinalIgnoreCase) ||
            Path.GetDirectoryName(fullFilePath)!.Equals(fullShapesDirectory, StringComparison.OrdinalIgnoreCase))
        {
            UiUtils.ShowError("Shapes must be saved within a subfolder of the 'sle_shapes' directory.", "Error", MessageBoxIcon.Error);
            return;
        }

        _lastUsedShapeFolder = Path.GetDirectoryName(fullFilePath);

        var clonedPolygons = selectedPolygons.Select(p => p.Clone()).ToList();
        var clonedObjects = selectedObjects.Select(o => o.Clone()).ToList();
        var clonedGraphicElements = selectedGraphicElements.Select(ge => ge with { Position = ge.Position.Clone() }).ToList();

        var tempLevel = new Level();
        tempLevel.Polygons.AddRange(clonedPolygons);
        tempLevel.Objects.AddRange(clonedObjects);
        tempLevel.GraphicElements.AddRange(clonedGraphicElements);
        tempLevel.UpdateImages(LevEditor.Renderer.OpenGlLgr?.DrawableImages ?? new Dictionary<string, DrawableImage>());
        if (tempLevel.PolygonCount > 0 && tempLevel.Polygons.Any(p => p.IsGrass == false))
        {
            tempLevel.UpdateAllPolygons(LevEditor.Settings.RenderingSettings.GrassZoom);
            tempLevel.UpdateBounds();
        }

        // Add start object, as it is needed.
        tempLevel.Objects.Add(new LevObject(new Vector(0, 0), ObjectType.Start, AppleType.Normal));

        tempLevel.Save(LevEditor.SaveShapeDialog.FileName);
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text =
            "LMouse: insert new shape; RMouse: select new shape; " +
            "1-5: change placement anchor; " +
            "+/-: adjust scaling factor; " +
            "0: reset all transformations; " +
            "6: toggle mirroring (None, Horizontal, Vertical, Both); " +
            "7: rotate left; " +
            "8: reset rotation; " +
            "9: rotate right";
    }

    public override bool Busy => false;
}