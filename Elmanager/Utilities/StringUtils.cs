using System;
using System.Text;

namespace Elmanager.Utilities;

internal static class StringUtils
{
    internal static bool CompareWith(this string str1, string str2)
    {
        return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
    }

    internal static double StringToTime(string timeStr)
    {
        return double.Parse(timeStr.Substring(0, 2)) * 60 + double.Parse(timeStr.Substring(3, 2)) +
               double.Parse(timeStr.Substring(6)) * 0.001;
    }

    internal static string ToTimeString(this double time, int digits = 3)
    {
        double T = Math.Abs(time);
        StringBuilder timeStr = new StringBuilder(9);
        int minutes = (int)Math.Floor(T / 60);
        int hours = (int)Math.Floor(T / 3600);
        if (hours > 0)
        {
            timeStr.Append(hours);
            timeStr.Append(":");
            minutes -= 60 * hours;
        }

        double timeVar = T - (60 * minutes + 3600 * hours);
        timeStr.Append(minutes.ToString("D2"));
        timeStr.Append(":");
        switch (digits)
        {
            case 3:
                timeStr.Append($"{timeVar:00.000}");
                break;
            case 2:
                timeStr.Append($"{timeVar:00.00}");
                break;
            default:
                throw new Exception("Invalid number of digits!");
        }

        if (time < 0)
            timeStr.Insert(0, '-');
        return timeStr.ToString();
    }
}