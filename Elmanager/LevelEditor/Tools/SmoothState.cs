using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

internal abstract record SmoothState
{
    internal record PolygonSmooth(Polygon P) : SmoothState;
    internal record AllSmooth : SmoothState;

    public static PolygonSmooth Polygon(Polygon p) => new(p);
    public static AllSmooth All => new();
}