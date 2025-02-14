using System;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Windows.Forms;
using System.Windows.Threading;
using BrightIdeasSoftware;
using Elmanager.Lev;
using Elmanager.LevelEditor.Playing;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.GLControl;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace Elmanager.LevelEditor.Shapes
{
    public class LevelControl : GLControl
    {
        private ElmaRenderer _renderer;
        private ElmaRenderer _originalElmaRenderer;
        private Level? _level;
        private ElmaCamera? _camera;
        private RenderingSettings _renderingSettings;
        private SceneSettings _sceneSettings;
        private ZoomController? _zoomController;
        private bool _isFirstRender = true;
        public bool DisableRendering { get; set; } = true;

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
        }

        /**
         * Set the level to be displayed in the control
         * To disable rendering, set level to null
         */
        internal void SetLevel(Level? level)
        {
            if (level != null)
            {
                _level = level;
                _camera = new ElmaCamera();
                _zoomController = new ZoomController(_camera, _level, () => RedrawScene());
                _isFirstRender = true;
            }
            else
            {
                // Refactor to something more elegant
                _level = null;
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
            Render();
        }

        private void Render()
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

            if (_isFirstRender && _level != null)
            {
                GL.Viewport(0, 0, Width, Height); // Set viewport to the entire control

                var r = _renderer.UpdateSettings(_level, _renderingSettings);
                _renderer.InitializeLevel(_level, _renderingSettings);
                _level.UpdateBounds();
                _zoomController?.ZoomFill(_renderingSettings);
                _isFirstRender = false;
            }
            // Use the ElmaRenderer to render the level
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

        private void CheckGLError(string location)
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
                // Dispose of any disposable resources here
                //_renderer.Dispose(); // Crashes.
            }
            base.Dispose(disposing);
        }
    }
}
