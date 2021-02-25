using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using Elmanager.Settings;
using Elmanager.Utilities;

namespace Elmanager.Rendering
{
    internal class RenderingSettings
    {
        private int _circleDrawingAccuracy = 30;
        private double _gridSize = 1.0;
        private string _lgrFile = "";
        private float _lineWidth = 2.0f;
        private int _smoothZoomDuration = 200;
        private double _vertexSize = 0.02;

        public RenderingSettings()
        {
            GroundFillColor = Color.Black;
            GroundEdgeColor = Color.Black;
            GrassEdgeColor = Color.Green;
            AppleColor = Color.Red;
            KillerColor = Color.Black;
            FlowerColor = Color.White;
            StartColor = Color.Green;
            GridColor = Color.White;
            PictureFrameColor = Color.Red;
            TextureFrameColor = Color.Red;
            VertexColor = Color.Red;
            SkyFillColor = Color.LightGray;
            MaxDimensionColor = Color.Blue;
            AppleGravityArrowColor = Color.White;
            ShowGroundEdges = true;
            ShowPictureFrames = false;
            ShowTextureFrames = false;
            ShowObjectFrames = true;
            ShowGravityAppleArrows = true;
            UseCirclesForVertices = false;
            ShowMaxDimensions = false;
            ShowInactiveGrassEdges = false;
            DisableFrameBuffer = false;
        }

        private RenderingSettings(RenderingSettings s)
        {
            GroundFillColor = s.GroundFillColor;
            GroundEdgeColor = s.GroundEdgeColor;
            GrassEdgeColor = s.GrassEdgeColor;
            SkyFillColor = s.SkyFillColor;
            AppleColor = s.AppleColor;
            FlowerColor = s.FlowerColor;
            StartColor = s.StartColor;
            KillerColor = s.KillerColor;
            GridColor = s.GridColor;
            VertexColor = s.VertexColor;
            PictureFrameColor = s.PictureFrameColor;
            TextureFrameColor = s.TextureFrameColor;
            AppleGravityArrowColor = s.AppleGravityArrowColor;
            MaxDimensionColor = s.MaxDimensionColor;
            SmoothZoomDuration = s.SmoothZoomDuration;
            LgrFile = s.LgrFile;
            GridSize = s.GridSize;
            LineWidth = s.LineWidth;
            VertexSize = s.VertexSize;
            SmoothZoomEnabled = s.SmoothZoomEnabled;
            ShowGround = s.ShowGround;
            ShowGroundEdges = s.ShowGroundEdges;
            ShowVertices = s.ShowVertices;
            ShowPictureFrames = s.ShowPictureFrames;
            ShowTextureFrames = s.ShowTextureFrames;
            ShowGrassEdges = s.ShowGrassEdges;
            ShowGrid = s.ShowGrid;
            DefaultGroundAndSky = s.DefaultGroundAndSky;
            GroundTextureEnabled = s.GroundTextureEnabled;
            SkyTextureEnabled = s.SkyTextureEnabled;
            ShowPictures = s.ShowPictures;
            ShowTextures = s.ShowTextures;
            ShowGrass = s.ShowGrass;
            ShowObjectFrames = s.ShowObjectFrames;
            ShowObjects = s.ShowObjects;
            ZoomTextures = s.ZoomTextures;
            CircleDrawingAccuracy = s.CircleDrawingAccuracy;
            ShowObjectCenters = s.ShowObjectCenters;
            ShowGravityAppleArrows = s.ShowGravityAppleArrows;
            UseCirclesForVertices = s.UseCirclesForVertices;
            ShowMaxDimensions = s.ShowMaxDimensions;
            ShowInactiveGrassEdges = s.ShowInactiveGrassEdges;
            DisableFrameBuffer = s.DisableFrameBuffer;
        }

        internal RenderingSettings Clone()
        {
            return new(this);
        }

        [Category("Colors"), DisplayName("Ground fill")]
        public Color GroundFillColor { get; set; }

        [Category("Colors"), DisplayName("Ground edge")]
        public Color GroundEdgeColor { get; set; }

        [Category("Colors"), DisplayName("Grass edge")]
        public Color GrassEdgeColor { get; set; }

        [Category("Colors"), DisplayName("Sky fill")]
        public Color SkyFillColor { get; set; }

        [Category("Colors"), DisplayName("Apple")]
        public Color AppleColor { get; set; }

        [Category("Colors"), DisplayName("Flower")]
        public Color FlowerColor { get; set; }

        [Category("Colors"), DisplayName("Start")]
        public Color StartColor { get; set; }

        [Category("Colors"), DisplayName("Killer")]
        public Color KillerColor { get; set; }

        [Category("Colors"), DisplayName("Grid")]
        public Color GridColor { get; set; }

        [Category("Colors"), DisplayName("Vertex")]
        public Color VertexColor { get; set; }

        [Category("Colors"), DisplayName("Picture frame")]
        public Color PictureFrameColor { get; set; }

