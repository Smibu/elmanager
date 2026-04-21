using System;
using OpenTK.Graphics.OpenGL;

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

    public void SetData<T>(T[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) where T : struct
    {
        Buffer.SetData(data, usage);
    }

    public void Dispose()
    {
        Vao.Dispose();
        Buffer.Dispose();
    }
}
