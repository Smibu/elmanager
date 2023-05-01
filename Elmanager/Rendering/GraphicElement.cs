using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal abstract record GraphicElement(
    ClippingType Clipping,
    int Distance,
    Vector Position
) : IPositionable
{
    public double X => Position.X;
    public double Y => Position.Y;

    public VectorMark Mark
    {
        get => Position.Mark;
        set => Position = Position with { Mark = value };
    }

    internal record Picture(
        ClippingType Clipping,
        int Distance,
        Vector Position,
        DrawableImage PictureInfo
    ) : GraphicElement(Clipping, Distance, Position)
    {
        internal override double Width => PictureInfo.Width;
        internal override double Height => PictureInfo.Height;

        internal override GraphicElementFileItem ToFileData() =>
            GraphicElementFileItem.Picture(PictureInfo.Name, Position, Distance, Clipping);
    }

    internal record Texture(
        ClippingType Clipping,
        int Distance,
        Vector Position,
        DrawableImage TextureInfo,
        DrawableImage MaskInfo
    ) : GraphicElement(Clipping, Distance, Position)
    {
        internal double AspectRatio => TextureInfo.AspectRatio;
        internal override double Width => MaskInfo.Width;
        internal override double Height => MaskInfo.Height;

        internal override GraphicElementFileItem ToFileData() =>
            GraphicElementFileItem.Texture(TextureInfo.Name, MaskInfo.Name, Position, Distance, Clipping);
    }

    internal static Texture Text(ClippingType clipping, int distance, Vector position, DrawableImage texture,
        DrawableImage mask) =>
        new(
            clipping,
            distance,
            position,
            texture,
            mask
        );

    internal static Picture Pic(DrawableImage pictureImage, Vector position, int distance,
        ClippingType clipping) => new(clipping, distance, position, pictureImage);

    internal abstract double Width { get; }
    internal abstract double Height { get; }
    internal abstract GraphicElementFileItem ToFileData();
    public Vector Position { get; set; } = Position;
}