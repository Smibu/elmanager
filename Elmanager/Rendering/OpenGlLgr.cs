using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;
using Elmanager.Rec;
using Elmanager.Rendering.Camera;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager.Rendering;

internal class OpenGlLgr : IDisposable
{
    private const double BikePicXFacingLeft = 0.62;
    private const double BikePicXFacingRight = 0.43;
    private const double BikePicYFacingLeft = -0.5;
    private const double BikePicYFacingRight = 0.24;
    private const double ObjectDiameter = ObjectRadius * 2;
    public const double ObjectRadius = 0.4;
    private const int BikeDistance = 500;
    private const double BikePicAspectRatio = 380.0 / 301.0;
    private const double BikePicRotationConst = 35.0;
    private const double BikePicSize = 1.29;
    private const double BodyHeight = 0.4;
    private const double BodyRotation = 45;
    private const double BodyWidth = 0.7;
    private const double Suspension1Factor = 1 / 170.0;
    private const double Suspension2Factor = 1 / 220.0;
    private const int GrassIgnoreAlpha = 0x7F;
    private const double DefaultDepth = 0.5 * (ElmaRenderer.ZFar - ElmaRenderer.ZNear) + ElmaRenderer.ZNear;

    private readonly Dictionary<int, int> _applePics = new();
    private readonly int _armPic;
    private readonly int _bikePic;
    private readonly double _bikePicTranslateXFacingLeft = BikePicXFacingLeft * Math.Cos(BikePicRotationConst * MathUtils.DegToRad) +
                                                           BikePicYFacingLeft * Math.Sin(BikePicRotationConst * MathUtils.DegToRad);
    private readonly double _bikePicTranslateXFacingRight = BikePicXFacingRight * Math.Cos(-BikePicRotationConst * MathUtils.DegToRad) +
                                                            BikePicYFacingRight * Math.Sin(-BikePicRotationConst * MathUtils.DegToRad);
    private readonly double _bikePicTranslateYFacingLeft = BikePicXFacingLeft * Math.Sin(BikePicRotationConst * MathUtils.DegToRad) +
                                                           BikePicYFacingLeft * Math.Cos(BikePicRotationConst * MathUtils.DegToRad);
    private readonly double _bikePicTranslateYFacingRight = BikePicXFacingRight * Math.Sin(-BikePicRotationConst * MathUtils.DegToRad) +
                                                            BikePicYFacingRight * Math.Cos(-BikePicRotationConst * MathUtils.DegToRad);
    private readonly int _bodyPic;
    private readonly int _flowerPic;
    private readonly int _handPic;
    private readonly int _headPic;
    private readonly int _killerPic;
    private readonly int _legPic;
    private readonly Suspension[] _suspensions = new Suspension[2];
    private readonly int _thighPic;
    private readonly int _wheelPic;
    private DrawableGrass? _grassData;
    private readonly List<GrassImage> _grassImages = new();
    private readonly LgrImage? _qgrassImage;
    public readonly Lgr.Lgr CurrentLgr;
    private bool _disposed;
    public Dictionary<string, DrawableImage> DrawableImages { get; } = new();

