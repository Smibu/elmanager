using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Elmanager.LevelEditor.Shapes;

public class LevelControl : GLControl
{
    private ElmaRenderer _renderer;
    
    private Level _level;
    
    private readonly ElmaCamera _camera;
    private readonly RenderingSettings _renderingSettings;
    private readonly SceneSettings _sceneSettings;
    private readonly ZoomController _zoomController;
    
    public bool DisableRendering { get; set; } = true;

    internal LevelControl(GLControl sharedContext, SceneSettings sceneSettings, RenderingSettings renderingSettings, ElmaRenderer elmaRenderer, Level level) :
        base(new GLControlSettings {
            AutoLoadBindings = false,
            Profile = ContextProfile.Compatability
        })
    {
        _renderingSettings = renderingSettings;

        _level = level;
        _camera = new ElmaCamera();
        _zoomController = new ZoomController(_camera, _level, () => RedrawScene());

        _sceneSettings = sceneSettings;

        _renderer = elmaRenderer;

        Load += (sender, e) =>
        {
            _renderer = new ElmaRenderer(this, _renderingSettings, elmaRenderer);
            UpdateRenderingContext();
        };

        if (!IsHandleCreated)
        {
            SharedContext = sharedContext; // Set shared context before initialization
        }

    }

    private void UpdateRenderingContext()
    {
        _renderer.UpdateSettings(_level, _renderingSettings);
        _renderer.InitializeLevel(_level, _renderingSettings);
        _level.UpdateBounds();
        _zoomController.ZoomFill(_renderingSettings);
        Render(true);
    }

    internal void SetLevel(Level level)
    {
        if (_level == level)
        {
            return;
        }

        _level = level;

        _zoomController.Lev = _level;

        UpdateRenderingContext();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (Context != null && !Context.IsCurrent)
        {
            MakeCurrent();
        }

        GL.Viewport(0, 0, Width, Height);

        if (Context != null)
            Context.SwapInterval = 0;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Render(false);
    }

    private void Render(bool resetViewport)
    {
        if (DisableRendering)
        {
            return;
        }
        
        if (Context == null)
        {
            return;
        }

        if (!Context.IsCurrent)
        {
            MakeCurrent();
        }

        if (resetViewport)
        {
            GL.Viewport(0, 0, Width, Height);
            _zoomController.ZoomFill(_renderingSettings);
        }

        RedrawScene();
        SwapBuffers();
    }

    internal void RedrawScene(object? sender = null, EventArgs? e = null)
    {
        _renderer.DrawScene(_level, _camera, _sceneSettings, _renderingSettings);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Causes a crash.
            //_renderer.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        Render(true);
    }
}