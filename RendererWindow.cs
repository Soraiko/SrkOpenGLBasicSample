using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace SrkOpenGLBasicSample
{
    public class RendererWindow : GameWindow
    {
        public RendererWindow(int x, int y, int width, int height, int sampleCount) : base(width, height, new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, sampleCount))
        {
            this.Location = new Point(x, y);
            if (Preferences.Fullscreen)
                WindowState = WindowState.Fullscreen;

            this.CursorVisible = true;
        }

        public KeyboardState keyboardState;
        public MouseState mouseState;
        public KeyboardState oldKeyboardState;
        public MouseState oldMouseState;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (keyboardState.IsKeyDown(Key.Escape))
                Close();

            if (Camera.Current!=null)
            {
                Camera.Current.MouseControl(mouseState, oldMouseState);
                Camera.Current.KeyboardControl(keyboardState, oldKeyboardState);
                Camera.Current.Update(this);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref Camera.Current.LookAtMatrix);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref Camera.Current.ProjectionMatrix);
            }

            StaticReferences.Light0_Position.X = (float)(3000 * Math.Cos(angle));
            StaticReferences.Light0_Position.Z = (float)(3000 * Math.Sin(angle));
            angle += 0.01f;

            map.Update();

            for (int i = 0; i < models.Count; i++)
                models[i].Update();


            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }
        double angle = 0;

        public static Color BackgroundColor = new Color(50,50,50,255);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            map.Draw();

            for (int i = 0; i < models.Count; i++)
                models[i].Draw();
            

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        List<Model> models;

        Model map;

        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();
            models = new List<Model>(0);

            map = new DAE(@"debug_files\BB00\BB00.dae").Parse();
            map.Compile();

            for (int i=0;i<1;i++)
            {
                Model model = new DAE(@"debug_files\H_EX500\H_EX500.dae").Parse();
                model.Compile();
                models.Add(model);
            }

            OnUpdateFrame(null);
            base.OnLoad(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            mouseState = e.Mouse;
            base.OnMouseWheel(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            mouseState = e.Mouse;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            mouseState = e.Mouse;
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            keyboardState = e.Keyboard;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            keyboardState = e.Keyboard;
            base.OnKeyDown(e);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            if (Camera.Current != null)
                Camera.Current.ProjectionMatrixDirty = true;

            base.OnResize(e);
        }
    }
}