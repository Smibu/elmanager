using System;
using System.Diagnostics;
using System.Threading;
using Elmanager.LevelEditor.Tools.Platform;

namespace Elmanager.Rendering.Camera;

internal static class WinFormsArrowScroll
{
    private static bool _scrollInProgress;
    public static bool AllowScroll;

    private static readonly WinFormsKeyboardState KeyboardState = new();

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
}
