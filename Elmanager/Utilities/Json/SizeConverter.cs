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
        var (fst, snd) = reader.ReadTwoInts("x");
        return new Size(fst, snd);
    }

    public override void Write(
        Utf8JsonWriter writer,
        Size size,
        JsonSerializerOptions options) =>
        writer.WriteStringValue($"{size.Width}x{size.Height}");
}