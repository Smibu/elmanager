#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Elmanager.Vectrast;

internal class VectRast
{
    public readonly List<List<DoubleVector2>> Polygons = new();

    public SortedList<IntVector2, VectorPixel> CreateVectors(byte[,] pixelOn, Bitmap bmp)
    {
        var vectors = new SortedList<IntVector2, VectorPixel>(bmp.Width * bmp.Height / 32);
        var vectorsReverse = new Dictionary<IntVector2, VectorPixel>(bmp.Width * bmp.Height / 32);
        for (var j = 0; j < bmp.Height + 1; j++)
            for (var i = 0; i < bmp.Width + 1; i++)
            {
                var type =
                    (pixelOn[i, j] & 1) +
                    ((pixelOn[i + 1, j] & 1) << 1) +
                    ((pixelOn[i, j + 1] & 1) << 2) +
                    ((pixelOn[i + 1, j + 1] & 1) << 3);
                if (type == 1 + 8 || type == 2 + 4)
                {
                    // get rid of illegal situations
                    pixelOn[i, j] |= 1;
                    pixelOn[i + 1, j] |= 1;
                    pixelOn[i, j + 1] |= 1;
                    pixelOn[i + 1, j + 1] |= 1;
                }
            }
        for (var j = 0; j < bmp.Height; j++)
            for (var i = 0; i < bmp.Width; i++)
            {
                if ((pixelOn[i + 1, j + 1] & 1) == 1)
                {
                    var type1 =
                        (pixelOn[i + 1, j] & 1) +
                        (pixelOn[i + 1, j + 2] & 1);
                    var type2 =
                        (pixelOn[i, j + 1] & 1) +
                        (pixelOn[i + 2, j + 1] & 1);
                    if (type1 == 2 && type2 == 0 || type1 == 0 && type2 == 2)
                    {
                        // get rid of illegal situations
                        pixelOn[i + 2, j + 1] |= 1;
                        pixelOn[i + 1, j + 2] |= 1;
                    }
                }
            }
        for (var j = -1; j < bmp.Height; j++)
            for (var i = -1; i < bmp.Width; i++)
            {
                var type =
                    (pixelOn[i + 1, j + 1] & 1) +
                    ((pixelOn[i + 2, j + 1] & 1) << 1) +
                    ((pixelOn[i + 1, j + 2] & 1) << 2) +
                    ((pixelOn[i + 2, j + 2] & 1) << 3);
                var fromPnt = new IntVector2();
                var toPnt = new IntVector2();
                switch (type)
                {
                    // create horizontal and vertical vectors between adjacent pixels
                    case 3:
                        // xx
                        // --
                        fromPnt = new IntVector2(i, j);
                        toPnt = new IntVector2(i + 1, j);
                        break;
                    case 12:
                        // --
                        // xx
                        fromPnt = new IntVector2(i + 1, j + 1);
                        toPnt = new IntVector2(i, j + 1);
                        break;
                    case 5:
                        // x-
                        // x-
                        fromPnt = new IntVector2(i, j + 1);
                        toPnt = new IntVector2(i, j);
                        break;
                    case 10:
                        // -x
                        // -x
                        fromPnt = new IntVector2(i + 1, j);
                        toPnt = new IntVector2(i + 1, j + 1);
                        break;
                    case 14:
                        // -x
                        // xx
                        fromPnt = new IntVector2(i + 1, j);
                        toPnt = new IntVector2(i, j + 1);
                        break;
                    case 13:
                        // x-
                        // xx
                        fromPnt = new IntVector2(i + 1, j + 1);
                        toPnt = new IntVector2(i, j);
                        break;
                    case 11:
                        // xx
                        // -x
                        fromPnt = new IntVector2(i, j);
                        toPnt = new IntVector2(i + 1, j + 1);
                        break;
                    case 7:
                        // xx
                        // x-
                        fromPnt = new IntVector2(i, j + 1);
                        toPnt = new IntVector2(i + 1, j);
                        break;
                }
                if (fromPnt.Defined)
                {
                    if (vectorsReverse.ContainsKey(fromPnt))
                    {
                        var oldVp = vectorsReverse[fromPnt];
                        if ((toPnt - fromPnt).Extension(oldVp.ToPnt - oldVp.FromPnt))
                        {
                            vectors.Remove(oldVp.FromPnt);
                            vectorsReverse.Remove(oldVp.ToPnt);
                            fromPnt = oldVp.FromPnt;
                        }
                    }
                    if (vectors.ContainsKey(toPnt))
                    {
                        var oldVp = vectors[toPnt];
                        if ((toPnt - fromPnt).Extension(oldVp.ToPnt - oldVp.FromPnt))
                        {
                            vectors.Remove(oldVp.FromPnt);
                            vectorsReverse.Remove(oldVp.ToPnt);
                            toPnt = oldVp.ToPnt;
                        }
                    }
                    if (!fromPnt.Equals(toPnt))
                    {
                        // do not add null vectors, they ugly :/
                        var newVp = new VectorPixel(fromPnt, toPnt);
                        if (vectors.ContainsKey(newVp.FromPnt))
                            throw new Exception("illegal edge configuration at pixel [" + newVp.FromPnt.X + ", " +
                                                newVp.FromPnt.Y + "]");
                        vectors.Add(newVp.FromPnt, newVp);
                        if (vectorsReverse.ContainsKey(newVp.ToPnt))
                            throw new Exception("illegal edge configuration at pixel [" + newVp.ToPnt.X + ", " +
                                                newVp.ToPnt.Y + "]");
                        vectorsReverse.Add(newVp.ToPnt, newVp);
                    }
                }
            }
        return vectors;
    }

