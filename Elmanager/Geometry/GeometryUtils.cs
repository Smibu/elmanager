using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Elmanager.Lev;
using Elmanager.Utilities;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Valid;
using Envelope = NetTopologySuite.Geometries.Envelope;
using Polygon = Elmanager.Lev.Polygon;

namespace Elmanager.Geometry;

internal static class GeometryUtils
{
    internal static IEnumerable<Polygon> ToElmaPolygons(this NetTopologySuite.Geometries.Polygon poly)
    {
        var p = new Polygon(poly.Shell);
        p.UpdateDecomposition();
        yield return p;
        foreach (var linearRing in poly.Holes)
        {
            p = new Polygon(linearRing);
            p.UpdateDecomposition();
            yield return p;
        }
    }

    internal static IEnumerable<NetTopologySuite.Geometries.Polygon> ToIPolygons(this IEnumerable<Polygon> polys)
    {
        return polys.Select(polygon => polygon.ToIPolygon());
    }

    internal static Vector FindPoint(Vector v1, Vector v2, Vector v3, double radius)
    {
        Vector a = v2 - v1;
        Vector b = v3 - v2;
        double angle = (180.0 - a.AngleBetween(b)) / 2;
        if (angle > 0.0 && angle < 180.0)
            return v2 + new Vector(angle + a.Angle) / Math.Sin(angle * MathUtils.DegToRad) * radius;
        return v2 + new Vector(angle + a.Angle) * radius;
    }

    internal static Polygon? Connect(Polygon p1, Polygon p2, Vector v1, Vector v2, double connectRadius)
    {
        if (p1.ToIPolygon().Crosses(p2.ToIPolygon()))
            return null;
        bool isContained = p1.AreaHasPoint(p2[0]) || p2.AreaHasPoint(p1[0]);
        var p1C = new Polygon(p1);
        var p2C = new Polygon(p2);
        if (p1C.IsCounterClockwise)
            p1C.ChangeOrientation();
        if (p2C.IsCounterClockwise ^ isContained)
            p2C.ChangeOrientation();
        int numberOfIntersectionsP1C = 0;
        int numberOfIntersectionsP2C = 0;
        var p1IsectPoint = new Vector();
        var p2IsectPoint = new Vector();
        Vector.MarkDefault = VectorMark.Selected;
        for (int i = 0; i < p1.Vertices.Count; i++)
        {
            var isectPoint = GetIntersectionPoint(p1[i], p1[i + 1], v1, v2);
            if (isectPoint is { } p)
            {
                p1IsectPoint = p;
                p1C.InsertIntersection(p, Tolerance);
                numberOfIntersectionsP1C++;
            }
        }

        if (numberOfIntersectionsP1C != 1)
            return null;
        for (int i = 0; i < p2.Vertices.Count; i++)
        {
            var isectPoint = GetIntersectionPoint(p2[i], p2[i + 1], v1, v2);
            if (isectPoint is { } p)
            {
                p2IsectPoint = p;
                p2C.InsertIntersection(p, Tolerance);
                numberOfIntersectionsP2C++;
            }
        }

        Vector.MarkDefault = VectorMark.None;
        if (numberOfIntersectionsP2C != 1)
            return null;
        var result = new Polygon();
        int p1Index = 0;
        int p2Index = p2C.IndexOf(p2IsectPoint);
        bool p1CCurrent = true;
        var p1ConnectVector = new Vector();
        var p2ConnectVector = new Vector();
        while (!(p1Index == 0 && p1CCurrent && result.Vertices.Count > 0))
        {
            if (p1CCurrent)
            {
                if (p1C[p1Index].Mark != VectorMark.Selected)
                    result.Add(p1C[p1Index]);
                else
                {
                    p1ConnectVector = p1C[p1Index + 1] - p1C[p1Index - 1];
                    p1ConnectVector /= p1ConnectVector.Length;
                    var p1AngleAbs = Math.Abs(p1ConnectVector.AngleBetween(v2 - v1));
                    if (p1AngleAbs < Tolerance)
                    {
                        return null;
                    }

                    p1ConnectVector *= connectRadius /
                                       Math.Sin(p1AngleAbs * MathUtils.DegToRad);
                    double distance1 = (p1C[p1Index + 1] - p1C[p1Index]).Length;
                    double distance2 = (p1C[p1Index] - p1C[p1Index - 1]).Length;
                    if (p1ConnectVector.Length > Math.Min(distance1, distance2))
                        p1ConnectVector = p1ConnectVector / p1ConnectVector.Length *
                                          Math.Min(distance1, distance2) /
                                          2;
                    result.Add(p1IsectPoint - p1ConnectVector);

                    p2ConnectVector = p2C[p2Index + 1] - p2C[p2Index - 1];
                    p2ConnectVector /= p2ConnectVector.Length;
                    var p2AngleAbs = Math.Abs(p2ConnectVector.AngleBetween(v2 - v1));
                    if (p2AngleAbs < Tolerance)
                    {
                        return null;
                    }

                    p2ConnectVector *= connectRadius /
                                       Math.Sin(p2AngleAbs * MathUtils.DegToRad);
                    double dist1 = (p2C[p2Index + 1] - p2C[p2Index]).Length;
                    double dist2 = (p2C[p2Index] - p2C[p2Index - 1]).Length;
                    if (p2ConnectVector.Length > Math.Min(dist1, dist2))
                        p2ConnectVector = p2ConnectVector / p2ConnectVector.Length * Math.Min(dist1, dist2) / 2;
                    result.Add(p2IsectPoint + p2ConnectVector);

                    p2Index++;
                    p1CCurrent = false;
                }

                p1Index++;
                p1Index = p1Index % p1C.Vertices.Count;
            }
            else
            {
                if (p2C[p2Index].Mark != VectorMark.Selected)
                    result.Add(p2C[p2Index]);
                else
                {
                    result.Add(p2IsectPoint - p2ConnectVector);
                    result.Add(p1IsectPoint + p1ConnectVector);
                    p1CCurrent = true;
                }

                p2Index++;
            }
        }

        result.UpdateDecomposition();
        return result;
    }

