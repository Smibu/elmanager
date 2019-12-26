using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager
{
    [Serializable]
    public class ElmanagerSettings
    {
        private const string SettingsFileDateFormat = "ddMMyyyy";
        private const string SettingsFileBaseName = "Elmanager";

        private static string _settingsFile =
            SettingsFileBaseName + Global.Version.ToString(SettingsFileDateFormat) + ".dat";

        public GeneralSettings General = new GeneralSettings();
        public LevelEditorSettings LevelEditor = new LevelEditorSettings();
        public ReplayManagerSettings ReplayManager = new ReplayManagerSettings();
        public LevelManagerSettings LevelManager = new LevelManagerSettings();
        public ReplayViewerSettings ReplayViewer = new ReplayViewerSettings();

        public static string ElmanagerFolder => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static ElmanagerSettings Load()
        {
            if (File.Exists(Path.Combine(ElmanagerFolder, _settingsFile)))
            {
                return GetSettings(_settingsFile);
            }

            var oldSettingFiles = Directory.GetFiles(ElmanagerFolder, "Elmanager*.dat");
            try
            {
                if (oldSettingFiles.Length > 0)
                {
                    var oldFileDate = oldSettingFiles.Select(
                            path =>
                                DateTime.ParseExact(
                                    Path.GetFileNameWithoutExtension(path).Substring(SettingsFileBaseName.Length),
                                    SettingsFileDateFormat, CultureInfo.InvariantCulture))
                        .Max()
                        .ToString(SettingsFileDateFormat);
                    return GetSettings(Path.Combine(ElmanagerFolder,
                        SettingsFileBaseName + oldFileDate + ".dat"));
                }
            }
            catch (Exception)
            {
                Utils.ShowError("Could not load old settings. You need to set them again.");
            }

            return new ElmanagerSettings();
        }

        public void Save()
        {
            var appSettingsFile = new FileStream(Path.Combine(ElmanagerFolder, _settingsFile), FileMode.Create);
            var binFormatter = new BinaryFormatter();
            binFormatter.Serialize(appSettingsFile, this);
            appSettingsFile.Close();
        }

        private static ElmanagerSettings GetSettings(string settingsFile)
        {
            var appSettingsFile = new FileStream(Path.Combine(ElmanagerFolder, settingsFile), FileMode.Open);
            var binFormatter = new BinaryFormatter();
            var loadedSettings = (ElmanagerSettings) (binFormatter.Deserialize(appSettingsFile));
            appSettingsFile.Close();
            if (loadedSettings.ReplayViewer.ZoomLevel <= 0)
            {
                loadedSettings.ReplayViewer.ZoomLevel = 5.0;
            }

            if (loadedSettings.LevelManager == null)
            {
                loadedSettings.LevelManager = new LevelManagerSettings();
            }

            return loadedSettings;
        }

        [Serializable]
        public class GeneralSettings
        {
            public bool CheckForUpdatesOnStartup = true;
            public string LevelDirectory = string.Empty;
            public string LgrDirectory = string.Empty;
            public string ReplayDirectory = string.Empty;
        }

        [Serializable]
        public class LevelEditorSettings
        {
            public double AutoGrassThickness = 0.2;
            public string BaseFilename = "MyLev";
            public double CaptureRadius = 0.015;
            public bool CheckTopologyDynamically;
            public bool CheckTopologyWhenSaving;
            public string DefaultTitle = "New level";
            public double DrawStep = 1.0;
            public int EllipseSteps = 10;
            public double FrameRadius = 0.2;
            public Color CrosshairColor = Color.Blue;
            public Color HighlightColor = Color.Yellow;
            public double InitialHeight = 50.0;
            public double InitialWidth = 50.0;
            public string LastLevel;
            public int MouseClickStep = 50;
            public string NumberFormat = "0";
            public double PipeRadius = 1.0;
            public RenderingSettings RenderingSettings = new RenderingSettings();
            public Color SelectionColor = Color.Blue;
            public Size Size = new Size(800, 600);
            public bool SnapToGrid;
            public bool ShowCrossHair;
            public int SmoothSteps = 3;
            public int SmoothVertexOffset = 50;
            public double UnsmoothAngle = 10;
            public double UnsmoothLength = 1.0;
            public bool UseFilenameForTitle;
            public bool UseFilenameSuggestion;
            public bool UseHighlight = true;
            public FormWindowState WindowState = FormWindowState.Normal;
            public string LevelTemplate = "50,50";
            public bool CapturePicturesAndTexturesFromBordersOnly = false;
            public bool AlwaysSetDefaultsInPictureTool = false;

            internal static Level TryGetTemplateLevel(string text)
            {
                if (text == null)
                {
                    throw new SettingsException("The level template is null.");
                }

                if (File.Exists(text))
                {
                    try
                    {
                        var template = Level.FromPath(text);
                        template.Path = null;
                        return template;
                    }
                    catch (Exception)
                    {
                        throw new SettingsException("The level template file is not a valid Elma level file.");
                    }
                }

                var regex = new Regex(@"^(\d+),(\d+)$");
                if (!regex.IsMatch(text))
                {
                    throw new SettingsException(
                        "The level template is neither a file nor a string of the form \"width,height\".");
                }

                double width = int.Parse(regex.Match(text).Groups[1].Value);
                double height = int.Parse(regex.Match(text).Groups[2].Value);
                return Level.FromDimensions(width, height);
            }

            internal Level GetTemplateLevel()
            {
                try
                {
                    return TryGetTemplateLevel(LevelTemplate);
                }
                catch (SettingsException)
                {
                    return Level.FromDimensions(50, 50);
                }
            }
        }

        [Serializable]
        public class ManagerSettings
        {
            public bool ConfirmDelete = true;
            public Point Location;
            public byte[] ListState;
            public string SearchPattern = string.Empty;
            public SearchOption RecDirSearchOption = SearchOption.AllDirectories;
            public bool ShowGridInList = true;
            public Size Size = new Size(800, 600);
            public FormWindowState WindowState = FormWindowState.Normal;
            public bool ShowTooltipInList = true;
            public SearchOption LevDirSearchOption = SearchOption.AllDirectories;

            public void SaveGui(IManagerGui m)
            {
                var f = m.Form;
                var list = m.ObjectList;
                Location = f.Location;
                Size = f.Size;
                WindowState = f.WindowState;
                ShowGridInList = list.GridLines;
                ListState = list.SaveState();
                SearchPattern = m.SearchPattern;
            }

            public void RestoreGui(IManagerGui m)
            {
                var f = m.Form;
                var list = m.ObjectList;
                f.Location = Location;
                f.Size = Size;
                f.WindowState = WindowState;
                list.GridLines = ShowGridInList;
                m.SearchPattern = SearchPattern;
                if (ListState != null)
                {
                    list.RestoreState(ListState);
                }
            }
        }

        [Serializable]
        public class ReplayManagerSettings : ManagerSettings
        {
            public bool Decimal3Shown;
            public string Nickname = "Nick";
            public bool NitroReplays;
            public string Pattern = "LNT";

            public int NumDecimals => Decimal3Shown ? 3 : 2;

        }

        [Serializable]
        public class LevelManagerSettings : ManagerSettings
        {
        }

        [Serializable]
        public class ReplayViewerSettings
        {
            public Color ActivePlayerColor = Color.Black;
            public bool DontSelectPlayersByDefault;
            public bool DrawOnlyPlayerFrames = true;
            public bool DrawTransparentInactive = true;
            public bool FollowDriver = true;
            public double FrameStep = 0.02;
            public bool HideStartObject = true;
            public Color InactivePlayerColor = Color.Green;
            public bool LockedCamera;
            public bool LoopPlaying;
            public int MouseClickStep = 50;
            public int MouseWheelStep = 50;
            public bool MultiSpy;
            public bool PicturesInBackGround;
            public RenderingSettings RenderingSettings = new RenderingSettings();
            public bool ShowBikeCoords = true;
            public bool ShowDriverPath;
            public Size Size = new Size(800, 600);
            public FormWindowState WindowState = FormWindowState.Normal;
            public double ZoomLevel = 5.0;
            public bool FollowAlsoWhenZooming = false;
        }
    }
}