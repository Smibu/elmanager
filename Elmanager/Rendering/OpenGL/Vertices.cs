using System;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class Vertices(
    BoundVertexArray vertexArray,
    Buffer indexBuffer,
    PrimitiveType primitiveType)
    : IDisposable
{
    public BoundVertexArray VertexArray { get; } = vertexArray;
    internal Buffer IndexBuffer { get; } = indexBuffer;
    private PrimitiveType PrimitiveType { get; } = primitiveType;

    public void Bind()
    {
        VertexArray.Bind();
        IndexBuffer.Bind();
    }

    public void DrawInstanced(int instanceCount)
    {
        GL.DrawElementsInstanced(PrimitiveType, IndexBuffer.Count, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceCount);
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        IndexBuffer.Dispose();
    }
}
