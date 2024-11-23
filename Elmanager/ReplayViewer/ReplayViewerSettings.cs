using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Elmanager.ReplayViewer;

internal class ReplayViewerSettings
{
    [JsonPropertyName("DontSelectPlayersByDefault")]
    public bool DontSelectPlayersByDefault { get; set; }
    [JsonPropertyName("DrawOnlyPlayerFrames")]
    public bool DrawOnlyPlayerFrames { get; set; } = true;
    [JsonPropertyName("DrawTransparentInactive")]
    public bool DrawTransparentInactive { get; set; } = true;
    [JsonPropertyName("FollowDriver")]
    public bool FollowDriver { get; set; } = true;
    [JsonPropertyName("FrameStep")]
    public double FrameStep { get; set; } = 0.02;
    [JsonPropertyName("HideStartObject")]
    public bool HideStartObject { get; set; } = true;
    [JsonPropertyName("LockedCamera")]
    public bool LockedCamera { get; set; }
    [JsonPropertyName("LoopPlaying")]
    public bool LoopPlaying { get; set; }
    [JsonPropertyName("MouseClickStep")]
    public int MouseClickStep { get; set; } = 50;
    [JsonPropertyName("MouseWheelStep")]
    public int MouseWheelStep { get; set; } = 20;
    [JsonPropertyName("MultiSpy")]
    public bool MultiSpy { get; set; }
    [JsonPropertyName("PicturesInBackGround")]
    public bool PicturesInBackGround { get; set; }
    [JsonPropertyName("RenderingSettings")]
    public ReplayViewerRenderingSettings RenderingSettings { get; set; } = new();
    [JsonPropertyName("ShowBikeCoords")]
    public bool ShowBikeCoords { get; set; } = true;
    [JsonPropertyName("ShowDriverPath")]
    public bool ShowDriverPath { get; set; }
    [JsonPropertyName("Size")]
    public Size Size { get; set; } = new(800, 600);
    [JsonPropertyName("WindowState")]
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    [JsonPropertyName("ZoomLevel")]
    public double ZoomLevel { get; set; } = 5.0;
    [JsonPropertyName("FollowAlsoWhenZooming")]
    public bool FollowAlsoWhenZooming { get; set; }
}