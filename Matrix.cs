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
        private static readonly Matrix IdentityMatrix;

        static Matrix()
        {
            IdentityMatrix = CreateIdentity();
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

        internal static Matrix Identity
        {
            get { return IdentityMatrix; }
        }

        internal double Determinant
        {
            get { return M11 * M22 - M12 * M21; }
        }

        internal bool HasInverse
        {
            get { return Determinant != 0; }
        }

        internal bool IsIdentity
        {
            get { return M11 == 1 && M12 == 0 && M21 == 0 && M22 == 1 && OffsetX == 0 && OffsetY == 0; }
        }

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

        internal static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            return new Matrix(scaleX, 0, 0, scaleY, (centerX - (scaleX * centerX)), (centerY - (scaleY * centerY)));
        }

        internal static Matrix CreateScaling(double scaleX, double scaleY)
        {
            return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
        }

        internal static Matrix CreateSkewRadians(double skewX, double skewY)
        {
            return new Matrix(1, Math.Tan(skewY), Math.Tan(skewX), 1, 0, 0);
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

        internal void Append(Matrix matrix)
        {
            SetMatrix(this * matrix);
        }

        internal void Invert()
        {
            double determinant = Determinant;
            if (determinant == 0)
                throw (new InvalidOperationException("Matrix is not invertible."));
            double num = 1 / determinant;
            SetMatrix(M22 * num, -M12 * num, -M21 * num, M11 * num, ((M21 * OffsetY) - (OffsetX * M22)) * num,
                      ((OffsetX * M12) - (M11 * OffsetY)) * num);
        }

        internal Vector MultiplyVector(Vector v)
        {
            Vector z = v.Clone();
            z.X = v.X * M11 + v.Y * M21 + OffsetX; 
            z.Y = v.Y * M22 + v.X * M12 + OffsetY;
            return z;
        }

        internal void Prepend(Matrix matrix)
        {
            SetMatrix(matrix * this);
        }

        internal void Rotate(double angle)
        {
            angle = angle % 360;
            SetMatrix(this * CreateRotationRadians(angle * Constants.DegToRad));
        }

        internal void RotateAt(double angle, double centerX, double centerY)
        {
            angle = angle % 360;
            SetMatrix(this * CreateRotationRadians(angle * Constants.DegToRad, centerX, centerY));
        }

        internal void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle = angle % 360;
            SetMatrix(CreateRotationRadians(angle * Constants.DegToRad, centerX, centerY) * this);
        }

        internal void RotatePrepend(double angle)
        {
            angle = angle % 360;
            SetMatrix(CreateRotationRadians(angle * Constants.DegToRad) * this);
        }

        internal void Scale(double scaleX, double scaleY)
        {
            SetMatrix(this * CreateScaling(scaleX, scaleY));
        }

        internal void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            SetMatrix(this * CreateScaling(scaleX, scaleY, centerX, centerY));
        }

        internal void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        {
            SetMatrix(CreateScaling(scaleX, scaleY, centerX, centerY) * this);
        }

        internal void ScalePrepend(double scaleX, double scaleY)
        {
            SetMatrix(CreateScaling(scaleX, scaleY) * this);
        }

        internal void Skew(double skewX, double skewY)
        {
            skewX = skewX % 360;
            skewY = skewY % 360;
            SetMatrix(this * CreateSkewRadians(skewX * Constants.DegToRad, skewY * Constants.DegToRad));
        }

        internal void SkewPrepend(double skewX, double skewY)
        {
            skewX = skewX % 360;
            skewY = skewY % 360;
            SetMatrix(CreateSkewRadians(skewX * Constants.DegToRad, skewY * Constants.DegToRad) * this);
        }

        internal Vector Transform(Vector vector)
        {
            return MultiplyVector(vector);
        }

        internal void Transform(Vector[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                    vectors[i] = MultiplyVector(vectors[i]);
            }
        }

        internal void Translate(double offsetX, double offsetY)
        {
            SetMatrix(this * CreateTranslation(offsetX, offsetY));
        }

        internal void TranslatePrepend(double offsetX, double offsetY)
        {
            SetMatrix(CreateTranslation(offsetX, offsetY) * this);
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

        private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}