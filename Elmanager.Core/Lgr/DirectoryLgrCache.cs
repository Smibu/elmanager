using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Elmanager.Lgr;

public class DirectoryLgrCache : ILgrCache
{
    private readonly Func<string?> _lgrDirectoryProvider;
    private readonly Dictionary<string, Lgr> _loadedLgrs = new();
    private string? _lastDirectory;

    public DirectoryLgrCache(Func<string?> lgrDirectoryProvider)
    {
        _lgrDirectoryProvider = lgrDirectoryProvider;
    }

    public DirectoryLgrCache(string? lgrDirectory) : this(() => lgrDirectory)
    {
    }

    public Task<Lgr?> GetOrLoadLgr(string lgrName) => Task.FromResult(TryGetLoaded(lgrName));

    public Lgr? TryGetLoaded(string lgrName)
    {
        var dir = _lgrDirectoryProvider();
        if (!string.Equals(dir, _lastDirectory, StringComparison.OrdinalIgnoreCase))
        {
            _loadedLgrs.Clear();
            _lastDirectory = dir;
        }

        if (dir == null)
        {
            return null;
        }

        var key = lgrName.ToLower();
        if (_loadedLgrs.TryGetValue(key, out var lgr))
        {
            return lgr;
        }

        var path = Path.Combine(dir, key + ".lgr");
        if (!File.Exists(path))
        {
            return null;
        }

        var newLgr = new Lgr(path);
        _loadedLgrs[key] = newLgr;
        return newLgr;
    }
}
