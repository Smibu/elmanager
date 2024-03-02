using System.Text.Json;
using System.Text.Json.Serialization;
using Elmanager.Settings;
using Elmanager.Updating;

namespace Elmanager.Utilities.Json;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ElmanagerSettings))]
[JsonSerializable(typeof(UpdateInfo))]
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
}