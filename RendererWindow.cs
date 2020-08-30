using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

namespace SrkOpenGLBasicSample
{
    public class RendererWindow : GameWindow
    {
        public RendererWindow(int width, int height, int sampleCount) : base(width, height, new GraphicsMode(ColorFormat.Empty, 0, 0, sampleCount, ColorFormat.Empty, 1))
        {
            if (Preferences.Fullscreen)
            {
                WindowState = WindowState.Fullscreen;
                this.CursorVisible = true;
                
            }
        }

        public static Color BackgroundColor;

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            
            Camera.Current = new Camera(200f);

            base.OnLoad(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            if (Camera.Current != null)
                Camera.Current.ProjectionMatrixDirty = true;

            base.OnResize(e);
        }

        public KeyboardState keyboardState;
        public KeyboardState oldKeyboardState;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            keyboardState = Keyboard.GetState();

            if (Camera.Current!=null)
            {
                Camera.Current.KeyboardControl(keyboardState, oldKeyboardState);
                Camera.Current.Update(this);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref Camera.Current.LookAtMatrix);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref Camera.Current.ProjectionMatrix);
            }

            oldKeyboardState = keyboardState;
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);


            GL.Begin(PrimitiveType.TriangleStrip);

            GL.Color4(1f, 0f, 0f, 1f);
            GL.Vertex3(0f, 0f, 0f);

            GL.Color4(0f, 1f, 0f, 1f);
            GL.Vertex3(0f, 100f, 0f);

            GL.Color4(0f, 0f, 1f, 1f);
            GL.Vertex3(100f, 0f, 0f);

            GL.Color4(1f, 1f, 1f, 1f);
            GL.Vertex3(100f, 100f, 0f);


            GL.End();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}