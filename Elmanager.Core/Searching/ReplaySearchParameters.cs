using System;
using System.Text.RegularExpressions;
using Elmanager.IO;
using Elmanager.Rec;

namespace Elmanager.Searching;

public class ReplaySearchParameters : SearchParameters
{
    public BoolOption Finished = BoolOption.Dontcare;
    public BoolOption InternalRec = BoolOption.Dontcare;
    public BoolOption LevExists = BoolOption.Dontcare;
    public Regex LevFilenameMatcher = new("");
    public BoolOption MultiPlayer = BoolOption.Dontcare;
    public PlayerBounds P1Bounds = new();
    public PlayerBounds P2Bounds = new();

    public Range<double> Time = new(0, 7200);
    public BoolOption WrongLev = BoolOption.Dontcare;

    public bool Matches(ElmaFileObject<Replay> o)
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

    public void ResetOptions()
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

    public class PlayerBounds
    {
        public Range<int> Apples = new(0, 10000);
        public Range<int> GroundTouches = new(0, 10000);
        public Range<int> LeftVolts = new(0, 10000);
        public Range<int> RightVolts = new(0, 10000);
        public Range<int> SuperVolts = new(0, 10000);
        public Range<int> Turns = new(0, 10000);

        internal bool Matches(Player p)
        {
            return Apples.Accepts(p.Apples) && Turns.Accepts(p.Turns) && LeftVolts.Accepts(p.LeftVolts) &&
                   RightVolts.Accepts(p.RightVolts) && GroundTouches.Accepts(p.GroundTouches) &&
                   SuperVolts.Accepts(p.SuperVolts);
        }
    }
}
