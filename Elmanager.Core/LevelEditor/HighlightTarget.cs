using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor;

public union HighlightTarget(
    HighlightTarget.PolygonTarget,
    HighlightTarget.VertexTarget,
    HighlightTarget.ObjectTarget,
    HighlightTarget.GraphicElementTarget,
    HighlightTarget.PlayerTarget)
{
    public record PolygonTarget(Polygon Polygon);

    public record VertexTarget(Polygon Polygon, int VertexIndex)
    {
        public int PolygonVertexCount { get; } = Polygon.Vertices.Count;
    }

    public record ObjectTarget(LevObject Object);
    public record GraphicElementTarget(GraphicElement GraphicElement);
    public record PlayerTarget;

    public bool ExistsIn(Level lev) => this switch
    {
        PolygonTarget pt => lev.Polygons.Contains(pt.Polygon),
        VertexTarget vt => lev.Polygons.Contains(vt.Polygon) &&
                           vt.Polygon.Vertices.Count == vt.PolygonVertexCount,
        ObjectTarget ot => lev.Objects.Contains(ot.Object),
        GraphicElementTarget gt => lev.GraphicElements.Contains(gt.GraphicElement),
        PlayerTarget => true
    };
}
