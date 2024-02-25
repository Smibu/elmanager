using System.Text.Json.Serialization;

namespace Elmanager.Settings;

internal class GeneralSettings
{
    [JsonPropertyName("CheckForUpdatesOnStartup")]
    public bool CheckForUpdatesOnStartup { get; set; } = true;
    [JsonPropertyName("LevelDirectory")]
    public string LevelDirectory { get; set; } = string.Empty;
    [JsonPropertyName("LgrDirectory")]
    public string LgrDirectory { get; set; } = string.Empty;
    [JsonPropertyName("ReplayDirectory")]
    public string ReplayDirectory { get; set; } = string.Empty;
}