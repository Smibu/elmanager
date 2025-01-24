using Elmanager.Lev;
using System.Collections.Generic;
using System.Linq;
using Elmanager.Rendering;
using Polygon = Elmanager.Lev.Polygon;

namespace Elmanager.LevelEditor.ShapeGallery;

internal class ShapeDataDto
{
    public List<PolygonDto> Polygons { get; set; }
    public List<LevObjectDto> Objects { get; set; }
    public List<GraphicElementDto> GraphicElements { get; set; }

    public ShapeDataDto()
    {
        Polygons = new List<PolygonDto>();
        Objects = new List<LevObjectDto>();
        GraphicElements = new List<GraphicElementDto>();
    }

    public ShapeDataDto(List<Polygon> polygons, List<LevObject> objects, List<GraphicElement> graphicElements)
    {
        Polygons = polygons.Select(p => new PolygonDto(p)).ToList();
        Objects = objects.Select(o => new LevObjectDto(o)).ToList();
        GraphicElements = graphicElements.Select(ge => ge.ToDto()).ToList();
    }
}