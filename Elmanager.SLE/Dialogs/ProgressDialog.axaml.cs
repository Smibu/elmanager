using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal partial class ProgressDialog : BaseDialog<bool>
{
    private readonly CancellationTokenSource _cancelSrc;
    private readonly Task _task;
    private bool _taskCompleted;

    public ProgressDialog()
    {
        InitializeComponent();
        _cancelSrc = new CancellationTokenSource();
        _task = Task.CompletedTask;
    }

    public ProgressDialog(Task task, CancellationTokenSource cancelSrc, Progress<double> progress)
    {
        InitializeComponent();
        _task = task;
        _cancelSrc = cancelSrc;
        progress.ProgressChanged += OnProgressChanged;
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        try
        {
            await _task;
        }
        catch
        {
        }

        _taskCompleted = true;
        Close(true);
    }

    public override bool OnClosing()
    {
        if (!_taskCompleted)
        {
            _cancelSrc.Cancel();
            return false;
        }

        return true;
    }

    private void OnProgressChanged(object? sender, double value)
    {
        var percent = Math.Clamp(value * 100, 0, 100);
        ProgressBarControl.Value = percent;
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        _cancelSrc.Cancel();
        CancelButton.IsEnabled = false;
        CancelButton.Content = "Canceling...";
    }
}
