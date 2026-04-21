using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Elmanager.Lev;
using Elmanager.Lgr;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager.Rendering;

internal class OpenGlLgr : IDisposable
{
    public const double ObjectRadius = 0.4;

    private readonly List<GrassPic> _grassImages = new();
    private readonly Texture? _qgrassImage;
    public readonly Lgr.Lgr CurrentLgr;
    public Dictionary<string, DrawableImage> DrawableImages { get; } = new();

    public BikeTextures Bike { get; }
    public Texture Flower { get; }
    public Texture Killer { get; }
    public Dictionary<int, Texture> Apples { get; } = new();
    public DrawableGrass? GrassData { get; }

    public OpenGlLgr(Lgr.Lgr lgr)
    {
        CurrentLgr = lgr;
        var firstFrameRect = new Rectangle(0, 0, 40, 40);
        Texture? wheel = null;
        Texture? head = null;
        Texture? bike = null;
        Texture? body = null;
        Texture? thigh = null;
        Texture? leg = null;
        Texture? forarm = null;
        Texture? upArm = null;
        Texture? flower = null;
        Texture? killer = null;
        Texture? susp1 = null;
        Texture? susp2 = null;
        foreach (var x in CurrentLgr.LgrImages.Values)
        {
            var isSpecial = true;
            switch (x.Name)
            {
                case "q1wheel":
                    wheel = LoadTexture(x);
                    break;
                case "q1head":
                    head = LoadTexture(x);
                    break;
                case "q1bike":
                    bike = LoadTexture(x);
                    break;
                case "q1body":
                    body = LoadTexture(x);
                    break;
                case "q1thigh":
                    thigh = LoadTexture(x);
                    break;
                case "q1leg":
                    leg = LoadTexture(x);
                    break;
                case "q1forarm":
                    forarm = LoadTexture(x);
                    break;
                case "q1up_arm":
                    upArm = LoadTexture(x);
                    break;
                case "qexit":
                    flower = LoadTexture(x, firstFrameRect);
                    break;
                case "qfood1":
                case "qfood2":
                case "qfood3":
                case "qfood4":
                case "qfood5":
                case "qfood6":
                case "qfood7":
                case "qfood8":
                case "qfood9":
                    var animNum = int.Parse(x.Name[5].ToString());
                    Apples[animNum] = LoadTexture(x, firstFrameRect);
                    break;
                case "qkiller":
                    killer = LoadTexture(x, firstFrameRect);
                    break;
                case "q1susp1":
                    susp1 = LoadTexture(x);
                    break;
                case "q1susp2":
                    susp2 = LoadTexture(x);
                    break;
                case "qgrass":
                    _qgrassImage = LoadTexture(x);
                    isSpecial = false;
                    break;
                case { } when x.Name.StartsWith("qup_"):
                    var upImg = new GrassImage(x, x.Bmp.Height - 41);
                    _grassImages.Add(CreateGrassPic(upImg));
                    break;
                case { } when x.Name.StartsWith("qdown_"):
                    var downImg = new GrassImage(x, 41 - x.Bmp.Height);
                    _grassImages.Add(CreateGrassPic(downImg));
                    break;
                default:
                    isSpecial = false;
                    break;
            }

            if (!isSpecial)
            {
                DrawableImages[x.Name!] = FromLgrImage(x, new TextureOptions(), 1 / 48.0);
            }
        }

        if (wheel == null || head == null || bike == null || body == null || thigh == null ||
            leg == null || forarm == null || upArm == null || susp1 == null || susp2 == null)
        {
            throw new Exception("LGR is missing some bike parts");
        }

        Bike = new BikeTextures(wheel, head, bike, body, thigh, leg,
            forarm, upArm, susp1, susp2);

        Flower = flower ?? throw new Exception("LGR is missing flower");
        Killer = killer ?? throw new Exception("LGR is missing killer");

        if (_qgrassImage != null)
        {
            GrassData = new DrawableGrass(_grassImages, _qgrassImage);
        }
    }

    public (DrawableImage Ground, DrawableImage Sky) GetGroundAndSky(Level lev, bool useDefault)
    {
        DrawableImage? sky;
        DrawableImage? ground;

        if (useDefault)
        {
            ground = DrawableImageFromName("ground", ImageType.Texture);
            sky = DrawableImageFromName("sky", ImageType.Texture);
        }
        else
        {
            ground = DrawableImageFromName(lev.GroundTextureName, ImageType.Texture) ??
                     DrawableImageFromName("ground", ImageType.Texture);
            sky = DrawableImageFromName(lev.SkyTextureName, ImageType.Texture) ??
                  DrawableImageFromName("sky", ImageType.Texture);
        }

        if (ground == null)
        {
            foreach (var x in DrawableImages.Values)
            {
                if (x.Type == ImageType.Texture && !x.Equals(sky))
                {
                    ground = x;
                    break;
                }
            }
        }

        if (sky == null)
        {
            foreach (var x in DrawableImages.Values)
            {
                if (x.Type == ImageType.Texture && !x.Equals(ground))
                {
                    sky = x;
                    break;
                }
            }
        }

        return (ground!, sky!);
    }

    public DrawableImage DrawableImageFromLgrImage(LgrImage img) => DrawableImages[img.Name];

    private DrawableImage? DrawableImageFromName(string name, ImageType type)
    {
        DrawableImages.TryGetValue(name, out var val);
        if (val is not null && val.Type == type)
        {
            return val;
        }

        return null;
    }

    private static Texture LoadTexture(LgrImage pcx, Rectangle srcRect)
    {
        var newBmp = new Bitmap(srcRect.Width, srcRect.Height, pcx.Bmp.PixelFormat);
        var gfx = Graphics.FromImage(newBmp);
        gfx.DrawImage(pcx.Bmp, srcRect with { X = 0, Y = 0 }, srcRect.X, srcRect.Y,
            srcRect.Width, srcRect.Height, GraphicsUnit.Pixel);
        gfx.Dispose();
        return LoadTexture(newBmp);
    }

    private static Texture LoadTexture(LgrImage pcx, TextureOptions? textureOptions = null) =>
        LoadTexture(pcx.Bmp, textureOptions);

    private static Texture LoadTexture(Bitmap bmp, TextureOptions? textureOptions = null)
    {
        textureOptions ??= new TextureOptions();
        var texture = new Texture();
        texture.Bind();
        var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)textureOptions.WrapMode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)textureOptions.WrapMode);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
        bmp.UnlockBits(bmpData);
        return texture;
    }

    private static DrawableImage FromLgrImage(LgrImage x, TextureOptions opts, double pixelFactor) =>
        new(LoadTexture(x, opts), x.Bmp.Width * pixelFactor, x.Bmp.Height * pixelFactor, x.Meta);

    private static GrassPic CreateGrassPic(GrassImage img)
    {
        return new GrassPic(
            FromLgrImage(img.Image, new TextureOptions { WrapMode = TextureWrapMode.Clamp },
                1 / 48.0),
            img.Image.Bmp, img.Delta);
    }

    public void Dispose()
    {
        Bike.Dispose();

        foreach (var apple in Apples.Values)
        {
            apple.Dispose();
        }

        foreach (var x in DrawableImages.Values)
        {
            x.Texture.Dispose();
        }

        if (GrassData == null)
        {
            return;
        }

        foreach (var gp in GrassData.GrassPics)
        {
            gp.Image.Texture.Dispose();
        }
    }
}
