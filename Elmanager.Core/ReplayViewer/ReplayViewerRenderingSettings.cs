using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using Elmanager.Rendering;

namespace Elmanager.ReplayViewer;

public class ReplayViewerRenderingSettings : RenderingSettings
{
    [Category("Colors"), DisplayName("Active player"), JsonPropertyName("ActivePlayerColor"), Description(TransparencyTip)]
    public Color ActivePlayerColor { get; set; } = Color.Black;
    [Category("Colors"), DisplayName("Inactive player"), JsonPropertyName("InactivePlayerColor"), Description(TransparencyTip)]
    public Color InactivePlayerColor { get; set; } = Color.Green;

    public ReplayViewerRenderingSettings()
    {

    }

    private ReplayViewerRenderingSettings(ReplayViewerRenderingSettings s) : base(s)
    {
        ActivePlayerColor = s.ActivePlayerColor;
        InactivePlayerColor = s.InactivePlayerColor;
    }

    public override ReplayViewerRenderingSettings Clone() => new(this);
}
