using System;

namespace Elmanager.LevelEditor;

internal class ImportException : Exception
{
    public ImportException(string message) : base(message)
    {
    }
}