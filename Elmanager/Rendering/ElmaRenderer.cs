using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Elmanager.Application;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;
using Elmanager.Rec;
using Elmanager.Rendering.Camera;
using Elmanager.UI;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager.Rendering;

internal class ElmaRenderer : IDisposable
{
    private const double ObjectDiameter = ObjectRadius * 2;
    internal const double ObjectRadius = 0.4;
    private const int BikeDistance = 500;
    private const double BikePicAspectRatio = 380.0 / 301.0;
    private const double BikePicRotationConst = 35.0;
    private const double BikePicSize = 1.29;
    private const double BikePicXFacingLeft = 0.62;
    private const double BikePicXFacingRight = 0.43;
    private const double BikePicYFacingLeft = -0.5;
    private const double BikePicYFacingRight = 0.24;
    private const double BodyHeight = 0.4;
    private const double BodyRotation = 45;
    private const double BodyWidth = 0.7;
    private const double GroundDepth = 0;
    private const int GroundStencil = 2;
    private const double PictureFactor = 1 / 48.0;
    private const double SkyDepth = 0;
    private const int SkyStencil = 1;
    private const int StencilMask = 255;
    private const double Suspension1Factor = 1 / 170.0;
    private const double Suspension2Factor = 1 / 220.0;
    private const double TextureCoordConst = TextureVertexConst * 7.0 / 3.0;
    private const double TextureVertexConst = 1000;
    private const double TextureZoomConst = 10000.0;
    private const double ZFar = 1;
    private const double ZNear = -1;
    private const int LinePattern = 0b0101010101010101;

    internal Lgr.Lgr? CurrentLgr;
    internal Level Lev = new();

    private readonly Dictionary<int, int> _applePics = new();
    private int _armPic;
    private int _bikePic;
    private double _bikePicTranslateXFacingLeft;
    private double _bikePicTranslateXFacingRight;
    private double _bikePicTranslateYFacingLeft;
    private double _bikePicTranslateYFacingRight;
    private int _bodyPic;
    private bool _disposed;

    public Dictionary<string, DrawableImage> DrawableImages { get; set; } = new();

    private int _flowerPic;
    private IGraphicsContext _gfxContext;
    private DrawableImage? _groundTexture;
    private DrawableImage? _skyTexture;
    private int _handPic;
    private int _headPic;
    private int _killerPic;
    private bool _lgrGraphicsLoaded;
    private int _legPic;
    private bool _openGlInitialized;
    private RenderingSettings _settings = new();
    private readonly Suspension?[] _suspensions = new Suspension[2];
    private int _thighPic;
    private int _wheelPic;
    private int _frameBuffer;
    private int _colorRenderBuffer;
    private int _depthStencilRenderBuffer;
    private int _maxRenderbufferSize;

    internal ElmaRenderer(GLControl renderingTarget, RenderingSettings settings)
    {
        _bikePicTranslateXFacingLeft = BikePicXFacingLeft * Math.Cos(BikePicRotationConst * MathUtils.DegToRad) +
                                       BikePicYFacingLeft * Math.Sin(BikePicRotationConst * MathUtils.DegToRad);
        _bikePicTranslateYFacingLeft = BikePicXFacingLeft * Math.Sin(BikePicRotationConst * MathUtils.DegToRad) +
                                       BikePicYFacingLeft * Math.Cos(BikePicRotationConst * MathUtils.DegToRad);
        _bikePicTranslateXFacingRight = BikePicXFacingRight * Math.Cos(-BikePicRotationConst * MathUtils.DegToRad) +
                                        BikePicYFacingRight * Math.Sin(-BikePicRotationConst * MathUtils.DegToRad);
        _bikePicTranslateYFacingRight = BikePicXFacingRight * Math.Sin(-BikePicRotationConst * MathUtils.DegToRad) +
                                        BikePicYFacingRight * Math.Cos(-BikePicRotationConst * MathUtils.DegToRad);
        _gfxContext = renderingTarget.Context;
        InitializeOpengl(disableFrameBuffer: settings.DisableFrameBuffer);
        UpdateSettings(settings);
    }

    internal bool LgrGraphicsLoaded => _lgrGraphicsLoaded;

    public void Dispose()
    {
        Dispose(true);
    }

    internal Bitmap GetSnapShot(ZoomController zoomCtrl, SceneSettings sceneSettings)
    {
        Bitmap snapShotBmp;
        if (_maxRenderbufferSize > 0)
        {
            var width = _maxRenderbufferSize;
            var height = _maxRenderbufferSize;
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
            var oldViewPort = (int[])zoomCtrl.Cam.Viewport.Clone();
            ResetViewport(width, height);
            zoomCtrl.ZoomFill();
            DrawScene(zoomCtrl.Cam, sceneSettings);
            snapShotBmp = new Bitmap(width, height);
            var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
                bmpData.Scan0);
            snapShotBmp.UnlockBits(bmpData);
            snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            ResetViewport(oldViewPort[2], oldViewPort[3]);
        }
        else
        {
            snapShotBmp = GetSnapShotOfCurrent(zoomCtrl);
        }

