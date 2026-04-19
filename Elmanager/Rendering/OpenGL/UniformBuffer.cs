using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class UniformBuffer : IDisposable
{
    private int Handle { get; } = GL.GenBuffer();
    private int BindingPoint { get; }

    internal UniformBuffer(int bindingPoint)
    {
        BindingPoint = bindingPoint;
    }

    private void Bind()
    {
        GL.BindBuffer(BufferTarget.UniformBuffer, Handle);
    }

    public void BindBufferBase()
    {
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, BindingPoint, Handle);
    }

    public void SetData<T>(T data) where T : struct
    {
        Bind();
        int size = Marshal.SizeOf<T>();
        GL.BufferData(BufferTarget.UniformBuffer, size, ref data, BufferUsageHint.DynamicDraw);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}
