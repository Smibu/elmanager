namespace Elmanager
{
    internal class DrawableImage
    {
        internal readonly ClippingType DefaultClipping;
        internal readonly int DefaultDistance;
        internal readonly double Height;
        internal readonly string Name;
        internal readonly int TextureIdentifier;
        internal readonly Lgr.ImageType Type;
        internal readonly double Width;

        internal DrawableImage(int textureId, double width, double height, ClippingType clipping,
            int distance,
            string name, Lgr.ImageType type)
        {
            TextureIdentifier = textureId;
            Width = width;
            Height = height;
            DefaultClipping = clipping;
            DefaultDistance = distance;
            Name = name;
            Type = type;
        }

        internal double WidthMinusMargin => Width - 2 * EmptyPixelXMargin;

        internal double HeightMinusMargin => Height - 2 * EmptyPixelYMargin;

        internal double EmptyPixelXMargin
        {
            get
            {
                switch (Name)
                {
                    case "maskhor":
                        return 0.029;
                    case "masklitt":
                        return 0.015;
                    case "maskbig":
                        return 0.092;
                    default:
                        return 0.092;
                }
            }
        }

        internal double EmptyPixelYMargin
        {
            get
            {
                switch (Name)
                {
                    case "maskhor":
                        return 0.029;
                    case "masklitt":
                        return 0.015;
                    case "maskbig":
                        return 0.112;
                    default:
                        return 0.112;
                }
            }
        }
    }
}