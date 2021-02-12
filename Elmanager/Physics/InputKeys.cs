namespace Elmanager.Physics
{
    internal class InputKeys
    {
        public bool Gas;
        public bool Brake;
        public bool LeftVolt;
        public bool RightVolt;
        public bool AloVolt;
        public bool Turn;

        public bool IsAnyDown => Gas || Brake || LeftVolt || RightVolt || AloVolt || Turn;
    }
}