using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor;

internal class LevelEditorRenderingSettings : RenderingSettings
{
    [Category("Colors"), DisplayName("Crosshair"), JsonPropertyName("CrosshairColor"), Description(TransparencyTip)]
    public Color CrosshairColor { get; set; } = Color.Blue;
    [Category("Colors"), DisplayName("Highlight"), JsonPropertyName("HighlightColor"), Description(TransparencyTip)]
    public Color HighlightColor { get; set; } = Color.Yellow;
    [Category("Colors"), DisplayName("Selection"), JsonPropertyName("SelectionColor"), Description(TransparencyTip)]
    public Color SelectionColor { get; set; } = Color.Blue;

    public LevelEditorRenderingSettings()
    {

    }

    private LevelEditorRenderingSettings(LevelEditorRenderingSettings s) : base(s)
    {
        CrosshairColor = s.CrosshairColor;
        HighlightColor = s.HighlightColor;
        SelectionColor = s.SelectionColor;
    }

    internal override LevelEditorRenderingSettings Clone() => new(this);
}