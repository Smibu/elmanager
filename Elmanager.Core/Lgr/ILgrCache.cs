using System.Threading.Tasks;

namespace Elmanager.Lgr;

public interface ILgrCache
{
    Task<Lgr?> GetOrLoadLgr(string lgrName);

    Lgr? TryGetLoaded(string lgrName);
}
