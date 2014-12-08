using System;
using System.Collections.Generic;
using System.Linq;

namespace Elmanager
{
    internal enum PolygonMark
    {
        None = 0,
        Highlight = 1,
        Selected = 2,
        Erroneous = 3
    }

    [Serializable]
    internal class Polygon
    {
        internal Vector[][] Decomposition;
        internal bool IsGrass;
        internal PolygonMark Mark;
        internal List<Vector> Vertices;

        internal Polygon(List<Vector> vertices)
        {
            Vertices = vertices;
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

        internal int Count
        {
            get { return Vertices.Count; }
        }

        internal Vector this[int index]
        {
            get { return index < 0 ? Vertices[Vertices.Count + index] : Vertices[index % Vertices.Count]; }
        }

        internal double SignedArea
        {
            get
            {
                double result = Vertices.Select((t, i) => this[i].X * this[i + 1].Y - this[i + 1].X * this[i].Y).Sum();
                return result / 2;
            }
        }

        internal bool IsCounterClockwise
        {
            get { return SignedArea > 0; }
        }

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

        internal bool IsSimple
        {
            get
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    if (Vertices[i] == Vertices[(i + 1) % Vertices.Count])
                    {
                        return false;
                    }
                }

                for (int i = 0; i <= Vertices.Count - 2; i++)
                {
                    for (int j = i + 2; j < Vertices.Count; j++)
                    {
                        if (i == 0 && j == Vertices.Count - 1)
                            continue;
                        if (Geometry.SegmentsIntersect(Vertices[i], Vertices[i + 1], Vertices[j],
                                                       Vertices[(j + 1) % Vertices.Count]))
                            return false;
                    }
                }
                return true;
            }
        }

        internal void UpdateDecomposition(bool updateGrass = true)
        {
            Decomposition = Geometry.Decompose(this);
            if (updateGrass && IsGrass)
            {
                double longest = Math.Abs(Vertices[Vertices.Count - 1].X - Vertices[0].X);
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

        internal static Polygon Square(Vector lowerLeftCorner, double side)
        {
            return new Polygon(new Vector(lowerLeftCorner.X, lowerLeftCorner.Y),
                               new Vector(lowerLeftCorner.X + side, lowerLeftCorner.Y),
                               new Vector(lowerLeftCorner.X + side, lowerLeftCorner.Y + side),
                               new Vector(lowerLeftCorner.X, lowerLeftCorner.Y + side));
        }

        internal static Polygon Rectangle(Vector lowerLeftCorner, double width, double height)
        {
            return new Polygon(new Vector(lowerLeftCorner.X, lowerLeftCorner.Y),
                               new Vector(lowerLeftCorner.X + width, lowerLeftCorner.Y),
                               new Vector(lowerLeftCorner.X + width, lowerLeftCorner.Y + height),
                               new Vector(lowerLeftCorner.X, lowerLeftCorner.Y + height));
        }

        internal static Polygon Ellipse(Vector mid, double a, double b, double angle, int steps)
        {
            var p = new Polygon();
            double beta = -angle * Constants.DegToRad;
            double sinBeta = Math.Sin(beta);
            double cosBeta = Math.Cos(beta);
            for (int i = 0; i < steps; i++)
            {
                double currentAngle = i * 360.0 / steps;
                double alpha = currentAngle * Constants.DegToRad;
                double sinAlpha = Math.Sin(alpha);
                double cosAlpha = Math.Cos(alpha);
                p.Add(new Vector(mid.X + a * cosAlpha * cosBeta - b * sinAlpha * sinBeta,
                                 mid.Y + a * cosAlpha * sinBeta + b * sinAlpha * cosBeta));
            }
            p.UpdateDecomposition();
            return p;
        }

        internal void Delete(int index)
        {
            Vertices.RemoveAt(GetIndex(index));
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

        internal bool Contains(Vector p)
        {
            return Vertices.Contains(p);
        }

        internal void Move(Vector delta)
        {
            Vertices.ForEach(vertex => { vertex.X += delta.X; vertex.Y += delta.Y; });
        }

        internal double DistanceFromPoint(Vector p)
        {
            double smallest = Math.Sqrt(Math.Pow((Vertices[0].X - p.X), 2) + Math.Pow((Vertices[0].Y - p.Y), 2));
            double current;
            int c = Vertices.Count - 1;
            for (int i = 0; i < c; i++)
            {
                current = Geometry.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X,
                                                       Vertices[i + 1].Y, p.X, p.Y);
                if (current < smallest)
                    smallest = current;
            }
            if (!IsGrass)
            {
                current = Geometry.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X,
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
            return Vertices[Vertices.Count - 1];
        }

        internal void MarkVectorsAs(Geometry.VectorMark mark)
        {
            foreach (Vector t in Vertices)
                t.Mark = mark;
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
            double smallest = Geometry.DistanceFromSegment(Vertices[0].X, Vertices[0].Y, Vertices[1].X,
                                                           Vertices[1].Y, p.X, p.Y);
            int smallestIndex = 0;
            double current;
            int c = Vertices.Count - 1;
            for (int i = 1; i < c; i++)
            {
                current = Geometry.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X,
                                                       Vertices[i + 1].Y, p.X, p.Y);
                if (current < smallest)
                {
                    smallest = current;
                    smallestIndex = i;
                }
            }
            current = Geometry.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X,
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
                    Geometry.DistanceFromSegment(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y,
                                                 p.X, p.Y) < delta)
                {
                    Insert(i + 1, p);
                    return;
                }
            }
            if (Geometry.DistanceFromSegment(Vertices[c].X, Vertices[c].Y, Vertices[0].X, Vertices[0].Y, p.X, p.Y) <
                delta)
            {
                Vertices.Add(p);
                return;
            }
            Utils.ShowError("Failed to add intersection!!");
        }

