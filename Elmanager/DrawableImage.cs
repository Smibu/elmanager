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

        internal double EmptyPixelXMargin =>
            Name switch
            {
                "maskhor" => 0.029,
                "masklitt" => 0.015,
                "maskbig" => 0.092,
                _ => 0.092
            };

        internal double EmptyPixelYMargin =>
            Name switch
            {
                "maskhor" => 0.029,
                "masklitt" => 0.015,
                "maskbig" => 0.112,
                _ => 0.112
            };
    }
}