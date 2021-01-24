using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Elmanager
{
    class ZoomController
    {
        private readonly RenderingSettings _settings;
        private double MaxDimension => Math.Max(ZoomFillxMax - ZoomFillxMin, ZoomFillyMax - ZoomFillyMin);
        private bool _smoothZoomInProgress;
        private readonly Action _redrawRequested;
        private const double ZoomFillMargin = 0.05;
        private const double MinimumZoom = 0.000001;


        public ZoomController(ElmaCamera camera, Level lev, RenderingSettings settings, Action redrawRequested)
        {
            Cam = camera;
            Lev = lev;
            _settings = settings;
            _redrawRequested = redrawRequested;
        }

        internal double CenterX
        {
            get => Cam.CenterX;
            set
            {
                if (value < ZoomFillxMin - Cam.XSize)
                    value = ZoomFillxMin - Cam.XSize;
                if (value > ZoomFillxMax + Cam.XSize)
                    value = ZoomFillxMax + Cam.XSize;
                Cam.CenterX = value;
            }
        }

        internal double CenterY
        {
            get => Cam.CenterY;
            set
            {
                if (value < ZoomFillyMin - Cam.YSize)
                    value = ZoomFillyMin - Cam.YSize;
                if (value > ZoomFillyMax + Cam.YSize)
                    value = ZoomFillyMax + Cam.YSize;
                Cam.CenterY = value;
            }
        }

        internal double ZoomLevel
        {
            get => Cam.ZoomLevel;
            set
            {
                if (value > MaxDimension * 2)
                    value = MaxDimension * 2;
                if (value < MinimumZoom)
                    value = MinimumZoom;
                Cam.ZoomLevel = value;
            }
        }

        internal ElmaCamera Cam { get; }

        private double ZoomFillxMin => (1 + ZoomFillMargin) * Lev.XMin - ZoomFillMargin * Lev.XMax;
        private double ZoomFillxMax => (1 + ZoomFillMargin) * Lev.XMax - ZoomFillMargin * Lev.XMin;
        private double ZoomFillyMin => (1 + ZoomFillMargin) * Lev.YMin - ZoomFillMargin * Lev.YMax;
        private double ZoomFillyMax => (1 + ZoomFillMargin) * Lev.YMax - ZoomFillMargin * Lev.YMin;

        public Level Lev { get; set; }

        internal void Zoom(Vector p, bool zoomIn, double zoomFactor)
        {
            var i = zoomIn ? zoomFactor : 1 / zoomFactor;
            var x = p.X;
            var y = p.Y;
            x -= (x - (Cam.XMax + Cam.XMin) / 2) * i;
            y -= (y - (Cam.YMax + Cam.YMin) / 2) * i;
            PerformZoom(ZoomLevel * i, x, y);
        }

        internal void ZoomFill()
        {
            var levelAspectRatio = (ZoomFillxMax - ZoomFillxMin) / (ZoomFillyMax - ZoomFillyMin);
            var newZoomLevel = (ZoomFillyMax - ZoomFillyMin) / 2;
            if (levelAspectRatio > Cam.AspectRatio)
                newZoomLevel = (ZoomFillxMax - ZoomFillxMin) / 2 / Cam.AspectRatio;
            PerformZoom(newZoomLevel, (ZoomFillxMax + ZoomFillxMin) / 2, (ZoomFillyMax + ZoomFillyMin) / 2);
        }

        internal void ZoomRect(Vector startPoint, Vector endPoint)
        {
            if (!Equals(startPoint, endPoint))
            {
                double x1;
                double x2;
                if (startPoint.X < endPoint.X)
                {
                    x1 = startPoint.X;
                    x2 = endPoint.X;
                }
                else
                {
                    x2 = startPoint.X;
                    x1 = endPoint.X;
                }

                double y1;
                double y2;
                if (startPoint.Y < endPoint.Y)
                {
                    y1 = startPoint.Y;
                    y2 = endPoint.Y;
                }
                else
                {
                    y2 = startPoint.Y;
                    y1 = endPoint.Y;
                }

                var i = (y2 - y1) / 2;
                var rectAspectRatio = (x2 - x1) / (y2 - y1);
                if (rectAspectRatio > Cam.AspectRatio)
                    i = (x2 - x1) / 2 / Cam.AspectRatio;
                PerformZoom(i, (x2 + x1) / 2, (y2 + y1) / 2);
            }
        }

        private void PerformZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (_settings.SmoothZoomEnabled)
                SmoothZoom(newZoomLevel, newCenterX, newCenterY);
            else
            {
                ZoomLevel = newZoomLevel;
                CenterX = newCenterX;
                CenterY = newCenterY;
                RequestRedraw();
            }
        }

        private void RequestRedraw()
        {
            _redrawRequested();
        }

        private async void SmoothZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (_smoothZoomInProgress)
                return;
            _smoothZoomInProgress = true;
            var oldZoomLevel = ZoomLevel;
            var oldCenterX = (Cam.XMax + Cam.XMin) / 2;
            var oldCenterY = (Cam.YMax + Cam.YMin) / 2;
            var zoomTimer = new Stopwatch();
            long elapsedTime = 0;
            zoomTimer.Start();
            var duration = _settings.SmoothZoomDuration;
            while (elapsedTime <= duration)
            {
                ZoomLevel = oldZoomLevel + (newZoomLevel - oldZoomLevel) * elapsedTime / duration;
                CenterX = oldCenterX + (newCenterX - oldCenterX) * elapsedTime / duration;
                CenterY = oldCenterY + (newCenterY - oldCenterY) * elapsedTime / duration;
                RequestRedraw();
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                elapsedTime = zoomTimer.ElapsedMilliseconds;
            }

            zoomTimer.Stop();
            // Draw the last frame separately to make sure the zoom was made correctly
            ZoomLevel = newZoomLevel;
            CenterX = newCenterX;
            CenterY = newCenterY;
            RequestRedraw();

            _smoothZoomInProgress = false;
        }
    }
}
