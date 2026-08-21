using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.OpenGL;
using Avalonia.Rendering.Composition;
using Avalonia.Skia;
using Elmanager.Rendering.OpenGL;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace Elmanager.SLE.Platform.OpenGL;

internal class GlVisualHandler(Func<PixelSize, float, bool> renderAction) : CompositionCustomVisualHandler
{
    private GlResources? _glResources;
    private RunOnRenderThreadMessage? _pendingRenderAction;
    private bool _reRender;

    public override void OnRender(ImmediateDrawingContext drawingContext)
    {
        var bounds = GetRenderBounds();

        if (!drawingContext.TryGetFeature<ISkiaSharpApiLeaseFeature>(out var skiaFeature))
        {
            return;
        }

        using var skiaLease = skiaFeature.Lease();
        var grContext = skiaLease.GrContext;

        var scaling = skiaLease.SkCanvas.TotalMatrix.ScaleX;
        var renderScaling = scaling > 0 ? scaling : 1.0f;
        var size = PixelSize.FromSize(bounds.Size, renderScaling);

        if (size.Width < 1 || size.Height < 1)
        {
            return;
        }

        if (grContext == null)
        {
            return;
        }

        OpenGlFbo fbo;

        using (var platformApiLease = skiaLease.TryLeasePlatformGraphicsApi())
        {
            if (platformApiLease?.Context is not IGlContext glContext)
            {
                return;
            }

            if (_glResources?.Gl != glContext)
            {
                _glResources?.SilkGl.Dispose();
                _glResources?.Fbo.Dispose();
                var silkGl = GL.GetApi(procName => glContext.GlInterface.GetProcAddress(procName));
                var isGles = glContext.Version.Type == GlProfileType.OpenGLES;
                GlProvider.Initialize(silkGl, isGles);
                _glResources = new GlResources(new OpenGlFbo(glContext, grContext), glContext, silkGl);
            }

            var gl = _glResources.SilkGl;

            gl.GetInteger(GLEnum.FramebufferBinding, out var oldFb);

            fbo = _glResources.Fbo;
            if (fbo.Size != size)
            {
                fbo.Resize(size);
            }

            gl.BindFramebuffer(GLEnum.Framebuffer, (uint)fbo.Fbo);
            gl.Disable(GLEnum.ScissorTest);
            gl.DepthMask(true);
            gl.BindSampler(0, 0);
            var requestNextFrame = renderAction(size, renderScaling);
            RunPendingRenderAction();
            if (requestNextFrame)
            {
                _reRender = true;
                RegisterForNextAnimationFrameUpdate();
            }

            gl.Flush();
            gl.BindFramebuffer(GLEnum.Framebuffer, (uint)oldFb);
        }

        using var snapshot = fbo.Snapshot();
        skiaLease.SkCanvas.DrawImage(snapshot, new SKRect(0, 0, (float)bounds.Width, (float)bounds.Height));
    }

    public override void OnMessage(object message)
    {
        switch (message)
        {
            case DisposeMessage { Disposable: var disposable }:
                {
                    if (_glResources != null)
                    {
                        using (_glResources.Gl.MakeCurrent())
                        {
                            disposable.Dispose();
                            _glResources.Fbo.Dispose();
                            _glResources.SilkGl.Dispose();
                        }

                        _glResources = null;
                    }

                    break;
                }
            case RenderRequestMessage:
                _reRender = true;
                RegisterForNextAnimationFrameUpdate();
                break;
            case RunOnRenderThreadMessage action:
                _pendingRenderAction = action;
                _reRender = true;
                RegisterForNextAnimationFrameUpdate();
                break;
        }

        base.OnMessage(message);
    }

    public override void OnAnimationFrameUpdate()
    {
        if (_reRender)
        {
            _reRender = false;
            Invalidate();
        }

        base.OnAnimationFrameUpdate();
    }

    private void RunPendingRenderAction()
    {
        var message = _pendingRenderAction;
        if (message == null)
        {
            return;
        }

        _pendingRenderAction = null;
        try
        {
            message.RenderAction();
            message.Completion.TrySetResult();
        }
        catch (Exception ex)
        {
            message.Completion.TrySetException(ex);
        }
    }

    private record GlResources(OpenGlFbo Fbo, IGlContext Gl, GL SilkGl);

    public record RenderRequestMessage;

    public record RunOnRenderThreadMessage(Action RenderAction, TaskCompletionSource Completion);

    public record DisposeMessage(IDisposable Disposable);
}
