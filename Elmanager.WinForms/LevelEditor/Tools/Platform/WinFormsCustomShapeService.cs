using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;
using Elmanager.UI;
using Path = System.IO.Path;

namespace Elmanager.LevelEditor.Tools.Platform;

internal class WinFormsCustomShapeService : ICustomShapeService
{
    private readonly LevelEditorForm _levEditor;

    internal WinFormsCustomShapeService(LevelEditorForm levEditor)
    {
        _levEditor = levEditor;
    }

    public ElmaFileObject<SleShape>? OpenShapeDialog(string? currentShapePath)
    {
        string shapesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sle_shapes");
        if (!Directory.Exists(shapesDirectory))
        {
            UiUtils.ShowError("The 'sle_shapes' folder does not exist.\nSelect + right-click in editor to save selection as a new shape.",
                "Shapes directory not found", MessageBoxIcon.Exclamation);
            return null;
        }

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
            return null;
        }

        return ShapeSelectionForm.ShowForm(_levEditor.EditorControl, _levEditor.Renderer, currentShapePath);
    }

    public string? SaveShape(ILevelEditor editor, ElmaRenderer renderer, string? lastUsedShapeFolder)
    {
        string? newFolder = lastUsedShapeFolder;
        var selectedPolygons = _levEditor.Lev.Polygons.Where(p => p.Vertices.Any(v => v.Mark == VectorMark.Selected)).ToList();
        var selectedObjects = _levEditor.Lev.Objects.Where(o => o.Position.Mark == VectorMark.Selected && o.Type != ObjectType.Start).ToList();
        var selectedGraphicElements = _levEditor.Lev.GraphicElements.Where(t => t.Position.Mark == VectorMark.Selected).ToList();

        if (selectedPolygons.Count == 0 && selectedObjects.Count == 0 && selectedGraphicElements.Count == 0)
        {
            return null;
        }

        bool allGrassSelected = selectedPolygons.All(pol => pol.IsGrass);
        if (allGrassSelected)
        {
            MessageBox.Show(@"All selected polygons are grass. Custom shapes require at least 1 ground polygon!",
                @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
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
                return null;
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
                return null;
            }

            newFolder = uncategorizedDirName;
        }

        if (newFolder != null && !Directory.Exists(newFolder))
        {
            newFolder = null;
        }

        _levEditor.SaveShapeDialog.FileName = "Type Shape Title Here";
        _levEditor.SaveShapeDialog.InitialDirectory = newFolder ?? shapesDirectory;

        var result = _levEditor.SaveShapeDialog.ShowDialog();

        if (result != DialogResult.OK)
        {
            return null;
        }

        string fullShapesDirectory = Path.GetFullPath(shapesDirectory);
        string fullFilePath = Path.GetFullPath(_levEditor.SaveShapeDialog.FileName);

        if (!fullFilePath.StartsWith(fullShapesDirectory, StringComparison.OrdinalIgnoreCase) ||
            Path.GetDirectoryName(fullFilePath)!.Equals(fullShapesDirectory, StringComparison.OrdinalIgnoreCase))
        {
            UiUtils.ShowError("Shapes must be saved within a subfolder of the 'sle_shapes' directory.", "Error", MessageBoxIcon.Error);
            return null;
        }

        newFolder = Path.GetDirectoryName(fullFilePath);

        var clonedPolygons = selectedPolygons.Select(p => p.Clone()).ToList();
        var clonedObjects = selectedObjects.Select(o => o.Clone()).ToList();
        var clonedGraphicElements = selectedGraphicElements.Select(ge => ge with { Position = ge.Position.Clone() }).ToList();

        var tempLevel = new Level();
        tempLevel.Polygons.AddRange(clonedPolygons);
        tempLevel.Objects.AddRange(clonedObjects);
        tempLevel.GraphicElements.AddRange(clonedGraphicElements);
        tempLevel.UpdateImages(renderer.OpenGlLgr?.DrawableImages ?? new Dictionary<string, DrawableImage>());
        if (tempLevel.PolygonCount > 0 && tempLevel.Polygons.Any(p => p.IsGrass == false))
        {
            tempLevel.UpdateGrass(_levEditor.Settings.RenderingSettings.GrassZoom);
            tempLevel.UpdateBounds();
        }

        tempLevel.Objects.Add(new LevObject(new Vector(0, 0), ObjectType.Start, AppleType.Normal));

        tempLevel.Save(_levEditor.SaveShapeDialog.FileName);
        return newFolder;
    }
}
