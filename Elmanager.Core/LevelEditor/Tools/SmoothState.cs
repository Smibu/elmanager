using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

public union SmoothState(SmoothState.PolygonSmooth, SmoothState.AllSmooth)
{
    public record PolygonSmooth(Polygon P);
    public record AllSmooth;

    public static PolygonSmooth Polygon(Polygon p) => new(p);
    public static AllSmooth All => new();
}
