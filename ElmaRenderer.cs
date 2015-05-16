using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager
{
    internal class ElmaRenderer : IDisposable
    {
        //Constants
        internal const double ObjectDiameter = ObjectRadius * 2;
        internal const double ObjectRadius = 0.4;
        internal List<int> ActivePlayerIndices;
        internal Action AdditionalPolys;
        internal Action AfterDrawing;
        internal Lgr CurrentLgr;
        internal Action CustomRendering;
        internal Color[] DrivingLineColors = new Color[] {};
        internal Level Lev;
        internal bool Playing;
        internal List<int> VisiblePlayerIndices;
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
        private const double zFar = 1;
        private const double zNear = -1;
        private readonly int[] _viewPort = new int[4];
        private static bool _smoothZoomInProgress;
        private Color ActivePlayerColor;
        private Dictionary<int, int> ApplePics = new Dictionary<int, int>();
        private int ArmPic;
        private double AspectRatio;
        private int BikePic;
        private double BikePicTranslateXFacingLeft;
        private double BikePicTranslateXFacingRight;
        private double BikePicTranslateYFacingLeft;
        private double BikePicTranslateYFacingRight;
        private int BodyPic;
        private IWindowInfo CtrlWindowInfo;
        private PlayerEvent[] CurrentPlayerAppleEvents;
        private bool Disposed;
        private bool DrawInActiveAsTransparent;
        private bool DrawOnlyPlayerFrames;

        public List<DrawableImage> DrawableImages { get; set; }

        private int FlowerPic;
        private bool FollowDriver;
        private double FrameStep = 0.02;
        private GraphicsContext GFXContext;
        private DrawableImage GroundTexture;
        private int HandPic;
        private int HeadPic;
        private bool HideStartObject;
        private Color InActivePlayerColor;
        private double InitialTime;
        private int KillerPic;
        private bool LGRGraphicsLoaded;
        private int LegPic;
        private bool LockedCamera;
        private bool LoopPlaying;
        private double MidX;
        private double MidY;
        private bool MultiSpy;
        private List<Level.Object> NotTakenApples;
        private bool OpenGLInitialized;
        private bool PicturesInBackground;
        private double PlayBackSpeed = 1.0;
        private Stopwatch PlayTimer = new Stopwatch();
        private List<Player> Players;
        private RenderingSettings Settings;
        private bool ShowDriverPath;
        private DrawableImage SkyTexture;
        private Suspension[] Suspensions = new Suspension[2];
        private int ThighPic;
        private int WheelPic;
        private bool WrongLevVersion;
        private double ZoomFillxMax;
        private double ZoomFillxMin;
        private double ZoomFillyMax;
        private double ZoomFillyMin;
        private double _CenterX;
        private double _CenterY;
        private double _ZoomLevel;
        private double _currentTime;

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
            get { return _CenterX; }
            set
            {
                if (value < ZoomFillxMin - XSize)
                    value = ZoomFillxMin - XSize;
                if (value > ZoomFillxMax + XSize)
                    value = ZoomFillxMax + XSize;
                _CenterX = value;
            }
        }

        internal double CenterY
        {
            get { return _CenterY; }
            set
            {
                if (value < ZoomFillyMin - YSize)
                    value = ZoomFillyMin - YSize;
                if (value > ZoomFillyMax + YSize)
                    value = ZoomFillyMax + YSize;
                _CenterY = value;
            }
        }

        internal double CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                Players.ForEach(x => { x.CurrentTime = _currentTime; });
            }
        }

        internal double MaxTime { get; private set; }

        internal double XMax
        {
            get { return CenterX + ZoomLevel * AspectRatio; }
        }

        internal double XMin
        {
            get { return CenterX - ZoomLevel * AspectRatio; }
        }

        internal double XSize
        {
            get { return XMax - XMin; }
        }

        internal double YMax
        {
            get { return CenterY + ZoomLevel; }
        }

        internal double YMin
        {
            get { return CenterY - ZoomLevel; }
        }

        internal double YSize
        {
            get { return YMax - YMin; }
        }

        internal double ZoomLevel
        {
            get { return _ZoomLevel; }
            set
            {
                if (value > MaxDimension * 2)
                    value = MaxDimension * 2;
                if (value < MinimumZoom)
                    value = MinimumZoom;
                _ZoomLevel = value;
            }
        }

        private double MaxDimension
        {
            get { return Math.Max(ZoomFillxMax - ZoomFillxMin, ZoomFillyMax - ZoomFillyMin); }
        }

        internal bool LgrGraphicsLoaded
        {
            get { return LGRGraphicsLoaded; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal static Bitmap GetSnapShot(int width, int height)
        {
            var snapShotBmp = new Bitmap(width, height);
            BitmapData bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
                          bmpData.Scan0);
            snapShotBmp.UnlockBits(bmpData);
            snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return snapShotBmp;
        }

        private int GetApple(int animNum)
        {
            int apple;
            return ApplePics.TryGetValue(animNum, out apple) ? apple : ApplePics[1];
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
            int accuracy = Settings.CircleDrawingAccuracy;
            GL.Color4(circleColor);
            GL.Begin(BeginMode.LineLoop);
            for (int i = 0; i <= accuracy; i++)
                GL.Vertex2(x + Math.Cos(i * 360 / (double) accuracy * Math.PI / 180) * radius,
                           y + Math.Sin(i * 360 / (double) accuracy * Math.PI / 180) * radius);
            GL.End();
        }

        internal void DrawCircle(Vector v, double radius, Color circleColor)
        {
            DrawCircle(v.X, v.Y, radius, circleColor);
        }

        internal void DrawDummyPlayer(double leftWheelx, double leftWheely)
        {
            DrawPlayer(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX,
                       leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY, leftWheelx, leftWheely,
                       leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely, 0, 0,
                       leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely - Level.HeadDifferenceFromLeftWheelY,
                       0, Direction.Left, 0, false);
        }

        internal void DrawFilledTriangles(IEnumerable<Vector[]> triangles)
        {
            const double depth = zFar - (zFar - zNear) * SkyDepth;
            GL.Begin(BeginMode.Triangles);
            foreach (Vector[] triangle in triangles)
                foreach (Vector x in triangle)
                    GL.Vertex3(x.X, x.Y, depth);
            GL.End();
        }

        internal void DrawFlower(Vector v)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            DrawObject(FlowerPic, v);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.AlphaTest);
        }

        internal void DrawKiller(Vector v)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            DrawObject(KillerPic, v);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.AlphaTest);
        }

        internal void DrawLine(Vector v1, Vector v2, Color color, double depth = 0)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(v1.X, v1.Y, depth);
            GL.Vertex3(v2.X, v2.Y, depth);
            GL.End();
        }

        internal void DrawLine(double x1, double y1, double x2, double y2, Color color, double depth = 0)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(x1, y1, depth);
            GL.Vertex3(x2, y2, depth);
            GL.End();
        }

        internal void DrawLineStrip(Polygon polygon, Color color, double depth = 0)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.LineStrip);
            foreach (Vector x in polygon.Vertices)
                GL.Vertex3(x.X, x.Y, depth);
            GL.End();
        }

        internal void DrawPicture(int pic, double startx, double starty, double endx, double endy, double width,
                                  double dist, bool mirror, double offset = 0.0)
        {
            double lx = endx - startx;
            double ly = endy - starty;
            double l = Math.Sqrt(lx * lx + ly * ly);
            double x = width * ly / (2 * l);
            double y = width * lx / (2 * l);
            double offsetx = offset * lx / l;
            double offsety = offset * ly / l;
            GL.BindTexture(TextureTarget.Texture2D, pic);
            if (mirror)
            {
                GL.Begin(BeginMode.Quads);
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
                GL.Begin(BeginMode.Quads);
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
            GL.Begin(BeginMode.Points);
            GL.Vertex3(v.X, v.Y, depth);
            GL.End();
        }

        internal void DrawPoint(double x, double y, Color color, double depth = 0)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.Points);
            GL.Vertex3(x, y, depth);
            GL.End();
        }

        internal void DrawPolygon(Polygon polygon, Color color, double depth = 0)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.LineLoop);
            foreach (Vector x in polygon.Vertices)
                GL.Vertex3(x.X, x.Y, depth);
            GL.End();
        }

        internal void DrawRectangle(double x1, double y1, double x2, double y2, Color rectColor)
        {
            GL.Color4(rectColor);
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1, y2);
            GL.End();
        }

        internal void DrawRectangle(double x1, double y1, double x2, double y2)
        {
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1, y2);
            GL.End();
        }

        internal void DrawRectangle(Vector v1, Vector v2, Color rectColor)
        {
            GL.Color4(rectColor);
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(v1.X, v1.Y);
            GL.Vertex2(v2.X, v1.Y);
            GL.Vertex2(v2.X, v2.Y);
            GL.Vertex2(v1.X, v2.Y);
            GL.End();
        }

        internal void DrawRectangle(Vector v1, Vector v2)
        {
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(v1.X, v1.Y);
            GL.Vertex2(v2.X, v1.Y);
            GL.Vertex2(v2.X, v2.Y);
            GL.Vertex2(v1.X, v2.Y);
            GL.End();
        }

        internal void DrawScene(bool zoomToDriver, bool showDriverPath)
        {
            GL.LoadIdentity();
            if (ActivePlayerIndices.Count > 0 && zoomToDriver)
            {
                CenterX = Players[ActivePlayerIndices[0]].GlobalBodyX;
                CenterY = Players[ActivePlayerIndices[0]].GlobalBodyY;
                GL.Ortho(XMin, XMax, YMin, YMax, zNear, zFar);
                if (LockedCamera)
                {
                    GL.Translate(CenterX, CenterY, 0);
                    GL.Rotate(-Players[ActivePlayerIndices[0]].BikeRotation, 0, 0, 1);
                    GL.Translate(-CenterX, -CenterY, 0);
                }
            }
            else
            {
                GL.Ortho(XMin, XMax, YMin, YMax, zNear, zFar);
            }
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.ColorBufferBit);
            GL.Scale(1.0, -1.0, 1.0);
            GL.Enable(EnableCap.StencilTest);
            GL.Disable(EnableCap.Texture2D);
            GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Decr);
            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
            GL.ColorMask(false, false, false, false);
            GL.Begin(BeginMode.Triangles);
            foreach (Polygon k in Lev.Polygons)
                if (!k.IsGrass)
                    DrawFilledTrianglesFast(k.Decomposition, zFar - (zFar - zNear) * SkyDepth);
            GL.End();
            if (AdditionalPolys != null)
                AdditionalPolys();
            GL.ColorMask(true, true, true, true);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.Scale(1.0, -1.0, 1.0);
            if (Settings.ShowGround)
            {
                const double depth = zFar - (zFar - zNear) * GroundDepth;
                if (Settings.GroundTextureEnabled && LgrGraphicsLoaded)
                {
                    GL.BindTexture(TextureTarget.Texture2D, GroundTexture.TextureIdentifier);
                    double gtW = GroundTexture.Width;
                    double gtH = GroundTexture.Height;
                    if (Settings.ZoomTextures)
                    {
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex3(MidX - TextureVertexConst, MidY - TextureVertexConst, depth);
                        GL.TexCoord2(TextureCoordConst / gtW, 0);
                        GL.Vertex3(MidX + TextureVertexConst, MidY - TextureVertexConst, depth);
                        GL.TexCoord2(TextureCoordConst / gtW, TextureCoordConst / gtW * gtW / gtH);
                        GL.Vertex3(MidX + TextureVertexConst, MidY + TextureVertexConst, depth);
                        GL.TexCoord2(0, TextureCoordConst / gtW * gtW / gtH);
                        GL.Vertex3(MidX - TextureVertexConst, MidY + TextureVertexConst, depth);
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(BeginMode.Quads);
                        const double texminx = 0;
                        const double texminy = 0;
                        GL.TexCoord2(texminx, texminy);
                        GL.Vertex3(MidX - TextureVertexConst, MidY - TextureVertexConst, depth);
                        GL.TexCoord2(texminx + TextureZoomConst / gtW / ZoomLevel, texminy);
                        GL.Vertex3(MidX + TextureVertexConst, MidY - TextureVertexConst, depth);
                        GL.TexCoord2(texminx + TextureZoomConst / gtW / ZoomLevel,
                                     texminy + TextureZoomConst / gtW * gtW / gtH / ZoomLevel);
                        GL.Vertex3(MidX + TextureVertexConst, MidY + TextureVertexConst, depth);
                        GL.TexCoord2(texminx, texminy + TextureZoomConst / gtW * gtW / gtH / ZoomLevel);
                        GL.Vertex3(MidX - TextureVertexConst, MidY + TextureVertexConst, depth);
                        GL.End();
                        //double xs = TextureVertexConst* ZoomLevel-ZoomLevel*CenterX;
                        //GL.TexCoord2(0, 0);
                        //GL.Vertex3(MidX - xs, MidY - xs, depth);
                        //GL.TexCoord2(TextureZoomConst / gtW, 0);
                        //GL.Vertex3(MidX + xs, MidY - xs, depth);
                        //GL.TexCoord2(TextureZoomConst / gtW, TextureZoomConst / gtW * gtW / gtH);
                        //GL.Vertex3(MidX + xs, MidY + xs, depth);
                        //GL.TexCoord2(0, TextureZoomConst / gtW * gtW / gtH);
                        //GL.Vertex3(MidX - xs, MidY + xs, depth);
                        //GL.End();
                    }
                }
                else
                {
                    GL.Disable(EnableCap.Texture2D);
                    GL.Color4(Settings.GroundFillColor);
                    GL.Begin(BeginMode.Quads);
                    GL.Vertex3(MidX - TextureVertexConst, MidY - TextureVertexConst, depth);
                    GL.Vertex3(MidX + TextureVertexConst, MidY - TextureVertexConst, depth);
                    GL.Vertex3(MidX + TextureVertexConst, MidY + TextureVertexConst, depth);
                    GL.Vertex3(MidX - TextureVertexConst, MidY + TextureVertexConst, depth);
                    GL.End();
                    GL.Enable(EnableCap.Texture2D);
                }
            }
            GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
            if (Settings.SkyTextureEnabled && LgrGraphicsLoaded)
            {
                const double depth = zFar - (zFar - zNear) * SkyDepth;
                GL.BindTexture(TextureTarget.Texture2D, SkyTexture.TextureIdentifier);
                if (Settings.ZoomTextures)
                {
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY - TextureVertexConst, depth);
                    GL.TexCoord2(TextureCoordConst / SkyTexture.Width, 0);
                    GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY - TextureVertexConst, depth);
                    GL.TexCoord2(TextureCoordConst / SkyTexture.Width,
                                 TextureCoordConst / SkyTexture.Width * SkyTexture.Width / SkyTexture.Height);
                    GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY + TextureVertexConst, depth);
                    GL.TexCoord2(0, TextureCoordConst / SkyTexture.Width * SkyTexture.Width / SkyTexture.Height);
                    GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY + TextureVertexConst, depth);
                    GL.End();
                }
                else
                {
                    //GL.Begin(BeginMode.Quads);
                    //GL.TexCoord2(0, 0);
                    //GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY - TextureVertexConst, depth);
                    //GL.TexCoord2(TextureZoomConst / SkyTexture.Width / ZoomLevel, 0);
                    //GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY - TextureVertexConst, depth);
                    //GL.TexCoord2(TextureZoomConst / SkyTexture.Width / ZoomLevel,
                    //             TextureZoomConst / SkyTexture.Width * SkyTexture.Width / SkyTexture.Height / ZoomLevel);
                    //GL.Vertex3(CenterX / 2 + TextureVertexConst, CenterY + TextureVertexConst, depth);
                    //GL.TexCoord2(0,
                    //             TextureZoomConst / SkyTexture.Width * SkyTexture.Width / SkyTexture.Height / ZoomLevel);
                    //GL.Vertex3(CenterX / 2 - TextureVertexConst, CenterY + TextureVertexConst, depth);
                    //GL.End();
                    double xdelta = CenterX /SkyTexture.Width;
                    GL.PushMatrix();
                    GL.LoadIdentity();
                    GL.Ortho(0, 1, 0, 1, zNear, zFar);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(xdelta, 0);
                    GL.Vertex3(0,1,depth);
                    GL.TexCoord2(2 + xdelta, 0);
                    GL.Vertex3(1,1,depth);
                    GL.TexCoord2(2 + xdelta,
                                  2 / AspectRatio);
                    GL.Vertex3(1,0, depth);
                    GL.TexCoord2(xdelta,
                                  2 / AspectRatio);
                    GL.Vertex3(0,0, depth);
                    GL.End();
                    GL.PopMatrix();
                }
            }
            GL.Scale(1.0, -1.0, 1.0);
            GL.Enable(EnableCap.AlphaTest);
            if (LgrGraphicsLoaded)
            {
                GL.DepthFunc(DepthFunction.Greater);
                GL.Enable(EnableCap.ScissorTest);
                foreach (Level.Picture picture in Lev.Pictures)
                {
                    int x = (int) ((picture.Position.X - XMin) / (XMax - XMin) * _viewPort[2]);
                    int y = (int) (((-picture.Position.Y - picture.Height) - YMin) / (YMax - YMin) * _viewPort[3]);
                    int w = (int) ((picture.Position.X + picture.Width - XMin) / (XMax - XMin) * _viewPort[2]) - x;
                    int h = (int) ((-picture.Position.Y - YMin) / (YMax - YMin) * _viewPort[3]) - y;
                    GL.Scissor(x, y, w, h);
                    if (picture.IsPicture && Settings.ShowPictures)
                    {
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                        switch (picture.Clipping)
                        {
                            case Level.ClippingType.Unclipped:
                                GL.StencilFunc(StencilFunction.Always, 0, StencilMask);
                                break;
                            case Level.ClippingType.Sky:
                                GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                                break;
                            case Level.ClippingType.Ground:
                                GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                                break;
                        }
                        DrawPicture(picture.Id, picture.Position.X, picture.Position.Y, picture.Width, picture.Height,
                                    (picture.Distance / 1000.0 * (zFar - zNear)) + zNear);
                    }
                    else if (!picture.IsPicture && Settings.ShowTextures)
                    {
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Invert);
                        GL.ColorMask(false, false, false, false);
                        switch (picture.Clipping)
                        {
                            case Level.ClippingType.Ground:
                                GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                                break;
                            case Level.ClippingType.Sky:
                                GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                                break;
                            case Level.ClippingType.Unclipped:
                                GL.StencilFunc(StencilFunction.Gequal, 5, StencilMask);
                                break;
                        }
                        DrawPicture(picture.Id, picture.Position.X, picture.Position.Y, picture.Width, picture.Height,
                                    (picture.Distance / 1000.0 * (zFar - zNear)) + zNear);

                        GL.BindTexture(TextureTarget.Texture2D, picture.TextureId);
                        GL.StencilFunc(StencilFunction.Lequal, 5, StencilMask);
                        GL.ColorMask(true, true, true, true);
                        GL.DepthMask(false);
                        GL.Scale(1.0, -1.0, 1.0);
                        const double depth = zFar - (zFar - zNear) * 1;
                        if (Settings.ZoomTextures)
                        {
                            GL.Begin(BeginMode.Quads);
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(MidX - TextureVertexConst, MidY - TextureVertexConst, depth);
                            GL.TexCoord2(TextureCoordConst / picture.TextureWidth, 0);
                            GL.Vertex3(MidX + TextureVertexConst, MidY - TextureVertexConst, depth);
                            GL.TexCoord2(TextureCoordConst / picture.TextureWidth,
                                         TextureCoordConst / picture.TextureWidth * picture.AspectRatio);
                            GL.Vertex3(MidX + TextureVertexConst, MidY + TextureVertexConst, depth);
                            GL.TexCoord2(0, TextureCoordConst / picture.TextureWidth * picture.AspectRatio);
                            GL.Vertex3(MidX - TextureVertexConst, MidY + TextureVertexConst, depth);
                            GL.End();
                        }
                        else
                        {
                            GL.Begin(BeginMode.Quads);
                            double ymin = MidY - TextureVertexConst;
                            double ymax = MidY + TextureVertexConst;
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(MidX - TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureZoomConst / picture.TextureWidth / ZoomLevel, 0);
                            GL.Vertex3(MidX + TextureVertexConst, ymin, depth);
                            GL.TexCoord2(TextureZoomConst / picture.TextureWidth / ZoomLevel,
                                         TextureZoomConst / picture.TextureWidth * picture.AspectRatio / ZoomLevel);
                            GL.Vertex3(MidX + TextureVertexConst, ymax, depth);
                            GL.TexCoord2(0, TextureZoomConst / picture.TextureWidth * picture.AspectRatio / ZoomLevel);
                            GL.Vertex3(MidX - TextureVertexConst, ymax, depth);
                            GL.End();
                        }
                        GL.Scale(1.0, -1.0, 1.0);
                        GL.DepthMask(true);
                    }
                }
                GL.Disable(EnableCap.ScissorTest);
                GL.DepthFunc(DepthFunction.Gequal);
            }
            GL.Disable(EnableCap.StencilTest);
            GL.Scale(1.0, -1.0, 1.0);
            DrawPlayers(ActivePlayerIndices, VisiblePlayerIndices);
            GL.Scale(1.0, -1.0, 1.0);
            if (Settings.ShowObjects && LgrGraphicsLoaded) //BUG Drawing order should be: 1. killers, 2. apples, 3. flowers.
            {
                double depth = PicturesInBackground ? 0 : 0.5 * (zFar - zNear) + zNear;
                foreach (var x in Lev.Objects)
                {
                    switch (x.Type)
                    {
                        case Level.ObjectType.Flower:
                            DrawObject(FlowerPic, x.Position, depth);
                            break;
                        case Level.ObjectType.Killer:
                            DrawObject(KillerPic, x.Position, depth);
                            break;
                        case Level.ObjectType.Apple:
                            if (WrongLevVersion || ActivePlayerIndices.Count == 0)
                                DrawObject(GetApple(x.AnimationNumber), x.Position, depth);
                            break;
                        case Level.ObjectType.Start:
                            if (!HideStartObject)
                            {
                                GL.Scale(1, -1, 1);
                                DrawDummyPlayer(x.Position.X, -x.Position.Y);
                                GL.Scale(1, -1, 1);
                            }
                            break;
                    }
                }
                if (!WrongLevVersion && ActivePlayerIndices.Count > 0)
                {
                    int i = 0;
                    while (!(i >= CurrentPlayerAppleEvents.Count() || CurrentPlayerAppleEvents[i].Time >= CurrentTime))
                        i++;
                    for (int j = i; j < CurrentPlayerAppleEvents.Count(); j++)
                    {
                        Level.Object z = Lev.Apples[CurrentPlayerAppleEvents[j].Info];
                        DrawObject(GetApple(z.AnimationNumber), z.Position);
                    }
                    foreach (Level.Object x in NotTakenApples)
                        DrawObject(GetApple(x.AnimationNumber), x.Position);
                }
            }
            DisableCaps();
            if (Settings.ShowGrid)
                DrawGrid();
            if (Settings.ShowObjectFrames)
                DrawObjectFrames();
            if (Settings.ShowObjectCenters)
                DrawObjectCenters();
            if (Settings.ShowGravityAppleArrows && (Settings.ShowObjectFrames || (LGRGraphicsLoaded && Settings.ShowObjects)))
            {
                GL.Color3(Settings.AppleGravityArrowColor);
                foreach (var o in Lev.Objects)
                {
                    if (o.Type == Level.ObjectType.Apple && o.AppleType != Level.AppleTypes.Normal)
                    {
                        double arrowRotation = 0.0;
                        switch (o.AppleType)
                        {
                            case Level.AppleTypes.GravityUp:
                                arrowRotation = 180.0;
                                break;
                            case Level.AppleTypes.GravityDown:
                                arrowRotation = 0.0;
                                break;
                            case Level.AppleTypes.GravityLeft:
                                arrowRotation = 90.0;
                                break;
                            case Level.AppleTypes.GravityRight:
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
                        GL.Begin(BeginMode.LineLoop);
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
            }
            foreach (Polygon x in Lev.Polygons)
            {
                if (x.IsGrass)
                {
                    if (Settings.ShowGrassEdges)
                    {
                        DrawLineStrip(x, Settings.GrassEdgeColor);
                    }
                }
                else if (Settings.ShowGroundEdges)
                    DrawPolygon(x, Settings.GroundEdgeColor);
            }

            GL.Color4(Settings.TextureFrameColor);
            foreach (Level.Picture z in Lev.Pictures)
            {
                if (z.IsPicture)
                {
                    if (!Settings.ShowPictureFrames) continue;
                    GL.Color4(Settings.PictureFrameColor);
                }
                else
                {
                    if (!Settings.ShowTextureFrames)
                        continue;
                    GL.Color4(Settings.TextureFrameColor);
                }
                DrawRectangle(z.Position.X, z.Position.Y, z.Position.X + z.Width, z.Position.Y + z.Height);
            }

            if (Settings.ShowVertices)
            {
                bool showGrassVertices = Settings.ShowGrassEdges;
                bool showGroundVertices = Settings.ShowGroundEdges || (Settings.ShowGround && LGRGraphicsLoaded);
                GL.Color3(Settings.VertexColor);
                if (Settings.UseCirclesForVertices)
                {
                    GL.PointSize((float)(Settings.VertexSize * 300));
                    GL.Begin(PrimitiveType.Points);
                foreach (Polygon x in Lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (Vector z in x.Vertices)
                                GL.Vertex3(z.X, z.Y, 0);
                }
                else
                {
                    GL.Begin(PrimitiveType.Triangles);
                    foreach (Polygon x in Lev.Polygons)
                        if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                            foreach (Vector z in x.Vertices)
                            DrawEquilateralTriangleFast(z, _ZoomLevel * Settings.VertexSize);
                }
                GL.End();
            }
            if (CustomRendering != null)
                CustomRendering();
            GL.Scale(1.0, -1.0, 1.0);
            if (ActivePlayerIndices.Count > 0 && showDriverPath)
            {
                foreach (int x in ActivePlayerIndices)
                {
                    if (Players[x].FrameCount > 1)
                    {
                        GL.Color4(DrivingLineColors[x]);
                        GL.Begin(BeginMode.LineStrip);
                        for (int k = 0; k < Players[x].FrameCount; k++)
                            GL.Vertex2(Players[x].GlobalBodyFromIndex(k).X, Players[x].GlobalBodyFromIndex(k).Y);
                        GL.End();
                    }
                }
            }
            GFXContext.SwapBuffers();
            if (AfterDrawing != null)
                AfterDrawing();
        }

        internal void DrawSceneDefault()
        {
            DrawScene(FollowDriver, ShowDriverPath);
        }

        internal void DrawSquare(Vector v, double side, Color color)
        {
            DrawRectangle(v.X - side, v.Y - side, v.X + side, v.Y + side, color);
        }

        internal void DrawEquilateralTriangle(Vector center, double side, Color color)
        {
            GL.Color3(color);
            GL.Begin(BeginMode.Triangles);
            DrawEquilateralTriangleFast(center, side);
            GL.End();
        }

        private void DrawEquilateralTriangleFast(Vector center, double side)
        {
            const double factor = 1 / (1.7320508075688772935274463415059 * 2);
            GL.Vertex3(center.X + side/2, center.Y - side*factor, 0);
            GL.Vertex3(center.X, center.Y + side/Math.Sqrt(3), 0);
            GL.Vertex3(center.X - side/2, center.Y - side*factor, 0);
        }

        internal DrawableImage DrawableImageFromName(string name)
        {
            return DrawableImages.FirstOrDefault(x => x.Name == name);
        }

        internal void FocusIndicesChanged()
        {
            if (!WrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                CurrentPlayerAppleEvents = Players[ActivePlayerIndices[0]].GetEvents(ReplayEventType.AppleTake);
                NotTakenApples = new List<Level.Object>();
                for (int i = 0; i < Lev.Apples.Count; i++)
                {
                    if (Lev.Apples[i].Type != Level.ObjectType.Apple)
                        continue;
                    int i1 = i;
                    bool isTaken = CurrentPlayerAppleEvents.Any(x => x.Info == i1);
                    if (!isTaken)
                        NotTakenApples.Add(Lev.Apples[i]);
                }
            }
        }

        internal Vector GetBikeCoordinates()
        {
            return ActivePlayerIndices.Count > 0
                       ? new Vector(Players[ActivePlayerIndices[0]].GlobalBodyX,
                                    Players[ActivePlayerIndices[0]].GlobalBodyY)
                       : new Vector();
        }

        internal double GetSpeed()
        {
            return ActivePlayerIndices.Count > 0 ? Players[ActivePlayerIndices[0]].Speed : 0.0;
        }

        internal void InitializeLevel(Level level)
        {
            Lev = level;
            Lev.DecomposeGroundPolygons();
            Lev.UpdateImages(DrawableImages);
            Lev.UpdateBounds();
            UpdateZoomFillBounds();
            SetZoomFill();
            UpdateGroundAndSky(Settings.DefaultGroundAndSky);
        }

        internal void InitializeReplays(List<Replay> replays)
        {
            InitializeLevel(replays[0].GetLevel());
            ZoomLevel = 5.0;
            MaxTime = 0.0;
            Players = new List<Player>();
            foreach (Replay t in replays)
            {
                Players.Add(t.Player1);
                if (t.Player1.FrameCount > MaxTime)
                    MaxTime = t.Player1.FrameCount;
                if (t.IsMulti)
                {
                    Players.Add(t.Player2);
                    if (t.Player2.FrameCount > MaxTime)
                        MaxTime = t.Player2.FrameCount;
                }
            }
            MaxTime /= 30.0;
            CurrentTime = 0.0;
            ActivePlayerIndices = new List<int>();
            VisiblePlayerIndices = new List<int>();
            int killerObjectCount = Lev.KillerObjectCount;
            foreach (Player x in Players)
            {
                x.InitializeForPlaying(killerObjectCount);
            }
            WrongLevVersion = replays[0].WrongLevelVersion;
        }

        internal void NextFrame(object sender = null, EventArgs e = null)
        {
            if (!Playing)
            {
                CurrentTime += FrameStep;
                if (CurrentTime > MaxTime)
                    CurrentTime = 0;
                DrawSceneDefault();
            }
        }

        internal void PreviousFrame(object sender = null, EventArgs e = null)
        {
            if (!Playing)
            {
                CurrentTime -= FrameStep;
                if (CurrentTime < 0)
                    CurrentTime = MaxTime;
                DrawSceneDefault();
            }
        }

        internal void RedrawScene(object sender = null, EventArgs e = null)
        {
            if (!Playing)
                DrawSceneNoDriverFocus();
        }

        internal void ResetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            AspectRatio = width / (double) height;
            GL.GetInteger(GetPName.Viewport, _viewPort);
        }

        internal void SetFullScreenMode(DisplayResolution newResolution)
        {
            DisplayDevice.Default.ChangeResolution(newResolution);
            GL.Viewport(0, 0, newResolution.Width, newResolution.Height);
            AspectRatio = newResolution.Width / (double) newResolution.Height;
        }

        internal void SetPlayBackSpeed(double newSpeed)
        {
            PlayBackSpeed = newSpeed;
            if (Playing)
            {
                InitialTime = CurrentTime;
                PlayTimer.Restart();
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
            InitialTime = CurrentTime;
            PlayTimer.Restart();
            while (Playing)
            {
                CurrentTime = InitialTime + elapsedTime;
                if (MultiSpy && VisiblePlayerIndices.Count > 0 && (!FollowDriver || ActivePlayerIndices.Count == 0))
                {
                    double xmin = Players[VisiblePlayerIndices[0]].GlobalBodyX;
                    double xmax = Players[VisiblePlayerIndices[0]].GlobalBodyX;
                    double ymin = Players[VisiblePlayerIndices[0]].GlobalBodyY;
                    double ymax = Players[VisiblePlayerIndices[0]].GlobalBodyY;
                    foreach (var i in VisiblePlayerIndices)
                    {
                        xmin = Math.Min(Players[i].GlobalBodyX, xmin);
                        xmax = Math.Max(Players[i].GlobalBodyX, xmax);
                        ymin = Math.Min(Players[i].GlobalBodyY, ymin);
                        ymax = Math.Max(Players[i].GlobalBodyY, ymax);
                    }
                    CenterX = (xmin + xmax) / 2;
                    CenterY = (ymin + ymax) / 2;
                    ZoomLevel = Math.Max((xmax + 5 - CenterX) / AspectRatio, ymax + 5 - CenterY);
                    ZoomLevel = Math.Max(ZoomLevel, 5);
                }
                if (CurrentTime > MaxTime)
                {
                    if (LoopPlaying)
                    {
                        CurrentTime = 0;
                        InitialTime = 0;
                        PlayTimer.Restart();
                    }
                    else
                    {
                        DrawSceneDefault();
                        break;
                    }
                }
                DrawSceneDefault();
                Application.DoEvents();
                Application.RaiseIdle(null);
                elapsedTime = PlayTimer.ElapsedMilliseconds / 1000.0 * PlayBackSpeed;
            }
            Playing = false;
            PlayTimer.Stop();
        }

        internal void UpdateGroundAndSky(bool useDefault)
        {
            Settings.DefaultGroundAndSky = useDefault;
            if (!LgrGraphicsLoaded) return;
            foreach (DrawableImage x in DrawableImages)
            {
                if (useDefault)
                {
                    if (x.Name == "ground")
                        GroundTexture = x;
                    if (x.Name == "sky")
                        SkyTexture = x;
                }
                else
                {
                    if (x.Name == Lev.GroundTextureName)
                        GroundTexture = x;
                    if (x.Name == Lev.SkyTextureName)
                        SkyTexture = x;
                }
            }
            if (GroundTexture == null)
            {
                foreach (DrawableImage x in DrawableImages)
                {
                    if (x.Type == Lgr.ImageType.Texture && !x.Equals(SkyTexture))
                    {
                        GroundTexture = x;
                        break;
                    }
                }
            }
            if (SkyTexture == null)
            {
                foreach (DrawableImage x in DrawableImages)
                {
                    if (x.Type == Lgr.ImageType.Texture && !x.Equals(GroundTexture))
                    {
                        SkyTexture = x;
                        break;
                    }
                }
            }
        }

        internal void UpdateReplaySettings()
        {
            ShowDriverPath = Global.AppSettings.ReplayViewer.ShowDriverPath;
            FollowDriver = Global.AppSettings.ReplayViewer.FollowDriver;
            LockedCamera = Global.AppSettings.ReplayViewer.LockedCamera;
            PicturesInBackground = Global.AppSettings.ReplayViewer.PicturesInBackGround;
            DrawInActiveAsTransparent = Global.AppSettings.ReplayViewer.DrawTransparentInactive;
            FrameStep = Global.AppSettings.ReplayViewer.FrameStep;
            LoopPlaying = Global.AppSettings.ReplayViewer.LoopPlaying;
            ActivePlayerColor = Global.AppSettings.ReplayViewer.ActivePlayerColor;
            InActivePlayerColor = Global.AppSettings.ReplayViewer.InactivePlayerColor;
            DrawOnlyPlayerFrames = Global.AppSettings.ReplayViewer.DrawOnlyPlayerFrames;
            HideStartObject = Global.AppSettings.ReplayViewer.HideStartObject;
            MultiSpy = Global.AppSettings.ReplayViewer.MultiSpy;
        }

        internal void UpdateSettings(RenderingSettings newSettings)
        {
            if (Settings.LgrFile != newSettings.LgrFile)
            {
                if (CurrentLgr != null)
                    CurrentLgr.Dispose();
                LoadLgrGraphics(newSettings.LgrFile);
                Lev.UpdateImages(DrawableImages);
                UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
            }
            if (Settings.DefaultGroundAndSky != newSettings.DefaultGroundAndSky)
                UpdateGroundAndSky(newSettings.DefaultGroundAndSky);
            if (Settings.SkyFillColor != newSettings.SkyFillColor)
                GL.ClearColor(newSettings.SkyFillColor);
            if (Settings.LineWidth != newSettings.LineWidth)
                GL.LineWidth(newSettings.LineWidth);
            Settings = newSettings.Clone();
        }

        internal void UpdateZoomFillBounds()
        {
            ZoomFillxMin = (1 + ZoomFillMargin) * Lev.XMin - ZoomFillMargin * Lev.XMax;
            ZoomFillxMax = (1 + ZoomFillMargin) * Lev.XMax - ZoomFillMargin * Lev.XMin;
            ZoomFillyMin = (1 + ZoomFillMargin) * Lev.YMin - ZoomFillMargin * Lev.YMax;
            ZoomFillyMax = (1 + ZoomFillMargin) * Lev.YMax - ZoomFillMargin * Lev.YMin;
            double tempVar = ZoomFillyMin;
            ZoomFillyMin = -ZoomFillyMax;
            ZoomFillyMax = -tempVar;
            MidX = (ZoomFillxMax + ZoomFillxMin) / 2;
            MidY = (ZoomFillyMax + ZoomFillyMin) / 2;
        }

        internal void Zoom(Vector p, bool zoomIn, double zoomFactor)
        {
            double i = zoomIn ? zoomFactor : 1 / zoomFactor;
            double x = p.X;
            double y = p.Y;
            x -= (x - (XMax + XMin) / 2) * i;
            y -= (y - (YMax + YMin) / 2) * i;
            PerformZoom(ZoomLevel * i, x, y);
        }

        internal void ZoomFill(object sender = null, EventArgs e = null)
        {
            double levelAspectRatio = (ZoomFillxMax - ZoomFillxMin) / (ZoomFillyMax - ZoomFillyMin);
            double newZoomLevel = (ZoomFillyMax - ZoomFillyMin) / 2;
            if (levelAspectRatio > AspectRatio)
                newZoomLevel = (ZoomFillxMax - ZoomFillxMin) / 2 / AspectRatio;
            PerformZoom(newZoomLevel, (ZoomFillxMax + ZoomFillxMin) / 2, (ZoomFillyMax + ZoomFillyMin) / 2);
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
                double i = (y2 - y1) / 2;
                double rectAspectRatio = (x2 - x1) / (y2 - y1);
                if (rectAspectRatio > AspectRatio)
                    i = (x2 - x1) / 2 / AspectRatio;
                PerformZoom(i, (x2 + x1) / 2, (y2 + y1) / 2);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!(Disposed))
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false,
                // only the following code is executed.
                GFXContext.Dispose();
                CtrlWindowInfo.Dispose();
            }
            Disposed = true;
        }

        private static void CalculateMiddle(double startx, double starty, double endx, double endy, double minWidth,
                                            bool mirror, out double midx, out double midy)
        {
            double distanceToFoot = Math.Sqrt((startx - endx) * (startx - endx) + (starty - endy) * (starty - endy));
            if (minWidth * 2 > distanceToFoot)
            {
                double d =
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
            foreach (Vector[] triangle in triangles)
                foreach (Vector x in triangle)
                    GL.Vertex3(x.X, x.Y, depth);
        }

        private static void DrawObject(int picture, double x, double y, double depth = 0.5 * (zFar - zNear) + zNear)
        {
            x -= ObjectRadius;
            y -= ObjectRadius;
            GL.BindTexture(TextureTarget.Texture2D, picture);
            GL.Begin(BeginMode.Quads);
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

        private static void DrawObject(int picture, Vector v, double depth = 0.5 * (zFar - zNear) + zNear)
        {
            DrawObject(picture, v.X, v.Y, depth);
        }

        private static void DrawPicture(int picture, double x, double y, double width, double height, double depth)
        {
            GL.BindTexture(TextureTarget.Texture2D, picture);
            GL.Begin(BeginMode.Quads);
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
            Graphics gfx = Graphics.FromImage(newBmp);
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
            int textureIdentifier = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureIdentifier);
            bmp.RotateFlip(flip);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                                              PixelFormat.Format32bppArgb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float) TextureWrapMode.Repeat);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);
            return textureIdentifier;
        }

        private void BaseInit(Control renderingTarget, RenderingSettings settings)
        {
            ActivePlayerColor = Color.Black;
            InActivePlayerColor = Color.Green;
            BikePicTranslateXFacingLeft = BikePicXFacingLeft * Math.Cos(BikePicRotationConst * Constants.DegToRad) +
                                          BikePicYFacingLeft * Math.Sin(BikePicRotationConst * Constants.DegToRad);
            BikePicTranslateYFacingLeft = BikePicXFacingLeft * Math.Sin(BikePicRotationConst * Constants.DegToRad) +
                                          BikePicYFacingLeft * Math.Cos(BikePicRotationConst * Constants.DegToRad);
            BikePicTranslateXFacingRight = BikePicXFacingRight * Math.Cos(-BikePicRotationConst * Constants.DegToRad) +
                                           BikePicYFacingRight * Math.Sin(-BikePicRotationConst * Constants.DegToRad);
            BikePicTranslateYFacingRight = BikePicXFacingRight * Math.Sin(-BikePicRotationConst * Constants.DegToRad) +
                                           BikePicYFacingRight * Math.Cos(-BikePicRotationConst * Constants.DegToRad);
            Settings = settings.Clone();
            AspectRatio = renderingTarget.Width / (double) renderingTarget.Height;
            CtrlWindowInfo = Utilities.CreateWindowsWindowInfo(renderingTarget.Handle);
            InitializeOpengl();
            DrawableImages = new List<DrawableImage>();
            if (File.Exists(Settings.LgrFile))
                LoadLgrGraphics(Settings.LgrFile);
            ActivePlayerIndices = new List<int>();
            VisiblePlayerIndices = new List<int>();
        }

        private void DeleteTextures()
        {
            if (OpenGLInitialized)
            {
                GL.DeleteTexture(WheelPic);
                foreach (var apple in ApplePics.Values)
                {
                    GL.DeleteTexture(apple);
                }
                ApplePics.Clear();
                GL.DeleteTexture(HeadPic);
                GL.DeleteTexture(KillerPic);
                GL.DeleteTexture(FlowerPic);
                GL.DeleteTexture(BikePic);
                GL.DeleteTexture(BodyPic);
                GL.DeleteTexture(ArmPic);
                GL.DeleteTexture(HandPic);
                GL.DeleteTexture(LegPic);
                GL.DeleteTexture(ThighPic);
                foreach (DrawableImage x in DrawableImages)
                    GL.DeleteTexture(x.TextureIdentifier);
                foreach (Suspension x in Suspensions)
                    if (x != null)
                        GL.DeleteTexture(x.TextureIdentifier);
            }
            GroundTexture = null;
            SkyTexture = null;
        }

        private void DrawGrid()
        {
            double current = (Math.Floor(XMin / Settings.GridSize) + 1) * Settings.GridSize - (Settings.GridSize - 0);
            GL.Enable(EnableCap.LineStipple);
            GL.LineWidth(1);
            GL.LineStipple(1, unchecked((short) (0xAAAA)));
            GL.Scale(1.0, -1.0, 1.0);
            while (!(current > XMax))
            {
                DrawLine(current, YMin, current, YMax, Settings.GridColor);
                current += Settings.GridSize;
            }
            current = (Math.Floor(YMin / Settings.GridSize) + 1) * Settings.GridSize - (Settings.GridSize - 0);
            while (!(current > YMax))
            {
                DrawLine(XMin, current, XMax, current, Settings.GridColor);
                current += Settings.GridSize;
            }
            GL.Scale(1.0, -1.0, 1.0);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(Settings.LineWidth);
        }

        private void DrawObjectCenters()
        {
            foreach (Level.Object x in Lev.Objects)
            {
                switch (x.Type)
                {
                    case Level.ObjectType.Flower:
                        DrawPoint(x.Position, Settings.FlowerColor);
                        break;
                    case Level.ObjectType.Killer:
                        DrawPoint(x.Position, Settings.KillerColor);
                        break;
                    case Level.ObjectType.Apple:
                        if (WrongLevVersion || ActivePlayerIndices.Count == 0)
                            DrawPoint(x.Position, Settings.AppleColor);
                        break;
                    case Level.ObjectType.Start:
                        if (!HideStartObject)
                        {
                            DrawPoint(x.Position, Settings.StartColor);
                            DrawPoint(x.Position.X + Level.RightWheelDifferenceFromLeftWheelX, x.Position.Y,
                                      Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                            DrawPoint(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                                      x.Position.Y + Level.HeadDifferenceFromLeftWheelY,
                                      Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                        }
                        break;
                }
            }
            if (!WrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                int i = 0;
                while (!(i >= CurrentPlayerAppleEvents.Count() || CurrentPlayerAppleEvents[i].Time >= CurrentTime))
                    i++;
                for (int j = i; j < CurrentPlayerAppleEvents.Count(); j++)
                    DrawCircle(Lev.Apples[CurrentPlayerAppleEvents[j].Info].Position, ObjectRadius,
                               Settings.AppleColor);
                foreach (Level.Object x in NotTakenApples)
                    DrawCircle(x.Position, ObjectRadius, Settings.AppleColor);
            }
        }

        private void DrawObjectFrames()
        {
            foreach (Level.Object x in Lev.Objects)
            {
                switch (x.Type)
                {
                    case Level.ObjectType.Flower:
                        DrawCircle(x.Position, ObjectRadius, Settings.FlowerColor);
                        break;
                    case Level.ObjectType.Killer:
                        DrawCircle(x.Position, ObjectRadius, Settings.KillerColor);
                        break;
                    case Level.ObjectType.Apple:
                        if (WrongLevVersion || ActivePlayerIndices.Count == 0)
                            DrawCircle(x.Position, ObjectRadius, Settings.AppleColor);
                        break;
                    case Level.ObjectType.Start:
                        if (!HideStartObject)
                        {
                            DrawPlayerFrames(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                                             x.Position.Y + Level.HeadDifferenceFromLeftWheelY, 0, false, x.Position.X,
                                             x.Position.Y, x.Position.X + Level.RightWheelDifferenceFromLeftWheelX,
                                             x.Position.Y, 0, 0, Settings.StartColor);
                        }
                        break;
                }
            }
            if (!WrongLevVersion && ActivePlayerIndices.Count > 0)
            {
                int i = 0;
                while (!(i >= CurrentPlayerAppleEvents.Count() || CurrentPlayerAppleEvents[i].Time >= CurrentTime))
                    i++;
                for (int j = i; j < CurrentPlayerAppleEvents.Count(); j++)
                    DrawCircle(Lev.Apples[CurrentPlayerAppleEvents[j].Info].Position, ObjectRadius,
                               Settings.AppleColor);
                foreach (Level.Object x in NotTakenApples)
                    DrawCircle(x.Position, ObjectRadius, Settings.AppleColor);
            }
        }

        private void DrawPlayer(Player player, bool isActive = true)
        {
            DrawPlayer(player.GlobalBodyX, player.GlobalBodyY, player.LeftWheelX, player.LeftWheelY, player.RightWheelX,
                       player.RightWheelY, player.LeftWheelRotation, player.RightWheelRotation, player.HeadX,
                       player.HeadY, player.BikeRotation, player.Dir, player.ArmRotation, isActive);
        }

        private void DrawPlayer(double globalBodyX, double globalBodyY, double leftWheelx, double leftWheely,
                                double rightWheelx, double rightWheely, double leftWheelRotation,
                                double rightWheelRotation, double headX, double headY, double bikeRotation,
                                Direction direction, double armRotation, bool isActive)
        {
            double distance = ((PicturesInBackground ? 1 : BikeDistance) - Utils.BooleanToInteger(isActive)) /
                              1000.0 * (zFar - zNear) + zNear;
            bool isright = direction == Direction.Right;
            if (!DrawOnlyPlayerFrames && LgrGraphicsLoaded)
            {
                double rotation = bikeRotation * Constants.DegToRad;
                double rotationCos = Math.Cos(rotation);
                double rotationSin = Math.Sin(rotation);

                if (!isActive && DrawInActiveAsTransparent)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusDstColor);
                }

                //Wheels
                DrawWheel(leftWheelx, leftWheely, leftWheelRotation, distance);
                DrawWheel(rightWheelx, rightWheely, rightWheelRotation, distance);

                //Suspensions
                int x = Utils.BooleanToInteger(isright);
                for (int i = 0; i < 2; i++)
                {
                    GL.PushMatrix();
                    if (x == 0)
                        x = -1;
                    double yPos = globalBodyY + Suspensions[i].Y * rotationCos - Suspensions[i].X * x * rotationSin;
                    double xPos = globalBodyX - Suspensions[i].X * x * rotationCos - Suspensions[i].Y * rotationSin;
                    if (x == -1)
                        x = Suspensions[i].WheelNumber;
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
                    double xDiff = xPos - wheelXpos;
                    double yDiff = yPos - wheelYpos;
                    double angle = Math.Atan2(yDiff, xDiff) * Constants.RadToDeg;
                    double length = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
                    GL.Translate(wheelXpos, wheelYpos, 0);
                    GL.Rotate(angle, 0, 0, 1);
                    GL.BindTexture(TextureTarget.Texture2D, Suspensions[i].TextureIdentifier);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(-Suspensions[i].OffsetX, -Suspensions[i].Height / 2, distance);
                    GL.TexCoord2(1, 0);
                    GL.Vertex3(length + Suspensions[i].Height / 2, -Suspensions[i].Height / 2, distance);
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(length + Suspensions[i].Height / 2, Suspensions[i].Height / 2, distance);
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(-Suspensions[i].OffsetX, Suspensions[i].Height / 2, distance);
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
                DrawPicture(HeadPic, headX - Constants.HeadDiameter / 2.0, headY - Constants.HeadDiameter / 2.0,
                            Constants.HeadDiameter, Constants.HeadDiameter, distance);
                GL.PopMatrix();

                //Bike
                GL.PushMatrix();
                double bikePicTranslateX;
                double bikePicTranslateY;
                GL.Translate(globalBodyX, globalBodyY, 0);
                if (!isright)
                {
                    bikePicTranslateX = BikePicTranslateXFacingLeft;
                    bikePicTranslateY = BikePicTranslateYFacingLeft;
                    GL.Rotate(bikeRotation + BikePicRotationConst + 180, 0, 0, 1);
                    GL.Scale(-1.0, 1.0, 1.0);
                }
                else
                {
                    GL.Rotate(bikeRotation + 180 - BikePicRotationConst, 0, 0, 1);
                    bikePicTranslateX = BikePicTranslateXFacingRight;
                    bikePicTranslateY = BikePicTranslateYFacingRight;
                }
                DrawPicture(BikePic, -BikePicAspectRatio * BikePicSize / 2 + bikePicTranslateX,
                            -BikePicSize / 2 + bikePicTranslateY, BikePicSize * BikePicAspectRatio, BikePicSize,
                            distance);
                GL.PopMatrix();

                //Thigh
                const double legMinimumWidth = 0.55;
                const double footsx = 0;
                const double footsy = -0.45;
                const double thighHeight = 0.3;
                double thighsx = 0.45;
                if (isright)
                {
                    thighsx *= -1;
                }
                const double thighsy = -0.55;
                double footx = globalBodyX + footsx * rotationCos - footsy * rotationSin;
                double footy = globalBodyY + footsx * rotationSin + footsy * rotationCos;
                double thighstartx = headX + thighsx * rotationCos - thighsy * rotationSin;
                double thighstarty = headY + thighsx * rotationSin + thighsy * rotationCos;
                double thighendx;
                double thighendy;
                CalculateMiddle(thighstartx, thighstarty, footx, footy, legMinimumWidth, isright, out thighendx,
                                out thighendy);
                DrawPicture(ThighPic, thighstartx, thighstarty, thighendx, thighendy, thighHeight, distance, isright,
                            0.05);

                //Leg
                const double legHeight = 0.4;
                DrawPicture(LegPic, footx, footy, thighendx, thighendy, legHeight, distance, isright, 0.05);

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
                DrawPicture(BodyPic, offsetx, offsety, BodyWidth, BodyHeight, distance);
                GL.PopMatrix();

                //Upper arm
                const double armMinimumWidth = 0.4;
                double handsx;
                double handsy = 0.4;
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
                double armsy = -0.2;
                double armx = headX + armsx * rotationCos - armsy * rotationSin;
                double army = headY + armsx * rotationSin + armsy * rotationCos;
                double initialx = globalBodyX + handsx * rotationCos - handsy * rotationSin;
                double initialy = globalBodyY + handsx * rotationSin + handsy * rotationCos;
                double dist = Math.Sqrt((initialx - armx) * (initialx - armx) + (initialy - army) * (initialy - army));
                double armAngle;
                if (isright)
                {
                    armAngle = Math.Atan2(initialy - army, initialx - armx) + armRotation * Constants.DegToRad;
                }
                else
                {
                    armAngle = Math.Atan2(initialy - army, initialx - armx) + armRotation * Constants.DegToRad;
                }
                double angleCos = Math.Cos(armAngle);
                double angleSin = Math.Sin(armAngle);
                double handx = armx + dist * angleCos;
                double handy = army + dist * angleSin;
                double armendx;
                double armendy;
                CalculateMiddle(armx, army, handx, handy, armMinimumWidth, !isright, out armendx, out armendy);
                DrawPicture(ArmPic, armx, army, armendx, armendy, upArmHeight, distance, !isright, 0.05);

                //Lower arm
                const double lowArmHeight = 0.15;
                DrawPicture(HandPic, armendx, armendy, handx, handy, lowArmHeight, distance, isright, 0.05);

                if (!isActive)
                    GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.DepthTest);
                DrawPlayerFrames(headX, headY, bikeRotation, isright, leftWheelx, leftWheely, rightWheelx, rightWheely,
                                 leftWheelRotation, rightWheelRotation,
                                 isActive ? ActivePlayerColor : InActivePlayerColor);
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.DepthTest);
            }
        }

        private void DrawPlayerFrames(double headX, double headY, double bikeRotation, bool isright, double leftWheelx,
                                      double leftWheely, double rightWheelx, double rightWheely,
                                      double leftWheelRotation, double rightWheelRotation, Color playerColor)
        {
            double headCos = Math.Cos(bikeRotation * Constants.DegToRad);
            double headSin = Math.Sin(bikeRotation * Constants.DegToRad);
            int f = isright ? 1 : -1;
            double headLineEndPointX = headX + f * headCos * Constants.HeadDiameter / 2;
            double headLineEndPointY = headY + f * headSin * Constants.HeadDiameter / 2;
            DrawCircle(leftWheelx, leftWheely, ObjectRadius, playerColor);
            DrawCircle(rightWheelx, rightWheely, ObjectRadius, playerColor);
            GL.Begin(BeginMode.Lines);
            for (int k = 0; k < 2; k++)
            {
                for (int j = 0; j < 4; j++)
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
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(headX, headY);
            GL.Vertex2(headLineEndPointX, headLineEndPointY);
            GL.End();
        }

        private void DrawPlayers(IList focusIndices, IList visibleIndices)
        {
            foreach (int x in visibleIndices)
            {
                bool isSelected = focusIndices.Contains(x);
                DrawPlayer(Players[x], isSelected);
            }
        }

        private void DrawSceneNoDriverFocus()
        {
            DrawScene(false, ShowDriverPath);
        }

        private void DrawWheel(double x, double y, double rot, double distance)
        {
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(rot * 180 / Math.PI, 0, 0, 1);
            DrawPicture(WheelPic, -ObjectRadius, -ObjectRadius, ObjectDiameter, ObjectDiameter, distance);
            GL.PopMatrix();
        }

        private void InitializeOpengl()
        {
            GFXContext = new GraphicsContext(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 8, 8), CtrlWindowInfo);
            GFXContext.MakeCurrent(CtrlWindowInfo);
            GFXContext.LoadAll();
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
            GL.ClearColor(Settings.SkyFillColor);
            GL.LineWidth(Settings.LineWidth);
            OpenGLInitialized = true;
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
            foreach (Lgr.LgrImage x in CurrentLgr.LgrImages)
            {
                switch (x.Name)
                {
                    case "q1wheel":
                        WheelPic = LoadTexture(x);
                        break;
                    case "q1head":
                        HeadPic = LoadTexture(x);
                        break;
                    case "q1bike":
                        BikePic = LoadTexture(x);
                        break;
                    case "q1body":
                        BodyPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                        break;
                    case "q1thigh":
                        ThighPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                        break;
                    case "q1leg":
                        LegPic = LoadTexture(x, RotateFlipType.RotateNoneFlipY);
                        break;
                    case "q1forarm":
                        HandPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                        break;
                    case "q1up_arm":
                        ArmPic = LoadTexture(x, RotateFlipType.RotateNoneFlipX);
                        break;
                    case "qexit":
                        FlowerPic = LoadTexture(x, firstFrameRect);
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
                        int animNum = int.Parse(x.Name[5].ToString());
                        ApplePics[animNum] = LoadTexture(x, firstFrameRect);
                        break;
                    case "qkiller":
                        KillerPic = LoadTexture(x, firstFrameRect);
                        break;
                    case "q1susp1":
                        Suspensions[0] = new Suspension(LoadTexture(x, RotateFlipType.RotateNoneFlipY), -0.5, 0.35,
                                                        x.Bmp.Height * Suspension1Factor,
                                                        x.Bmp.Height * Suspension1Factor / 2.0, 0);
                        break;
                    case "q1susp2":
                        Suspensions[1] = new Suspension(LoadTexture(x, RotateFlipType.Rotate180FlipY), 0.0, -0.4,
                                                        x.Bmp.Height * Suspension2Factor,
                                                        x.Bmp.Height * Suspension2Factor / 1.3, 1);
                        break;
                }
                if (!x.Name.StartsWith("q"))
                {
                    DrawableImages.Add(new DrawableImage(LoadTexture(x), x.Bmp.Width * PictureFactor,
                                                         x.Bmp.Height * PictureFactor, x.ClippingType, x.Distance,
                                                         x.Name, x.Type));
                }
            }
            LGRGraphicsLoaded = true;
        }

        private void PerformZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (Settings.SmoothZoomEnabled)
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

        private void SetZoomFill()
        {
            double levelAspectRatio = (ZoomFillxMax - ZoomFillxMin) / (ZoomFillyMax - ZoomFillyMin);
            ZoomLevel = (ZoomFillyMax - ZoomFillyMin) / 2.0;
            if (levelAspectRatio > AspectRatio)
                ZoomLevel = (ZoomFillxMax - ZoomFillxMin) / 2.0 / AspectRatio;
            CenterX = (ZoomFillxMax + ZoomFillxMin) / 2.0;
            CenterY = (ZoomFillyMax + ZoomFillyMin) / 2.0;
        }

        private void SmoothZoom(double newZoomLevel, double newCenterX, double newCenterY)
        {
            if (_smoothZoomInProgress)
                return;
            _smoothZoomInProgress = true;
            double oldZoomLevel = ZoomLevel;
            double oldCenterX = (XMax + XMin) / 2;
            double oldCenterY = (YMax + YMin) / 2;
            var zoomTimer = new Stopwatch();
            long elapsedTime = 0;
            double initialTime = CurrentTime;
            zoomTimer.Start();
            while (elapsedTime <= Settings.SmoothZoomDuration)
            {
                ZoomLevel = oldZoomLevel + (newZoomLevel - oldZoomLevel) * elapsedTime / Settings.SmoothZoomDuration;
                CenterX = oldCenterX + (newCenterX - oldCenterX) * elapsedTime / Settings.SmoothZoomDuration;
                CenterY = oldCenterY + (newCenterY - oldCenterY) * elapsedTime / Settings.SmoothZoomDuration;
                if (Playing)
                {
                    CurrentTime = initialTime + elapsedTime / 1000.0 * PlayBackSpeed;
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
            internal readonly Level.ClippingType DefaultClipping;
            internal readonly int DefaultDistance;
            internal readonly double Height;
            internal readonly string Name;
            internal readonly int TextureIdentifier;
            internal readonly Lgr.ImageType Type;
            internal readonly double Width;

            internal DrawableImage(int textureId, double width, double height, Level.ClippingType clipping, int distance,
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
            internal double AspectRatio
            {
                get { return Width / Height; }
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

        public void DrawDashLine(double x1, double y1, double x2, double y2, Color color)
        {
            GL.Enable(EnableCap.LineStipple);
            GL.LineWidth(1);
            GL.LineStipple(1, unchecked((short)(0xAAAA)));
            DrawLine(x1, y1, x2, y2, color);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(Settings.LineWidth);
        }
    }
}