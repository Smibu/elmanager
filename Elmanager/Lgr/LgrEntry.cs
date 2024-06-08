namespace Elmanager.Lgr;

internal record LgrEntry(string Filename, string? Name)
{
    public override string ToString() => Name != null ? $"{Filename} [{Name}]" : Filename;
}

internal record TextureEntry(string Name, bool Missing)
{
    public override string ToString() => Missing ? $"{Name} [missing in LGR]" : Name;
}
