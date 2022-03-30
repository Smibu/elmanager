using System.Collections.Generic;
using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

internal class TransformState
{
    public Polygon OriginalRectangle;
    public readonly List<LevObject> OriginalTransformObjects;
    public readonly List<Polygon> OriginalTransformPolygons;
    public readonly List<GraphicElement> OriginalTransformTextures;
    public int? TransformPolygonIndex;
    public Polygon TransformRectangle;

    public TransformState(List<LevObject> originalTransformObjects, List<Polygon> originalTransformPolygons, List<GraphicElement> originalTransformTextures, Polygon transformRectangle)
    {
        OriginalTransformObjects = originalTransformObjects;
        OriginalTransformPolygons = originalTransformPolygons;
        OriginalTransformTextures = originalTransformTextures;
        TransformRectangle = transformRectangle;
        OriginalRectangle = transformRectangle.Clone();
    }
}