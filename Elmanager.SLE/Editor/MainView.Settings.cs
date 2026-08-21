using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;
using Elmanager.SLE.Dialogs;
using Size = System.Drawing.Size;
using WindowState = Elmanager.Settings.WindowState;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private const double ToolbarIconPaddingRatio = 0.1;

    private LevelEditorSettings Settings { get; set; }

    LevelEditorRenderingSettings ILevelEditor.RenderingSettings => Settings.RenderingSettings;
    LevelEditorSettings ILevelEditor.Settings => Settings;
    bool ILevelEditor.ObjectFramesVisible => ShowObjectFramesButton.IsChecked == true;
    bool ILevelEditor.ObjectsVisible => ShowObjectsButton.IsChecked == true;
    bool ILevelEditor.GrassEdgesVisible => ShowGrassEdgesButton.IsChecked == true;
    bool ILevelEditor.GrassVisible => ShowGrassButton.IsChecked == true;
    bool ILevelEditor.GroundEdgesVisible => ShowGroundEdgesButton.IsChecked == true;
    bool ILevelEditor.GroundVisible => ShowGroundButton.IsChecked == true;
    bool ILevelEditor.TextureFramesVisible => ShowTextureFramesButton.IsChecked == true;
    bool ILevelEditor.TexturesVisible => ShowTexturesButton.IsChecked == true;
    bool ILevelEditor.PictureFramesVisible => ShowPictureFramesButton.IsChecked == true;
    bool ILevelEditor.PicturesVisible => ShowPicturesButton.IsChecked == true;

    private void ApplyToolbarIconSize()
    {
        var iconSize = Settings.ToolbarIconSize;
        Resources["ToolbarIconSize"] = iconSize;
        Resources["ToolbarIconPadding"] = new Thickness(iconSize * ToolbarIconPaddingRatio);
    }

    private void OnToolbarPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if ((e.KeyModifiers & KeyModifiers.Control) == 0 || e.Delta.Y == 0)
        {
            return;
        }

        var direction = e.Delta.Y > 0 ? 1 : -1;
        Settings.ToolbarIconSize += direction * LevelEditorSettings.ToolbarIconSizeStep;
        ApplyToolbarIconSize();
        _ = Settings.Save();
        e.Handled = true;
    }

    public void RestoreWindowState(Window window)
    {
        var size = Settings.Size;
        if (size is { Width: > 0, Height: > 0 })
        {
            window.Width = size.Width;
            window.Height = size.Height;
        }

        window.WindowState = Settings.WindowState == WindowState.Maximized
            ? Avalonia.Controls.WindowState.Maximized
            : Avalonia.Controls.WindowState.Normal;
    }

    public async Task SaveWindowState(Window window)
    {
        if (window.WindowState == Avalonia.Controls.WindowState.Normal)
        {
            Settings.Size = new Size((int)window.Width, (int)window.Height);
        }

        Settings.WindowState = window.WindowState == Avalonia.Controls.WindowState.Maximized
            ? WindowState.Maximized
            : WindowState.Normal;

        await Settings.Save();
    }

    private void SyncRenderingSettings()
    {
        var rs = Settings.RenderingSettings;
        rs.ShowGround = ShowGroundButton.IsChecked == true;
        rs.ShowGroundEdges = ShowGroundEdgesButton.IsChecked == true;
        rs.ShowGrassEdges = ShowGrassEdgesButton.IsChecked == true;
        rs.ShowInactiveGrassEdges = ShowInactiveGrassEdgesButton.IsChecked == true;
        rs.ShowGrass = ShowGrassButton.IsChecked == true;
        rs.ShowVertices = ShowVerticesButton.IsChecked == true;
        rs.ShowGrid = ShowGridButton.IsChecked == true;
        rs.ShowPictureFrames = ShowPictureFramesButton.IsChecked == true;
        rs.ShowTextureFrames = ShowTextureFramesButton.IsChecked == true;
        rs.ShowPictures = ShowPicturesButton.IsChecked == true;
        rs.ShowTextures = ShowTexturesButton.IsChecked == true;
        rs.ShowObjectFrames = ShowObjectFramesButton.IsChecked == true;
        rs.ShowObjects = ShowObjectsButton.IsChecked == true;
        rs.ShowGravityAppleArrows = ShowGravAppleButton.IsChecked == true;
        rs.GroundTextureEnabled = ShowGroundTextureButton.IsChecked == true;
        rs.SkyTextureEnabled = ShowSkyTextureButton.IsChecked == true;
        rs.ZoomTextures = ZoomTexturesButton.IsChecked == true;
        Settings.ShowCrossHair = ShowCrossHairButton.IsChecked == true;
        Settings.SnapToGrid = SnapToGridButton.IsChecked == true;
        Settings.LockGrid = LockGridButton.IsChecked == true;
        _ = Settings.Save();
    }

    private void SyncRenderingSettingsToUi()
    {
        ShowGroundButton.IsChecked = Settings.RenderingSettings.ShowGround;
        ShowGroundEdgesButton.IsChecked = Settings.RenderingSettings.ShowGroundEdges;
        ShowGrassEdgesButton.IsChecked = Settings.RenderingSettings.ShowGrassEdges;
        ShowInactiveGrassEdgesButton.IsChecked = Settings.RenderingSettings.ShowInactiveGrassEdges;
        ShowGrassButton.IsChecked = Settings.RenderingSettings.ShowGrass;
        ShowVerticesButton.IsChecked = Settings.RenderingSettings.ShowVertices;
        ShowGridButton.IsChecked = Settings.RenderingSettings.ShowGrid;
        ShowPictureFramesButton.IsChecked = Settings.RenderingSettings.ShowPictureFrames;
        ShowTextureFramesButton.IsChecked = Settings.RenderingSettings.ShowTextureFrames;
        ShowPicturesButton.IsChecked = Settings.RenderingSettings.ShowPictures;
        ShowTexturesButton.IsChecked = Settings.RenderingSettings.ShowTextures;
        ShowObjectFramesButton.IsChecked = Settings.RenderingSettings.ShowObjectFrames;
        ShowObjectsButton.IsChecked = Settings.RenderingSettings.ShowObjects;
        ShowGravAppleButton.IsChecked = Settings.RenderingSettings.ShowGravityAppleArrows;
        ShowGroundTextureButton.IsChecked = Settings.RenderingSettings.GroundTextureEnabled;
        ShowSkyTextureButton.IsChecked = Settings.RenderingSettings.SkyTextureEnabled;
        ZoomTexturesButton.IsChecked = Settings.RenderingSettings.ZoomTextures;
        ShowCrossHairButton.IsChecked = Settings.ShowCrossHair;
        SnapToGridButton.IsChecked = Settings.SnapToGrid;
        LockGridButton.IsChecked = Settings.LockGrid;
    }

    private async Task ShowFolderSettingsPrompt(string message)
    {
        TwofoldDialog dialog = new() { Message = message, PositiveText = "Open settings", NegativeText = "Close" };
        if (await dialog.ShowAsync() != true)
        {
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(
            static () => { },
            DispatcherPriority.Background);
        await OpenSettings();
    }

    private async void OnSettingsClick(object? sender, RoutedEventArgs e) => await OpenSettings();

    private async Task OpenSettings(string? selectedCategory = null)
    {
        var levelFolderBookmarkId = Settings.LevelFolder?.Id;
        var lgrFolderBookmarkId = Settings.LgrFolder?.Id;
        var dialog = new SettingsDialog(Settings, Top.StorageProvider, () =>
        {
            ApplyToolbarIconSize();
            _pendingSettingsUpdate = true;
            RedrawScene();
        }, () =>
        {
            Settings = new LevelEditorSettings();
            ApplyToolbarIconSize();
            SyncRenderingSettingsToUi();
            return Settings;
        }, selectedCategory);
        await dialog.ShowAsync();
        if (levelFolderBookmarkId != Settings.LevelFolder?.Id)
        {
            InvalidateLevelFileCache();
        }

        if (lgrFolderBookmarkId != Settings.LgrFolder?.Id)
        {
            await RefreshLgrUi();
        }

        _playController.Settings = Settings.PlayingSettings;
        await Settings.Save();
    }

    private void OnRenderingToggleChanged(object? sender, RoutedEventArgs e)
    {
        if (_renderer == null)
        {
            return;
        }

        SyncRenderingSettings();
        _pendingSettingsUpdate = true;
        RedrawScene();
    }
}