        private bool HasEdgeIntersectionsWith(Polygon p)
        {
            for (int i = 0; i < p.Vertices.Count; i++)
            {
                for (int j = 0; j < Vertices.Count; j++)
                    if (Geometry.IsEdgeIntersection(p[i], p[i + 1], this[j], this[j+1]))
                        return true;
            }
            return false;
        }

        internal Polygon Smoothen(int steps, double vertexOffset, bool onlySelected) //0.5 <= VertexOffset <= 1.0
        {
            if (Math.Abs(vertexOffset - 1.0) < 0.000001)
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
                        if (this[i + j].Mark != Geometry.VectorMark.Selected)
                        {
                            if (this[i - 1].Mark != Geometry.VectorMark.Selected ||
                                this[i + 1].Mark != Geometry.VectorMark.Selected ||
                                this[i].Mark != Geometry.VectorMark.Selected)
                                smoothPoly.Add(this[i].Clone());
                            break;
                        }
                    }
                    if (j < 3)
                        continue;
                    if (this[i-1].Mark != Geometry.VectorMark.Selected)
                        smoothPoly.Add(this[i].Clone());
                }
                Vector startPoint = this[i] + (this[i + 1] - this[i]) * vertexOffset;
                Vector endPoint = this[i + 1] + (this[i + 2] - this[i + 1]) * (1.0 - vertexOffset);
                Vector midPoint = this[i + 1];

                int numPoints = steps;
                if (Math.Abs(vertexOffset - 0.5) < 0.000001 && (!onlySelected || (this[i + 3].Mark == Geometry.VectorMark.Selected)))
                {
                    numPoints--;
                }

                for (int j = 0; j < numPoints; j++)
                {
                    double t = j / (double) (steps - 1);
                    smoothPoly.Add((1 - t) * (1 - t) * startPoint + 2 * (1 - t) * t * midPoint +
                                   t * t * endPoint);
                }
            }
            return smoothPoly;
        }

        internal Polygon Unsmoothen(double angle, double length, bool onlySelected)
        {
            var unsmoothPoly = new Polygon(this);
            if (unsmoothPoly.Count == 3)
                return unsmoothPoly;
            for (int i = 0; i < unsmoothPoly.Vertices.Count; i++)
            {
                if (onlySelected)
                {
                    int j;
                    for (j = 0; j <= 2; j++)
                        if (unsmoothPoly[i + j].Mark != Geometry.VectorMark.Selected)
                            break;
                    if (j < 3)
                        continue;
                }
                if (
                    Math.Abs(
                        (unsmoothPoly[i + 1] - unsmoothPoly[i]).AngleBetween(unsmoothPoly[i + 2] - unsmoothPoly[i + 1])) <
                    angle)
                    unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Count);
                else if ((unsmoothPoly[i + 1] - unsmoothPoly[i]).Length < length)
                    unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Count);
                else if ((unsmoothPoly[i + 2] - unsmoothPoly[i + 1]).Length < length)
                    unsmoothPoly.Vertices.RemoveAt((i + 1) % unsmoothPoly.Count);
                if (unsmoothPoly.Count == 3)
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
            if (!IsCounterClockwise)
                ChangeOrientation();
            if ((!p.IsCounterClockwise) ^ (type == PolygonOperationType.Difference))
                p.ChangeOrientation();
            var merged = new Polygon(p);
            var pClone = new Polygon(p);
            var clip = new Polygon(this);
            var results = new List<Polygon>();
            if (!merged.IntersectsWith(clip))
                throw new PolygonException("The polygons do not intersect.");

            while (merged.HasEdgeIntersectionsWith(clip))
            {
                merged.Move(new Vector(0.000001, 0.000001));
                pClone.Move(new Vector(0.000001, 0.000001));
            }
                

            results.Add(new Polygon());
            Vector.MarkDefault = Geometry.VectorMark.Selected;
            for (int i = 0; i < p.Vertices.Count; i++)
            {
                for (int j = 0; j < Vertices.Count; j++)
                {
                    Vector isectPoint = Geometry.GetIntersectionPoint(pClone[i], pClone[i + 1], this[j], this[j + 1]);
                    if ((object) isectPoint != null)
                    {
                        merged.InsertIntersection(isectPoint, 0.00000001);
                        clip.InsertIntersection(isectPoint, 0.00000001);
                    }
                }
            }
            Vector.MarkDefault = Geometry.VectorMark.None;
            int k = 0;
            Polygon currentPolygon = merged;
            Polygon notCurrentPolygon = clip;
            bool ready = false;
            while (true)
            {
                //Search for intersection
                while (currentPolygon[k].Mark != Geometry.VectorMark.Selected)
                {
                    k++;
                    k = k % currentPolygon.Vertices.Count;
                }
                //Is this inbound intersection
                int j = notCurrentPolygon.Vertices.IndexOf(currentPolygon.Vertices[k]);
                if (Geometry.IsInboundIntersection(currentPolygon[k - 1], currentPolygon[k + 1],
                    notCurrentPolygon[j - 1], notCurrentPolygon[j + 1]))
                {
                    k--;
                    if (k == -1)
                        k = currentPolygon.Vertices.Count - 1;
                    while (currentPolygon[k].Mark != Geometry.VectorMark.Selected)
                    {
                        k--;
                        if (k == -1)
                            k = currentPolygon.Vertices.Count - 1;
                    }
                    k++;
                    while (true)
                    {
                        k = k % currentPolygon.Vertices.Count;
                        //Now we are at the first vertex
                        while (currentPolygon[k].Mark != Geometry.VectorMark.Selected)
                        {
                            if (currentPolygon[k].Mark == Geometry.VectorMark.None)
                            {
                                results[results.Count - 1].Add(currentPolygon[k]);
                                currentPolygon.Vertices[k % currentPolygon.Count].Mark =
                                    Geometry.VectorMark.Highlight;
                            }
                            else
                            {
                                //Check if there are unexplored vertices
                                int i;
                                for (i = 0; i < currentPolygon.Vertices.Count; i++)
                                    if (currentPolygon.Vertices[i].Mark == Geometry.VectorMark.Selected)
                                        break;
                                if (i < currentPolygon.Vertices.Count)
                                {
                                    k = i;
                                    results.Add(new Polygon());
                                    goto endOfWhileLoop;
                                }
                                for (i = 0; i < notCurrentPolygon.Vertices.Count; i++)
                                    if (notCurrentPolygon.Vertices[i].Mark == Geometry.VectorMark.Selected)
                                        break;
                                if (i < notCurrentPolygon.Vertices.Count)
                                {
                                    k = i;
                                    results.Add(new Polygon());
                                    currentPolygon = clip;
                                    notCurrentPolygon = merged;
                                    goto endOfWhileLoop;
                                }
                                ready = true;
                                goto endOfWhileLoop;
                            }
                            k++;
                        }
                        k = k % currentPolygon.Vertices.Count;
                        results[results.Count - 1].Add(currentPolygon.Vertices[k]); //Add the intersection point
                        int g = notCurrentPolygon.IndexOf(currentPolygon.Vertices[k]);
                        currentPolygon.Vertices[k].Mark = Geometry.VectorMark.Visited; //Mark as visited
                        notCurrentPolygon.Vertices[g].Mark = Geometry.VectorMark.Visited; //Mark as visited
                        //Switch to the other polygon
                        if (currentPolygon.Equals(merged))
                        {
                            currentPolygon = clip;
                            notCurrentPolygon = merged;
                        }
                        else
                        {
                            currentPolygon = merged;
                            notCurrentPolygon = clip;
                        }
                        k = g + 1;
                    }
                    endOfWhileLoop:
                    if (ready)
                    {
                        foreach (Polygon x in results)
                            x.UpdateDecomposition();
                        return results;
                    }
                    continue;
                }
                k++;
            }
        }

        internal int IndexOf(Vector v)
        {
            return Vertices.IndexOf(v);
        }

        internal Polygon GetBoundingRectangle()
        {
            double xMin = Vertices[0].X;
            double xMax = xMin;
            double yMin = Vertices[0].Y;
            double yMax = yMin;
            foreach (Vector z in Vertices)
            {
                if (z.X < xMin)
                    xMin = z.X;
                if (z.X > xMax)
                    xMax = z.X;
                if (z.Y < yMin)
                    yMin = z.Y;
                if (z.Y > yMax)
                    yMax = z.Y;
            }
            return
                new Polygon(new List<Vector>
                                {
                                    new Vector(xMin, yMin),
                                    new Vector(xMax, yMin),
                                    new Vector(xMax, yMax),
                                    new Vector(xMin, yMax)
                                });
        }

        internal void ChangeOrientation()
        {
            Vertices.Reverse();
        }

        internal Vector GetCentroid()
        {
            var centroid = new Vector(0, 0);
            centroid = Vertices.Aggregate(centroid, (current, z) => current + z);
            return centroid / Vertices.Count;
        }

        internal bool IntersectsWith(Polygon p)
        {
            int i;
            int j = 0;
            for (i = 0; i <= Vertices.Count - 2; i++)
                for (j = 0; j <= p.Vertices.Count - 2; j++)
                    if (Geometry.SegmentsIntersect(Vertices[i], Vertices[i + 1], p.Vertices[j], p.Vertices[j + 1]))
                        return true;
            for (i = 0; i <= Vertices.Count - 2; i++)
                if (Geometry.SegmentsIntersect(Vertices[i], Vertices[i + 1], p.Vertices[j], p.Vertices[0]))
                    return true;
            for (j = 0; j <= p.Vertices.Count - 2; j++)
                if (Geometry.SegmentsIntersect(Vertices[i], Vertices[0], p.Vertices[j], p.Vertices[j + 1]))
                    return true;
            return Geometry.SegmentsIntersect(Vertices[i], Vertices[0], p.Vertices[j], p.Vertices[0]);
        }

        internal bool IntersectsWith(Vector v1, Vector v2)
        {
            int i;
            for (i = 0; i <= Vertices.Count - 2; i++)
                if (Geometry.SegmentsIntersect(Vertices[i], Vertices[i + 1], v1, v2))
                    return true;
            return Geometry.SegmentsIntersect(Vertices[i], Vertices[0], v1, v2);
        }

        internal bool AreaHasPoint(Vector p)
        {
            double x = p.X;
            double y = p.Y;
            bool isInside = false;
            if (Contains(p))
                return false;
            double k;
            double x1;
            double y1;
            double x2;
            double y2;
            for (int i = 0; i < Count - 1; i++)
            {
                x1 = Vertices[i].X;
                y1 = Vertices[i].Y;
                x2 = Vertices[i + 1].X;
                y2 = Vertices[i + 1].Y;
                if (!(y1 <= y ^ y2 > y))
                {
                    if (x1 <= x && x2 <= x)
                        isInside = !isInside;
                    else if (!(x1 <= x ^ x2 > x))
                    {
                        k = (y2 - y1) / (x2 - x1);
                        if (!(y < k * (x - x1) + y1 ^ k > 0))
                            isInside = !isInside;
                    }
                }
            }
            //Last edge
            int f = Count - 1;
            x1 = Vertices[0].X;
            y1 = Vertices[0].Y;
            x2 = Vertices[f].X;
            y2 = Vertices[f].Y;
            if (!(y1 <= y ^ y2 > y))
            {
                if (x1 <= x && x2 <= x)
                    isInside = !isInside;
                else if (!(x1 <= x ^ x2 > x))
                {
                    k = (y2 - y1) / (x2 - x1);
                    if (!(y < k * (x - x1) + y1 ^ k > 0))
                        isInside = !isInside;
                }
            }
            return isInside;
        }

        internal List<Polygon> Cut(Vector v1, Vector v2, double cutRadius)
        {
            var clone = new Polygon(this);
            Vector.MarkDefault = Geometry.VectorMark.Selected;
            int numberOfIntersections = 0;
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector isectPoint = Geometry.GetIntersectionPoint(this[i], this[i + 1], v1, v2);
                if ((object) isectPoint != null)
                {
                    clone.InsertIntersection(isectPoint, 0.00000001);
                    numberOfIntersections++;
                }
            }
            Vector.MarkDefault = Geometry.VectorMark.None;
            if (numberOfIntersections % 2 == 0 && numberOfIntersections > 0)
            {
                var result = new List<Polygon> {new Polygon()};
                int k = 0;
                bool isBeginning = true;
                while (k != clone.Count)
                {
                    if (clone.Vertices[k].Mark == Geometry.VectorMark.Selected)
                    {
                        Vector cutVector = clone[k + 1] - clone[k - 1];
                        cutVector /= cutVector.Length;
                        cutVector *= cutRadius / Math.Sin(Math.Abs(cutVector.AngleBetween(v2 - v1)) * Constants.DegToRad);
                        double distance1 = (clone[k + 1] - clone[k]).Length;
                        double distance2 = (clone[k] - clone[k - 1]).Length;
                        if (cutVector.Length > Math.Min(distance1, distance2))
                            cutVector = cutVector / cutVector.Length * Math.Min(distance1, distance2) / 2;
                        if (!isBeginning)
                        {
                            result[result.Count - 1].Add(clone.Vertices[k] - cutVector);
                            result[0].Add(clone.Vertices[k] + cutVector);
                        }
                        else
                        {
                            result.Add(new Polygon());
                            result[result.Count - 1].Add(clone.Vertices[k] + cutVector);
                            result[0].Add(clone.Vertices[k] - cutVector);
                        }
                        isBeginning = !isBeginning;
                    }
                    else
                    {
                        if (isBeginning)
                            result[0].Add(clone.Vertices[k]);
                        else
                            result[result.Count - 1].Add(clone.Vertices[k]);
                    }
                    k++;
                }
                result.ForEach(p => p.IsGrass = IsGrass);
                return result.Any(polygon => polygon.Count < 3) ? null : result;
            }
            return null;
        }

        internal Polygon Rotate(double angle, Vector center)
        {
            var rotated = new Polygon(this);
            Matrix rotationMatrix = Matrix.Identity;
            rotationMatrix.RotateAt(angle, center.X, center.Y);
            for (int i = 0; i < Vertices.Count; i++)
                rotated.Vertices[i] = rotationMatrix.Transform(Vertices[i]);
            return rotated;
        }

        internal Polygon ApplyTransformation(Matrix matrix, bool applySelectedOnly = false)
        {
            var transformed = new Polygon(this);
            for (int i = 0; i < Vertices.Count; i++)
                if (!applySelectedOnly || Vertices[i].Mark == Geometry.VectorMark.Selected)
                    transformed.Vertices[i] = matrix.Transform(Vertices[i]);
            return transformed;
        }

        internal Polygon Scale(double x, double y, Vector center)
        {
            var scaled = new Polygon(this);
            Matrix rotationMatrix = Matrix.Identity;
            rotationMatrix.ScaleAt(x, y, center.X, center.Y);
            for (int i = 0; i < Vertices.Count; i++)
                scaled.Vertices[i] = rotationMatrix.Transform(Vertices[i]);
            return scaled;
        }

        internal List<Vector> GetSelfInterSections()
        {
            var isects = new List<Vector>();
            for (int i = 0; i <= Vertices.Count - 2; i++)
            {
                for (int j = i + 2; j < Vertices.Count; j++)
                {
                    if (i == 0 && j == Vertices.Count - 1)
                        continue;
                    Vector isectPoint = Geometry.GetIntersectionPoint(Vertices[i], Vertices[i + 1], Vertices[j],
                                                                      Vertices[(j + 1) % Vertices.Count]);
                    if ((object) isectPoint != null)
                        isects.Add(isectPoint);
                }
            }
            return isects;
        }

        internal List<Vector> GetIntersectionsWith(Polygon p)
        {
            var isects = new List<Vector>();
            Vector isect;
            int i;
            int j = 0;
            for (i = 0; i <= Vertices.Count - 2; i++)
            {
                for (j = 0; j <= p.Vertices.Count - 2; j++)
                {
                    isect = Geometry.GetIntersectionPoint(Vertices[i], Vertices[i + 1], p.Vertices[j],
                                                          p.Vertices[j + 1]);
                    if ((object) isect != null)
                        isects.Add(isect);
                }
            }
            for (i = 0; i <= Vertices.Count - 2; i++)
            {
                isect = Geometry.GetIntersectionPoint(Vertices[i], Vertices[i + 1], p.Vertices[j], p.Vertices[0]);
                if ((object) isect != null)
                    isects.Add(isect);
            }
            for (j = 0; j <= p.Vertices.Count - 2; j++)
            {
                isect = Geometry.GetIntersectionPoint(Vertices[i], Vertices[0], p.Vertices[j], p.Vertices[j + 1]);
                if ((object) isect != null)
                    isects.Add(isect);
            }
            isect = Geometry.GetIntersectionPoint(Vertices[i], Vertices[0], p.Vertices[j], p.Vertices[0]);
            if ((object) isect != null)
                isects.Add(isect);
            return isects;
        }

        public Polygon Clone()
        {
            return new Polygon(this);
        }

        public static Polygon Rectangle(Vector corner1, Vector corner2)
        {
            return new Polygon(corner1, new Vector(corner2.X, corner1.Y), corner2, new Vector(corner1.X, corner2.Y));
        }
    }
}