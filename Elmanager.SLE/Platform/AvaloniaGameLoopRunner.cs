using System;
using System.Threading.Tasks;
using Elmanager.LevelEditor.Playing;

namespace Elmanager.SLE.Platform;

internal sealed class AvaloniaGameLoopRunner(Action requestNextFrame) : IGameLoopRunner
{
    private RunningLoop? _loop;

    private sealed record RunningLoop(TaskCompletionSource Completion, Func<bool> ShouldStop, Action Tick);

    public Task Run(Func<bool> shouldStop, Action tick)
    {
        if (_loop is not null)
        {
            throw new InvalidOperationException("The game loop is already running.");
        }

        var loop = new RunningLoop(
            new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously), shouldStop, tick);
        _loop = loop;
        requestNextFrame();
        return loop.Completion.Task;
    }

    public bool RunFrame()
    {
        if (_loop is not { } loop)
        {
            return false;
        }

        if (loop.ShouldStop())
        {
            _loop = null;
            loop.Completion.TrySetResult();
            return false;
        }

        try
        {
            loop.Tick();
        }
        catch (Exception ex)
        {
            _loop = null;
            loop.Completion.TrySetException(ex);
            return false;
        }

        if (!loop.ShouldStop())
        {
            return true;
        }

        _loop = null;
        loop.Completion.TrySetResult();
        return false;
    }
}
