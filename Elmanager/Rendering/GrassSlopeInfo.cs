using System;
using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal class GrassSlopeInfo
{
    private readonly Dictionary<int, int> _info = new();
    private int _polygonXMin = -1;
    private readonly double _zoom;
    private readonly Bounds _bounds;
    private List<GraphicElement.Picture>? _placed;

    private double Factor => 48.0 * _zoom;

    public GrassSlopeInfo(Polygon polygon, Bounds groundBounds, double grassZoom)
    {
        _zoom = grassZoom;
        _bounds = groundBounds with
        {
            XMin = RoundToPixelMiddle(groundBounds.XMin - 10000.0 / Factor, Factor),
            YMin = RoundToPixelMiddle(groundBounds.YMin - 1000.0 / Factor, Factor)
        };
        var maxEdgeEndIndex = polygon.GrassStart;
        var maxEdgeBeginIndex = maxEdgeEndIndex - 1;
        if (maxEdgeBeginIndex < 0)
        {
            maxEdgeBeginIndex = polygon.Vertices.Count - 1;
        }

        var iterForward = !(polygon.Vertices[maxEdgeBeginIndex].X < polygon.Vertices[maxEdgeEndIndex].X);
        var currX = -1;
        var loopStep = iterForward ? 1 : -1;

        for (var i = 0; i < polygon.Vertices.Count - 1; i++)
        {
            maxEdgeBeginIndex += loopStep + polygon.Vertices.Count;
            maxEdgeEndIndex += loopStep + polygon.Vertices.Count;
            maxEdgeBeginIndex %= polygon.Vertices.Count;
            maxEdgeEndIndex %= polygon.Vertices.Count;

            var i1 = maxEdgeBeginIndex;
            var i2 = maxEdgeEndIndex;
            if (!iterForward)
            {
                (i1, i2) = (i2, i1);
            }

            currX = ProcessEdge(polygon, i1, i2, currX);
        }
    }

    public static double RoundToPixelMiddle(double value, double factor) => ((int)(value * factor) + 0.5) / factor;

    private int ProcessEdge(Polygon polygon, int i1, int i2, int currX)
    {
        var v1 = polygon.Vertices[i1];
        var v2 = polygon.Vertices[i2];

        if (v1.X > v2.X)
        {
            return currX;
        }

        var x1 = (int)((v1.X - _bounds.XMin) * Factor);
        var y1 = (v1.Y - _bounds.YMin) * Factor;
        var x2 = (int)((v2.X - _bounds.XMin) * Factor);
        var y2 = (v2.Y - _bounds.YMin) * Factor;

        var result = currX;

        if (currX < 0)
        {
            _polygonXMin = x1;
            _info[0] = (int)y1;
            result = x1;
        }

        if (x1 < x2 && result >= x1 - 1)
        {
            var xi = x1;
            while (xi <= x2)
            {
                var index = xi - _polygonXMin;
                if (index >= 10000)
                    return result;
                _info[index] = (int)((xi - x1) * (y2 - y1) / (x2 - x1) + y1);
                ++xi;
                result = xi;
            }
        }

        return result;
    }

    public List<GraphicElement.Picture> GetGrassPics(List<GrassPic> pics)
    {
        if (_placed != null)
        {
            return _placed;
        }
        _placed = new List<GraphicElement.Picture>();
        if (pics.Count == 0)
        {
            return _placed;
        }
        var width = _info.Count;
        var startX = _polygonXMin;
        var currX = startX;
        var currY = _info[0];
        var maxX = startX + width;
        while (currX < maxX)
        {
            var bestFit = int.MaxValue;
            GrassPic? bestPic = null;
            foreach (var p in pics)
            {
                var nextX = currX + p.Width;
                var index = nextX < maxX ? nextX - startX : width - 1;
                if (!_info.TryGetValue(index, out var grassPolyY))
                {
                    HasError = true;
                }
                var currentFit = Math.Abs(currY + p.Delta - grassPolyY);
                if (currentFit < bestFit)
                {
                    bestFit = currentFit;
                    bestPic = p;
                }
            }

            if (bestPic is not { } best)
                throw new Exception("Did not find any grass pic?");
            var finalY = best.Delta >= 0 ? currY - 20 : currY + 21 - best.HeightWithoutExtension;
            _placed.Add(new GraphicElement.Picture(ClippingType.Ground, 601,
                new Vector(_bounds.XMin + currX / Factor, _bounds.YMin + finalY / Factor), best.Image));
            currX += best.Width;
            currY += best.Delta;
        }

        return _placed;
    }

    public bool HasError { get; private set; }
}