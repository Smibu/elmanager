using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.Rendering;
using Elmanager.SLE.Dialogs;
using Elmanager.SLE.Editor.Tools;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private static readonly string[] ImportPatterns =
    [
        "*.lev", "*.bmp", "*.png", "*.gif", "*.tiff", "*.exif", "*.svg", "*.svgz"
    ];

    private static readonly HashSet<string> ImportableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".lev",
        ".bmp",
        ".png",
        ".gif",
        ".tiff",
        ".exif",
        ".svg",
        ".svgz"
    };

    private static readonly FilePickerFileType ImportFileType =
        new("Elasto Mania level or image") { Patterns = ImportPatterns };

    private SvgImportOptions _svgImportOptions = SvgImportOptions.Default;

    private async void OnImportLevelsClick(object? sender, RoutedEventArgs e)
    {
        var files = await Top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import level(s)/image(s)",
            AllowMultiple = true,
            FileTypeFilter = [ImportFileType]
        });

        await ImportFiles(files);
    }

    private async Task ImportFiles(IEnumerable<IStorageFile> files)
    {
        var imported = 0;
        foreach (var file in files)
        {
            try
            {
                var level = await ReadImportFile(file);
                if (level is null)
                {
                    continue;
                }

                _controller.Lev.Import(level);
                imported++;
            }
            catch (Exception ex) when (ex is
                                           ImportException or
                                           BadFileException or
                                           IOException or
                                           UnauthorizedAccessException or
                                           FormatException or
                                           InvalidOperationException or
                                           ArgumentException or
                                           XmlException)
            {
                LogException(ex, $"Could not import \"{file.Name}\".");
            }
        }

        if (imported == 0)
        {
            return;
        }

        _controller.Lev.UpdateGrass(Settings.RenderingSettings.GrassZoom);
        _controller.UpdateSelectionInfo();
        SetModified(LevModification.All);
        ZoomFill(_renderer!.AspectRatio);
        RedrawScene();
    }

    private async Task<Level?> ReadImportFile(IStorageFile file)
    {
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!ImportableExtensions.Contains(extension))
        {
            throw new ImportException($"The file type '{extension}' is not supported.");
        }

        SvgImportOptions? svgOptions = null;
        if (extension is ".svg" or ".svgz")
        {
            var dialog = new SvgImportOptionsDialog(_svgImportOptions, file.Name);
            var result = await dialog.ShowAsync();
            if (!result.HasValue)
            {
                return null;
            }

            _svgImportOptions = result.Value;
            svgOptions = result.Value;
        }

        await using var input = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await input.CopyToAsync(memory);
        memory.Position = 0;

        if (svgOptions.HasValue)
        {
            return SvgImporter.FromStream(memory, extension == ".svgz", svgOptions.Value);
        }

        if (extension == ".lev")
        {
            var level = Level.FromStream(memory);
            level.UpdateImages(
                _renderer?.OpenGlLgr?.DrawableImages ??
                new Dictionary<string, DrawableImage>());
            return level;
        }

        return BitmapImporter.FromStream(memory, file.Name);
    }

    private static bool IsImportable(IStorageFile file) => ImportableExtensions.Contains(Path.GetExtension(file.Name));
}
