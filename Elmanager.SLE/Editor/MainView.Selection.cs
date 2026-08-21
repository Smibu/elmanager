using Avalonia.Interactivity;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private readonly SelectionFilter _selectionFilter;
    ISelectionFilter ILevelEditor.SelectionFilter => _selectionFilter;
    void ILevelEditor.PreserveSelection() => _controller.PreserveSelection();
    void ILevelEditor.UpdateSelectionInfo() => UpdateSelectionInfo();

    private void UpdateSelectionInfo()
    {
        _controller.UpdateSelectionInfo();
        SelectionLabel.Text = _controller.GetSelectionText();
    }

    private void OnDeselectGroundClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectPolygonsWith(p => !p.IsGrass);
        RedrawScene();
    }

    private void OnDeselectGrassClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectPolygonsWith(p => p.IsGrass);
        RedrawScene();
    }

    private void OnDeselectApplesClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectObjectsWith(o => o.Type == ObjectType.Apple);
        RedrawScene();
    }

    private void OnDeselectKillersClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectObjectsWith(o => o.Type == ObjectType.Killer);
        RedrawScene();
    }

    private void OnDeselectFlowersClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectObjectsWith(o => o.Type == ObjectType.Flower);
        RedrawScene();
    }

    private void OnDeselectPicturesClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectGraphicElementsWith(ge => ge is GraphicElement.Picture);
        RedrawScene();
    }

    private void OnDeselectTexturesClick(object? sender, RoutedEventArgs e)
    {
        _controller.DeselectGraphicElementsWith(ge => ge is GraphicElement.Texture);
        RedrawScene();
    }

    private void OnEnableAllFiltersClick(object? sender, RoutedEventArgs e) => SetAllFilters(true);
    private void OnDisableAllFiltersClick(object? sender, RoutedEventArgs e) => SetAllFilters(false);

    private void OnFilterItemClick(object? sender, RoutedEventArgs e)
    {
        _selectionFilter.GroundFilter = GroundFilterMenuItem.IsChecked;
        _selectionFilter.GrassFilter = GrassFilterMenuItem.IsChecked;
        _selectionFilter.AppleFilter = ApplesFilterMenuItem.IsChecked;
        _selectionFilter.KillerFilter = KillersFilterMenuItem.IsChecked;
        _selectionFilter.FlowerFilter = FlowersFilterMenuItem.IsChecked;
        _selectionFilter.StartFilter = StartFilterMenuItem.IsChecked;
        _selectionFilter.PictureFilter = PicturesFilterMenuItem.IsChecked;
        _selectionFilter.TextureFilter = TexturesFilterMenuItem.IsChecked;
    }

    private void SetAllFilters(bool value)
    {
        _selectionFilter.GroundFilter = value;
        _selectionFilter.GrassFilter = value;
        _selectionFilter.AppleFilter = value;
        _selectionFilter.KillerFilter = value;
        _selectionFilter.FlowerFilter = value;
        _selectionFilter.StartFilter = value;
        _selectionFilter.PictureFilter = value;
        _selectionFilter.TextureFilter = value;
        GroundFilterMenuItem.IsChecked = value;
        GrassFilterMenuItem.IsChecked = value;
        ApplesFilterMenuItem.IsChecked = value;
        KillersFilterMenuItem.IsChecked = value;
        FlowersFilterMenuItem.IsChecked = value;
        StartFilterMenuItem.IsChecked = value;
        PicturesFilterMenuItem.IsChecked = value;
        TexturesFilterMenuItem.IsChecked = value;
    }
}
