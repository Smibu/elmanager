using Avalonia.Platform.Storage;

namespace Elmanager.SLE.Platform;

public static class LevelFileTypes
{
    public static readonly FilePickerFileType LevType =
        new("Elasto Mania level") { Patterns = ["*.lev"], MimeTypes = ["application/x-elma-level"] };
}
