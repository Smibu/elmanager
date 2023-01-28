using System;
using Elmanager.Utilities;
using NetTopologySuite.Geometries;

namespace Elmanager.Geometry;

internal struct Vector : IPositionable
{
    internal static VectorMark MarkDefault = VectorMark.None;

    internal Vector(double x, double y)
    {
        X = x;
        Y = y;
        Mark = MarkDefault;
    }

    internal Vector(double x, double y, VectorMark mark)
    {
        X = x;
        Y = y;
        Mark = mark;
    }

    internal Vector(double angle)
    {
        X = Math.Cos(angle * MathUtils.DegToRad);
        Y = Math.Sin(angle * MathUtils.DegToRad);
        Mark = MarkDefault;
    }

    internal double Angle
    {
        get
        {
            double ang = Math.Atan2(Y, X) * MathUtils.RadToDeg;
            return ang;
        }
    }

    internal double AnglePositive
    {
        get
        {
            double ang = Math.Atan2(Y, X) * MathUtils.RadToDeg;
            if (ang < 0)
                ang += 360;
            return ang;
        }
    }

    internal double Length => Math.Sqrt(X * X + Y * Y);

    internal double LengthSquared => X * X + Y * Y;

    public Vector Transform(Matrix m)
    {
        return this * m;
    }

    public void SetPosition(Vector v)
    {
        X = v.X;
        Y = v.Y;
    }

    public static implicit operator Coordinate(Vector v)
    {
        return new(v.X, v.Y);
    }

    public static Vector operator +(Vector vector1, Vector vector2)
    {
        return new(vector1.X + vector2.X, vector1.Y + vector2.Y);
    }

    public static Vector operator /(Vector vector, double scalar)
    {
        return (vector * (1 / scalar));
    }

    public static Vector operator *(Vector vector, double scalar)
    {
        return new((vector.X * scalar), (vector.Y * scalar));
    }

    public static Vector operator *(double scalar, Vector vector)
    {
        return new((vector.X * scalar), (vector.Y * scalar));
    }

    public static Vector operator *(Vector vector, Matrix matrix)
    {
        return matrix.Transform(vector);
    }

    public static double operator *(Vector vector1, Vector vector2)
    {
        return vector1.X * vector2.X + vector1.Y * vector2.Y;
    }

    public static Vector operator -(Vector vector1, Vector vector2)
    {
        return new(vector1.X - vector2.X, vector1.Y - vector2.Y);
    }

    public static Vector operator -(Vector vector)
    {
        return new(-vector.X, -vector.Y);
    }

    internal static double CrossProduct(Vector vector1, Vector vector2)
    {
        return vector1.X * vector2.Y - vector1.Y * vector2.X;
    }

    internal double AngleBetween(Vector vector1)
    {
        double y = (vector1.X * Y) - (X * vector1.Y);
        double x = (vector1.X * X) + (vector1.Y * Y);
        return (Math.Atan2(y, x) * MathUtils.RadToDeg);
    }

    internal Vector Clone()
    {
        return new(X, Y, Mark);
    }

    internal void Select()
    {
        Mark = VectorMark.Selected;
    }

    internal Vector Unit()
    {
        return this / Length;
    }

    internal double Dist(Vector other)
    {
        var xd = X - other.X;
        var yd = Y - other.Y;
        return Math.Sqrt(xd * xd + yd * yd);
    }

    public override string ToString()
    {
        return $"({X:F3}, {Y:F3})";
    }

    public double X { get; set; }
    public double Y { get; set; }
    public VectorMark Mark { get; set; }
    public Vector Position => this;

    public Coordinate ToCoordinate() => new(X, Y);
}