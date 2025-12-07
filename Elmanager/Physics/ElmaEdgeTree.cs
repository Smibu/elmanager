using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Elmanager.Geometry;

namespace Elmanager.Physics;

internal class ElmaEdgeTree : IElmaEdgeTree
{
    private List<Edge>?[] _area = null!;
    private Vector _bottomLeft;
    private int _xdim;
    private int _ydim;
    private double _scale;

    public void Init(IEnumerable<Edge> edges, double radius)
    {
        var (xmin, xmax, ymin, ymax) = edges.Aggregate(
            (double.MaxValue, double.MinValue, double.MaxValue, double.MinValue),
            (t, e) =>
            {
                var (xmin, xmax, ymin, ymax) = t;
                return (Math.Min(xmin, e.XMin), Math.Max(xmax, e.XMax), Math.Min(ymin, e.YMin),
                    Math.Max(ymax, e.YMax));
            });
        var bottomLeft = new Vector(xmin - 6.0, ymin - 6.0);
        var scale = 1.0;
        var xdim = (int)((xmax + 6.0 - bottomLeft.X) / scale + 1.0);
        var ydim = (int)((ymax + 6.0 - bottomLeft.Y) / scale + 1.0);
        var area = new List<Edge>?[ydim * xdim];
        foreach (var edge in edges)
        {
            var startRel = (edge.From - bottomLeft) * (1.0 / scale);
            var direction = edge.Direction;
            direction *= 1.0 / scale;
            bool tmp1;
            if (Math.Abs(direction.X) < Math.Abs(direction.Y))
            {
                direction = new Vector(direction.Y, direction.X);
                startRel = new Vector(startRel.Y, startRel.X);
                tmp1 = true;
            }
            else
            {
                tmp1 = false;
            }

            if (direction.X < 0.0)
            {
                startRel += direction;
                direction = new Vector() - direction;
            }

            var x = 0;
            var slope = direction.Y / direction.X;
            var tmp2 = startRel.Y - direction.Y / direction.X * startRel.X;
            var tmp3 = 1.5 / scale * radius;
            if (startRel.X - tmp3 > 0.0)
            {
                x = (int)(startRel.X - tmp3);
            }

            Debug.Assert(startRel.X + direction.X + tmp3 >= 0.0);
            var tmp4 = (int)(startRel.X + direction.X + tmp3);
            while (x <= tmp4)
            {
                var low = (x) * slope + tmp2;
                var high = (x + 1) * slope + tmp2;
                if (low > high)
                {
                    var tmp = low;
                    low = high;
                    high = tmp;
                }

                low = low - tmp3;
                high = high + tmp3;
                Debug.Assert(high >= 0.0);
                var y = low > 0.0 ? (int)low : 0;
                var highu = (int)high;
                while (y <= highu)
                {
                    var index = tmp1 ? y + xdim * x : x + xdim * y;
                    (area[index] ??= new List<Edge>()).Add(edge);
                    y += 1;
                }

                x += 1;
            }
        }

        this._area = area;
        this._bottomLeft = bottomLeft;
        this._xdim = xdim;
        this._ydim = ydim;
        this._scale = scale;
    }

    private List<Edge>? GetEdgesAtLocation(Vector loc)
    {
        Debug.Assert(_scale != 0);
        var p = (loc - _bottomLeft) * (1.0 / _scale);
        var x = p.X > 0.0 ? (int)p.X : 0;
        var y = p.Y > 0.0 ? (int)p.Y : 0;
        if (x >= _xdim)
        {
            x = _xdim - 1;
        }

        if (y >= _ydim)
        {
            y = _ydim - 1;
        }

        return _area[x + y * _xdim];
    }

    public (Vector?, Vector?) GetTouchingEdges(Vector location, double radius)
    {
        var edges = GetEdgesAtLocation(location);
        return edges is null ? (null, null) : EdgeTree.FilterEdges(edges, location, radius);
    }
}