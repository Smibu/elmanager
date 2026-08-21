using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Platform;

namespace Elmanager.SLE.LgrUtil;

internal class KnownLgr
{
    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("hash")] public required string Hash { get; set; }
}

[JsonSerializable(typeof(List<KnownLgr>))]
internal partial class KnownLgrSourceGenerationContext : JsonSerializerContext;

internal static class KnownLgrs
{
    internal const string UnknownName = "Unknown";

    private static readonly Lazy<Dictionary<string, string>> HashToName = new(Load);

    private static Dictionary<string, string> Load()
    {
        try
        {
            using var stream = AssetLoader.Open(new Uri("avares://Elmanager.SLE/Resources/lgrs.json"));
            var entries = JsonSerializer.Deserialize(stream, KnownLgrSourceGenerationContext.Default.ListKnownLgr);
            return entries?.GroupBy(e => e.Hash)
                       .ToDictionary(g => g.Key, g => g.First().Name, StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load known LGR metadata: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    public static string ResolveName(byte[] lgrBytes)
    {
        var hash = Convert.ToHexString(SHA256.HashData(lgrBytes));
        return HashToName.Value.GetValueOrDefault(hash, UnknownName);
    }
}
