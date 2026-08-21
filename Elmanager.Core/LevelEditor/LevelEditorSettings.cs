using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Elmanager.Lev;
using Elmanager.LevelEditor.Playing;
using Elmanager.Settings;
using Elmanager.Utilities.Json;

namespace Elmanager.LevelEditor;

public enum LevSaveState
{
    Saved,
    Unsaved,
    New
}

public partial class LevelEditorSettings
{
    private const string SettingsFileName = "sle_settings.json";
    private const string SleFolderBrowser = "/sle";

    public const double MinToolbarIconSize = 16;
    public const double MaxToolbarIconSize = 60;
    public const double ToolbarIconSizeStep = 1;

    private static string SleFolder => OperatingSystem.IsBrowser() ? SleFolderBrowser : AppContext.BaseDirectory;

    [JsonPropertyName("AutoGrassThickness")]
    public double AutoGrassThickness { get; set; } = 0.2;
    [JsonPropertyName("BaseFilename")]
    public string BaseFilename { get; set; } = "MyLev";
    [JsonPropertyName("CaptureRadius")]
    public int CaptureRadius { get; set; } = 10;
    [JsonPropertyName("CheckTopologyDynamically")]
    public bool CheckTopologyDynamically { get; set; }
    [JsonPropertyName("CheckTopologyWhenSaving")]
    public bool CheckTopologyWhenSaving { get; set; }
    [JsonPropertyName("DefaultTitle")]
    public string DefaultTitle { get; set; } = "New level";
    [JsonPropertyName("DefaultFilename")]
    public string DefaultFilename { get; set; } = "";
    [JsonPropertyName("DrawStep")]
    public double DrawStep { get; set; } = 1.0;
    [JsonPropertyName("EllipseSteps")]
    public int EllipseSteps { get; set; } = 10;
    [JsonPropertyName("FrameRadius")]
    public double FrameRadius { get; set; } = 0.2;
    [JsonPropertyName("LastLevel")]
    public string? LastLevel { get; set; }
    [JsonPropertyName("LevelFolder")]
    public Bookmark? LevelFolder { get; set; }
    [JsonPropertyName("SavedFile")]
    public Bookmark? SavedFile { get; set; }
    [JsonPropertyName("LgrFolder")]
    public Bookmark? LgrFolder { get; set; }
    [JsonPropertyName("DroppedLgrs")]
    public List<Bookmark> DroppedLgrs { get; set; } = [];
    [JsonPropertyName("NumberFormat")]
    public string NumberFormat { get; set; } = "0";
    [JsonPropertyName("PipeRadius")]
    public double PipeRadius { get; set; } = 1.0;
    [JsonPropertyName("RenderingSettings")]
    public LevelEditorRenderingSettings RenderingSettings { get; set; } = new();
    [JsonPropertyName("ShapeFolder")]
    public Bookmark? ShapeFolder { get; set; }
    [JsonPropertyName("Size")]
    public Size Size { get; set; } = new(800, 600);
    [JsonPropertyName("SnapToGrid")]
    public bool SnapToGrid { get; set; }

    [JsonPropertyName("ToolbarIconSize")]
    public double ToolbarIconSize
    {
        get;
        set => field = Math.Clamp(value, MinToolbarIconSize, MaxToolbarIconSize);
    } = 28;

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
    [JsonPropertyName("WindowState")]
    public WindowState WindowState { get; set; } = WindowState.Maximized;
    [JsonPropertyName("LevelTemplate")]
    public Bookmark? LevelTemplate { get; set; }
    [JsonPropertyName("CapturePicturesAndTexturesFromBordersOnly")]
    public bool CapturePicturesAndTexturesFromBordersOnly { get; set; }
    [JsonPropertyName("AlwaysSetDefaultsInPictureTool")]
    public bool AlwaysSetDefaultsInPictureTool { get; set; }
    [JsonPropertyName("PlayingSettings")]
    public PlaySettings PlayingSettings { get; set; } = new();
    [JsonPropertyName("EnableStartPositionFeature")]
    public bool EnableStartPositionFeature { get; set; } = true;
    [JsonPropertyName("SaveState")]
    public LevSaveState SaveState { get; set; }
    [JsonPropertyName("NonChromiumWarningShown")]
    public bool NonChromiumWarningShown { get; set; }

    public static LevelEditorSettings Load()
    {
        var path = Path.Combine(SleFolder, SettingsFileName);
        if (!File.Exists(path))
            return new LevelEditorSettings();

        var json = File.ReadAllText(path);
        try
        {
            return JsonSerializer.Deserialize(json, SourceGenerationContext.GetLevelEditorSettingsTypeInfo()) ??
                   new LevelEditorSettings();
        }
        catch (Exception)
        {
            return new LevelEditorSettings();
        }
    }

    public string ToJson() =>
        JsonSerializer.Serialize(this, SourceGenerationContext.GetLevelEditorSettingsTypeInfo());

    public async Task Save()
    {
        Directory.CreateDirectory(SleFolder);
        var path = Path.Combine(SleFolder, SettingsFileName);
        await using (var stream = new FileStream(path, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(
                stream,
                this,
                SourceGenerationContext.GetLevelEditorSettingsTypeInfo());
        }

        if (OperatingSystem.IsBrowser())
            await SyncToIndexedDb();
    }

    [JSImport("syncToIndexedDb", "filesystem.js")]
    private static partial Task SyncToIndexedDb();

    public static Level TryGetTemplateLevel(string? text)
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

    public Level GetTemplateLevel()
    {
        try
        {
            return TryGetTemplateLevel(LevelTemplate?.Id);
        }
        catch (SettingsException)
        {
            return Level.FromDimensions(50, 50);
        }
    }
}
