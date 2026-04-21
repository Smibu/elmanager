using System;
using Elmanager.Rendering.OpenGL;
using Elmanager.Rendering.Scene;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering;

internal record Pipelines : IDisposable
{
    public Pipeline GroundSky { get; } = Scene.GroundSky.CreatePipeline();
    public Pipeline Objects { get; } = Scene.Objects.CreatePipeline();
    public Pipeline Grass { get; } = Scene.Grass.CreatePipeline();
    public Pipeline PictureUnclipped { get; } = Pictures.CreatePipeline(StencilUnclipped);
    public Pipeline PictureGround { get; } = Pictures.CreatePipeline(StencilGround);
    public Pipeline PictureSky { get; } = Pictures.CreatePipeline(StencilSky);
    public Pipeline TextureUnclipped { get; } = Textures.CreatePipeline(StencilUnclipped);
    public Pipeline TextureGround { get; } = Textures.CreatePipeline(StencilGround);
    public Pipeline TextureSky { get; } = Textures.CreatePipeline(StencilSky);
    public Pipeline ObjectFrames { get; } = Scene.ObjectFrames.CreatePipeline();
    public Pipeline GraphicElementFrames { get; } = Scene.GraphicElementFrames.CreatePipeline();
    public Pipeline GraphicElementFramesDashed { get; } = Scene.GraphicElementFrames.CreateDashedPipeline();
    public Pipeline Lines { get; } = Scene.Lines.CreatePipeline();
    public Pipeline LinesDashed { get; } = Scene.Lines.CreateDashedPipeline();
    public Pipeline Players { get; } = Scene.Players.CreatePipeline();
    public Texture White1X1Texture { get; } = CreateWhiteTexture();

    private static Texture CreateWhiteTexture()
    {
        var texture = new Texture();
        texture.Bind();
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
            new byte[] { 255, 255, 255, 255 });
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);
        return texture;
    }

    public void Dispose()
    {
        GroundSky.Dispose();
        Objects.Dispose();
        Grass.Dispose();
        PictureUnclipped.Dispose();
        PictureGround.Dispose();
        PictureSky.Dispose();
        TextureUnclipped.Dispose();
        TextureGround.Dispose();
        TextureSky.Dispose();
        ObjectFrames.Dispose();
        GraphicElementFrames.Dispose();
        GraphicElementFramesDashed.Dispose();
        Lines.Dispose();
        LinesDashed.Dispose();
        Players.Dispose();
        White1X1Texture.Dispose();
    }

    public static readonly StencilOptions StencilUnclipped = new()
    {
        Compare = StencilFunction.Always,
        Reference = 0,
        WriteMask = 0xFF,
        StencilFail = StencilOp.Keep,
        DepthFail = StencilOp.Keep,
        Pass = StencilOp.Keep,
        ReadMask = 0xFF
    };

    public static readonly StencilOptions StencilGround = new()
    {
        Compare = StencilFunction.Equal,
        Reference = 1,
        WriteMask = 0xFF,
        StencilFail = StencilOp.Keep,
        DepthFail = StencilOp.Keep,
        Pass = StencilOp.Keep,
        ReadMask = 0xFF
    };

    private static readonly StencilOptions StencilSky = new()
    {
        Compare = StencilFunction.Equal,
        Reference = 0,
        WriteMask = 0xFF,
        StencilFail = StencilOp.Keep,
        DepthFail = StencilOp.Keep,
        Pass = StencilOp.Keep,
        ReadMask = 0xFF
    };
}
