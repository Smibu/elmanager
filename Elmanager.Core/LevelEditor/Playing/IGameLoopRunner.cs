using System;
using System.Threading.Tasks;

namespace Elmanager.LevelEditor.Playing;

public interface IGameLoopRunner
{
    Task Run(Func<bool> shouldStop, Action tick);
}
