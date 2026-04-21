using Elmanager.Geometry;

namespace Elmanager.Rendering.Camera;

public class ElmaCamera
{
    public double CenterX;
    public double CenterY;
    public double ZoomLevel;

    public Bounds GetBounds(double aspectRatio) => new()
    {
        XMin = CenterX - ZoomLevel * aspectRatio,
        XMax = CenterX + ZoomLevel * aspectRatio,
        YMin = CenterY - ZoomLevel,
        YMax = CenterY + ZoomLevel
    };

    public Vector FixJitter(int viewPortWidth, int viewPortHeight)
    {
        var aspectRatio = viewPortWidth / (double)viewPortHeight;
        var fixx = CenterX % (2 * ZoomLevel * aspectRatio / viewPortWidth);
        var fixy = CenterY % (2 * ZoomLevel / viewPortHeight);
        CenterX -= fixx;
        CenterY -= fixy;
        return new Vector(fixx, fixy);
    }
}
