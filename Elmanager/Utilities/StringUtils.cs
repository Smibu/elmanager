using System;

namespace Elmanager.Utilities;

internal static class StringUtils
{
    internal static bool EqualsIgnoreCase(this string? str1, string? str2)
    {
        return str1 == null && str2 == null || str1 != null && str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
    }

    internal static double StringToTime(string timeStr)
    {
        return double.Parse(timeStr.Substring(0, 2)) * 60 + double.Parse(timeStr.Substring(3, 2)) +
               double.Parse(timeStr.Substring(6)) * 0.001;
    }

    internal static string ToTimeString(this double time, int digits = 3)
    {
        if (digits is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(digits), "Invalid number of digits!");

        double T = Math.Abs(time);
        int hours = (int)(T / 3600);
        int minutes = (int)(T / 60) % 60;
        double seconds = T - (60 * minutes + 3600 * hours);

        var secondsFormat = "00." + new string('0', digits);
        var result = hours > 0
            ? $"{hours}:{minutes:D2}:{seconds.ToString(secondsFormat)}"
            : $"{minutes:D2}:{seconds.ToString(secondsFormat)}";

        return time < 0 ? "-" + result : result;
    }
}
