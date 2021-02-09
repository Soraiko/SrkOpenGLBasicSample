using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Model : Resource
    {
        public int UniformBufferObject = -1;
        public int matrices_loc = -1;
        public bool locationsFound = false;

        public ModelController Controller;
        public Skeleton Skeleton;
        public Mesh[] Meshes;
        public List<int[]> meshGroups;

        public Model(string filename) : base(filename)
        {
            this.meshGroups = new List<int[]>(0);
        }

        public void Compile()
        {
            if (this.Reference == null)
            {
                if (this.meshGroups.Count == 0)
                    this.meshGroups.Add(new int[] { 0, this.Meshes.Length });
                for (int i = 0; i < this.Meshes.Length; i++)
                    this.Meshes[i].Compile();
            }
            else
                this.meshGroups = (this.Reference as Model).meshGroups;

            var skeleton = this.Skeleton;
            if (skeleton!= null)
            {
                this.UniformBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, this.UniformBufferObject);
                GL.BufferData(BufferTarget.UniformBuffer, skeleton.MatricesBuffer.Length * sizeof(float), skeleton.MatricesBuffer, BufferUsageHint.DynamicCopy);
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
        }

        public void Update()
        {
            var skeleton = this.Skeleton;
            if (skeleton != null)
            {
                var controller = this.Controller;
                if (controller != null)
                {
                    var moveset = controller.Moveset;
                    if (moveset != null)
                        controller.RenderNextFrame();
                }
                skeleton.UpdateRotate();
                this.SkipRender = skeleton.ComputeMatrices();
                if (controller != null)
                {
                    controller.ProceedCalculations();
                }


                GL.BindBuffer(BufferTarget.UniformBuffer, this.UniformBufferObject);
                IntPtr matricesPtr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);
                System.Runtime.InteropServices.Marshal.Copy(skeleton.MatricesBuffer, 0, matricesPtr, skeleton.Joints.Count * 16);
                GL.UnmapBuffer(BufferTarget.UniformBuffer);

            }
        }

        static int lastTexture = -1;
        static int lastBump = -1;

        public void Draw(int groupIndex)
        {
            if (this.SkipRender)
                return;

            if (this.locationsFound)
            {
                GL.UniformBlockBinding(this.Meshes[0].shader.Handle, this.matrices_loc, 0);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, this.UniformBufferObject);
            }
            else
            {
                this.matrices_loc = GL.GetUniformBlockIndex(this.Meshes[0].shader.Handle, "transform_data");

                if (this.matrices_loc > -1)
                    this.locationsFound = true;
            }
            GL.Enable(EnableCap.Texture2D);

            int start = this.meshGroups[groupIndex][0];
            int end = start + this.meshGroups[groupIndex][1];

            for (int i = start; i < end; i++)
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
