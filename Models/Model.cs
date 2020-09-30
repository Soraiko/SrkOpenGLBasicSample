using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Model
    {
        public Skeleton Skeleton;
        public Mesh[] Meshes;

        public void Compile()
        {
            for (int i = 0; i < this.Meshes.Length; i++)
            {
                this.Meshes[i].Compile(this.Skeleton);
            }
        }
        public void Draw()
        {
            for (int i=0;i<this.Meshes.Length;i++)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, this.Meshes[i].Texture);

                this.Meshes[i].Draw(this.Skeleton);
            }
        }
    }
}
