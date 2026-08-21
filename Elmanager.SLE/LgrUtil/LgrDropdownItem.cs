namespace Elmanager.SLE.LgrUtil;

public enum LgrSource
{
    Folder,
    Dropped
}

public enum LgrAvailability
{
    Found,
    NotFound
}

public sealed record LgrDropdownItem(
    string Filename,
    string KnownName,
    LgrSource Source,
    LgrAvailability Availability)
{
    public bool IsDropped => Source == LgrSource.Dropped;
    public bool IsFound => Availability == LgrAvailability.Found;
}
