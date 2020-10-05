using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.IO;
using System.Diagnostics;

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

                for (int i = 0; i < mdl.Skeleton.Joints.Length; i++)
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

                    mdl.Skeleton.Joints[i].TransformLocal = m;

                }
                mdl.Skeleton.ComputeMatrices(Matrix4.CreateScale(1f));
            }




            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }

        public static Color BackgroundColor;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /*Stopwatch stp = new Stopwatch();
            stp.Start();
            for (int i=0;i<1;i++)*/
          map.Draw();
            mdl.Draw();
            /*totalTicks += stp.Elapsed.Ticks;
            ticksCount++;
            Console.WriteLine(totalTicks/(float)ticksCount);*/
            if (DateTime.Now.Millisecond <100)
            Title = ((int)base.RenderFrequency).ToString();
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
        long totalTicks = 0;
        long ticksCount = 0;


        Model mdl;
        Model map;
        BinaryReader br;
        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();

            mdl = new DAE(@"debug_mode\H_EX500\H_EX500.dae");
            mdl.Compile();

            map = new DAE(@"debug_mode\BB00\BB00.dae");
            map.Compile();


            FileStream input = new FileStream(@"debug_mode\raw_anim.bin", FileMode.Open);
            br = new BinaryReader(input);
            br.BaseStream.Position = 0x10;

            /*mdl = new Model();
            mdl.Meshes = new Mesh[1];
            mdl.Meshes[0] = new Mesh();
            mdl.Meshes[0].primitiveType = PrimitiveType.TriangleStrip;



            mdl.Meshes[0].Colors = new Color[]
            {
                new Color(255, 0, 0, 255),
                new Color(0, 255, 0, 255),
                new Color(0, 0, 255, 255),
                new Color(255, 255, 255, 255)
            };

            mdl.Meshes[0].Texture = Texturing.LoadTexture("texture.png", TextureMinFilter.Nearest,TextureWrapMode.Clamp);
            mdl.Meshes[0].TextureCoordinates = new Vector2[]
            {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,0),
                new Vector2(1,1)
            };

            mdl.Meshes[0].Vertices = 
                new System.Collections.Generic.List<Vector4>(
                new Vector4[]
            {
                new Vector4(0,100,0,1),
                new Vector4(0,0,0,1f),
                new Vector4(100,100,0,1),
                new Vector4(100,0,0,1)
            });

            Skeleton s = new Skeleton(4);
            s.Joints[0] = new Joint("bone000", Matrix4.CreateScale(1f)); 
            s.Joints[1] = new Joint("bone001", Matrix4.CreateScale(1f));
            s.Joints[2] = new Joint("bone002", Matrix4.CreateScale(1f));
            s.Joints[3] = new Joint("bone003", Matrix4.CreateScale(1f));

            s.ComputeMatrices(Matrix4.CreateScale(1f));
            mdl.Skeleton = s;

            mdl.Meshes[0].Influences = new int[][]
            {
                new int[] {0},
                new int[] {1},
                new int[] {2},
                new int[] {3}
            };
            mdl.Compile();*/

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