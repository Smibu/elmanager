using System;
using Elmanager.Geometry;
using NetTopologySuite.Geometries;

namespace Elmanager.Physics;

internal class Edge
{
    public Vector From;
    public Vector To;
    public Vector Direction;
    public Vector NormalizedDirection;
    public double Length;

    public Edge(Vector v1, Vector v2)
    {
        var dir = v2 - v1;
        var len = dir.Length;
        Direction = dir;
        From = v1;
        To = v2;
        NormalizedDirection = dir.Unit();
        Length = len;
    }

    public double XMin => Math.Min(From.X, To.X);
    public double XMax => Math.Max(From.X, To.X);
    public double YMin => Math.Min(From.Y, To.Y);
    public double YMax => Math.Max(From.Y, To.Y);

    public Envelope Envelope => new(From, To);

    public Vector? GetTouchPoint(Vector midpoint, double radius)
    {
        var proj = (midpoint - From).Dotp(NormalizedDirection);
        if (proj < 0.0)
        {
            if ((midpoint - From).Length < radius)
            {
                return From;
            }

            return null;
        }

        if (proj <= Length)
        {
            var v = (midpoint - From).Dotp(NormalizedDirection.Ortho());
            if (v >= -radius && v <= radius)
            {
                return From + NormalizedDirection * proj;
            }
        }
        else if ((midpoint - To).Length < radius)
        {
            return To;
        }

        return null;
    }

    public override string ToString()
    {
        return $"{From} -> {To}";
    }
}