    public OpenGlLgr(Level lev, Lgr.Lgr lgr, RenderingSettings newSettings)
    {
        CurrentLgr = lgr;
        var firstFrameRect = new Rectangle(0, 0, 40, 40);
        foreach (var x in CurrentLgr.LgrImages.Values)
        {
            var isSpecial = true;
            switch (x.Name)
            {
                case "q1wheel":
                    _wheelPic = LoadTexture(x);
                    break;
                case "q1head":
                    _headPic = LoadTexture(x);
                    break;
                case "q1bike":
                    _bikePic = LoadTexture(x);
                    break;
                case "q1body":
                    _bodyPic = LoadTexture(x);
                    break;
                case "q1thigh":
                    _thighPic = LoadTexture(x);
                    break;
                case "q1leg":
                    _legPic = LoadTexture(x);
                    break;
                case "q1forarm":
                    _handPic = LoadTexture(x);
                    break;
                case "q1up_arm":
                    _armPic = LoadTexture(x);
                    break;
                case "qexit":
                    _flowerPic = LoadTexture(x, firstFrameRect);
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
                    _applePics[animNum] = LoadTexture(x, firstFrameRect);
                    break;
                case "qkiller":
                    _killerPic = LoadTexture(x, firstFrameRect);
                    break;
                case "q1susp1":
                    _suspensions[0] = new Suspension(LoadTexture(x), -0.5, 0.35,
                        x.Bmp.Height * Suspension1Factor,
                        x.Bmp.Height * Suspension1Factor / 2.0, 0);
                    break;
                case "q1susp2":
                    _suspensions[1] = new Suspension(LoadTexture(x), 0.0, -0.4,
                        x.Bmp.Height * Suspension2Factor,
                        x.Bmp.Height * Suspension2Factor / 1.3, 1);
                    break;
                case "qgrass":
                    _qgrassImage = x;
                    isSpecial = false;
                    break;
                case { } when x.Name.StartsWith("qup_"):
                    _grassImages.Add(new GrassImage(x, x.Bmp.Height - 41));
                    break;
                case { } when x.Name.StartsWith("qdown_"):
                    _grassImages.Add(new GrassImage(x, 41 - x.Bmp.Height));
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

        RefreshGrassPics(lev, newSettings);
    }

    public (DrawableImage ground, DrawableImage sky) GetGroundAndSky(Level lev, bool useDefault)
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

    private int GetApple(int animNum) => _applePics.TryGetValue(animNum, out var apple) ? apple : _applePics[1];

    public static void BeginDrawObjects()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);
    }

