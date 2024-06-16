using System.Collections.Generic;
using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

internal record TransientElements(List<Polygon> Polygons, List<LevObject> Objects, List<GraphicElement> GraphicElements)
{
    public static TransientElements Empty => new(new List<Polygon>(), new List<LevObject>(), new List<GraphicElement>());
    public static TransientElements FromPolygons(List<Polygon> polygons) => new(polygons, new List<LevObject>(), new List<GraphicElement>());
    public static TransientElements FromGraphicElements(List<GraphicElement> graphicElements) => new(new List<Polygon>(), new List<LevObject>(), graphicElements);
    public static TransientElements FromObjects(List<LevObject> objects) => new(new List<Polygon>(), objects, new List<GraphicElement>());
}
