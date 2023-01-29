namespace Elmanager.Utilities;

internal static class BoolUtils
{
    internal static string BoolToString(object x)
    {
        return (bool)x ? "Yes" : "No";
    }

    internal static int BoolToInteger(bool b)
    {
        return b ? 1 : 0;
    }
}