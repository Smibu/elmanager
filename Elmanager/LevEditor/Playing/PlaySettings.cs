using System.Windows.Forms;

namespace Elmanager.LevEditor.Playing
{
    internal class PlaySettings
    {
        public Keys Gas { get; set; } = Keys.Up;
        public Keys Brake { get; set; } = Keys.Down;
        public Keys LeftVolt { get; set; } = Keys.Left;
        public Keys RightVolt { get; set; } = Keys.Right;
        public Keys AloVolt { get; set; } = Keys.Insert;
        public Keys Turn { get; set; } = Keys.Space;
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
            DyingBehavior = other.DyingBehavior;
            FollowDriverOption = other.FollowDriverOption;
        }
    }
}