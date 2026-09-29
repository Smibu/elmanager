using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

public static class GlProvider
{
    private static GL? _gl;

    public static GL GL => _gl ?? throw new InvalidOperationException("GL context has not been initialized.");

    public static bool IsOpenGLES { get; private set; }

    public static bool SupportsEsShaders { get; private set; }

    public static bool SupportsMultiDrawIndirect { get; private set; }

    public static void Initialize(GL gl, bool isOpenGLES = false)
    {
        _gl = gl;
        IsOpenGLES = isOpenGLES;
        var isGl43 = !isOpenGLES && IsVersionAtLeast(gl, 4, 3);
        SupportsEsShaders = isOpenGLES || isGl43 || gl.IsExtensionPresent("GL_ARB_ES3_compatibility");
        SupportsMultiDrawIndirect =
            isGl43 || (!isOpenGLES && gl.IsExtensionPresent("GL_ARB_multi_draw_indirect"));
    }

    private static bool IsVersionAtLeast(GL gl, int major, int minor)
    {
        gl.GetInteger(GetPName.MajorVersion, out var actualMajor);
        gl.GetInteger(GetPName.MinorVersion, out var actualMinor);
        return actualMajor > major || (actualMajor == major && actualMinor >= minor);
    }

    public static void CheckError()
    {
        var error = GL.GetError();
        if (error != GLEnum.NoError)
            throw new InvalidOperationException($"OpenGL error: {error}");
    }
}
