using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Properties;
using Elmanager.Rec;
using Elmanager.Settings;
using Elmanager.UI;
using Elmanager.Updating;

namespace Elmanager.Application;

internal static class Global
{
    internal static ElmanagerSettings AppSettings = null!; //TODO Settings should not be global
    internal static readonly List<Level> Internals = new();
    private static List<string>? _levelFiles;
    internal static DateTime Version = new(2023, 5, 5);

    internal static List<string> GetLevelFiles()
    {
        return _levelFiles ??= DirUtils.GetLevelFiles(SearchOption.AllDirectories);
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
        else if (args[0].EndsWith(DirUtils.LevExtension, StringComparison.OrdinalIgnoreCase))
            ComponentManager.LaunchLevelEditor(args[0]);
        else if (args[0].EndsWith(DirUtils.RecExtension, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var rp = Replay.FromPath(args[0]);
                if (rp.Obj.LevelExists)
                {
                    ComponentManager.LaunchReplayViewer(rp);
                }
                else
                    UiUtils.ShowError("Could not find level file: " + rp.Obj.LevelFilename);
            }
            catch (Exception ex)
            {
                UiUtils.ShowError("Error occurred when loading file " + args[0] + ". Exception text: " +
                                  ex.Message);
            }
        }
        else
            UiUtils.ShowError("Invalid command line argument: " + args[0]);
    }

    private static void Startup(IList<string> args)
    {
        Task.Run(LoadInternals);
        ApplicationConfiguration.Initialize();
        AppSettings = ElmanagerSettings.Load();
        if (AppSettings.General.CheckForUpdatesOnStartup)
        {
            Task.Run(UpdateChecker.CheckForUpdates);
        }

        ParseCommandLine(args);
        System.Windows.Forms.Application.Run(ComponentManager.AppCtx);
    }
}