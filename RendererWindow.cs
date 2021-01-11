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

            var camera = Camera.Current;

            if (camera != null)
            {
                camera.MouseControl(mouseState, oldMouseState);
                camera.KeyboardControl(keyboardState, oldKeyboardState);

                camera.ControlTarget(keyboardState, oldKeyboardState);
                camera.Update(this);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref camera.LookAtMatrix);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref camera.ProjectionMatrix);
            }

            //StaticReferences.Light0_Position.X = (float)(3000 * Math.Cos(angle));
            //StaticReferences.Light0_Position.Z = (float)(3000 * Math.Sin(angle));

            for (int i = 0; i < models.Count; i++)
                models[i].Update();


            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }

        float angle = 0;
        float y = 130;

        public static Color BackgroundColor = new Color(50, 50, 50, 255);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            for (int i = 0; i < models.Count; i++)
                models[i].Draw();


            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        List<Model> models;

        public BinaryReader br;
        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();
            models = new List<Model>(0);


            FileStream raw_anim = new FileStream(@"binary_files\raw_anim.bin", FileMode.Open);
            br = new BinaryReader(raw_anim);
            br.BaseStream.Position = 0x10;

            Title = "Press E key to freeze light position";

            Model model = new DAE(@"debug_files\BB00\BB00.dae");
            model.Compile();
            models.Add(model); 
            
            model = new DAE(@"debug_files\H_EX500\H_EX500.dae");
            model.Compile();
            model.Skeleton.Location = new Vector3(-100, 0, 0);
            models.Add(model);
            Camera.Current.Target = model;

            model = new MDLX(@"binary_files\H_EX510\H_EX510.mdlx");
            model.Compile();
            model.Skeleton.Location = new Vector3(100, 0, 0);
            models.Add(model);

            /*for (int i = 0; i < 100; i++)
            {
                Model model = new DAE(@"debug_files\H_EX500\H_EX500.dae");
                model.Compile();
                model.Skeleton.TransformMatrix = Matrix4.CreateTranslation(-2500 + i*50,0,0);
                models.Add(model);
            }
*/

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