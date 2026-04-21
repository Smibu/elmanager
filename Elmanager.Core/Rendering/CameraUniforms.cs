using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Elmanager.Rendering;

[StructLayout(LayoutKind.Sequential)]
internal struct CameraUniforms(Matrix4 projection, Vector2 cameraPosition, float grassZoom, float zoom)
{
    public Matrix4 Projection = projection;
    public Vector2 CameraPosition = cameraPosition;
    public float GrassZoom = grassZoom;
    public float Zoom = zoom;
}
