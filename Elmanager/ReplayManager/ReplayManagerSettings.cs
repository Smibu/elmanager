using Elmanager.Settings;

namespace Elmanager.ReplayManager
{
    internal class ReplayManagerSettings : ManagerSettings
    {
        public bool Decimal3Shown { get; set; }
        public string Nickname { get; set; } = "Nick";
        public bool NitroReplays { get; set; }
        public string Pattern { get; set; } = "LNT";

        public int NumDecimals => Decimal3Shown ? 3 : 2;

    }
}