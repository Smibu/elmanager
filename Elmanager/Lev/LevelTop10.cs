using System;
using System.Collections.Generic;
using System.Linq;

namespace Elmanager.Lev
{
    internal class LevelTop10
    {
        internal List<Top10EntryMulti> MultiPlayer = new();
        internal List<Top10EntrySingle> SinglePlayer = new();

        internal bool IsEmpty => SinglePlayer.Count == 0 && MultiPlayer.Count == 0;

        internal void Clear()
        {
            SinglePlayer.Clear();
            MultiPlayer.Clear();
        }

        internal double GetMultiPlayerAverage()
        {
            var avg = MultiPlayer.Sum(x => x.TimeInSecs);
            return MultiPlayer.Count > 0 ? avg / MultiPlayer.Count : 0.0;
        }

        internal string GetMultiPlayerString(int index)
        {
            return MultiPlayer.Count <= index ? "None" : MultiPlayer[index].FormatEntry(21);
        }

        internal string GetMultiPlayerString()
        {
            return GetTop10String(GetMultiPlayerString);
        }

        internal string GetSinglePlayerString()
        {
            return GetTop10String(GetSinglePlayerString);
        }

        private static string GetTop10String(Func<int, string> act)
        {
            return Enumerable.Range(0, 10).Select(i => (index: i, s: act(i))).Aggregate("", (s, s1) =>
                $"{s}{s1.index + 1,2}. {s1.s}{Environment.NewLine}").TrimEnd();
        }

        internal double GetSinglePlayerAverage()
        {
            var avg = SinglePlayer.Sum(x => x.TimeInSecs);
            return SinglePlayer.Count > 0 ? avg / SinglePlayer.Count : 0.0;
        }

        internal string GetSinglePlayerString(int index)
        {
            return SinglePlayer.Count <= index ? "None" : SinglePlayer[index].FormatEntry(12);
        }
    }
}