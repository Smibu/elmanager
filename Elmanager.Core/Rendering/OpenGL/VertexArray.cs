using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class VertexArray : IDisposable
{
    private static GL GL => GlProvider.GL;
    private uint Handle { get; } = GlProvider.GL.GenVertexArray();

    private VertexArray()
    {
    }

    public static BoundVertexArray Create(VertexInfo info, float[]? data)
    {
        var buffer = new Buffer(BufferTargetARB.ArrayBuffer);
        var vao = new VertexArray();
        vao.ConfigureVertexAttributes(buffer, info);
        var boundVao = new BoundVertexArray(vao, buffer);
        if (data != null)
            boundVao.SetData(data, BufferUsageARB.StaticDraw);
        return boundVao;
    }

    public static BoundVertexArray CreateInstanced(Vertices geometry, VertexInfo perVertexInfo, VertexInfo instanceInfo)
    {
        var instanceBuffer = new Buffer(BufferTargetARB.ArrayBuffer);
        var vao = new VertexArray();
        vao.ConfigureVertexAttributes(geometry.VertexArray.Buffer, perVertexInfo);
        vao.ConfigureVertexAttributes(instanceBuffer, instanceInfo);
        geometry.IndexBuffer.Bind();
        return new BoundVertexArray(vao, instanceBuffer);
    }

    public static UnboundVertexArray CreateSeparateInstanced(Vertices geometry, VertexInfo perVertexInfo, VertexInfo instanceInfo)
    {
        var vao = new VertexArray();
        vao.ConfigureVertexAttributes(geometry.VertexArray.Buffer, perVertexInfo);

        vao.Bind();
        foreach (var attr in instanceInfo.Attrs)
        {
            GL.EnableVertexAttribArray((uint)attr.Location);
            GL.VertexAttribDivisor((uint)attr.Location, 1);
        }

        geometry.IndexBuffer.Bind();

        return new UnboundVertexArray(vao, instanceInfo);
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
            GL.EnableVertexAttribArray((uint)attr.Location);

            var glType = GetOpenGLType(attr.Format);
            var size = attr.Format.Size();
            var normalized = attr.Format.Normalized();

            unsafe
            {
                GL.VertexAttribPointer(
                    (uint)attr.Location,
                    size,
                    glType,
                    normalized,
                    (uint)stride,
                    (void*)offset
                );
            }

            GL.VertexAttribDivisor((uint)attr.Location, info.StepMode == VertexStepMode.Instance ? 1u : 0u);

            offset += attr.Format.Bytes();
        }
    }

    internal static VertexAttribPointerType GetOpenGLType(VertexFormat format)
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

    public void Dispose()
    {
        GL.DeleteVertexArray(Handle);
    }
}
