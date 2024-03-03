using System.Drawing;
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
        internal override double Width => MaskInfo.Width;
        internal override double Height => MaskInfo.Height;

        internal override GraphicElementFileItem ToFileData() =>
            GraphicElementFileItem.Texture(TextureInfo.Name, MaskInfo.Name, Position, Distance, Clipping);
    }

    internal record MissingPicture(
        string Name,
        ClippingType Clipping,
        int Distance,
        Vector Position
    ) : GraphicElement(Clipping, Distance, Position)
    {
        private double? _width;
        private double? _height;
        internal override double Width => _width ??= DefaultSize.Width * 1 / 48.0;
        internal override double Height => _height ??= DefaultSize.Height * 1 / 48.0;

        private Size DefaultSize => LgrManager.GetDefaultSize(Name);

        internal override GraphicElementFileItem ToFileData() =>
            GraphicElementFileItem.Picture(Name, Position, Distance, Clipping);
    }

    internal record MissingTexture(
        string TextureName,
        string MaskName,
        ClippingType Clipping,
        int Distance,
        Vector Position
    ) : GraphicElement(Clipping, Distance, Position)
    {
        private double? _width;
        private double? _height;
        internal override double Width => _width ??= DefaultSize.Width * 1 / 48.0;
        internal override double Height => _height ??= DefaultSize.Height * 1 / 48.0;

        private Size DefaultSize => LgrManager.GetDefaultSize(MaskName);

        internal override GraphicElementFileItem ToFileData() =>
            GraphicElementFileItem.Texture(TextureName, MaskName, Position, Distance, Clipping);
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

    internal static MissingTexture MissingText(string textureName, string maskName, ClippingType clipping, int distance, Vector position) =>
        new(
            textureName,
            maskName,
            clipping,
            distance,
            position
        );

    internal static Picture Pic(DrawableImage pictureImage, Vector position, int distance,
        ClippingType clipping) => new(clipping, distance, position, pictureImage);

    internal static MissingPicture MissingPic(string name, ClippingType clipping, int distance, Vector position) =>
        new(
            name,
            clipping,
            distance,
            position
        );

    internal abstract double Width { get; }
    internal abstract double Height { get; }
    internal abstract GraphicElementFileItem ToFileData();
    public Vector Position { get; set; } = Position;
}