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
    private Level? _level;
    private ElmaCamera? _camera;
    private readonly RenderingSettings _renderingSettings;
    private readonly SceneSettings _sceneSettings;
    private ZoomController? _zoomController;
    private bool _isFirstRender = true;
    public bool DisableRendering { get; set; } = true;

    private DispatcherTimer? _resizeTimer;
    private const int DebounceInterval = 6; // Debounce interval in milliseconds

    internal LevelControl(GLControl sharedContext, SceneSettings sceneSettings, RenderingSettings renderingSettings, ElmaRenderer elmaRenderer, Level? level=null) :
        base(new GLControlSettings {
            AutoLoadBindings = false,
            Profile = ContextProfile.Compatability
        })
    {
        _renderingSettings = renderingSettings;

        if (level != null)
        {
            _level = level;
            _camera = new ElmaCamera();
            _zoomController = new ZoomController(_camera, _level, () => RedrawScene());
        }

        _sceneSettings = sceneSettings;
        _originalElmaRenderer = elmaRenderer;

        _renderer = elmaRenderer;

        this.Load += LevelControl_Load;

        if (!IsHandleCreated)
        {
            SharedContext = sharedContext; // Set shared context before initialization
        }

        // Initialize the resize timer
        _resizeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(DebounceInterval)
        };
        _resizeTimer.Tick += ResizeTimer_Tick;
    }

    /**
     * Set the level to be displayed in the control
     * To disable rendering, set level to null
     */
    internal void SetLevel(Level? level)
    {
        if (_level == level)
        {
            // Prevent redundant reassignments
            return;
        }

        _level = level;

        if (_level != null)
        {
            _camera ??= new ElmaCamera();
            _zoomController ??= new ZoomController(_camera, _level, () => RedrawScene());
            
            _zoomController.Lev = _level;

            _isFirstRender = true;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (Context != null && !Context.IsCurrent)
        {
            MakeCurrent();
        }

        GL.Viewport(0, 0, Width, Height); // Set viewport to the entire control
        if (Context != null)
            Context.SwapInterval = 0;
    }

    private void LevelControl_Load(object? sender, EventArgs e)
    {
        // Initialization code for OpenGL
        _renderer = new ElmaRenderer(this, _renderingSettings);
        _renderer.OpenGlLgr = _originalElmaRenderer.OpenGlLgr; // Slightly faster with these lines. There is however likely a memory leak here
        _renderer._lgrCache = _originalElmaRenderer._lgrCache; // Slightly faster with these lines. There is however likely a memory leak here
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

        bool viewportChanged = _isFirstRender || resetViewport;

        if (_isFirstRender && _level != null)
        {
            _renderer.UpdateSettings(_level, _renderingSettings);
            _renderer.InitializeLevel(_level, _renderingSettings);
            _level.UpdateBounds();
            _isFirstRender = false;
        }

        if (viewportChanged)
        {
            GL.Viewport(0, 0, Width, Height);
            _zoomController?.ZoomFill(_renderingSettings);
        }

        RedrawScene();
        SwapBuffers();
    }

    internal void RedrawScene(object? sender = null, EventArgs? e = null)
    {
        if (_level != null && _camera != null)
        {
            _renderer.DrawScene(_level, _camera, _sceneSettings, _renderingSettings);
        }
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
            _resizeTimer?.Stop();
            if (_resizeTimer != null)
            {
                _resizeTimer.Tick -= ResizeTimer_Tick;
            }
            _resizeTimer = null;

            _renderer.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
            
        // Restart the debounce timer
        _resizeTimer?.Stop();
        _resizeTimer?.Start();
    }

    private void ResizeTimer_Tick(object? sender, EventArgs e)
    {
        // Stop the timer
        _resizeTimer?.Stop();

        Render(true);
    }
}