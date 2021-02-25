using Elmanager.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.Camera
{
    internal class ElmaCamera
    {
        private readonly int[] _viewport = new int[4];
        internal double CenterX;
        internal double CenterY;
        internal double XMax => CenterX + ZoomLevel * AspectRatio;

        internal double AspectRatio
        {
            get
            {
                GL.GetInteger(GetPName.Viewport, _viewport);
                return _viewport[2] / (double) _viewport[3];
            }
        }

        internal double XMin => CenterX - ZoomLevel * AspectRatio;

        internal double XSize => XMax - XMin;

        internal double YMax => CenterY + ZoomLevel;

        internal double YMin => CenterY - ZoomLevel;

        internal double YSize => YMax - YMin;

        internal double ZoomLevel;

        internal int ViewPortWidth => _viewport[2];
        internal int ViewPortHeight => _viewport[3];

        internal int[] Viewport => _viewport;

        internal Vector FixJitter()
        {
            var fixx = CenterX % (2 * ZoomLevel * AspectRatio / ViewPortWidth);
            var fixy = CenterY % (2 * ZoomLevel / ViewPortHeight);
            CenterX -= fixx;
            CenterY -= fixy;
            return new Vector(fixx, fixy);
        }
    }
}