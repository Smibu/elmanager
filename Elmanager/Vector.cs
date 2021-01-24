using System;
using NetTopologySuite.Geometries;

namespace Elmanager
{
    [Serializable]
    internal struct Vector
    {
        internal static VectorMark MarkDefault = VectorMark.None;
        internal VectorMark Mark;
        internal double X;
        internal double Y;

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
            X = Math.Cos(angle * Constants.DegToRad);
            Y = Math.Sin(angle * Constants.DegToRad);
            Mark = MarkDefault;
        }

        internal double Angle
        {
            get
            {
                double ang = Math.Atan2(Y, X) * Constants.RadToDeg;
                return ang;
            }
        }

        internal double AnglePositive
        {
            get
            {
                double ang = Math.Atan2(Y, X) * Constants.RadToDeg;
                if (ang < 0)
                    ang += 360;
                return ang;
            }
        }

        internal double Length => Math.Sqrt(X * X + Y * Y);

        internal double LengthSquared => X * X + Y * Y;

        public void Transform(Matrix m)
        {
            var transformed = this * m;
            X = transformed.X;
            Y = transformed.Y;
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
            return (Math.Atan2(y, x) * Constants.RadToDeg);
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
    }
}