using System;
using System.Threading.Tasks;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Tools.Platform;
using Elmanager.UI;

namespace Elmanager.LevelEditor.Playing;

internal class WinFormsGameLoopRunner : IGameLoopRunner
{
    public Task Run(Func<bool> shouldStop, Action tick)
    {
        var tcs = new TaskCompletionSource();

        void OnIdle(object? sender, EventArgs e)
        {
            while (NativeUtils.IsApplicationIdle())
            {
                if (shouldStop())
                {
                    System.Windows.Forms.Application.Idle -= OnIdle;
                    tcs.TrySetResult();
                    return;
                }

                tick();

                if (shouldStop())
                {
                    System.Windows.Forms.Application.Idle -= OnIdle;
                    tcs.TrySetResult();
                    return;
                }
            }
        }

        System.Windows.Forms.Application.Idle += OnIdle;
        return tcs.Task;
    }
}

internal class WinFormsPlayController : PlayController
{
    private static readonly IKeyboardState KeyboardState = new WinFormsKeyboardState();

    public WinFormsPlayController() : base(new WinFormsGameLoopRunner())
    {
    }

    public void UpdateInputKeys() => UpdateInputKeys(KeyboardState);
}
