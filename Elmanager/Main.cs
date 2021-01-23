using Elmanager.Updating;
using My.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elmanager
{
    public static class Global
    {
        internal static ElmanagerSettings AppSettings; //TODO Settings should not be global
        internal static DateTime BuildDate = ThisAssembly.GitCommitDate;
        internal static readonly List<Level> Internals = new();
        private static List<string> _levelFiles;
        internal static DateTime Version;

        internal static List<string> GetLevelFiles()
        {
            return _levelFiles ?? (_levelFiles = Utils.GetLevelFiles(SearchOption.AllDirectories));
        }

        internal static void ResetLevelFiles()
        {
            _levelFiles = null;
        }

        /// <summary>
        ///   The entry point of the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // use TLS 1.2; required for GitLab https downloads to work
            }
            catch (NotSupportedException)
            {
            }
            Version = BuildDate;
            Startup(args);
        }

        /// <summary>
        ///   Loads internal levels to memory.
        /// </summary>
        private static void LoadInternals()
        {
            Internals.Clear();
            using var zip = new ZipArchive(new MemoryStream(Resources.Internals));
            foreach (var entry in zip.Entries)
                Internals.Add(Level.FromStream(entry.Open()));
        }

        private static void ParseCommandLine(IList<string> args)
        {
            if (args.Count == 0)
                ComponentManager.LaunchMainForm();
            else if (args[0] == "/replaymanager")
                ComponentManager.LaunchReplayManager();
            else if (args[0] == "/leveleditor")
                ComponentManager.LaunchLevelEditor();
            else if (args[0] == "/levelmanager")
                ComponentManager.LaunchLevelManager();
            else if (args[0].EndsWith(Constants.LevExtension, StringComparison.OrdinalIgnoreCase))
                ComponentManager.LaunchLevelEditor(args[0]);
            else if (args[0].EndsWith(Constants.RecExtension, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var rp = new Replay(args[0]);
                    if (rp.LevelExists)
                    {
                        ComponentManager.LaunchReplayViewer(rp);
                    }
                    else
                        Utils.ShowError("Could not find level file: " + rp.LevelFilename);
                }
                catch (Exception ex)
                {
                    Utils.ShowError("Error occurred when loading file " + args[0] + ". Exception text: " +
                                    ex.Message);
                }
            }
            else
                Utils.ShowError("Invalid command line argument: " + args[0]);
        }

        private static void Startup(IList<string> args)
        {
            Task.Run(LoadInternals);
            Application.EnableVisualStyles();
            AppSettings = ElmanagerSettings.Load();
            if (AppSettings.General.CheckForUpdatesOnStartup)
            {
                Task.Run(UpdateChecker.CheckForUpdates);
            }

            ParseCommandLine(args);
            Application.Run(ComponentManager.AppCtx);
        }
    }
}