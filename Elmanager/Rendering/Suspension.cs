namespace Elmanager.Rendering;

internal class Suspension
{
    internal readonly double Height;
    internal readonly double OffsetX;
    internal readonly int TextureIdentifier;
    internal readonly int WheelNumber;
    internal readonly double X;
    internal readonly double Y;

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