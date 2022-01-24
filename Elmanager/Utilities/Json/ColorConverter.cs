using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elmanager.Utilities.Json;

internal class ColorConverter : JsonConverter<Color>
{
    public override Color Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var hexStr = reader.GetString();
        return Color.FromArgb(Convert.ToInt32(hexStr, 16));
    }

    public override void Write(
        Utf8JsonWriter writer,
        Color color,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(color.ToArgb().ToString("X"));
}