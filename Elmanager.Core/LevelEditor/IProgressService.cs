using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmanager.LevelEditor;

public interface IProgressService
{
    Task<T?> RunWithProgress<T>(Task<T> task, CancellationTokenSource src, Progress<double> progress) where T : class;
}
