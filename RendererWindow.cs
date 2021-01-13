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

        double angle = 0;

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
            angle += 0.02;
            models[models.Count - 2].Skeleton.SetRotationY((float)angle);
            models[models.Count - 1].Skeleton.SetRotationY((float)angle);

            //StaticReferences.Light0_Position.X = (float)(3000 * Math.Cos(angle));
            //StaticReferences.Light0_Position.Z = (float)(3000 * Math.Sin(angle));
            StaticReferences.Light0_Position = Camera.Current.Target.Skeleton.Position + Camera.Current.Target.Controller.HeadPosition;

            for (int i = 0; i < models.Count; i++)
                models[i].Update();


            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }


        public static Color BackgroundColor = new Color(50, 50, 50, 255);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < models.Count; i++)
                models[i].Draw();

            Title = this.RenderFrequency.ToString();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        List<Model> models;

        protected override void OnLoad(EventArgs e)
        {
            var random = new Random();

            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();
            models = new List<Model>(0);

            Title = "Press E key to freeze light position";

            Model model = new MDLX(@"binary_files\bb00.map");
            model.Compile();
            models.Add(model); 
            
            for (int i=0;i<50;i++)
            {
                model = new DAE(@"debug_files\H_EX500\H_EX500.dae");
                model.Compile();
                model.Skeleton.Position = new Vector3(-100, 0, 0);
                model.Controller.Moveset.AnimationFrame = random.Next(0, 200);
                System.Threading.Thread.Sleep(1);
                float x = random.Next(-500, 500);
                System.Threading.Thread.Sleep(1);
                float y = random.Next(-500, 500);
                model.Skeleton.Position = new Vector3(x,0,y);

                models.Add(model);
                Camera.Current.Target = model;
            }

            model = new MDLX(@"binary_files\H_EX510\H_EX510.mdlx");
            model.Compile();
            model.Skeleton.Position = new Vector3(100, 0, 0);
            models.Add(model);


            /*string[] mdlxes = Directory.GetFiles(@"E:\Jeux\KingdomHearts\app_KH2Tools\export\@KH2\obj\", "*.mdlx");
            int count = 0;
            while (count<50)
            {
                int randomIndex = random.Next(0, mdlxes.Length);
                FileInfo f = new FileInfo(mdlxes[randomIndex]);
                if (f.Name.Contains("H_") && f.Length>200*1024)
                {
                    model = new MDLX(f.FullName);
                    model.Compile();
                    float x = random.Next(-500, 500);
                    System.Threading.Thread.Sleep(1);
                    float y = random.Next(-500, 500);
                    model.Skeleton.Position = new Vector3(x,0,y);
                    models.Add(model);
                    count++;
                }
            }*/


            model = new DAE(@"debug_files\cube\cube.dae");
            model.Compile();
            model.Skeleton.Position = new Vector3(-100, 0, 100);
            models.Add(model);

            model = new DAE(@"debug_files\cube2\cube2.dae");
            model.Compile();
            model.Skeleton.Position = new Vector3(100, 0, 100);
            models.Add(model);

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