using System.Numerics;
using System.Runtime.InteropServices;

namespace Elmanager.Rendering;

[StructLayout(LayoutKind.Sequential)]
internal struct CameraUniforms(Matrix4x4 projection, Vector2 cameraPosition, float grassZoom, float zoom, float pointSize)
{
    public Matrix4x4 Projection = projection;
    public Vector2 CameraPosition = cameraPosition;
    public float GrassZoom = grassZoom;
    public float Zoom = zoom;
    public float PointSize = pointSize;
}
