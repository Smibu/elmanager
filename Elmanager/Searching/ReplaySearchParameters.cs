using System;
using System.Text.RegularExpressions;
using Elmanager.IO;
using Elmanager.Rec;

namespace Elmanager.Searching;

internal class ReplaySearchParameters : SearchParameters
{
    internal BoolOption Finished = BoolOption.Dontcare;
    internal BoolOption InternalRec = BoolOption.Dontcare;
    internal BoolOption LevExists = BoolOption.Dontcare;
    internal Regex LevFilenameMatcher = new("");
    internal BoolOption MultiPlayer = BoolOption.Dontcare;
    internal PlayerBounds P1Bounds = new();
    internal PlayerBounds P2Bounds = new();

    internal Range<double> Time = new(0, 7200);
    internal BoolOption WrongLev = BoolOption.Dontcare;

    internal bool Matches(ElmaFileObject<Replay> o)
    {
        var r = o.Obj;
        bool levOk = Check(LevExists, r.LevelExists) && Check(WrongLev, r.WrongLevelVersion) &&
                     Check(AcrossLev, r.AcrossLevel) && LevFilenameMatcher.IsMatch(r.LevelFilename);
        bool recOk = Check(InternalRec, r.IsInternal) && Check(Finished, r.Finished) &&
                     Check(MultiPlayer, r.IsMulti) &&
                     Date.Accepts(o.File.DateModified) && Time.Accepts(r.Time) && Size.Accepts(o.File.Size);
        bool playersOk = P1Bounds.Matches(r.Player1) && (!r.IsMulti || P2Bounds.Matches(r.Player2));

        return levOk && recOk && playersOk;
    }

    internal void ResetOptions()
    {
        AcrossLev = BoolOption.Dontcare;

        Date = new Range<DateTime>(DateTime.MinValue,
            DateTime.MaxValue);

        Finished = BoolOption.Dontcare;
        InternalRec = BoolOption.Dontcare;
        LevExists = BoolOption.Dontcare;
        LevFilenameMatcher = new Regex("");
        MultiPlayer = BoolOption.Dontcare;
        P1Bounds = new PlayerBounds();
        P2Bounds = new PlayerBounds();

        Size = new Range<int>(0, 10000000);
        Time = new Range<double>(0, 7200);
        WrongLev = BoolOption.Dontcare;
    }

    private static bool Check(BoolOption o, bool b)
    {
        return o == BoolOption.Dontcare || (o == BoolOption.True && b) || (o == BoolOption.False && !b);
    }

    internal class PlayerBounds
    {
        internal Range<int> Apples = new(0, 10000);
        internal Range<int> GroundTouches = new(0, 10000);
        internal Range<int> LeftVolts = new(0, 10000);
        internal Range<int> RightVolts = new(0, 10000);
        internal Range<int> SuperVolts = new(0, 10000);
        internal Range<int> Turns = new(0, 10000);

        internal bool Matches(Player p)
        {
            return Apples.Accepts(p.Apples) && Turns.Accepts(p.Turns) && LeftVolts.Accepts(p.LeftVolts) &&
                   RightVolts.Accepts(p.RightVolts) && GroundTouches.Accepts(p.GroundTouches) &&
                   SuperVolts.Accepts(p.SuperVolts);
        }
    }
}