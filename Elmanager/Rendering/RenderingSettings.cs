﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Elmanager.Application;
using Elmanager.Lev;
using System.Text.Json.Serialization;

namespace Elmanager.Rendering;

internal class RenderingSettings
{
    private int _circleDrawingAccuracy = 30;
    private double _gridSize = 1.0;
    private float _lineWidth = 2.0f;
    private int _smoothZoomDuration = 200;
    private double _vertexSize = 0.02;
    private double _grassZoom = 1.0;
    protected const string TransparencyTip = "For transparency, add 4th value at the start, for example: 80, 255, 255, 255";

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

    protected RenderingSettings(RenderingSettings s)
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
        GrassZoom = s.GrassZoom;
    }

    internal virtual RenderingSettings Clone() => new(this);

    [Category("Colors"), DisplayName("Ground fill"), JsonPropertyName("GroundFillColor")]
    public Color GroundFillColor { get; set; }

    [Category("Colors"), DisplayName("Ground edge"), JsonPropertyName("GroundEdgeColor"), Description(TransparencyTip)]
    public Color GroundEdgeColor { get; set; }

    [Category("Colors"), DisplayName("Grass edge"), JsonPropertyName("GrassEdgeColor"), Description(TransparencyTip)]
    public Color GrassEdgeColor { get; set; }

    [Category("Colors"), DisplayName("Sky fill"), JsonPropertyName("SkyFillColor")]
    public Color SkyFillColor { get; set; }

    [Category("Colors"), DisplayName("Apple"), JsonPropertyName("AppleColor"), Description(TransparencyTip)]
    public Color AppleColor { get; set; }

    [Category("Colors"), DisplayName("Flower"), JsonPropertyName("FlowerColor"), Description(TransparencyTip)]
    public Color FlowerColor { get; set; }

    [Category("Colors"), DisplayName("Start"), JsonPropertyName("StartColor"), Description(TransparencyTip)]
    public Color StartColor { get; set; }

    [Category("Colors"), DisplayName("Killer"), JsonPropertyName("KillerColor"), Description(TransparencyTip)]
    public Color KillerColor { get; set; }

    [Category("Colors"), DisplayName("Grid"), JsonPropertyName("GridColor"), Description(TransparencyTip)]
    public Color GridColor { get; set; }

    [Category("Colors"), DisplayName("Vertex"), JsonPropertyName("VertexColor"), Description(TransparencyTip)]
    public Color VertexColor { get; set; }

    [Category("Colors"), DisplayName("Picture frame"), JsonPropertyName("PictureFrameColor"), Description(TransparencyTip)]
    public Color PictureFrameColor { get; set; }

    [Category("Colors"), DisplayName("Texture frame"), JsonPropertyName("TextureFrameColor"), Description(TransparencyTip)]
    public Color TextureFrameColor { get; set; }

    [Category("Colors"), DisplayName("Apple gravity arrow"), JsonPropertyName("AppleGravityArrowColor"), Description(TransparencyTip)]
    public Color AppleGravityArrowColor { get; set; }

    [Category("Colors"), DisplayName("Maximum dimensions"), JsonPropertyName("MaxDimensionColor"), Description(TransparencyTip)]
    public Color MaxDimensionColor { get; set; }

    [DisplayName("Smooth zoom duration"), JsonPropertyName("SmoothZoomDuration")]
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

    [DisplayName("Circle drawing accuracy"), Description("The number of vertices used to draw a circle."), JsonPropertyName("CircleDrawingAccuracy")]
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

    [DisplayName("Grid size"), JsonPropertyName("GridSize")]
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

    [DisplayName("Line width"), JsonPropertyName("LineWidth")]
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

    [DisplayName("Vertex size"), JsonPropertyName("VertexSize")]
    public double VertexSize
    {
        get => _vertexSize;
        set => _vertexSize = value > 0 ? value : 0.02;
    }

    [DisplayName("Use circles for vertices"), JsonPropertyName("UseCirclesForVertices")]
    public bool UseCirclesForVertices { get; set; }

    [DisplayName("Smooth zoom enabled"), JsonPropertyName("SmoothZoomEnabled")]
    public bool SmoothZoomEnabled { get; set; }

    [Category("Visibility"), DisplayName("Ground"), JsonPropertyName("ShowGround")]
    public bool ShowGround { get; set; }

    [Category("Visibility"), DisplayName("Center points of objects"), JsonPropertyName("ShowObjectCenters")]
    public bool ShowObjectCenters { get; set; }

    [Category("Visibility"), DisplayName("Ground edges"), JsonPropertyName("ShowGroundEdges")]
    public bool ShowGroundEdges { get; set; }

    [Category("Visibility"), DisplayName("Vertices"), JsonPropertyName("ShowVertices")]
    public bool ShowVertices { get; set; }

    [Category("Visibility"), DisplayName("Picture frames"), JsonPropertyName("ShowPictureFrames")]
    public bool ShowPictureFrames { get; set; }

    [Category("Visibility"), DisplayName("Texture frames"), JsonPropertyName("ShowTextureFrames")]
    public bool ShowTextureFrames { get; set; }

    [Category("Visibility"), DisplayName("Grass edges"), JsonPropertyName("ShowGrassEdges")]
    public bool ShowGrassEdges { get; set; }

    [Category("Visibility"), DisplayName("Grid"), JsonPropertyName("ShowGrid")]
    public bool ShowGrid { get; set; }

    [DisplayName("Default ground and sky"), JsonPropertyName("DefaultGroundAndSky")]
    public bool DefaultGroundAndSky { get; set; }

    [Category("Visibility"), DisplayName("Ground texture"), JsonPropertyName("GroundTextureEnabled")]
    public bool GroundTextureEnabled { get; set; }

    [Category("Visibility"), DisplayName("Sky texture"), JsonPropertyName("SkyTextureEnabled")]
    public bool SkyTextureEnabled { get; set; }

    [Category("Visibility"), DisplayName("Pictures"), JsonPropertyName("ShowPictures")]
    public bool ShowPictures { get; set; }

    [Category("Visibility"), DisplayName("Textures"), JsonPropertyName("ShowTextures")]
    public bool ShowTextures { get; set; }

    [Category("Visibility"), DisplayName("Grass"), JsonPropertyName("ShowGrass")]
    public bool ShowGrass { get; set; }

    [Category("Visibility"), DisplayName("Objects"), JsonPropertyName("ShowObjects")]
    public bool ShowObjects { get; set; }

    [Category("Visibility"), DisplayName("Object frames"), JsonPropertyName("ShowObjectFrames")]
    public bool ShowObjectFrames { get; set; }

    [Category("Visibility"), DisplayName("Gravity apple arrows"), JsonPropertyName("ShowGravityAppleArrows")]
    public bool ShowGravityAppleArrows { get; set; }

    [Category("Visibility"), DisplayName("Maximum dimensions"), JsonPropertyName("ShowMaxDimensions")]
    public bool ShowMaxDimensions { get; set; }

    [Category("Visibility"), DisplayName("Inactive grass edges"), JsonPropertyName("ShowInactiveGrassEdges")]
    public bool ShowInactiveGrassEdges { get; set; }

    [DisplayName("Zoom textures"), JsonPropertyName("ZoomTextures")]
    public bool ZoomTextures { get; set; } = true;

    [Category("Workarounds"), DisplayName("Disable framebuffer usage"), JsonPropertyName("DisableFrameBuffer")]
    public bool DisableFrameBuffer { get; set; }

    [Browsable(false)]
    public bool ShowObjectsOrFrames => ShowObjects || ShowObjectFrames;

    [Browsable(false)]
    public bool ShowGrassOrEdges => ShowGrassEdges;

    [Browsable(false)]
    public bool ShowGroundOrEdges => ShowGround || ShowGroundEdges;

    [DisplayName("Grass zoom"),
     Description("Grass detail level. Set this the same as eolconf zoom to make grass look the same as in EOL."),
     JsonPropertyName("GrassZoom")
    ]
    public double GrassZoom
    {
        get => _grassZoom;
        set => _grassZoom = Math.Clamp(value, 1, 3);
    }

    public string? ResolveLgr(Level lev)
    {
        var lgrDir = Global.AppSettings.General.LgrDirectory;
        if (!Directory.Exists(lgrDir))
        {
            return null;
        }

        var levLgr = Path.Combine(lgrDir, lev.LgrFile + ".lgr");
        if (File.Exists(levLgr))
        {
            return levLgr;
        }

        var defaultLgr = Path.Combine(lgrDir, "default.lgr");
        if (File.Exists(defaultLgr))
        {
            return defaultLgr;
        }

        return null;
    }
}