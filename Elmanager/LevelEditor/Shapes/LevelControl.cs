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
    private readonly ElmaRenderer _originalElmaRenderer;
    
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
        _originalElmaRenderer = elmaRenderer;

        _renderer = elmaRenderer;

        this.Load += LevelControl_Load;

        if (!IsHandleCreated)
        {
            SharedContext = sharedContext; // Set shared context before initialization
        }

        if (SharedContext == null)
        {
            System.Diagnostics.Debug.WriteLine("Warning: SharedContext was not properly initialized.");
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

    /**
     * Set the level to be displayed in the control
     * To disable rendering, set level to null
     */
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

    private void LevelControl_Load(object? sender, EventArgs e)
    {
        // Initialization code for OpenGL
        _renderer = new ElmaRenderer(this, _renderingSettings, _originalElmaRenderer);

        UpdateRenderingContext();
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

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        CheckGLError("GL.Clear");

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

    private static void CheckGLError(string location)
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            System.Diagnostics.Debug.WriteLine($"OpenGL Error at {location}: {error}");
        }
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