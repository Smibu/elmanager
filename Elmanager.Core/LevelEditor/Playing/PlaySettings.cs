using System.Text.Json.Serialization;
using Elmanager.LevelEditor.Input;

namespace Elmanager.LevelEditor.Playing;

public class PlaySettings
{
    [JsonPropertyName("Gas")]
    public EditorKey Gas { get; set; } = EditorKey.Up;
    [JsonPropertyName("Brake")]
    public EditorKey Brake { get; set; } = EditorKey.Down;
    [JsonPropertyName("BrakeAlias")]
    public EditorKey BrakeAlias { get; set; } = EditorKey.X;
    [JsonPropertyName("LeftVolt")]
    public EditorKey LeftVolt { get; set; } = EditorKey.Left;
    [JsonPropertyName("RightVolt")]
    public EditorKey RightVolt { get; set; } = EditorKey.Right;
    [JsonPropertyName("AloVolt")]
    public EditorKey AloVolt { get; set; } = EditorKey.Insert;
    [JsonPropertyName("Turn")]
    public EditorKey Turn { get; set; } = EditorKey.Space;
    [JsonPropertyName("EscAlias")]
    public EditorKey EscAlias { get; set; } = EditorKey.Escape;
    [JsonPropertyName("Save")]
    public EditorKey Save { get; set; } = EditorKey.LShiftKey;
    [JsonPropertyName("Load")]
    public EditorKey Load { get; set; } = EditorKey.RShiftKey;
    [JsonPropertyName("DyingBehavior")]
    public DyingBehavior DyingBehavior { get; set; } = DyingBehavior.StopPlaying;
    [JsonPropertyName("FollowDriverOption")]
    public FollowDriverOption FollowDriverOption { get; set; } = FollowDriverOption.WhenPressingKey;
    [JsonPropertyName("DisableShortcuts")]
    public bool DisableShortcuts { get; set; }
    [JsonPropertyName("PhysicsFps")]
    public int PhysicsFps { get; set; } = 1000;
    [JsonPropertyName("ConstantFps")]
    public bool ConstantFps { get; set; }
    [JsonPropertyName("ToggleFullscreen")]
    public bool ToggleFullscreen { get; set; }
    [JsonPropertyName("PlayZoomLevel")]
    public double PlayZoomLevel { get; set; } = 5;

    public PlaySettings() { }

    public PlaySettings(PlaySettings other)
    {
        Gas = other.Gas;
        Brake = other.Brake;
        BrakeAlias = other.BrakeAlias;
        LeftVolt = other.LeftVolt;
        RightVolt = other.RightVolt;
        AloVolt = other.AloVolt;
        Turn = other.Turn;
        EscAlias = other.EscAlias;
        Save = other.Save;
        Load = other.Load;
        DyingBehavior = other.DyingBehavior;
        FollowDriverOption = other.FollowDriverOption;
        DisableShortcuts = other.DisableShortcuts;
        PhysicsFps = other.PhysicsFps;
        ConstantFps = other.ConstantFps;
        ToggleFullscreen = other.ToggleFullscreen;
        PlayZoomLevel = other.PlayZoomLevel;
    }
}
