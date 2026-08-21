using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Elmanager.LevelEditor;
using Elmanager.Updating;

namespace Elmanager.Utilities.Json;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(UpdateInfo))]
[JsonSerializable(typeof(LevelEditorSettings))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
    public static JsonSerializerOptions GetOptions() =>
        new()
        {
            TypeInfoResolver = Default,
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
            Converters = { new ColorConverter(), new PointConverter(), new SizeConverter() }
        };

    public static JsonTypeInfo<LevelEditorSettings> GetLevelEditorSettingsTypeInfo()
    {
        var options = GetOptions();
        options.MakeReadOnly();
        return (JsonTypeInfo<LevelEditorSettings>)options.GetTypeInfo(typeof(LevelEditorSettings));
    }
}
