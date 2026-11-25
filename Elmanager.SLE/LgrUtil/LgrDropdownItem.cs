namespace Elmanager.SLE.LgrUtil;

public enum LgrSource
{
    Folder,
    Dropped
}

public union LgrAvailability(LgrAvailability.Found, LgrAvailability.NotFound)
{
    public record Found(string KnownName);
    public record NotFound;
}

public sealed record LgrDropdownItem(
    string Filename,
    LgrSource Source,
    LgrAvailability Availability)
{
    public bool IsDropped => Source == LgrSource.Dropped;
    public bool IsFound => Availability is LgrAvailability.Found;

    public string KnownName => Availability switch
    {
        LgrAvailability.Found found => found.KnownName,
        LgrAvailability.NotFound => KnownLgrs.UnknownName
    };
}
