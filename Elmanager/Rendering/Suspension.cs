namespace Elmanager.Rendering
{
    internal class Suspension
    {
        internal double Height;
        internal double OffsetX;
        internal int TextureIdentifier;
        internal int WheelNumber;
        internal double X;
        internal double Y;

        internal Suspension(int textureId, double x, double y, double height, double offsetX, int wheelNumber)
        {
            TextureIdentifier = textureId;
            X = x;
            Y = y;
            Height = height;
            OffsetX = offsetX;
            WheelNumber = wheelNumber;
        }
    }
}