using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Elmanager.Application;
using Elmanager.LevelEditor;
using Elmanager.LevelManager;
using Elmanager.ReplayManager;
using Elmanager.ReplayViewer;
using Elmanager.UI;
using Elmanager.Utilities.Json;

namespace Elmanager.Settings;

internal class ElmanagerSettings
{
    private const string SettingsFileDateFormat = "ddMMyyyy";
    private const string SettingsFileBaseName = "ElmanagerSettings";

    private static readonly string _settingsFile =
        SettingsFileBaseName + Global.Version.ToString(SettingsFileDateFormat) + ".json";

    [JsonPropertyName("General"), JsonInclude]
    public GeneralSettings General = new();
    [JsonPropertyName("LevelEditor"), JsonInclude]
    public LevelEditorSettings LevelEditor = new();
    [JsonPropertyName("ReplayManager"), JsonInclude]
    public ReplayManagerSettings ReplayManager = new();
    [JsonPropertyName("LevelManager"), JsonInclude]
    public LevelManagerSettings LevelManager = new();
    [JsonPropertyName("ReplayViewer"), JsonInclude]
    public ReplayViewerSettings ReplayViewer = new();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IgnoreReadOnlyProperties = true,
        Converters = { new ColorConverter(), new SizeConverter(), new PointConverter() }
    };

    public static string ElmanagerFolder => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;

    public static ElmanagerSettings Load()
    {
        if (File.Exists(Path.Combine(ElmanagerFolder, _settingsFile)))
        {
            return GetSettings(_settingsFile);
        }

        var oldSettingFiles = Directory.GetFiles(ElmanagerFolder, "ElmanagerSettings*.json");
        try
        {
            if (oldSettingFiles.Length > 0)
            {
                var oldFileDate = oldSettingFiles.Select(
                        path =>
                            DateTime.ParseExact(
                                Path.GetFileNameWithoutExtension(path).Substring(SettingsFileBaseName.Length),
                                SettingsFileDateFormat, CultureInfo.InvariantCulture))
                    .Max()
                    .ToString(SettingsFileDateFormat);
                return GetSettings(Path.Combine(ElmanagerFolder,
                    SettingsFileBaseName + oldFileDate + ".json"));
            }
        }
        catch (Exception)
        {
            UiUtils.ShowError("Could not load old settings. You need to set them again.");
        }

        return new ElmanagerSettings();
    }

    public async Task Save()
    {
        await using var appSettingsFile = new FileStream(Path.Combine(ElmanagerFolder, _settingsFile), FileMode.Create);
        await JsonSerializer.SerializeAsync(appSettingsFile, this, JsonSerializerOptions);
    }

    private static ElmanagerSettings GetSettings(string settingsFile)
    {
        var path = Path.Combine(ElmanagerFolder, settingsFile);
        try
        {
            var loadedSettings = JsonSerializer.Deserialize<ElmanagerSettings>(File.ReadAllText(path), JsonSerializerOptions);
            return loadedSettings!;
        }
        catch (JsonException e)
        {
            UiUtils.ShowError(e.Message);
            return new ElmanagerSettings();
        }
    }
}