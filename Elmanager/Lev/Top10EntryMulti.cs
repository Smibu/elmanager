using System.ComponentModel;
using Elmanager.Utilities;

namespace Elmanager.Lev
{
    internal class Top10EntryMulti : Top10Entry
    {
        internal Top10EntryMulti(string playerA, string playerB, int time)
        {
            Init(playerA, playerB, time);
        }

        [Description("Player 1")] public override string PlayerA { get; protected set; }
        [Description("Player 2")] public override string PlayerB { get; protected set; }

        public override string FormatEntry(int pad)
        {
            return $"{PlayerA}, {PlayerB}".PadRight(pad) + TimeInSecs.ToTimeString(2);
        }
    }
}