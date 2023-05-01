using System;

namespace Elmanager.Geometry;

internal struct Bounds
{
    public required double XMin;
    public required double XMax;
    public required double YMin;
    public required double YMax;

    public Bounds Max(Bounds o) =>
        new()
        {
            XMax = Math.Max(XMax, o.XMax),
            XMin = Math.Min(XMin, o.XMin),
            YMax = Math.Max(YMax, o.YMax),
            YMin = Math.Min(YMin, o.YMin)
        };
}