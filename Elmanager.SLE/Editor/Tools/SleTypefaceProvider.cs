using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Platform;
using SkiaSharp;

namespace Elmanager.SLE.Editor.Tools;

internal readonly record struct TypefaceStyle(string Name, bool Bold, bool Italic);

internal static class SleTypefaceProvider
{
    public static readonly IReadOnlyList<string> FontFamilies = LoadFontFamilies();

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

    private static readonly Dictionary<TypefaceKey, string> CssCache = new();
    private static readonly HttpClient Http = new();

    private static IReadOnlyList<string> LoadFontFamilies()
    {
        using var stream = AssetLoader.Open(new Uri("avares://Elmanager.SLE/Resources/googlefonts.txt"));
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd()
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public static async Task<IReadOnlyList<TypefaceStyle>> GetSupportedStylesAsync(string family)
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
                await IsStyleAvailableFromGoogleFontsAsync(family, style.Bold, style.Italic))
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

    private static async Task<bool> IsStyleAvailableFromGoogleFontsAsync(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        lock (InFlight)
        {
            if (FailedDownloads.Contains(key))
            {
                return false;
            }
        }

        try
        {
            var css = await GetGoogleFontsCssAsync(family, bold, italic);
            return GetBestFontUrlMatch(css).Success;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            MarkFailed(key);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SleTypefaceProvider: CSS probe failed for '{family}': {ex.Message}");
            return false;
        }
    }

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
        string css;
        try
        {
            css = await GetGoogleFontsCssAsync(family, bold, italic);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SleTypefaceProvider: CSS fetch failed for '{family}': {ex.Message}");
            if (ex is HttpRequestException { StatusCode: HttpStatusCode.BadRequest } && (bold || italic))
            {
                return await DownloadFromGoogleFonts(family, false, false);
            }

            return null;
        }

        var match = GetBestFontUrlMatch(css);
        if (!match.Success)
        {
            return null;
        }

        var fontUrl = match.Groups[1].Value.Trim('\'', '"');
        byte[] fontBytes;
        try
        {
            fontBytes = await Http.GetByteArrayAsync(fontUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SleTypefaceProvider: font download failed for '{family}': {ex.Message}");
            return null;
        }

        return SKTypeface.FromData(SKData.CreateCopy(fontBytes));
    }

    private static async Task<string> GetGoogleFontsCssAsync(string family, bool bold, bool italic)
    {
        var key = new TypefaceKey(family, bold, italic);
        lock (InFlight)
        {
            if (CssCache.TryGetValue(key, out var cachedCss))
            {
                return cachedCss;
            }
        }

        using var req = new HttpRequestMessage(HttpMethod.Get, BuildGoogleFontsCssUrl(family, bold, italic));
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Elmanager)");
        using var resp = await Http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var css = await resp.Content.ReadAsStringAsync();

        lock (InFlight)
        {
            CssCache[key] = css;
        }

        return css;
    }

    private static string BuildGoogleFontsCssUrl(string family, bool bold, bool italic)
    {
        var style = (bold, italic) switch
        {
            (true, true) => ":bolditalic",
            (false, true) => ":italic",
            (true, false) => ":bold",
            _ => ""
        };
        var encodedFamily = family.Replace(' ', '+');
        return $"https://fonts.googleapis.com/css?family={encodedFamily}{style}";
    }

    private static Match GetBestFontUrlMatch(string css)
    {
        var best = Match.Empty;
        var blocks = Regex.Matches(css, @"@font-face\s*\{(?<block>.*?)\}",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        foreach (Match blockMatch in blocks)
        {
            var block = blockMatch.Groups["block"].Value;
            var urlMatch = Regex.Match(block, @"src:\s*url\(([^)]+)\)", RegexOptions.IgnoreCase);
            if (!urlMatch.Success)
            {
                continue;
            }

            best = urlMatch;
            if (ContainsBasicLatin(block))
            {
                return urlMatch;
            }
        }

        return best.Success
            ? best
            : Regex.Match(css, @"src:\s*url\(([^)]+)\)", RegexOptions.IgnoreCase);
    }

    private static bool ContainsBasicLatin(string fontFaceBlock)
    {
        var rangeMatch = Regex.Match(fontFaceBlock, @"unicode-range:\s*([^;]+)", RegexOptions.IgnoreCase);
        if (!rangeMatch.Success)
        {
            return true;
        }

        foreach (Match part in Regex.Matches(rangeMatch.Groups[1].Value, @"U\+([0-9A-F]{1,6})(?:-([0-9A-F]{1,6}))?",
                     RegexOptions.IgnoreCase))
        {
            var start = Convert.ToInt32(part.Groups[1].Value, 16);
            var end = part.Groups[2].Success ? Convert.ToInt32(part.Groups[2].Value, 16) : start;
            if (start <= 0x7E && end >= 0x20)
            {
                return true;
            }
        }

        return false;
    }

    private record TypefaceKey(string Family, bool Bold, bool Italic);
}
