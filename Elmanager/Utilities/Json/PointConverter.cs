using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elmanager.Utilities.Json;

internal class PointConverter : JsonConverter<Point>
{
    public override Point Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var (fst, snd) = reader.ReadTwoInts(",");
        return new Point(fst, snd);
    }

    public override void Write(
        Utf8JsonWriter writer,
        Point point,
        JsonSerializerOptions options) =>
        writer.WriteStringValue($"{point.X},{point.Y}");
}