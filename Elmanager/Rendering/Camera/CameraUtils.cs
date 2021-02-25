using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;

namespace Elmanager.Rendering.Camera
{
    internal static class CameraUtils
    {
        private static bool _scrollInProgress;
        public static bool AllowScroll;

        internal static void BeginArrowScroll(Action render, ZoomController zoomCtrl)
        {
            if (_scrollInProgress)
                return;
            _scrollInProgress = true;
            AllowScroll = true;
            var timer = new Stopwatch();
            timer.Start();
            long lastTime = timer.ElapsedMilliseconds;
            while ((Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Left) ||
                    Keyboard.IsKeyDown(Key.Right)) && AllowScroll)
            {
                long timeDelta = timer.ElapsedMilliseconds - lastTime;
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    zoomCtrl.CenterY += timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    zoomCtrl.CenterY -= timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    zoomCtrl.CenterX += timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Left))
                {
                    zoomCtrl.CenterX -= timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                lastTime = timer.ElapsedMilliseconds;
                render();
                Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
            }

            timer.Stop();
            _scrollInProgress = false;
        }
    }
}