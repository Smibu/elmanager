using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

public static class GlProvider
{
    private static GL? _gl;

    public static GL GL => _gl ?? throw new InvalidOperationException("GL context has not been initialized.");

    public static bool IsOpenGLES { get; private set; }

    public static void Initialize(GL gl, bool isOpenGLES = false)
    {
        _gl = gl;
        IsOpenGLES = isOpenGLES;
    }

    public static void CheckError()
    {
        var error = GL.GetError();
        if (error != GLEnum.NoError)
            throw new InvalidOperationException($"OpenGL error: {error}");
    }
}
