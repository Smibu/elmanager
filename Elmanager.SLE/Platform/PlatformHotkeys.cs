using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Elmanager.SLE.Platform;

internal static class PlatformHotkeys
{
    public static KeyModifiers Command =>
        Application.Current?.PlatformSettings?.HotkeyConfiguration.CommandModifiers ?? KeyModifiers.Control;

    public static KeyGesture Redo => Command == KeyModifiers.Control
        ? new KeyGesture(Key.Y, Command)
        : new KeyGesture(Key.Z, Command | KeyModifiers.Shift);
}

internal class CmdExtension(Key key) : MarkupExtension
{
    public bool Shift { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new KeyGesture(key, PlatformHotkeys.Command | (Shift ? KeyModifiers.Shift : KeyModifiers.None));
}

internal class RedoExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => PlatformHotkeys.Redo;
}
