using Elmanager.Lev;

namespace Elmanager.LevelEditor.ShapeGallery;

internal class LevObjectDto
{
    public VectorDto Position { get; set; }
    public ObjectType Type { get; set; }
    public AppleType AppleType { get; set; }
    public int AnimationNumber { get; set; }

    public LevObjectDto()
    {
        Position = new VectorDto();
    }

    public LevObjectDto(LevObject levObject)
    {
        Position = new VectorDto(levObject.Position);
        Type = levObject.Type;
        AppleType = levObject.AppleType;
        AnimationNumber = levObject.AnimationNumber;
    }

    public LevObject ToLevObject()
    {
        return new LevObject(Position.ToVector(), Type, AppleType, AnimationNumber);
    }
}