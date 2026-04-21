namespace Elmanager.Lgr;

public record LgrEntry(string Filename, string? Name)
{
    public override string ToString() => Name != null ? $"{Filename} [{Name}]" : Filename;
}

public record TextureEntry(string Name, bool Missing)
{
    public override string ToString() => Missing ? $"{Name} [missing in LGR]" : Name;
}
