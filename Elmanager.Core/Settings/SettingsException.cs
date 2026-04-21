using System;

namespace Elmanager.Settings;

public class SettingsException : Exception
{
    public SettingsException(string message) : base(message)
    {
    }
}
