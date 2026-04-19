using Elmanager.Geometry;

namespace Elmanager.Rendering.Camera;

internal class ElmaCamera
{
    internal double CenterX;
    internal double CenterY;
    internal double ZoomLevel;

    internal Bounds GetBounds(double aspectRatio) => new()
    {
        XMin = CenterX - ZoomLevel * aspectRatio,
        XMax = CenterX + ZoomLevel * aspectRatio,
        YMin = CenterY - ZoomLevel,
        YMax = CenterY + ZoomLevel
    };

    internal Vector FixJitter(int viewPortWidth, int viewPortHeight)
    {
        var aspectRatio = viewPortWidth / (double)viewPortHeight;
        var fixx = CenterX % (2 * ZoomLevel * aspectRatio / viewPortWidth);
        var fixy = CenterY % (2 * ZoomLevel / viewPortHeight);
        CenterX -= fixx;
        CenterY -= fixy;
        return new Vector(fixx, fixy);
    }
}
