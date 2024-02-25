using System.Text.Json.Serialization;

namespace Elmanager.Settings;

internal class GeneralSettings
{
    [JsonPropertyName("CheckForUpdatesOnStartup")]
    public bool CheckForUpdatesOnStartup { get; set; } = true;
    [JsonPropertyName("LevelDirectory")]
    public string? LevelDirectory { get; set; }
    [JsonPropertyName("LgrDirectory")]
    public string? LgrDirectory { get; set; }
    [JsonPropertyName("ReplayDirectory")]
    public string? ReplayDirectory { get; set; }
}