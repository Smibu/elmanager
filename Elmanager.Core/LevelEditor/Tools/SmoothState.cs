using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

public abstract record SmoothState
{
    public record PolygonSmooth(Polygon P) : SmoothState;
    public record AllSmooth : SmoothState;

    public static PolygonSmooth Polygon(Polygon p) => new(p);
    public static AllSmooth All => new();
}
