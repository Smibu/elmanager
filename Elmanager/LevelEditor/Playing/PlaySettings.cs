using System.Windows.Forms;

namespace Elmanager.LevelEditor.Playing;

internal class PlaySettings
{
    public Keys Gas { get; set; } = Keys.Up;
    public Keys Brake { get; set; } = Keys.Down;
    public Keys BrakeAlias { get; set; } = Keys.X;
    public Keys LeftVolt { get; set; } = Keys.Left;
    public Keys RightVolt { get; set; } = Keys.Right;
    public Keys AloVolt { get; set; } = Keys.Insert;
    public Keys Turn { get; set; } = Keys.Space;
    public Keys Save { get; set; } = Keys.LShiftKey;
    public Keys Load { get; set; } = Keys.RShiftKey;
    public DyingBehavior DyingBehavior { get; set; } = DyingBehavior.StopPlaying;
    public FollowDriverOption FollowDriverOption { get; set; } = FollowDriverOption.WhenPressingKey;
    public bool DisableShortcuts { get; set; }
    public int PhysicsFps { get; set; } = 1000;
    public bool ConstantFps { get; set; }
    public bool ToggleFullscreen { get; set; }
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