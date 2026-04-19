using System.Collections.Generic;
using Elmanager.Lev;

namespace Elmanager.Rendering;

internal record LevEditState(Level Lev, TransientElements Elements)
{
    public IEnumerable<Polygon> GetPolygons()
    {
        foreach (var poly in Lev.Polygons) yield return poly;
        foreach (var poly in Elements.Polygons) yield return poly;
    }

    public IEnumerable<LevObject> GetObjects()
    {
        foreach (var obj in Lev.Objects) yield return obj;
        foreach (var obj in Elements.Objects) yield return obj;
    }

    public IEnumerable<GraphicElement> GetGraphicElements()
    {
        foreach (var ge in Lev.GraphicElements) yield return ge;
        foreach (var ge in Elements.GraphicElements) yield return ge;
    }
}
