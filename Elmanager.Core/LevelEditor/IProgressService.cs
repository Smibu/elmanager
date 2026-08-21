using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elmanager.LevelEditor;

public interface IProgressService
{
    Task<List<T>?> RunWithProgress<T>(Func<IProgress<double>, CancellationToken, IEnumerable<T>> work);
}
