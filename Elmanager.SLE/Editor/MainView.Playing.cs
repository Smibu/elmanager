using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Elmanager.LevelEditor.Playing;
using Elmanager.Physics;
using Elmanager.Rendering;
using Elmanager.Utilities;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private HashSet<int>? _fadedObjects;
    private HashSet<int>? _pendingFadedObjects;
    private Task? _playTask;
    private bool _exitFullscreenAfterPlaying;

    public bool IsPlaying => _playTask is not null;

    public async Task StopPlaying()
    {
        var playTask = _playTask;
        if (playTask is null)
        {
            return;
        }

        try
        {
            if (_playController.PlayingOrPaused)
            {
                await _playController.StopPlaying();
            }
        }
        finally
        {
            await playTask;
        }
    }

    private async void OnPlayClick(object? sender, RoutedEventArgs e)
    {
        if (_playController.Paused)
        {
            _playController.PlayState = PlayState.Playing;
            SetPlayingUi();
            return;
        }

        if (_playController.Playing)
        {
            _playController.PlayState = PlayState.Paused;
            SetPausedUi();
            return;
        }

        if (_playTask is not null)
        {
            return;
        }

        var playTask = PlayLevel(_renderer!);
        _playTask = playTask;
        try
        {
            await playTask;
        }
        finally
        {
            _playTask = null;
        }
    }

    private async Task PlayLevel(ElmaRenderer renderer)
    {
        _playController.Settings = Settings.PlayingSettings;
        Focus();
        _playController.UpdateInputKeys(_keyboardState);
        SetPlayingUi();

        var oldZoom = _zoomCtrl.ZoomLevel;
        var usePlayZoom = _playController.Settings.FollowDriverOption ==
                          FollowDriverOption.WhenPressingKey;
        if (usePlayZoom)
        {
            _zoomCtrl.ZoomLevel = _playController.Settings.PlayZoomLevel;
        }

        EnterPlayFullscreen();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (_, _) => UpdatePlayTime(1);
        timer.Start();

        try
        {
            await _playController.BeginLoop(
                _controller.Lev,
                renderer,
                _zoomCtrl,
                updateFadedObjects: QueueFadedObjects);
        }
        catch (Exception ex)
        {
            LogException(ex, "Playing stopped because an error occurred.");
        }
        finally
        {
            timer.Stop();
            UpdatePlayTime(3);

            if (usePlayZoom)
            {
                _playController.Settings.PlayZoomLevel = _zoomCtrl.ZoomLevel;
                _zoomCtrl.ZoomLevel = oldZoom;
            }

            SetStoppedUi();
            ExitPlayFullscreen();
            RedrawScene();
            await Settings.Save();
        }
    }

    private async void OnStopClick(object? sender, RoutedEventArgs e) => await StopPlaying();

    private async void OnPlaySettingsClick(object? sender, RoutedEventArgs e) => await OpenSettings("Playing");

    private void OnPlayingPaused() => Dispatcher.UIThread.Post(SetPausedUi);

    private void SetPlayingUi()
    {
        PlayIconImage.IsVisible = false;
        PauseIconImage.IsVisible = true;
        ToolTip.SetTip(PlayButton, "Pause");
        StopButton.IsEnabled = true;
    }

    private void SetPausedUi()
    {
        PlayIconImage.IsVisible = true;
        PauseIconImage.IsVisible = false;
        ToolTip.SetTip(PlayButton, "Play");
        StopButton.IsEnabled = true;
    }

    private void SetStoppedUi()
    {
        SetPausedUi();
        StopButton.IsEnabled = false;
    }

    private void UpdatePlayTime(int digits)
    {
        if (_playController.Driver is not { } driver)
        {
            return;
        }

        PlayTimeLabel.Text = driver.CurrentTime.ToSeconds().ToTimeString(digits);
        if (driver.Condition == DriverCondition.Finished)
        {
            PlayTimeLabel.Text += " F";
        }
    }

    private void EnterPlayFullscreen()
    {
        if (!_playController.Settings.ToggleFullscreen)
        {
            return;
        }

        _exitFullscreenAfterPlaying = !_fullscreenController.IsFullscreen;
        _fullscreenController.SetFullscreen(true);
    }

    private void ExitPlayFullscreen()
    {
        if (!_exitFullscreenAfterPlaying)
        {
            return;
        }

        _exitFullscreenAfterPlaying = false;
        _fullscreenController.SetFullscreen(false);
    }

    private void ToggleFullscreen()
    {
        _exitFullscreenAfterPlaying = false;
        _fullscreenController.Toggle();
    }

    private void ExitFullscreen()
    {
        _exitFullscreenAfterPlaying = false;
        _fullscreenController.SetFullscreen(false);
    }

    private async void OnFullscreenDismissed() => await StopPlaying();

    private void SetFullscreenUi(bool fullscreen)
    {
        var showEditorChrome = !fullscreen;
        EditorMenu.IsVisible = showEditorChrome;
        EditorToolbars.IsVisible = showEditorChrome;
        InfoBar.IsVisible = showEditorChrome;
        StatusBar.IsVisible = showEditorChrome;
        EditorToolPanel.IsVisible = showEditorChrome;
        ExceptionNotificationList.IsVisible = showEditorChrome;

        Grid.SetColumn(ViewportDropTarget, fullscreen ? 0 : 1);
        Grid.SetColumnSpan(ViewportDropTarget, fullscreen ? 2 : 1);
        GlViewport.Focus();
    }

    private void QueueFadedObjects(HashSet<int> fadedObjects) => _pendingFadedObjects = [.. fadedObjects];

    private void ApplyFadedObjects(bool applesUpdated)
    {
        if (_pendingFadedObjects is { } fadedObjects)
        {
            _fadedObjects = fadedObjects;
            _pendingFadedObjects = null;
            applesUpdated = true;
        }

        if (applesUpdated && _fadedObjects is not null)
        {
            _renderer!.UpdateFadedObjects(_controller.Lev, _fadedObjects);
        }
    }
}
