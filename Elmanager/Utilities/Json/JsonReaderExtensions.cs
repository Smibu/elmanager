using System.Text.Json;

namespace Elmanager.Utilities.Json;

internal static class JsonReaderExtensions
{
    public static (int, int) ReadTwoInts(this Utf8JsonReader reader, string separator)
    {
        var hexStr = reader.GetString()?.Split(separator);
        if (hexStr is null || hexStr.Length != 2)
        {
            throw new JsonException($"Expected a string having one '{separator}', got '{hexStr}'");
        }

        try
        {
            return (int.Parse(hexStr[0]), int.Parse(hexStr[1]));
        }
        catch
        {
            throw new JsonException($"Failed to parse integers from string: '{hexStr}'");
        }
    }
}