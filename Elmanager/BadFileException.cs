using System;

namespace Elmanager
{
    internal class BadFileException : Exception
    {
        public BadFileException(string message)
            : base(message)
        {
        }
    }
}