using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class StaticReferences
    {
        public static void GraphicsSettings()
        {
            GL.Enable(EnableCap.CullFace);

            /*GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0,0,1});
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] {1,1,1,1});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1, 1, 1, 1 });*/

            //GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthFunc(OpenTK.Graphics.OpenGL.DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
        }

        public static Texture whitePixel1x1;
        public static void InitReferences()
        {
            whitePixel1x1 = Texture.LoadTexture(@"resources\whitePixel1x1.png", TextureMinFilter.Nearest, TextureWrapMode.Clamp, TextureWrapMode.Clamp);
            Camera.Current = new Camera(200f);
            Camera.Current.LookAt = new OpenTK.Vector3(0,120,0);
            Camera.Current.SkipTransitions();
        }
    }
}
