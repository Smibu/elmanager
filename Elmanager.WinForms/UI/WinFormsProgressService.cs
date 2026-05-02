using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Elmanager.LevelEditor;

namespace Elmanager.UI;

internal class WinFormsProgressService : IProgressService
{
    private readonly Form _owner;

    internal WinFormsProgressService(Form owner)
    {
        _owner = owner;
    }

    public async Task<T?> RunWithProgress<T>(Task<T> task, CancellationTokenSource src, Progress<double> progress) where T : class
    {
        var progressForm = new ProgressDialog(task, src, progress);
        _owner.BeginInvoke(() => { progressForm.ShowDialog(); });
        try
        {
            return await task;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}
