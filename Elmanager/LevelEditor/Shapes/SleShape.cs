using Elmanager.Lev;
using Elmanager.Rendering;
using System.Collections.Generic;
using System.Linq;
using System;
using Elmanager.Geometry;
using Elmanager.IO;

namespace Elmanager.LevelEditor.Shapes;

internal class SleShape(Level level)
{
    public Level Level { get; set; } = level;

    public static ElmaFileObject<SleShape> LoadFromPath(string filePath)
    {
        if (!filePath.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($@"Invalid file format: {filePath}. Expected a .lev file.", nameof(filePath));
        }

        Level level;
        try
        {
            level = Level.FromPath(filePath).Obj;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($@"Failed to load level from {filePath}.", ex);
        }

        // Remove start object
        level.Objects = level.Objects.Where(o => o.Type != ObjectType.Start).ToList();

        Vector centerByBounds = new Vector((level.Bounds.XMin + level.Bounds.XMax) / 2, (level.Bounds.YMin + level.Bounds.YMax) / 2);

        // Normalize positions
        List<Polygon> levelPolygons = level.Polygons;
        List<LevObject> levObjects = level.Objects; // No need to filter start object since it is already removed
        List<GraphicElement> levelGraphicElements = level.GraphicElements;

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

        level.UpdateBounds();

        var sleShape = new SleShape(level);
        var elmaFile = new ElmaFile(filePath);

        return new ElmaFileObject<SleShape>(elmaFile, sleShape);
    }
}
