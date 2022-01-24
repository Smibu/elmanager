namespace Elmanager.Settings;

internal class GeneralSettings
{
    public bool CheckForUpdatesOnStartup { get; set; } = true;
    public string LevelDirectory { get; set; } = string.Empty;
    public string LgrDirectory { get; set; } = string.Empty;
    public string ReplayDirectory { get; set; } = string.Empty;
}