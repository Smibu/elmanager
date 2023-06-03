using System.Drawing;
using System.Windows.Forms;
using Elmanager.Rendering;

namespace Elmanager.ReplayViewer;

internal class ReplayViewerSettings
{
    public Color ActivePlayerColor { get; set; } = Color.Black;
    public bool DontSelectPlayersByDefault { get; set; }
    public bool DrawOnlyPlayerFrames { get; set; } = true;
    public bool DrawTransparentInactive { get; set; } = true;
    public bool FollowDriver { get; set; } = true;
    public double FrameStep { get; set; } = 0.02;
    public bool HideStartObject { get; set; } = true;
    public Color InactivePlayerColor { get; set; } = Color.Green;
    public bool LockedCamera { get; set; }
    public bool LoopPlaying { get; set; }
    public int MouseClickStep { get; set; } = 50;
    public int MouseWheelStep { get; set; } = 20;
    public bool MultiSpy { get; set; }
    public bool PicturesInBackGround { get; set; }
    public RenderingSettings RenderingSettings { get; set; } = new();
    public bool ShowBikeCoords { get; set; } = true;
    public bool ShowDriverPath { get; set; }
    public Size Size { get; set; } = new(800, 600);
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    public double ZoomLevel { get; set; } = 5.0;
    public bool FollowAlsoWhenZooming { get; set; }
}