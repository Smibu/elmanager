using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.Physics;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private Vector _contextMenuClickPosition;

    private bool IsLgrLoaded => _renderer?.OpenGlLgr != null;
    private int SelectedElementCount => _controller.SelectedElementCount;

    private void OnEditorContextMenuOpening(object? sender, CancelEventArgs e)
    {
        if (_currentTool.Busy)
        {
            e.Cancel = true;
            return;
        }

        var p = _lastMouseCoords;
        _contextMenuClickPosition = p;

        _controller.UpdateSelectionInfo();

        var hitTester = _tools.SelectionTool;
        var info = hitTester.GetNearestVertexInfo(p);
        var nearestObjectIndex = hitTester.GetNearestObjectIndex(p);
        var nearestPictureIndex = hitTester.GetNearestPictureIndex(p);
        var player = _playController.GetNearestDriverBodyPart(p, hitTester.CaptureRadiusScaled);
        _playController.FollowDriver = false;

        CtxCopyMenuItem.IsVisible = false;
        CtxDeleteMenuItem.IsVisible = false;
        CtxGravityNoneMenuItem.IsVisible = false;
        CtxGravityUpMenuItem.IsVisible = false;
        CtxGravityDownMenuItem.IsVisible = false;
        CtxGravityLeftMenuItem.IsVisible = false;
        CtxGravityRightMenuItem.IsVisible = false;
        CtxGrassMenuItem.IsVisible = false;
        CtxPicturePropertiesMenuItem.IsVisible = false;
        CtxTransformMenuItem.IsVisible = false;
        CtxBringToFrontMenuItem.IsVisible = false;
        CtxSendToBackMenuItem.IsVisible = false;
        CtxConvertMenuItem.IsVisible = false;
        CtxSaveStartPositionMenuItem.IsVisible = false;
        CtxRestoreStartPositionMenuItem.IsVisible = false;
        CtxMoveStartHereMenuItem.IsVisible = ReferenceEquals(_currentTool, _tools.SelectionTool);
        CtxSaveShapeMenuItem.IsVisible = false;

        CtxGravityNoneMenuItem.IsChecked = false;
        CtxGravityUpMenuItem.IsChecked = false;
        CtxGravityDownMenuItem.IsChecked = false;
        CtxGravityLeftMenuItem.IsChecked = false;
        CtxGravityRightMenuItem.IsChecked = false;

        if (SelectedElementCount > 0)
        {
            CtxCopyMenuItem.IsVisible = true;
            CtxDeleteMenuItem.IsVisible = true;
            CtxConvertMenuItem.IsVisible = true;
            CtxPicturesConvertItem.IsVisible = IsLgrLoaded;
        }

        CtxTransformMenuItem.IsVisible = SelectedElementCount > 1;
        _controller.SelectedObjectIndex = nearestObjectIndex;
        if (nearestObjectIndex >= 0)
        {
            CtxBringToFrontMenuItem.IsVisible = true;
            CtxSendToBackMenuItem.IsVisible = true;
            switch (_controller.Lev.Objects[nearestObjectIndex].Type)
            {
                case ObjectType.Apple:
                    CtxGravityNoneMenuItem.IsVisible = true;
                    CtxGravityUpMenuItem.IsVisible = true;
                    CtxGravityDownMenuItem.IsVisible = true;
                    CtxGravityLeftMenuItem.IsVisible = true;
                    CtxGravityRightMenuItem.IsVisible = true;
                    switch (_controller.Lev.Objects[nearestObjectIndex].AppleType)
                    {
                        case AppleType.Normal:
                            UpdateGravityMenu(CtxGravityNoneMenuItem);
                            break;
                        case AppleType.GravityUp:
                            UpdateGravityMenu(CtxGravityUpMenuItem);
                            break;
                        case AppleType.GravityDown:
                            UpdateGravityMenu(CtxGravityDownMenuItem);
                            break;
                        case AppleType.GravityLeft:
                            UpdateGravityMenu(CtxGravityLeftMenuItem);
                            break;
                        case AppleType.GravityRight:
                            UpdateGravityMenu(CtxGravityRightMenuItem);
                            break;
                    }

                    break;
                case ObjectType.Start when Settings.EnableStartPositionFeature:
                    CtxSaveStartPositionMenuItem.IsVisible = true;
                    if (_controller.SavedStartPosition != null)
                    {
                        CtxRestoreStartPositionMenuItem.IsVisible = true;
                    }

                    break;
            }
        }

        if (info is not null)
        {
            CtxGrassMenuItem.IsVisible = true;
            _controller.GrassInfo = info;
            if (info.Polygon.IsGrass)
            {
                CtxBringToFrontMenuItem.IsVisible = true;
                CtxSendToBackMenuItem.IsVisible = true;
            }
        }

        _controller.SelectedPictureIndex = nearestPictureIndex;
        if (nearestPictureIndex >= 0)
        {
            CtxPicturePropertiesMenuItem.IsVisible = true;
            CtxBringToFrontMenuItem.IsVisible = true;
            CtxSendToBackMenuItem.IsVisible = true;
        }

        if (_controller.SelectedPolygonCount > 0)
        {
            var allGrassSelected = _controller.Lev.Polygons
                .Where(pol => pol.Vertices.Any(v => v.Mark == VectorMark.Selected))
                .All(pol => pol.IsGrass);
            CtxSaveShapeMenuItem.IsVisible = !allGrassSelected;
        }

        if (player != null && nearestObjectIndex < 0)
        {
            CtxGravityUpMenuItem.IsVisible = true;
            CtxGravityDownMenuItem.IsVisible = true;
            CtxGravityLeftMenuItem.IsVisible = true;
            CtxGravityRightMenuItem.IsVisible = true;
            switch (_playController.Driver!.GravityDirection)
            {
                case GravityDirection.Up:
                    UpdateGravityMenu(CtxGravityUpMenuItem);
                    break;
                case GravityDirection.Down:
                    UpdateGravityMenu(CtxGravityDownMenuItem);
                    break;
                case GravityDirection.Left:
                    UpdateGravityMenu(CtxGravityLeftMenuItem);
                    break;
                case GravityDirection.Right:
                    UpdateGravityMenu(CtxGravityRightMenuItem);
                    break;
            }
        }

        e.Cancel = !EditorContextMenu.Items
            .OfType<MenuItem>()
            .Any(item => item.IsVisible);
    }

    private void UpdateGravityMenu(MenuItem chosen)
    {
        CtxGravityNoneMenuItem.IsChecked = ReferenceEquals(chosen, CtxGravityNoneMenuItem);
        CtxGravityUpMenuItem.IsChecked = ReferenceEquals(chosen, CtxGravityUpMenuItem);
        CtxGravityDownMenuItem.IsChecked = ReferenceEquals(chosen, CtxGravityDownMenuItem);
        CtxGravityLeftMenuItem.IsChecked = ReferenceEquals(chosen, CtxGravityLeftMenuItem);
        CtxGravityRightMenuItem.IsChecked = ReferenceEquals(chosen, CtxGravityRightMenuItem);
    }

    private void OnCtxTransformClick(object? sender, RoutedEventArgs e) => TransformSelection();

    private void OnCtxToggleGrassClick(object? sender, RoutedEventArgs e)
    {
        if (_controller.WouldToggleGrassLeaveNoGroundPolygons())
        {
            ShowError("At least one ground polygon must remain in the level.", "Unable to toggle grass");
            return;
        }

        _controller.ToggleGrass();
        RedrawScene();
    }

    private void OnCtxGravityClick(object? sender, RoutedEventArgs e)
    {
        AppleType chosenAppleType;
        if (ReferenceEquals(sender, CtxGravityNoneMenuItem))
        {
            chosenAppleType = AppleType.Normal;
        }
        else if (ReferenceEquals(sender, CtxGravityUpMenuItem))
        {
            chosenAppleType = AppleType.GravityUp;
        }
        else if (ReferenceEquals(sender, CtxGravityDownMenuItem))
        {
            chosenAppleType = AppleType.GravityDown;
        }
        else if (ReferenceEquals(sender, CtxGravityLeftMenuItem))
        {
            chosenAppleType = AppleType.GravityLeft;
        }
        else
        {
            chosenAppleType = AppleType.GravityRight;
        }

        _controller.HandleGravity(chosenAppleType, _playController);
        RedrawScene();
    }

    private async void OnCtxPicturePropertiesClick(object? sender, RoutedEventArgs e) =>
        await _controller.ShowPictureProperties(Settings.AlwaysSetDefaultsInPictureTool);

    private void OnCtxBringToFrontClick(object? sender, RoutedEventArgs e)
    {
        _controller.BringToFront();
        RedrawScene();
    }

    private void OnCtxSendToBackClick(object? sender, RoutedEventArgs e)
    {
        _controller.SendToBack();
        RedrawScene();
    }

    private async void OnCtxConvertClick(object? sender, RoutedEventArgs e)
    {
        ObjectType? objType = null;
        if (ReferenceEquals(sender, CtxApplesConvertItem))
        {
            objType = ObjectType.Apple;
        }
        else if (ReferenceEquals(sender, CtxKillersConvertItem))
        {
            objType = ObjectType.Killer;
        }
        else if (ReferenceEquals(sender, CtxFlowersConvertItem))
        {
            objType = ObjectType.Flower;
        }

        await _controller.ConvertSelected(objType);
        RedrawScene();
    }

    private void OnCtxSaveStartPositionClick(object? sender, RoutedEventArgs e)
    {
        _controller.SaveStartPosition(_controller.Lev);
        RedrawScene();
    }

    private void OnCtxRestoreStartPositionClick(object? sender, RoutedEventArgs e)
    {
        var mod = _controller.RestoreStartPosition();
        if (mod != LevModification.Nothing)
        {
            SetModified(mod);
            RedrawScene();
        }
    }

    private void OnCtxMoveStartHereClick(object? sender, RoutedEventArgs e)
    {
        var mod = _controller.MoveStartHere(_contextMenuClickPosition);
        if (mod != LevModification.Nothing)
        {
            SetModified(mod);
            RedrawScene();
        }
    }

    private async void OnCtxSaveShapeClick(object? sender, RoutedEventArgs e) =>
        await _tools.CustomShapeTool.SaveShape();
}
