using System;
using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Elmanager.Lev;
using Elmanager.LevelEditor.Playing;
using Elmanager.Rendering;
using Elmanager.Settings;

namespace Elmanager.LevelEditor;

internal class LevelEditorSettings
{
    [JsonPropertyName("AutoGrassThickness")]
    public double AutoGrassThickness { get; set; } = 0.2;
    [JsonPropertyName("BaseFilename")]
    public string BaseFilename { get; set; } = "MyLev";
    [JsonPropertyName("CaptureRadius")]
    public double CaptureRadius { get; set; } = 0.015;
    [JsonPropertyName("CheckTopologyDynamically")]
    public bool CheckTopologyDynamically { get; set; }
    [JsonPropertyName("CheckTopologyWhenSaving")]
    public bool CheckTopologyWhenSaving { get; set; }
    [JsonPropertyName("DefaultTitle")]
    public string DefaultTitle { get; set; } = "New level";
    [JsonPropertyName("DrawStep")]
    public double DrawStep { get; set; } = 1.0;
    [JsonPropertyName("EllipseSteps")]
    public int EllipseSteps { get; set; } = 10;
    [JsonPropertyName("FrameRadius")]
    public double FrameRadius { get; set; } = 0.2;
    [JsonPropertyName("CrosshairColor")]
    public Color CrosshairColor { get; set; } = Color.Blue;
    [JsonPropertyName("HighlightColor")]
    public Color HighlightColor { get; set; } = Color.Yellow;
    [JsonPropertyName("LastLevel")]
    public string? LastLevel { get; set; }
    [JsonPropertyName("MouseClickStep")]
    public int MouseClickStep { get; set; } = 50;
    [JsonPropertyName("NumberFormat")]
    public string NumberFormat { get; set; } = "0";
    [JsonPropertyName("PipeRadius")]
    public double PipeRadius { get; set; } = 1.0;
    [JsonPropertyName("RenderingSettings")]
    public RenderingSettings RenderingSettings { get; set; } = new();
    [JsonPropertyName("SelectionColor")]
    public Color SelectionColor { get; set; } = Color.Blue;
    [JsonPropertyName("Size")]
    public Size Size { get; set; } = new(800, 600);
    [JsonPropertyName("SnapToGrid")]
    public bool SnapToGrid { get; set; }
    [JsonPropertyName("LockGrid")]
    public bool LockGrid { get; set; }
    [JsonPropertyName("ShowCrossHair")]
    public bool ShowCrossHair { get; set; }
    [JsonPropertyName("SmoothSteps")]
    public int SmoothSteps { get; set; } = 3;
    [JsonPropertyName("SmoothVertexOffset")]
    public int SmoothVertexOffset { get; set; } = 50;
    [JsonPropertyName("UnsmoothAngle")]
    public double UnsmoothAngle { get; set; } = 10;
    [JsonPropertyName("UnsmoothLength")]
    public double UnsmoothLength { get; set; } = 1.0;
    [JsonPropertyName("UseFilenameForTitle")]
    public bool UseFilenameForTitle { get; set; }
    [JsonPropertyName("UseFilenameSuggestion")]
    public bool UseFilenameSuggestion { get; set; }
    [JsonPropertyName("UseHighlight")]
    public bool UseHighlight { get; set; } = true;
    [JsonPropertyName("WindowState")]
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    [JsonPropertyName("LevelTemplate")]
    public string LevelTemplate { get; set; } = "50,50";
    [JsonPropertyName("CapturePicturesAndTexturesFromBordersOnly")]
    public bool CapturePicturesAndTexturesFromBordersOnly { get; set; }
    [JsonPropertyName("AlwaysSetDefaultsInPictureTool")]
    public bool AlwaysSetDefaultsInPictureTool { get; set; }
    [JsonPropertyName("PlayingSettings")]
    public PlaySettings PlayingSettings { get; set; } = new();
    [JsonPropertyName("EnableStartPositionFeature")]
    public bool EnableStartPositionFeature { get; set; } = true;

    internal static Level TryGetTemplateLevel(string text)
    {
        if (text == null)
        {
            throw new SettingsException("The level template is null.");
        }

        if (File.Exists(text))
        {
            try
            {
                var template = Level.FromPath(text).Obj;
                return template;
            }
            catch (Exception)
            {
                throw new SettingsException("The level template file is not a valid Elma level file.");
            }
        }

        var regex = new Regex(@"^(\d+),(\d+)$");
        if (!regex.IsMatch(text))
        {
            throw new SettingsException(
                "The level template is neither a file nor a string of the form \"width,height\".");
        }

        double width = int.Parse(regex.Match(text).Groups[1].Value);
        double height = int.Parse(regex.Match(text).Groups[2].Value);
        return Level.FromDimensions(width, height);
    }

    internal Level GetTemplateLevel()
    {
        try
        {
            return TryGetTemplateLevel(LevelTemplate);
        }
        catch (SettingsException)
        {
            return Level.FromDimensions(50, 50);
        }
    }
}