    public void CollapseVectors(SortedList<IntVector2, VectorPixel> vectors)
    {
        while (vectors.Count > 0)
        {
            var vectorOld = vectors.GetValueAtIndex(0);
            // ensures we begin at start/end of some line (as opposed to an inner segment)
            var startPnt = vectorOld.FromPnt;
            VectorPixel vectorPrev = null;
            var line = new Line(vectorOld);
            var vertices = new List<DoubleVector2>();
            do
            {
                var vectorNow = vectors[vectorOld.ToPnt];
                vectors.TryGetValue(vectorNow.ToPnt, out var vectorNew);
                if (!vectorNow.FromPnt.Equals(startPnt) && !vectorNow.ToPnt.Equals(startPnt) &&
                    vectorNow.LinkVector() && line.SameDir(vectorNow) && !vectorNew!.LinkVector() &&
                    line.SameDir(vectorNew))
                {
                    if (line.SatisfiesInner(vectorNew))
                    {
                        // new segment ok, let's try the next one
                        line.Update(vectorNew);
                        vectors.Remove(vectorNow.FromPnt);
                        vectorOld = vectorNew;
                    }
                    else if (line.SatisfiesOuter(vectorNew))
                    {
                        // vectorNow can be an outer segment
                        line.Update(vectorNew);

                        vertices.Add(new DoubleVector2(line.ToPnt));
                        vectors.Remove(vectorNew.FromPnt);
                        vectors.Remove(vectorNow.FromPnt);
                        vectorOld = vectors[vectorNew.ToPnt];
                        line = new Line(vectorOld);
                    }
                    else
                    {
                        // new segment off, but link is ok as an outer segment
                        line.Update(vectorNow);
                        vertices.Add(new DoubleVector2(line.ToPnt));
                        vectors.Remove(vectorNow.FromPnt);
                        vectorOld = vectorNew;
                        line = new Line(vectorOld);
                    }
                }
                else if (!vectorNow.FromPnt.Equals(startPnt) && line.SameDir(vectorNow))
                {
                    // vectorNow is not a simple link between two segments
                    if (line.SatisfiesInner(vectorNow))
                    {
                        // new segment ok, let's try the next one
                        line.Update(vectorNow);
                        vectorOld = vectorNow;
                    }
                    else if (line.SatisfiesOuter(vectorNow))
                    {
                        // vectorNow can be an outer segment
                        line.Update(vectorNow);
                        vertices.Add(new DoubleVector2(line.ToPnt));
                        vectors.Remove(vectorNow.FromPnt);
                        vectorOld = vectorNew;
                        line = new Line(vectorOld);
                    }
                    else
                    {
                        // vectorNow just won't fit - process it separately
                        vertices.Add(new DoubleVector2(line.ToPnt));
                        vectorOld = vectorNow;
                        line = new Line(vectorOld);
                    }
                }
                else
                {
                    // vectorNow just won't fit - process it separately
                    vertices.Add(new DoubleVector2(line.ToPnt));
                    vectorOld = vectorNow;
                    line = new Line(vectorOld);
                }
                if (vectorPrev != null)
                    vectors.Remove(vectorPrev.FromPnt);
                vectorPrev = vectorOld;
            } while (!vectorOld.FromPnt.Equals(startPnt));
            vectors.Remove(startPnt);
            Polygons.Add(vertices);
        }
    }

    public void TransformVectors(Matrix2D matrix)
    {
        var randGen = new Random();
        foreach (var vertices in Polygons)
        {
            for (var i = 0; i < vertices.Count; i++)
            {
                var vertexOld = vertices[i];
                vertices.RemoveAt(i);
                var vertexNew = vertexOld * matrix;
                // add a little random jitter so as to remove the 'horizontal line' bug
                vertexNew.X += randGen.NextDouble() / 100000.0;
                vertexNew.Y += randGen.NextDouble() / 100000.0;
                vertices.Insert(i, vertexNew);
            }
        }
    }

    public void LoadAsBmp(string fileName, out Bitmap bmp, out byte[,] pixelOn, byte merging)
    {
        bmp = new Bitmap(fileName);
        pixelOn = new byte[bmp.Width + 2, bmp.Height + 2];
        for (var j = -1; j <= bmp.Height; j++)
            for (var i = -1; i <= bmp.Width; i++)
            {
                if (j < 0 || j >= bmp.Height || i < 0 || i >= bmp.Width)
                    pixelOn[i + 1, j + 1] = merging;
                else
                {
                    var col = bmp.GetPixel(i, j);
                    if (col.R < 250 || col.G < 250 || col.B < 250)
                    {
                        pixelOn[i + 1, j + 1] = 1;
                    }
                }
            }
    }
}