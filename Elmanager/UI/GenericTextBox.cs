using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class GenericTextBox<T> : TextBox where T : struct
{
    private T _defaultValue;
    private Func<string, T>? _parseInput;

    protected GenericTextBox(Func<string, T> parser)
    {
        TextChanged += ValidateInput;
        SetParser(parser);
    }

    [Description("Gets or sets the default value.")]
    public T DefaultValue
    {
        get => _defaultValue;
        set => _defaultValue = value;
    }

    public T Value
    {
        get
        {
            if (IsInputValid() && _parseInput != null)
                return _parseInput(Text);
            return _defaultValue;
        }
    }

    private bool IsInputValid()
    {
        try
        {
            _parseInput?.Invoke(Text);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void SetParser(Func<string, T>? val)
    {
        if (val == null)
            return;
        _parseInput = val;
    }

    private void ValidateInput(object? sender, EventArgs eventArgs)
    {
        BackColor = IsInputValid() ? SystemColors.Window : Color.Red;
    }
}