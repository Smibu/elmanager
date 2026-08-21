using System.Globalization;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal partial class ZoomDialog : BaseDialog<double>
{
    public ZoomDialog(double initialValue)
    {
        InitializeComponent();
        ZoomBox.Text = initialValue.ToString("F3", CultureInfo.InvariantCulture);
        ZoomBox.SelectAll();
        ZoomBox.TextChanged += (_, _) => UpdateOkButton();
        UpdateOkButton();
    }

    private void UpdateOkButton() => OkButton.IsEnabled = TryParseZoom(out _);

    private bool TryParseZoom(out double value) =>
        double.TryParse(ZoomBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
        && value > 0;

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TryParseZoom(out var value))
        {
            Close(value);
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
}
