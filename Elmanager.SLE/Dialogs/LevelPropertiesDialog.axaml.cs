using System;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Elmanager.Lev;
using Elmanager.Utilities;

namespace Elmanager.SLE.Dialogs;

internal partial class LevelPropertiesDialog : BaseDialog<bool>
{
    public LevelPropertiesDialog(Level level)
    {
        InitializeComponent();
        PropertiesBox.Text =
            $"Polygons: {level.PolygonCount}{Environment.NewLine}" +
            $"Vertices: {level.VertexCount}{Environment.NewLine}" +
            $"Ground polygons: {level.GroundPolygonCount}{Environment.NewLine}" +
            $"Ground vertices: {level.GroundVertexCount}{Environment.NewLine}" +
            $"Grass polygons: {level.GrassPolygonCount}{Environment.NewLine}" +
            $"Grass vertices: {level.GrassVertexCount}{Environment.NewLine}" +
            $"Objects: {level.Objects.Count}{Environment.NewLine}" +
            $"Apples: {level.AppleObjectCount}{Environment.NewLine}" +
            $"Killers: {level.KillerObjectCount}{Environment.NewLine}" +
            $"Flowers: {level.ExitObjectCount}{Environment.NewLine}" +
            $"Pictures: {level.PictureCount}{Environment.NewLine}" +
            $"Textures: {level.TextureCount}{Environment.NewLine}" +
            $"Width: {level.Width:F3}{Environment.NewLine}" +
            $"Height: {level.Height:F3}";

        var top10 = level.Top10;
        SinglePlayerTimesBox.Text =
            $"{top10.GetSinglePlayerString()}{Environment.NewLine}" +
            $"Average: {top10.GetSinglePlayerAverage().ToTimeString()}";
        MultiPlayerTimesBox.Text =
            $"{top10.GetMultiPlayerString()}{Environment.NewLine}" +
            $"Average: {top10.GetMultiPlayerAverage().ToTimeString()}";
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close();
}
