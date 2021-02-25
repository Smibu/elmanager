using System.Drawing;

namespace Elmanager.Lgr
{
    internal class LgrImage
    {
        internal Bitmap Bmp;
        internal ClippingType ClippingType;
        internal int Distance;
        internal string Name;
        internal ImageType Type;

        internal LgrImage(string name, Bitmap bmp, ImageType type, int dist, ClippingType ctype)
        {
            Name = name;
            Bmp = bmp;
            Type = type;
            Distance = dist;
            ClippingType = ctype;
        }
    }
}