using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Elmanager.Lev;
using Elmanager.LevelEditor.Playing;
using Elmanager.Rendering;
using Elmanager.Settings;

namespace Elmanager.LevelEditor
{
    internal class LevelEditorSettings
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
}