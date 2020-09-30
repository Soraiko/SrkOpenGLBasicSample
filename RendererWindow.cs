using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.IO;

namespace SrkOpenGLBasicSample
{
    public class RendererWindow : GameWindow
    {
        public RendererWindow(int width, int height, int sampleCount) : base(width, height, new GraphicsMode(ColorFormat.Empty, 0, 0, sampleCount, ColorFormat.Empty, 1))
        {
            if (Preferences.Fullscreen)
            {
                WindowState = WindowState.Fullscreen;
            }
            this.CursorVisible = true;
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

        public static Color BackgroundColor;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            mdl.Draw();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }



        protected override void OnLoad(EventArgs e)
        {
            StaticReferences.GraphicsSettings();
            StaticReferences.InitReferences();

            mdl = new DAE(@"H_EX500\H_EX500.dae");

            mdl.Compile();

            FileStream input = new FileStream(@"anim.bin", FileMode.Open);
            BinaryReader br = new BinaryReader(input);
            br.BaseStream.Position = 0x10;

            for (int i=0;i< mdl.Skeleton.Joints.Length;i++)
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
            /*mdl.Meshes = new Mesh[1];
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

            mdl.Meshes[0].Vertices = new Vector4[]
            {
                new Vector4(0,0,0,1f),
                new Vector4(0,100,0,1),
                new Vector4(100,0,0,1),
                new Vector4(100,100,0,1)
            };

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
            };*/


            OnUpdateFrame(null);

            base.OnLoad(e);
        }
        Model mdl;

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            if (Camera.Current != null)
                Camera.Current.ProjectionMatrixDirty = true;

            base.OnResize(e);
        }
    }
}