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

namespace Elmanager.Geometry
{
    internal static class GeometryUtils
    {
        private const int RoundingPrecision = 10;

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

        internal static Polygon Connect(Polygon p1, Polygon p2, Vector v1, Vector v2, double connectRadius)
        {
            if (p1.IntersectsWith(p2))
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
                if (isectPoint is {} p)
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
            while (!(p1Index == 0 && p1CCurrent && result.Count > 0))
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
                    p1Index = p1Index % p1C.Count;
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

        /// <summary>
        ///   Determines whether two line segments intersect.
        /// </summary>
        /// <param name = "a1">Start point of first segment</param>
        /// <param name = "a2">End point of first segment</param>
        /// <param name = "b1">Start point of second segment</param>
        /// <param name = "b2">End point of second segment</param>
        /// <returns></returns>
        internal static bool SegmentsIntersect(Vector a1, Vector a2, Vector b1, Vector b2)
        {
            double b2Minusb1X = b2.X - b1.X;
            double b2Minusb1Y = b2.Y - b1.Y;
            double a2Minusa1X = a2.X - a1.X;
            double a2Minusa1Y = a2.Y - a1.Y;
            double a1Minusb1X = a1.X - b1.X;
            double a1Minusb1Y = a1.Y - b1.Y;
            if (a2Minusa1X == 0 && a2Minusa1Y == 0 || b2Minusb1X == 0 && b2Minusb1Y == 0)
            {
                return false;
            }

            double uaT = b2Minusb1X * a1Minusb1Y - b2Minusb1Y * a1Minusb1X;
            double ubT = a2Minusa1X * a1Minusb1Y - a2Minusa1Y * a1Minusb1X;
            double uB = b2Minusb1Y * a2Minusa1X - b2Minusb1X * a2Minusa1Y;
            if (Math.Round(uB, RoundingPrecision) != 0)
            {
                double ua = uaT / uB;
                double ub = ubT / uB;
                return 0 <= ua && ua <= 1 && 0 <= ub && ub <= 1;
                //Intersection point is
                //X = a1.x + ua * (a2.x - a1.x);
                //Y = a1.y + ua * (a2.y - a1.y);
            }

            if (Math.Round(ubT, RoundingPrecision) != 0)
                return false;
            return Math.Round(uaT, RoundingPrecision) != 0;
        }

        internal static Vector? GetIntersectionPoint(Vector a1, Vector a2, Vector b1, Vector b2)
        {
            double b2Minusb1X = b2.X - b1.X;
            double b2Minusb1Y = b2.Y - b1.Y;
            double a2Minusa1X = a2.X - a1.X;
            double a2Minusa1Y = a2.Y - a1.Y;
            double a1Minusb1X = a1.X - b1.X;
            double a1Minusb1Y = a1.Y - b1.Y;
            double uaT = b2Minusb1X * a1Minusb1Y - b2Minusb1Y * a1Minusb1X;
            double ubT = a2Minusa1X * a1Minusb1Y - a2Minusa1Y * a1Minusb1X;
            double uB = b2Minusb1Y * a2Minusa1X - b2Minusb1X * a2Minusa1Y;
            if (Math.Round(uB, RoundingPrecision) != 0)
            {
                double ua = uaT / uB;
                double ub = ubT / uB;
                if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
                    return new Vector(a1.X + ua * (a2.X - a1.X), a1.Y + ua * (a2.Y - a1.Y));
                return null;
            }

            return null;
        }

        internal static double DistanceFromSegment(double ax, double ay, double bx, double by, double px, double py)
        {
            double aMinusBx = ax - bx;
            double aMinusBy = ay - @by;
            double aMinusBMinusPx = aMinusBx - px;
            double aMinusBMinusPy = aMinusBy - py;
            double aMinusBLengthSquared = aMinusBx * aMinusBx + aMinusBy * aMinusBy;
            double v1 = ax * aMinusBMinusPx + ay * aMinusBMinusPy;
            double v2 = bx * px + @by * py;
            double r = (v1 + v2) / aMinusBLengthSquared;
            if (r >= 0 && r <= 1)
            {
                double v3 = px - ax + r * aMinusBx;
                double v4 = py - ay + r * aMinusBy;
                return Math.Sqrt(v3 * v3 + v4 * v4);
            }

            double v5 = px - ax;
            double v6 = py - ay;
            double v7 = Math.Sqrt(v5 * v5 + v6 * v6);
            double v8 = px - bx;
            double v9 = py - @by;
            double v10 = Math.Sqrt(v8 * v8 + v9 * v9);
            return Math.Min(v7, v10);
        }

        /// <summary>
        ///   Gets the distance between a straight line and a point in a 2D plane.
        /// </summary>
        /// <param name = "ax">X-coordinate of the first point that defines the line.</param>
        /// <param name = "ay">Y-coordinate of the first point that defines the line.</param>
        /// <param name = "bx">X-coordinate of the second point that defines the line.</param>
        /// <param name = "by">Y-coordinate of the second point that defines the line.</param>
        /// <param name = "px">X-coordinate of the point.</param>
        /// <param name = "py">Y-coordinate of the point.</param>
        /// <returns>The distance between the line and the point.</returns>
        internal static double DistanceFromLine(double ax, double ay, double bx, double by, double px, double py)
        {
            double aMinusBx = ax - bx;
            double aMinusBy = ay - @by;
            double aMinusBMinusPx = aMinusBx - px;
            double aMinusBMinusPy = aMinusBy - py;
            double aMinusBLengthSquared = aMinusBx * aMinusBx + aMinusBy * aMinusBy;
            double v1 = ax * aMinusBMinusPx + ay * aMinusBMinusPy;
            double v2 = bx * px + @by * py;
            double r = (v1 + v2) / aMinusBLengthSquared;
            double v3 = px - ax + r * aMinusBx;
            double v4 = py - ay + r * aMinusBy;
            return Math.Sqrt(v3 * v3 + v4 * v4);
        }

        internal static double DistanceFromLine(Vector a, Vector b, Vector p)
        {
            return DistanceFromLine(a.X, a.Y, b.X, b.Y, p.X, p.Y);
        }

        internal static Vector OrthogonalProjection(double ax, double ay, double bx, double by, double px, double py)
        {
            double aMinusBx = ax - bx;
            double aMinusBy = ay - @by;
            double aMinusBMinusPx = aMinusBx - px;
            double aMinusBMinusPy = aMinusBy - py;
            double aMinusBLengthSquared = aMinusBx * aMinusBx + aMinusBy * aMinusBy;
            double v1 = ax * aMinusBMinusPx + ay * aMinusBMinusPy;
            double v2 = bx * px + @by * py;
            double r = (v1 + v2) / aMinusBLengthSquared;
            return new Vector(ax - r * aMinusBx, ay - r * aMinusBy);
        }

        internal static Vector OrthogonalProjection(Vector a, Vector b, Vector p)
        {
            return OrthogonalProjection(a.X, a.Y, b.X, b.Y, p.X, p.Y);
        }

        internal static void Decompose(List<Polygon> polygons)
        {
            for (int i = 0; i < polygons.Count; i++)
            {
                Polygon poly = polygons[i];
                if (poly.Count <= 3)
                    continue;
                int firstDiagonal = 0;
                int secondDiagonal = 2;
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
            var triangulatedPoly = new List<Polygon> {new(polygon)};
            Decompose(triangulatedPoly);
            var triangulatedPolyArray = new Vector[triangulatedPoly.Count][];
            for (int i = 0; i < triangulatedPoly.Count; i++)
                triangulatedPolyArray[i] = triangulatedPoly[i].Vertices.ToArray();
            return triangulatedPolyArray;
        }

        internal static List<Vector> GetIntersectionPoints(List<Polygon> polygons)
        {
            var f = GeometryFactory.Floating;
            var iarray = polygons.Where(poly => !poly.IsGrass).Select(p => p.ToIPolygon()).ToArray();
            var multipoly = f.CreateMultiPolygon(iarray);
            var isects = iarray.Where(p => !p.IsValid).Select(p =>
            {
                var validOp = new IsValidOp(p).ValidationError.Coordinate;
                return new Vector(validOp.X, validOp.Y);
            }).ToList();
            if (!multipoly.IsValid)
            {
                var validOp = new IsValidOp(multipoly).ValidationError;
                if (validOp.ErrorType == TopologyValidationErrors.SelfIntersection)
                {
                    isects.Add(new Vector(validOp.Coordinate.X, validOp.Coordinate.Y));
                }
            }

            var vectors = polygons.SelectMany(p => p.Vertices);
            var query = vectors.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key);
            isects.AddRange(query);
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
            if (multipoly.IsEmpty)
            {
                return multipoly;
            }

            NetTopologySuite.Geometries.Geometry SymDiff(NetTopologySuite.Geometries.Geometry g, NetTopologySuite.Geometries.Geometry p) => p.SymmetricDifference(g);
            var selection = multipoly.Geometries.Aggregate(SymDiff);
            return selection;
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
                    NetTopologySuite.Geometries.Geometry bestRect = null;
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
                        throw new PolygonException(errorMsg);
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

        public const double Tolerance = 0.00000001;
    }
}