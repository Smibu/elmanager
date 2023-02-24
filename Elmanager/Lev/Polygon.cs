using System;
using System.Collections.Generic;
using System.Linq;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.LevelEditor;
using Elmanager.UI;
using Elmanager.Utilities;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace Elmanager.Lev;

internal class Polygon
{
    internal Vector[][] Decomposition = null!;
    internal bool IsGrass;
    internal PolygonMark Mark;
    internal List<Vector> Vertices;

    internal const double BufferDistance = -1e-10;

    internal Polygon(IEnumerable<Vector> vertices, bool isGrass = false)
    {
        Vertices = new List<Vector>();
        Vertices.AddRange(vertices);
        IsGrass = isGrass;
    }

    internal Polygon()
    {
        Vertices = new List<Vector>(10);
    }

    internal Polygon(Polygon p)
        : this()
    {
        foreach (Vector x in p.Vertices)
            Vertices.Add(x.Clone());
        Mark = PolygonMark.None;
        IsGrass = p.IsGrass;
        Decomposition = p.Decomposition;
    }

    internal Polygon(params Vector[] vertices)
        : this()
    {
        foreach (Vector x in vertices)
            Vertices.Add(x);
    }

    public Polygon(LinearRing vertices) : this()
    {
        foreach (var v in vertices.Coordinates.Skip(1))
        {
            Vertices.Add(new Vector(v.X, v.Y));
        }
    }

    public Polygon WithYNegated()
    {
        return new(Vertices.Select(v => new Vector(v.X, -v.Y)), IsGrass);
    }

    internal IEnumerable<Vector> VerticesRing
    {
        get
        {
            foreach (var vertex in Vertices)
            {
                yield return vertex;
            }

            yield return Vertices[0];
        }
    }

    internal Vector this[int index] =>
        index < 0 ? Vertices[Vertices.Count + index] : Vertices[index % Vertices.Count];

    private double SignedArea
    {
        get
        {
            double result = Vertices.Select((_, i) => this[i].X * this[i + 1].Y - this[i + 1].X * this[i].Y).Sum();
            return result / 2;
        }
    }

    internal bool IsCounterClockwise => SignedArea > 0;

    internal double XMin
    {
        get
        {
            double result = Vertices[0].X;
            foreach (Vector x in Vertices)
                if (x.X < result)
                    result = x.X;
            return result;
        }
    }

    internal double XMax
    {
        get
        {
            double result = Vertices[0].X;
            foreach (Vector x in Vertices)
                if (x.X > result)
                    result = x.X;
            return result;
        }
    }

    internal double YMax
    {
        get
        {
            double result = Vertices[0].Y;
            foreach (Vector x in Vertices)
                if (x.Y > result)
                    result = x.Y;
            return result;
        }
    }

    internal double YMin
    {
        get
        {
            double result = Vertices[0].Y;
            foreach (Vector x in Vertices)
                if (x.Y < result)
                    result = x.Y;
            return result;
        }
    }

    private bool IsSimple => ToIPolygon().IsSimple;

