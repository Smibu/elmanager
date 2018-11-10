using System;

namespace Elmanager
{
    internal class SettingsException : Exception
    {
        public SettingsException(string message) : base(message)
        {
        }
    }
}