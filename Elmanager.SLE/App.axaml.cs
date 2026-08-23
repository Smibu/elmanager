using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Markup.Xaml;
using Elmanager.SLE.Editor;

namespace Elmanager.SLE;

public class App : Application
{
    private static readonly DateTime Version = new(2026, 8, 23);

    internal static string TitleWithVersion(string title) => $"{title} [{Version:dd.MM.yyyy}]";

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        DefaultMenuInteractionHandler.MenuShowDelay = new TimeSpan(0);
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow();
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView();
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
