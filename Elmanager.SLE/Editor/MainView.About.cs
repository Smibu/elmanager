using Avalonia.Interactivity;
using Elmanager.SLE.Dialogs;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private async void OnAboutClick(object? sender, RoutedEventArgs e) =>
        await new AboutDialog().ShowAsync();
}
