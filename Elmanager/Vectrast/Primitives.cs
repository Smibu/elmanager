#nullable disable

using System;
using static System.Math;

namespace Elmanager.Vectrast;

internal readonly struct IntVector2 : IComparable
{
    public readonly bool Defined;
    public readonly int X;
    public readonly int Y;

    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
        Defined = true;
    }

    int IComparable.CompareTo(object o)
    {
        // lexicographic
        var v = (IntVector2)o;
        if (X < v.X || X == v.X && Y < v.Y)
            return -1;
        if (X > v.X || X == v.X && Y > v.Y)
            return 1;
        return 0;
    }

    public static IntVector2 operator -(IntVector2 v1, IntVector2 v2) => new(v1.X - v2.X, v1.Y - v2.Y);

    public bool Extension(IntVector2 otherVector) => X * Y == 0 && X * otherVector.Y - Y * otherVector.X == 0; // parallel to either axis AND colinear

    public override int GetHashCode() => X + Y * 7919;

    public override bool Equals(object o) => o is IntVector2 v && X == v.X && Y == v.Y;
}

internal struct DoubleVector2
{
    public double X;
    public double Y;

    private DoubleVector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public DoubleVector2(IntVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public static DoubleVector2 operator -(DoubleVector2 v1, DoubleVector2 v2) => new(v1.X - v2.X, v1.Y - v2.Y);
}

internal class Matrix2D
{
    private readonly double[,] _elements;

    private Matrix2D()
    {
        _elements = new double[3, 3];
    }

    private static Matrix2D IdentityM()
    {
        var matrix = new Matrix2D();
        for (var i = 0; i < 3; i++)
            matrix._elements[i, i] = 1;
        return matrix;
    }

    public static Matrix2D TranslationM(double x, double y)
    {
        var matrix = IdentityM();
        matrix._elements[0, 2] = x;
        matrix._elements[1, 2] = y;
        return matrix;
    }

    public static Matrix2D ScaleM(double x, double y)
    {
        var matrix = new Matrix2D
        {
            _elements =
            {
                [0, 0] = x,
                [1, 1] = y,
                [2, 2] = 1
            }
        };
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
                    sum += m1._elements[k, j] * m2._elements[i, k];
                matrix._elements[i, j] = sum;
            }
        return matrix;
    }

    public static DoubleVector2 operator *(DoubleVector2 v, Matrix2D m)
    {
        var vector = new DoubleVector2
        {
            X = v.X * m._elements[0, 0] + v.Y * m._elements[0, 1] + m._elements[0, 2],
            Y = v.X * m._elements[1, 0] + v.Y * m._elements[1, 1] + m._elements[1, 2]
        };
        return vector;
    }
}

internal class VectorPixel
{
    public IntVector2 FromPnt;
    public IntVector2 ToPnt;

    protected VectorPixel()
    {
    }

    public VectorPixel(IntVector2 fromPnt, IntVector2 toPnt)
    {
        FromPnt = fromPnt;
        ToPnt = toPnt;
    }

    protected int Dx => Abs(ToPnt.X - FromPnt.X) + 1;

    protected int Dy => Abs(ToPnt.Y - FromPnt.Y) + 1;

    public bool LinkVector() => Dx == 2 && Dy == 2;
}

internal class Line : VectorPixel
{
    private double _maxleft;
    private double _minright;
    private int _nextmaxdx;
    private int _nextmaxdy;
    private int _nextmindx;
    private int _nextmindy;
    private readonly IntVector2 _outerFromPnt;

    public Line(VectorPixel v)
    {
        _maxleft = double.MinValue;
        _minright = double.MaxValue;
        _outerFromPnt = v.FromPnt;
        ToPnt = v.ToPnt;
        _nextmindx = 0;
        _nextmaxdx = int.MaxValue;
        _nextmindy = 0;
        _nextmaxdy = int.MaxValue;
    }

    public bool SatisfiesOuter(VectorPixel v)
    {
        return
            Abs(v.ToPnt.X - ToPnt.X) <= _nextmaxdx &&
            Abs(v.ToPnt.Y - ToPnt.Y) <= _nextmaxdy; // outer segment must be shorter than inner segment
    }

