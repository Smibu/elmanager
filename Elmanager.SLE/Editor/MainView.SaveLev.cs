using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;
using Elmanager.SLE.Dialogs.Settings;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private static readonly FilePickerFileType PngPictureType =
        new("Portable Network Graphics") { Patterns = ["*.png"], MimeTypes = ["image/png"] };

    private static readonly FilePickerFileType SvgPictureType =
        new("Scalable Vector Graphics") { Patterns = ["*.svg"], MimeTypes = ["image/svg+xml"] };

    private static string AutosaveDir => OperatingSystem.IsBrowser() ? "/sle" : AppContext.BaseDirectory;
    private static string AutosavePath => Path.Combine(AutosaveDir, "sle_autosave.lev");

    private void UpdateSaveButtons()
    {
        var enable = Settings.SaveState is LevSaveState.Unsaved or LevSaveState.New;
        SaveButton.IsEnabled = enable;
        SaveMenuItem.IsEnabled = enable;

        var hasLevelFile = _controller.EditorLev.StorageFile != null;
        RenameButton.IsEnabled = hasLevelFile;
        DeleteButton.IsEnabled = hasLevelFile;
        DeleteLevMenuItem.IsEnabled = hasLevelFile;
    }

    private void OnTitleTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (TitleBox.Text == _controller.Lev.Title)
        {
            return;
        }

        Settings.SaveState = LevSaveState.Unsaved;
        UpdateSaveButtons();
        UpdateAppTitle();
    }

    private async Task Autosave()
    {
        try
        {
            Directory.CreateDirectory(AutosaveDir);
            {
                await using var stream = new FileStream(AutosavePath, FileMode.Create);
                await _controller.Lev.SaveToStreamAsync(stream, false);
            }
            if (OperatingSystem.IsBrowser())
            {
                await BrowserInterop.SyncToIndexedDb();
            }
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not autosave the level.");
        }
    }

    private void OnSaveAsClick(object? sender, RoutedEventArgs e) => _ = SaveAs();

    private async Task SaveAs()
    {
        using var levelFolder = await OpenLevelFolder();
        var levelFiles = levelFolder == null
            ? []
            : await GetLevelFileNames(levelFolder);
        var file = await Top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Level As",
            DefaultExtension = "lev",
            SuggestedFileName = FilenameSuggestion.Create(Settings.DefaultFilename, levelFiles),
            SuggestedFileType = LevelFileTypes.LevType,
            SuggestedStartLocation = levelFolder,
            FileTypeChoices = [LevelFileTypes.LevType]
        });
        if (file != null)
        {
            await SaveToStorageFile(file, true);
        }
    }

    private async Task<IStorageFolder?> OpenLevelFolder()
    {
        var bookmark = Settings.LevelFolder;
        if (bookmark is null)
        {
            return null;
        }

        try
        {
            var folder = await Top.StorageProvider.OpenFolderBookmarkAsync(bookmark.Id);
            if (folder == null)
            {
                Console.WriteLine("The configured level folder could not be restored.");
            }

            return folder;
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not restore the configured level folder.");
            return null;
        }
    }

    private async Task<IReadOnlyList<string>> GetLevelFileNames(IStorageFolder folder)
    {
        List<string> levelFiles = [];
        try
        {
            var bookmark = Settings.LevelFolder;
            if (bookmark == null)
            {
                return levelFiles;
            }

            levelFiles.AddRange(await folder.GetFileNamesAsync(bookmark.Id, ".lev"));
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not inspect the configured level folder.");
        }

        return levelFiles;
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e) => _ = Save();

    private async Task Save()
    {
        if (_controller.EditorLev.StorageFile != null)
        {
            await SaveToStorageFile(_controller.EditorLev.StorageFile, true);
            return;
        }

        await SaveAs();
    }

    private async Task SaveToStorageFile(IStorageFile storageFile, bool saveAsFresh)
    {
        try
        {
            _controller.Lev.Title = TitleBox.Text ?? "New level";
            {
                await using var stream = await storageFile.OpenWriteAsync();
                await _controller.Lev.SaveToStreamAsync(stream, saveAsFresh);
            }
            if (saveAsFresh)
            {
                UpdateBestTimesUi();
            }

            _controller.MarkSaved();
            _controller.SetNotModified();
            _controller.SetEditorLev(_controller.EditorLev with { StorageFile = storageFile });
            if (saveAsFresh)
            {
                InvalidateLevelFileCache();
            }

            var bookmarkId = await storageFile.SaveBookmarkAsync();
            if (bookmarkId != null)
            {
                Settings.SavedFile = new Bookmark(storageFile.Name, bookmarkId);
            }

            Settings.SaveState = LevSaveState.Saved;
            UpdateSaveButtons();
            UpdateAppTitle();
            await Settings.Save();
        }
        catch (Exception ex)
        {
            LogException(ex, $"Could not save \"{storageFile.Name}\".");
        }
    }

    private async void OnSaveAsPictureClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var levelFileName = _controller.EditorLev.StorageFile?.Name ?? "Untitled";
            var file = await Top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Level as Picture",
                DefaultExtension = "png",
                SuggestedFileName = Path.GetFileNameWithoutExtension(levelFileName),
                SuggestedFileType = PngPictureType,
                FileTypeChoices = [PngPictureType, SvgPictureType]
            });
            if (file == null)
            {
                return;
            }

            var extension = Path.GetExtension(file.Name);
            if (!extension.Equals(".png", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("File type must be PNG or SVG.", "Save as picture");
                return;
            }

            await using var stream = await file.OpenWriteAsync();
            if (extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
            {
                var svgBytes =
                    Encoding.UTF8.GetBytes(SvgExporter.CreateSvg(_controller.Lev, Settings.RenderingSettings));
                await stream.WriteAsync(svgBytes);
            }
            else
            {
                await SavePng(stream);
            }

            await stream.FlushAsync();
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not save the level picture.");
        }
    }

    private async Task SavePng(Stream stream)
    {
        var renderer = _renderer ?? throw new InvalidOperationException("The renderer is not ready.");
        var snapshotSettings = Settings.RenderingSettings.Clone();
        snapshotSettings.SmoothZoomEnabled = false;
        var zoomLevel = _zoomCtrl.ZoomLevel;
        var centerX = _zoomCtrl.CenterX;
        var centerY = _zoomCtrl.CenterY;
        byte[]? pngBytes = null;

        await RunOnRenderThread(() =>
        {
            try
            {
                pngBytes = renderer.GetSnapShotPngBytes(
                    _zoomCtrl, snapshotSettings, _controller.Lev, DrawEditorScene);
            }
            finally
            {
                _zoomCtrl.ZoomLevel = zoomLevel;
                _zoomCtrl.CenterX = centerX;
                _zoomCtrl.CenterY = centerY;
            }
        });
        await stream.WriteAsync(pngBytes ??
                                throw new InvalidOperationException("The renderer did not produce a PNG image."));
    }

    private async Task<bool> ConfirmDiscardUnsavedChanges()
    {
        if (Settings.SaveState is LevSaveState.Saved or LevSaveState.New)
        {
            return true;
        }

        ThreefoldDialog dialog = new()
        {
            Message = "Level has unsaved changes. Do you want to save them?",
            PositiveText = "Yes",
            NegativeText = "No",
            NeutralText = "Cancel"
        };
        var result = await dialog.ShowAsync();
        if (!result.HasValue || result.Value == ThreefoldDialog.ButtonType.Neutral)
        {
            return false;
        }

        if (result.Value == ThreefoldDialog.ButtonType.Positive)
        {
            await Save();
            return Settings.SaveState == LevSaveState.Saved;
        }

        return true;
    }
}
