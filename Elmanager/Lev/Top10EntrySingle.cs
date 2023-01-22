using System.ComponentModel;
using Elmanager.ElmaPrimitives;
using Elmanager.Utilities;

namespace Elmanager.Lev;

internal sealed class Top10EntrySingle : Top10Entry
{
    [Description("Player 1")] public override string PlayerA { get; protected set; }
    public override string PlayerB { get; protected set; }
    [Description("Time")] public ElmaTime TimeA => TimeInSecs;

    internal Top10EntrySingle(string playerA, string playerB, int time)
    {
        PlayerA = playerA;
        PlayerB = playerB;
        Time = time;
    }

    public string FormatEntry(int pad)
    {
        return $"{PlayerA}".PadRight(pad) + TimeInSecs.ToTimeString(2);
    }
}