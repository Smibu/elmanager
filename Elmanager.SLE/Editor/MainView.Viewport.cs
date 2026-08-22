using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;
using Elmanager.SLE.Platform;
using Elmanager.SLE.Platform.OpenGL;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private readonly AvaloniaGraphicsContext _glContext = new();

    private CompositionCustomVisual? _glVisual;

    private void OnViewportPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        GlViewport.Focus();
        if (_renderer == null)
        {
            return;
        }

        var point = e.GetCurrentPoint(GlViewport);
        var p = ScreenToWorld(point.Position);
        _lastMouseCoords = p;
        _currentTool.MouseMove(p);

        var props = point.Properties;
        if (props.IsMiddleButtonPressed)
        {
            if (_keyboardState.IsKeyDown(ModifierKey.LeftCtrl))
            {
                _draggingGrid = true;
                _gridStartOffset = _sceneSettings.GridOffset;
            }
            else
            {
                _draggingScreen = true;
            }

            _moveStartPosition = p;
            e.Handled = true;
            return;
        }

        var button = props.IsLeftButtonPressed ? EditorMouseButton.Left
            : props.IsRightButtonPressed ? EditorMouseButton.Right
            : EditorMouseButton.None;

        var mod = _currentTool.MouseDown(new EditorMouseEventArgs(button));
        SetPendingModification(mod);
        UpdateToolHelp();
        RedrawScene();
        e.Handled = true;
    }

    private void OnViewportPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_renderer == null || _pendingSettingsUpdate)
        {
            return;
        }

        _hasFocus = true;
        var point = e.GetCurrentPoint(GlViewport);
        var p = ScreenToWorld(point.Position);
        _lastMouseCoords = p;

        MouseXLabel.Text = $"X: {p.X:F3}";
        MouseYLabel.Text = $"Y: {p.Y:F3}";

        if (_draggingScreen || _draggingGrid)
        {
            _controller.HandleDragMove(p, _moveStartPosition, _draggingGrid,
                _sceneSettings, _gridStartOffset, Settings.LockGrid, _zoomCtrl);
        }

        var mod = _currentTool.MouseMove(p);
        SetPendingModification(mod);
        UpdateToolHelp();
        RedrawScene();
    }

    private void OnViewportPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _currentTool.MouseUp();
        UpdateToolHelp();
        _draggingScreen = false;
        _draggingGrid = false;
        RedrawScene();
    }

    private void OnViewportPointerExited(object? sender, PointerEventArgs e)
    {
        if (new Rect(GlViewport.Bounds.Size).Contains(e.GetPosition(GlViewport)))
        {
            return;
        }

        _hasFocus = false;
        _cursorManager.ChangeToDefaultCursorIfHand();
        var mod = _currentTool.MouseOutOfEditor();
        SetPendingModification(mod);
        UpdateToolHelp();
        RedrawScene();
    }

    private void OnViewportPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_renderer == null)
        {
            return;
        }

        var delta = e.Delta.Y;
        if (delta == 0)
        {
            return;
        }

        _controller.MouseWheelZoom((long)(delta * 120), _lastMouseCoords, _zoomCtrl, _sceneSettings, Settings,
            _renderer);
        ZoomLabel.Content = $"Zoom: {_zoomCtrl.ZoomLevel:F3}";
        RedrawScene();
        e.Handled = true;
    }

    private void OnViewportAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var visual = ElementComposition.GetElementVisual(GlViewport);
        _glVisual = visual!.Compositor.CreateCustomVisual(new GlVisualHandler(DoRenderScene));
        ElementComposition.SetElementChildVisual(GlViewport, _glVisual);
        UpdateVisualSize();
    }

    private async void OnViewportDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (IsPlaying)
        {
            try
            {
                await StopPlaying();
            }
            catch (Exception ex)
            {
                LogException(ex, "Could not stop playing while closing the viewport.");
            }
        }

        _glVisual?.SendHandlerMessage(new GlVisualHandler.DisposeMessage(_renderer!));
        _glVisual = null;
        ElementComposition.SetElementChildVisual(GlViewport, null);
    }

    private Vector ScreenToWorld(Point screenPos)
    {
        if (_renderer == null)
        {
            return new Vector();
        }

        var viewportSize = GlViewport.Bounds.Size;
        if (viewportSize.Width < 1 || viewportSize.Height < 1)
        {
            return new Vector();
        }

        var cam = _zoomCtrl.Cam;
        var aspectRatio = viewportSize.Width / viewportSize.Height;
        var bounds = cam.GetBounds(aspectRatio);

        var worldX = bounds.XMin + (screenPos.X / viewportSize.Width * (bounds.XMax - bounds.XMin));
        var worldY = bounds.YMax - (screenPos.Y / viewportSize.Height * (bounds.YMax - bounds.YMin));

        return new Vector(worldX, worldY);
    }

    private void RedrawScene()
    {
        if (_glVisual == null)
        {
            return;
        }

        _glVisual.SendHandlerMessage(new GlVisualHandler.RenderRequestMessage());
    }

    private Task RunOnRenderThread(Action action)
    {
        var visual = _glVisual ?? throw new InvalidOperationException("The renderer is not ready.");
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        visual.SendHandlerMessage(new GlVisualHandler.RunOnRenderThreadMessage(action, completion));
        return completion.Task;
    }

    private void UpdateVisualSize()
    {
        if (_glVisual != null)
        {
            var s = GlViewport.Bounds.Size;
            _glVisual.Size = new Avalonia.Vector(s.Width, s.Height);
        }
    }

    private bool DoRenderScene(PixelSize size, float renderScaling)
    {
        try
        {
            RendererSettingsChangeResult? settingsChange = null;
            var firstRender = _renderer == null;
            _renderer ??= new ElmaRenderer(_glContext, Settings.RenderingSettings, _lgrCache);
            _renderer.SetRenderScaling(renderScaling);
            if (firstRender || _pendingSettingsUpdate)
            {
                settingsChange = _renderer.UpdateSettings(_controller.Lev, Settings.RenderingSettings);
                _pendingSettingsUpdate = false;
            }

            if (firstRender || _pendingZoomFill)
            {
                _pendingZoomFill = false;
                ZoomFill(size.Width / (double)size.Height);
            }

            var requestNextFrame = _gameLoopRunner.RunFrame();
            var applesUpdated = false;

            if (_pendingModification is { } mod)
            {
                UpdateRendererBuffers(mod);
                applesUpdated = mod.HasFlag(LevVisualChange.Apples);
                _pendingModification = null;
            }

            ApplyFadedObjects(applesUpdated);

            DrawEditorScene(size.Width, size.Height);

            if (firstRender)
            {
                Dispatcher.UIThread.Post(() => Console.WriteLine("First render done"), DispatcherPriority.Background);
            }

            if (settingsChange?.LgrUpdated == true)
            {
                Dispatcher.UIThread.Post(() => Console.WriteLine("LGR load ready"), DispatcherPriority.Background);
            }

            if (settingsChange?.LgrLoadException is { } lgrLoadException)
            {
                Dispatcher.UIThread.Post(() => LogException(
                        lgrLoadException,
                        "Could not load the selected LGR."),
                    DispatcherPriority.Background);
            }

            return requestNextFrame;
        }
        catch (Exception e)
        {
            LogException(e, "Could not render the level.");
            throw;
        }
    }

    private void DrawEditorScene(int width, int height)
    {
        _renderer!.SetLineWidth(Settings.RenderingSettings.LineWidth);
        _renderer.ResetViewport(width, height);
        _controller.DrawEditorScene(_renderer, _zoomCtrl.Cam, _sceneSettings, Settings,
            _playController, _currentTool, _currentHighlight,
            width, height, () => _lastMouseCoords);
    }
}
