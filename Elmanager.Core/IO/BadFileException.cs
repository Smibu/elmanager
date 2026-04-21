using System;

namespace Elmanager.IO;

public class BadFileException : Exception
{
    public BadFileException(string message)
        : base(message)
    {
    }
}
