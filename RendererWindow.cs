using System;
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
            {
                WindowState = WindowState.Fullscreen;
            }
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

            if (keyboardState.IsKeyDown(Key.P))
            {
                if (br.BaseStream.Position >= br.BaseStream.Length)
                    br.BaseStream.Position = 0x10;

                for (int i = 0; i < sora.Skeleton.Joints.Length; i++)
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

                    sora.Skeleton.Joints[i].TransformLocal = m;

                }
                sora.Skeleton.ComputeMatrices(Matrix4.CreateTranslation(0,0,-150));
            }
            if (keyboardState.IsKeyDown(Key.R))
                Camera.Current.RotationY = -300000000f;
            if (keyboardState.IsKeyDown(Key.T))
                Camera.Current.RotationY = 3f;

            Title = Camera.Current.RotationY.ToString();

            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }

        public static Color BackgroundColor = new Color(50,50,50,255);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            sora.Draw();
            mdl.Draw();


            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }



        Model sora;
        Model mdl;
        BinaryReader br;
        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();



            sora = new DAE(@"debug_files\H_EX500\H_EX500.dae");
            sora.Compile();

            /*sora = new DAE(@"C:\Users\Cdministrateur\Desktop\furnitures\local.dae");
            sora.Compile();*/


            /*mdl = new DAE(@"D:\Desktop\KHDebug\KHDebug\bin\DesktopGL\AnyCPU\Debug\Content\Models\TT08\TT08 - Copie.dae");
            mdl.Compile();*/


            FileStream input = new FileStream(@"binary_files\raw_anim.bin", FileMode.Open);
            br = new BinaryReader(input);
            br.BaseStream.Position = 0x10;

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