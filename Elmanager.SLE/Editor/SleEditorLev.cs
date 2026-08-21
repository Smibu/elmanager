using Avalonia.Platform.Storage;
using Elmanager.Lev;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Editor;

public record SleEditorLev(Level Lev, IStorageFile? StorageFile) : IEditorLev
{
    public IEditorLev WithLev(Level lev) => this with { Lev = lev };
}
