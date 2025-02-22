using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;
using Elmanager.UI;
using Path = System.IO.Path;
using Polygon = Elmanager.Lev.Polygon;

namespace Elmanager.LevelEditor.Tools;

internal class CustomShapeTool : ToolBase, IEditorTool
{
    private ShapeDataDto? _selectedShapeData;
    private string? _selectedShapeName = null;
    private Vector _initialMousePosition = new();
    private bool _hasFocus;
    private double _scalingFactor = 1.0;
    private double _rotationAngle = 0.0;

    private Level? _level;
    private Level? _originalLevel;

    private PlacementAnchor _anchor = PlacementAnchor.Center;

    // Add a property to store the selected mirroring option
    private ShapeMirrorOption _selectedMirrorOption = ShapeMirrorOption.None;

    internal CustomShapeTool(LevelEditorForm editorForm) : base(editorForm)
    {
    }

    public void ExtraRendering()
    {
        if (_selectedShapeData == null || _level == null)
        {
            return;
        }

        var settings = Global.AppSettings.LevelEditor.RenderingSettings;
        foreach (var polygon in _level.Polygons)
        {
            if (polygon.IsGrass)
            {
                if (settings.ShowGrassEdges)
                {
                    var color = polygon.SlopeInfo?.HasError ?? false ? Color.Red : settings.GrassEdgeColor;
                    Renderer.DrawGrassPolygon(polygon, color, settings.ShowInactiveGrassEdges, settings);
                }
            }
            else
            {
                if (settings.ShowGroundEdges)
                {
                    Renderer.DrawPolygon(polygon, settings.GroundEdgeColor);
                }
            }
        }
    }

    public TransientElements GetTransientElements()
    {
        return _hasFocus && _level != null
            ? new TransientElements(new List<Polygon>(_level.Polygons), new List<LevObject>(_level.Objects), new List<GraphicElement>(_level.GraphicElements))
            : TransientElements.Empty;
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
                "No shapes found", MessageBoxIcon.Exclamation);
            return;
        }

        // Open Shape Selection Form
        var shapeSelectionForm = new ShapeSelectionForm(LevEditor.EditorControl, LevEditor.Renderer, _selectedShapeName, _scalingFactor, _rotationAngle, _selectedMirrorOption);
        shapeSelectionForm.ShapeDataLoaded += ShapeSelectionForm_ShapeDataLoaded;
        shapeSelectionForm.ShowDialog();
    }

    private void ShapeSelectionForm_ShapeDataLoaded(object? sender, ShapeDataDto shapeDataDto)
    {
        if (sender is ShapeSelectionForm shapeSelectionForm)
        {
            _selectedShapeData = shapeDataDto;
            _scalingFactor = shapeSelectionForm.ScalingFactor;
            _rotationAngle = shapeSelectionForm.RotationAngle;
            _selectedMirrorOption = shapeSelectionForm.ShapeMirrorOption;
            _selectedShapeName = shapeSelectionForm.SelectedShapeName;
            LoadShapeData();
            ApplyTransformations(CurrentPos);
        }
    }

    private void LoadShapeData()
    {
        if (_selectedShapeData == null)
        {
            _level = null;
            _originalLevel = null;
            return;
        }

        _level = _selectedShapeData.Level.Clone();
        _level.UpdateAllPolygons(Global.AppSettings.LevelEditor.RenderingSettings.GrassZoom);
        _level.UpdateBounds();

        // Store the original state
        _originalLevel = _selectedShapeData.Level.Clone();

        ApplyTransformations(CurrentPos);
    }

    private void ApplyTransformations(Vector mousePosition)
    {
        if (_level == null || _originalLevel == null)
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

        // Center shape around mouse position
        Vector center = new Vector((_level.Bounds.XMin + _level.Bounds.XMax) / 2, (_level.Bounds.YMin + _level.Bounds.YMax) / 2);
        var min = new Vector(_level.Bounds.XMin, _level.Bounds.YMin);
        var max = new Vector(_level.Bounds.XMax, _level.Bounds.YMax);

        Vector anchorOffset = GetAnchorOffset(min, max);
        var translationMatrix = Matrix.CreateTranslation(mousePosition.X - center.X + anchorOffset.X, mousePosition.Y - center.Y + anchorOffset.Y);
        transformationMatrix = transformationMatrix * translationMatrix;

        _level.Polygons = _originalLevel.Polygons.Select(p => p.ApplyTransformation(transformationMatrix)).ToList();
        _level.Polygons.ForEach(polygon => polygon.UpdateDecomposition());

        _level.Objects = _originalLevel.Objects.Select(o =>
        {
            var newObj = o.Clone();
            newObj.Position = newObj.Position.Transform(transformationMatrix);
            return newObj;
        }).ToList();

        _level.GraphicElements = _originalLevel.GraphicElements.Select(ge =>
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
        if (_selectedShapeData != null)
        {
            _initialMousePosition = CurrentPos;
            InsertShapeIntoLevel(CurrentPos - _initialMousePosition);
            _hasFocus = false;
            LoadShapeData();
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
        if (_selectedShapeData != null)
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
        if (_level == null)
        {
            return;
        }

        _level.Polygons.ForEach(polygon => polygon.Move(translation));
        _level.Objects.ForEach(obj => obj.Position += translation);
        _level.GraphicElements.ForEach(graphicElement => graphicElement.Position += translation);
    }

    private void InsertShapeIntoLevel(Vector position)
    {
        if (_selectedShapeData == null || _level == null) return;

        TranslateShape(position);
        
        Lev.Polygons.AddRange(_level.Polygons);
        Lev.Objects.AddRange(_level.Objects);
        Lev.GraphicElements.AddRange(_level.GraphicElements);
        Lev.UpdateAllPolygons(Global.AppSettings.LevelEditor.RenderingSettings.GrassZoom);
        LevEditor.SetModified(LevModification.Ground | LevModification.Decorations | LevModification.Objects);
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text =
            "LMouse: insert new shape; RMouse: select new shape; " +
            "1-5: change placement anchor; " +
            "+/-: adjust scaling factor; " +
            "0 : reset all transformations; " +
            "6 : toggle mirroring (None, Horizontal, Vertical, Both); " +
            "7 : rotate left; " +
            "8 : reset rotation; " +
            "9 : rotate right";
    }

    public override bool Busy => false;
}