using System.Collections.Generic;
using Elmanager.Lev;

namespace Elmanager.Rendering;

internal record TransientElements(List<Polygon> Polygons, List<LevObject> Objects, List<GraphicElement> GraphicElements)
{
    public static readonly TransientElements Empty = new(new List<Polygon>(), new List<LevObject>(), new List<GraphicElement>());
    public static TransientElements FromPolygons(List<Polygon> polygons) => new(polygons, new List<LevObject>(), new List<GraphicElement>());
    public static TransientElements FromGraphicElements(List<GraphicElement> graphicElements) => new(new List<Polygon>(), new List<LevObject>(), graphicElements);
    public static TransientElements FromObjects(List<LevObject> objects) => new(new List<Polygon>(), objects, new List<GraphicElement>());
}
