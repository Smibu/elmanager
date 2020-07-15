using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager
{
    internal class ElmaRenderer : IDisposable
    {
        //Constants
        internal const double ObjectDiameter = ObjectRadius * 2;
        internal const double ObjectRadius = 0.4;
        internal List<int> ActivePlayerIndices = new List<int>();
        internal Action AdditionalPolys;
        internal Action AfterDrawing;
        internal Lgr CurrentLgr;
        internal Action CustomRendering;
        internal Color[] DrivingLineColors = new Color[] { };
        internal Level Lev;
        internal bool Playing;
        internal List<int> VisiblePlayerIndices = new List<int>();
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
        private const double MinimumZoom = 0.000001;
        private const double PictureFactor = 1 / 48.0;
        private const double SkyDepth = 0;
        private const int SkyStencil = 1;
        private const int StencilMask = 255;

        private const double Suspension1Factor = 1 / 170.0;
        private const double Suspension2Factor = 1 / 220.0;
        private const double TextureCoordConst = TextureVertexConst * 7.0 / 3.0;
        private const double TextureVertexConst = 1000;
        private const double TextureZoomConst = 10000.0;

        private const double ZoomFillMargin = 0.05;
        private const double ZFar = 1;
        private const double ZNear = -1;
        private const int LinePattern = 0b0101010101010101;
        private readonly int[] _viewPort = new int[4];
        private static bool _smoothZoomInProgress;
        private Color _activePlayerColor;
        private Dictionary<int, int> _applePics = new Dictionary<int, int>();
        private int _armPic;
        private double _aspectRatio;
        private int _bikePic;
        private double _bikePicTranslateXFacingLeft;
        private double _bikePicTranslateXFacingRight;
        private double _bikePicTranslateYFacingLeft;
        private double _bikePicTranslateYFacingRight;
        private int _bodyPic;
        private IWindowInfo _ctrlWindowInfo;
        private PlayerEvent<LogicalEventType>[] _currentPlayerAppleEvents;
        private bool _disposed;
        private bool _drawInActiveAsTransparent;
        private bool _drawOnlyPlayerFrames;

        public List<DrawableImage> DrawableImages { get; set; } = new List<DrawableImage>();

        private int _flowerPic;
        private bool _followDriver;
        private double _frameStep = 0.02;
        private GraphicsContext _gfxContext;
        private DrawableImage _groundTexture;
        private int _handPic;
        private int _headPic;
        private bool _hideStartObject;
        private Color _inActivePlayerColor;
        private double _initialTime;
        private int _killerPic;
        private bool _lgrGraphicsLoaded;
        private int _legPic;
        private bool _lockedCamera;
        private bool _loopPlaying;
        private double _midX;
        private double _midY;
        private bool _multiSpy;
        private List<LevObject> _notTakenApples;
        private bool _openGlInitialized;
        private bool _picturesInBackground;
        private double _playBackSpeed = 1.0;
        private Stopwatch _playTimer = new Stopwatch();
        private List<Player> _players = new List<Player>();
        private RenderingSettings _settings = new RenderingSettings();
        private bool _showDriverPath;
        private DrawableImage _skyTexture;
        private Suspension[] _suspensions = new Suspension[2];
        private int _thighPic;
        private int _wheelPic;
        private bool _wrongLevVersion;
        private double _zoomFillxMax;
        private double _zoomFillxMin;
        private double _zoomFillyMax;
        private double _zoomFillyMin;
        private ViewSettings _viewSettings;
        private double _currentTime;
        private Vector _gridOffset = new Vector(0, 0);
        private int _frameBuffer;
        private int _colorRenderBuffer;
        private int _depthStencilRenderBuffer;
        private int _maxRenderbufferSize;
        private Control _renderTarget;
        private bool _followAlsoWhenZooming;

        internal ElmaRenderer(Control renderingTarget, RenderingSettings settings)
        {
            BaseInit(renderingTarget, settings);
        }

        internal ElmaRenderer(Level level, Control renderingTarget, RenderingSettings settings)
        {
            BaseInit(renderingTarget, settings);
            InitializeLevel(level);
        }

        internal double CenterX
        {
            get => _viewSettings.CenterX;
            set
            {
                if (value < _zoomFillxMin - XSize)
                    value = _zoomFillxMin - XSize;
                if (value > _zoomFillxMax + XSize)
                    value = _zoomFillxMax + XSize;
                _viewSettings.CenterX = value;
            }
        }

        internal double CenterY
        {
            get => _viewSettings.CenterY;
            set
            {
                if (value < _zoomFillyMin - YSize)
                    value = _zoomFillyMin - YSize;
                if (value > _zoomFillyMax + YSize)
                    value = _zoomFillyMax + YSize;
                _viewSettings.CenterY = value;
            }
        }

        internal double CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                _players.ForEach(x => { x.CurrentTime = _currentTime; });
            }
        }

        internal double MaxTime { get; private set; }

        internal double XMax => CenterX + ZoomLevel * _aspectRatio;

        internal double XMin => CenterX - ZoomLevel * _aspectRatio;

        internal double XSize => XMax - XMin;

        internal double YMax => CenterY + ZoomLevel;

        internal double YMin => CenterY - ZoomLevel;

        internal double YSize => YMax - YMin;

        internal double ZoomLevel
        {
            get => _viewSettings.ZoomLevel;
            set
            {
                if (value > MaxDimension * 2)
                    value = MaxDimension * 2;
                if (value < MinimumZoom)
                    value = MinimumZoom;
                _viewSettings.ZoomLevel = value;
            }
        }

        private double MaxDimension => Math.Max(_zoomFillxMax - _zoomFillxMin, _zoomFillyMax - _zoomFillyMin);

        internal bool LgrGraphicsLoaded => _lgrGraphicsLoaded;

        public void Dispose()
        {
            Dispose(true);
        }

        internal Bitmap GetSnapShot()
        {
            Bitmap snapShotBmp;
            if (_maxRenderbufferSize > 0)
            {
                var width = _maxRenderbufferSize;
                var height = _maxRenderbufferSize;
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
                var oldViewPort = (int[]) _viewPort.Clone();
                var oldViewSettings = _viewSettings;
                ResetViewport(width, height);
                ZoomFill();
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
                _viewSettings = oldViewSettings;
                RedrawScene();
            }
            else
            {
                var width = _viewPort[2];
                var height = _viewPort[3];
                snapShotBmp = new Bitmap(width, height);
                var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
                    bmpData.Scan0);
                snapShotBmp.UnlockBits(bmpData);
                snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

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
                GL.Vertex2(x + Math.Cos(i * 360 / (double) accuracy * Math.PI / 180) * radius,
                    y + Math.Sin(i * 360 / (double) accuracy * Math.PI / 180) * radius);
            GL.End();
        }

        internal void DrawCircle(Vector v, double radius, Color circleColor)
        {
            DrawCircle(v.X, v.Y, radius, circleColor);
        }

        internal void DrawDummyPlayer(double leftWheelx, double leftWheely, bool active = true, bool useGraphics = true)
        {
            GL.Scale(1, -1, 1);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);
            DrawPlayer(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX,
                leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY, leftWheelx, leftWheely,
                leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely, 0, 0,
                leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely - Level.HeadDifferenceFromLeftWheelY,
                0, Direction.Left, 0, active, useGraphics);
            GL.Scale(1, -1, 1);
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

        internal void DrawScene(bool zoomToDriver, bool showDriverPath)
        {
            _gfxContext.MakeCurrent(_ctrlWindowInfo);
            GL.LoadIdentity();
            double fixx = 0;
            double fixy = 0;
            if (ActivePlayerIndices.Count > 0 && zoomToDriver)
            {
                CenterX = _players[ActivePlayerIndices[0]].GlobalBodyX;
                CenterY = _players[ActivePlayerIndices[0]].GlobalBodyY;
                fixx = CenterX % (2 * ZoomLevel * _aspectRatio / _renderTarget.Width);
                fixy = CenterY % (2 * ZoomLevel / _renderTarget.Height);
                CenterX -= fixx;
                CenterY -= fixy;
                GL.Ortho(XMin, XMax, YMin, YMax, ZNear, ZFar);
                if (_lockedCamera)
                {
                    GL.Translate(CenterX, CenterY, 0);
                    GL.Rotate(-_players[ActivePlayerIndices[0]].BikeRotationDegrees, 0, 0, 1);
                    GL.Translate(-CenterX, -CenterY, 0);
                }
            }
            else
            {
                GL.Ortho(XMin, XMax, YMin, YMax, ZNear, ZFar);
            }

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit |
                     ClearBufferMask.ColorBufferBit);
            GL.Scale(1.0, -1.0, 1.0);
            GL.Enable(EnableCap.StencilTest);
            GL.Disable(EnableCap.Texture2D);
            GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Decr);
            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
            GL.ColorMask(false, false, false, false);
            GL.Begin(PrimitiveType.Triangles);
            foreach (var k in Lev.Polygons)
                if (!k.IsGrass)
                    DrawFilledTrianglesFast(k.Decomposition, ZFar - (ZFar - ZNear) * SkyDepth);
            GL.End();
            AdditionalPolys?.Invoke();
            GL.ColorMask(true, true, true, true);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.Scale(1.0, -1.0, 1.0);
            if (_settings.ShowGround)
            {
                const double depth = ZFar - (ZFar - ZNear) * GroundDepth;
                if (_settings.GroundTextureEnabled && LgrGraphicsLoaded)
                {
                    GL.BindTexture(TextureTarget.Texture2D, _groundTexture.TextureIdentifier);
                    var gtW = _groundTexture.Width;
                    var gtH = _groundTexture.Height;
                    if (_settings.ZoomTextures)
                    {
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex3(_midX - TextureVertexConst, _midY - TextureVertexConst, depth);
                        GL.TexCoord2(TextureCoordConst / gtW, 0);
                        GL.Vertex3(_midX + TextureVertexConst, _midY - TextureVertexConst, depth);
                        GL.TexCoord2(TextureCoordConst / gtW, TextureCoordConst / gtW * gtW / gtH);
                        GL.Vertex3(_midX + TextureVertexConst, _midY + TextureVertexConst, depth);
                        GL.TexCoord2(0, TextureCoordConst / gtW * gtW / gtH);
                        GL.Vertex3(_midX - TextureVertexConst, _midY + TextureVertexConst, depth);
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(PrimitiveType.Quads);
                        const double texminx = 0;
                        const double texminy = 0;
                        GL.TexCoord2(texminx, texminy);
                        GL.Vertex3(_midX - TextureVertexConst, _midY - TextureVertexConst, depth);
                        GL.TexCoord2(texminx + TextureZoomConst / gtW / ZoomLevel, texminy);
                        GL.Vertex3(_midX + TextureVertexConst, _midY - TextureVertexConst, depth);
                        GL.TexCoord2(texminx + TextureZoomConst / gtW / ZoomLevel,
                            texminy + TextureZoomConst / gtW * gtW / gtH / ZoomLevel);
                        GL.Vertex3(_midX + TextureVertexConst, _midY + TextureVertexConst, depth);
                        GL.TexCoord2(texminx, texminy + TextureZoomConst / gtW * gtW / gtH / ZoomLevel);
                        GL.Vertex3(_midX - TextureVertexConst, _midY + TextureVertexConst, depth);
                        GL.End();
                    }
                }
                else
                {
                    GL.Disable(EnableCap.Texture2D);
                    GL.Color4(_settings.GroundFillColor);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(_midX - TextureVertexConst, _midY - TextureVertexConst, depth);
                    GL.Vertex3(_midX + TextureVertexConst, _midY - TextureVertexConst, depth);
                    GL.Vertex3(_midX + TextureVertexConst, _midY + TextureVertexConst, depth);
                    GL.Vertex3(_midX - TextureVertexConst, _midY + TextureVertexConst, depth);
                    GL.End();
                    GL.Enable(EnableCap.Texture2D);
                }
            }

            GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
            if (_settings.SkyTextureEnabled && LgrGraphicsLoaded)
            {
                const double depth = ZFar - (ZFar - ZNear) * SkyDepth;
                GL.BindTexture(TextureTarget.Texture2D, _skyTexture.TextureIdentifier);
                if (_settings.ZoomTextures)
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY - TextureVertexConst, depth);
                    GL.TexCoord2(TextureCoordConst / _skyTexture.Width, 0);
                    GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY - TextureVertexConst, depth);
                    GL.TexCoord2(TextureCoordConst / _skyTexture.Width,
                        TextureCoordConst / _skyTexture.Width * _skyTexture.Width / _skyTexture.Height);
                    GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY + TextureVertexConst, depth);
                    GL.TexCoord2(0, TextureCoordConst / _skyTexture.Width * _skyTexture.Width / _skyTexture.Height);
                    GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY + TextureVertexConst, depth);
                    GL.End();
                }
                else
                {
                    var xdelta = CenterX / _skyTexture.Width;
                    GL.PushMatrix();
                    GL.LoadIdentity();
                    GL.Ortho(0, 1, 0, 1, ZNear, ZFar);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(xdelta, 0);
                    GL.Vertex3(0, 1, depth);
                    GL.TexCoord2(2 + xdelta, 0);
                    GL.Vertex3(1, 1, depth);
                    GL.TexCoord2(2 + xdelta,
                        2 / _aspectRatio);
                    GL.Vertex3(1, 0, depth);
                    GL.TexCoord2(xdelta,
                        2 / _aspectRatio);
                    GL.Vertex3(0, 0, depth);
                    GL.End();
                    GL.PopMatrix();
                }
            }

            GL.Scale(1.0, -1.0, 1.0);
            GL.Enable(EnableCap.AlphaTest);
            if (LgrGraphicsLoaded)
            {
                GL.DepthFunc(DepthFunction.Gequal);
                GL.Enable(EnableCap.ScissorTest);
                for (var i = Lev.Pictures.Count - 1; i >= 0; --i)
                {
                    var picture = Lev.Pictures[i];
                    DoPictureScissor(picture);
                    if (picture.IsPicture && _settings.ShowPictures)
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

                        DrawPicture(picture.Id, picture.Position.X, picture.Position.Y, picture.Width, picture.Height,
                            (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear);
                    }
                }

                foreach (var picture in Lev.Pictures)
                {
                    DoPictureScissor(picture);
                    if (!picture.IsPicture && _settings.ShowTextures)
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
                        DrawPicture(picture.Id, picture.Position.X, picture.Position.Y, picture.Width, picture.Height,
                            depth + 0.001);

                        GL.BindTexture(TextureTarget.Texture2D, picture.TextureId);
                        GL.StencilFunc(StencilFunction.Lequal, 5, StencilMask);
                        if (_settings.ZoomTextures)
                        {
                            GL.Begin(PrimitiveType.Quads);
                            var ymin = -(_midY - TextureVertexConst);
                            var ymax = -(_midY + TextureVertexConst);
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(_midX - TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureCoordConst / picture.TextureWidth, 0);
                            GL.Vertex3(_midX + TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureCoordConst / picture.TextureWidth,
                                TextureCoordConst / picture.TextureWidth * picture.AspectRatio);
                            GL.Vertex3(_midX + TextureVertexConst, ymax, depth);
                            GL.TexCoord2(0, TextureCoordConst / picture.TextureWidth * picture.AspectRatio);
                            GL.Vertex3(_midX - TextureVertexConst, ymax, depth);
                            GL.End();
                        }
                        else
                        {
                            GL.Begin(PrimitiveType.Quads);
                            var ymin = -(_midY - TextureVertexConst);
                            var ymax = -(_midY + TextureVertexConst);
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(_midX - TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureZoomConst / picture.TextureWidth / ZoomLevel, 0);
                            GL.Vertex3(_midX + TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureZoomConst / picture.TextureWidth / ZoomLevel,
                                TextureZoomConst / picture.TextureWidth * picture.AspectRatio / ZoomLevel);
                            GL.Vertex3(_midX + TextureVertexConst, ymax, depth);
                            GL.TexCoord2(0, TextureZoomConst / picture.TextureWidth * picture.AspectRatio / ZoomLevel);
                            GL.Vertex3(_midX - TextureVertexConst, ymax, depth);
                            GL.End();
                        }
                    }
                }

                GL.Disable(EnableCap.ScissorTest);
                GL.DepthFunc(DepthFunction.Gequal);
            }

            GL.Disable(EnableCap.StencilTest);
            GL.Scale(1.0, -1.0, 1.0);
            GL.Translate(-fixx, -fixy, 0);
            DrawPlayers(ActivePlayerIndices, VisiblePlayerIndices);
            GL.Translate(fixx, fixy, 0);
            GL.Scale(1.0, -1.0, 1.0);
            if (_settings.ShowObjects && LgrGraphicsLoaded)
            {
                var depth = _picturesInBackground ? 0 : 0.5 * (ZFar - ZNear) + ZNear;
                foreach (var x in Lev.Objects)
                {
                    switch (x.Type)
                    {
                        case ObjectType.Flower:
                            DrawObject(_flowerPic, x.Position, depth);
                            break;
                        case ObjectType.Killer:
                            DrawObject(_killerPic, x.Position, depth);
                            break;
                        case ObjectType.Apple:
                            if (_wrongLevVersion || ActivePlayerIndices.Count == 0)
                                DrawObject(GetApple(x.AnimationNumber), x.Position, depth);
                            break;
                        case ObjectType.Start:
                            if (!_hideStartObject)
                            {
                                DrawDummyPlayer(x.Position.X, -x.Position.Y, true, !_drawOnlyPlayerFrames);
                            }

                            break;
                    }
                }

                if (!_wrongLevVersion && ActivePlayerIndices.Count > 0)
                {
                    var i = 0;
                    while (!(i >= _currentPlayerAppleEvents.Length || _currentPlayerAppleEvents[i].Time >= CurrentTime))
                        i++;
                    for (var j = i; j < _currentPlayerAppleEvents.Length; j++)
                    {
                        var z = Lev.Objects[_currentPlayerAppleEvents[j].Info];
                        DrawObject(GetApple(z.AnimationNumber), z.Position);
                    }

                    foreach (var x in _notTakenApples)
                        DrawObject(GetApple(x.AnimationNumber), x.Position);
                }
            }

            DisableCaps();
            if (_settings.ShowGrid)
                DrawGrid();
            if (_settings.ShowMaxDimensions)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple((int) _settings.LineWidth * 2, LinePattern);
                var centerX = (Lev.XMin + Lev.XMax) / 2;
                var centerY = (Lev.YMin + Lev.YMax) / 2;
                DrawRectangle(centerX - Level.MaximumSize / 2,
                    centerY - Level.MaximumSize / 2,
                    centerX + Level.MaximumSize / 2,
                    centerY + Level.MaximumSize / 2,
                    _settings.MaxDimensionColor);
                GL.Disable(EnableCap.LineStipple);
                GL.LineWidth(_settings.LineWidth);
            }

            if (_settings.ShowObjectFrames)
                DrawObjectFrames();
            if (_settings.ShowObjectCenters)
                DrawObjectCenters();
            if (_settings.ShowGravityAppleArrows &&
                (_settings.ShowObjectFrames || (_lgrGraphicsLoaded && _settings.ShowObjects)))
            {
                if (_wrongLevVersion || ActivePlayerIndices.Count == 0)
                    foreach (var o in Lev.Objects)
                    {
                        DrawGravityArrowMaybe(o);
                    }
                else if (!_wrongLevVersion && ActivePlayerIndices.Count > 0)
                {
                    var i = 0;
                    while (!(i >= _currentPlayerAppleEvents.Length || _currentPlayerAppleEvents[i].Time >= CurrentTime))
                        i++;
                    for (var j = i; j < _currentPlayerAppleEvents.Length; j++)
                    {
                        var z = Lev.Objects[_currentPlayerAppleEvents[j].Info];
                        DrawGravityArrowMaybe(z);
                    }

                    foreach (var x in _notTakenApples)
                        DrawGravityArrowMaybe(x);
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
            foreach (var z in Lev.Pictures)
            {
                if (z.IsPicture)
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

                DrawRectangle(z.Position.X, z.Position.Y, z.Position.X + z.Width, z.Position.Y + z.Height);
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
                                DrawEquilateralTriangleFast(z, _viewSettings.ZoomLevel * _settings.VertexSize);
                }

                GL.End();
            }

            CustomRendering?.Invoke();
            GL.Scale(1.0, -1.0, 1.0);
            if (ActivePlayerIndices.Count > 0 && showDriverPath)
            {
                foreach (var x in ActivePlayerIndices)
                {
                    if (_players[x].FrameCount > 1)
                    {
                        GL.Color4(DrivingLineColors[x]);
                        GL.Begin(PrimitiveType.LineStrip);
                        for (var k = 0; k < _players[x].FrameCount; k++)
                            GL.Vertex2(_players[x].GlobalBodyFromIndex(k).X, _players[x].GlobalBodyFromIndex(k).Y);
                        GL.End();
                    }
                }
            }

            _gfxContext.SwapBuffers();
            AfterDrawing?.Invoke();
        }

        private void DoPictureScissor(Picture picture)
        {
            var x = (int) ((picture.Position.X - XMin) / (XMax - XMin) * _viewPort[2]);
            var y = (int) (((-picture.Position.Y - picture.Height) - YMin) / (YMax - YMin) * _viewPort[3]);
            var w = (int) ((picture.Position.X + picture.Width - XMin) / (XMax - XMin) * _viewPort[2]) - x;
            var h = (int) ((-picture.Position.Y - YMin) / (YMax - YMin) * _viewPort[3]) - y;
            GL.Scissor(x, y, w, h);
        }

        private void DrawGravityArrowMaybe(LevObject o)
        {
            if (o.Type == ObjectType.Apple && o.AppleType != AppleType.Normal)
            {
                GL.Color3(_settings.AppleGravityArrowColor);
                double arrowRotation;
                switch (o.AppleType)
                {
                    case AppleType.GravityUp:
                        arrowRotation = 180.0;
                        break;
                    case AppleType.GravityDown:
                        arrowRotation = 0.0;
                        break;
                    case AppleType.GravityLeft:
                        arrowRotation = 90.0;
                        break;
                    case AppleType.GravityRight:
                        arrowRotation = 270.0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

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

        internal void DrawSceneDefault()
        {
            DrawScene(_followDriver, _showDriverPath);
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

        internal DrawableImage DrawableImageFromName(string name)
        {
            return DrawableImages.FirstOrDefault(x => x.Name == name);
        }

        internal void FocusIndicesChanged()
        {
            if (!_wrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                _currentPlayerAppleEvents = _players[ActivePlayerIndices[0]].GetEvents(LogicalEventType.AppleTake);
                _notTakenApples = new List<LevObject>();
                for (var i = 0; i < Lev.Objects.Count; i++)
                {
                    if (Lev.Objects[i].Type != ObjectType.Apple)
                        continue;
                    var i1 = i;
                    var isTaken = _currentPlayerAppleEvents.Any(x => x.Info == i1);
                    if (!isTaken)
                        _notTakenApples.Add(Lev.Objects[i]);
                }
            }
        }

        internal Vector GetBikeCoordinates()
        {
            return ActivePlayerIndices.Count > 0
                ? new Vector(_players[ActivePlayerIndices[0]].GlobalBodyX,
                    _players[ActivePlayerIndices[0]].GlobalBodyY)
                : new Vector();
        }

        internal double GetSpeed()
        {
            return ActivePlayerIndices.Count > 0 ? _players[ActivePlayerIndices[0]].Speed : 0.0;
        }

        internal void InitializeLevel(Level level)
        {
            Lev = level;
            Lev.Objects = Lev.Objects.OrderBy(o => {
                switch (o.Type)
                {
                    case ObjectType.Flower:
                        return 3;
                    case ObjectType.Apple:
                        return 2;
                    case ObjectType.Killer:
                        return 1;
                    case ObjectType.Start:
                        return 4;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).ToList();
            Lev.DecomposeGroundPolygons();
            Lev.UpdateImages(DrawableImages);
            Lev.UpdateBounds();
            UpdateZoomFillBounds();
            UpdateGroundAndSky(_settings.DefaultGroundAndSky);
        }

        internal void InitializeReplays(List<Replay> replays)
        {
            ZoomLevel = 5.0;
            MaxTime = 0.0;
            _players = new List<Player>();
            foreach (var t in replays)
            {
                _players.Add(t.Player1);
                if (t.Player1.FrameCount > MaxTime)
                    MaxTime = t.Player1.FrameCount;
                if (t.IsMulti)
                {
                    _players.Add(t.Player2);
                    if (t.Player2.FrameCount > MaxTime)
                        MaxTime = t.Player2.FrameCount;
                }
            }

            MaxTime /= 30.0;
            CurrentTime = 0.0;
            InitializeLevel(replays[0].GetLevel());
            ActivePlayerIndices = new List<int>();
            VisiblePlayerIndices = new List<int>();

            _wrongLevVersion = replays[0].WrongLevelVersion;
        }

        internal void NextFrame(object sender = null, EventArgs e = null)
        {
            if (!Playing)
            {
                CurrentTime += _frameStep;
                if (CurrentTime > MaxTime)
                    CurrentTime = 0;
                DrawSceneDefault();
            }
        }

        internal void PreviousFrame(object sender = null, EventArgs e = null)
        {
            if (!Playing)
            {
                CurrentTime -= _frameStep;
                if (CurrentTime < 0)
                    CurrentTime = MaxTime;
                DrawSceneDefault();
            }
        }

        internal void RedrawScene(object sender = null, EventArgs e = null)
        {
            DrawSceneNoDriverFocus();
        }

        internal void ResetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            _aspectRatio = width / (double) height;
            GL.GetInteger(GetPName.Viewport, _viewPort);
        }

        internal void SetFullScreenMode(DisplayResolution newResolution)
        {
            DisplayDevice.Default.ChangeResolution(newResolution);
            GL.Viewport(0, 0, newResolution.Width, newResolution.Height);
            _aspectRatio = newResolution.Width / (double) newResolution.Height;
        }

        internal void SetPlayBackSpeed(double newSpeed)
        {
            _playBackSpeed = newSpeed;
            if (Playing)
            {
                _initialTime = CurrentTime;
                _playTimer.Restart();
            }
        }

        internal void StopPlaying(object sender = null, EventArgs e = null)
        {
            Playing = false;
            CurrentTime = 0;
            DrawSceneDefault();
        }

        internal void TogglePlay(object sender = null, EventArgs e = null)
        {
            if (Playing)
            {
                Playing = false;
                return;
            }

            if (CurrentTime > MaxTime)
            {
                CurrentTime = 0;
                DrawSceneDefault();
            }

            Playing = true;
            double elapsedTime = 0;
            _initialTime = CurrentTime;
            _playTimer.Restart();
            while (Playing)
            {
                CurrentTime = _initialTime + elapsedTime;
                if (_multiSpy && VisiblePlayerIndices.Count > 0 && (!_followDriver || ActivePlayerIndices.Count == 0))
                {
                    var xmin = _players[VisiblePlayerIndices[0]].GlobalBodyX;
                    var xmax = _players[VisiblePlayerIndices[0]].GlobalBodyX;
                    var ymin = _players[VisiblePlayerIndices[0]].GlobalBodyY;
                    var ymax = _players[VisiblePlayerIndices[0]].GlobalBodyY;
                    foreach (var i in VisiblePlayerIndices)
                    {
                        xmin = Math.Min(_players[i].GlobalBodyX, xmin);
                        xmax = Math.Max(_players[i].GlobalBodyX, xmax);
                        ymin = Math.Min(_players[i].GlobalBodyY, ymin);
                        ymax = Math.Max(_players[i].GlobalBodyY, ymax);
                    }

                    CenterX = (xmin + xmax) / 2;
                    CenterY = (ymin + ymax) / 2;
                    ZoomLevel = Math.Max((xmax + 5 - CenterX) / _aspectRatio, ymax + 5 - CenterY);
                    ZoomLevel = Math.Max(ZoomLevel, 5);
                }

                if (CurrentTime > MaxTime)
                {
                    if (_loopPlaying)
                    {
                        CurrentTime = 0;
                        _initialTime = 0;
                        _playTimer.Restart();
                    }
                    else
                    {
                        DrawSceneDefault();
                        break;
                    }
                }

                DrawSceneDefault();
                Application.DoEvents();
                elapsedTime = _playTimer.ElapsedMilliseconds / 1000.0 * _playBackSpeed;
            }

            Playing = false;
            _playTimer.Stop();
        }

        internal void UpdateGroundAndSky(bool useDefault)
        {
            _settings.DefaultGroundAndSky = useDefault;
            if (!LgrGraphicsLoaded) return;
            foreach (var x in DrawableImages)
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
                foreach (var x in DrawableImages)
                {
                    if (x.Type == Lgr.ImageType.Texture && !x.Equals(_skyTexture))
                    {
                        _groundTexture = x;
                        break;
                    }
                }
            }

            if (_skyTexture == null)
            {
                foreach (var x in DrawableImages)
                {
                    if (x.Type == Lgr.ImageType.Texture && !x.Equals(_groundTexture))
                    {
                        _skyTexture = x;
                        break;
                    }
                }
            }
        }

        internal void UpdateReplaySettings()
        {
            _showDriverPath = Global.AppSettings.ReplayViewer.ShowDriverPath;
            _followDriver = Global.AppSettings.ReplayViewer.FollowDriver;
            _followAlsoWhenZooming = Global.AppSettings.ReplayViewer.FollowAlsoWhenZooming;
            _lockedCamera = Global.AppSettings.ReplayViewer.LockedCamera;
            _picturesInBackground = Global.AppSettings.ReplayViewer.PicturesInBackGround;
            _drawInActiveAsTransparent = Global.AppSettings.ReplayViewer.DrawTransparentInactive;
            _frameStep = Global.AppSettings.ReplayViewer.FrameStep;
            _loopPlaying = Global.AppSettings.ReplayViewer.LoopPlaying;
            _activePlayerColor = Global.AppSettings.ReplayViewer.ActivePlayerColor;
            _inActivePlayerColor = Global.AppSettings.ReplayViewer.InactivePlayerColor;
            _drawOnlyPlayerFrames = Global.AppSettings.ReplayViewer.DrawOnlyPlayerFrames;
            _hideStartObject = Global.AppSettings.ReplayViewer.HideStartObject;
            _multiSpy = Global.AppSettings.ReplayViewer.MultiSpy;
        }

        internal void UpdateSettings(RenderingSettings newSettings)
        {
            if (_settings.LgrFile != newSettings.LgrFile)
            {
                CurrentLgr?.Dispose();
                if (File.Exists(newSettings.LgrFile))
                {
                    LoadLgrGraphics(newSettings.LgrFile);
                    if (Lev != null)
                    {
                        Lev.UpdateImages(DrawableImages);
                        UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
                    }
                }
            }
            else if (_settings.DefaultGroundAndSky != newSettings.DefaultGroundAndSky)
            {
                UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
            }

            GL.ClearColor(newSettings.SkyFillColor);
            GL.LineWidth(newSettings.LineWidth);
            GL.PointSize((float) (newSettings.VertexSize * 300));
            _settings = newSettings.Clone();
        }

        internal void UpdateZoomFillBounds()
        {
            _zoomFillxMin = (1 + ZoomFillMargin) * Lev.XMin - ZoomFillMargin * Lev.XMax;
            _zoomFillxMax = (1 + ZoomFillMargin) * Lev.XMax - ZoomFillMargin * Lev.XMin;
            _zoomFillyMin = (1 + ZoomFillMargin) * Lev.YMin - ZoomFillMargin * Lev.YMax;
            _zoomFillyMax = (1 + ZoomFillMargin) * Lev.YMax - ZoomFillMargin * Lev.YMin;
            var tempVar = _zoomFillyMin;
            _zoomFillyMin = -_zoomFillyMax;
            _zoomFillyMax = -tempVar;
            _midX = (_zoomFillxMax + _zoomFillxMin) / 2;
            _midY = (_zoomFillyMax + _zoomFillyMin) / 2;
        }

        internal void Zoom(Vector p, bool zoomIn, double zoomFactor)
        {
            var i = zoomIn ? zoomFactor : 1 / zoomFactor;
            var x = p.X;
            var y = p.Y;
            if (ActivePlayerIndices.Count > 0 && _followDriver && _followAlsoWhenZooming)
            {
                x = _players[ActivePlayerIndices[0]].GlobalBodyX;
                y = _players[ActivePlayerIndices[0]].GlobalBodyY;
                CenterX = x;
                CenterY = y;
            }

            x -= (x - (XMax + XMin) / 2) * i;
            y -= (y - (YMax + YMin) / 2) * i;
            PerformZoom(ZoomLevel * i, x, y);
        }

        internal void ZoomFill(object sender = null, EventArgs e = null)
        {
            var levelAspectRatio = (_zoomFillxMax - _zoomFillxMin) / (_zoomFillyMax - _zoomFillyMin);
            var newZoomLevel = (_zoomFillyMax - _zoomFillyMin) / 2;
            if (levelAspectRatio > _aspectRatio)
                newZoomLevel = (_zoomFillxMax - _zoomFillxMin) / 2 / _aspectRatio;
            PerformZoom(newZoomLevel, (_zoomFillxMax + _zoomFillxMin) / 2, (_zoomFillyMax + _zoomFillyMin) / 2);
        }

        internal void ZoomRect(Vector startPoint, Vector endPoint)
        {
            if (startPoint != endPoint)
            {
                double x1;
                double x2;
                if (startPoint.X < endPoint.X)
                {
                    x1 = startPoint.X;
                    x2 = endPoint.X;
                }
                else
                {
                    x2 = startPoint.X;
                    x1 = endPoint.X;
                }

                double y1;
                double y2;
                if (startPoint.Y < endPoint.Y)
                {
                    y1 = startPoint.Y;
                    y2 = endPoint.Y;
                }
                else
                {
                    y2 = startPoint.Y;
                    y1 = endPoint.Y;
                }

                var i = (y2 - y1) / 2;
                var rectAspectRatio = (x2 - x1) / (y2 - y1);
                if (rectAspectRatio > _aspectRatio)
                    i = (x2 - x1) / 2 / _aspectRatio;
                PerformZoom(i, (x2 + x1) / 2, (y2 + y1) / 2);
            }
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
                _gfxContext.Dispose();
                _ctrlWindowInfo.Dispose();
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

        private static void DisableCaps()
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.StencilTest);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.AlphaTest);
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
            GL.BindTexture(TextureTarget.Texture2D, picture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(x, y, depth);
            GL.TexCoord2(1, 0);
            GL.Vertex3(x + ObjectDiameter, y, depth);
            GL.TexCoord2(1, 1);
            GL.Vertex3(x + ObjectDiameter, y + ObjectDiameter, depth);
            GL.TexCoord2(0, 1);
            GL.Vertex3(x, y + ObjectDiameter, depth);
            GL.End();
        }

        private static void DrawObject(int picture, Vector v, double depth = 0.5 * (ZFar - ZNear) + ZNear)
        {
            DrawObject(picture, v.X, v.Y, depth);
        }

        private static void DrawPicture(int picture, double x, double y, double width, double height, double depth)
        {
            GL.BindTexture(TextureTarget.Texture2D, picture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(x, y, depth);
            GL.TexCoord2(1, 0);
            GL.Vertex3(x + width, y, depth);
            GL.TexCoord2(1, 1);
            GL.Vertex3(x + width, y + height, depth);
            GL.TexCoord2(0, 1);
            GL.Vertex3(x, y + height, depth);
            GL.End();
        }

        private static int LoadTexture(Lgr.LgrImage pcx, Rectangle srcRect)
        {
            var newBmp = new Bitmap(srcRect.Width, srcRect.Height, pcx.Bmp.PixelFormat);
            var gfx = Graphics.FromImage(newBmp);
            gfx.DrawImage(pcx.Bmp, new Rectangle(0, 0, srcRect.Width, srcRect.Height), srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, GraphicsUnit.Pixel);
            gfx.Dispose();
            return LoadTexture(newBmp);
        }

        private static int LoadTexture(Lgr.LgrImage pcx, RotateFlipType flip = RotateFlipType.RotateNoneFlipNone)
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
                (float) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (float) TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float) TextureWrapMode.Repeat);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);
            return textureIdentifier;
        }

        private void BaseInit(Control renderingTarget, RenderingSettings settings)
        {
            _activePlayerColor = Color.Black;
            _inActivePlayerColor = Color.Green;
            _bikePicTranslateXFacingLeft = BikePicXFacingLeft * Math.Cos(BikePicRotationConst * Constants.DegToRad) +
                                           BikePicYFacingLeft * Math.Sin(BikePicRotationConst * Constants.DegToRad);
            _bikePicTranslateYFacingLeft = BikePicXFacingLeft * Math.Sin(BikePicRotationConst * Constants.DegToRad) +
                                           BikePicYFacingLeft * Math.Cos(BikePicRotationConst * Constants.DegToRad);
            _bikePicTranslateXFacingRight = BikePicXFacingRight * Math.Cos(-BikePicRotationConst * Constants.DegToRad) +
                                            BikePicYFacingRight * Math.Sin(-BikePicRotationConst * Constants.DegToRad);
            _bikePicTranslateYFacingRight = BikePicXFacingRight * Math.Sin(-BikePicRotationConst * Constants.DegToRad) +
                                            BikePicYFacingRight * Math.Cos(-BikePicRotationConst * Constants.DegToRad);
            _aspectRatio = renderingTarget.Width / (double) renderingTarget.Height;
            _ctrlWindowInfo = Utilities.CreateWindowsWindowInfo(renderingTarget.Handle);
            InitializeOpengl(disableFrameBuffer: settings.DisableFrameBuffer);
            UpdateSettings(settings);
            _renderTarget = renderingTarget;
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
                foreach (var x in DrawableImages)
                    GL.DeleteTexture(x.TextureIdentifier);
                foreach (var x in _suspensions)
                    if (x != null)
                        GL.DeleteTexture(x.TextureIdentifier);
            }

            _groundTexture = null;
            _skyTexture = null;
        }

        public Vector GridOffset
        {
            get => _gridOffset;
            set
            {
                _gridOffset = value;
                _gridOffset.X %= _settings.GridSize;
                _gridOffset.Y %= _settings.GridSize;
            }
        }

        internal void SetGridSizeWithMouse(double newSize, Vector mouseCoords)
        {
            GridOffset.X = (GridOffset.X + GetFirstGridLine(newSize, GridOffset.X, XMin)
                            - mouseCoords.X + GetGridMouseRatio(_settings.GridSize, GridOffset.X, XMin, mouseCoords.X) *
                            newSize) % newSize;
            GridOffset.Y = (GridOffset.Y + GetFirstGridLine(newSize, GridOffset.Y, YMin)
                            - mouseCoords.Y + GetGridMouseRatio(_settings.GridSize, GridOffset.Y, YMin, mouseCoords.Y) *
                            newSize) % newSize;
            _settings.GridSize = newSize;
            RedrawScene();
        }

        private static double GetGridMouseRatio(double size, double offset, double min, double mouse)
        {
            var dist = mouse - GetFirstGridLine(size, offset, min);
            return (dist % size) / size;
        }

        private static double GetFirstGridLine(double size, double offset, double min)
        {
            var tmp = (Math.Floor(min / size) + 1) * size;
            var left = (tmp - (size + offset));
            return left;
        }

        private void DrawGrid()
        {
            var current = GetFirstGridLine(_settings.GridSize, GridOffset.X, XMin);
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple((int) _settings.LineWidth, LinePattern);
            GL.Scale(1.0, -1.0, 1.0);
            GL.Color3(_settings.GridColor);
            GL.Begin(PrimitiveType.Lines);
            while (!(current > XMax))
            {
                DrawLineFast(current, YMin, current, YMax);
                current += _settings.GridSize;
            }

            current = GetFirstGridLine(_settings.GridSize, GridOffset.Y, YMin);
            while (!(current > YMax))
            {
                DrawLineFast(XMin, current, XMax, current);
                current += _settings.GridSize;
            }

            GL.End();
            GL.Scale(1.0, -1.0, 1.0);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(_settings.LineWidth);
        }

        private void DrawObjectCenters()
        {
            foreach (var x in Lev.Objects)
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
                        if (_wrongLevVersion || ActivePlayerIndices.Count == 0)
                            DrawPoint(x.Position, _settings.AppleColor);
                        break;
                    case ObjectType.Start:
                        if (!_hideStartObject)
                        {
                            DrawPoint(x.Position, _settings.StartColor);
                            DrawPoint(x.Position.X + Level.RightWheelDifferenceFromLeftWheelX, x.Position.Y,
                                Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                            DrawPoint(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                                x.Position.Y + Level.HeadDifferenceFromLeftWheelY,
                                Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                        }

                        break;
                }
            }

            if (!_wrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                var i = 0;
                while (!(i >= _currentPlayerAppleEvents.Length || _currentPlayerAppleEvents[i].Time >= CurrentTime))
                    i++;
                for (var j = i; j < _currentPlayerAppleEvents.Length; j++)
                    DrawPoint(Lev.Objects[_currentPlayerAppleEvents[j].Info].Position,
                        _settings.AppleColor);
                foreach (var x in _notTakenApples)
                    DrawPoint(x.Position, _settings.AppleColor);
            }
        }

        private void DrawObjectFrames()
        {
            foreach (var x in Lev.Objects)
            {
                switch (x.Type)
                {
                    case ObjectType.Flower:
                        DrawCircle(x.Position, ObjectRadius, _settings.FlowerColor);
                        break;
                    case ObjectType.Killer:
                        DrawCircle(x.Position, ObjectRadius, _settings.KillerColor);
                        break;
                    case ObjectType.Apple:
                        if (_wrongLevVersion || ActivePlayerIndices.Count == 0)
                            DrawCircle(x.Position, ObjectRadius, _settings.AppleColor);
                        break;
                    case ObjectType.Start:
                        if (!_hideStartObject)
                        {
                            DrawPlayerFrames(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                                x.Position.Y + Level.HeadDifferenceFromLeftWheelY, 0, false, x.Position.X,
                                x.Position.Y, x.Position.X + Level.RightWheelDifferenceFromLeftWheelX,
                                x.Position.Y, 0, 0, _settings.StartColor);
                        }

                        break;
                }
            }

            if (!_wrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                var i = 0;
                while (!(i >= _currentPlayerAppleEvents.Length || _currentPlayerAppleEvents[i].Time >= CurrentTime))
                    i++;
                for (var j = i; j < _currentPlayerAppleEvents.Length; j++)
                    DrawCircle(Lev.Objects[_currentPlayerAppleEvents[j].Info].Position, ObjectRadius,
                        _settings.AppleColor);
                foreach (var x in _notTakenApples)
                    DrawCircle(x.Position, ObjectRadius, _settings.AppleColor);
            }
        }

        private void DrawPlayer(Player player, bool isActive = true)
        {
            DrawPlayer(player.GlobalBodyX, player.GlobalBodyY, player.LeftWheelX, player.LeftWheelY, player.RightWheelX,
                player.RightWheelY, player.LeftWheelRotation, player.RightWheelRotation, player.HeadX,
                player.HeadY, player.BikeRotationDegrees, player.Dir, player.ArmRotation,
                isActive || !_drawInActiveAsTransparent, !_drawOnlyPlayerFrames);
        }

        private void DrawPlayer(double globalBodyX, double globalBodyY, double leftWheelx, double leftWheely,
            double rightWheelx, double rightWheely, double leftWheelRotation,
            double rightWheelRotation, double headX, double headY, double bikeRotation,
            Direction direction, double armRotation, bool isActive, bool useGraphics)
        {
            var distance = ((_picturesInBackground ? 1 : BikeDistance) - Utils.BooleanToInteger(isActive)) /
                           1000.0 * (ZFar - ZNear) + ZNear;
            var isright = direction == Direction.Right;
            if (useGraphics && LgrGraphicsLoaded)
            {
                var rotation = bikeRotation * Constants.DegToRad;
                var rotationCos = Math.Cos(rotation);
                var rotationSin = Math.Sin(rotation);

                if (!isActive)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
                }

                //Wheels
                DrawWheel(leftWheelx, leftWheely, leftWheelRotation, distance);
                DrawWheel(rightWheelx, rightWheely, rightWheelRotation, distance);

                //Suspensions
                var x = Utils.BooleanToInteger(isright);
                for (var i = 0; i < 2; i++)
                {
                    GL.PushMatrix();
                    if (x == 0)
                        x = -1;
                    var yPos = globalBodyY + _suspensions[i].Y * rotationCos - _suspensions[i].X * x * rotationSin;
                    var xPos = globalBodyX - _suspensions[i].X * x * rotationCos - _suspensions[i].Y * rotationSin;
                    if (x == -1)
                        x = _suspensions[i].WheelNumber;
                    else
                        x -= i;
                    double wheelXpos;
                    double wheelYpos;
                    if (x == 0)
                    {
                        wheelXpos = leftWheelx;
                        wheelYpos = leftWheely;
                    }
                    else
                    {
                        wheelXpos = rightWheelx;
                        wheelYpos = rightWheely;
                    }

                    var xDiff = xPos - wheelXpos;
                    var yDiff = yPos - wheelYpos;
                    var angle = Math.Atan2(yDiff, xDiff) * Constants.RadToDeg;
                    var length = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
                    GL.Translate(wheelXpos, wheelYpos, 0);
                    GL.Rotate(angle, 0, 0, 1);
                    GL.BindTexture(TextureTarget.Texture2D, _suspensions[i].TextureIdentifier);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(-_suspensions[i].OffsetX, -_suspensions[i].Height / 2, distance);
                    GL.TexCoord2(1, 0);
                    GL.Vertex3(length + _suspensions[i].Height / 2, -_suspensions[i].Height / 2, distance);
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(length + _suspensions[i].Height / 2, _suspensions[i].Height / 2, distance);
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(-_suspensions[i].OffsetX, _suspensions[i].Height / 2, distance);
                    GL.End();
                    GL.PopMatrix();
                }

                //Head
                GL.PushMatrix();
                GL.Translate(headX, headY, 0);
                GL.Rotate(bikeRotation + 180, 0, 0, 1);
                if (!isright)
                    GL.Scale(-1.0, 1.0, 1.0);
                GL.Translate(-headX, -headY, 0);
                DrawPicture(_headPic, headX - Constants.HeadDiameter / 2.0, headY - Constants.HeadDiameter / 2.0,
                    Constants.HeadDiameter, Constants.HeadDiameter, distance);
                GL.PopMatrix();

                //Bike
                GL.PushMatrix();
                double bikePicTranslateX;
                double bikePicTranslateY;
                GL.Translate(globalBodyX, globalBodyY, 0);
                if (!isright)
                {
                    bikePicTranslateX = _bikePicTranslateXFacingLeft;
                    bikePicTranslateY = _bikePicTranslateYFacingLeft;
                    GL.Rotate(bikeRotation + BikePicRotationConst + 180, 0, 0, 1);
                    GL.Scale(-1.0, 1.0, 1.0);
                }
                else
                {
                    GL.Rotate(bikeRotation + 180 - BikePicRotationConst, 0, 0, 1);
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
                var footx = globalBodyX + footsx * rotationCos - footsy * rotationSin;
                var footy = globalBodyY + footsx * rotationSin + footsy * rotationCos;
                var thighstartx = headX + thighsx * rotationCos - thighsy * rotationSin;
                var thighstarty = headY + thighsx * rotationSin + thighsy * rotationCos;
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
                GL.Translate(headX, headY, 0);
                if (isright)
                {
                    GL.Scale(-1.0, 1.0, 1.0);
                    GL.Rotate(-bikeRotation - BodyRotation, 0, 0, 1);
                }
                else
                {
                    GL.Rotate(bikeRotation - BodyRotation, 0, 0, 1);
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
                var armx = headX + armsx * rotationCos - armsy * rotationSin;
                var army = headY + armsx * rotationSin + armsy * rotationCos;
                var initialx = globalBodyX + handsx * rotationCos - handsy * rotationSin;
                var initialy = globalBodyY + handsx * rotationSin + handsy * rotationCos;
                var dist = Math.Sqrt((initialx - armx) * (initialx - armx) + (initialy - army) * (initialy - army));
                double armAngle;
                if (isright)
                {
                    armAngle = Math.Atan2(initialy - army, initialx - armx) + armRotation * Constants.DegToRad;
                }
                else
                {
                    armAngle = Math.Atan2(initialy - army, initialx - armx) + armRotation * Constants.DegToRad;
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

                if (!isActive)
                    GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.DepthTest);
                if (!isActive)
                {
                    GL.Enable(EnableCap.LineStipple);
                    GL.LineStipple((int) _settings.LineWidth, LinePattern);
                }

                DrawPlayerFrames(headX, headY, bikeRotation, isright, leftWheelx, leftWheely, rightWheelx, rightWheely,
                    leftWheelRotation, rightWheelRotation,
                    isActive ? _activePlayerColor : _inActivePlayerColor);
                if (!isActive)
                {
                    GL.Disable(EnableCap.LineStipple);
                }

                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.DepthTest);
            }
        }

        private void DrawPlayerFrames(double headX, double headY, double bikeRotation, bool isright, double leftWheelx,
            double leftWheely, double rightWheelx, double rightWheely,
            double leftWheelRotation, double rightWheelRotation, Color playerColor)
        {
            var headCos = Math.Cos(bikeRotation * Constants.DegToRad);
            var headSin = Math.Sin(bikeRotation * Constants.DegToRad);
            var f = isright ? 1 : -1;
            var headLineEndPointX = headX + f * headCos * Constants.HeadDiameter / 2;
            var headLineEndPointY = headY + f * headSin * Constants.HeadDiameter / 2;
            DrawCircle(leftWheelx, leftWheely, ObjectRadius, playerColor);
            DrawCircle(rightWheelx, rightWheely, ObjectRadius, playerColor);
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
                        wheelx = leftWheelx;
                        wheely = leftWheely;
                        wheelrot = leftWheelRotation;
                    }
                    else
                    {
                        wheelx = rightWheelx;
                        wheely = rightWheely;
                        wheelrot = rightWheelRotation;
                    }

                    GL.Vertex2(wheelx, wheely);
                    GL.Vertex2(wheelx + ObjectRadius * Math.Cos(wheelrot + j * Math.PI / 2),
                        wheely + ObjectRadius * Math.Sin(wheelrot + j * Math.PI / 2));
                }
            }

            GL.End();
            DrawCircle(headX, headY, Constants.HeadDiameter / 2, playerColor);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(headX, headY);
            GL.Vertex2(headLineEndPointX, headLineEndPointY);
            GL.End();
        }

        private void DrawPlayers(IList focusIndices, IList visibleIndices)
        {
            foreach (int x in visibleIndices)
            {
                var isSelected = focusIndices.Contains(x);
                DrawPlayer(_players[x], isSelected);
            }
        }

        private void DrawSceneNoDriverFocus()
        {
            DrawScene(false, _showDriverPath);
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
            var opts = ToolkitOptions.Default;
            opts.Backend = PlatformBackend.PreferNative;
            Toolkit.Init(opts);
            _gfxContext = new GraphicsContext(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 8, 8), _ctrlWindowInfo);
            _gfxContext.MakeCurrent(_ctrlWindowInfo);
            _gfxContext.LoadAll();
            GL.MatrixMode(MatrixMode.Projection);
            GL.ClearDepth(GroundDepth);
            GL.StencilMask(255);
            GL.ClearStencil(GroundStencil);
            GL.DepthFunc(DepthFunction.Gequal);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.9f);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
            GL.Hint(HintTarget.TextureCompressionHint, HintMode.Fastest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
            GL.Enable(EnableCap.PointSmooth);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float) TextureEnvMode.Replace);

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
            DrawableImages = new List<DrawableImage>();
            try
            {
                CurrentLgr = new Lgr(lgr);
            }
            catch (Exception ex)
            {
                Utils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + ex.Message);
                return;
            }

            var firstFrameRect = new Rectangle(0, 0, 40, 40);
            foreach (var x in CurrentLgr.LgrImages)
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
                    DrawableImages.Add(new DrawableImage(LoadTexture(x), x.Bmp.Width * PictureFactor,
                        x.Bmp.Height * PictureFactor, x.ClippingType, x.Distance,
                        x.Name, x.Type));
                }
            }

            _lgrGraphicsLoaded = true;
        }

        private void PerformZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (_settings.SmoothZoomEnabled)
                SmoothZoom(newZoomLevel, newCenterX, newCenterY);
            else
            {
                ZoomLevel = newZoomLevel;
                CenterX = newCenterX;
                CenterY = newCenterY;
                if (!Playing)
                    DrawSceneNoDriverFocus();
            }
        }

        private void SmoothZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (_smoothZoomInProgress)
                return;
            _smoothZoomInProgress = true;
            var oldZoomLevel = ZoomLevel;
            var oldCenterX = (XMax + XMin) / 2;
            var oldCenterY = (YMax + YMin) / 2;
            var zoomTimer = new Stopwatch();
            long elapsedTime = 0;
            var initialTime = CurrentTime;
            zoomTimer.Start();
            while (elapsedTime <= _settings.SmoothZoomDuration)
            {
                ZoomLevel = oldZoomLevel + (newZoomLevel - oldZoomLevel) * elapsedTime / _settings.SmoothZoomDuration;
                CenterX = oldCenterX + (newCenterX - oldCenterX) * elapsedTime / _settings.SmoothZoomDuration;
                CenterY = oldCenterY + (newCenterY - oldCenterY) * elapsedTime / _settings.SmoothZoomDuration;
                if (Playing)
                {
                    CurrentTime = initialTime + elapsedTime / 1000.0 * _playBackSpeed;
                    DrawSceneDefault();
                }
                else
                    DrawSceneNoDriverFocus();

                elapsedTime = zoomTimer.ElapsedMilliseconds;
            }

            zoomTimer.Stop();
            //Draw the last frame separately to make sure the zoom was made correctly
            ZoomLevel = newZoomLevel;
            if (!Playing)
            {
                CenterX = newCenterX;
                CenterY = newCenterY;
                DrawSceneNoDriverFocus();
            }

            _smoothZoomInProgress = false;
        }

        internal class DrawableImage
        {
            internal readonly ClippingType DefaultClipping;
            internal readonly int DefaultDistance;
            internal readonly double Height;
            internal readonly string Name;
            internal readonly int TextureIdentifier;
            internal readonly Lgr.ImageType Type;
            internal readonly double Width;

            internal DrawableImage(int textureId, double width, double height, ClippingType clipping,
                int distance,
                string name, Lgr.ImageType type)
            {
                TextureIdentifier = textureId;
                Width = width;
                Height = height;
                DefaultClipping = clipping;
                DefaultDistance = distance;
                Name = name;
                Type = type;
            }

            internal double WidthMinusMargin => Width - 2 * EmptyPixelXMargin;

            internal double HeightMinusMargin => Height - 2 * EmptyPixelYMargin;

            internal double EmptyPixelXMargin
            {
                get
                {
                    switch (Name)
                    {
                        case "maskhor":
                            return 0.029;
                        case "masklitt":
                            return 0.015;
                        case "maskbig":
                            return 0.092;
                        default:
                            return 0.092;
                    }
                }
            }

            internal double EmptyPixelYMargin
            {
                get
                {
                    switch (Name)
                    {
                        case "maskhor":
                            return 0.029;
                        case "masklitt":
                            return 0.015;
                        case "maskbig":
                            return 0.112;
                        default:
                            return 0.112;
                    }
                }
            }
        }

        internal class Suspension
        {
            internal double Height;
            internal double OffsetX;
            internal int TextureIdentifier;
            internal int WheelNumber;
            internal double X;
            internal double Y;

            internal Suspension(int textureId, double x, double y, double height, double offsetX, int wheelNumber)
            {
                TextureIdentifier = textureId;
                X = x;
                Y = y;
                Height = height;
                OffsetX = offsetX;
                WheelNumber = wheelNumber;
            }
        }

        public void DrawDashLine(Vector v1, Vector v2, Color color)
        {
            DrawDashLine(v1.X, v1.Y, v2.X, v2.Y, color);
        }

        public void DrawDashLine(double x1, double y1, double x2, double y2, Color color)
        {
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple((int) _settings.LineWidth, LinePattern);
            DrawLine(x1, y1, x2, y2, color);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(_settings.LineWidth);
        }

        public void SaveSnapShot(string fileName)
        {
            using (var bmp = GetSnapShot())
            {
                bmp.Save(fileName, ImageFormat.Png);
            }
        }
    }
}