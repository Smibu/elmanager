using System.Collections.Generic;
using System.Linq;
using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.ShapeGallery;

internal class ShapeDataDto(Level level)
{
    public Level Level { get; set; } = level;

    public List<Polygon> Polygons { get; set; } = level.Polygons;

    public List<LevObject> Objects { get; set; } = level.Objects.Where(o => o.Type != ObjectType.Start).ToList();

    public List<GraphicElement> GraphicElements { get; set; } = level.GraphicElements;
}