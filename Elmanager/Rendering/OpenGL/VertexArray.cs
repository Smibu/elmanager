using System;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class VertexArray : IDisposable
{
    private int Handle { get; } = GL.GenVertexArray();
    private const int InstanceBinding = 1;

    private VertexArray()
    {
    }

    public static BoundVertexArray Create(VertexInfo info, float[]? data)
    {
        var buffer = new Buffer(BufferTarget.ArrayBuffer);
        var vao = new VertexArray();
        vao.ConfigureVertexAttributes(buffer, info);
        var boundVao = new BoundVertexArray(vao, buffer);
        if (data != null)
            boundVao.SetData(data, BufferUsageHint.StaticDraw);
        return boundVao;
    }

    public static BoundVertexArray CreateInstanced(Vertices geometry, VertexInfo perVertexInfo, VertexInfo instanceInfo)
    {
        var instanceBuffer = new Buffer(BufferTarget.ArrayBuffer);
        var vao = new VertexArray();
        vao.ConfigureVertexAttributes(geometry.VertexArray.Buffer, perVertexInfo);
        vao.ConfigureVertexAttributes(instanceBuffer, instanceInfo);
        geometry.IndexBuffer.Bind();
        return new BoundVertexArray(vao, instanceBuffer);
    }

    public static VertexArray CreateSeparateInstanced(Vertices geometry, VertexInfo perVertexInfo, VertexInfo instanceInfo)
    {
        const int perVertexBinding = 0;

        var vao = new VertexArray();
        vao.Bind();

        ConfigureVertexFormats(perVertexInfo, perVertexBinding);
        GL.VertexBindingDivisor(perVertexBinding, 0);

        ConfigureVertexFormats(instanceInfo, InstanceBinding);
        GL.VertexBindingDivisor(InstanceBinding, 1);

        GL.BindVertexBuffer(perVertexBinding, geometry.VertexArray.Buffer.Handle, IntPtr.Zero,
            perVertexInfo.CalculateStride());
        geometry.IndexBuffer.Bind();

        return vao;
    }

    public void Bind()
    {
        GL.BindVertexArray(Handle);
    }

    private void ConfigureVertexAttributes(Buffer buffer, VertexInfo info)
    {
        Bind();
        buffer.Bind();

        int stride = info.CalculateStride();
        int offset = 0;

        foreach (var attr in info.Attrs)
        {
            GL.EnableVertexAttribArray(attr.Location);

            var glType = GetOpenGLType(attr.Format);
            var size = attr.Format.Size();
            var normalized = attr.Format.Normalized();

            GL.VertexAttribPointer(
                attr.Location,
                size,
                glType,
                normalized,
                stride,
                offset
            );

            GL.VertexAttribDivisor(attr.Location, info.StepMode == VertexStepMode.Instance ? 1 : 0);

            offset += attr.Format.Bytes();
        }
    }

    private static VertexAttribPointerType GetOpenGLType(VertexFormat format)
    {
        return format switch
        {
            VertexFormat.Float32 or VertexFormat.Float32x2 or VertexFormat.Float32x3 or VertexFormat.Float32x4
                => VertexAttribPointerType.Float,
            VertexFormat.UInt8 or VertexFormat.UInt8x2 or VertexFormat.UInt8x3 or VertexFormat.UInt8x4 or
                VertexFormat.UInt8Norm or VertexFormat.UInt8x2Norm or VertexFormat.UInt8x3Norm
                or VertexFormat.UInt8x4Norm
                => VertexAttribPointerType.UnsignedByte,
            _ => VertexAttribPointerType.Float
        };
    }

    private static VertexAttribType GetAttribType(VertexFormat format)
    {
        return format switch
        {
            VertexFormat.Float32 or VertexFormat.Float32x2 or VertexFormat.Float32x3 or VertexFormat.Float32x4
                => VertexAttribType.Float,
            VertexFormat.UInt8 or VertexFormat.UInt8x2 or VertexFormat.UInt8x3 or VertexFormat.UInt8x4 or
                VertexFormat.UInt8Norm or VertexFormat.UInt8x2Norm or VertexFormat.UInt8x3Norm
                or VertexFormat.UInt8x4Norm
                => VertexAttribType.UnsignedByte,
            _ => VertexAttribType.Float
        };
    }

    private static void ConfigureVertexFormats(VertexInfo info, int bindingPoint)
    {
        int offset = 0;
        foreach (var attr in info.Attrs)
        {
            GL.EnableVertexAttribArray(attr.Location);
            GL.VertexAttribFormat(attr.Location, attr.Format.Size(), GetAttribType(attr.Format),
                attr.Format.Normalized(), offset);
            GL.VertexAttribBinding(attr.Location, bindingPoint);
            offset += attr.Format.Bytes();
        }
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(Handle);
    }

    public void BindInstanceBuffer(Buffer buffer, int instanceStride)
    {
        GL.BindVertexBuffer(InstanceBinding, buffer.Handle, IntPtr.Zero, instanceStride);
    }
}
