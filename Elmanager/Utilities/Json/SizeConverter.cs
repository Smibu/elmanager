using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elmanager.Utilities.Json;

internal class SizeConverter : JsonConverter<Size>
{
    public override Size Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        try
        {
            var hexStr = reader.GetString().Split("x");
            return new Size(int.Parse(hexStr[0]), int.Parse(hexStr[1]));
        }
        catch (Exception)
        {
            throw new JsonException();
        }
    }

    public override void Write(
        Utf8JsonWriter writer,
        Size size,
        JsonSerializerOptions options) =>
        writer.WriteStringValue($"{size.Width}x{size.Height}");
}