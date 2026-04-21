using System;

namespace Elmanager.LevelEditor;

public class ImportException : Exception
{
    public ImportException(string message) : base(message)
    {
    }
}
