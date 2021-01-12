using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Model : Controllable
    {
        public int UniformBufferObject = -1;
        public int matrices_loc = -1;

        public string Directory;
        public string Name;
        public Mesh[] Meshes;
        public Model Reference;

        static List<Model> References;

        static Model()
        {
            References = new List<Model>(0);
        }

        public Model(string filename)
        {
            this.Name = Path.GetFileNameWithoutExtension(filename);
            this.Directory = Path.GetDirectoryName(filename);

            for (int i=0;i< References.Count;i++)
            {
                if (References[i].Name == this.Name && References[i].Directory == this.Directory)
                {
                    this.Reference = References[i];
                    break;
                }
            }

            if (this.Reference == null)
                References.Add(this);
            else
            {
                this.Meshes = Reference.Meshes;
                this.Skeleton = Reference.Skeleton.Clone();
                this.Skeleton.Compile(this);
            }
        }

        public void Compile()
        {
            for (int i = 0; i < this.Meshes.Length; i++)
                this.Meshes[i].Compile(this.Skeleton);
        }

        public void Update()
        {
            var skeleton = this.Skeleton;
            if (skeleton != null)
            {
                skeleton.UpdateRotate();
                //skeleton.ComputeMatrices();
            }
        }

        static int lastTexture = -1;
        static int lastBump = -1;
        public void Draw()
        {
            GL.Enable(EnableCap.Texture2D);
            if (this.Meshes.Length>0)
                this.Meshes[0].Update(this.Skeleton);

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

                this.Meshes[i].Draw();
            }
        }
    }
}
