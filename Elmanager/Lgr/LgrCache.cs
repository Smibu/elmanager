using System.Collections.Generic;

namespace Elmanager.Lgr;

internal class LgrCache
{
    private readonly Dictionary<string, Lgr> _loadedLgrs = new();

    public Lgr GetOrLoadLgr(string path)
    {
        if (_loadedLgrs.TryGetValue(path.ToLower(), out var lgr))
        {
            return lgr;
        }
        var newLgr = new Lgr(path);
        _loadedLgrs[path.ToLower()] = newLgr;
        return newLgr;
    }
}