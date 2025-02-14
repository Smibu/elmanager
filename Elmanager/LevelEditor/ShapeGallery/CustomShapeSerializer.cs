using System.Collections.Generic;
using System.Linq;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.ShapeGallery;

using Elmanager.Geometry;
using Elmanager.Lev;
using System;

internal static class CustomShapeSerializer
{
    public static ShapeDataDto DeserializeShapeDataLev(string filePath)
    {
        if (filePath.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            Level level = Level.FromPath(filePath).Obj;

            if (level.IsAcrossLevel)
            {
                throw new InvalidOperationException("Cannot load across level shapes.");
            }

            level.Objects = level.Objects.Where(o => o.Type != ObjectType.Start).ToList(); // Remove start object
            level.UpdateBounds();

            //// Cannot use Bounds since it relies on the Start Object, and we are default start position to (0, 0) in shapes
            Vector centerByBounds = new Vector((level.Bounds.XMin + level.Bounds.XMax) / 2, (level.Bounds.YMin + level.Bounds.YMax) / 2);

            // Normalize positions on
            List<Polygon> levelPolygons = level.Polygons;
            List<LevObject> levObjects = level.Objects; // No need to filter start object since it is already removed
            List<GraphicElement> levelGraphicElements = level.GraphicElements;

            // Normalize positions
            foreach (var polygon in levelPolygons)
            {
                for (int i = 0; i < polygon.Vertices.Count; i++)
                {
                    polygon.Vertices[i] = new Vector(polygon.Vertices[i].X - centerByBounds.X, polygon.Vertices[i].Y - centerByBounds.Y);
                }
            }

            foreach (var obj in levObjects)
            {
                obj.Position = new Vector(obj.Position.X - centerByBounds.X, obj.Position.Y - centerByBounds.Y);
            }

            foreach (var graphicElement in levelGraphicElements)
            {
                graphicElement.Position = new Vector(graphicElement.Position.X - centerByBounds.X, graphicElement.Position.Y - centerByBounds.Y);
            }


            return new ShapeDataDto(level);
        }

        throw new InvalidOperationException("Failed to deserialize Level.");
    }
}