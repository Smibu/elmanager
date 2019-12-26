using System;

namespace Elmanager
{
    internal class LevObject
    {
        internal int AnimationNumber;
        internal AppleType AppleType;
        internal Vector Position;
        internal ObjectType Type;

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
            return new LevObject(exitPosition, ObjectType.Flower, AppleType.Normal);
        }

        internal static LevObject StartObject(Vector startPosition)
        {
            return new LevObject(startPosition, ObjectType.Start, AppleType.Normal);
        }

        internal LevObject Clone()
        {
            return new LevObject(this);
        }
    }
}