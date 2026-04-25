using System.Windows.Forms;
using Elmanager.Settings;

namespace Elmanager.UI;

internal static class WindowStateExtensions
{
    public static WindowState ToSettingsWindowState(this FormWindowState state) =>
        state switch
        {
            FormWindowState.Minimized => WindowState.Minimized,
            FormWindowState.Maximized => WindowState.Maximized,
            _ => WindowState.Normal
        };

    public static FormWindowState ToFormWindowState(this WindowState state) =>
        state switch
        {
            WindowState.Minimized => FormWindowState.Minimized,
            WindowState.Maximized => FormWindowState.Maximized,
            _ => FormWindowState.Normal
        };
}
