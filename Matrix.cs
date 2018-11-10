using System;

namespace Elmanager
{
    internal struct Matrix
    {
        internal double M11;
        internal double M12;
        internal double M21;
        internal double M22;
        internal double OffsetX;
        internal double OffsetY;

        static Matrix()
        {
            Identity = CreateIdentity();
        }

        internal Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        internal static Matrix Identity { get; }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return ((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) &&
                     ((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22))) &&
                    (matrix1.OffsetX == matrix2.OffsetX)) && (matrix1.OffsetY == matrix2.OffsetY);
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static Matrix operator *(Matrix trans1, Matrix trans2)
        {
            return MultiplyMatrix(trans1, trans2);
        }

        internal static Matrix CreateRotationRadians(double angle, double centerX = 0, double centerY = 0)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return new Matrix(cos, -sin, sin, cos, centerX * cos - centerX + centerY * sin,
                              centerY * cos - centerY - centerX * sin);
        }

        internal static Matrix CreateScaling(double scaleX, double scaleY)
        {
            return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
        }

        internal static Matrix CreateTranslation(double offsetX, double offsetY)
        {
            return new Matrix(1, 0, 0, 1, offsetX, offsetY);
        }

        internal static Matrix MultiplyMatrix(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)),
                              ((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)),
                              ((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)),
                              ((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)),
                              (((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) +
                               matrix2.OffsetX),
                              (((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) +
                               matrix2.OffsetY));
        }

        internal Vector MultiplyVector(Vector v)
        {
            Vector z = v.Clone();
            z.X = v.X * M11 + v.Y * M21 + OffsetX; 
            z.Y = v.Y * M22 + v.X * M12 + OffsetY;
            return z;
        }

        internal void Rotate(double angle)
        {
            angle = angle % 360;
            SetMatrix(this * CreateRotationRadians(angle * Constants.DegToRad));
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
            return new Matrix(1, 0, 0, 1, 0, 0);
        }

        private void SetMatrix(Matrix m)
        {
            M11 = m.M11;
            M12 = m.M12;
            M21 = m.M21;
            M22 = m.M22;
            OffsetX = m.OffsetX;
            OffsetY = m.OffsetY;
        }
    }
}