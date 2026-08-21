using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Tools;
using SleEditorTools = Elmanager.SLE.Editor.Tools.EditorTools;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private readonly Dictionary<ToggleButton, IEditorTool> _buttonToolMap;
    private readonly SleEditorTools _tools;

    private ToggleButton[] ToolButtons =>
    [
        SelectButton, VertexButton, DrawButton, ObjectButton, PipeButton,
        EllipseButton, PolyOpButton, FrameButton, SmoothenButton, CutConnectButton,
        AutoGrassButton, PictureButton, TextButton, CustomShapeButton
    ];

    void ILevelEditor.ChangeToSelectionTool() => ChangeToolTo(_tools.SelectionTool);

    private void ChangeToolTo(IEditorTool tool)
    {
        var mod = _currentTool.InActivate();
        SetPendingModification(mod);

        _currentTool = tool;
        _currentTool.Activate();
        UpdateToolHelp();
        RedrawScene();
    }

    private void UpdateToolHelp() => InfoLabel.Text = _currentTool.GetHelp();

    private void ActivateTool(ToggleButton button)
    {
        button.IsChecked = true;
        OnToolChanged(button, new RoutedEventArgs());
    }

    private async void OnToolChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton clicked)
        {
            return;
        }

        if (ReferenceEquals(clicked, PictureButton) && _renderer?.OpenGlLgr == null)
        {
            await RejectToolSelection(clicked, "an LGR", "picture");
            return;
        }

        if (ReferenceEquals(clicked, CustomShapeButton) && Settings.ShapeFolder is null)
        {
            await RejectToolSelection(clicked, "a shape folder", "shape");
            return;
        }

        GlViewport.Focus();

        if (clicked.IsChecked == false)
        {
            clicked.IsChecked = true;
            return;
        }

        foreach (var btn in ToolButtons)
        {
            if (btn != clicked)
            {
                btn.IsChecked = false;
            }
        }

        var tool = _buttonToolMap.GetValueOrDefault(clicked, _tools.SelectionTool);
        ChangeToolTo(tool);
    }

    private async Task RejectToolSelection(ToggleButton button, string requirement, string toolName)
    {
        button.IsChecked = false;
        await ShowFolderSettingsPrompt(
            $"Select {requirement} before using the {toolName} tool.");
    }
}
