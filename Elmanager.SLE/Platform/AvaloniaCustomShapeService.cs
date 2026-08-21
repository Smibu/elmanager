using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Shapes;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.SLE.Dialogs;

namespace Elmanager.SLE.Platform;

internal sealed record ShapeEntry(
    string Category,
    string Name,
    string Identity,
    SleShape Shape,
    RenderingSettings RenderingSettings);

internal sealed class AvaloniaCustomShapeService : ICustomShapeService
{
    private readonly Func<LevelEditorSettings> _settingsFactory;
    private readonly Action<string, string> _showError;
    private readonly Func<IStorageProvider?> _storageProviderFactory;
    private string? _lastSelectedCategory;

    internal AvaloniaCustomShapeService(
        Func<LevelEditorSettings> settingsFactory,
        Func<IStorageProvider?> storageProviderFactory,
        Action<string, string> showError)
    {
        _settingsFactory = settingsFactory;
        _storageProviderFactory = storageProviderFactory;
        _showError = showError;
    }

    public async Task<ElmaFileObject<SleShape>?> OpenShapeDialog(string? currentShapePath)
    {
        using var root = await OpenShapeFolder();
        if (root == null)
        {
            return null;
        }

        List<ShapeEntry> entries;
        List<string> failures;
        try
        {
            (entries, failures) = await LoadShapeEntries(
                root, _settingsFactory().RenderingSettings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to enumerate shape folder: {ex}");
            _showError($"Could not read the configured shape folder: {ex.Message}", "Shape loading");
            return null;
        }

        string? loadWarning = null;
        if (failures.Count > 0)
        {
            var details = string.Join(Environment.NewLine, failures.Take(5));
            if (failures.Count > 5)
            {
                details += $"{Environment.NewLine}...and {failures.Count - 5} more.";
            }

            loadWarning =
                $"Some shape files could not be loaded:{Environment.NewLine}{details}";
        }

        var dialog = new ShapeSelectionDialog(
            entries, currentShapePath, _lastSelectedCategory, loadWarning);
        var selected = await dialog.ShowAsync();
        _lastSelectedCategory = dialog.SelectedCategory;
        return selected.HasValue
            ? ElmaFileObject<SleShape>.FromPath(selected.Value.Identity, selected.Value.Shape)
            : null;
    }

    public async Task<string?> SaveShape(
        ILevelEditor editor,
        ElmaRenderer renderer,
        string? lastUsedShapeFolder)
    {
        var shapeLevel = CreateShapeLevel(editor);
        if (shapeLevel == null)
        {
            return null;
        }

        using var root = await OpenShapeFolder();
        if (root == null)
        {
            return null;
        }

        List<string> categories;
        try
        {
            categories = await ListCategories(root);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to enumerate shape categories: {ex}");
            _showError($"Could not read shape categories: {ex.Message}", "Save as shape");
            return null;
        }

        var dialog = new ShapeSaveDialog(categories, lastUsedShapeFolder);
        var result = await dialog.ShowAsync();
        if (!result.HasValue)
        {
            return null;
        }

        var options = result.Value;
        var fileName = options.Name + ".lev";
        try
        {
            var shapeBytes = shapeLevel.GetBytes();
            using var category = await root.CreateFolderAsync(options.Category) ??
                                 throw new IOException($"Could not open category \"{options.Category}\".");
            var existing = await FindFile(category, fileName);
            if (existing != null)
            {
                using (existing)
                {
                    var confirm = new TwofoldDialog
                    {
                        Message =
                            $"A shape named \"{options.Name}\" already exists in \"{options.Category}\". Replace it?",
                        PositiveText = "Replace",
                        NegativeText = "Cancel"
                    };
                    if (await confirm.ShowAsync() != true)
                    {
                        return null;
                    }
                }
            }

            using var file = await category.CreateFileAsync(fileName);
            if (file == null)
            {
                throw new IOException($"Could not create \"{fileName}\".");
            }

            await using var stream = await file.OpenWriteAsync();
            await stream.WriteAsync(shapeBytes);
            await stream.FlushAsync();
            return options.Category;
        }
        catch (Exception ex)
        {
            _showError($"Could not save shape \"{options.Name}\": {ex.Message}", "Save as shape");
            return null;
        }
    }

    private async Task<IStorageFolder?> OpenShapeFolder()
    {
        var settings = _settingsFactory();
        var shapeFolder = settings.ShapeFolder;
        if (shapeFolder is null)
        {
            _showError("Select a shape folder in Settings (F7) before using the shape tool.",
                "Shape folder not selected");
            return null;
        }

        var provider = _storageProviderFactory();
        if (provider == null)
        {
            _showError("The storage provider is not available.", "Shape folder unavailable");
            return null;
        }

        try
        {
            var folder = await provider.OpenFolderBookmarkAsync(shapeFolder.Id);
            if (folder != null)
            {
                return folder;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open shape folder bookmark: {ex}");
        }

        _showError("The selected shape folder is no longer accessible. Select it again in Settings (F7).",
            "Shape folder unavailable");
        return null;
    }

    private static async Task<(List<ShapeEntry> Entries, List<string> Failures)> LoadShapeEntries(
        IStorageFolder root,
        RenderingSettings renderingSettings)
    {
        var entries = new List<ShapeEntry>();
        var failures = new List<string>();
        await foreach (var item in root.GetItemsAsync())
        {
            using (item)
            {
                if (item is not IStorageFolder category)
                {
                    continue;
                }

                await foreach (var categoryItem in category.GetItemsAsync())
                {
                    using (categoryItem)
                    {
                        if (categoryItem is not IStorageFile file ||
                            !file.Name.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var identity = $"{category.Name}/{file.Name}";
                        try
                        {
                            await using var stream = await file.OpenReadAsync();
                            using var bufferedStream = new MemoryStream();
                            await stream.CopyToAsync(bufferedStream);
                            bufferedStream.Position = 0;
                            var shape = SleShape.LoadFromStream(bufferedStream, identity).Obj;
                            entries.Add(new ShapeEntry(
                                category.Name,
                                Path.GetFileNameWithoutExtension(file.Name),
                                identity,
                                shape,
                                renderingSettings));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load shape '{identity}': {ex}");
                            failures.Add($"{identity}: {ex.GetBaseException().Message}");
                        }
                    }
                }
            }
        }

        entries.Sort((left, right) =>
        {
            var categoryComparison = StringComparer.OrdinalIgnoreCase.Compare(left.Category, right.Category);
            return categoryComparison != 0
                ? categoryComparison
                : StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name);
        });
        return (entries, failures);
    }

    private static async Task<List<string>> ListCategories(IStorageFolder root)
    {
        var categories = new List<string>();
        await foreach (var item in root.GetItemsAsync())
        {
            using (item)
            {
                if (item is IStorageFolder folder)
                {
                    categories.Add(folder.Name);
                }
            }
        }

        categories.Sort(StringComparer.OrdinalIgnoreCase);
        return categories;
    }

    private static async Task<IStorageFile?> FindFile(IStorageFolder folder, string fileName)
    {
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFile file &&
                string.Equals(file.Name, fileName, StringComparison.OrdinalIgnoreCase))
            {
                return file;
            }

            item.Dispose();
        }

        return null;
    }

