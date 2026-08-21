using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private async void OnDeleteLevClick(object? sender, RoutedEventArgs e)
    {
        var storageFile = _controller.EditorLev.StorageFile;
        if (storageFile == null)
        {
            return;
        }

        TwofoldDialog dialog = new()
        {
            Message = $"Are you sure you want to delete \"{storageFile.Name}\"?",
            PositiveText = "Yes",
            NegativeText = "No"
        };
        var result = await dialog.ShowAsync();
        if (result != true)
        {
            return;
        }

        var deleted = false;
        try
        {
            var nextLevel = await FindNextLevel(storageFile);
            await storageFile.DeleteAsync();
            deleted = true;

            if (nextLevel == null || !await OpenLevelFromStorageFile(nextLevel))
            {
                var blankLevel = await CreateBlankLevel();
                await InitializeLevel(blankLevel, LevSaveState.New);
                await Settings.Save();
            }
        }
        catch (Exception ex)
        {
            LogException(ex, $"Could not delete \"{storageFile.Name}\".");
        }
        finally
        {
            if (deleted)
            {
                InvalidateLevelFileCache();
            }
        }
    }

    private static async Task<IStorageFile?> FindNextLevel(IStorageFile currentLevel)
    {
        using var parent = await currentLevel.GetParentAsync();
        if (parent == null)
        {
            return null;
        }

        List<IStorageFile> levels = [];
        await foreach (var item in parent.GetItemsAsync())
        {
            if (item is IStorageFile file &&
                file.Name.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
            {
                levels.Add(file);
            }
        }

        levels.Sort((left, right) => StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name));
        var currentIndex = levels.FindIndex(file => file.Path == currentLevel.Path);
        return currentIndex >= 0 && currentIndex + 1 < levels.Count ? levels[currentIndex + 1] : null;
    }
}
