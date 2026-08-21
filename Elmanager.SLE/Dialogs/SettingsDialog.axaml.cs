using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Input;
using Elmanager.SLE.Dialogs.Settings;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Dialogs;

internal partial class SettingsDialog : BaseDialog<bool>
{
    private readonly Action _onChanged;
    private readonly Func<LevelEditorSettings> _resetSettings;
    private readonly IStorageProvider _storageProvider;
    private KeySettingViewModel? _capturedKeySetting;
    private SettingsDialogViewModel _viewModel = null!;

    public SettingsDialog(
        LevelEditorSettings settings,
        IStorageProvider storageProvider,
        Action onChanged,
        Func<LevelEditorSettings> resetSettings,
        string? selectedCategory = null)
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, OnDialogKeyDown, RoutingStrategies.Tunnel, true);
        _storageProvider = storageProvider;
        _onChanged = onChanged;
        _resetSettings = resetSettings;
        SetSettings(settings, selectedCategory);
    }

    private void SetSettings(LevelEditorSettings settings, string? selectedCategory = null)
    {
        CancelKeyCapture();
        _viewModel = new SettingsDialogViewModel(
            settings,
            _storageProvider,
            _onChanged,
            selectedCategory);
        DataContext = _viewModel;
    }

    private void KeySettingButton_Click(object? sender, RoutedEventArgs e)
    {
        CancelKeyCapture();
        if (sender is not Button { DataContext: KeySettingViewModel setting } button)
        {
            throw new InvalidOperationException("Playing key button has an invalid data context.");
        }

        _capturedKeySetting = setting;
        setting.BeginCapture();
        button.Focus();
    }

    private void OnDialogKeyDown(object? sender, KeyEventArgs e)
    {
        if (_capturedKeySetting is not { } setting)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            CancelKeyCapture();
            e.Handled = true;
            return;
        }

        var editorKey = AvaloniaKeyboardState.FromAvaloniaKey(e.Key);
        if (editorKey == EditorKey.None)
        {
            setting.RejectKey();
            e.Handled = true;
            return;
        }

        setting.Capture(editorKey);
        _capturedKeySetting = null;
        e.Handled = true;
    }

    private void CancelKeyCapture()
    {
        _capturedKeySetting?.CancelCapture();
        _capturedKeySetting = null;
    }

    private void ConfirmResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ResetAllSettingsButton.Flyout?.Hide();
        SetSettings(_resetSettings(), _viewModel.SelectedCategory?.Name);
        _onChanged();
    }

    private void CancelResetButton_Click(object? sender, RoutedEventArgs e) => ResetAllSettingsButton.Flyout?.Hide();

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        var invalidSetting = _viewModel.Validate();
        if (invalidSetting is null)
        {
            CancelKeyCapture();
            Close(true);
            return;
        }

        Dispatcher.UIThread.Post(() =>
            CategoryTabs
                .GetVisualDescendants()
                .OfType<Control>()
                .FirstOrDefault(control =>
                    ReferenceEquals(control.DataContext, invalidSetting) &&
                    control.Classes.Contains("setting-editor"))
                ?.Focus());
    }
}
