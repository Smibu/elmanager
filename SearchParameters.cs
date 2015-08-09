using System;
using System.Text.RegularExpressions;

namespace Elmanager
{
    internal class Bound<T> where T : IComparable
    {
        private T _max;
        private T _min;

        internal Bound(T min, T max)
        {
            _min = min;
            _max = max;
        }

        internal bool Accepts(T x)
        {
            return _min.CompareTo(x) <= 0 && _max.CompareTo(x) >= 0;
        }

        internal void Set(T min, T max)
        {
            _min = min;
            _max = max;
        }
    }

    internal enum RSearchOption
    {
        True,
        False,
        Dontcare
    }

    internal class SearchParameters
    {
        internal RSearchOption AcrossLev = RSearchOption.Dontcare;

        internal Bound<DateTime> Date = new Bound<DateTime>(new DateTime(1, 1, 1, 0, 0, 0),
                                                            new DateTime(9999, 1, 1, 0, 0, 0));

        internal RSearchOption Finished = RSearchOption.Dontcare;
        internal RSearchOption InternalRec = RSearchOption.Dontcare;
        internal RSearchOption LevExists = RSearchOption.Dontcare;
        internal Regex LevFilenameMatcher = new Regex("");
        internal RSearchOption MultiPlayer = RSearchOption.Dontcare;
        internal PlayerBounds P1Bounds = new PlayerBounds();
        internal PlayerBounds P2Bounds = new PlayerBounds();

        internal Bound<int> Size = new Bound<int>(0, 10000000);
        internal Bound<double> Time = new Bound<double>(0, 7200);
        internal RSearchOption WrongLev = RSearchOption.Dontcare;

        /// <summary>
        ///   Determines whether the search parameters match the given replay.
        /// </summary>
        /// <param name = "r">Replay to check.</param>
        /// <returns>True if the search parameters match this replay.</returns>
        internal bool Matches(Replay r)
        {
            bool levOk = Check(LevExists, r.LevelExists) && Check(WrongLev, r.WrongLevelVersion) &&
                         Check(AcrossLev, r.AcrossLevel) && LevFilenameMatcher.IsMatch(r.LevelFilename);
            bool recOk = Check(InternalRec, r.IsInternal) && Check(Finished, r.Finished) &&
                         Check(MultiPlayer, r.IsMulti) &&
                         Date.Accepts(r.DateModified) && Time.Accepts(r.Time) && Size.Accepts(r.Size);
            bool playersOk = P1Bounds.Matches(r.Player1) && (!r.IsMulti || P2Bounds.Matches(r.Player2));

            return levOk && recOk && playersOk;
        }

        internal void ResetOptions()
        {
            AcrossLev = RSearchOption.Dontcare;

            Date = new Bound<DateTime>(new DateTime(1, 1, 1, 0, 0, 0),
                                       new DateTime(9999, 1, 1, 0, 0, 0));

            Finished = RSearchOption.Dontcare;
            InternalRec = RSearchOption.Dontcare;
            LevExists = RSearchOption.Dontcare;
            LevFilenameMatcher = new Regex("");
            MultiPlayer = RSearchOption.Dontcare;
            P1Bounds = new PlayerBounds();
            P2Bounds = new PlayerBounds();

            Size = new Bound<int>(0, 10000000);
            Time = new Bound<double>(0, 7200);
            WrongLev = RSearchOption.Dontcare;
        }

        private static bool Check(RSearchOption o, bool b)
        {
            return o == RSearchOption.Dontcare || (o == RSearchOption.True && b) || (o == RSearchOption.False && !b);
        }

        internal class PlayerBounds
        {
            internal Bound<int> Apples = new Bound<int>(0, 10000);
            internal Bound<int> GroundTouches = new Bound<int>(0, 10000);
            internal Bound<int> LeftVolts = new Bound<int>(0, 10000);
            internal Bound<int> RightVolts = new Bound<int>(0, 10000);
            internal Bound<int> SuperVolts = new Bound<int>(0, 10000);
            internal Bound<int> Turns = new Bound<int>(0, 10000);

            internal bool Matches(Player p)
            {
                return Apples.Accepts(p.Apples) && Turns.Accepts(p.Turns) && LeftVolts.Accepts(p.LeftVolts) &&
                       RightVolts.Accepts(p.RightVolts) && GroundTouches.Accepts(p.GroundTouches) &&
                       SuperVolts.Accepts(p.SuperVolts);
            }
        }
    }
}