using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor;

public abstract record HighlightTarget
{
    public record PolygonTarget(Polygon Polygon) : HighlightTarget;

    public record VertexTarget(Polygon Polygon, int VertexIndex) : HighlightTarget
    {
        public int PolygonVertexCount { get; } = Polygon.Vertices.Count;
    }

    public record ObjectTarget(LevObject Object) : HighlightTarget;
    public record GraphicElementTarget(GraphicElement GraphicElement) : HighlightTarget;
    public record PlayerTarget : HighlightTarget;

    public bool ExistsIn(Level lev) => this switch
    {
        PolygonTarget pt => lev.Polygons.Contains(pt.Polygon),
        VertexTarget vt => lev.Polygons.Contains(vt.Polygon) &&
                           vt.Polygon.Vertices.Count == vt.PolygonVertexCount,
        ObjectTarget ot => lev.Objects.Contains(ot.Object),
        GraphicElementTarget gt => lev.GraphicElements.Contains(gt.GraphicElement),
        _ => true
    };
}
