using System.Collections.Generic;
using Elmanager.Geometry;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Quadtree;

namespace Elmanager.Physics;

internal class EdgeTree : IElmaEdgeTree
{
    private readonly Quadtree<Edge> _tree = new();

    public void Init(IEnumerable<Edge> edges, double radius)
    {
        foreach (var edge in edges)
        {
            _tree.Insert(edge.Envelope, edge);
        }
    }

    public static (Vector?, Vector?) FilterEdges(List<Edge> edges, Vector location, double radius)
    {
        Vector? first = null;
        foreach (var edge in edges)
        {
            var mp = edge.GetTouchPoint(location, radius);
            if (mp is not { } p) continue;
            if (first is { } rp)
            {
                if ((rp - p).Length >= 0.1)
                {
                    return (first, p);
                }
                else
                {
                    first = (rp + p) / 2;
                }
            }
            else
            {
                first = p;
            }
        }

        return (first, null);
    }

    public static bool HasHit(List<Edge> edges, Vector location, double radius)
    {
        foreach (var edge in edges)
        {
            var mp = edge.GetTouchPoint(location, radius);
            if (mp is { }) return true;
        }

        return false;
    }

    public (Vector?, Vector?) GetTouchingEdges(Vector location, double radius)
    {
        return FilterEdges((List<Edge>)_tree.Query(EnvelopeFromCircle(location, radius)), location, radius);
    }

    public static Envelope EnvelopeFromCircle(Vector location, double radius)
    {
        return new(location.X - radius,
            location.X + radius,
            location.Y - radius,
            location.Y + radius);
    }
}