        [Category("Colors"), DisplayName("Texture frame")]
        public Color TextureFrameColor { get; set; }

        [Category("Colors"), DisplayName("Apple gravity arrow")]
        public Color AppleGravityArrowColor { get; set; }

        [Category("Colors"), DisplayName("Maximum dimensions")]
        public Color MaxDimensionColor { get; set; }

        [DisplayName("Smooth zoom duration")]
        public int SmoothZoomDuration
        {
            get => _smoothZoomDuration;
            set
            {
                if (value > 0 && value <= 2000)
                    _smoothZoomDuration = value;
                else
                    throw (new ArgumentException("Smooth zoom duration must be in range 1 through 2000!"));
            }
        }

        [DisplayName("Circle drawing accuracy"), Description("The number of vertices used to draw a circle.")]
        public int CircleDrawingAccuracy
        {
            get => _circleDrawingAccuracy;
            set
            {
                if (value >= 3 && value <= 100)
                    _circleDrawingAccuracy = value;
                else
                    throw (new ArgumentException("Circle drawing accuracy must be in range 3 through 100!"));
            }
        }

        [DisplayName("LGR file"), Editor(typeof(CustomFileNameEditor), typeof(UITypeEditor))]
        public string LgrFile
        {
            get => _lgrFile;
            set
            {
                if (value != string.Empty)
                {
                    if (Path.GetExtension(value).CompareWith(".lgr"))
                        _lgrFile = value;
                    else
                        throw (new ArgumentException("The specified file is not LGR file!"));
                }
            }
        }

        [DisplayName("Grid size")]
        public double GridSize
        {
            get => _gridSize;
            set
            {
                if (value > 0)
                    _gridSize = value;
                else
                    throw (new ArgumentException("Grid size must be greater than 0!"));
            }
        }

        [DisplayName("Line width")]
        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                if (value > 0)
                    _lineWidth = value;
                else
                    throw (new ArgumentException("Line width must be greater than 0!"));
            }
        }

        [DisplayName("Vertex size")]
        public double VertexSize
        {
            get => _vertexSize;
            set => _vertexSize = value > 0 ? value : 0.02;
        }

        [DisplayName("Use circles for vertices")]
        public bool UseCirclesForVertices { get; set; }

        [DisplayName("Smooth zoom enabled")]
        public bool SmoothZoomEnabled { get; set; }

        [Category("Visibility"), DisplayName("Ground")]
        public bool ShowGround { get; set; }

        [Category("Visibility"), DisplayName("Center points of objects")]
        public bool ShowObjectCenters { get; set; }

        [Category("Visibility"), DisplayName("Ground edges")]
        public bool ShowGroundEdges { get; set; }

        [Category("Visibility"), DisplayName("Vertices")]
        public bool ShowVertices { get; set; }

        [Category("Visibility"), DisplayName("Picture frames")]
        public bool ShowPictureFrames { get; set; }

        [Category("Visibility"), DisplayName("Texture frames")]
        public bool ShowTextureFrames { get; set; }

        [Category("Visibility"), DisplayName("Grass edges")]
        public bool ShowGrassEdges { get; set; }

        [Category("Visibility"), DisplayName("Grid")]
        public bool ShowGrid { get; set; }

        [DisplayName("Default ground and sky")]
        public bool DefaultGroundAndSky { get; set; }

        [Category("Visibility"), DisplayName("Ground texture")]
        public bool GroundTextureEnabled { get; set; }

        [Category("Visibility"), DisplayName("Sky texture")]
        public bool SkyTextureEnabled { get; set; }

        [Category("Visibility"), DisplayName("Pictures")]
        public bool ShowPictures { get; set; }

        [Category("Visibility"), DisplayName("Textures")]
        public bool ShowTextures { get; set; }

        [Category("Visibility"), DisplayName("Grass (not done yet)")]
        public bool ShowGrass { get; set; }

        [Category("Visibility"), DisplayName("Objects")]
        public bool ShowObjects { get; set; }

        [Category("Visibility"), DisplayName("Object frames")]
        public bool ShowObjectFrames { get; set; }

        [Category("Visibility"), DisplayName("Gravity apple arrows")]
        public bool ShowGravityAppleArrows { get; set; }

        [Category("Visibility"), DisplayName("Maximum dimensions")]
        public bool ShowMaxDimensions { get; set; }

        [Category("Visibility"), DisplayName("Inactive grass edges")]
        public bool ShowInactiveGrassEdges { get; set; }

        [DisplayName("Zoom textures")]
        public bool ZoomTextures { get; set; }

        [Category("Workarounds"), DisplayName("Disable framebuffer usage")]
        public bool DisableFrameBuffer { get; set; }

        public bool ShowObjectsOrFrames => ShowObjects || ShowObjectFrames;
        public bool ShowGrassOrEdges => ShowGrassEdges; // ShowGrass is not implemented.
        public bool ShowGroundOrEdges => ShowGround || ShowGroundEdges;
    }
}