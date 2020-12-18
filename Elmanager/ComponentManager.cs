using Elmanager.Forms;

namespace Elmanager
{
    internal static class ComponentManager
    {
        private static bool _config;

        public static AppContext AppCtx { get; } = new AppContext();

        internal static void LaunchLevelEditor(string levPath = null)
        {
            var le = levPath != null ? new LevelEditor(levPath) : new LevelEditor();
            AppCtx.AddAndShow(le);
        }

        internal static void LaunchMainForm()
        {
            AppCtx.AddAndShow(new MainForm());
        }

        internal static void LaunchReplayManager()
        {
            AppCtx.AddAndShow(new ReplayManager());
        }

        internal static void LaunchReplayViewer(Replay replay)
        {
            var rv = new ReplayViewer();
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
                Utils.ShowError("Configuration window is already open in some other window.");
            }
        }

        internal static void LaunchLevelManager()
        {
            AppCtx.AddAndShow(new LevelManager());
        }
    }
}