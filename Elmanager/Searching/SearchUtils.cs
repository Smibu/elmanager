using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Elmanager.Utilities;

namespace Elmanager.Searching;

internal static class SearchUtils
{
    public static IEnumerable<string> FilterByRegex(IEnumerable<string> files, string pattern)
    {
        Regex matcher;
        try
        {
            matcher = new Regex(pattern, RegexOptions.IgnoreCase);
        }
        catch (Exception)
        {
            matcher = new Regex(string.Empty, RegexOptions.IgnoreCase);
        }

        return files.Where(x =>
        {
            var f = Path.GetFileNameWithoutExtension(x);
            return matcher.IsMatch(f);
        });
    }

    public static Range<double> GetTimeRange(string timeMin, string timeMax)
    {
        return new(StringUtils.StringToTime(timeMin),
            StringUtils.StringToTime(timeMax));
    }
}