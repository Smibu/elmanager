using System.Collections.Generic;
using System.Linq;
using Elmanager.Lev;

namespace Elmanager.LevelEditor.ShapeGallery;

internal class PolygonDto
{
    public List<VectorDto> Vertices { get; set; } = new();
    public bool IsGrass { get; set; }

    public PolygonDto()
    {

    }

    public PolygonDto(Polygon polygon)
    {
        Vertices = polygon.Vertices.Select(v => new VectorDto(v)).ToList();
        IsGrass = polygon.IsGrass;
    }

    public Polygon ToPolygon()
    {
        var polygon = new Polygon(Vertices.Select(v => v.ToVector()).ToList());
        polygon.IsGrass = IsGrass;
        return polygon;
    }
}