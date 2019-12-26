namespace Elmanager
{
    internal class LevelFileTexture
    {
        internal ClippingType Clipping;
        internal int Distance;
        internal string MaskName;
        internal string Name;
        internal Vector Position;

        internal LevelFileTexture(string name, string maskName, Vector position, int distance,
            ClippingType clipping)
        {
            Name = name;
            MaskName = maskName;
            Position = position;
            Distance = distance;
            Clipping = clipping;
        }

        internal bool IsTexture => MaskName != null;
        internal bool IsPicture => !IsTexture;
    }
}