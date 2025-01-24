using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elmanager.Geometry;

namespace Elmanager.LevelEditor.ShapeGallery;

internal class VectorDto
{
    public double X { get; set; }
    public double Y { get; set; }

    public VectorDto()
    {

    }

    public VectorDto(Vector vector)
    {
        X = vector.X;
        Y = vector.Y;
    }
    public Vector ToVector()
    {
        return new Vector(X, Y);
    }
}

internal class VectorDtoConverter : JsonConverter<VectorDto>
{
    public override VectorDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            double x = reader.GetDouble();
            reader.Read();
            double y = reader.GetDouble();
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            return new VectorDto { X = x, Y = y };
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            double x = 0, y = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? string.Empty;
                    reader.Read();
                    if (propertyName == "X")
                    {
                        x = reader.GetDouble();
                    }
                    else if (propertyName == "Y")
                    {
                        y = reader.GetDouble();
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new VectorDto { X = x, Y = y };
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, VectorDto value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(Math.Round(value.X, 5));
        writer.WriteNumberValue(Math.Round(value.Y, 5));
        writer.WriteEndArray();
    }
}