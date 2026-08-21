using System;
using Avalonia;
using Avalonia.OpenGL;
using SkiaSharp;
using static Avalonia.OpenGL.GlConsts;

namespace Elmanager.SLE.Platform.OpenGL;

internal class OpenGlFbo : IDisposable
{
    private readonly GRContext _grContext;
    private GRBackendTexture? _backendTexture;
    private int _depthStencilBuffer;
    private int _texture;

    public OpenGlFbo(IGlContext context, GRContext grContext)
    {
        _grContext = grContext;
        Context = context;
        Fbo = Gl.GenFramebuffer();
    }

    private GlInterface Gl => Context.GlInterface;
    private IGlContext Context { get; }
    public PixelSize Size { get; private set; }

    private bool IsValid { get; set; }

    public int Fbo { get; private set; }

    public void Dispose()
    {
        _backendTexture?.Dispose();
        _backendTexture = null;

        if (Fbo != 0)
        {
            Gl.DeleteFramebuffer(Fbo);
        }

        Fbo = 0;

        if (_depthStencilBuffer != 0)
        {
            Gl.DeleteRenderbuffer(_depthStencilBuffer);
        }

        _depthStencilBuffer = 0;

        if (_texture != 0)
        {
            Gl.DeleteTexture(_texture);
        }

        _texture = 0;
    }

    public void Resize(PixelSize size)
    {
        if (Size == size)
        {
            return;
        }

        _backendTexture?.Dispose();
        _backendTexture = null;

        Gl.BindFramebuffer(GL_FRAMEBUFFER, Fbo);

        if (_texture == 0)
        {
            _texture = Gl.GenTexture();
        }

        var textureFormat = Context.Version.Type == GlProfileType.OpenGLES && Context.Version.Major == 2
            ? GL_RGBA
            : GL_RGBA8;

        Gl.BindTexture(GL_TEXTURE_2D, _texture);
        Gl.TexImage2D(GL_TEXTURE_2D, 0, textureFormat, size.Width, size.Height, 0, GL_RGBA,
            GL_UNSIGNED_BYTE, IntPtr.Zero);
        Gl.FramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, _texture, 0);

        if (_depthStencilBuffer == 0)
        {
            _depthStencilBuffer = Gl.GenRenderbuffer();
        }

        Gl.BindRenderbuffer(GL_RENDERBUFFER, _depthStencilBuffer);
        Gl.RenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, size.Width, size.Height);
        Gl.FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, _depthStencilBuffer);
        Gl.FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _depthStencilBuffer);

        var status = Gl.CheckFramebufferStatus(GL_FRAMEBUFFER);
        IsValid = status == GL_FRAMEBUFFER_COMPLETE;
        if (!IsValid)
        {
            var code = Gl.GetError();
            Console.WriteLine("Unable to configure OpenGL FBO: " + code);
        }

        _backendTexture = CreateBackendTexture(size);
        Size = size;
    }

    public SKImage Snapshot() =>
        SKImage.FromTexture(_grContext, _backendTexture, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);

    private GRBackendTexture CreateBackendTexture(PixelSize size)
    {
        var textureInfo = new GRGlTextureInfo(GL_TEXTURE_2D, (uint)_texture, SKColorType.Rgba8888.ToGlSizedFormat());
        return new GRBackendTexture(size.Width, size.Height, false, textureInfo);
    }
}
