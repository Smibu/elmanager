using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<List<T>?> RunWithProgress<T>(Func<IProgress<double>, CancellationToken, IEnumerable<T>> work)
    {
        var src = new CancellationTokenSource();
        var progress = new Progress<double>();
        var task = Task.Factory.StartNew(() => work(progress, src.Token).ToList(), src.Token);
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
