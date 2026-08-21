using System;
using System.IO;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Dialogs;

internal partial class SvgImportOptionsDialog : BaseDialog<SvgImportOptions>
{
    private const double SmoothnessPower = 1.09648;

    public SvgImportOptionsDialog(SvgImportOptions options, string fileName)
    {
        InitializeComponent();
        TitleText.Text = $"SVG import options for {Path.GetFileNameWithoutExtension(fileName)}";
        SmoothnessSlider.Value = Math.Clamp(
            Math.Round(-Math.Log(options.Smoothness / 10) / Math.Log(SmoothnessPower)),
            SmoothnessSlider.Minimum,
            SmoothnessSlider.Maximum);
        UseOutlinedGeometryCheck.IsChecked = options.UseOutlinedGeometry;
        NeverWidenClosedPathsCheck.IsChecked = options.NeverWidenClosedPaths;
        EvenOddRadio.IsChecked = options.FillRule == FillRule.EvenOdd;
        NonZeroRadio.IsChecked = options.FillRule == FillRule.Nonzero;
    }

    private SvgImportOptions CurrentOptions => new()
    {
        Smoothness = 10 * Math.Pow(SmoothnessPower, -SmoothnessSlider.Value),
        FillRule = EvenOddRadio.IsChecked == true ? FillRule.EvenOdd : FillRule.Nonzero,
        UseOutlinedGeometry = UseOutlinedGeometryCheck.IsChecked == true,
        NeverWidenClosedPaths = NeverWidenClosedPathsCheck.IsChecked == true
    };

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close(CurrentOptions);

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
}
