using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.LevelEditor.Input;

namespace Elmanager.Rendering.Camera;

internal static class WinFormsArrowScroll
{
    private static bool _scrollInProgress;
    public static bool AllowScroll;

    private static readonly ArrowKeyboardState KeyboardState = new();

    internal static void BeginArrowScroll(Action render, ZoomController zoomCtrl)
    {
        if (_scrollInProgress)
            return;
        _scrollInProgress = true;
        AllowScroll = true;
        var timer = new Stopwatch();
        timer.Start();
        long lastTime = timer.ElapsedMilliseconds;
        bool anyDown;
        do
        {
            long now = timer.ElapsedMilliseconds;
            long timeDelta = now - lastTime;
            lastTime = now;
            anyDown = CameraUtils.StepArrowScroll(timeDelta, KeyboardState, zoomCtrl);
            render();
            Thread.Sleep(1);
            System.Windows.Forms.Application.DoEvents();
        }
        while (anyDown && AllowScroll);
        timer.Stop();
        _scrollInProgress = false;
    }

    private sealed class ArrowKeyboardState : IKeyboardState
    {
        public bool IsKeyDown(ModifierKey key) => false;

        public bool IsKeyDown(EditorKey key) => key switch
        {
            EditorKey.Up => KeyboardUtils.IsKeyDown(Keys.Up),
            EditorKey.Down => KeyboardUtils.IsKeyDown(Keys.Down),
            EditorKey.Left => KeyboardUtils.IsKeyDown(Keys.Left),
            EditorKey.Right => KeyboardUtils.IsKeyDown(Keys.Right),
            _ => false,
        };
    }
}
