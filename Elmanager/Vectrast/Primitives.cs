#nullable disable

using System;
using System.Drawing;

namespace vectrast;

internal struct IntVector2 : IComparable
{
    public bool defined;
    public int x, y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
        defined = true;
    }

    int IComparable.CompareTo(object o)
    {
        // lexicographic
        var v = (IntVector2)o;
        if (x < v.x || x == v.x && y < v.y)
            return -1;
        if (x > v.x || x == v.x && y > v.y)
            return 1;
        return 0;
    }

    public static IntVector2 operator -(IntVector2 v1, IntVector2 v2)
    {
        return new(v1.x - v2.x, v1.y - v2.y);
    }

    public bool extension(IntVector2 otherVector)
    {
        return (x * y == 0 && x * otherVector.y - y * otherVector.x == 0); // parallel to either axis AND colinear
    }

    public override int GetHashCode()
    {
        return x + y * 7919;
    }

    public override bool Equals(object v)
    {
        try
        {
            return x == ((IntVector2)v).x && y == ((IntVector2)v).y;
        }
        catch
        {
            return false;
        }
    }
}

internal struct DoubleVector2
{
    public double x, y;

    public DoubleVector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public DoubleVector2(IntVector2 v)
    {
        x = v.x;
        y = v.y;
    }

    public double length
    {
        get { return Math.Sqrt(x * x + y * y); }
    }

    public DoubleVector2 perpendicular
    {
        get { return new(y, -x); }
    }

    public void normalize()
    {
        // throws exception for zero length
        var l = length;
        x /= l;
        y /= l;
    }

    public double scalarProduct(DoubleVector2 dv2)
    {
        return (x * dv2.x + y * dv2.y);
    }

    public static DoubleVector2 operator -(DoubleVector2 v1, DoubleVector2 v2)
    {
        return new(v1.x - v2.x, v1.y - v2.y);
    }
}

internal enum ObjectTypes
{
    Flower = 1,
    Food = 2,
    Killer = 3,
    Start = 4
}

internal struct ElmaObject
{
    public uint animationNumber;
    public uint type;
    public uint typeOfFood;
    public double x;
    public double y;

    public ElmaObject(double x, double y, uint type, uint typeOfFood, uint animationNumber)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.typeOfFood = typeOfFood;
        this.animationNumber = animationNumber;
    }
}

internal class Matrix2D
{
    public double[,] elements;

    public Matrix2D()
    {
        elements = new double[3, 3];
    }

    public static Matrix2D identityM()
    {
        var matrix = new Matrix2D();
        for (var i = 0; i < 3; i++)
            matrix.elements[i, i] = 1;
        return matrix;
    }

    public static Matrix2D translationM(double x, double y)
    {
        var matrix = identityM();
        matrix.elements[0, 2] = x;
        matrix.elements[1, 2] = y;
        return matrix;
    }

    public static Matrix2D scaleM(double x, double y)
    {
        var matrix = new Matrix2D();
        matrix.elements[0, 0] = x;
        matrix.elements[1, 1] = y;
        matrix.elements[2, 2] = 1;
        return matrix;
    }

    public static Matrix2D rotationM(double x)
    {
        var matrix = identityM();
        matrix.elements[0, 0] = Math.Cos(x * Math.PI / 180);
        matrix.elements[1, 1] = matrix.elements[0, 0];
        matrix.elements[1, 0] = Math.Sin(x * Math.PI / 180);
        matrix.elements[0, 1] = -matrix.elements[1, 0];
        return matrix;
    }

    public static Matrix2D operator *(Matrix2D m1, Matrix2D m2)
    {
        var matrix = new Matrix2D();
        for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                double sum = 0;
                for (var k = 0; k < 3; k++)
                    sum += m1.elements[k, j] * m2.elements[i, k];
                matrix.elements[i, j] = sum;
            }
        return matrix;
    }

    public static DoubleVector2 operator *(DoubleVector2 v, Matrix2D m)
    {
        var vector = new DoubleVector2();
        vector.x = v.x * m.elements[0, 0] + v.y * m.elements[0, 1] + m.elements[0, 2];
        vector.y = v.x * m.elements[1, 0] + v.y * m.elements[1, 1] + m.elements[1, 2];
        return vector;
    }
}

internal class VectorPixel
{
    public IntVector2 fromPnt;
    public IntVector2 toPnt;

    public VectorPixel()
    {
    }

    public VectorPixel(IntVector2 fromPnt, IntVector2 toPnt)
    {
        this.fromPnt = fromPnt;
        this.toPnt = toPnt;
    }

    public int dx
    {
        get { return Math.Abs(toPnt.x - fromPnt.x) + 1; }
    }

    public int dy
    {
        get { return Math.Abs(toPnt.y - fromPnt.y) + 1; }
    }

    public int sx
    {
        get { return Math.Sign(toPnt.x - fromPnt.x); }
    }

    public int sy
    {
        get { return Math.Sign(toPnt.y - fromPnt.y); }
    }

    public bool linkVector()
    {
        return dx == 2 && dy == 2;
    }
}

internal class Line : VectorPixel
{
    private double maxleft;
    private double minright;
    private int nextmaxdx;
    private int nextmaxdy;
    private int nextmindx;
    private int nextmindy;
    public IntVector2 outerFromPnt;

    public Line(VectorPixel v)
    {
        maxleft = double.MinValue;
        minright = double.MaxValue;
        outerFromPnt = v.fromPnt;
        toPnt = v.toPnt;
        nextmindx = 0;
        nextmaxdx = int.MaxValue;
        nextmindy = 0;
        nextmaxdy = int.MaxValue;
    }