    public bool SatisfiesInner(VectorPixel v)
    {
        if (!FromPnt.Defined)
            return Abs(ToPnt.X - _outerFromPnt.X) == Abs(ToPnt.Y - _outerFromPnt.Y) || Abs(v.ToPnt.X - ToPnt.X) >= Abs(v.FromPnt.X - _outerFromPnt.X) &&
                Abs(v.ToPnt.Y - ToPnt.Y) >= Abs(v.FromPnt.Y - _outerFromPnt.Y);
        // inner segment must be >= the outer segment
        return
            _nextmindx <= Abs(v.ToPnt.X - ToPnt.X) && Abs(v.ToPnt.X - ToPnt.X) <= _nextmaxdx &&
            _nextmindy <= Abs(v.ToPnt.Y - ToPnt.Y) && Abs(v.ToPnt.Y - ToPnt.Y) <= _nextmaxdy;
    }

    public bool SameDir(VectorPixel v)
    {
        if (v.LinkVector())
            return
                Abs(Sign(ToPnt.X - _outerFromPnt.X) - (v.ToPnt.X - ToPnt.X)) <= 1 &&
                Abs(Sign(ToPnt.Y - _outerFromPnt.Y) - (v.ToPnt.Y - ToPnt.Y)) <= 1;
        if (Abs(ToPnt.X - _outerFromPnt.X) > Abs(ToPnt.Y - _outerFromPnt.Y))
            return
                Sign(ToPnt.X - _outerFromPnt.X) == Sign(v.ToPnt.X - ToPnt.X) &&
                Abs(Sign(ToPnt.Y - _outerFromPnt.Y) - (v.ToPnt.Y - ToPnt.Y)) <= 1;
        if (Abs(ToPnt.X - _outerFromPnt.X) < Abs(ToPnt.Y - _outerFromPnt.Y))
            return
                Abs(Sign(ToPnt.X - _outerFromPnt.X) - (v.ToPnt.X - ToPnt.X)) <= 1 &&
                Sign(ToPnt.Y - _outerFromPnt.Y) == Sign(v.ToPnt.Y - ToPnt.Y);
        return
            Abs(Sign(ToPnt.X - _outerFromPnt.X) - Sign(v.ToPnt.X - ToPnt.X)) <= 1 &&
            Abs(Sign(ToPnt.Y - _outerFromPnt.Y) - Sign(v.ToPnt.Y - ToPnt.Y)) <= 1;
    }

    public void Update(VectorPixel v)
    {
        if (!FromPnt.Defined)
            FromPnt = v.FromPnt;
        ToPnt = v.ToPnt;
        int d1;
        int d2;
        if (Dx > Dy)
        {
            d1 = Dx;
            d2 = Dy;
        }
        else
        {
            d1 = Dy;
            d2 = Dx;
        }
        _maxleft = Max(_maxleft, 1.0 * (d1 - 1) / d2);
        _minright = Min(_minright, 1.0 * (d1 + 1) / d2);
        var dmin = _maxleft * (d2 + 1) + 0.5 - d1;
        var dmax = _minright * (d2 + 1) - 0.5 - d1;
        if (Ceiling(dmin) - dmin == 0.5)
            dmin += 0.5;
        if (Ceiling(dmax) - dmax == 0.5)
            dmax -= 0.5;
        if (Dx > Dy)
        {
            _nextmindx = (int)Round(dmin);
            _nextmaxdx = (int)Round(dmax);
            _nextmindy = 0;
            _nextmaxdy = 1;
        }
        else if (Dx < Dy)
        {
            _nextmindx = 0;
            _nextmaxdx = 1;
            _nextmindy = (int)Round(dmin);
            _nextmaxdy = (int)Round(dmax);
        }
        else
        {
            _nextmindx = 0;
            _nextmaxdx = 2;
            _nextmindy = 0;
            _nextmaxdy = 2;
        }
    }
}