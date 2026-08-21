using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private static readonly int[] InternalLevelIds =
    [
        2, 4, 5, 6, 7, 8, 9, 10, 15, 59, 78, 109, 139, 219, 71, 51, 165, 57, 128, 197,
        43, 107, 98, 100, 175, 192, 38, 198, 31, 16, 18, 164, 66, 131, 156, 357, 45, 13,
        408, 412, 24, 416, 415, 95, 29, 33, 46, 21, 52, 257, 135, 133, 413, 17, 39
    ];

    private List<LevelFileEntry>? _levelFiles;
    private string? _levelFilesFolderBookmark;

    private bool _levelNavigationInProgress;

    private async void OnPrevNextClick(object? sender, RoutedEventArgs e)
    {
        var previous = ReferenceEquals(sender, PrevButton) ||
                       ReferenceEquals(sender, PreviousLevelMenuItem);
        await NavigateLevel(previous);
    }

    private async Task NavigateLevel(bool previous)
    {
        if (_levelNavigationInProgress)
        {
            return;
        }

        if (Settings.LevelFolder is null)
        {
            await ShowFolderSettingsPrompt(
                "Select a level folder before browsing levels.");
            return;
        }

        _levelNavigationInProgress = true;
        try
        {
            var levels = await GetLevelFiles();
            if (levels == null)
            {
                ShowError(
                    "The configured level folder could not be opened. Select it again in Settings.",
                    "Level folder unavailable");
                return;
            }

            if (levels.Count == 0)
            {
                ShowError(
                    "The configured level folder does not contain any .lev files.",
                    "Browse levels");
                return;
            }

            var currentFile = _controller.EditorLev.StorageFile;
            var currentFileName = currentFile?.Name;
            var currentIndex = currentFile == null
                ? -1
                : levels.FindIndex(level =>
                    string.Equals(level.Name, currentFileName, StringComparison.OrdinalIgnoreCase));
            if (currentIndex >= 0)
            {
                levels[currentIndex].StorageFile ??= currentFile;
            }

            var targetIndex = currentIndex < 0
                ? 0
                : previous
                    ? (currentIndex - 1 + levels.Count) % levels.Count
                    : (currentIndex + 1) % levels.Count;

            if (!await ConfirmDiscardUnsavedChanges())
            {
                return;
            }

            var target = levels[targetIndex];
            var targetFile = target.StorageFile;
            if (targetFile == null)
            {
                using var folder = await OpenLevelFolder();
                targetFile = folder == null ? null : await folder.GetFileAsync(target.Name);
                if (targetFile == null)
                {
                    ShowError($"Could not open \"{target.Name}\".", "Browse levels");
                    return;
                }

                target.StorageFile = targetFile;
            }

            await OpenLevelFromStorageFile(targetFile);
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not browse the configured level folder.");
        }
        finally
        {
            _levelNavigationInProgress = false;
        }
    }

    private async Task<List<LevelFileEntry>?> GetLevelFiles()
    {
        var bookmarkId = Settings.LevelFolder?.Id;
        if (bookmarkId == null)
        {
            return null;
        }

        if (_levelFiles != null &&
            string.Equals(_levelFilesFolderBookmark, bookmarkId, StringComparison.Ordinal))
        {
            return _levelFiles;
        }

        InvalidateLevelFileCache();
        var loadTask = LoadLevelFiles(bookmarkId);
        var loadingDialog = new LoadingDialog(loadTask) { Message = "Loading level folder..." };
        await loadingDialog.ShowAsync();
        return await loadTask;
    }

    private async Task<List<LevelFileEntry>?> LoadLevelFiles(string bookmarkId)
    {
        using var folder = await OpenLevelFolder();
        if (folder == null)
        {
            return null;
        }

        _levelFiles = await ReadLevelFiles(folder, bookmarkId);
        _levelFilesFolderBookmark = bookmarkId;
        return _levelFiles;
    }

    private void InvalidateLevelFileCache()
    {
        var currentFile = _controller.EditorLev.StorageFile;
        if (_levelFiles != null)
        {
            foreach (var level in _levelFiles)
            {
                if (level.StorageFile != null &&
                    !ReferenceEquals(level.StorageFile, currentFile))
                {
                    level.StorageFile.Dispose();
                }
            }
        }

        _levelFiles = null;
        _levelFilesFolderBookmark = null;
    }

    private static async Task<List<LevelFileEntry>> ReadLevelFiles(
        IStorageFolder folder,
        string bookmarkId)
    {
        var fileNames = await folder.GetFileNamesAsync(bookmarkId, ".lev");
        List<LevelFileEntry> levels = [];
        foreach (var name in fileNames)
        {
            levels.Add(new LevelFileEntry(name, null));
        }

        levels.Sort((left, right) =>
            StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name));
        return levels;
    }

    private async Task TryRestoreLastLevel()
    {
        IStorageFile? storageFile = null;
        var bookmark = Settings.SavedFile;
        if (bookmark != null)
        {
            try
            {
                storageFile = await Top.StorageProvider.OpenFileBookmarkAsync(bookmark.Id);
            }
            catch (Exception ex)
            {
                LogException(ex, "Could not restore the last-opened level bookmark.");
            }
        }

        if (Settings.SaveState == LevSaveState.Saved &&
            storageFile != null &&
            await OpenLevelFromStorageFile(storageFile))
        {
            return;
        }

        try
        {
            var restoredSaveState = Settings.SaveState switch
            {
                LevSaveState.Unsaved => LevSaveState.Unsaved,
                LevSaveState.Saved when storageFile != null => LevSaveState.Unsaved,
                _ => LevSaveState.New
            };
            await OpenLevelFromPath(AutosavePath, storageFile, restoredSaveState);
            return;
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not load the autosaved level.");
        }

        if (storageFile != null)
        {
            try
            {
                if (await OpenLevelFromStorageFile(storageFile))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "Could not load the last-opened level.");
            }
        }

        await InitializeLevel(await CreateBlankLevel(), LevSaveState.New);
    }

    private void InitializeInternalMenu()
    {
        for (var i = 0; i < Level.InternalTitles.Length; i++)
        {
            var index = i;
            var item = new MenuItem { Header = $"{i + 1}. {Level.InternalTitles[i]}" };
            item.Click += (_, _) => _ = OpenInternalLevel(index);
            if (i < 28)
            {
                OpenInternal1MenuItem.Items.Add(item);
            }
            else
            {
                OpenInternal2MenuItem.Items.Add(item);
            }
        }
    }

    private async Task OpenInternalLevel(int index)
    {
        if (!await ConfirmDiscardUnsavedChanges())
        {
            return;
        }

        var levelId = InternalLevelIds[index];
        var cacheFile = Path.Combine(AutosaveDir, $"int{index + 1:D2}.lev");
        try
        {
            await OpenLevelFromPath(cacheFile, null, LevSaveState.New);
        }
        catch (FileNotFoundException)
        {
            var bytes = await Http.GetByteArrayAsync($"https://api.elma.online/dl/level/{levelId}");
            Directory.CreateDirectory(AutosaveDir);
            await File.WriteAllBytesAsync(cacheFile, bytes);
            await OpenLevelFromPath(cacheFile, null, LevSaveState.New);
        }
        catch (Exception ex)
        {
            LogException(ex, $"Could not load internal level {index + 1}.");
        }
    }

    private async Task<bool> OpenLevelFromStorageFile(IStorageFile storageFile)
    {
        try
        {
            await using var stream = await storageFile.OpenReadAsync();
            using var mem = new MemoryStream();
            await stream.CopyToAsync(mem);
            mem.Position = 0;
            await OpenLevelFromStream(mem, storageFile, LevSaveState.Saved);
            var bookmarkId = await storageFile.SaveBookmarkAsync();
            Settings.SavedFile = bookmarkId is null
                ? null
                : new Bookmark(storageFile.Name, bookmarkId);
            await Settings.Save();
            return true;
        }
        catch (Exception ex)
        {
            LogException(ex, $"Could not open \"{storageFile.Name}\".");
            return false;
        }
    }

    private async Task OpenLevelFromPath(
        string path,
        IStorageFile? storageFile,
        LevSaveState initialSaveState)
    {
        Level lev;
        await using (var stream = File.OpenRead(path))
        {
            lev = Level.FromStream(stream);
        }

        await OpenLevel(lev, storageFile, initialSaveState);
    }

    private Task OpenLevelFromStream(
        Stream mem,
        IStorageFile? storageFile,
        LevSaveState initialSaveState) =>
        OpenLevel(Level.FromStream(mem), storageFile, initialSaveState);

    private async Task OpenLevel(
        Level lev,
        IStorageFile? storageFile,
        LevSaveState initialSaveState)
    {
        await InitializeLevel(new SleEditorLev(lev, storageFile), initialSaveState);
        await Settings.Save();
    }

    private async Task<SleEditorLev> CreateBlankLevel()
    {
        var level = Level.FromDimensions(50, 50);
        var bookmark = Settings.LevelTemplate;
        if (bookmark is not null)
        {
            try
            {
                using var templateFile = await Top.StorageProvider.OpenFileBookmarkAsync(bookmark.Id);
                if (templateFile == null)
                {
                    return CreateNewEditorLevel(level);
                }

                await using var stream = await templateFile.OpenReadAsync();
                using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer);
                buffer.Position = 0;
                level = Level.FromStream(buffer);
            }
            catch (Exception ex)
            {
                LogException(ex, "Could not load the configured level template. A blank level will be used.");
            }
        }

        return CreateNewEditorLevel(level);
    }

    private SleEditorLev CreateNewEditorLevel(Level level)
    {
        if (!Settings.UseFilenameForTitle)
        {
            level.Title = Settings.DefaultTitle;
        }

        return new SleEditorLev(level, null);
    }

    private async Task InitializeLevel(SleEditorLev lev, LevSaveState initialSaveState)
    {
        lev.Lev.UpdateGrass(Settings.RenderingSettings.GrassZoom);
        await StopPlaying();
        await _playController.NotifyLevelChanged();
        _controller.SetEditorLev(lev);
        PlayTimeLabel.Text = "";
        UpdateBestTimesUi();
        SetStoppedUi();
        if (lev.StorageFile == null)
        {
            Settings.SavedFile = null;
        }

        await Autosave();
        Settings.SaveState = initialSaveState;
        UpdateSaveButtons();
        UpdateAppTitle();

        _pendingSettingsUpdate = true;
        _pendingZoomFill = true;

        _controller.ClearHistory();
        UpdateUndoRedo();
        _currentTool.Activate();
        UpdateToolHelp();
        UpdateSelectionInfo();
        TitleBox.Text = _controller.Lev.Title;
        await RefreshLgrUi();
    }

    private async void OnOpenClick(object? sender, RoutedEventArgs e)
    {
        if (!await ConfirmDiscardUnsavedChanges())
        {
            return;
        }

        var files = await Top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open level",
            AllowMultiple = false,
            FileTypeFilter = [LevelFileTypes.LevType]
        });
        if (files.Count > 0)
        {
            await OpenLevelFromStorageFile(files[0]);
        }
    }

    private sealed class LevelFileEntry(string name, IStorageFile? storageFile)
    {
        public string Name { get; } = name;
        public IStorageFile? StorageFile { get; set; } = storageFile;
    }
}
