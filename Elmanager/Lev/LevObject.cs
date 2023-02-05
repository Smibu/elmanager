using System;
using Elmanager.Geometry;

namespace Elmanager.Lev;

internal class LevObject : IPositionable
{
    internal int AnimationNumber;
    internal AppleType AppleType;
    public Vector Position { get; set; }
    internal readonly ObjectType Type;

    internal LevObject(Vector position, ObjectType type, AppleType appleType, int animNum = 1)
    {
        Position = position;
        Type = type;
        AppleType = appleType;
        AnimationNumber = Math.Min(Math.Max(animNum, 1), 9);
    }

    private LevObject(LevObject o)
    {
        AnimationNumber = o.AnimationNumber;
        AppleType = o.AppleType;
        Position = o.Position.Clone();
        Type = o.Type;
    }

    internal static LevObject ExitObject(Vector exitPosition)
    {
        return new(exitPosition, ObjectType.Flower, AppleType.Normal);
    }

    internal static LevObject StartObject(Vector startPosition)
    {
        return new(startPosition, ObjectType.Start, AppleType.Normal);
    }

    internal LevObject Clone()
    {
        return new(this);
    }

    public double X => Position.X;
    public double Y => Position.Y;

    public VectorMark Mark
    {
        get => Position.Mark;
        set => Position = Position with { Mark = value };
    }
}