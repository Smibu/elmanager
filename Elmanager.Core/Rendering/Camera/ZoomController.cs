using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.Rendering.Camera;

internal class ZoomController
{
    private double MaxDimension => Math.Max(ZoomFillxMax - ZoomFillxMin, ZoomFillyMax - ZoomFillyMin);
    private bool _smoothZoomInProgress;
    private readonly Action _redrawRequested;
    private const double ZoomFillMargin = 0.05;
    private const double MinimumZoom = 0.000001;


    public ZoomController(ElmaCamera camera, Level lev, Action redrawRequested)
    {
        Cam = camera;
        Lev = lev;
        _redrawRequested = redrawRequested;
    }

    internal double CenterX
    {
        get => Cam.CenterX;
        set => Cam.CenterX = value;
    }

    internal double CenterY
    {
        get => Cam.CenterY;
        set => Cam.CenterY = value;
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

    private double ZoomFillxMin => (1 + ZoomFillMargin) * Lev.Bounds.XMin - ZoomFillMargin * Lev.Bounds.XMax;
    private double ZoomFillxMax => (1 + ZoomFillMargin) * Lev.Bounds.XMax - ZoomFillMargin * Lev.Bounds.XMin;
    private double ZoomFillyMin => (1 + ZoomFillMargin) * Lev.Bounds.YMin - ZoomFillMargin * Lev.Bounds.YMax;
    private double ZoomFillyMax => (1 + ZoomFillMargin) * Lev.Bounds.YMax - ZoomFillMargin * Lev.Bounds.YMin;

    public Level Lev { get; set; }

    internal void Zoom(Vector p, bool zoomIn, double zoomFactor, RenderingSettings settings)
    {
        var i = zoomIn ? zoomFactor : 1 / zoomFactor;
        var x = p.X;
        var y = p.Y;
        x -= (x - Cam.CenterX) * i;
        y -= (y - Cam.CenterY) * i;
        PerformZoom(ZoomLevel * i, x, y, settings);
    }

    internal void ZoomFill(RenderingSettings settings, double aspectRatio)
    {
        var levelAspectRatio = (ZoomFillxMax - ZoomFillxMin) / (ZoomFillyMax - ZoomFillyMin);
        var newZoomLevel = (ZoomFillyMax - ZoomFillyMin) / 2;
        if (levelAspectRatio > aspectRatio)
            newZoomLevel = (ZoomFillxMax - ZoomFillxMin) / 2 / aspectRatio;
        PerformZoom(newZoomLevel, (ZoomFillxMax + ZoomFillxMin) / 2, (ZoomFillyMax + ZoomFillyMin) / 2, settings);
    }

    private void PerformZoom(double newZoomLevel, double newCenterX, double newCenterY, RenderingSettings settings)
    {
        if (settings.SmoothZoomEnabled)
            SmoothZoom(newZoomLevel, newCenterX, newCenterY, settings);
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

    private async void SmoothZoom(double newZoomLevel, double newCenterX, double newCenterY, RenderingSettings settings)
    {
        if (_smoothZoomInProgress)
            return;
        _smoothZoomInProgress = true;
        var oldZoomLevel = ZoomLevel;
        var oldCenterX = Cam.CenterX;
        var oldCenterY = Cam.CenterY;
        var zoomTimer = new Stopwatch();
        long elapsedTime = 0;
        zoomTimer.Start();
        var duration = settings.SmoothZoomDuration;
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