        return snapShotBmp;
    }

    internal static Bitmap GetSnapShotOfCurrent(ZoomController zoomCtrl)
    {
        var width = zoomCtrl.Cam.ViewPortWidth;
        var height = zoomCtrl.Cam.ViewPortHeight;
        var snapShotBmp = new Bitmap(width, height);
        var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb);
        GL.ReadBuffer(ReadBufferMode.Front);
        GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
            bmpData.Scan0);
        snapShotBmp.UnlockBits(bmpData);
        snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        return snapShotBmp;
    }

    private int GetApple(int animNum)
    {
        return _applePics.TryGetValue(animNum, out var apple) ? apple : _applePics[1];
    }

    internal void DrawApple(Vector v, int animNum = 1)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);
        DrawObject(GetApple(animNum), v);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);
    }

    internal void DrawCircle(double x, double y, double radius, Color circleColor)
    {
        var accuracy = _settings.CircleDrawingAccuracy;
        GL.Color4(circleColor);
        GL.Begin(PrimitiveType.LineLoop);
        for (var i = 0; i <= accuracy; i++)
            GL.Vertex2(x + Math.Cos(i * 360 / (double)accuracy * Math.PI / 180) * radius,
                y + Math.Sin(i * 360 / (double)accuracy * Math.PI / 180) * radius);
        GL.End();
    }

    internal void DrawCircle(Vector v, double radius, Color circleColor)
    {
        DrawCircle(v.X, v.Y, radius, circleColor);
    }

    internal void DrawDummyPlayer(double leftWheelx, double leftWheely, SceneSettings sceneSettings, PlayerRenderOpts opts)
    {
        DrawPlayer(new PlayerState(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX,
            leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY, leftWheelx, leftWheely,
            leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely, 0, 0,
            leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely + Level.HeadDifferenceFromLeftWheelY,
            0, Direction.Left, 0), opts, sceneSettings);
    }

    internal void DrawFilledTriangles(IEnumerable<Vector[]> triangles)
    {
        const double depth = ZFar - (ZFar - ZNear) * SkyDepth;
        GL.Begin(PrimitiveType.Triangles);
        foreach (var triangle in triangles)
            foreach (var x in triangle)
                GL.Vertex3(x.X, x.Y, depth);
        GL.End();
    }

    internal void DrawFlower(Vector v)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);
        DrawObject(_flowerPic, v);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);
    }

    internal void DrawKiller(Vector v)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);
        DrawObject(_killerPic, v);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);
    }

    internal void DrawLine(Vector v1, Vector v2, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex3(v1.X, v1.Y, depth);
        GL.Vertex3(v2.X, v2.Y, depth);
        GL.End();
    }

    internal void DrawLine(double x1, double y1, double x2, double y2, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex3(x1, y1, depth);
        GL.Vertex3(x2, y2, depth);
        GL.End();
    }

    internal void DrawLineFast(double x1, double y1, double x2, double y2, double depth = 0)
    {
        GL.Vertex3(x1, y1, depth);
        GL.Vertex3(x2, y2, depth);
    }

    internal void DrawLineStrip(Polygon polygon, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.LineStrip);
        foreach (var x in polygon.Vertices)
            GL.Vertex3(x.X, x.Y, depth);
        GL.End();
    }

    internal void DrawPicture(int pic, double startx, double starty, double endx, double endy, double width,
        double dist, bool mirror, double offset = 0.0)
    {
        var lx = endx - startx;
        var ly = endy - starty;
        var l = Math.Sqrt(lx * lx + ly * ly);
        var x = width * ly / (2 * l);
        var y = width * lx / (2 * l);
        var offsetx = offset * lx / l;
        var offsety = offset * ly / l;
        GL.BindTexture(TextureTarget.Texture2D, pic);
        if (mirror)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex3(startx + x - offsetx, starty - y - offsety, dist);
            GL.TexCoord2(1, 1);
            GL.Vertex3(endx + x + offsetx, endy - y + offsety, dist);
            GL.TexCoord2(1, 0);
            GL.Vertex3(endx - x + offsetx, endy + y + offsety, dist);
            GL.TexCoord2(0, 0);
            GL.Vertex3(startx - x - offsetx, starty + y - offsety, dist);
            GL.End();
        }
        else
        {
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(startx + x - offsetx, starty - y - offsety, dist);
            GL.TexCoord2(1, 0);
            GL.Vertex3(endx + x + offsetx, endy - y + offsety, dist);
            GL.TexCoord2(1, 1);
            GL.Vertex3(endx - x + offsetx, endy + y + offsety, dist);
            GL.TexCoord2(0, 1);
            GL.Vertex3(startx - x - offsetx, starty + y - offsety, dist);
            GL.End();
        }
    }

    internal void DrawPoint(Vector v, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Points);
        GL.Vertex3(v.X, v.Y, depth);
        GL.End();
    }

    internal void DrawPoint(double x, double y, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Points);
        GL.Vertex3(x, y, depth);
        GL.End();
    }

    internal void DrawPolygon(Polygon polygon, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.LineLoop);
        foreach (var x in polygon.Vertices)
            GL.Vertex3(x.X, x.Y, depth);
        GL.End();
    }

    internal void DrawRectangle(double x1, double y1, double x2, double y2, Color rectColor)
    {
        GL.Color4(rectColor);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x1, y1);
        GL.Vertex2(x2, y1);
        GL.Vertex2(x2, y2);
        GL.Vertex2(x1, y2);
        GL.End();
    }

    internal void DrawRectangle(double x1, double y1, double x2, double y2)
    {
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x1, y1);
        GL.Vertex2(x2, y1);
        GL.Vertex2(x2, y2);
        GL.Vertex2(x1, y2);
        GL.End();
    }

    internal void DrawRectangle(Vector v1, Vector v2, Color rectColor)
    {
        GL.Color4(rectColor);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(v1.X, v1.Y);
        GL.Vertex2(v2.X, v1.Y);
        GL.Vertex2(v2.X, v2.Y);
        GL.Vertex2(v1.X, v2.Y);
        GL.End();
    }

    internal void MakeCurrent()
    {
        _gfxContext.MakeCurrent();
    }

    internal void MakeNoneCurrent()
    {
        _gfxContext.MakeNoneCurrent();
    }

    internal void DrawScene(ElmaCamera cam, SceneSettings sceneSettings)
    {
        MakeCurrent();
        var aspectRatio = cam.AspectRatio;
        var centerX = cam.CenterX;
        var centerY = cam.CenterY;
        GL.LoadIdentity();
        GL.Ortho(cam.XMin, cam.XMax, cam.YMin, cam.YMax, ZNear, ZFar);

        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit |
                 ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.StencilTest);
        GL.Disable(EnableCap.Texture2D);
        GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Decr);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.ColorMask(false, false, false, false);
        GL.Begin(PrimitiveType.Triangles);
        foreach (var k in Lev.Polygons.Concat(sceneSettings.AdditionalPolys))
            if (!k.IsGrass)
                DrawFilledTrianglesFast(k.Decomposition, ZFar - (ZFar - ZNear) * SkyDepth);
        GL.End();
        GL.ColorMask(true, true, true, true);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.DepthTest);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        var midX = (Lev.XMin + Lev.XMax) / 2;
        var midY = (Lev.YMin + Lev.YMax) / 2;
        if (_settings.ShowGround)
        {
            const double depth = ZFar - (ZFar - ZNear) * GroundDepth;
            if (_settings.GroundTextureEnabled && LgrGraphicsLoaded)
            {
                GL.BindTexture(TextureTarget.Texture2D, _groundTexture!.TextureId);
                var gtW = _groundTexture.Width;
                var gtH = _groundTexture.Height;
                var zl = cam.ZoomLevel;
                var c = TextureZoomConst;
                if (_settings.ZoomTextures)
                {
                    zl = 1;
                    c = TextureCoordConst;
                }
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, c / gtW * gtW / gtH / zl);
                GL.Vertex3(midX - TextureVertexConst, midY - TextureVertexConst, depth);
                GL.TexCoord2(c / gtW / zl, c / gtW * gtW / gtH / zl);
                GL.Vertex3(midX + TextureVertexConst, midY - TextureVertexConst, depth);
                GL.TexCoord2(c / gtW / zl, 0);
                GL.Vertex3(midX + TextureVertexConst, midY + TextureVertexConst, depth);
                GL.TexCoord2(0, 0);
                GL.Vertex3(midX - TextureVertexConst, midY + TextureVertexConst, depth);
                GL.End();
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(_settings.GroundFillColor);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(midX - TextureVertexConst, midY - TextureVertexConst, depth);
                GL.Vertex3(midX + TextureVertexConst, midY - TextureVertexConst, depth);
                GL.Vertex3(midX + TextureVertexConst, midY + TextureVertexConst, depth);
                GL.Vertex3(midX - TextureVertexConst, midY + TextureVertexConst, depth);
                GL.End();
                GL.Enable(EnableCap.Texture2D);
            }
        }

        GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
        if (_settings.SkyTextureEnabled && LgrGraphicsLoaded)
        {
            const double depth = ZFar - (ZFar - ZNear) * SkyDepth;
            GL.BindTexture(TextureTarget.Texture2D, _skyTexture!.TextureId);
            if (_settings.ZoomTextures)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, TextureCoordConst / _skyTexture.Width * _skyTexture.Width / _skyTexture.Height);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / _skyTexture.Width,
                    TextureCoordConst / _skyTexture.Width * _skyTexture.Width / _skyTexture.Height);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / _skyTexture.Width, 0);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.TexCoord2(0, 0);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.End();
            }
            else
            {
                var xdelta = centerX / _skyTexture.Width;
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Ortho(0, 1, 0, 1, ZNear, ZFar);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(xdelta, 0);
                GL.Vertex3(0, 1, depth);
                GL.TexCoord2(2 + xdelta, 0);
                GL.Vertex3(1, 1, depth);
                GL.TexCoord2(2 + xdelta,
                    2 / aspectRatio);
                GL.Vertex3(1, 0, depth);
                GL.TexCoord2(xdelta,
                    2 / aspectRatio);
                GL.Vertex3(0, 0, depth);
                GL.End();
                GL.PopMatrix();
            }
        }

        GL.Enable(EnableCap.AlphaTest);
        if (LgrGraphicsLoaded)
        {
            GL.DepthFunc(DepthFunction.Gequal);
            GL.Enable(EnableCap.ScissorTest);
            for (var i = Lev.GraphicElements.Count - 1; i >= 0; --i)
            {
                var picture = Lev.GraphicElements[i];
                DoPictureScissor(picture, cam);
                if (picture is GraphicElement.Picture p && _settings.ShowPictures)
                {
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                    switch (picture.Clipping)
                    {
                        case ClippingType.Unclipped:
                            GL.StencilFunc(StencilFunction.Always, 0, StencilMask);
                            break;
                        case ClippingType.Sky:
                            GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                            break;
                        case ClippingType.Ground:
                            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                            break;
                    }

                    DrawPicture(p.PictureInfo.TextureId, picture.Position.X, picture.Position.Y, picture.Width, -picture.Height,
                        (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear);
                }
            }

            foreach (var picture in Lev.GraphicElements)
            {
                DoPictureScissor(picture, cam);
                if (picture is GraphicElement.Texture t && _settings.ShowTextures)
                {
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Invert);
                    switch (picture.Clipping)
                    {
                        case ClippingType.Ground:
                            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                            break;
                        case ClippingType.Sky:
                            GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                            break;
                        case ClippingType.Unclipped:
                            GL.StencilFunc(StencilFunction.Gequal, 5, StencilMask);
                            break;
                    }

                    var depth = (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear;
                    DrawPicture(t.MaskInfo.TextureId, picture.Position.X, picture.Position.Y, picture.Width, -picture.Height,
                        depth + 0.001);

                    GL.BindTexture(TextureTarget.Texture2D, t.TextureInfo.TextureId);
                    GL.StencilFunc(StencilFunction.Lequal, 5, StencilMask);
                    var zl = cam.ZoomLevel;
                    var c = TextureZoomConst;
                    if (_settings.ZoomTextures)
                    {
                        zl = 1;
                        c = TextureCoordConst;
                    }
                    GL.Begin(PrimitiveType.Quads);
                    var ymin = -(midY - TextureVertexConst);
                    var ymax = -(midY + TextureVertexConst);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(midX - TextureVertexConst, ymin, depth);
                    GL.TexCoord2(c / t.TextureInfo.Width / zl, 0);
                    GL.Vertex3(midX + TextureVertexConst, ymin, depth);
                    GL.TexCoord2(c / t.TextureInfo.Width / zl, c / t.TextureInfo.Width * t.AspectRatio / zl);
                    GL.Vertex3(midX + TextureVertexConst, ymax, depth);
                    GL.TexCoord2(0, c / t.TextureInfo.Width * t.AspectRatio / zl);
                    GL.Vertex3(midX - TextureVertexConst, ymax, depth);
                    GL.End();
                }
            }

            GL.Disable(EnableCap.ScissorTest);
            GL.DepthFunc(DepthFunction.Gequal);
        }

        GL.Disable(EnableCap.StencilTest);
        if (_settings.ShowObjects && LgrGraphicsLoaded)
        {
            var depth = sceneSettings.PicturesInBackground ? 0 : 0.5 * (ZFar - ZNear) + ZNear;
            foreach (var (x, i) in GetVisibleObjects(sceneSettings))
            {
                if (sceneSettings.FadedObjectIndices.Contains(i))
                {
                    GL.Enable(EnableCap.Blend);
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                }
                switch (x.Type)
                {
                    case ObjectType.Flower:
                        DrawObject(_flowerPic, x.Position, depth);
                        break;
                    case ObjectType.Killer:
                        DrawObject(_killerPic, x.Position, depth);
                        break;
                    case ObjectType.Apple:
                        DrawObject(GetApple(x.AnimationNumber), x.Position, depth);
                        break;
                    case ObjectType.Start:
                        DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings, new PlayerRenderOpts { IsActive = true, UseTransparency = false, UseGraphics = true });
                        break;
                }
            }
        }

        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.StencilTest);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.AlphaTest);
        GL.Disable(EnableCap.Blend);
        if (_settings.ShowGrid)
            DrawGrid(cam, sceneSettings.GridOffset, _settings.GridSize);
        if (_settings.ShowMaxDimensions)
        {
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple((int)_settings.LineWidth * 2, LinePattern);
            var levCenterX = (Lev.XMin + Lev.XMax) / 2;
            var levCenterY = (Lev.YMin + Lev.YMax) / 2;
            DrawRectangle(levCenterX - Level.MaximumSize / 2,
                levCenterY - Level.MaximumSize / 2,
                levCenterX + Level.MaximumSize / 2,
                levCenterY + Level.MaximumSize / 2,
                _settings.MaxDimensionColor);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(_settings.LineWidth);
        }

        if (_settings.ShowObjectFrames)
            DrawObjectFrames(sceneSettings);
        if (_settings.ShowObjectCenters)
            DrawObjectCenters(sceneSettings);
        if (_settings.ShowGravityAppleArrows &&
            (_settings.ShowObjectFrames || (_lgrGraphicsLoaded && _settings.ShowObjects)))
        {
            foreach (var (o, i) in GetVisibleObjects(sceneSettings))
            {
                DrawGravityArrowMaybe(o);
            }
        }

        foreach (var x in Lev.Polygons)
        {
            if (x.IsGrass)
            {
                if (_settings.ShowGrassEdges)
                {
                    DrawLineStrip(x, _settings.GrassEdgeColor);
                    if (_settings.ShowInactiveGrassEdges)
                    {
                        DrawDashLine(x.Vertices.First(), x.Vertices.Last(), _settings.GrassEdgeColor);
                    }
                }
            }
            else if (_settings.ShowGroundEdges)
                DrawPolygon(x, _settings.GroundEdgeColor);
        }

        GL.Color4(_settings.TextureFrameColor);
        foreach (var z in Lev.GraphicElements)
        {
            if (z is GraphicElement.Picture)
            {
                if (!_settings.ShowPictureFrames) continue;
                GL.Color4(_settings.PictureFrameColor);
            }
            else
            {
                if (!_settings.ShowTextureFrames)
                    continue;
                GL.Color4(_settings.TextureFrameColor);
            }

            DrawRectangle(z.Position.X, z.Position.Y, z.Position.X + z.Width, z.Position.Y - z.Height);
        }

        if (_settings.ShowVertices)
        {
            var showGrassVertices = _settings.ShowGrassEdges;
            var showGroundVertices = _settings.ShowGroundEdges || (_settings.ShowGround && _lgrGraphicsLoaded);
            GL.Color3(_settings.VertexColor);
            if (_settings.UseCirclesForVertices)
            {
                GL.Begin(PrimitiveType.Points);
                foreach (var x in Lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            GL.Vertex3(z.X, z.Y, 0);
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                foreach (var x in Lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            DrawEquilateralTriangleFast(z, cam.ZoomLevel * _settings.VertexSize);
            }

            GL.End();
        }
    }

    internal void Swap()
    {
        _gfxContext.SwapBuffers();
    }

    private IEnumerable<(LevObject, int)> GetVisibleObjects(SceneSettings sceneSettings)
    {
        return Lev.Objects.Select((t, i) => (t, i)).Where(t => !sceneSettings.HiddenObjectIndices.Contains(t.i));
    }

    private void DoPictureScissor(GraphicElement graphicElement, ElmaCamera cam)
    {
        var x = (int)((graphicElement.Position.X - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth);
        var y = (int)(((graphicElement.Position.Y - graphicElement.Height) - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight);
        var w = (int)((graphicElement.Position.X + graphicElement.Width - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth) - x;
        var h = (int)((graphicElement.Position.Y - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight) - y;
        GL.Scissor(x, y, w, h);
    }

    private void DrawGravityArrowMaybe(LevObject o)
    {
        if (o.Type == ObjectType.Apple && o.AppleType != AppleType.Normal)
        {
            GL.Color3(_settings.AppleGravityArrowColor);
            double arrowRotation = o.AppleType switch
            {
                AppleType.GravityUp => 0.0,
                AppleType.GravityDown => 180.0,
                AppleType.GravityLeft => 90.0,
                AppleType.GravityRight => 270.0,
                _ => throw new ArgumentOutOfRangeException()
            };

            const double arrowThickness = 0.4;
            GL.PushMatrix();
            GL.Translate(o.Position.X, o.Position.Y, 0.0);
            GL.Scale(0.5, 0.5, 1.0);
            GL.Rotate(arrowRotation, 0.0, 0.0, 1.0);
            GL.Translate(-0.5, -0.5, 0.0);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2((1 - arrowThickness) / 2.0, 0.0);
            GL.Vertex2((1 - arrowThickness) / 2.0, 0.5);
            GL.Vertex2(0.0, 0.5);
            GL.Vertex2(0.5, 1.0);
            GL.Vertex2(1.0, 0.5);
            GL.Vertex2(1.0 - (1 - arrowThickness) / 2.0, 0.5);
            GL.Vertex2(1.0 - (1 - arrowThickness) / 2.0, 0.0);
            GL.End();
            GL.PopMatrix();
        }
    }

    internal void DrawSquare(Vector v, double side, Color color)
    {
        DrawRectangle(v.X - side, v.Y - side, v.X + side, v.Y + side, color);
    }

    internal void DrawEquilateralTriangle(Vector center, double side, Color color)
    {
        GL.Color3(color);
        GL.Begin(PrimitiveType.Triangles);
        DrawEquilateralTriangleFast(center, side);
        GL.End();
    }

    private void DrawEquilateralTriangleFast(Vector center, double side)
    {
        const double factor = 1 / (1.7320508075688772935274463415059 * 2);
        GL.Vertex3(center.X + side / 2, center.Y - side * factor, 0);
        GL.Vertex3(center.X, center.Y + side / Math.Sqrt(3), 0);
        GL.Vertex3(center.X - side / 2, center.Y - side * factor, 0);
    }

    internal DrawableImage DrawableImageFromName(LgrImage img) => DrawableImages[img.Name];

    internal void InitializeLevel(Level level)
    {
        Lev = level;
        Lev.Objects = Lev.Objects.OrderBy(o => o.Type switch
        {
            ObjectType.Flower => 3,
            ObjectType.Apple => 2,
            ObjectType.Killer => 1,
            ObjectType.Start => 4,
            _ => throw new ArgumentOutOfRangeException()
        }).ToList();
        Lev.DecomposeGroundPolygons();
        Lev.UpdateImages(DrawableImages);
        Lev.UpdateBounds();
        UpdateGroundAndSky(_settings.DefaultGroundAndSky);
    }

    internal void ResetViewport(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
    }

    internal void UpdateGroundAndSky(bool useDefault)
    {
        _settings.DefaultGroundAndSky = useDefault;
        if (!LgrGraphicsLoaded) return;
        foreach (var x in DrawableImages.Values)
        {
            if (useDefault)
            {
                if (x.Name == "ground")
                    _groundTexture = x;
                if (x.Name == "sky")
                    _skyTexture = x;
            }
            else
            {
                if (x.Name == Lev.GroundTextureName)
                    _groundTexture = x;
                if (x.Name == Lev.SkyTextureName)
                    _skyTexture = x;
            }
        }

        if (_groundTexture == null)
        {
            foreach (var x in DrawableImages.Values)
            {
                if (x.Type == ImageType.Texture && !x.Equals(_skyTexture))
                {
                    _groundTexture = x;
                    break;
                }
            }
        }

        if (_skyTexture == null)
        {
            foreach (var x in DrawableImages.Values)
            {
                if (x.Type == ImageType.Texture && !x.Equals(_groundTexture))
                {
                    _skyTexture = x;
                    break;
                }
            }
        }
    }

    internal void UpdateSettings(RenderingSettings newSettings)
    {
        if (_settings.LgrFile != newSettings.LgrFile)
        {
            CurrentLgr?.Dispose();
            if (File.Exists(newSettings.LgrFile))
            {
                LoadLgrGraphics(newSettings.LgrFile);
                Lev.UpdateImages(DrawableImages);
                UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
            }
        }
        else if (_settings.DefaultGroundAndSky != newSettings.DefaultGroundAndSky)
        {
            UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
        }

        GL.ClearColor(newSettings.SkyFillColor);
        GL.LineWidth(newSettings.LineWidth);
        GL.PointSize((float)(newSettings.VertexSize * 300));
        _settings = newSettings.Clone();
    }

    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!_disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Release unmanaged resources. If disposing is false,
            // only the following code is executed.
            if (_frameBuffer != 0)
            {
                GL.DeleteFramebuffer(_frameBuffer);
            }
            if (_colorRenderBuffer != 0)
            {
                GL.DeleteRenderbuffer(_colorRenderBuffer);
            }
            if (_depthStencilRenderBuffer != 0)
            {
                GL.DeleteRenderbuffer(_depthStencilRenderBuffer);
            }
        }

        _disposed = true;
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

    private static void DrawFilledTrianglesFast(IEnumerable<Vector[]> triangles, double depth = 0.0)
    {
        foreach (var triangle in triangles)
            foreach (var x in triangle)
                GL.Vertex3(x.X, x.Y, depth);
    }

    private static void DrawObject(int picture, double x, double y, double depth = 0.5 * (ZFar - ZNear) + ZNear)
    {
        x -= ObjectRadius;
        y -= ObjectRadius;
        DrawPicture(picture, x, y, ObjectDiameter, ObjectDiameter, depth, -1);
    }

    private static void DrawObject(int picture, Vector v, double depth = 0.5 * (ZFar - ZNear) + ZNear)
    {
        DrawObject(picture, v.X, v.Y, depth);
    }

    private static void DrawPicture(int picture, double x, double y, double width, double height, double depth, double texcoordy = 1)
    {
        GL.BindTexture(TextureTarget.Texture2D, picture);
        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(0, 0);
        GL.Vertex3(x, y, depth);
        GL.TexCoord2(1, 0);
        GL.Vertex3(x + width, y, depth);
        GL.TexCoord2(1, texcoordy);
        GL.Vertex3(x + width, y + height, depth);
        GL.TexCoord2(0, texcoordy);
        GL.Vertex3(x, y + height, depth);
        GL.End();
    }

    private static int LoadTexture(LgrImage pcx, Rectangle srcRect)
    {
        var newBmp = new Bitmap(srcRect.Width, srcRect.Height, pcx.Bmp.PixelFormat);
        var gfx = Graphics.FromImage(newBmp);
        gfx.DrawImage(pcx.Bmp, new Rectangle(0, 0, srcRect.Width, srcRect.Height), srcRect.X, srcRect.Y,
            srcRect.Width, srcRect.Height, GraphicsUnit.Pixel);
        gfx.Dispose();
        return LoadTexture(newBmp);
    }

    private static int LoadTexture(LgrImage pcx, RotateFlipType flip = RotateFlipType.RotateNoneFlipNone)
    {
        return LoadTexture(pcx.Bmp, flip);
    }

    private static int LoadTexture(Bitmap bmp, RotateFlipType flip = RotateFlipType.RotateNoneFlipNone)
    {
        var textureIdentifier = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureIdentifier);
        bmp.RotateFlip(flip);
        var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (float)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (float)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Repeat);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
        bmp.UnlockBits(bmpData);
        return textureIdentifier;
    }

    private void DeleteTextures()
    {
        if (_openGlInitialized)
        {
            GL.DeleteTexture(_wheelPic);
            foreach (var apple in _applePics.Values)
            {
                GL.DeleteTexture(apple);
            }

            _applePics.Clear();
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
                GL.DeleteTexture(x.TextureId);
            foreach (var x in _suspensions)
                if (x != null)
                    GL.DeleteTexture(x.TextureIdentifier);
        }

        _groundTexture = null;
        _skyTexture = null;
    }

    private void DrawGrid(ElmaCamera cam, Vector gridOffset, double gridSize)
    {
        var current = GetFirstGridLine(gridSize, gridOffset.X, cam.XMin);
        EnableDashLines(LinePattern);
        GL.Color3(_settings.GridColor);
        GL.Begin(PrimitiveType.Lines);
        while (!(current > cam.XMax))
        {
            DrawLineFast(current, cam.YMin, current, cam.YMax);
            current += gridSize;
        }

        current = GetFirstGridLine(gridSize, gridOffset.Y, cam.YMin);
        while (!(current > cam.YMax))
        {
            DrawLineFast(cam.XMin, current, cam.XMax, current);
            current += gridSize;
        }

        GL.End();
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(_settings.LineWidth);
    }

    private void EnableDashLines(short pattern)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)_settings.LineWidth, pattern);
    }

    private void DrawObjectCenters(SceneSettings sceneSettings)
    {
        foreach (var (x, i) in GetVisibleObjects(sceneSettings))
        {
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawPoint(x.Position, _settings.FlowerColor);
                    break;
                case ObjectType.Killer:
                    DrawPoint(x.Position, _settings.KillerColor);
                    break;
                case ObjectType.Apple:
                    DrawPoint(x.Position, _settings.AppleColor);
                    break;
                case ObjectType.Start:
                    DrawPoint(x.Position, _settings.StartColor);
                    DrawPoint(x.Position.X + Level.RightWheelDifferenceFromLeftWheelX, x.Position.Y,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    DrawPoint(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                        x.Position.Y + Level.HeadDifferenceFromLeftWheelY,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    break;
            }
        }
    }

    private void DrawObjectFrames(SceneSettings sceneSettings)
    {
        foreach (var (x, i) in GetVisibleObjects(sceneSettings))
        {
            if (sceneSettings.FadedObjectIndices.Contains(i))
            {
                EnableDashLines(0b0110011001100110);
            }
            else
            {
                GL.Disable(EnableCap.LineStipple);
            }
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawCircle(x.Position, ObjectRadius, _settings.FlowerColor);
                    break;
                case ObjectType.Killer:
                    DrawCircle(x.Position, ObjectRadius, _settings.KillerColor);
                    break;
                case ObjectType.Apple:
                    DrawCircle(x.Position, ObjectRadius, _settings.AppleColor);
                    break;
                case ObjectType.Start:
                    DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings, new PlayerRenderOpts(_settings.StartColor, true, false, false));
                    break;
            }
        }
        GL.Disable(EnableCap.LineStipple);
    }

    internal void DrawPlayer(PlayerState player, PlayerRenderOpts opts, SceneSettings sceneSettings)
    {
        var distance = ((sceneSettings.PicturesInBackground ? 1 : BikeDistance) - BoolUtils.BoolToInteger(opts.IsActive)) /
            1000.0 * (ZFar - ZNear) + ZNear;
        var isright = player.Direction == Direction.Right;
        if (opts.UseGraphics && LgrGraphicsLoaded)
        {
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
                var suspension = _suspensions[i]!;
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
                GL.Translate(wheelXpos, wheelYpos, 0);
                GL.Rotate(angle, 0, 0, 1);
                GL.BindTexture(TextureTarget.Texture2D, suspension.TextureIdentifier);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0);
                GL.Vertex3(-suspension.OffsetX, -suspension.Height / 2, distance);
                GL.TexCoord2(1, 0);
                GL.Vertex3(length + suspension.Height / 2, -suspension.Height / 2, distance);
                GL.TexCoord2(1, 1);
                GL.Vertex3(length + suspension.Height / 2, suspension.Height / 2, distance);
                GL.TexCoord2(0, 1);
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
            DrawPicture(_headPic, player.HeadX - ElmaConstants.HeadDiameter / 2.0, player.HeadY - ElmaConstants.HeadDiameter / 2.0,
                ElmaConstants.HeadDiameter, ElmaConstants.HeadDiameter, distance);
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

            DrawPicture(_bikePic, -BikePicAspectRatio * BikePicSize / 2 + bikePicTranslateX,
                -BikePicSize / 2 + bikePicTranslateY, BikePicSize * BikePicAspectRatio, BikePicSize,
                distance);
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
            DrawPicture(_thighPic, thighstartx, thighstarty, thighendx, thighendy, thighHeight, distance, isright,
                0.05);

            //Leg
            const double legHeight = 0.4;
            DrawPicture(_legPic, footx, footy, thighendx, thighendy, legHeight, distance, isright, 0.05);

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

            DrawPicture(_bodyPic, offsetx, offsety, BodyWidth, BodyHeight, distance);
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
            DrawPicture(_armPic, armx, army, armendx, armendy, upArmHeight, distance, !isright, 0.05);

            //Lower arm
            const double lowArmHeight = 0.15;
            DrawPicture(_handPic, armendx, armendy, handx, handy, lowArmHeight, distance, isright, 0.05);

            GL.Disable(EnableCap.Blend);
        }
        else
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            if (!opts.IsActive)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple((int)_settings.LineWidth, LinePattern);
            }

            DrawPlayerFrames(player, opts.Color);
            if (!opts.IsActive)
            {
                GL.Disable(EnableCap.LineStipple);
            }
        }
    }

    private void DrawPlayerFrames(PlayerState player, Color playerColor)
    {
        var headCos = Math.Cos(player.BikeRotation * MathUtils.DegToRad);
        var headSin = Math.Sin(player.BikeRotation * MathUtils.DegToRad);
        var f = player.IsRight ? 1 : -1;
        var headLineEndPointX = player.HeadX + f * headCos * ElmaConstants.HeadDiameter / 2;
        var headLineEndPointY = player.HeadY + f * headSin * ElmaConstants.HeadDiameter / 2;
        DrawCircle(player.LeftWheelX, player.LeftWheelY, ObjectRadius, playerColor);
        DrawCircle(player.RightWheelX, player.RightWheelY, ObjectRadius, playerColor);
        GL.Begin(PrimitiveType.Lines);
        for (var k = 0; k < 2; k++)
        {
            for (var j = 0; j < 4; j++)
            {
                double wheelx;
                double wheely;
                double wheelrot;
                if (k == 0)
                {
                    wheelx = player.LeftWheelX;
                    wheely = player.LeftWheelY;
                    wheelrot = player.LeftWheelRotation;
                }
                else
                {
                    wheelx = player.RightWheelX;
                    wheely = player.RightWheelY;
                    wheelrot = player.RightWheelRotation;
                }

                GL.Vertex2(wheelx, wheely);
                GL.Vertex2(wheelx + ObjectRadius * Math.Cos(wheelrot + j * Math.PI / 2),
                    wheely + ObjectRadius * Math.Sin(wheelrot + j * Math.PI / 2));
            }
        }

        GL.End();
        DrawCircle(player.HeadX, player.HeadY, ElmaConstants.HeadDiameter / 2, playerColor);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(player.HeadX, player.HeadY);
        GL.Vertex2(headLineEndPointX, headLineEndPointY);
        GL.End();
    }

    private void DrawWheel(double x, double y, double rot, double distance)
    {
        GL.PushMatrix();
        GL.Translate(x, y, 0);
        GL.Rotate(rot * 180 / Math.PI, 0, 0, 1);
        DrawPicture(_wheelPic, -ObjectRadius, -ObjectRadius, ObjectDiameter, ObjectDiameter, distance);
        GL.PopMatrix();
    }

    private void InitializeOpengl(bool disableFrameBuffer)
    {
        _gfxContext.MakeCurrent();
        GL.MatrixMode(MatrixMode.Projection);
        GL.ClearDepth(GroundDepth);
        GL.StencilMask(255);
        GL.ClearStencil(GroundStencil);
        GL.DepthFunc(DepthFunction.Gequal);
        GL.AlphaFunc(AlphaFunction.Gequal, 0.9f);
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
        GL.Hint(HintTarget.TextureCompressionHint, HintMode.Fastest);
        GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
        GL.Enable(EnableCap.PointSmooth);
        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace);

        _maxRenderbufferSize = Math.Min(GL.GetInteger(GetPName.MaxRenderbufferSize), 4096);

        if (GL.GetError() != ErrorCode.NoError || disableFrameBuffer)
        {
            _maxRenderbufferSize = 0;
        }

        if (_maxRenderbufferSize > 0)
        {
            _colorRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Rgba8, _maxRenderbufferSize,
                _maxRenderbufferSize);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            _depthStencilRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthStencilRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil,
                _maxRenderbufferSize, _maxRenderbufferSize);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            _frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                RenderbufferTarget.Renderbuffer, _colorRenderBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, _depthStencilRenderBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        _openGlInitialized = true;
    }

    private void LoadLgrGraphics(string lgr)
    {
        DeleteTextures();
        DrawableImages = new Dictionary<string, DrawableImage>();
        try
        {
            CurrentLgr = new Lgr.Lgr(lgr);
        }
        catch (Exception ex)
        {
            UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + ex.Message);
            return;
        }

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
                    _bodyPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                    break;
                case "q1thigh":
                    _thighPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                    break;
                case "q1leg":
                    _legPic = LoadTexture(x, RotateFlipType.RotateNoneFlipY);
                    break;
                case "q1forarm":
                    _handPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                    break;
                case "q1up_arm":
                    _armPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
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
                    _suspensions[0] = new Suspension(LoadTexture(x, RotateFlipType.RotateNoneFlipY), -0.5, 0.35,
                        x.Bmp.Height * Suspension1Factor,
                        x.Bmp.Height * Suspension1Factor / 2.0, 0);
                    break;
                case "q1susp2":
                    _suspensions[1] = new Suspension(LoadTexture(x, RotateFlipType.Rotate180FlipY), 0.0, -0.4,
                        x.Bmp.Height * Suspension2Factor,
                        x.Bmp.Height * Suspension2Factor / 1.3, 1);
                    break;
                default:
                    isSpecial = false;
                    break;
            }

            if (!isSpecial)
            {
                DrawableImages[x.Name] = new DrawableImage(LoadTexture(x), x.Bmp.Width * PictureFactor,
                    x.Bmp.Height * PictureFactor, x.Meta);
            }
        }

        _lgrGraphicsLoaded = true;
    }

    public void DrawDashLine(Vector v1, Vector v2, Color color)
    {
        DrawDashLine(v1.X, v1.Y, v2.X, v2.Y, color);
    }

    public void DrawDashLine(double x1, double y1, double x2, double y2, Color color)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)_settings.LineWidth, LinePattern);
        DrawLine(x1, y1, x2, y2, color);
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(_settings.LineWidth);
    }

    public void SaveSnapShot(string fileName, ZoomController zoomCtrl, SceneSettings sceneSettings)
    {
        using var bmp = GetSnapShot(zoomCtrl, sceneSettings);
        bmp.Save(fileName, ImageFormat.Png);
    }

    internal static double GetFirstGridLine(double size, double offset, double min)
    {
        if (offset < 0)
        {
            offset = size + (offset % size);
        }
        var tmp = (Math.Floor(min / size) + 1) * size;
        var left = (tmp - (size + offset));
        return left;
    }
}