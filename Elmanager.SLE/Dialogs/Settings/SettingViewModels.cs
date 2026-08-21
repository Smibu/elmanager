using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Elmanager.LevelEditor.Input;
using DrawingColor = System.Drawing.Color;

namespace Elmanager.SLE.Dialogs.Settings;

internal abstract class SettingViewModel(string name) : ObservableObject
{
    public string Name { get; } = name;
    public string? HelpText { get; init; }

    protected static void ApplyIfChanged<T>(
        T currentValue,
        T newValue,
        Action<T> apply,
        Action onChanged)
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
        {
            return;
        }

        apply(newValue);
        onChanged();
    }

    protected static string? TryApply<T>(
        T currentValue,
        T newValue,
        Action<T> apply,
        Action onChanged)
    {
        try
        {
            ApplyIfChanged(currentValue, newValue, apply, onChanged);
            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}

internal abstract class ValueSettingViewModel<T>(
    string name,
    Func<T> getValue,
    Action<T> setValue,
    Action onChanged)
    : SettingViewModel(name)
{
    private T _value = getValue();

    public T Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                OnValueChanged(value);
            }
        }
    }

    protected virtual void OnValueChanged(T value) =>
        ApplyIfChanged(getValue(), value, setValue, onChanged);
}

internal abstract class ValidationSettingViewModel(
    string name,
    string validatedPropertyName)
    : SettingViewModel(name), INotifyDataErrorInfo
{
    public IReadOnlyList<string> ValidationErrors { get; private set; } = [];

    public bool HasErrors => ValidationErrors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName) || propertyName == validatedPropertyName
            ? ValidationErrors
            : Array.Empty<string>();

    protected bool SetValidationError(string? error)
    {
        var currentError = ValidationErrors.Count == 0 ? null : ValidationErrors[0];
        if (string.Equals(currentError, error, StringComparison.Ordinal))
        {
            return error is null;
        }

        ValidationErrors = error is null ? [] : [error];
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(ValidationErrors));
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(validatedPropertyName));
        return error is null;
    }
}

internal abstract class ValidatableSettingViewModel(
    string name,
    string validatedPropertyName)
    : ValidationSettingViewModel(name, validatedPropertyName)
{
    public abstract bool Validate();
}

internal abstract class ValidatableValueSettingViewModel<T>(
    string name,
    T value)
    : ValidatableSettingViewModel(name, nameof(Value))
{
    private T _value = value;

    public T Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                Validate();
            }
        }
    }
}

internal sealed class BooleanSettingViewModel(
    string name,
    Func<bool> getValue,
    Action<bool> setValue,
    Action onChanged)
    : ValueSettingViewModel<bool>(name, getValue, setValue, onChanged);

internal sealed class ChoiceSettingViewModel(
    string name,
    Func<int> getValue,
    Action<int> setValue,
    IReadOnlyList<string> options,
    Action onChanged)
    : ValueSettingViewModel<int>(name, getValue, setValue, onChanged)
{
    public IReadOnlyList<string> Options { get; } = options;
}

internal sealed class KeySettingViewModel(
    string name,
    Func<EditorKey> getValue,
    Action<EditorKey> setValue,
    Action onChanged)
    : ValidationSettingViewModel(name, nameof(DisplayValue))
{
    private bool _isCapturing;
    private EditorKey _value = getValue();

    public string DisplayValue => _isCapturing
        ? "Press a key or ESC to cancel"
        : _value.ToString();

    public void BeginCapture()
    {
        _isCapturing = true;
        SetValidationError(null);
        OnPropertyChanged(nameof(DisplayValue));
    }

    public void CancelCapture()
    {
        _isCapturing = false;
        SetValidationError(null);
        OnPropertyChanged(nameof(DisplayValue));
    }

    public void RejectKey() => SetValidationError("This key cannot be used as a playing key.");

    public void Capture(EditorKey value)
    {
        var changed = _value != value;
        setValue(value);
        _value = value;
        _isCapturing = false;
        SetValidationError(null);
        OnPropertyChanged(nameof(DisplayValue));

        if (changed)
        {
            onChanged();
        }
    }
}

internal sealed class TextSettingViewModel(
    string name,
    Func<string> getValue,
    Action<string> setValue,
    Action onChanged)
    : ValidatableValueSettingViewModel<string>(name, getValue())
{
    internal Func<string, string?>? ValidateValue { get; init; }

    public override bool Validate() =>
        SetValidationError(
            ValidateValue?.Invoke(Value) ??
            TryApply(getValue(), Value, setValue, onChanged));
}

internal sealed class NumericSettingViewModel(
    string name,
    Func<decimal> getValue,
    Action<decimal> setValue,
    Action onChanged)
    : ValidatableValueSettingViewModel<decimal?>(name, getValue())
{
    internal bool AllowDecimalInput { get; init; }

    public required decimal Minimum { get; init; }
    public required decimal Maximum { get; init; }
    public required decimal Increment { get; init; }
    public string FormatString => AllowDecimalInput ? "0.############################" : "0";

    public NumberStyles ParsingNumberStyle =>
        AllowDecimalInput ? NumberStyles.Number : NumberStyles.Integer;

    public override bool Validate()
    {
        if (Value is not { } value)
        {
            return SetValidationError("Enter a value.");
        }

        return SetValidationError(TryApply(
            getValue(),
            value,
            setValue,
            onChanged));
    }
}

internal sealed class ColorSettingViewModel(
    string name,
    Func<DrawingColor> getValue,
    Action<DrawingColor> setValue,
    Action onChanged)
    : ValueSettingViewModel<Color>(
        name,
        () => ToAvaloniaColor(getValue()),
        value => setValue(ToDrawingColor(value)),
        onChanged)
{
    private static Color ToAvaloniaColor(DrawingColor color) =>
        Color.FromArgb(color.A, color.R, color.G, color.B);

    private static DrawingColor ToDrawingColor(Color color) =>
        DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
}
