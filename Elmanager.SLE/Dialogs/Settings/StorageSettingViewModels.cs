using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using CommunityToolkit.Mvvm.Input;
using Elmanager.Lev;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Dialogs.Settings;

internal abstract partial class StorageSettingViewModel(
    string name,
    Func<Bookmark?> getValue,
    Action<Bookmark?> setValue,
    IStorageProvider storageProvider,
    Action onChanged)
    : ValueSettingViewModel<Bookmark?>(name, getValue, setValue, onChanged)
{
    protected IStorageProvider StorageProvider { get; } = storageProvider;

    public string ButtonText => Value is not null ? "Change..." : "Select...";

    protected abstract Task<Bookmark?> PickBookmark();

    protected override void OnValueChanged(Bookmark? value)
    {
        base.OnValueChanged(value);
        OnPropertyChanged(nameof(ButtonText));
    }

    [RelayCommand]
    private async Task Select()
    {
        var bookmark = await PickBookmark();
        if (bookmark is not null)
        {
            Value = bookmark;
        }
    }

    [RelayCommand]
    private void Clear() => Value = null;
}

internal sealed class FolderSettingViewModel(
    string name,
    Func<Bookmark?> getValue,
    Action<Bookmark?> setValue,
    IStorageProvider storageProvider,
    Action onChanged)
    : StorageSettingViewModel(name, getValue, setValue, storageProvider, onChanged)
{
    protected override async Task<Bookmark?> PickBookmark()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { Title = $"Select {Name.ToLowerInvariant()}", AllowMultiple = false });
        if (folders.Count == 0)
        {
            return null;
        }

        using var selectedFolder = folders[0];
        var bookmarkId = await selectedFolder.SaveBookmarkAsync();
        if (bookmarkId is null)
        {
            await new SingleActionDialog
            {
                Message = "The selected folder cannot be saved for later access.",
                ButtonText = "OK"
            }.ShowAsync();
            return null;
        }

        return new Bookmark(selectedFolder.Name, bookmarkId);
    }
}

internal sealed class FileSettingViewModel(
    string name,
    Func<Bookmark?> getValue,
    Action<Bookmark?> setValue,
    FilePickerFileType fileType,
    IStorageProvider storageProvider,
    Action onChanged)
    : StorageSettingViewModel(name, getValue, setValue, storageProvider, onChanged)
{
    protected override async Task<Bookmark?> PickBookmark()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = $"Select {Name.ToLowerInvariant()}",
            AllowMultiple = false,
            FileTypeFilter = [fileType]
        });
        if (files.Count == 0)
        {
            return null;
        }

        using var selectedFile = files[0];
        try
        {
            await using var stream = await selectedFile.OpenReadAsync();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            buffer.Position = 0;
            _ = Level.FromStream(buffer);

            var bookmarkId = await selectedFile.SaveBookmarkAsync();
            if (bookmarkId is null)
            {
                await ShowMessage("The selected file cannot be saved for later access.");
                return null;
            }

            return new Bookmark(selectedFile.Name, bookmarkId);
        }
        catch (Exception ex)
        {
            await ShowMessage($"The selected file could not be used as a level template.\n\n{ex.Message}");
            return null;
        }
    }

    private static Task ShowMessage(string message) =>
        new SingleActionDialog { Message = message, ButtonText = "OK" }.ShowAsync();
}
