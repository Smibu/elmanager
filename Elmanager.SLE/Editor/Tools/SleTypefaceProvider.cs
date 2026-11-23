using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Platform;
using SkiaSharp;

namespace Elmanager.SLE.Editor.Tools;

internal readonly record struct TypefaceStyle(string Name, bool Bold, bool Italic);

internal static class SleTypefaceProvider
{
    private const string FontUrlPrefix = "https://fonts.gstatic.com/s/";

    private static readonly IReadOnlyList<GoogleFont> FontList = LoadFontList();

    public static readonly IReadOnlyList<string> FontFamilies = FontList.Select(f => f.Family).ToList();

    private static readonly Dictionary<string, GoogleFont> FontsByFamily =
        FontList.ToDictionary(f => f.Family, StringComparer.OrdinalIgnoreCase);

    private static readonly IReadOnlyList<TypefaceStyle> AllStyles =
    [
        new("Regular", false, false),
        new("Bold", true, false),
        new("Italic", false, true),
        new("Bold Italic", true, true)
    ];

    private static readonly Dictionary<TypefaceKey, SKTypeface> Cache = new();
    private static readonly Dictionary<TypefaceKey, Task<SKTypeface?>> InFlight = new();
    private static readonly HashSet<TypefaceKey> FailedDownloads = new();

    private static readonly Dictionary<string, IReadOnlyList<TypefaceStyle>> SupportedStyleCache =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly HttpClient Http = new();

    private static IReadOnlyList<GoogleFont> LoadFontList()
    {
        using var stream = AssetLoader.Open(new Uri("avares://Elmanager.SLE/Resources/googlefonts.tsv"));
        using var reader = new StreamReader(stream);
        return ParseFontList(reader.ReadToEnd());
    }

