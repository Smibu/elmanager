using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class Vertices(
    BoundVertexArray vertexArray,
    Buffer indexBuffer,
    PrimitiveType primitiveType)
    : IDisposable
{
    private static GL GL => GlProvider.GL;
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
        unsafe
        {
            GL.DrawElementsInstanced(PrimitiveType, (uint)IndexBuffer.Count, DrawElementsType.UnsignedInt, (void*)0, (uint)instanceCount);
        }
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        IndexBuffer.Dispose();
    }
}
