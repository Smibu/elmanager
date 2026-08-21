using System;
using System.Threading.Tasks;
using Elmanager.LevelEditor.Playing;

namespace Elmanager.SLE.Platform;

internal sealed class AvaloniaGameLoopRunner(Action requestNextFrame) : IGameLoopRunner
{
    private TaskCompletionSource? _completion;
    private Func<bool>? _shouldStop;
    private Action? _tick;

    public Task Run(Func<bool> shouldStop, Action tick)
    {
        if (_completion is not null)
        {
            throw new InvalidOperationException("The game loop is already running.");
        }

        _shouldStop = shouldStop;
        _tick = tick;
        _completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        requestNextFrame();
        return _completion.Task;
    }

    public bool RunFrame()
    {
        if (_completion is not { } completion)
        {
            return false;
        }

        if (_shouldStop!())
        {
            Clear();
            completion.TrySetResult();
            return false;
        }

        try
        {
            _tick!();
        }
        catch (Exception ex)
        {
            Clear();
            completion.TrySetException(ex);
            return false;
        }

        if (!_shouldStop!())
        {
            return true;
        }

        Clear();
        completion.TrySetResult();
        return false;
    }

    private void Clear()
    {
        _shouldStop = null;
        _tick = null;
        _completion = null;
    }
}
