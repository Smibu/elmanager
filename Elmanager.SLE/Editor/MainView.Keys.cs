using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Input;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private readonly AvaloniaKeyboardState _keyboardState = new();

    IKeyboardState ILevelEditor.KeyboardState => _keyboardState;

    private void RegisterHotkeys()
    {
        AddHotkey(new KeyGesture(Key.N, KeyModifiers.Control), OnNewClick);
        AddHotkey(new KeyGesture(Key.O, KeyModifiers.Control), OnOpenClick);
        AddHotkey(new KeyGesture(Key.S, KeyModifiers.Control), OnSaveClick);
        AddHotkey(new KeyGesture(Key.P, KeyModifiers.Control), OnSaveAsPictureClick);
        AddHotkey(new KeyGesture(Key.G, KeyModifiers.Control), OnQuickGrassClick);
        AddHotkey(new KeyGesture(Key.D, KeyModifiers.Control), OnDeleteAllGrassClick);
        AddHotkey(new KeyGesture(Key.Z, KeyModifiers.Control), OnUndoClick);
        AddHotkey(new KeyGesture(Key.Y, KeyModifiers.Control), OnRedoClick);
        AddHotkey(new KeyGesture(Key.F2), () => _ = NavigateLevel(true));
        AddHotkey(new KeyGesture(Key.F3), () => _ = NavigateLevel(false));
        AddHotkey(new KeyGesture(Key.F4), OnLevelPropertiesClick);
        AddHotkey(new KeyGesture(Key.F5), OnZoomFillClick);
        AddHotkey(new KeyGesture(Key.F6), OnCheckTopologyClick);
        AddHotkey(new KeyGesture(Key.F7), OnSettingsClick);
        AddHotkey(new KeyGesture(Key.F9), OnFixSelfIntClick);
        AddHotkey(new KeyGesture(Key.F11), ToggleFullscreen);
        AddHotkey(new KeyGesture(Key.Escape), ExitFullscreen, () => _fullscreenController.IsFullscreen);
        AddHotkey(new KeyGesture(Key.C, KeyModifiers.Control | KeyModifiers.Shift), OnCopyClick);
        AddHotkey(new KeyGesture(Key.M, KeyModifiers.Control), OnMirrorHClick);
        AddHotkey(new KeyGesture(Key.M, KeyModifiers.Control | KeyModifiers.Shift), OnMirrorVClick);

        var notInTitleBox = () => !TitleBox.IsFocused;
        AddHotkey(new KeyGesture(Key.S), () => ActivateTool(SelectButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.V), () => ActivateTool(VertexButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.D), () => ActivateTool(DrawButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.O), () => ActivateTool(ObjectButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.P), () => ActivateTool(PipeButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.E), () => ActivateTool(EllipseButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.L), () => ActivateTool(PolyOpButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.F), () => ActivateTool(FrameButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.M), () => ActivateTool(SmoothenButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.C), () => ActivateTool(CutConnectButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.A), () => ActivateTool(AutoGrassButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.I), () => ActivateTool(PictureButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.T), () => ActivateTool(TextButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.H), () => ActivateTool(CustomShapeButton), notInTitleBox);
        AddHotkey(new KeyGesture(Key.A, KeyModifiers.Control), OnSelectAllClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.C, KeyModifiers.Control), OnCopyClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.V, KeyModifiers.Control), OnImportLevelsClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.Delete), OnDeleteSelectedClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.OemComma), OnUnionClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.OemPeriod), OnDifferenceClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.Oem2), OnSymDiffClick, notInTitleBox);
        AddHotkey(new KeyGesture(Key.Oem5), OnTexturizeClick, notInTitleBox);
    }

    private void AddHotkey(KeyGesture gesture, EventHandler<RoutedEventArgs> handler,
        Func<bool>? extraCanExecute = null)
        => AddHotkey(gesture, () => handler(this, new RoutedEventArgs()), extraCanExecute);

    private void AddHotkey(KeyGesture gesture, Action action, Func<bool>? extraCanExecute = null) =>
        KeyBindings.Add(new KeyBinding
        {
            Gesture = gesture,
            Command = new RelayCommand(
                action,
                () => !RootDialogHost.IsOpen &&
                      (!_playController.Playing || !_playController.Settings.DisableShortcuts) &&
                      (extraCanExecute?.Invoke() ?? true))
        });

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (IsKeyboardFocusWithin)
        {
            return;
        }

        _keyboardState.Clear();
        if (_playController.PlayingOrPaused)
        {
            _playController.UpdateInputKeys(_keyboardState);
        }
    }

    private void OnPlayingKeyUp(object? sender, KeyEventArgs e)
    {
        if (RootDialogHost.IsOpen)
        {
            return;
        }

        _keyboardState.OnKeyUp(e.Key);
        if (!_playController.PlayingOrPaused)
        {
            return;
        }

        _playController.UpdateInputKeys(_keyboardState);
    }

    private void OnPlayingKeyDown(object? sender, KeyEventArgs e)
    {
        if (RootDialogHost.IsOpen)
        {
            return;
        }

        _keyboardState.OnKeyDown(e.Key);
        if (!_playController.PlayingOrPaused)
        {
            return;
        }

        _playController.UpdateInputKeys(_keyboardState);

        if (e.Key == Key.Enter)
        {
            OnPlayClick(sender, new RoutedEventArgs());
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            ExitFullscreen();
            OnStopClick(sender, new RoutedEventArgs());
            e.Handled = true;
        }
        else if (_playController.Playing && _playController.Settings.DisableShortcuts)
        {
            e.Handled = true;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (RootDialogHost.IsOpen)
        {
            return;
        }

        if (!_playController.PlayingOrPaused)
        {
            _keyboardState.OnKeyUp(e.Key);
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (RootDialogHost.IsOpen)
        {
            return;
        }

        if (!_playController.PlayingOrPaused)
        {
            _keyboardState.OnKeyDown(e.Key);
            if (e.Key == Key.Enter && !TitleBox.IsFocused)
            {
                OnPlayClick(sender, new RoutedEventArgs());
                e.Handled = true;
                return;
            }
        }

        if (e.Handled)
        {
            return;
        }

        var editorKey = AvaloniaKeyboardState.FromAvaloniaKey(e.Key);
        if (editorKey == EditorKey.None)
        {
            return;
        }

        var mod = _currentTool.KeyDown(new EditorKeyEventArgs(editorKey));
        SetPendingModification(mod);
        UpdateToolHelp();
        RedrawScene();
    }
}
