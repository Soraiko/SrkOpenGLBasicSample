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

            if (true||keyboardState.IsKeyDown(Key.P))
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

            if (keyboardState.IsKeyDown(Key.B) && oldKeyboardState.IsKeyUp(Key.B))
            {
                Blend = !Blend;
                if (Blend)
                    GL.Enable(EnableCap.Blend);
                else
                    GL.Disable(EnableCap.Blend);
            }
            if (keyboardState.IsKeyDown(Key.D) && oldKeyboardState.IsKeyUp(Key.D))
            {
                DepthTest = !DepthTest;
                if (DepthTest)
                    GL.Enable(EnableCap.DepthTest);
                else
                    GL.Disable(EnableCap.DepthTest);
            }
            if (keyboardState.IsKeyDown(Key.A) && oldKeyboardState.IsKeyUp(Key.A))
            {
                AlphaTest = !AlphaTest;
                if (AlphaTest)
                    GL.Enable(EnableCap.AlphaTest);
                else
                    GL.Disable(EnableCap.AlphaTest);
            }


            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            base.OnUpdateFrame(e);
        }
        bool Blend = false;
        bool DepthTest = true;
        bool AlphaTest = true;

        public static Color BackgroundColor = new Color(50,50,50,255);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            sora.Draw();
            mdl.Draw();

            if (DateTime.Now.Millisecond <100)
            Title = ((int)base.RenderFrequency).ToString();
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


            /*mdl = new DAE(@"D:\Desktop\KHDebug\KHDebug\bin\DesktopGL\AnyCPU\Debug\Content\Models\TT08\TT08 - Copie.dae");
            mdl.Compile();*/


            FileStream input = new FileStream(@"binary_files\raw_anim.bin", FileMode.Open);
            br = new BinaryReader(input);
            br.BaseStream.Position = 0x10;


            mdl = new Model();
            mdl.Meshes = new Mesh[3];
            for (int i=0;i<mdl.Meshes.Length;i++)
            {
                mdl.Meshes[i] = new Mesh();
                //mdl.Meshes[i].primitiveType = PrimitiveType.TriangleStrip;

                mdl.Meshes[i].Colors = new Color[]
                {
                    new Color(255, 0, 0, 255),
                    new Color(0, 255, 0, 255),
                    new Color(0, 0, 255, 255),
                    new Color(255, 255, 255, 255)
                };

                mdl.Meshes[i].Texture = Texture.LoadTexture("texture"+i.ToString("d3")+".png", TextureMinFilter.Nearest, TextureWrapMode.Repeat, TextureWrapMode.Repeat);
                mdl.Meshes[i].TextureCoordinates = new Vector2[]
                {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,0),
                new Vector2(1,0),
                new Vector2(0,1),
                new Vector2(1,1)
                };

                mdl.Meshes[i].Vertices =
                    new System.Collections.Generic.List<Vector4>(
                    new Vector4[]
                {
                new Vector4(-100,100,0,1),
                new Vector4(-100,-100,0,1f),
                new Vector4(100,100,0,1),
                new Vector4(100,100,0,1),
                new Vector4(-100,-100,0,1f),
                new Vector4(100,-100,0,1)
                });
                mdl.Meshes[i].Colors =

                    new Color[]
                {
                new Color(255,0,0,255),
                new Color(0,255,0,255),
                new Color(0,0,255,255),
                new Color(0,0,255,255),
                new Color(0,255,0,255),
                new Color(255,255,255,255)
                };
            }

            for (int i = 0; i < mdl.Meshes[0].Vertices.Count; i++)
            {
                mdl.Meshes[0].Vertices[i] = Vector4.Transform(mdl.Meshes[0].Vertices[i], Matrix4.CreateRotationX(-OpenTK.MathHelper.PiOver2));
                mdl.Meshes[0].Vertices[i] = Vector4.Transform(mdl.Meshes[0].Vertices[i], Matrix4.CreateScale(1000f));
                mdl.Meshes[0].Vertices[i] = Vector4.Transform(mdl.Meshes[0].Vertices[i], Matrix4.CreateTranslation(0, -100, 0));
                mdl.Meshes[0].TextureCoordinates[i].X *= 1000f;
                mdl.Meshes[0].TextureCoordinates[i].Y *= 1000f;
            }

            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[0] * 1f);
            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[1] * 1f);
            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[2] * 1f);
            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[3] * 1f);
            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[4] * 1f);
            mdl.Meshes[1].Vertices.Add(mdl.Meshes[1].Vertices[5] * 1f);

            Color[] colors = new Color[mdl.Meshes[1].Colors.Length * 2];
            Array.Copy(mdl.Meshes[1].Colors, 0, colors, 0, mdl.Meshes[1].Colors.Length);
            Array.Copy(mdl.Meshes[1].Colors, 0, colors, mdl.Meshes[1].Colors.Length, mdl.Meshes[1].Colors.Length);
            mdl.Meshes[1].Colors = colors;

            Vector2[] texCoords = new Vector2[mdl.Meshes[1].TextureCoordinates.Length * 2];
            Array.Copy(mdl.Meshes[1].TextureCoordinates, 0, texCoords, 0, mdl.Meshes[1].TextureCoordinates.Length);
            Array.Copy(mdl.Meshes[1].TextureCoordinates, 0, texCoords, mdl.Meshes[1].TextureCoordinates.Length, mdl.Meshes[1].TextureCoordinates.Length);
            mdl.Meshes[1].TextureCoordinates = texCoords;

            for (int i = 0; i < mdl.Meshes[1].Vertices.Count / 2; i++)
                mdl.Meshes[1].Vertices[i] = Vector4.Transform(mdl.Meshes[1].Vertices[i], Matrix4.CreateRotationY(OpenTK.MathHelper.PiOver2));

            for (int i = 0; i < mdl.Meshes[1].Vertices.Count; i++)
                mdl.Meshes[1].Vertices[i] = Vector4.Transform(mdl.Meshes[1].Vertices[i], Matrix4.CreateRotationY(OpenTK.MathHelper.PiOver4));


            for (int i = 0; i < mdl.Meshes[2].Vertices.Count; i++)
            {
                mdl.Meshes[2].Vertices[i] = Vector4.Transform(mdl.Meshes[2].Vertices[i], Matrix4.CreateTranslation(2, 2, -150));
            }
            mdl.Compile();

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