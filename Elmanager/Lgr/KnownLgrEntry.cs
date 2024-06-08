using System.Text.Json.Serialization;

namespace Elmanager.Lgr;

internal class KnownLgrEntry
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("hash")]
    public required string Hash { get; set; }
}