using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager
{
    internal static class ComponentManager
    {
        private static bool _config;
        private static List<Thread> _threads = new List<Thread>();
        private static readonly List<Form> Windows = new List<Form>();

        internal static void LaunchLevelEditor(string levPath = null)
        {
            StartThread(() =>
            {
                LevelEditor le = levPath != null ? new LevelEditor(levPath) : new LevelEditor();
                AddAndRun(le);
            });
        }

        internal static void LaunchMainForm()
        {
            StartThread(() => AddAndRun(new MainForm()));
        }

        internal static void LaunchReplayManager()
        {
            StartThread(() => AddAndRun(new ReplayManager()));
        }

        internal static void LaunchReplayViewer(Replay replay)
        {
            StartThread(() =>
            {
                ReplayViewer rv = new ReplayViewer(replay);
                AddAndRun(rv);
            });
        }

        internal static void ShowConfiguration(int defaultTabIndex = 0)
        {
            if (!_config)
            {
                _config = true;
                ConfigForm c = new ConfigForm();
                c.TabControl1.SelectTab(defaultTabIndex);
                c.ShowDialog();
                _config = false;
            }
            else
            {
                Utils.ShowError("Configuration window is already open in some other window.");
            }
        }

        internal static void WaitAllThreads()
        {
            while (_threads.Count > 0)
            {
                Thread.Sleep(1);
                Application.DoEvents();
            }
        }

        private static void AddAndRun(Form form)
        {
            Windows.Add(form);
            form.FormClosed += FormClosed;
            Application.Run(form);
        }

        private static void FormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            Windows.Remove((Form) sender);
            if (_threads.Count == 1)
            {
                Global.AppSettings.Save();
            }

            _threads.Remove(Thread.CurrentThread);
        }

        private static void StartThread(ThreadStart stub)
        {
            Thread t = new Thread(stub);
            _threads.Add(t);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        internal static void LaunchLevelManager()
        {
            LevelManager.My.MySettings.Default.LevelDirectory = Global.AppSettings.General.LevelDirectory;
            LevelManager.My.MySettings.Default.ReplayDirectory = Global.AppSettings.General.ReplayDirectory;
            StartThread(() => { AddAndRun(new LevelManager.MainForm()); });
        }
    }
}