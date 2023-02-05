using System;
using Elmanager.Utilities;

namespace Elmanager.Geometry;

internal struct Matrix
{
    private double _m11;
    private double _m12;
    private double _m21;
    private double _m22;
    private double _offsetX;
    private double _offsetY;

    static Matrix()
    {
        Identity = CreateIdentity();
    }

    public override bool Equals(object? o)
    {
        if (o is not Matrix matrix)
            return false;

        return this == matrix;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    private Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
    {
        _m11 = m11;
        _m12 = m12;
        _m21 = m21;
        _m22 = m22;
        _offsetX = offsetX;
        _offsetY = offsetY;
    }

    internal static Matrix Identity { get; }

    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        return matrix1._m11 == matrix2._m11 && matrix1._m12 == matrix2._m12 && matrix1._m21 == matrix2._m21 &&
               matrix1._m22 == matrix2._m22 && matrix1._offsetX == matrix2._offsetX &&
               matrix1._offsetY == matrix2._offsetY;
    }

    public static bool operator !=(Matrix matrix1, Matrix matrix2)
    {
        return !(matrix1 == matrix2);
    }

    public static Matrix operator *(Matrix trans1, Matrix trans2)
    {
        return MultiplyMatrix(trans1, trans2);
    }

    private static Matrix CreateRotationRadians(double angle, double centerX = 0, double centerY = 0)
    {
        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        return new Matrix(cos, -sin, sin, cos, centerX * cos - centerX + centerY * sin,
            centerY * cos - centerY - centerX * sin);
    }

    internal static Matrix CreateScaling(double scaleX, double scaleY)
    {
        return new(scaleX, 0, 0, scaleY, 0, 0);
    }

    internal static Matrix CreateTranslation(double offsetX, double offsetY)
    {
        return new(1, 0, 0, 1, offsetX, offsetY);
    }

    private static Matrix MultiplyMatrix(Matrix matrix1, Matrix matrix2)
    {
        return new(
            matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
            matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,
            matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
            matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,
            matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 +
            matrix2._offsetX,
            matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 +
            matrix2._offsetY);
    }

    private Vector MultiplyVector(Vector v)
    {
        return new() { X = v.X * _m11 + v.Y * _m21 + _offsetX, Y = v.Y * _m22 + v.X * _m12 + _offsetY, Mark = v.Mark };
    }

    internal void Rotate(double angle)
    {
        angle = angle % 360;
        SetMatrix(this * CreateRotationRadians(angle * MathUtils.DegToRad));
    }

    internal void Scale(double scaleX, double scaleY)
    {
        SetMatrix(this * CreateScaling(scaleX, scaleY));
    }

    internal Vector Transform(Vector vector)
    {
        return MultiplyVector(vector);
    }

    internal void Translate(double offsetX, double offsetY)
    {
        SetMatrix(this * CreateTranslation(offsetX, offsetY));
    }

    private static Matrix CreateIdentity()
    {
        return new(1, 0, 0, 1, 0, 0);
    }

    private void SetMatrix(Matrix m)
    {
        _m11 = m._m11;
        _m12 = m._m12;
        _m21 = m._m21;
        _m22 = m._m22;
        _offsetX = m._offsetX;
        _offsetY = m._offsetY;
    }
}