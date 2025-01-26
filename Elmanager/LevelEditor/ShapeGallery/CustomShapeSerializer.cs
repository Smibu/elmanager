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



            return new ShapeDataDto(level);
        }

        throw new InvalidOperationException("Failed to deserialize Level.");
    }
}