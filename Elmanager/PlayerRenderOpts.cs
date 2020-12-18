using System.Drawing;

namespace Elmanager
{
    internal struct PlayerRenderOpts
    {
        public Color color;
        public bool isActive;
        public bool useGraphics;
        public bool useTransparency;

        public PlayerRenderOpts(Color color, bool isActive, bool useGraphics)
        {
            this.isActive = isActive;
            this.color = color;
            this.useGraphics = useGraphics;
            useTransparency = true;
        }
    }
}