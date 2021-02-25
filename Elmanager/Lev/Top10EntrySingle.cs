using System.ComponentModel;
using Elmanager.Utilities;

namespace Elmanager.Lev
{
    internal class Top10EntrySingle : Top10Entry
    {
        [Description("Player 1")] public override string PlayerA { get; protected set; }
        public override string PlayerB { get; protected set; }

        internal Top10EntrySingle(string playerA, string playerB, int time)
        {
            Init(playerA, playerB, time);
        }

        public override string FormatEntry(int pad)
        {
            return $"{PlayerA}".PadRight(pad) + TimeInSecs.ToTimeString(2);
        }
    }
}