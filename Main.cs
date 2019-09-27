using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using Elmanager.Updating;
using My.Resources;

namespace Elmanager
{
    public static class Global
    {
        internal static ElmanagerSettings AppSettings; //TODO Settings should not be global
        internal static DateTime BuildDate;
        internal static Level[] Internals;
        internal static List<string> LevelFiles;
        internal static List<Replay> ReplayDataBase;
        internal static DateTime Version;

        static Global()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            BuildDate = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2);
        }

        /// <summary>
        ///   The entry point of the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // use TLS 1.2; required for GitLab https downloads to work
            }
            catch (NotSupportedException)
            {
            }
            Version = BuildDate;
            Startup(args);
        }

        /// <summary>
        ///   Helper function for serializing internal levels.
        /// </summary>
        internal static void SerializeInternals()
        {
            string[] levfiles = Directory.GetFiles("e:\\intasext", Constants.AllLevs);
            Internals = new Level[55];
            for (int i = 0; i < 55; i++)
            {
                var lev = new Level();
                lev.LoadFromPath(levfiles[i]);
                lev.Path = null;
                lev.Title = "";
                lev.Pictures = null;
                Internals[i] = lev;
            }

            var ms = new FileStream("e:\\intres.dat", FileMode.Create);
            var x = new BinaryFormatter();
            x.Serialize(ms, Internals);
            ms.Close();
        }

        /// <summary>
        ///   Loads internal levels to memory.
        /// </summary>
        /// <param name = "state">Not used.</param>
        private static void LoadInternals(object state)
        {
            using (var ms = new MemoryStream(Resources.IntRes))
            {
                var bf = new BinaryFormatter();
                using (var unzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    Internals = (Level[]) bf.Deserialize(unzip);
                }
            }
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
                        rp.InitializeFrameData();
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
            ThreadPool.QueueUserWorkItem(LoadInternals);
            Application.EnableVisualStyles();
            AppSettings = ElmanagerSettings.Load();
            if (AppSettings.General.CheckForUpdatesOnStartup)
            {
                ThreadPool.QueueUserWorkItem(UpdateChecker.CheckForUpdates);
            }

            ParseCommandLine(args);
            ComponentManager.WaitAllThreads();
        }
    }
}