using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Elmanager.LevelEditor;
using Elmanager.SLE.Dialogs;

namespace Elmanager.SLE.Platform;

internal class AvaloniaProgressService : IProgressService
{
    public async Task<List<T>?> RunWithProgress<T>(Func<IProgress<double>, CancellationToken, IEnumerable<T>> work)
    {
        var src = new CancellationTokenSource();
        var progress = new Progress<double>();

        await YieldToRendering();
        var task = EnumerateCooperatively(work, progress, src.Token);
        var dialog = new ProgressDialog(task, src, progress);
        await dialog.ShowAsync();

        try
        {
            return await task;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private static async Task<List<T>> EnumerateCooperatively<T>(
        Func<IProgress<double>, CancellationToken, IEnumerable<T>> work,
        IProgress<double> progress,
        CancellationToken token)
    {
        await YieldToRendering();

        var list = new List<T>();
        foreach (var item in work(progress, token))
        {
            list.Add(item);
            await YieldToRendering();
        }

        return list;
    }

    private static async Task YieldToRendering() =>
        await Dispatcher.UIThread.InvokeAsync(
            static () => { },
            DispatcherPriority.Background);
}