    public static void EndDrawObjects()
    {
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);
    }

    public void DrawApple(Vector v, int animNum, double depth)
    {
        DrawObject(GetApple(animNum), v, depth);
    }

    public void DrawFlower(Vector v, double depth)
    {
        DrawObject(_flowerPic, v, depth);
    }

    public void DrawKiller(Vector v, double depth)
    {
        DrawObject(_killerPic, v, depth);
    }

    public void DrawAppleSingle(Vector v, int animNum = 1, double depth = DefaultDepth)
    {
        BeginDrawObjects();
        DrawApple(v, animNum, depth);
        EndDrawObjects();
    }

    public void DrawFlowerSingle(Vector v, double depth = DefaultDepth)
    {
        BeginDrawObjects();
        DrawFlower(v, depth);
        EndDrawObjects();
    }

    public void DrawKillerSingle(Vector v, double depth = DefaultDepth)
    {
        BeginDrawObjects();
        DrawKiller(v, depth);
        EndDrawObjects();
    }

    private void DrawPicture(int pic, double startx, double starty, double endx, double endy, double width,
        double dist, bool mirror, TexCoord texCoord, double offset = 0.0)
    {
        var lx = endx - startx;
        var ly = endy - starty;
        var l = Math.Sqrt(lx * lx + ly * ly);
        var x = width * ly / (2 * l);
        var y = width * lx / (2 * l);
        var offsetx = offset * lx / l;
        var offsety = offset * ly / l;
        GL.BindTexture(TextureTarget.Texture2D, pic);
        var texCoordYEnd = texCoord.Y2;
        if (mirror)
        {
            texCoordYEnd *= -1;
        }
        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(texCoord.X1, texCoord.Y1);
        GL.Vertex3(startx + x - offsetx, starty - y - offsety, dist);
        GL.TexCoord2(texCoord.X2, texCoord.Y1);
        GL.Vertex3(endx + x + offsetx, endy - y + offsety, dist);
        GL.TexCoord2(texCoord.X2, texCoordYEnd);
        GL.Vertex3(endx - x + offsetx, endy + y + offsety, dist);
        GL.TexCoord2(texCoord.X1, texCoordYEnd);
        GL.Vertex3(startx - x - offsetx, starty + y - offsety, dist);
        GL.End();
    }

    public void DrawFullScreenTexture(ElmaCamera cam, DrawableImage info, double midX, double midY, double depth, RenderingSettings settings)
    {
        GL.BindTexture(TextureTarget.Texture2D, info.TextureId);
        var zl = cam.ZoomLevel;
        var c = ElmaRenderer.TextureZoomConst;
        if (settings.ZoomTextures)
        {
            zl = 1;
            c = ElmaRenderer.TextureCoordConst;
        }

        GL.Begin(PrimitiveType.Quads);
        var ymin = -(midY - ElmaRenderer.TextureVertexConst);
        var ymax = -(midY + ElmaRenderer.TextureVertexConst);
        var pixelAlignedX = GrassSlopeInfo.RoundToPixelMiddle(midX - ElmaRenderer.TextureVertexConst, GetGrassFactor(settings.GrassZoom));
        var diffX = midX - ElmaRenderer.TextureVertexConst - pixelAlignedX;
        var pixelAlignedY = GrassSlopeInfo.RoundToPixelMiddle(ymin, GetGrassFactor(settings.GrassZoom));
        var diffY = ymin - pixelAlignedY;
        GL.TexCoord2(0, 0);
        GL.Vertex3(midX - ElmaRenderer.TextureVertexConst - diffX, ymin - diffY, depth);
        GL.TexCoord2(c / info.Width / zl, 0);
        GL.Vertex3(midX + ElmaRenderer.TextureVertexConst - diffX, ymin - diffY, depth);
        GL.TexCoord2(c / info.Width / zl, c / info.Width * info.AspectRatio / zl);
        GL.Vertex3(midX + ElmaRenderer.TextureVertexConst - diffX, ymax - diffY, depth);
        GL.TexCoord2(0, c / info.Width * info.AspectRatio / zl);
        GL.Vertex3(midX - ElmaRenderer.TextureVertexConst - diffX, ymax - diffY, depth);
        GL.End();
    }

    private static void DrawObject(int picture, double x, double y, double depth = DefaultDepth)
    {
        x -= ObjectRadius;
        y -= ObjectRadius;
        DrawPicture(picture, x, y, ObjectDiameter, ObjectDiameter, depth, new TexCoord(0, 1, 0, -1));
    }

    private static void DrawObject(int picture, Vector v, double depth = DefaultDepth) =>
        DrawObject(picture, v.X, v.Y, depth);

    public static void DrawPicture(int picture,
        double x,
        double y,
        double width,
        double height,
        double depth,
        TexCoord texCoord)
    {
        GL.BindTexture(TextureTarget.Texture2D, picture);
        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(texCoord.X1, texCoord.Y1);
        GL.Vertex3(x, y, depth);
        GL.TexCoord2(texCoord.X2, texCoord.Y1);
        GL.Vertex3(x + width, y, depth);
        GL.TexCoord2(texCoord.X2, texCoord.Y2);
        GL.Vertex3(x + width, y + height, depth);
        GL.TexCoord2(texCoord.X1, texCoord.Y2);
        GL.Vertex3(x, y + height, depth);
        GL.End();
    }

    private static int LoadTexture(LgrImage pcx, Rectangle srcRect)
    {
        var newBmp = new Bitmap(srcRect.Width, srcRect.Height, pcx.Bmp.PixelFormat);
        var gfx = Graphics.FromImage(newBmp);
        gfx.DrawImage(pcx.Bmp, srcRect with { X = 0, Y = 0 }, srcRect.X, srcRect.Y,
            srcRect.Width, srcRect.Height, GraphicsUnit.Pixel);
        gfx.Dispose();
        return LoadTexture(newBmp);
    }

    private static int LoadTexture(LgrImage pcx, TextureOptions? textureOptions = null) =>
        LoadTexture(pcx.Bmp, textureOptions);

    private static int LoadTexture(Bitmap bmp, TextureOptions? textureOptions = null)
    {
        textureOptions ??= new TextureOptions();
        var textureIdentifier = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureIdentifier);
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
        return textureIdentifier;
    }

    private static DrawableImage FromLgrImage(LgrImage x, TextureOptions opts, double pixelFactor) =>
        new(LoadTexture(x, opts), x.Bmp.Width * pixelFactor, x.Bmp.Height * pixelFactor, x.Meta);

    public void DrawLgrPlayer(PlayerState player, PlayerRenderOpts opts, SceneSettings sceneSettings)
    {
        var distance = ((sceneSettings.PicturesInBackground ? 1 : BikeDistance) -
                        BoolUtils.BoolToInteger(opts.IsActive)) /
            1000.0 * (ElmaRenderer.ZFar - ElmaRenderer.ZNear) + ElmaRenderer.ZNear;
        var isright = player.Direction == Direction.Right;
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);
        GL.Enable(EnableCap.DepthTest);

        var rotation = player.BikeRotation * MathUtils.DegToRad;
        var rotationCos = Math.Cos(rotation);
        var rotationSin = Math.Sin(rotation);

        if (opts.UseTransparency)
        {
            GL.Enable(EnableCap.Blend);
        }
        else
        {
            GL.Disable(EnableCap.Blend);
        }

        //Wheels
        DrawWheel(player.LeftWheelX, player.LeftWheelY, player.LeftWheelRotation, distance);
        DrawWheel(player.RightWheelX, player.RightWheelY, player.RightWheelRotation, distance);

        //Suspensions
        var x = BoolUtils.BoolToInteger(isright);
        for (var i = 0; i < 2; i++)
        {
            GL.PushMatrix();
            if (x == 0)
                x = -1;
            var suspension = _suspensions[i];
            var yPos = player.GlobalBodyY + suspension.Y * rotationCos - suspension.X * x * rotationSin;
            var xPos = player.GlobalBodyX - suspension.X * x * rotationCos - suspension.Y * rotationSin;
            if (x == -1)
                x = suspension.WheelNumber;
            else
                x -= i;
            double wheelXpos;
            double wheelYpos;
            if (x == 0)
            {
                wheelXpos = player.LeftWheelX;
                wheelYpos = player.LeftWheelY;
            }
            else
            {
                wheelXpos = player.RightWheelX;
                wheelYpos = player.RightWheelY;
            }

            var xDiff = xPos - wheelXpos;
            var yDiff = yPos - wheelYpos;
            var angle = Math.Atan2(yDiff, xDiff) * MathUtils.RadToDeg;
            var length = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            var texCoordXEnd = i == 1 ? -1 : 1;
            GL.Translate(wheelXpos, wheelYpos, 0);
            GL.Rotate(angle, 0, 0, 1);
            GL.BindTexture(TextureTarget.Texture2D, suspension.TextureIdentifier);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-suspension.OffsetX, -suspension.Height / 2, distance);
            GL.TexCoord2(texCoordXEnd, 0);
            GL.Vertex3(length + suspension.Height / 2, -suspension.Height / 2, distance);
            GL.TexCoord2(texCoordXEnd, -1);
            GL.Vertex3(length + suspension.Height / 2, suspension.Height / 2, distance);
            GL.TexCoord2(0, -1);
            GL.Vertex3(-suspension.OffsetX, suspension.Height / 2, distance);
            GL.End();
            GL.PopMatrix();
        }

        //Head
        GL.PushMatrix();
        GL.Translate(player.HeadX, player.HeadY, 0);
        GL.Rotate(player.BikeRotation + 180, 0, 0, 1);
        if (!isright)
            GL.Scale(-1.0, 1.0, 1.0);
        GL.Translate(-player.HeadX, -player.HeadY, 0);
        DrawPicture(_headPic, player.HeadX - ElmaConstants.HeadDiameter / 2.0,
            player.HeadY - ElmaConstants.HeadDiameter / 2.0,
            ElmaConstants.HeadDiameter, ElmaConstants.HeadDiameter, distance, TexCoord.Default);
        GL.PopMatrix();

        //Bike
        GL.PushMatrix();
        double bikePicTranslateX;
        double bikePicTranslateY;
        GL.Translate(player.GlobalBodyX, player.GlobalBodyY, 0);
        if (!isright)
        {
            bikePicTranslateX = _bikePicTranslateXFacingLeft;
            bikePicTranslateY = _bikePicTranslateYFacingLeft;
            GL.Rotate(player.BikeRotation + BikePicRotationConst + 180, 0, 0, 1);
            GL.Scale(-1.0, 1.0, 1.0);
        }
        else
        {
            GL.Rotate(player.BikeRotation + 180 - BikePicRotationConst, 0, 0, 1);
            bikePicTranslateX = _bikePicTranslateXFacingRight;
            bikePicTranslateY = _bikePicTranslateYFacingRight;
        }

        DrawPicture(_bikePic,
            -BikePicAspectRatio * BikePicSize / 2 + bikePicTranslateX,
            -BikePicSize / 2 + bikePicTranslateY, BikePicSize * BikePicAspectRatio,
            BikePicSize,
            distance, TexCoord.Default);
        GL.PopMatrix();

        //Thigh
        const double legMinimumWidth = 0.55;
        const double footsx = 0;
        const double footsy = -0.45;
        const double thighHeight = 0.3;
        var thighsx = 0.45;
        if (isright)
        {
            thighsx *= -1;
        }

        const double thighsy = -0.55;
        var footx = player.GlobalBodyX + footsx * rotationCos - footsy * rotationSin;
        var footy = player.GlobalBodyY + footsx * rotationSin + footsy * rotationCos;
        var thighstartx = player.HeadX + thighsx * rotationCos - thighsy * rotationSin;
        var thighstarty = player.HeadY + thighsx * rotationSin + thighsy * rotationCos;
        CalculateMiddle(thighstartx, thighstarty, footx, footy, legMinimumWidth, isright, out var thighendx,
            out var thighendy);
        DrawPicture(_thighPic, thighstartx, thighstarty, thighendx, thighendy, thighHeight, distance,
            isright, new TexCoord(0, -1, 0, 1), 0.05);

        //Leg
        const double legHeight = 0.4;
        DrawPicture(_legPic, footx, footy, thighendx, thighendy, legHeight, distance, isright,
            new TexCoord(0, 1, 0, -1), 0.05);

        //Body
        const double offsetx = 0.15;
        const double offsety = -0.35;
        GL.PushMatrix();
        GL.Translate(player.HeadX, player.HeadY, 0);
        if (isright)
        {
            GL.Scale(-1.0, 1.0, 1.0);
            GL.Rotate(-player.BikeRotation - BodyRotation, 0, 0, 1);
        }
        else
        {
            GL.Rotate(player.BikeRotation - BodyRotation, 0, 0, 1);
        }

        DrawPicture(_bodyPic, offsetx, offsety, BodyWidth, BodyHeight, distance, new TexCoord(0, -1, 0, 1));
        GL.PopMatrix();

        //Upper arm
        const double armMinimumWidth = 0.4;
        double handsx;
        var handsy = 0.4;
        const double upArmHeight = 0.2;
        double armsx;
        if (isright)
        {
            armsx = -0.12;
            handsx = 0.5;
        }
        else
        {
            armsx = 0.12;
            handsx = -0.5;
        }

        var armsy = -0.2;
        var armx = player.HeadX + armsx * rotationCos - armsy * rotationSin;
        var army = player.HeadY + armsx * rotationSin + armsy * rotationCos;
        var initialx = player.GlobalBodyX + handsx * rotationCos - handsy * rotationSin;
        var initialy = player.GlobalBodyY + handsx * rotationSin + handsy * rotationCos;
        var dist = Math.Sqrt((initialx - armx) * (initialx - armx) + (initialy - army) * (initialy - army));
        double armAngle;
        if (isright)
        {
            armAngle = Math.Atan2(initialy - army, initialx - armx) + player.ArmRotation * MathUtils.DegToRad;
        }
        else
        {
            armAngle = Math.Atan2(initialy - army, initialx - armx) + player.ArmRotation * MathUtils.DegToRad;
        }

        var angleCos = Math.Cos(armAngle);
        var angleSin = Math.Sin(armAngle);
        var handx = armx + dist * angleCos;
        var handy = army + dist * angleSin;
        CalculateMiddle(armx, army, handx, handy, armMinimumWidth, !isright, out var armendx, out var armendy);
        DrawPicture(_armPic, armx, army, armendx, armendy, upArmHeight, distance, !isright, new TexCoord(0, -1, 0, 1), 0.05);

        //Lower arm
        const double lowArmHeight = 0.15;
        DrawPicture(_handPic, armendx, armendy, handx, handy, lowArmHeight, distance, isright, new TexCoord(0, -1, 0, 1), 0.05);

        GL.Disable(EnableCap.Blend);
    }

    private void DrawWheel(double x, double y, double rot, double distance)
    {
        GL.PushMatrix();
        GL.Translate(x, y, 0);
        GL.Rotate(rot * 180 / Math.PI, 0, 0, 1);
        DrawPicture(_wheelPic, -ObjectRadius, -ObjectRadius, ObjectDiameter, ObjectDiameter, distance, TexCoord.Default);
        GL.PopMatrix();
    }

    private static void CalculateMiddle(double startx, double starty, double endx, double endy, double minWidth,
        bool mirror, out double midx, out double midy)
    {
        var distanceToFoot = Math.Sqrt((startx - endx) * (startx - endx) + (starty - endy) * (starty - endy));
        if (minWidth * 2 > distanceToFoot)
        {
            var d =
                Math.Sqrt(
                    -(startx * startx - 2 * startx * endx + endx * endx + starty * starty - 2 * starty * endy +
                        endy * endy - 4 * minWidth * minWidth) /
                    (startx * startx - 2 * startx * endx + endx * endx + starty * starty - 2 * starty * endy +
                     endy * endy)) / 2.0;
            if (mirror)
                d *= -1;
            midx = (startx + endx) / 2.0 + d * (endy - starty);
            midy = (starty + endy) / 2.0 + d * (startx - endx);
        }
        else
        {
            midx = startx - (startx - endx) / 2;
            midy = starty - (starty - endy) / 2;
        }
    }

    private static double GetGrassFactor(double zoom) => 48.0 * zoom;

    public void RefreshGrassPics(Level lev, RenderingSettings settings)
    {
        if (_qgrassImage == null)
        {
            return;
        }

        var oldQgrassId = _grassData?.Qgrass.TextureId;
        if (_grassData != null)
        {
            if (Math.Abs(_grassData.GrassZoom - settings.GrassZoom) < 0.00001)
            {
                return;
            }
            GL.DeleteTexture(_grassData.Qgrass.TextureId);
        }
        DeleteGrassPics();

        var qgrass = FromLgrImage(_qgrassImage, new TextureOptions(), 1 / GetGrassFactor(settings.GrassZoom));
        DrawableImages["qgrass"] = qgrass;

        // If the lev has qgrass textures, they must be updated too.
        lev.GraphicElements = lev.GraphicElements.Select(element =>
        {
            if (element is GraphicElement.Texture t && t.TextureInfo.TextureId == oldQgrassId)
            {
                return t with { TextureInfo = qgrass };
            }

            return element;
        }).ToList();

        int grassHeightExtension = (int)(settings.GrassZoom * 40 - 20); // this is how eolconf seems to compute the extension
        var imgs = _grassImages.AsParallel().Select(img => img.SetAlphaIgnore(GrassIgnoreAlpha, grassHeightExtension))
            .ToList();

        var grassPics = imgs.Select(img =>
            new GrassPic(
                FromLgrImage(img.Image, new TextureOptions { WrapMode = TextureWrapMode.Clamp },
                    1 / GetGrassFactor(settings.GrassZoom)),
                img.Image.Bmp, img.Delta, grassHeightExtension)).ToList();
        _grassData = new DrawableGrass(grassPics, qgrass, settings.GrassZoom);
        foreach (var polygon in lev.Polygons.Where(p => p.IsGrass))
        {
            polygon.SlopeInfo = new GrassSlopeInfo(polygon, lev.GroundBounds, settings.GrassZoom);
        }
    }

    public void DrawGrass(Level lev, ElmaCamera cam, double midX, double midY, RenderingSettings settings,
        SceneSettings sceneSettings)
    {
        if (_grassData is { GrassPics.Count: > 0 })
        {
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            GL.StencilFunc(StencilFunction.Equal, ElmaRenderer.GroundStencil, ElmaRenderer.StencilMask);
            GL.AlphaFunc(AlphaFunction.Notequal, GrassIgnoreAlpha / 255f);
            GL.Disable(EnableCap.Blend);
            for (var i = sceneSettings.TransientElements.Polygons.Count - 1; i >= 0; i--)
            {
                DrawPolygonGrass(sceneSettings.TransientElements.Polygons[i], _grassData);
            }
            for (var i = lev.Polygons.Count - 1; i >= 0; i--)
            {
                DrawPolygonGrass(lev.Polygons[i], _grassData);
            }

            GL.AlphaFunc(AlphaFunction.Gequal, 0.9f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.OneMinusDstAlpha, BlendingFactor.DstAlpha);
            DrawFullScreenTexture(cam, _grassData.Qgrass, midX, midY, 0, settings);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
        }
    }

    private void DrawPolygonGrass(Polygon p, DrawableGrass drawableGrass)
    {
        if (p is { IsGrass: true, SlopeInfo: { } })
        {
            foreach (var pic in p.SlopeInfo.GetGrassPics(drawableGrass.GrassPics))
            {
                var depth = pic.Distance / 1000.0 * (ElmaRenderer.ZFar - ElmaRenderer.ZNear) +
                            ElmaRenderer.ZNear;
                DrawPicture(pic.PictureInfo.TextureId, pic.Position.X, pic.Position.Y, pic.Width,
                    pic.Height,
                    depth, new TexCoord(0, 1, 1, 0));
            }
        }
    }

    private void DeleteTextures()
    {
        GL.DeleteTexture(_wheelPic);
        foreach (var apple in _applePics.Values)
        {
            GL.DeleteTexture(apple);
        }

        GL.DeleteTexture(_headPic);
        GL.DeleteTexture(_killerPic);
        GL.DeleteTexture(_flowerPic);
        GL.DeleteTexture(_bikePic);
        GL.DeleteTexture(_bodyPic);
        GL.DeleteTexture(_armPic);
        GL.DeleteTexture(_handPic);
        GL.DeleteTexture(_legPic);
        GL.DeleteTexture(_thighPic);
        foreach (var x in DrawableImages.Values)
        {
            GL.DeleteTexture(x.TextureId);
        }

        foreach (var x in _suspensions)
        {
            GL.DeleteTexture(x.TextureIdentifier);
        }

        DeleteGrassPics();
    }

    private void DeleteGrassPics()
    {
        if (_grassData == null)
        {
            return;
        }
        foreach (var x in _grassData.GrassPics)
        {
            // At least on some machines, deleting grass pic textures causes incorrect grass rendering when changing LGR.
            GL.DeleteTexture(x.Image.TextureId);
            x.Bmp.Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        DeleteTextures();
        _disposed = true;
    }
}