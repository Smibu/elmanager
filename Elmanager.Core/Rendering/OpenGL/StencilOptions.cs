using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal readonly struct StencilOptions
{
    public StencilOp StencilFail { get; init; }
    public StencilOp DepthFail { get; init; }
    public StencilOp Pass { get; init; }
    public StencilFunction Compare { get; init; }
    public uint ReadMask { get; init; }
    public uint WriteMask { get; init; }
    public uint Reference { get; init; }
}
