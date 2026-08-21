using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Elmanager.Lev;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;
using DrawingColor = System.Drawing.Color;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.SLE.UI;

internal sealed class ShapePreview : Control
{
    public static readonly StyledProperty<SleShape?> ShapeProperty =
        AvaloniaProperty.Register<ShapePreview, SleShape?>(nameof(Shape));

    public static readonly StyledProperty<RenderingSettings?> RenderingSettingsProperty =
        AvaloniaProperty.Register<ShapePreview, RenderingSettings?>(nameof(RenderingSettings));

    private static readonly RenderingSettings DefaultRenderingSettings = new();

    public SleShape? Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public RenderingSettings? RenderingSettings
    {
        get => GetValue(RenderingSettingsProperty);
        set => SetValue(RenderingSettingsProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ShapeProperty || change.Property == RenderingSettingsProperty)
        {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var settings = RenderingSettings ?? DefaultRenderingSettings;
        var groundPen = CreatePen(settings.GroundEdgeColor, 1.2);
        var grassPen = CreatePen(settings.GrassEdgeColor, 1.6);
        var picturePen = CreatePen(settings.PictureFrameColor, 1.2);
        var texturePen = CreatePen(settings.TextureFrameColor, 1.2);
        context.DrawRectangle(CreateBrush(settings.SkyFillColor), null, new Rect(Bounds.Size));

        var level = Shape?.Level;
        if (level == null)
        {
            return;
        }

        const double margin = 8;
        var worldWidth = Math.Max(level.Bounds.XMax - level.Bounds.XMin, 0.01);
        var worldHeight = Math.Max(level.Bounds.YMax - level.Bounds.YMin, 0.01);
        var scale = Math.Min(
            Math.Max(Bounds.Width - (margin * 2), 1) / worldWidth,
            Math.Max(Bounds.Height - (margin * 2), 1) / worldHeight);
        var offsetX = (Bounds.Width - (worldWidth * scale)) / 2;
        var offsetY = (Bounds.Height - (worldHeight * scale)) / 2;

        Point Map(Vector point)
        {
            return new Point(
                offsetX + ((point.X - level.Bounds.XMin) * scale),
                offsetY + ((level.Bounds.YMax - point.Y) * scale));
        }

        foreach (var polygon in level.Polygons)
        {
            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(Map(polygon.Vertices[0]), false);
                for (var i = 1; i < polygon.Vertices.Count; i++)
                {
                    geometryContext.LineTo(Map(polygon.Vertices[i]));
                }

                geometryContext.EndFigure(true);
            }

            context.DrawGeometry(null, polygon.IsGrass ? grassPen : groundPen, geometry);
        }

        foreach (var element in level.GraphicElements)
        {
            var topLeft = Map(element.Position);
            var bottomRight = Map(new Vector(
                element.Position.X + element.Width,
                element.Position.Y - element.Height));
            var rect = new Rect(topLeft, bottomRight);
            context.DrawRectangle(null,
                element is GraphicElement.Picture or GraphicElement.MissingPicture ? picturePen : texturePen,
                rect);
        }

        foreach (var obj in level.Objects)
        {
            var center = Map(obj.Position);
            var color = obj.Type switch
            {
                ObjectType.Apple => settings.AppleColor,
                ObjectType.Killer => settings.KillerColor,
                ObjectType.Flower => settings.FlowerColor,
                ObjectType.Start => settings.StartColor,
                _ => throw new ArgumentOutOfRangeException()
            };
            context.DrawEllipse(null, CreatePen(color, 1.2), center, 3.5, 3.5);
        }
    }

    private static IBrush CreateBrush(DrawingColor color) =>
        new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

    private static Pen CreatePen(DrawingColor color, double thickness) =>
        new(CreateBrush(color), thickness);
}
