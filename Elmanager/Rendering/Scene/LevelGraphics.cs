using System;

namespace Elmanager.Rendering.Scene;

internal record LgrGraphics(
    OpenGlLgr Lgr,
    Pictures Pictures,
    Textures Textures,
    Grass Grass,
    Players Players) : IDisposable
{
    public void Dispose()
    {
        Pictures.Dispose();
        Textures.Dispose();
        Grass.Dispose();
        Players.Dispose();
    }
}

internal record LevelGraphics(
    GroundSky GroundSky,
    Objects Objects,
    ObjectFrames ObjectFrames,
    PolygonFrames PolygonFrames,
    GraphicElementFrames GraphicElementFrames,
    Lines Lines,
    Selection Selection,
    LgrGraphics? LgrGraphics) : IDisposable
{
    public void Dispose()
    {
        GroundSky.Dispose();
        Objects.Dispose();
        ObjectFrames.Dispose();
        PolygonFrames.Dispose();
        GraphicElementFrames.Dispose();
        Lines.Dispose();
        Selection.Dispose();
        LgrGraphics?.Dispose();
    }
}
