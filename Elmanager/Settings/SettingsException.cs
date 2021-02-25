using System;

namespace Elmanager.Settings
{
    internal class SettingsException : Exception
    {
        public SettingsException(string message) : base(message)
        {
        }
    }
}