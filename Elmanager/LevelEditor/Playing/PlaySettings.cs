using System.Windows.Forms;

namespace Elmanager.LevelEditor.Playing
{
    internal class PlaySettings
    {
        public Keys Gas { get; set; } = Keys.Up;
        public Keys Brake { get; set; } = Keys.Down;
        public Keys LeftVolt { get; set; } = Keys.Left;
        public Keys RightVolt { get; set; } = Keys.Right;
        public Keys AloVolt { get; set; } = Keys.Insert;
        public Keys Turn { get; set; } = Keys.Space;
        public Keys Save { get; set; } = Keys.LShiftKey;
        public Keys Load { get; set; } = Keys.RShiftKey;
        public DyingBehavior DyingBehavior { get; set; } = DyingBehavior.StopPlaying;
        public FollowDriverOption FollowDriverOption { get; set; } = FollowDriverOption.WhenPressingKey;

        public PlaySettings() { }

        public PlaySettings(PlaySettings other)
        {
            Gas = other.Gas;
            Brake = other.Brake;
            LeftVolt = other.LeftVolt;
            RightVolt = other.RightVolt;
            AloVolt = other.AloVolt;
            Turn = other.Turn;
            Save = other.Save;
            Load = other.Load;
            DyingBehavior = other.DyingBehavior;
            FollowDriverOption = other.FollowDriverOption;
        }
    }
}