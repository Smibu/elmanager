using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Elmanager.LevelEditor.Playing;

internal class PlaySettings
{
    [JsonPropertyName("Gas")]
    public Keys Gas { get; set; } = Keys.Up;
    [JsonPropertyName("Brake")]
    public Keys Brake { get; set; } = Keys.Down;
    [JsonPropertyName("BrakeAlias")]
    public Keys BrakeAlias { get; set; } = Keys.X;
    [JsonPropertyName("LeftVolt")]
    public Keys LeftVolt { get; set; } = Keys.Left;
    [JsonPropertyName("RightVolt")]
    public Keys RightVolt { get; set; } = Keys.Right;
    [JsonPropertyName("AloVolt")]
    public Keys AloVolt { get; set; } = Keys.Insert;
    [JsonPropertyName("Turn")]
    public Keys Turn { get; set; } = Keys.Space;
    [JsonPropertyName("EscAlias")]
    public Keys EscAlias { get; set; } = Keys.Escape;
    [JsonPropertyName("Save")]
    public Keys Save { get; set; } = Keys.LShiftKey;
    [JsonPropertyName("Load")]
    public Keys Load { get; set; } = Keys.RShiftKey;
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