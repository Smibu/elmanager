using Elmanager.Lev;

namespace Elmanager.LevelEditor;

public abstract record HighlightTarget
{
    public record PolygonTarget(Polygon Polygon) : HighlightTarget;
    public record VertexTarget(Polygon Polygon, int VertexIndex) : HighlightTarget;
    public record ObjectTarget(int ObjectIndex) : HighlightTarget;
    public record GraphicElementTarget(int GraphicElementIndex) : HighlightTarget;
    public record PlayerTarget : HighlightTarget;
}
