using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Elmanager.Rendering;

[StructLayout(LayoutKind.Sequential)]
internal struct ColorUniform(Color color)
{
    public Vector4 Color = new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
}
