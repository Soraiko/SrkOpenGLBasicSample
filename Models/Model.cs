using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Model
    {
        public string Directory;
        public string Name;
        public Skeleton Skeleton;
        public Mesh[] Meshes;


        public void Compile()
        {
            for (int i = 0; i < this.Meshes.Length; i++)
            {
                this.Meshes[i].Compile(this.Skeleton);
            }
        }

        public void Update()
        {
            /*Matrix4[] matrices = new Matrix4[0];
            if (this.Skeleton != null && this.Skeleton.Matrices != null && this.Skeleton.Matrices.Length > 0)
            {
                matrices = new Matrix4[this.Skeleton.Matrices.Length];
                Array.Copy(this.Skeleton.Matrices, matrices, matrices.Length);
            }*/
            this.Skeleton.ComputeMatrices();
        }

        static int lastTexture = -1;
        static int lastBump = -1;
        public void Draw()
        {
            GL.Enable(EnableCap.Texture2D);

            for (int i = 0; i < this.Meshes.Length; i++)
            {
                if (this.Meshes[i].Texture.Integer != lastTexture)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, this.Meshes[i].Texture.Integer);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, this.Meshes[i].Texture.TextureMinFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, this.Meshes[i].Texture.TextureMinFilter);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, this.Meshes[i].Texture.TextureWrapS);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, this.Meshes[i].Texture.TextureWrapT);
                }
                if (this.Meshes[i].BumpMapping.Integer != lastBump)
                {
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, this.Meshes[i].BumpMapping.Integer);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, this.Meshes[i].BumpMapping.TextureMinFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, this.Meshes[i].BumpMapping.TextureMinFilter);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, this.Meshes[i].BumpMapping.TextureWrapS);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, this.Meshes[i].BumpMapping.TextureWrapT);
                }
                lastTexture = this.Meshes[i].Texture.Integer;
                lastBump = this.Meshes[i].BumpMapping.Integer;

                this.Meshes[i].Update(this.Skeleton);
                this.Meshes[i].Draw();
            }
        }
    }
}
