using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Elmanager.Forms;
using Elmanager.LevEditor.Playing;
using ColorConverter = Elmanager.Utilities.Json.ColorConverter;
using PointConverter = Elmanager.Utilities.Json.PointConverter;
using SizeConverter = Elmanager.Utilities.Json.SizeConverter;

namespace Elmanager
{
    internal class ElmanagerSettings
    {
        private const string SettingsFileDateFormat = "ddMMyyyy";
        private const string SettingsFileBaseName = "ElmanagerSettings";

        private static readonly string _settingsFile =
            SettingsFileBaseName + Global.Version.ToString(SettingsFileDateFormat) + ".json";

        [JsonInclude]
        public GeneralSettings General = new();
        [JsonInclude]
        public LevelEditorSettings LevelEditor = new();
        [JsonInclude]
        public ReplayManagerSettings ReplayManager = new();
        [JsonInclude]
        public LevelManagerSettings LevelManager = new();
        [JsonInclude]
        public ReplayViewerSettings ReplayViewer = new();

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true,
            IgnoreReadOnlyProperties = true,
            Converters = { new ColorConverter(), new SizeConverter(), new PointConverter() }
        };

        public static string ElmanagerFolder => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static ElmanagerSettings Load()
        {
            if (File.Exists(Path.Combine(ElmanagerFolder, _settingsFile)))
            {
                return GetSettings(_settingsFile);
            }

            var oldSettingFiles = Directory.GetFiles(ElmanagerFolder, "ElmanagerSettings*.json");
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
                        SettingsFileBaseName + oldFileDate + ".json"));
                }
            }
            catch (Exception)
            {
                Utils.ShowError("Could not load old settings. You need to set them again.");
            }

            return new ElmanagerSettings();
        }

        public async Task Save()
        {
            await using var appSettingsFile = new FileStream(Path.Combine(ElmanagerFolder, _settingsFile), FileMode.Create);
            await JsonSerializer.SerializeAsync(appSettingsFile, this, JsonSerializerOptions);
        }

        private static ElmanagerSettings GetSettings(string settingsFile)
        {
            var path = Path.Combine(ElmanagerFolder, settingsFile);
            try
            {
                var loadedSettings = JsonSerializer.Deserialize<ElmanagerSettings>(File.ReadAllText(path), JsonSerializerOptions);
                return loadedSettings;
            }
            catch (JsonException e)
            {
                Utils.ShowError(e.Message);
                return new ElmanagerSettings();
            }
        }

        public class GeneralSettings
        {
            public bool CheckForUpdatesOnStartup { get; set; } = true;
            public string LevelDirectory { get; set; } = string.Empty;
            public string LgrDirectory { get; set; } = string.Empty;
            public string ReplayDirectory { get; set; } = string.Empty;
        }

        public class LevelEditorSettings
        {
            public double AutoGrassThickness { get; set; } = 0.2;
            public string BaseFilename { get; set; } = "MyLev";
            public double CaptureRadius { get; set; } = 0.015;
            public bool CheckTopologyDynamically { get; set; }
            public bool CheckTopologyWhenSaving { get; set; }
            public string DefaultTitle { get; set; } = "New level";
            public double DrawStep { get; set; } = 1.0;
            public int EllipseSteps { get; set; } = 10;
            public double FrameRadius { get; set; } = 0.2;
            public Color CrosshairColor { get; set; } = Color.Blue;
            public Color HighlightColor { get; set; } = Color.Yellow;
            public double InitialHeight { get; set; } = 50.0;
            public double InitialWidth { get; set; } = 50.0;
            public string LastLevel { get; set; }
            public int MouseClickStep { get; set; } = 50;
            public string NumberFormat { get; set; } = "0";
            public double PipeRadius { get; set; } = 1.0;
            public RenderingSettings RenderingSettings { get; set; } = new();
            public Color SelectionColor { get; set; } = Color.Blue;
            public Size Size { get; set; } = new(800, 600);
            public bool SnapToGrid { get; set; }
            public bool ShowCrossHair { get; set; }
            public int SmoothSteps { get; set; } = 3;
            public int SmoothVertexOffset { get; set; } = 50;
            public double UnsmoothAngle { get; set; } = 10;
            public double UnsmoothLength { get; set; } = 1.0;
            public bool UseFilenameForTitle { get; set; }
            public bool UseFilenameSuggestion { get; set; }
            public bool UseHighlight { get; set; } = true;
            public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
            public string LevelTemplate { get; set; } = "50,50";
            public bool CapturePicturesAndTexturesFromBordersOnly { get; set; } = false;
            public bool AlwaysSetDefaultsInPictureTool { get; set; } = false;
            public PlaySettings PlayingSettings { get; set; } = new();

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

        public class ManagerSettings
        {
            public bool ConfirmDelete { get; set; } = true;
            public Point Location { get; set; }
            public byte[] ListState { get; set; }
            public string SearchPattern { get; set; } = string.Empty;
            public SearchOption RecDirSearchOption { get; set; } = SearchOption.AllDirectories;
            public bool ShowGridInList { get; set; } = true;
            public Size Size { get; set; } = new(800, 600);
            public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
            public bool ShowTooltipInList { get; set; } = true;
            public SearchOption LevDirSearchOption { get; set; } = SearchOption.AllDirectories;

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

        public class ReplayManagerSettings : ManagerSettings
        {
            public bool Decimal3Shown { get; set; }
            public string Nickname { get; set; } = "Nick";
            public bool NitroReplays { get; set; }
            public string Pattern { get; set; } = "LNT";

            public int NumDecimals => Decimal3Shown ? 3 : 2;

        }

        public class LevelManagerSettings : ManagerSettings
        {
        }

        public class ReplayViewerSettings
        {
            public Color ActivePlayerColor { get; set; } = Color.Black;
            public bool DontSelectPlayersByDefault { get; set; }
            public bool DrawOnlyPlayerFrames { get; set; } = true;
            public bool DrawTransparentInactive { get; set; } = true;
            public bool FollowDriver { get; set; } = true;
            public double FrameStep { get; set; } = 0.02;
            public bool HideStartObject { get; set; } = true;
            public Color InactivePlayerColor { get; set; } = Color.Green;
            public bool LockedCamera { get; set; }
            public bool LoopPlaying { get; set; }
            public int MouseClickStep { get; set; } = 50;
            public int MouseWheelStep { get; set; } = 50;
            public bool MultiSpy { get; set; }
            public bool PicturesInBackGround { get; set; }
            public RenderingSettings RenderingSettings { get; set; } = new();
            public bool ShowBikeCoords { get; set; } = true;
            public bool ShowDriverPath { get; set; }
            public Size Size { get; set; } = new(800, 600);
            public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
            public double ZoomLevel { get; set; } = 5.0;
            public bool FollowAlsoWhenZooming { get; set; } = false;
        }
    }
}