using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class StaticReferences
    {
        public static Vector3 Light0_Position = new Vector3(0, 0, 0);
        public static Vector3 Light0_Color = new Vector3(1,1,1);
        public static float Light0_AmbientStrentgh = 0.5f;

        public static void GraphicsSettings()
        {
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthFunc(OpenTK.Graphics.OpenGL.DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
        }

        public static Texture whitePixel1x1;
        public static Texture bumpPixel1x1;
        public static void InitReferences()
        {
            whitePixel1x1 = Texture.LoadTexture(@"resources\whitePixel1x1.png", TextureMinFilter.Nearest, TextureWrapMode.Repeat, TextureWrapMode.Repeat);
            bumpPixel1x1 = Texture.LoadTexture(@"resources\bumpPixel1x1.png", TextureMinFilter.Nearest, TextureWrapMode.Repeat, TextureWrapMode.Repeat);

            Camera.Current = new Camera(400f);
            Camera.Current.LookAt = new OpenTK.Vector3(0,0,100);
            Camera.Current.SkipInterpolation();
        }
    }
}
