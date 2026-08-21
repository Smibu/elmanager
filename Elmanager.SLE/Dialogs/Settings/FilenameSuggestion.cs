using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Elmanager.SLE.Dialogs.Settings;

internal static class FilenameSuggestion
{
    private static readonly char[] InvalidFilenameCharacters = ['/', '\\', ':', '*', '"', '<', '>', '|'];

    public static string? ValidatePattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return null;
        }

        if (pattern.IndexOfAny(InvalidFilenameCharacters) >= 0)
        {
            return "The default filename contains an invalid character.";
        }

        foreach (var character in pattern)
        {
            if (char.IsControl(character))
            {
                return "The default filename contains an invalid character.";
            }
        }

        if (pattern is "." or ".." || pattern.EndsWith(' ') || pattern.EndsWith('.'))
        {
            return "Enter a valid filename.";
        }

        return TryGetCounter(pattern, out _, out _)
            ? null
            : "Use only one consecutive group of ? characters.";
    }

    public static string? Create(string pattern, IEnumerable<string> levelFiles)
    {
        if (string.IsNullOrWhiteSpace(pattern) || ValidatePattern(pattern) is not null)
        {
            return null;
        }

        if (!TryGetCounter(pattern, out var counterIndex, out var counterLength) || counterLength == 0)
        {
            return pattern;
        }

        var prefix = pattern[..counterIndex];
        var suffix = pattern[(counterIndex + counterLength)..];
        var patternIncludesExtension = pattern.EndsWith(".lev", StringComparison.OrdinalIgnoreCase);
        var highestNumber = 0;
        int? lowestNumber = null;

        foreach (var levelFile in levelFiles)
        {
            var fileName = patternIncludesExtension
                ? Path.GetFileName(levelFile)
                : Path.GetFileNameWithoutExtension(levelFile);
            if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                !fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var numberLength = fileName.Length - prefix.Length - suffix.Length;
            if (numberLength <= 0)
            {
                continue;
            }

            var numberText = fileName.Substring(prefix.Length, numberLength);
            if (!int.TryParse(
                    numberText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var number))
            {
                continue;
            }

            highestNumber = Math.Max(highestNumber, number);
            lowestNumber = lowestNumber is null ? number : Math.Min(lowestNumber.Value, number);
        }

        var lowest = lowestNumber ?? 0;
        var nextNumberValue = highestNumber == 0 || lowest <= 1
            ? highestNumber + 1
            : lowest - 1;
        var nextNumber = nextNumberValue.ToString(CultureInfo.InvariantCulture);
        return prefix + nextNumber.PadLeft(counterLength, '0') + suffix;
    }

    private static bool TryGetCounter(string pattern, out int index, out int length)
    {
        index = pattern.IndexOf('?');
        if (index < 0)
        {
            length = 0;
            return true;
        }

        var end = index;
        while (end < pattern.Length && pattern[end] == '?')
        {
            end++;
        }

        length = end - index;
        return pattern.IndexOf('?', end) < 0;
    }
}
