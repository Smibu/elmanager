using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class UniformBuffer : IDisposable
{
    private static GL GL => GlProvider.GL;
    private uint Handle { get; } = GlProvider.GL.GenBuffer();
    private int BindingPoint { get; }

    internal UniformBuffer(int bindingPoint)
    {
        BindingPoint = bindingPoint;
    }

    private void Bind()
    {
        GL.BindBuffer(BufferTargetARB.UniformBuffer, Handle);
    }

    public void BindBufferBase()
    {
        GL.BindBufferBase(BufferTargetARB.UniformBuffer, (uint)BindingPoint, Handle);
    }

    public void SetData<T>(T data) where T : unmanaged
    {
        Bind();
        int size = Marshal.SizeOf<T>();
        GL.BufferData(BufferTargetARB.UniformBuffer, (nuint)size, ref data, BufferUsageARB.DynamicDraw);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}
