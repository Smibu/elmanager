using System;

namespace Elmanager.LevelEditor;

[Flags]
internal enum LevModification
{
    Nothing = 0,
    Ground = 1,
    Objects = 2,
    Decorations = 4,
}