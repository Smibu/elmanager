using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;

namespace Elmanager.Rendering;

public record GraphicElement(
    ClippingType Clipping,
    int Distance,
    Vector Position,
    GraphicElementKind Kind
) : IPositionable
{
    public double X => Position.X;
    public double Y => Position.Y;

    public VectorMark Mark
    {
        get => Position.Mark;
        set => Position = Position with { Mark = value };
    }

    public record Picture(DrawableImage PictureInfo);

    public record Texture(DrawableImage TextureInfo, DrawableImage MaskInfo);

    public record MissingPicture(string Name)
    {
        private double? _width;
        private double? _height;
        public double Width => _width ??= DefaultSize.Width * 1 / 48.0;
        public double Height => _height ??= DefaultSize.Height * 1 / 48.0;

        private Size DefaultSize => LgrManager.GetDefaultSize(Name);
    }

    public record MissingTexture(string TextureName, string MaskName)
    {
        private double? _width;
        private double? _height;
        public double Width => _width ??= DefaultSize.Width * 1 / 48.0;
        public double Height => _height ??= DefaultSize.Height * 1 / 48.0;

        private Size DefaultSize => LgrManager.GetDefaultSize(MaskName);
    }

    public static GraphicElement Text(ClippingType clipping, int distance, Vector position, DrawableImage texture,
        DrawableImage mask) =>
        new(clipping, distance, position, new Texture(texture, mask));

    internal static GraphicElement MissingText(string textureName, string maskName, ClippingType clipping, int distance, Vector position) =>
        new(clipping, distance, position, new MissingTexture(textureName, maskName));

    public static GraphicElement Pic(DrawableImage pictureImage, Vector position, int distance,
        ClippingType clipping) => new(clipping, distance, position, new Picture(pictureImage));

    internal static GraphicElement MissingPic(string name, ClippingType clipping, int distance, Vector position) =>
        new(clipping, distance, position, new MissingPicture(name));

    public bool IsPicture => Kind is Picture or MissingPicture;
    public bool IsTexture => Kind is Texture or MissingTexture;

    public double Width => Kind switch
    {
        Picture p => p.PictureInfo.Width,
        Texture t => t.MaskInfo.Width,
        MissingPicture p => p.Width,
        MissingTexture t => t.Width
    };

    public double Height => Kind switch
    {
        Picture p => p.PictureInfo.Height,
        Texture t => t.MaskInfo.Height,
        MissingPicture p => p.Height,
        MissingTexture t => t.Height
    };

    internal GraphicElementFileItem ToFileData() => Kind switch
    {
        Picture p => GraphicElementFileItem.Picture(p.PictureInfo.Name, Position, Distance, Clipping),
        Texture t => GraphicElementFileItem.Texture(t.TextureInfo.Name, t.MaskInfo.Name, Position, Distance, Clipping),
        MissingPicture p => GraphicElementFileItem.Picture(p.Name, Position, Distance, Clipping),
        MissingTexture t => GraphicElementFileItem.Texture(t.TextureName, t.MaskName, Position, Distance, Clipping)
    };

    public Vector Position { get; set; } = Position;
}

public union GraphicElementKind(
    GraphicElement.Picture,
    GraphicElement.Texture,
    GraphicElement.MissingPicture,
    GraphicElement.MissingTexture);
