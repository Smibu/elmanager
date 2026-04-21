namespace Elmanager.Utilities;

public static class BoolUtils
{
    public static string BoolToString(object x)
    {
        return (bool)x ? "Yes" : "No";
    }
}
