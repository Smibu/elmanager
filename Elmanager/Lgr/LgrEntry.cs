namespace Elmanager.Lgr;

internal record LgrEntry(string Filename, string? Name)
{
    public override string ToString() => Name != null ? $"{Filename} [{Name}]" : Filename;
}