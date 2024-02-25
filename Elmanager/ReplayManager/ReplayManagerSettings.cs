using System.Text.Json.Serialization;
using Elmanager.Settings;

namespace Elmanager.ReplayManager;

internal class ReplayManagerSettings : ManagerSettings
{
    [JsonPropertyName("Decimal3Shown")]
    public bool Decimal3Shown { get; set; }
    [JsonPropertyName("Nickname")]
    public string Nickname { get; set; } = "Nick";
    [JsonPropertyName("NitroReplays")]
    public bool NitroReplays { get; set; }
    [JsonPropertyName("Pattern")]
    public string Pattern { get; set; } = "LNT";

    public int NumDecimals => Decimal3Shown ? 3 : 2;

}