    private Level? CreateShapeLevel(ILevelEditor editor)
    {
        var selectedPolygons = editor.Lev.Polygons
            .Where(polygon => polygon.Vertices.Any(vertex => vertex.Mark == VectorMark.Selected))
            .ToList();
        if (!selectedPolygons.Any(polygon => !polygon.IsGrass))
        {
            _showError("A shape must contain at least one selected ground polygon.", "Save as shape");
            return null;
        }

        var selectedObjects = editor.Lev.Objects
            .Where(obj => obj.Position.Mark == VectorMark.Selected && obj.Type != ObjectType.Start)
            .ToList();
        var selectedGraphicElements = editor.Lev.GraphicElements
            .Where(element => element.Position.Mark == VectorMark.Selected)
            .ToList();

        var level = new Level
        {
            Polygons = selectedPolygons.Select(polygon => polygon.Clone()).ToList(),
            Objects = selectedObjects.Select(obj => obj.Clone()).ToList(),
            GraphicElements = selectedGraphicElements
                .Select(element => element with { Position = element.Position.Clone() })
                .ToList()
        };
        level.UpdateGrass(editor.RenderingSettings.GrassZoom);
        level.UpdateBounds();
        level.Objects.Add(new LevObject(new Vector(0, 0), ObjectType.Start, AppleType.Normal));
        return level;
    }
}