    public bool satisfiesOuter(VectorPixel v)
    {
        return
            Math.Abs(v.toPnt.x - toPnt.x) <= nextmaxdx &&
            Math.Abs(v.toPnt.y - toPnt.y) <= nextmaxdy; // outer segment must be shorter than inner segment
    }

    public bool satisfiesInner(VectorPixel v)
    {
        if (!fromPnt.defined)
            return Math.Abs(toPnt.x - outerFromPnt.x) == Math.Abs(toPnt.y - outerFromPnt.y)
                ? true
                : // anything goes
                Math.Abs(v.toPnt.x - toPnt.x) >= Math.Abs(v.fromPnt.x - outerFromPnt.x) &&
                Math.Abs(v.toPnt.y - toPnt.y) >= Math.Abs(v.fromPnt.y - outerFromPnt.y);
        // inner segment must be >= the outer segment
        return
            nextmindx <= Math.Abs(v.toPnt.x - toPnt.x) && Math.Abs(v.toPnt.x - toPnt.x) <= nextmaxdx &&
            nextmindy <= Math.Abs(v.toPnt.y - toPnt.y) && Math.Abs(v.toPnt.y - toPnt.y) <= nextmaxdy;
    }

    public bool sameDir(VectorPixel v)
    {
        if (v.linkVector())
            return
                Math.Abs(Math.Sign(toPnt.x - outerFromPnt.x) - (v.toPnt.x - toPnt.x)) <= 1 &&
                Math.Abs(Math.Sign(toPnt.y - outerFromPnt.y) - (v.toPnt.y - toPnt.y)) <= 1;
        if (Math.Abs(toPnt.x - outerFromPnt.x) > Math.Abs(toPnt.y - outerFromPnt.y))
            return
                Math.Sign(toPnt.x - outerFromPnt.x) == Math.Sign(v.toPnt.x - toPnt.x) &&
                Math.Abs(Math.Sign(toPnt.y - outerFromPnt.y) - (v.toPnt.y - toPnt.y)) <= 1;
        if (Math.Abs(toPnt.x - outerFromPnt.x) < Math.Abs(toPnt.y - outerFromPnt.y))
            return
                Math.Abs(Math.Sign(toPnt.x - outerFromPnt.x) - (v.toPnt.x - toPnt.x)) <= 1 &&
                Math.Sign(toPnt.y - outerFromPnt.y) == Math.Sign(v.toPnt.y - toPnt.y);
        return
            Math.Abs(Math.Sign(toPnt.x - outerFromPnt.x) - Math.Sign(v.toPnt.x - toPnt.x)) <= 1 &&
            Math.Abs(Math.Sign(toPnt.y - outerFromPnt.y) - Math.Sign(v.toPnt.y - toPnt.y)) <= 1;
    }

    public void update(VectorPixel v)
    {
        if (!fromPnt.defined)
            fromPnt = v.fromPnt;
        toPnt = v.toPnt;
        int d1;
        int d2;
        if (dx > dy)
        {
            d1 = dx;
            d2 = dy;
        }
        else
        {
            d1 = dy;
            d2 = dx;
        }
        maxleft = Math.Max(maxleft, 1.0 * (d1 - 1) / d2);
        minright = Math.Min(minright, 1.0 * (d1 + 1) / d2);
        var dmin = maxleft * (d2 + 1) + 0.5 - d1;
        var dmax = minright * (d2 + 1) - 0.5 - d1;
        if (Math.Ceiling(dmin) - dmin == 0.5)
            dmin += 0.5;
        if (Math.Ceiling(dmax) - dmax == 0.5)
            dmax -= 0.5;
        if (dx > dy)
        {
            nextmindx = (int)Math.Round(dmin);
            nextmaxdx = (int)Math.Round(dmax);
            nextmindy = 0;
            nextmaxdy = 1;
        }
        else if (dx < dy)
        {
            nextmindx = 0;
            nextmaxdx = 1;
            nextmindy = (int)Math.Round(dmin);
            nextmaxdy = (int)Math.Round(dmax);
        }
        else
        {
            nextmindx = 0;
            nextmaxdx = 2;
            nextmindy = 0;
            nextmaxdy = 2;
        }
        //Console.WriteLine("({0}, {1}) - ({2}, {3})", outerFromPnt.x, outerFromPnt.y, toPnt.x, toPnt.y);
    }

    public void finishLine(Bitmap bmp_out)
    {
        //Console.WriteLine("({0}, {1}) - ({2}, {3})", outerFromPnt.x, outerFromPnt.y, toPnt.x, toPnt.y);
        if (outerFromPnt.x < 0 || outerFromPnt.y < 0 ||
            outerFromPnt.x >= bmp_out.Width || outerFromPnt.y >= bmp_out.Height)
        {
        }
        else
            bmp_out.SetPixel(outerFromPnt.x, outerFromPnt.y, Color.Green);
        if (toPnt.x < 0 || toPnt.y < 0 ||
            toPnt.x >= bmp_out.Width || toPnt.y >= bmp_out.Height)
        {
        }
        else
            bmp_out.SetPixel(toPnt.x, toPnt.y, Color.Green);
    }
}

internal enum IOType
{
    Level,
    Bitmap,
    LevelBitmap,
    None
};

internal struct FromToInt : IComparable
{
    public int px;
    public int sx;
    public int sy;
    public int x;

    public FromToInt(int x, int sx, int sy, int px)
    {
        this.x = x;
        this.sx = sx;
        this.sy = sy;
        this.px = px;
    }

    int IComparable.CompareTo(object o)
    {
        var f = (FromToInt)o;
        return x == f.x ? px.CompareTo(f.px) : x.CompareTo(f.x);
    }
}