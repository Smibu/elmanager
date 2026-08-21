using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Elmanager.LevelEditor;
using Elmanager.SLE.Dialogs;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private async void OnRenameLevClick(object? sender, RoutedEventArgs e)
    {
        var storageFile = _controller.EditorLev.StorageFile;
        if (storageFile == null)
        {
            return;
        }

        var currentName = Path.GetFileNameWithoutExtension(storageFile.Name);
        var result = await new RenameLevelDialog(currentName).ShowAsync();
        if (!result.HasValue)
        {
            return;
        }

        await RenameLevelFile(storageFile, $"{result.Value}.lev");
    }

    private async Task RenameLevelFile(IStorageFile storageFile, string newFileName)
    {
        IStorageFile renamedFile;
        try
        {
            renamedFile = await storageFile.RenameAsync(newFileName, Top.StorageProvider);
        }
        catch (Exception ex)
        {
            LogException(ex, $"Could not rename \"{storageFile.Name}\".");
            return;
        }

        InvalidateLevelFileCache();
        _controller.SetEditorLev(_controller.EditorLev with { StorageFile = renamedFile });
        storageFile.Dispose();
        UpdateSaveButtons();
        UpdateAppTitle();

        try
        {
            var bookmarkId = await renamedFile.SaveBookmarkAsync();
            Settings.SavedFile = bookmarkId is null
                ? null
                : new Bookmark(renamedFile.Name, bookmarkId);
            await Settings.Save();
        }
        catch (Exception ex)
        {
            LogException(
                ex,
                $"The level was renamed to \"{newFileName}\", but it could not be saved as the last opened level.");
        }
    }
}
