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

            //StaticReferences.Light0_Position.X = (float)(3000 * Math.Cos(angle));
            //StaticReferences.Light0_Position.Z = (float)(3000 * Math.Sin(angle));
            //if (keyboardState.IsKeyDown(Key.R))
                angle += 0.01f;

            map.Update();
            for (int i=0;i<models[0].Skeleton.Joints.Count;i++)
            {
                Matrix4 m = Matrix4.Identity;
                m.M11 = br.ReadSingle();
                m.M12 = br.ReadSingle();
                m.M13 = br.ReadSingle();
                m.M14 = br.ReadSingle();

                m.M21 = br.ReadSingle();
                m.M22 = br.ReadSingle();
                m.M23 = br.ReadSingle();
                m.M24 = br.ReadSingle();

                m.M31 = br.ReadSingle();
                m.M32 = br.ReadSingle();
                m.M33 = br.ReadSingle();
                m.M34 = br.ReadSingle();

                m.M41 = br.ReadSingle();
                m.M42 = br.ReadSingle();
                m.M43 = br.ReadSingle();
                m.M44 = br.ReadSingle();
                models[0].Skeleton.Joints[i].Matrix = m;
            }

            if (keyboardState.IsKeyDown(Key.KeypadAdd))
            {
                y += 2f;
            }
            if (keyboardState.IsKeyDown(Key.KeypadSubtract))
            {
                y -= 2f;
            }
            if (keyboardState.IsKeyUp(Key.E))
            {
                StaticReferences.Light0_Position = Camera.Current.LookAt;
                StaticReferences.Light0_Position.Y = y;
            }

            models[3].Skeleton.TransformMatrix = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(StaticReferences.Light0_Position);

            models[0].Skeleton.TransformMatrix = Matrix4.CreateRotationY((float)angle) * Matrix4.CreateTranslation(-200, 0f, -200f);
            models[1].Skeleton.TransformMatrix = Matrix4.CreateRotationY((float)angle) * Matrix4.CreateTranslation(0f, 100f, -200f);
            models[2].Skeleton.TransformMatrix = Matrix4.CreateRotationY((float)angle) * Matrix4.CreateTranslation(200, 0f, -200f);

            if (br.BaseStream.Position>= br.BaseStream.Length)
                br.BaseStream.Position = 0x10;
            
            
            for (int i = 0; i < models.Count; i++)
                models[i].Update();


            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }
        double angle = 0;
        float y = 120;

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

        public BinaryReader br;
        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();
            models = new List<Model>(0);

            FileStream raw_anim = new FileStream(@"binary_files\raw_anim.bin", FileMode.Open);
            br = new BinaryReader(raw_anim);
            br.BaseStream.Position = 0x10;

            Title = "Press E key to set light position";
            map = new DAE(@"debug_files\BB00\BB00.dae").Parse();
            map.Compile();

            for (int i=0;i<1;i++)
            {
                Model model = new DAE(@"debug_files\H_EX500\H_EX500.dae").Parse();
                model.Compile();
                models.Add(model);

                model = new DAE(@"debug_files\cube\cube.dae").Parse();
                model.Compile();
                models.Add(model);

                model = new DAE(@"debug_files\P_EX100\P_EX100.dae").Parse();
                model.Compile();
                models.Add(model);

                model = new DAE(@"debug_files\cube\cube.dae").Parse();
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