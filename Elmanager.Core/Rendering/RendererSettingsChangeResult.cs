using System;

namespace Elmanager.Rendering;

public record RendererSettingsChangeResult(bool LgrUpdated, Exception? LgrLoadException = null);
