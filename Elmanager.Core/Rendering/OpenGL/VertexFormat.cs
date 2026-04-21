using System;

namespace Elmanager.Rendering.OpenGL;

internal enum VertexFormat
{
    Float32,
    Float32x2,
    Float32x3,
    Float32x4,
    UInt8,
    UInt8Norm,
    UInt8x2,
    UInt8x2Norm,
    UInt8x3,
    UInt8x3Norm,
    UInt8x4,
    UInt8x4Norm
}

internal static class VertexFormatExtensions
{
    public static int Size(this VertexFormat format)
    {
        return format switch
        {
            VertexFormat.Float32 => 1,
            VertexFormat.Float32x2 => 2,
            VertexFormat.Float32x3 => 3,
            VertexFormat.Float32x4 => 4,
            VertexFormat.UInt8 => 1,
            VertexFormat.UInt8Norm => 1,
            VertexFormat.UInt8x2 => 2,
            VertexFormat.UInt8x2Norm => 2,
            VertexFormat.UInt8x3 => 3,
            VertexFormat.UInt8x3Norm => 3,
            VertexFormat.UInt8x4 => 4,
            VertexFormat.UInt8x4Norm => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
    }

    public static int Bytes(this VertexFormat format)
    {
        return format switch
        {
            VertexFormat.UInt8 or VertexFormat.UInt8x2 or VertexFormat.UInt8x3 or VertexFormat.UInt8x4 => format.Size(),
            VertexFormat.UInt8Norm or VertexFormat.UInt8x2Norm or VertexFormat.UInt8x3Norm
                or VertexFormat.UInt8x4Norm => format.Size(),
            _ => format.Size() * 4
        };
    }

    public static bool Normalized(this VertexFormat format)
    {
        return format is VertexFormat.UInt8Norm or VertexFormat.UInt8x2Norm or VertexFormat.UInt8x3Norm
            or VertexFormat.UInt8x4Norm;
    }
}
