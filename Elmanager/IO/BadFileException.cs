using System;

namespace Elmanager.IO
{
    internal class BadFileException : Exception
    {
        public BadFileException(string message)
            : base(message)
        {
        }
    }
}