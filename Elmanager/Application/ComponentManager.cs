using Elmanager.IO;
using Elmanager.LevelEditor;
using Elmanager.LevelManager;
using Elmanager.Rec;
using Elmanager.ReplayManager;
using Elmanager.ReplayViewer;
using Elmanager.Settings;
using Elmanager.UI;

namespace Elmanager.Application;

internal static class ComponentManager
{
    private static bool _config;

    public static AppContext AppCtx { get; } = new();

    internal static void LaunchLevelEditor(string? levPath = null)
    {
        var le = new LevelEditorForm(levPath);
        AppCtx.AddAndShow(le);
    }

    internal static void LaunchMainForm()
    {
        AppCtx.AddAndShow(new MainForm());
    }

    internal static void LaunchReplayManager()
    {
        AppCtx.AddAndShow(new ReplayManagerForm());
    }

    internal static async void LaunchReplayViewer(ElmaFileObject<Replay> replay)
    {
        var rv = new ReplayViewerForm();
        rv.Show();
        await rv.WaitInit();
        rv.SetReplays(replay);
        AppCtx.AddAndShow(rv);
    }

    internal static void ShowConfiguration(string defaultTab = "general")
    {
        if (!_config)
        {
            _config = true;
            var c = new ConfigForm();
            c.TabControl1.SelectTab($"{defaultTab}Tab");
            c.ShowDialog();
            _config = false;
        }
        else
        {
            UiUtils.ShowError("Configuration window is already open in some other window.");
        }
    }

    internal static void LaunchLevelManager()
    {
        AppCtx.AddAndShow(new LevelManagerForm());
    }
}