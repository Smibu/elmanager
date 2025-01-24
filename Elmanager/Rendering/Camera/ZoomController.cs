using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        x -= (x - (Cam.XMax + Cam.XMin) / 2) * i;
        y -= (y - (Cam.YMax + Cam.YMin) / 2) * i;
        PerformZoom(ZoomLevel * i, x, y, settings);
    }

    internal void ZoomFill(RenderingSettings settings)
    {
        var levelAspectRatio = (ZoomFillxMax - ZoomFillxMin) / (ZoomFillyMax - ZoomFillyMin);
        var newZoomLevel = (ZoomFillyMax - ZoomFillyMin) / 2;
        if (levelAspectRatio > Cam.AspectRatio)
            newZoomLevel = (ZoomFillxMax - ZoomFillxMin) / 2 / Cam.AspectRatio;
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
        var oldCenterX = (Cam.XMax + Cam.XMin) / 2;
        var oldCenterY = (Cam.YMax + Cam.YMin) / 2;
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

    internal void ZoomToSelection(List<Polygon> selectedPolygons, List<LevObject> selectedObjects, List<GraphicElement> selectedTextures, RenderingSettings settings)
    {
        if (!selectedPolygons.Any() && !selectedObjects.Any() && !selectedTextures.Any())
        {
            return; // No selection to zoom to
        }

        // Initialize the bounding box with extreme values
        double xMin = double.MaxValue;
        double xMax = double.MinValue;
        double yMin = double.MaxValue;
        double yMax = double.MinValue;

        // Update the bounding box with selected polygons
        foreach (var polygon in selectedPolygons)
        {
            foreach (var vertex in polygon.Vertices)
            {
                if (vertex.X < xMin) xMin = vertex.X;
                if (vertex.X > xMax) xMax = vertex.X;
                if (vertex.Y < yMin) yMin = vertex.Y;
                if (vertex.Y > yMax) yMax = vertex.Y;
            }
        }

        // Update the bounding box with selected objects
        foreach (var obj in selectedObjects)
        {
            var pos = obj.Position;
            // add a margin to the bounding box of the object

            var objectRadius = 0.4;
            xMin = Math.Min(xMin, pos.X - objectRadius);
            xMax = Math.Max(xMax, pos.X + objectRadius);
            yMin = Math.Min(yMin, pos.Y - objectRadius);
            yMax = Math.Max(yMax, pos.Y + objectRadius);
        }

        // Update the bounding box with selected textures
        foreach (var texture in selectedTextures)
        {
            var pos = texture.Position;
            xMin = Math.Min(xMin, pos.X);
            xMax = Math.Max(xMax, pos.X + texture.Width);
            yMin = Math.Min(yMin, pos.Y - texture.Height);
            yMax = Math.Max(yMax, pos.Y);
        }

        // Define a margin as a percentage of the bounding box dimensions
        const double marginPercentage = 0.02; // 2% margin
        double xMargin = (xMax - xMin) * marginPercentage;
        double yMargin = (yMax - yMin) * marginPercentage;

        // Adjust the bounding box by adding the margin
        xMin -= xMargin;
        xMax += xMargin;
        yMin -= yMargin;
        yMax += yMargin;

        // Calculate the center of the bounding box
        double centerX = (xMin + xMax) / 2;
        double centerY = (yMin + yMax) / 2;

        // Calculate the new zoom level to fit the bounding box
        double width = xMax - xMin;
        double height = yMax - yMin;
        double newZoomLevel = Math.Max(width / Cam.AspectRatio, height) / 2;

        // Apply the zoom and center the camera
        PerformZoom(newZoomLevel, centerX, centerY, settings);
    }
}