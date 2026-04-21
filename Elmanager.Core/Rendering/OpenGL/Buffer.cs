using System;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class Buffer(BufferTarget type) : IDisposable
{
    internal int Handle { get; } = GL.GenBuffer();
    private BufferTarget Type { get; } = type;
    public int Count { get; private set; }

    public void Bind()
    {
        GL.BindBuffer(Type, Handle);
    }

    public void SetData<T>(T[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) where T : struct
    {
        Bind();
        GL.BufferData(Type, data.Length * System.Runtime.InteropServices.Marshal.SizeOf<T>(), data, usage);
        Count = data.Length;
    }

    public static Buffer CreateIndex(uint[] data)
    {
        var buffer = new Buffer(BufferTarget.ElementArrayBuffer);
        buffer.SetData(data, BufferUsageHint.StaticDraw);
        return buffer;
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}