    internal static Vector? GetIntersectionPoint(Vector a1, Vector a2, Vector b1, Vector b2) =>
        new LineSegment(a1, a2).Intersection(new LineSegment(b1, b2))?.ToVector();

    internal static double DistanceFromSegment(double ax, double ay, double bx, double by, double px, double py) =>
        new LineSegment(new Coordinate(ax, ay), new Coordinate(bx, by)).Distance(new Coordinate(px, py));

    internal static double DistanceFromLine(Vector a, Vector b, Vector p) =>
        new LineSegment(a, b).DistancePerpendicular(p);

    internal static Vector OrthogonalProjection(Vector a, Vector b, Vector p) =>
        new LineSegment(a, b).Project(p).ToVector();

    private static void Decompose(List<Polygon> polygons)
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            var poly = polygons[i];
            if (poly.Vertices.Count <= 3)
                continue;
            const int firstDiagonal = 0;
            const int secondDiagonal = 2;
            var newPoly = new Polygon();
            for (int j = firstDiagonal; j <= secondDiagonal; j++)
                newPoly.Add(poly.Vertices[j]);
            poly.RemoveRange(firstDiagonal + 1, secondDiagonal - firstDiagonal - 1);
            polygons.Add(newPoly);
            i -= 1;
        }
    }

    internal static Vector[][] Decompose(Polygon polygon)
    {
        var triangulatedPoly = new List<Polygon> { new(polygon) };
        Decompose(triangulatedPoly);
        var triangulatedPolyArray = new Vector[triangulatedPoly.Count][];
        for (int i = 0; i < triangulatedPoly.Count; i++)
            triangulatedPolyArray[i] = triangulatedPoly[i].Vertices.ToArray();
        return triangulatedPolyArray;
    }

    internal static List<Vector> GetIntersectionPoints(List<Polygon> polygons)
    {
        var f = GeometryFactory.Floating;
        var iPolygons = polygons.Where(poly => !poly.IsGrass).Select(p => p.ToIPolygon()).ToArray();
        var multipoly = f.CreateMultiPolygon(iPolygons);
        var isects = (from iPolygon in iPolygons
                      select new IsValidOp(iPolygon).ValidationError?.Coordinate
            into validOp
                      where validOp != null
                      select new Vector(validOp.X, validOp.Y)).ToList();
        var validOpMulti = new IsValidOp(multipoly).ValidationError;
        if (validOpMulti is { ErrorType: TopologyValidationErrors.SelfIntersection })
        {
            isects.Add(new Vector(validOpMulti.Coordinate.X, validOpMulti.Coordinate.Y));
        }

        return isects;
    }

    internal static IEnumerable<Polygon> GetSelectedPolygons(this IEnumerable<Polygon> polys,
        bool includeGrass = false)
    {
        return polys.Where(p => (includeGrass || !p.IsGrass) && p.Vertices.Any(v => v.Mark == VectorMark.Selected));
    }

    internal static NetTopologySuite.Geometries.Geometry GetSelectedPolygonsAsMultiPolygon(this IEnumerable<Polygon> px)
    {
        var polys = px.GetSelectedPolygons().ToList();
        var multipoly = GeometryFactory.Floating.CreateMultiPolygon(polys.ToIPolygons().ToArray());
        return multipoly.IsEmpty ? multipoly : multipoly.Geometries.Aggregate((g, p) => p.SymmetricDifference(g));
    }

    internal static IEnumerable<Envelope> FindCovering(this NetTopologySuite.Geometries.Geometry g, IEnumerable<Envelope> rectangles,
        CancellationToken token, IProgress<double> progress, int iterations = 2, double minRectCover = 0.33,
        double minCoverBreak = 0.9)
    {
        var sortedRectangles = rectangles.ToList();
        sortedRectangles.Sort((b, a) => a.Area.CompareTo(b.Area));
        var lastRect = sortedRectangles.Last();
        var f = GeometryFactory.Floating;
        var remaining = f.CreateGeometry(g);

        var centroid = remaining.Centroid;
        // For some reason, the Intersection method throws a TopologyException more easily if the coordinates are near origin,
        // so we translate the geometry further away, and cancel the translation when reporting the final result.
        double translationx = 100.0 - centroid.X;
        double translationy = 100.0 - centroid.Y;
        remaining = AffineTransformation.TranslationInstance(translationx, translationy).Transform(remaining);
        var coveredArea = 0.0;
        var totalArea = remaining.Area;

        while (!remaining.IsEmpty)
        {
            var p = remaining.Coordinate;
            foreach (var c in remaining.Coordinates)
            {
                if (p.Y > c.Y)
                    p = c;
            }

            foreach (var rectangle in sortedRectangles)
            {
                var bestArea = 0.0;
                NetTopologySuite.Geometries.Geometry? bestRect = null;
                var bestxmin = -1.0;
                var bestymin = -1.0;
                var bestxmax = -1.0;
                var bestymax = -1.0;
                const string errorMsg =
                    "Could not texturize the selected polygon(s). Make sure they don't have topology errors.";
                for (var x = -iterations; x <= iterations; x++)
                {
                    rectangle.Translate(p.X - rectangle.Centre.X + x * rectangle.Width / (iterations * 2),
                        p.Y - rectangle.Centre.Y + iterations * rectangle.Height / (iterations * 2));
                    var rectg = f.ToGeometry(rectangle);

                    NetTopologySuite.Geometries.Geometry isect;
                    try
                    {
                        isect = remaining.Intersection(rectg);
                    }
                    catch (TopologyException)
                    {
                        throw new PolygonException(errorMsg);
                    }

                    if (isect.Area > bestArea)
                    {
                        bestArea = isect.Area;
                        bestRect = rectg;
                        bestxmin = rectangle.MinX;
                        bestymin = rectangle.MinY;
                        bestxmax = rectangle.MaxX;
                        bestymax = rectangle.MaxY;
                        if (bestArea / bestRect.Area > minCoverBreak)
                        {
                            x = iterations;
                        }
                    }
                }

                if (bestRect == null)
                {
                    continue;
                }

                if (bestArea / bestRect.Area > minRectCover || rectangle.Equals(lastRect))
                {
                    // we need to call Buffer(0) to remove any possible degenerate parts from the intersection result
                    remaining = remaining.Difference(bestRect).Buffer(0);
                    coveredArea += bestArea;
                    progress.Report(coveredArea / totalArea);
                    yield return new Envelope(
                        bestxmin - translationx,
                        bestxmax - translationx,
                        bestymin - translationy,
                        bestymax - translationy);
                    break;
                }
            }

            token.ThrowIfCancellationRequested();
        }
    }

    public static Vector ToVector(this Coordinate coord) => new(coord.X, coord.Y);

    public const double Tolerance = 0.00000001;
}