    internal static IReadOnlyList<GoogleFont> ParseFontList(string text) =>
        text.Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0)
            .Select(line =>
            {
                var columns = line.Split('\t');
                var paths = Enumerable.Range(1, 4)
                    .Select(i => i < columns.Length && columns[i].Length > 0 ? columns[i] : null)
                    .ToArray();
                return new GoogleFont(columns[0], paths);
            })
            .ToList();

    public static IReadOnlyList<TypefaceStyle> GetSupportedStyles(string family)
    {
        lock (InFlight)
        {
            if (SupportedStyleCache.TryGetValue(family, out var cached))
            {
                return cached;
            }
        }

        var result = new List<TypefaceStyle>();
        foreach (var style in AllStyles)
        {
            if (IsStyleAvailableLocally(family, style.Bold, style.Italic) ||
                IsStyleAvailableFromGoogleFonts(family, style.Bold, style.Italic))
            {
                result.Add(style);
            }
        }

        if (result.Count == 0)
        {
            result.Add(AllStyles[0]);
        }

        lock (InFlight)
        {
            SupportedStyleCache[family] = result;
        }

        return result;
    }

    public static bool IsCached(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        if (Cache.ContainsKey(key))
        {
            return true;
        }

        lock (InFlight)
        {
            if (FailedDownloads.Contains(key))
            {
                return true;
            }
        }

        var system = GetSystemTypeface(family, bold, italic);
        var resolved = system != null;
        system?.Dispose();
        return resolved;
    }

    public static SKTypeface? GetCached(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        if (Cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var system = GetSystemTypeface(family, bold, italic);
        if (system != null)
        {
            Cache[key] = system;
            return system;
        }

        lock (InFlight)
        {
            if (FailedDownloads.Contains(key))
            {
                return null;
            }
        }

        return null;
    }

    public static async Task<SKTypeface?> LoadAsync(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        if (Cache.TryGetValue(key, out var hit))
        {
            return hit;
        }

        var system = GetSystemTypeface(family, bold, italic);
        if (system != null)
        {
            Cache[key] = system;
            return system;
        }

        Task<SKTypeface?>? downloadTask;
        var startedDownload = false;
        lock (InFlight)
        {
            if (FailedDownloads.Contains(key))
            {
                return null;
            }

            if (!InFlight.TryGetValue(key, out downloadTask))
            {
                downloadTask = LoadFromRemoteAsync(key);
                InFlight[key] = downloadTask;
                startedDownload = true;
            }
        }

        try
        {
            return await downloadTask;
        }
        finally
        {
            if (startedDownload)
            {
                lock (InFlight)
                {
                    InFlight.Remove(key);
                }
            }
        }
    }

    private static async Task<SKTypeface?> LoadFromRemoteAsync(TypefaceKey key)
    {
        try
        {
            var typeface = await DownloadFromGoogleFonts(key.Family, key.Bold, key.Italic);
            if (typeface != null)
            {
                Cache[key] = typeface;
                if (!MatchesRequest(typeface, key.Family, key.Bold, key.Italic))
                {
                    MarkFailed(key);
                }

                var regularKey = key with { Bold = false, Italic = false };
                if (!Cache.ContainsKey(regularKey))
                {
                    var regularSystem = SKTypeface.FromFamilyName(key.Family);
                    if (regularSystem != null &&
                        regularSystem.FamilyName.Equals(key.Family, StringComparison.OrdinalIgnoreCase))
                    {
                        Cache[regularKey] = regularSystem;
                    }
                    else
                    {
                        regularSystem?.Dispose();
                    }
                }

                return typeface;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SleTypefaceProvider: failed to load '{key.Family}': {ex.Message}");
        }

        MarkFailed(key);
        return null;
    }

    private static void MarkFailed(TypefaceKey key)
    {
        lock (InFlight)
        {
            FailedDownloads.Add(key);
            SupportedStyleCache.Remove(key.Family);
        }
    }

    private static bool IsStyleAvailableLocally(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        if (Cache.ContainsKey(key))
        {
            return true;
        }

        lock (InFlight)
        {
            if (FailedDownloads.Contains(key))
            {
                return false;
            }
        }

        var system = GetSystemTypeface(family, bold, italic);
        var available = system != null;
        system?.Dispose();
        return available;
    }

    private static bool IsStyleAvailableFromGoogleFonts(string family, bool bold, bool italic)
    {
        lock (InFlight)
        {
            if (FailedDownloads.Contains(new TypefaceKey(family, bold, italic)))
            {
                return false;
            }
        }

        return GetFontPath(family, bold, italic) != null;
    }

    private static string? GetFontPath(string family, bool bold, bool italic) =>
        FontsByFamily.TryGetValue(family, out var font) ? font.Paths[(bold ? 1 : 0) + (italic ? 2 : 0)] : null;

    private static SKTypeface? GetSystemTypeface(string family, bool bold, bool italic)
    {
        var slant = italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
        var weight = bold ? (int)SKFontStyleWeight.Bold : (int)SKFontStyleWeight.Normal;
        var system = SKTypeface.FromFamilyName(family, weight, (int)SKFontStyleWidth.Normal, slant);
        if (MatchesRequest(system, family, bold, italic))
        {
            return system;
        }

        system?.Dispose();
        return null;
    }

    private static bool MatchesRequest(SKTypeface? typeface, string family, bool bold, bool italic)
    {
        if (typeface is null || !typeface.FamilyName.Equals(family, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var style = typeface.FontStyle;
        return (style.Weight >= (int)SKFontStyleWeight.SemiBold) == bold
               && style.Slant == (italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
    }

    private static async Task<SKTypeface?> DownloadFromGoogleFonts(string family, bool bold, bool italic)
    {
        var path = GetFontPath(family, bold, italic) ?? GetFontPath(family, false, false);
        if (path == null)
        {
            return null;
        }

        byte[] fontBytes;
        try
        {
            fontBytes = await Http.GetByteArrayAsync(FontUrlPrefix + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SleTypefaceProvider: font download failed for '{family}': {ex.Message}");
            return null;
        }

        return SKTypeface.FromData(SKData.CreateCopy(fontBytes));
    }

    private record TypefaceKey(string Family, bool Bold, bool Italic);

    internal sealed record GoogleFont(string Family, string?[] Paths);
}
