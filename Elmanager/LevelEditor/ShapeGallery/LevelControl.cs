using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;
using OpenTK.GLControl;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.LevelEditor.ShapeGallery
{
    public class LevelControl : GLControl
    {
        private Timer _renderTimer;

        internal LevelControl(GLControl sharedContext) : 
            base()
        {
            this.Load += LevelControl_Load;

            if (!IsHandleCreated)
            {
                SharedContext = sharedContext; // Set shared context before initialization
            }

            _renderTimer = new Timer{ Interval = 16 };
            _renderTimer.Tick += RenderLoop;
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
            GL.ClearColor(Color.Red);
        }

        public void StartRendering()
        {
            _renderTimer.Start(); // Start the DispatcherTimer
        }

        private void RenderLoop(object? sender, EventArgs e)
        {
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

            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CheckGLError("GL.Clear");

            // Simple triangle rendering
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex2(-0.5f, -0.5f);
            GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex2(0.5f, -0.5f);
            GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex2(0.0f, 0.5f);
            GL.End();
            CheckGLError("GL.Begin/GL.End");

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
