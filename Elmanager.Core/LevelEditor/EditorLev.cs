using System;
using Elmanager.IO;
using Elmanager.Lev;

namespace Elmanager.LevelEditor;

public interface IEditorLev
{
    Level Lev { get; }
    IEditorLev WithLev(Level lev);
}

public record EditorLev(Level Lev, ElmaFile? File) : IEditorLev
{
    public IEditorLev WithLev(Level lev) => this with { Lev = lev };

    public static TEditorLev CreateBlankLevel<TEditorLev>(ILevelEditor levelEditor, Func<Level, TEditorLev> factory) where TEditorLev : IEditorLev
    {
        var settings = levelEditor.Settings;
        var lev = settings.GetTemplateLevel();
        if (!settings.UseFilenameForTitle)
            lev.Title = settings.DefaultTitle;
        return factory(lev);
    }
}
