using System.ComponentModel;
using Elmanager.ElmaPrimitives;
using Elmanager.Utilities;

namespace Elmanager.Lev;

internal sealed class Top10EntryMulti : Top10Entry
{
    internal Top10EntryMulti(string playerA, string playerB, int time)
    {
        PlayerA = playerA;
        PlayerB = playerB;
        Time = time;
    }

    [Description("Player 1")] public override string PlayerA { get; protected set; }
    [Description("Player 2")] public override string PlayerB { get; protected set; }
    [Description("Time")] public ElmaTime TimeA => TimeInSecs;

    public string FormatEntry(int pad)
    {
        return $"{PlayerA}, {PlayerB}".PadRight(pad) + TimeInSecs.ToTimeString(2);
    }
}