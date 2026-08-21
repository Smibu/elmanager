using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;
using Elmanager.Lgr;
using Elmanager.SLE.LgrUtil;
using Elmanager.Utilities;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private bool _lgrListLoadInProgress;

    private IReadOnlyList<LgrDropdownItem>? _loadedLgrItems;

    private async Task RefreshLgrUi()
    {
        var lgrSourcesChanged = _lgrCache.Configure(Settings.LgrFolder, Settings.DroppedLgrs);
        if (lgrSourcesChanged)
        {
            _loadedLgrItems = null;
        }

        await PreloadCurrentLgr();

        var showDropdown = _lgrCache.HasFolder || Settings.DroppedLgrs.Count > 0;
        LgrBox.IsVisible = showDropdown;
        SelectLgrFolderButton.IsVisible = !showDropdown;

        if (showDropdown)
        {
            var levLgr = _controller.Lev.LgrFile.ToLowerInvariant();
            PopulateLgrBox(_loadedLgrItems ?? [CreateLgrDropdownItem(levLgr)]);
        }
    }

    private async Task PreloadCurrentLgr()
    {
        var lgrName = _controller.Lev.LgrFile.ToLowerInvariant();
        var lgr = await _lgrCache.GetOrLoadLgr(lgrName);
        if (lgr == null && !string.Equals(lgrName, "default", StringComparison.OrdinalIgnoreCase))
        {
            lgr = await _lgrCache.GetOrLoadLgr("default");
        }

        PopulateTextureBoxes(lgr);
        _pendingSettingsUpdate = true;
        RedrawScene();
    }

    private void PopulateTextureBoxes(Lgr.Lgr? lgr)
    {
        var textures = lgr?.ListedImagesExcludingSpecial
            .Where(image => image.Type == ImageType.Texture)
            .Select(image => new TextureEntry(image.Name, false))
            .OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

        _programmaticTextureChange = true;
        try
        {
            GroundComboBox.Items.Clear();
            SkyComboBox.Items.Clear();
            foreach (var texture in textures)
            {
                GroundComboBox.Items.Add(texture);
                SkyComboBox.Items.Add(texture);
            }

            var enabled = lgr != null;
            GroundComboBox.IsEnabled = enabled;
            SkyComboBox.IsEnabled = enabled;
            SelectTexture(GroundComboBox, _controller.Lev.GroundTextureName, enabled);
            SelectTexture(SkyComboBox, _controller.Lev.SkyTextureName, enabled);
        }
        finally
        {
            _programmaticTextureChange = false;
        }
    }

    private static void SelectTexture(ComboBox comboBox, string name, bool lgrLoaded)
    {
        var selected = comboBox.Items
            .OfType<TextureEntry>()
            .FirstOrDefault(texture => texture.Name == name);
        if (selected == null)
        {
            selected = new TextureEntry(name, lgrLoaded);
            comboBox.Items.Add(selected);
        }

        comboBox.SelectedItem = selected;
    }

    private void OnTextureSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_programmaticTextureChange || sender is not ComboBox comboBox ||
            comboBox.SelectedItem is not TextureEntry selected)
        {
            return;
        }

        if (ReferenceEquals(comboBox, GroundComboBox))
        {
            if (_controller.Lev.GroundTextureName == selected.Name)
            {
                return;
            }

            _controller.Lev.GroundTextureName = selected.Name;
        }
        else if (ReferenceEquals(comboBox, SkyComboBox))
        {
            if (_controller.Lev.SkyTextureName == selected.Name)
            {
                return;
            }

            _controller.Lev.SkyTextureName = selected.Name;
        }
        else
        {
            return;
        }

        if (Settings.RenderingSettings.DefaultGroundAndSky)
        {
            ShowError("Default ground and sky is enabled, so you won't see this change in editor.", "Warning");
        }

        SetModified(LevModification.Start);
        _pendingSettingsUpdate = true;
        RedrawScene();
    }

    private LgrDropdownItem CreateLgrDropdownItem(string filename)
    {
        var isFound = _lgrCache.TryGetLoadedKnownName(filename, out var knownName);
        var source = _lgrCache.IsDropped(filename) ? LgrSource.Dropped : LgrSource.Folder;
        var availability = isFound ? LgrAvailability.Found : LgrAvailability.NotFound;
        return new LgrDropdownItem(filename, knownName, source, availability);
    }

    private void PopulateLgrBox(IReadOnlyList<LgrDropdownItem> entries)
    {
        var levLgr = _controller.Lev.LgrFile.ToLowerInvariant();
        var items = entries.ToList();

        _programmaticLgrChange = true;
        try
        {
            var selected = items.FirstOrDefault(e => e.Filename.EqualsIgnoreCase(levLgr));
            if (selected == null)
            {
                selected = CreateLgrDropdownItem(levLgr);
                items.Add(selected);
            }

            LgrBox.ItemsSource = items;
            LgrBox.SelectedItem = selected;
        }
        finally
        {
            _programmaticLgrChange = false;
        }
    }

    private void OnLgrPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_loadedLgrItems != null)
        {
            return;
        }

        e.Handled = true;
        _ = LoadLgrListAndOpen();
    }

    private void OnLgrKeyDownBeforeOpen(object? sender, KeyEventArgs e)
    {
        var altPressed = (e.KeyModifiers & KeyModifiers.Alt) != 0;
        var opensDropDown =
            (e.Key == Key.F4 && !altPressed) ||
            (e.Key is Key.Down or Key.Up && altPressed) ||
            e.Key is Key.Enter or Key.Space;
        if (_loadedLgrItems != null || !opensDropDown)
        {
            return;
        }

        e.Handled = true;
        _ = LoadLgrListAndOpen();
    }

    private async Task LoadLgrListAndOpen()
    {
        if (_loadedLgrItems != null || _lgrListLoadInProgress)
        {
            return;
        }

        _lgrListLoadInProgress = true;
        IReadOnlyList<LgrDropdownItem>? entries = null;
        try
        {
            var loadTask = _lgrCache.ListLgrs();
            var loadingDialog = new LoadingDialog(loadTask) { Message = "Loading LGR folder..." };
            await loadingDialog.ShowAsync();
            entries = await loadTask;
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not load the configured LGR folder.");
        }
        finally
        {
            _lgrListLoadInProgress = false;
        }

        if (entries != null)
        {
            _loadedLgrItems = entries;
            PopulateLgrBox(entries);
            LgrBox.IsDropDownOpen = true;
        }
    }

    private async void OnLgrSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_programmaticLgrChange)
        {
            return;
        }

        if (LgrBox.SelectedItem is not LgrDropdownItem entry)
        {
            return;
        }

        if (_controller.Lev.LgrFile.EqualsIgnoreCase(entry.Filename))
        {
            return;
        }

        _controller.Lev.LgrFile = entry.Filename;
        await PreloadCurrentLgr();
        SetModified(LevModification.GraphicElements);
    }

    private async void OnSelectLgrFolderClick(object? sender, RoutedEventArgs e)
    {
        var folders =
            await Top.StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { Title = "Select LGR folder", AllowMultiple = false });
        if (folders.Count == 0)
        {
            return;
        }

        using var folder = folders[0];
        var bookmarkId = await folder.SaveBookmarkAsync();
        if (bookmarkId == null)
        {
            return;
        }

        Settings.LgrFolder = new Bookmark(folder.Name, bookmarkId);
        await Settings.Save();
        await RefreshLgrUi();
    }

    private async Task AddDroppedLgr(IStorageFile storageFile)
    {
        var name = Path.GetFileNameWithoutExtension(storageFile.Name).ToLower();
        var bookmarkId = await storageFile.SaveBookmarkAsync();
        if (bookmarkId != null)
        {
            Settings.DroppedLgrs.RemoveAll(bookmark =>
                string.Equals(
                    Path.GetFileNameWithoutExtension(bookmark.DisplayName),
                    name,
                    StringComparison.OrdinalIgnoreCase));
            Settings.DroppedLgrs.Add(new Bookmark(storageFile.Name, bookmarkId));
            await Settings.Save();
        }

        _controller.Lev.LgrFile = name;
        await RefreshLgrUi();
        SetModified(LevModification.GraphicElements);
    }
}
