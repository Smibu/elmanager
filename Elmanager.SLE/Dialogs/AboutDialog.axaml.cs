using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal partial class AboutDialog : BaseDialog<bool>
{
    public AboutDialog() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();
}
