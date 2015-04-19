using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using My.Resources;

namespace Elmanager
{
    public static class Global
    {
        internal static readonly PrivateFontCollection ElmaFonts = new PrivateFontCollection();
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
            //SerializeInternals();
            //return;
//            var txt = File.ReadAllText(@"D:\Archives\em_changelog.txt");
//            var r = new Regex(@"\*\*\*\*\*\*\*\*\*\*\r\n\d+\.\d+\.\d+\r\n\*\*\*\*\*\*\*\*\*\*\r\n[^*]+", RegexOptions.Singleline);
//            var matches=r.Matches(txt);
//            int i = matches.Count - 1;
//            using(var f=new StreamWriter(@"D:\em_changelog_reversed.txt",false))
//            {
//                while (i>=0)
//                f.Write(matches[i--]);
//            }
//            return;
            Version = BuildDate;
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;
            var controller = new SingleInstanceController();
            controller.Run(args);
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

                //for zipping
//                using (var f = new FileStream(@"d:\compressed.dat", FileMode.Create))
//                {
//                    using (var compress = new GZipStream(f, CompressionMode.Compress))
//                    {
//                        compress.Write(Resources.IntRes,0,Resources.IntRes.Length);
//                        //f.Close();
//                    }
//                }
            }
        }

        private static void ParseCommandLine(ReadOnlyCollection<string> args)
        {
            if (args.Count == 0)
                ComponentManager.LaunchMainForm();
            else if (args[0] == "/replaymanager")
                ComponentManager.LaunchReplayManager();
            else if (args[0] == "/leveleditor")
                ComponentManager.LaunchLevelEditor();
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

        private static void Startup(ReadOnlyCollection<string> args)
        {
            ThreadPool.QueueUserWorkItem(LoadInternals);
            Application.EnableVisualStyles();
            AppSettings = ElmanagerSettings.Load();
            if (AppSettings.General.CheckForUpdatesOnStartup)
            {
                ThreadPool.QueueUserWorkItem(Utils.CheckForUpdates);
            }
            ParseCommandLine(args);
            ComponentManager.WaitAllThreads();
        }

        private class SingleInstanceController : WindowsFormsApplicationBase
        {
            public SingleInstanceController()
            {
                IsSingleInstance = false; //disable single instance mode because there would be some bugs with it
                StartupNextInstance += ThisStartupNextInstance;
            }

            protected override void OnRun()
            {
                Global.Startup(CommandLineArgs);
            }

            private void ThisStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
            {
                ParseCommandLine(e.CommandLine);
            }
        }
    }
}