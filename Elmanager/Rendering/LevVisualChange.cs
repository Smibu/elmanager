using System;

namespace Elmanager.Rendering;

[Flags]
internal enum LevVisualChange
{
    Nothing = 0,
    Ground = 0x1,
    Grass = 0x2,
    Apples = 0x4,
    Killers = 0x8,
    Flowers = 0x10,
    Pictures = 0x20,
    Textures = 0x40,
    Start = 0x80,
    Objects = Apples | Killers | Flowers | Start,
    GraphicElements = Pictures | Textures,
    All = Ground | Grass | Objects | GraphicElements,
}
