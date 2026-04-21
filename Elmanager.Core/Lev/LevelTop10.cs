using System;
using System.Collections.Generic;
using System.Linq;

namespace Elmanager.Lev;

public class LevelTop10
{
    public List<Top10EntryMulti> MultiPlayer = new();
    public List<Top10EntrySingle> SinglePlayer = new();

    public bool IsEmpty => SinglePlayer.Count == 0 && MultiPlayer.Count == 0;

    internal void Clear()
    {
        SinglePlayer.Clear();
        MultiPlayer.Clear();
    }

    public double GetMultiPlayerAverage()
    {
        var avg = MultiPlayer.Sum(x => x.TimeInSecs);
        return MultiPlayer.Count > 0 ? avg / MultiPlayer.Count : 0.0;
    }

    public string GetMultiPlayerString(int index)
    {
        return MultiPlayer.Count <= index ? "None" : MultiPlayer[index].FormatEntry(21);
    }

    public string GetMultiPlayerString()
    {
        return GetTop10String(GetMultiPlayerString);
    }

    public string GetSinglePlayerString()
    {
        return GetTop10String(GetSinglePlayerString);
    }

    private static string GetTop10String(Func<int, string> act)
    {
        return Enumerable.Range(0, 10).Select(i => (index: i, s: act(i))).Aggregate("", (s, s1) =>
            $"{s}{s1.index + 1,2}. {s1.s}{Environment.NewLine}").TrimEnd();
    }

    public double GetSinglePlayerAverage()
    {
        var avg = SinglePlayer.Sum(x => x.TimeInSecs);
        return SinglePlayer.Count > 0 ? avg / SinglePlayer.Count : 0.0;
    }

    public string GetSinglePlayerString(int index)
    {
        return SinglePlayer.Count <= index ? "None" : SinglePlayer[index].FormatEntry(12);
    }
}
