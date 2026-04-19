using Elmanager.Lev;

namespace Elmanager.LevelEditor;

internal abstract record HighlightTarget
{
    internal record PolygonTarget(Polygon Polygon) : HighlightTarget;
    internal record VertexTarget(Polygon Polygon, int VertexIndex) : HighlightTarget;
    internal record ObjectTarget(int ObjectIndex) : HighlightTarget;
    internal record GraphicElementTarget(int GraphicElementIndex) : HighlightTarget;
    internal record PlayerTarget : HighlightTarget;
}
