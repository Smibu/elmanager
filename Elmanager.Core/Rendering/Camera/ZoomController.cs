using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.Rendering.Camera;

public class ZoomController
{
    private bool _smoothZoomInProgress;
    private readonly Action _redrawRequested;
    private const double ZoomFillMargin = 0.05;
    private const double MinimumZoom = 0.000001;


    public ZoomController(ElmaCamera camera, Action redrawRequested)
    {
        Cam = camera;
        _redrawRequested = redrawRequested;
    }

    public double CenterX
    {
        get => Cam.CenterX;
        set => Cam.CenterX = value;
    }

    public double CenterY
    {
        get => Cam.CenterY;
        set => Cam.CenterY = value;
    }

    public double ZoomLevel
    {
        get => Cam.ZoomLevel;
        set
        {
            if (value < MinimumZoom)
                value = MinimumZoom;
            Cam.ZoomLevel = value;
        }
    }

    public ElmaCamera Cam { get; }

    public void ZoomFill(RenderingSettings settings, double aspectRatio, Level lev)
    {
        var xMin = (1 + ZoomFillMargin) * lev.Bounds.XMin - ZoomFillMargin * lev.Bounds.XMax;
        var xMax = (1 + ZoomFillMargin) * lev.Bounds.XMax - ZoomFillMargin * lev.Bounds.XMin;
        var yMin = (1 + ZoomFillMargin) * lev.Bounds.YMin - ZoomFillMargin * lev.Bounds.YMax;
        var yMax = (1 + ZoomFillMargin) * lev.Bounds.YMax - ZoomFillMargin * lev.Bounds.YMin;
        var levelAspectRatio = (xMax - xMin) / (yMax - yMin);
        var newZoomLevel = (yMax - yMin) / 2;
        if (levelAspectRatio > aspectRatio)
            newZoomLevel = (xMax - xMin) / 2 / aspectRatio;
        PerformZoom(newZoomLevel, (xMax + xMin) / 2, (yMax + yMin) / 2, settings);
    }

    public void Zoom(Vector p, bool zoomIn, double zoomFactor, RenderingSettings settings)
    {
        var i = zoomIn ? zoomFactor : 1 / zoomFactor;
        var x = p.X;
        var y = p.Y;
        x -= (x - Cam.CenterX) * i;
        y -= (y - Cam.CenterY) * i;
        PerformZoom(ZoomLevel * i, x, y, settings);
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
