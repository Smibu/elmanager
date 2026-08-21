using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal partial class BrowserWarningDialog : BaseDialog<bool>
{
    public BrowserWarningDialog() => InitializeComponent();

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close(true);
}
