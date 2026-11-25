using System;

namespace Elmanager.Rendering;

public union RendererSettingsChangeResult(
    RendererSettingsChangeResult.LgrUnchanged,
    RendererSettingsChangeResult.LgrUpdated,
    RendererSettingsChangeResult.LgrLoadFailed)
{
    public record LgrUnchanged;
    public record LgrUpdated;
    public record LgrLoadFailed(Exception Exception);
}