    internal void UpdateDecomposition(bool updateGrass = true)
    {
        Decomposition = GeometryUtils.Decompose(this);
        if (updateGrass && IsGrass)
        {
            double longest = Math.Abs(Vertices[^1].X - Vertices[0].X);
            int longestIndex = Vertices.Count - 1;
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                double current = Math.Abs(Vertices[i].X - Vertices[i + 1].X);
                if (current > longest)
                {
                    longest = current;
                    longestIndex = i;
                }
            }

            SetBeginPoint(longestIndex + 1);
        }
    }

    internal static Polygon Rectangle(Vector lowerLeftCorner, double width, double height)
    {
        return new(new Vector(lowerLeftCorner.X, lowerLeftCorner.Y),
            new Vector(lowerLeftCorner.X + width, lowerLeftCorner.Y),
            new Vector(lowerLeftCorner.X + width, lowerLeftCorner.Y + height),
            new Vector(lowerLeftCorner.X, lowerLeftCorner.Y + height));
    }

    internal static Polygon Ellipse(Vector mid, double a, double b, double angle, int steps)
    {
        var p = new Polygon();
        double beta = -angle * MathUtils.DegToRad;
        double sinBeta = Math.Sin(beta);
        double cosBeta = Math.Cos(beta);
        for (int i = 0; i < steps; i++)
        {
            double currentAngle = i * 360.0 / steps;
            double alpha = currentAngle * MathUtils.DegToRad;
            double sinAlpha = Math.Sin(alpha);
            double cosAlpha = Math.Cos(alpha);
            p.Add(new Vector(mid.X + a * cosAlpha * cosBeta - b * sinAlpha * sinBeta,
                mid.Y + a * cosAlpha * sinBeta + b * sinAlpha * cosBeta));
        }

        p.UpdateDecomposition();
        return p;
    }

    internal void Add(Vector p)
    {
        Vertices.Add(p);
    }

    internal void Insert(int index, Vector p)
    {
        index = GetIndex(index);
        Vertices.Insert(index, p);
    }

    internal void RemoveRange(int index, int count)
    {
        index = GetIndex(index);
        if (index + count <= Vertices.Count)
            Vertices.RemoveRange(index, count);
        else
        {
            int firstCount = Vertices.Count - index;
            Vertices.RemoveRange(index, firstCount);
            Vertices.RemoveRange(0, count - firstCount);
        }
    }

    private int GetIndex(int index)
    {
        return index % Vertices.Count;
    }

    internal void Move(Vector delta)
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vertices[i] += delta;
        }
    }

    internal double DistanceFromPoint(Vector p)
    {
        double smallest = Math.Sqrt(Math.Pow((Vertices[0].X - p.X), 2) + Math.Pow((Vertices[0].Y - p.Y), 2));
        double current;
        int c = Vertices.Count - 1;
        for (int i = 0; i < c; i++)
        {
            current = GeometryUtils.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X,
                Vertices[i + 1].Y, p.X, p.Y);
            if (current < smallest)
                smallest = current;
        }

        if (!IsGrass || Global.AppSettings.LevelEditor.RenderingSettings.ShowInactiveGrassEdges)
        {
            current = GeometryUtils.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X,
                p.Y);
            if (current < smallest)
                smallest = current;
        }

        return smallest;
    }

    internal void SetBeginPoint(int index)
    {
        int i = GetIndex(index);
        if (i == 0)
            return;
        for (int j = 0; j < i; j++)
        {
            Vertices.Add(Vertices[0]);
            Vertices.RemoveAt(0);
        }
    }

    internal void RemoveLastVertex()
    {
        Vertices.RemoveAt(Vertices.Count - 1);
    }

    internal Vector GetLastVertex()
    {
        return Vertices[^1];
    }

    internal void MarkVectorsAs(VectorMark mark)
    {
        for (var index = 0; index < Vertices.Count; index++)
        {
            Vertices[index] = Vertices[index] with { Mark = mark };
        }
    }

    internal int GetNearestVertexIndex(Vector p)
    {
        double smallest = (Vertices[0] - p).LengthSquared;
        int smallestIndex = 0;
        for (int i = 1; i < Vertices.Count; i++)
        {
            double current = (Vertices[i] - p).LengthSquared;
            if (current < smallest)
            {
                smallest = current;
                smallestIndex = i;
            }
        }

        return smallestIndex;
    }

    internal double GetNearestVertexDistance(Vector p)
    {
        return Math.Sqrt(Vertices.Select(t => (t - p).LengthSquared).Min());
    }

    internal int GetNearestSegmentIndex(Vector p)
    {
        double smallest = GeometryUtils.DistanceFromSegment(Vertices[0].X, Vertices[0].Y, Vertices[1].X,
            Vertices[1].Y, p.X, p.Y);
        int smallestIndex = 0;
        double current;
        int c = Vertices.Count - 1;
        for (int i = 1; i < c; i++)
        {
            current = GeometryUtils.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X,
                Vertices[i + 1].Y, p.X, p.Y);
            if (current < smallest)
            {
                smallest = current;
                smallestIndex = i;
            }
        }

        current = GeometryUtils.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X,
            p.Y);
        if (current < smallest)
        {
            smallestIndex = c;
        }

        return smallestIndex;
    }

    internal void InsertIntersection(Vector p, double delta)
    {
        int c = Vertices.Count - 1;
        for (int i = 0; i < c; i++)
        {
            if (
                GeometryUtils.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y,
                    p.X, p.Y) < delta)
            {
                Insert(i + 1, p);
                return;
            }
        }

        if (GeometryUtils.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X, p.Y) <
            delta)
        {
            Vertices.Add(p);
            return;
        }

        UiUtils.ShowError("Failed to add intersection!!");
    }

    internal Polygon Smoothen(int steps, double vertexOffset, bool onlySelected) //0.5 <= VertexOffset <= 1.0
    {
        if (Math.Abs(vertexOffset - 1.0) < GeometryUtils.Tolerance)
        {
            return Clone();
        }

        var smoothPoly = new Polygon();
        for (int i = 0; i < Vertices.Count; i++)
        {
            if (onlySelected)
            {
                int j;
                for (j = 0; j <= 2; j++)
                {
                    if (this[i + j].Mark != VectorMark.Selected)
                    {
                        if (this[i - 1].Mark != VectorMark.Selected ||
                            this[i + 1].Mark != VectorMark.Selected ||
                            this[i].Mark != VectorMark.Selected)
                            smoothPoly.Add(this[i].Clone());
                        break;
                    }
                }

                if (j < 3)
                    continue;
                if (this[i - 1].Mark != VectorMark.Selected)
                    smoothPoly.Add(this[i].Clone());
            }

            Vector startPoint = this[i] + (this[i + 1] - this[i]) * vertexOffset;
            Vector endPoint = this[i + 1] + (this[i + 2] - this[i + 1]) * (1.0 - vertexOffset);
            Vector midPoint = this[i + 1];

            int numPoints = steps;
            if (Math.Abs(vertexOffset - 0.5) < GeometryUtils.Tolerance &&
                (!onlySelected || (this[i + 3].Mark == VectorMark.Selected)))
            {
                numPoints--;
            }

            for (int j = 0; j < numPoints; j++)
            {
                double t = j / (double)(steps - 1);
                smoothPoly.Add((1 - t) * (1 - t) * startPoint + 2 * (1 - t) * t * midPoint +
                               t * t * endPoint);
            }
        }

        return smoothPoly;
    }

    internal Polygon Unsmoothen(double angle, double length, bool onlySelected)
    {
        var unsmoothPoly = new Polygon(this);
        if (unsmoothPoly.Vertices.Count == 3)
            return unsmoothPoly;
        for (int i = 0; i < unsmoothPoly.Vertices.Count; i++)
        {
            if (onlySelected)
            {
                int j;
                for (j = 0; j <= 2; j++)
                    if (unsmoothPoly[i + j].Mark != VectorMark.Selected)
                        break;
                if (j < 3)
                    continue;
            }

            if (
                Math.Abs(
                    (unsmoothPoly[i + 1] - unsmoothPoly[i]).AngleBetween(
                        unsmoothPoly[i + 2] - unsmoothPoly[i + 1])) <
                angle)
                unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Vertices.Count);
            else if ((unsmoothPoly[i + 1] - unsmoothPoly[i]).Length < length)
                unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Vertices.Count);
            else if ((unsmoothPoly[i + 2] - unsmoothPoly[i + 1]).Length < length)
                unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Vertices.Count);
            if (unsmoothPoly.Vertices.Count == 3)
                break;
        }

        return unsmoothPoly;
    }

    internal List<Polygon> PolygonOperationWith(Polygon p, PolygonOperationType type)
    {
        if (!IsSimple || !p.IsSimple)
        {
            throw new PolygonException("Both polygons must be non-self-intersecting.");
        }

        var resultPolys = type switch
        {
            PolygonOperationType.Intersection => p.ToIPolygon().Intersection(ToIPolygon()),
            PolygonOperationType.Union => p.ToIPolygon().Union(ToIPolygon()),
            PolygonOperationType.Difference => p.ToIPolygon().Difference(ToIPolygon()),
            PolygonOperationType.SymmetricDifference => p.ToIPolygon()
                .SymmetricDifference(ToIPolygon())
                .Buffer(BufferDistance),
            _ => throw new PolygonException("Unsupported operation type.")
        };

        var multiPolygon = resultPolys as MultiPolygon;
        var results = new List<Polygon>();
        if (multiPolygon != null)
        {
            results.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().SelectMany(poly => poly.ToElmaPolygons()));
        }

        if (resultPolys is NetTopologySuite.Geometries.Polygon polygon)
        {
            results.AddRange(polygon.ToElmaPolygons());
        }

        foreach (var x in results)
            x.UpdateDecomposition();
        return results;
    }

    internal int IndexOf(Vector v)
    {
        return Vertices.IndexOf(v);
    }

    internal void ChangeOrientation()
    {
        Vertices.Reverse();
    }

    internal bool AreaHasPoint(Vector p) =>
        RayCrossingCounter.LocatePointInRing(p, ToCoordinateArray()) == Location.Interior;

    internal List<Polygon>? Cut(Vector v1, Vector v2, double cutRadius)
    {
        var clone = new Polygon(this);
        Vector.MarkDefault = VectorMark.Selected;
        int numberOfIntersections = 0;
        for (int i = 0; i < Vertices.Count; i++)
        {
            var isectPoint = GeometryUtils.GetIntersectionPoint(this[i], this[i + 1], v1, v2);
            if (isectPoint is { } p)
            {
                clone.InsertIntersection(p, GeometryUtils.Tolerance);
                numberOfIntersections++;
            }
        }

        Vector.MarkDefault = VectorMark.None;
        if (numberOfIntersections % 2 == 0 && numberOfIntersections > 0)
        {
            var result = new List<Polygon> { new() };
            int k = 0;
            bool isBeginning = true;
            while (k != clone.Vertices.Count)
            {
                if (clone.Vertices[k].Mark == VectorMark.Selected)
                {
                    Vector cutVector = clone[k + 1] - clone[k - 1];
                    cutVector /= cutVector.Length;
                    var angleAbs = Math.Abs(cutVector.AngleBetween(v2 - v1));
                    if (angleAbs < GeometryUtils.Tolerance)
                    {
                        return null;
                    }

                    cutVector *= cutRadius / Math.Sin(angleAbs * MathUtils.DegToRad);
                    double distance1 = (clone[k + 1] - clone[k]).Length;
                    double distance2 = (clone[k] - clone[k - 1]).Length;
                    if (cutVector.Length > Math.Min(distance1, distance2))
                        cutVector = cutVector / cutVector.Length * Math.Min(distance1, distance2) / 2;
                    if (!isBeginning)
                    {
                        result[^1].Add(clone.Vertices[k] - cutVector);
                        result[0].Add(clone.Vertices[k] + cutVector);
                    }
                    else
                    {
                        result.Add(new Polygon());
                        result[^1].Add(clone.Vertices[k] + cutVector);
                        result[0].Add(clone.Vertices[k] - cutVector);
                    }

                    isBeginning = !isBeginning;
                }
                else
                {
                    if (isBeginning)
                        result[0].Add(clone.Vertices[k]);
                    else
                        result[^1].Add(clone.Vertices[k]);
                }

                k++;
            }

            result.ForEach(p => p.IsGrass = IsGrass);
            return result.Any(polygon => polygon.Vertices.Count < 3) ? null : result;
        }

        return null;
    }

    internal Polygon ApplyTransformation(Matrix matrix, bool applySelectedOnly = false)
    {
        var transformed = new Polygon(this);
        for (int i = 0; i < Vertices.Count; i++)
            if (!applySelectedOnly || Vertices[i].Mark == VectorMark.Selected)
                transformed.Vertices[i] = matrix.Transform(Vertices[i]);
        return transformed;
    }

    public Polygon Clone()
    {
        return new(this);
    }

    public static Polygon Rectangle(Vector corner1, Vector corner2)
    {
        return new(corner1, new Vector(corner2.X, corner1.Y), corner2, new Vector(corner1.X, corner2.Y));
    }

    internal NetTopologySuite.Geometries.Polygon ToIPolygon() => GeometryFactory.Floating.CreatePolygon(ToCoordinateArray());

    private Coordinate[] ToCoordinateArray()
    {
        var verts = Vertices.Select(v => v.ToCoordinate());
        var ring = verts.ToList();
        ring.Add(ring.First());
        var coordinates = ring.ToArray();
        return coordinates;
    }

    public Polygon RemoveDuplicateVertices()
    {
        for (int i = Vertices.Count; i >= 1; i--)
        {
            if ((this[i] - this[i - 1]).LengthSquared < 1e-16)
            {
                Vertices.RemoveAt(i % Vertices.Count);
            }
        }

        return this;
    }
}