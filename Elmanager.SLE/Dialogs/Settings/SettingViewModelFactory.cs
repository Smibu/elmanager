using System;
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Input;
using DrawingColor = System.Drawing.Color;

namespace Elmanager.SLE.Dialogs.Settings;

internal sealed class SettingViewModelFactory(
    IStorageProvider storageProvider,
    Action onChanged)
{
    public BooleanSettingViewModel Boolean(
        string name,
        Func<bool> getValue,
        Action<bool> setValue,
        string? helpText = null) =>
        new(name, getValue, setValue, onChanged) { HelpText = helpText };

    public ChoiceSettingViewModel Choice(
        string name,
        Func<int> getValue,
        Action<int> setValue,
        IReadOnlyList<string> options,
        string? helpText = null) =>
        new(name, getValue, setValue, options, onChanged) { HelpText = helpText };

    public ColorSettingViewModel Color(
        string name,
        Func<DrawingColor> getValue,
        Action<DrawingColor> setValue,
        string? helpText = null) =>
        new(name, getValue, setValue, onChanged) { HelpText = helpText };

    public FileSettingViewModel File(
        string name,
        Func<Bookmark?> getValue,
        Action<Bookmark?> setValue,
        FilePickerFileType fileType,
        string? helpText = null) =>
        new(name, getValue, setValue, fileType, storageProvider, onChanged) { HelpText = helpText };

    public FolderSettingViewModel Folder(
        string name,
        Func<Bookmark?> getValue,
        Action<Bookmark?> setValue,
        string? helpText = null) =>
        new(name, getValue, setValue, storageProvider, onChanged) { HelpText = helpText };

    public KeySettingViewModel Key(
        string name,
        Func<EditorKey> getValue,
        Action<EditorKey> setValue,
        string? helpText = null) =>
        new(name, getValue, setValue, onChanged) { HelpText = helpText };

    public NumericSettingViewModel Number(
        string name,
        Func<double> getValue,
        Action<double> setValue,
        double minimum,
        double maximum,
        double increment,
        string? helpText = null,
        bool allowDecimalInput = false) =>
        new(
            name,
            () => ToDecimal(getValue()),
            value => setValue(decimal.ToDouble(value)),
            onChanged)
        {
            Minimum = ToDecimal(minimum),
            Maximum = ToDecimal(maximum),
            Increment = (decimal)increment,
            HelpText = helpText,
            AllowDecimalInput = allowDecimalInput
        };

    public NumericSettingViewModel Number(
        string name,
        Func<int> getValue,
        Action<int> setValue,
        int minimum,
        int maximum,
        int increment,
        string? helpText = null) =>
        new(
            name,
            () => getValue(),
            value => setValue(decimal.ToInt32(value)),
            onChanged)
        {
            Minimum = minimum,
            Maximum = maximum,
            Increment = increment,
            HelpText = helpText
        };

    public TextSettingViewModel Text(
        string name,
        Func<string> getValue,
        Action<string> setValue,
        string? helpText = null,
        Func<string, string?>? validate = null) =>
        new(name, getValue, setValue, onChanged) { HelpText = helpText, ValidateValue = validate };

    private static decimal ToDecimal(double value)
    {
        if (value >= (double)decimal.MaxValue)
        {
            return decimal.MaxValue;
        }

        if (value <= (double)decimal.MinValue)
        {
            return decimal.MinValue;
        }

        var converted = (decimal)value;
        if (converted == 0 && value != 0)
        {
            return value > 0 ? 0.0000000000000000000000000001m : -0.0000000000000000000000000001m;
        }

        return converted;
    }
}
