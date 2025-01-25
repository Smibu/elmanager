namespace Elmanager.LevelEditor.ShapeGallery;

using Elmanager.Lev;
using Elmanager.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

internal static class CustomShapeSerializer
{
    public static string SerializeShapeData(List<Polygon> polygons, List<LevObject> objects, List<GraphicElement> graphicElements)
    {
        var shapeDataDto = new ShapeDataDto(polygons, objects, graphicElements);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new GraphicElementDtoConverter(), new VectorDtoConverter() }
        };

        return JsonSerializer.Serialize(shapeDataDto, options);
    }

    public static ShapeDataDto DeserializeShapeData(string json)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters = { new GraphicElementDtoConverter(), new VectorDtoConverter() }
        };

        var shapeData = JsonSerializer.Deserialize<ShapeDataDto>(json, options);
        if (shapeData == null)
        {
            throw new InvalidOperationException("Failed to deserialize ShapeDataDto.");
        }

        return shapeData;
    }

    public static ShapeDataDto DeserializeShapeDataLev(string filePath)
    {
        if (filePath.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            var level = Level.FromPath(filePath).Obj;

            var polygons = level.Polygons;
            var objects = level.Objects.Where(o => o.Type != ObjectType.Start).ToList();
            var graphicElements = level.GraphicElements;

            return new ShapeDataDto(polygons, objects, graphicElements);
        }

        throw new InvalidOperationException("Failed to deserialize Level.");
    }
}