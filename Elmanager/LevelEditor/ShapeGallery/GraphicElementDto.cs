using Elmanager.Lgr;
using Elmanager.Rendering;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Elmanager.LevelEditor.ShapeGallery;

internal abstract record GraphicElementDto
{
    public string Type { get; set; } = string.Empty;
    public ClippingType Clipping { get; set; }
    public int Distance { get; set; }
    public VectorDto Position { get; set; } = new VectorDto();

    protected GraphicElementDto() { }

    protected GraphicElementDto(GraphicElement element)
    {
        Type = element.GetType().Name;
        Clipping = element.Clipping;
        Distance = element.Distance;
        Position = new VectorDto(element.Position);
    }

    public abstract GraphicElement ToGraphicElement();
}

internal record PictureDto : GraphicElementDto
{
    public DrawableImageDto PictureInfo { get; set; } = new DrawableImageDto();

    public PictureDto() { }

    public PictureDto(GraphicElement.Picture picture) : base(picture)
    {
        PictureInfo = new DrawableImageDto(picture.PictureInfo);
    }

    public override GraphicElement ToGraphicElement()
    {
        return new GraphicElement.Picture(Clipping, Distance, Position.ToVector(), PictureInfo.ToDrawableImage());
    }
}

internal record TextureDto : GraphicElementDto
{
    public DrawableImageDto TextureInfo { get; set; } = new DrawableImageDto();
    public DrawableImageDto MaskInfo { get; set; } = new DrawableImageDto();

    public TextureDto() { }

    public TextureDto(GraphicElement.Texture texture) : base(texture)
    {
        TextureInfo = new DrawableImageDto(texture.TextureInfo);
        MaskInfo = new DrawableImageDto(texture.MaskInfo);
    }

    public override GraphicElement ToGraphicElement()
    {
        return new GraphicElement.Texture(Clipping, Distance, Position.ToVector(), TextureInfo.ToDrawableImage(), MaskInfo.ToDrawableImage());
    }
}

internal record MissingPictureDto : GraphicElementDto
{
    public string Name { get; set; } = string.Empty;

    public MissingPictureDto() { }

    public MissingPictureDto(GraphicElement.MissingPicture missingPicture) : base(missingPicture)
    {
        Name = missingPicture.Name;
    }

    public override GraphicElement ToGraphicElement()
    {
        return new GraphicElement.MissingPicture(Name, Clipping, Distance, Position.ToVector());
    }
}

internal record MissingTextureDto : GraphicElementDto
{
    public string TextureName { get; set; } = string.Empty;
    public string MaskName { get; set; } = string.Empty;

    public MissingTextureDto() { }

    public MissingTextureDto(GraphicElement.MissingTexture missingTexture) : base(missingTexture)
    {
        TextureName = missingTexture.TextureName;
        MaskName = missingTexture.MaskName;
    }

    public override GraphicElement ToGraphicElement()
    {
        return new GraphicElement.MissingTexture(TextureName, MaskName, Clipping, Distance, Position.ToVector());
    }
}
internal record ImageMetaDto
{
    public string Name { get; set; } = string.Empty;
    public ImageType Type { get; set; }
    public ClippingType ClippingType { get; set; }
    public int Distance { get; set; }

    public ImageMetaDto() { }

    public ImageMetaDto(ImageMeta meta)
    {
        Name = meta.Name;
        Type = meta.Type;
        ClippingType = meta.ClippingType;
        Distance = meta.Distance;
    }

    public ImageMeta ToImageMeta()
    {
        return new ImageMeta(Name, Type, ClippingType, Distance);
    }
}

internal record DrawableImageDto
{
    public int TextureId { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public ImageMetaDto Meta { get; set; } = new ImageMetaDto();

    public DrawableImageDto() { }

    public DrawableImageDto(DrawableImage image)
    {
        TextureId = image.TextureId;
        Width = image.Width;
        Height = image.Height;
        Meta = new ImageMetaDto(image.Meta);
    }

    public DrawableImage ToDrawableImage()
    {
        return new DrawableImage(TextureId, Width, Height, Meta.ToImageMeta());
    }
}

internal static class GraphicElementExtensions
{
    public static GraphicElementDto ToDto(this GraphicElement element)
    {
        return element switch
        {
            GraphicElement.Picture picture => new PictureDto(picture),
            GraphicElement.Texture texture => new TextureDto(texture),
            GraphicElement.MissingPicture missingPicture => new MissingPictureDto(missingPicture),
            GraphicElement.MissingTexture missingTexture => new MissingTextureDto(missingTexture),
            _ => throw new ArgumentOutOfRangeException(nameof(element), element, null)
        };
    }

    public static GraphicElement ToGraphicElement(this GraphicElementDto dto)
    {
        return dto switch
        {
            PictureDto pictureDto => pictureDto.ToGraphicElement(),
            TextureDto textureDto => textureDto.ToGraphicElement(),
            MissingPictureDto missingPictureDto => missingPictureDto.ToGraphicElement(),
            MissingTextureDto missingTextureDto => missingTextureDto.ToGraphicElement(),
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, null)
        };
    }
}

internal class GraphicElementDtoConverter : JsonConverter<GraphicElementDto>
{
    public override GraphicElementDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;
        string? type = root.GetProperty("Type").GetString();

        return type switch
        {
            "Picture" => JsonSerializer.Deserialize<PictureDto>(root.GetRawText(), options),
            "Texture" => JsonSerializer.Deserialize<TextureDto>(root.GetRawText(), options),
            "MissingPicture" => JsonSerializer.Deserialize<MissingPictureDto>(root.GetRawText(), options),
            "MissingTexture" => JsonSerializer.Deserialize<MissingTextureDto>(root.GetRawText(), options),
            _ => throw new NotSupportedException($"Type '{type}' is not supported")
        };
    }

    public override void Write(Utf8JsonWriter writer, GraphicElementDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}