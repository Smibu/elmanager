using System;
using System.Drawing;
using System.Printing;
using System.Windows.Forms;
using System.Windows.Threading;
using BrightIdeasSoftware;
using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.GLControl;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace Elmanager.LevelEditor.ShapeGallery
{
    public class LevelControl : GLControl
    {
        private ElmaRenderer _renderer;
        private Level _level;
        private ElmaCamera _camera;
        private RenderingSettings _renderingSettings;
        private SceneSettings _sceneSettings;
        private ZoomController _zoomController;

        internal LevelControl(GLControl sharedContext, ElmaRenderer renderer, Level level, ElmaCamera camera, SceneSettings sceneSettings, RenderingSettings renderingSettings) :
            base()
        {
            Profile = ContextProfile.Compatability;

            _renderer = renderer;
            _level = level;
            _camera = camera;
            _renderingSettings = new RenderingSettings();
            _sceneSettings = sceneSettings;

            _zoomController = new ZoomController(_camera, _level, () => Render());

            this.Load += LevelControl_Load;

            if (!IsHandleCreated)
            {
                SharedContext = sharedContext; // Set shared context before initialization
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
            //GL.ClearColor(Color.Red);

            var r = _renderer.UpdateSettings(_level, _renderingSettings);
            _renderer.InitializeLevel(_level, _renderingSettings);
            _level.UpdateBounds();
            _zoomController.ZoomFill(_renderingSettings);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render();
        }

        private void Render()
        {
            if (Context == null)
            {
                return;
            }

            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }

            GL.Viewport(0, 0, Width, Height); // Set viewport to the entire control

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CheckGLError("GL.Clear");

            
            // Use the ElmaRenderer to render the level
            _renderer.DrawScene(_level, _camera, _sceneSettings, _renderingSettings);

            SwapBuffers();
        }

        private void CheckGLError(string location)
        {
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                System.Diagnostics.Debug.WriteLine($"OpenGL Error at {location}: {error}");
            }
        }
    }
}
