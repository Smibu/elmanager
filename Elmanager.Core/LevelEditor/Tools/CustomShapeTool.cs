using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class CustomShapeTool : ToolBase, IEditorTool
{
    private ShapeSelection? _shapeSelection;
    private string? _lastUsedShapeFolder;
    private bool _dialogOpen;

    // Mouse Interaction
    private Vector _initialMousePosition = new();

    // Transformation Properties
    private double _scalingFactor = 1.0;
    private double _rotationAngle = 0.0;
    private ShapeMirrorOption _selectedMirrorOption = ShapeMirrorOption.None;
    private PlacementAnchor _anchor = PlacementAnchor.Center;

    public CustomShapeTool(ILevelEditor editorForm) : base(editorForm)
    {
        _customShapeService = editorForm.CustomShapeService;
    }

    private readonly ICustomShapeService _customShapeService;

    public void ExtraRendering()
    {
    }

    public TransientElements GetTransientElements(bool hasFocus)
    {
        if (_shapeSelection == null)
        {
            return TransientElements.Empty;
        }

        Level level = _shapeSelection.Shape.Obj.Level;
        return new TransientElements(level.Polygons, level.Objects, level.GraphicElements);
    }

    private async Task OpenDialog()
    {
        if (_dialogOpen)
            return;

        _dialogOpen = true;
        try
        {
            var shape = await _customShapeService.OpenShapeDialog(_shapeSelection?.Shape.File.Path);
            if (shape != null)
            {
                shape.Obj.Level.UpdateImages(LevEditor.Renderer.OpenGlLgr?.DrawableImages ?? new Dictionary<string, DrawableImage>());
                _shapeSelection = new ShapeSelection(shape, new SleShape(shape.Obj.Level.Clone()));
                ApplyTransformations(CurrentPos);
            }
        }
        catch (Exception ex)
        {
            LevEditor.ShowError($"Could not open shapes: {ex.Message}", "Shape tool");
        }
        finally
        {
            _dialogOpen = false;
            LevEditor.RedrawScene();
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

        var transformedAnchor = GetAnchorPosition(originalLevel).Transform(transformationMatrix);
        var translationMatrix = Matrix.CreateTranslation(
            mousePosition.X - transformedAnchor.X,
            mousePosition.Y - transformedAnchor.Y);
        transformationMatrix = transformationMatrix * translationMatrix;

        level.Polygons = originalLevel.Polygons.Select(p => p.ApplyTransformation(transformationMatrix)).ToList();
        level.Polygons.ForEach(polygon => polygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom));

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

    public void Activate() { }

    public LevVisualChange InActivate()
    {
        return LevVisualChange.All;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        if (mouseData.Button == EditorMouseButton.Left)
        {
            HandleLeftMouseDown();
        }
        else if (mouseData.Button == EditorMouseButton.Right)
        {
            _initialMousePosition = CurrentPos;
            _ = OpenDialog();
        }
        return LevVisualChange.Nothing;
    }

    private void HandleLeftMouseDown()
    {
        if (_shapeSelection != null)
        {
            _initialMousePosition = CurrentPos;
            InsertShapeIntoLevel(CurrentPos - _initialMousePosition);
            ApplyTransformations(CurrentPos);
        }
        else
        {
            _initialMousePosition = CurrentPos;
            _ = OpenDialog();
        }
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        if (_shapeSelection != null)
        {
            ApplyTransformations(CurrentPos);
            _initialMousePosition = CurrentPos;
            return LevVisualChange.All;
        }

        return LevVisualChange.Nothing;
    }

    public void MouseUp() { }

    public LevVisualChange MouseOutOfEditor()
    {
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case EditorKey.D0:
                _scalingFactor = 1.0;
                _rotationAngle = 0.0;
                _selectedMirrorOption = ShapeMirrorOption.None;
                _anchor = PlacementAnchor.Center;
                break;
            case EditorKey.D1:
                _anchor = PlacementAnchor.Center;
                break;
            case EditorKey.D2:
                _anchor = PlacementAnchor.TopLeft;
                break;
            case EditorKey.D3:
                _anchor = PlacementAnchor.TopRight;
                break;
            case EditorKey.D4:
                _anchor = PlacementAnchor.BottomLeft;
                break;
            case EditorKey.D5:
                _anchor = PlacementAnchor.BottomRight;
                break;
            case EditorKey.OemPlus:
            case EditorKey.Add:
                _scalingFactor += 0.1;
                break;
            case EditorKey.OemMinus:
            case EditorKey.Subtract:
                _scalingFactor = Math.Max(0.1, _scalingFactor - 0.1);
                break;
            case EditorKey.D6:
                _selectedMirrorOption = _selectedMirrorOption switch
                {
                    ShapeMirrorOption.None => ShapeMirrorOption.Horizontal,
                    ShapeMirrorOption.Horizontal => ShapeMirrorOption.Vertical,
                    ShapeMirrorOption.Vertical => ShapeMirrorOption.Both,
                    ShapeMirrorOption.Both => ShapeMirrorOption.None,
                    _ => ShapeMirrorOption.None
                };
                break;
            case EditorKey.D7:
                _rotationAngle -= 5.0; // Rotate left by 5 degrees
                break;
            case EditorKey.D8:
                _rotationAngle = 0.0; // Reset rotation
                break;
            case EditorKey.D9:
                _rotationAngle += 5.0; // Rotate right by 5 degrees
                break;
        }

        ApplyTransformations(CurrentPos);
        return LevVisualChange.All;
    }

    public void KeyUp(EditorKeyEventArgs e) { }

    private Vector GetAnchorPosition(Level level) =>
        _anchor switch
        {
            PlacementAnchor.Center => new Vector(
                (level.Bounds.XMin + level.Bounds.XMax) / 2,
                (level.Bounds.YMin + level.Bounds.YMax) / 2),
            PlacementAnchor.TopLeft => new Vector(level.Bounds.XMin, level.Bounds.YMax),
            PlacementAnchor.TopRight => new Vector(level.Bounds.XMax, level.Bounds.YMax),
            PlacementAnchor.BottomLeft => new Vector(level.Bounds.XMin, level.Bounds.YMin),
            PlacementAnchor.BottomRight => new Vector(level.Bounds.XMax, level.Bounds.YMin),
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
        Lev.UpdateGrass(LevEditor.RenderingSettings.GrassZoom);
        Lev.UpdateImages(LevEditor.Renderer.OpenGlLgr?.DrawableImages ?? new Dictionary<string, DrawableImage>());
        LevEditor.SetModified(LevModification.All);
    }

    public async Task SaveShape()
    {
        if (_dialogOpen)
            return;

        _dialogOpen = true;
        try
        {
            _lastUsedShapeFolder = await _customShapeService.SaveShape(
                LevEditor, LevEditor.Renderer, _lastUsedShapeFolder) ?? _lastUsedShapeFolder;
        }
        catch (Exception ex)
        {
            LevEditor.ShowError($"Could not save shape: {ex.Message}", "Save as shape");
        }
        finally
        {
            _dialogOpen = false;
        }
    }

    public string GetHelp() =>
        "LMouse: insert new shape; RMouse: select new shape; " +
        "1-5: change placement anchor; " +
        "+/-: adjust scaling factor; " +
        "0: reset all transformations; " +
        "6: toggle mirroring (None, Horizontal, Vertical, Both); " +
        "7: rotate left; " +
        "8: reset rotation; " +
        "9: rotate right";

    public override bool Busy => _dialogOpen;
}
