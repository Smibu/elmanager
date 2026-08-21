using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Elmanager.SLE.Editor;

public sealed class ExceptionNotification(string message, string details)
{
    public string Message { get; } = message;
    public string Details { get; } = details;
}

public partial class MainView
{
    private readonly ObservableCollection<ExceptionNotification> _exceptionNotifications = [];

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandledException;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispatcher.UIThread.UnhandledException -= OnDispatcherUnhandledException;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnDispatcherUnhandledException(
        object? sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        LogException(e.Exception);
    }

    private void LogException(Exception exception, string? message = null)
    {
        var details = message is null
            ? exception.ToString()
            : $"{message}{Environment.NewLine}{exception}";
        Console.WriteLine(details);

        var exceptionMessage = exception.GetBaseException().Message;
        var toastMessage = message is null
            ? exceptionMessage
            : $"{message}{Environment.NewLine}{exceptionMessage}";

        if (Dispatcher.UIThread.CheckAccess())
        {
            ShowExceptionNotification(toastMessage, details);
        }
        else
        {
            Dispatcher.UIThread.Post(() => ShowExceptionNotification(toastMessage, details));
        }
    }

    private void ShowExceptionNotification(string message, string details) =>
        _exceptionNotifications.Add(new ExceptionNotification(message, details));

    private void OnDismissExceptionClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: ExceptionNotification notification })
        {
            _exceptionNotifications.Remove(notification);
        }
    }
}
