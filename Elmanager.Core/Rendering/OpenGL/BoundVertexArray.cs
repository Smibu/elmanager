using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class BoundVertexArray : IDisposable
{
    private VertexArray Vao { get; }
    public Buffer Buffer { get; }

    internal BoundVertexArray(VertexArray vao, Buffer buffer)
    {
        Vao = vao;
        Buffer = buffer;
    }

    public int Count => Buffer.Count;

    public void Bind()
    {
        Vao.Bind();
    }

    public void SetData<T>(T[] data, BufferUsageARB usage = BufferUsageARB.DynamicDraw) where T : unmanaged
    {
        Buffer.SetData(data, usage);
    }

    public void Dispose()
    {
        Vao.Dispose();
        Buffer.Dispose();
    }
}
