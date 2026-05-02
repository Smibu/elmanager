using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class Buffer(BufferTargetARB type) : IDisposable
{
    private static GL GL => GlProvider.GL;
    internal uint Handle { get; } = GlProvider.GL.GenBuffer();
    private BufferTargetARB Type { get; } = type;
    public int Count { get; private set; }

    public void Bind()
    {
        GL.BindBuffer(Type, Handle);
    }

    public void SetData<T>(T[] data, BufferUsageARB usage = BufferUsageARB.DynamicDraw) where T : unmanaged
    {
        Bind();
        GL.BufferData(Type, (nuint)(data.Length * System.Runtime.InteropServices.Marshal.SizeOf<T>()), data, usage);
        Count = data.Length;
    }

    public static Buffer CreateIndex(uint[] data)
    {
        var buffer = new Buffer(BufferTargetARB.ElementArrayBuffer);
        buffer.SetData(data, BufferUsageARB.StaticDraw);
        return buffer;
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}
