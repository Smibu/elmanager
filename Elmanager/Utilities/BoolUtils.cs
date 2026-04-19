namespace Elmanager.Utilities;

internal static class BoolUtils
{
    internal static string BoolToString(object x)
    {
        return (bool)x ? "Yes" : "No";
    }
}
