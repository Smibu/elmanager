using System.Drawing;

namespace Elmanager
{
    internal struct PlayerRenderOpts
    {
        public Color Color;
        public bool IsActive;
        public bool UseGraphics;
        public bool UseTransparency;

        public PlayerRenderOpts(Color color, bool isActive, bool useGraphics)
        {
            IsActive = isActive;
            Color = color;
            UseGraphics = useGraphics;
            UseTransparency = true;
        }
    }
}