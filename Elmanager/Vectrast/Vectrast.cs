#nullable disable

using System;
using System.Collections;
using System.Drawing;

namespace vectrast;

internal class VectRast
{
    public readonly ArrayList polygons = new();

    public SortedList createVectors(byte[,] pixelOn, Bitmap bmp)
    {
        var vectors = new SortedList(bmp.Width * bmp.Height / 32);
        var vectorsReverse = new Hashtable(bmp.Width * bmp.Height / 32);
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
                if (fromPnt.defined)
                {
                    if (vectorsReverse.Contains(fromPnt))
                    {
                        var oldVP = (VectorPixel)vectorsReverse[fromPnt];
                        if ((toPnt - fromPnt).extension(oldVP.toPnt - oldVP.fromPnt))
                        {
                            vectors.Remove(oldVP.fromPnt);
                            vectorsReverse.Remove(oldVP.toPnt);
                            fromPnt = oldVP.fromPnt;
                        }
                    }
                    if (vectors.Contains(toPnt))
                    {
                        var oldVP = (VectorPixel)vectors[toPnt];
                        if ((toPnt - fromPnt).extension(oldVP.toPnt - oldVP.fromPnt))
                        {
                            vectors.Remove(oldVP.fromPnt);
                            vectorsReverse.Remove(oldVP.toPnt);
                            toPnt = oldVP.toPnt;
                        }
                    }
                    if (!fromPnt.Equals(toPnt))
                    {
                        // do not add null vectors, they ugly :/
                        var newVP = new VectorPixel(fromPnt, toPnt);
                        if (vectors.Contains(newVP.fromPnt))
                            throw new Exception("illegal edge configuration at pixel [" + newVP.fromPnt.x + ", " +
                                                newVP.fromPnt.y + "]");
                        vectors.Add(newVP.fromPnt, newVP);
                        if (vectorsReverse.Contains(newVP.toPnt))
                            throw new Exception("illegal edge configuration at pixel [" + newVP.toPnt.x + ", " +
                                                newVP.toPnt.y + "]");
                        vectorsReverse.Add(newVP.toPnt, newVP);
                    }
                }
            }
        return vectors;
    }

    public void collapseVectors(SortedList vectors)
    {
        ArrayList vertices;
        while (vectors.Count > 0)
        {
            var vectorOld = (VectorPixel)vectors.GetByIndex(0);
            // ensures we begin at start/end of some line (as opposed to an inner segment)
            var startPnt = vectorOld.fromPnt;
            VectorPixel vectorNow;
            VectorPixel vectorNew;
            VectorPixel vectorPrev = null;
            var line = new Line(vectorOld);
            vertices = new ArrayList(1000);
            do
            {
                vectorNow = (VectorPixel)vectors[vectorOld.toPnt];
                vectorNew = (VectorPixel)vectors[vectorNow.toPnt];
                if (!vectorNow.fromPnt.Equals(startPnt) && !vectorNow.toPnt.Equals(startPnt) &&
                    vectorNow.linkVector() && line.sameDir(vectorNow) && !vectorNew.linkVector() &&
                    line.sameDir(vectorNew))
                {
                    if (line.satisfiesInner(vectorNew))
                    {
                        // new segment ok, let's try the next one
                        line.update(vectorNew);
                        vectors.Remove(vectorNow.fromPnt);
                        vectorOld = vectorNew;
                    }
                    else if (line.satisfiesOuter(vectorNew))
                    {
                        // vectorNow can be an outer segment
                        line.update(vectorNew);

                        vertices.Add(new DoubleVector2(line.toPnt));
                        vectors.Remove(vectorNew.fromPnt);
                        vectors.Remove(vectorNow.fromPnt);
                        vectorOld = (VectorPixel)vectors[vectorNew.toPnt];
                        line = new Line(vectorOld);
                    }
                    else
                    {
                        // new segment off, but link is ok as an outer segment
                        line.update(vectorNow);
                        vertices.Add(new DoubleVector2(line.toPnt));
                        vectors.Remove(vectorNow.fromPnt);
                        vectorOld = vectorNew;
                        line = new Line(vectorOld);
                    }
                }
                else if (!vectorNow.fromPnt.Equals(startPnt) && line.sameDir(vectorNow))
                {
                    // vectorNow is not a simple link between two segments
                    if (line.satisfiesInner(vectorNow))
                    {
                        // new segment ok, let's try the next one
                        line.update(vectorNow);
                        vectorOld = vectorNow;
                    }
                    else if (line.satisfiesOuter(vectorNow))
                    {
                        // vectorNow can be an outer segment
                        line.update(vectorNow);
                        vertices.Add(new DoubleVector2(line.toPnt));
                        vectors.Remove(vectorNow.fromPnt);
                        vectorOld = vectorNew;
                        line = new Line(vectorOld);
                    }
                    else
                    {
                        // vectorNow just won't fit - process it separately
                        vertices.Add(new DoubleVector2(line.toPnt));
                        vectorOld = vectorNow;
                        line = new Line(vectorOld);
                    }
                }
                else
                {
                    // vectorNow just won't fit - process it separately
                    vertices.Add(new DoubleVector2(line.toPnt));
                    vectorOld = vectorNow;
                    line = new Line(vectorOld);
                }
                if (vectorPrev != null)
                    vectors.Remove(vectorPrev.fromPnt);
                vectorPrev = vectorOld;
            } while (!vectorOld.fromPnt.Equals(startPnt));
            vectors.Remove(startPnt);
            vertices.TrimToSize();
            polygons.Add(vertices);
        }
    }

    public void transformVectors(Matrix2D matrix)
    {
        var randGen = new Random();
        for (var j = 0; j < polygons.Count; j++)
        {
            var vertices = (ArrayList)polygons[j];
            for (var i = 0; i < vertices.Count; i++)
            {
                var vertexOld = (DoubleVector2)vertices[i];
                vertices.RemoveAt(i);
                var vertexNew = vertexOld * matrix;
                // add a little random jitter so as to remove the 'horizontal line' bug
                vertexNew.x += randGen.NextDouble() / 100000.0;
                vertexNew.y += randGen.NextDouble() / 100000.0;
                vertices.Insert(i, vertexNew);
            }
        }
    }

    public void loadAsBmp(string fileName, out Bitmap bmp, out byte[,] pixelOn, byte merging)
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