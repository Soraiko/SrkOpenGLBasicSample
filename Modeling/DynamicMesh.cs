using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class DynamicMesh : Mesh
    {
        public DynamicMesh(Model model):base(model)
        {

        }

        public void Update(float[] matricesBuffer, int bonesCount)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, this.Model.UniformBufferObject);

            IntPtr matricesPtr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);
            System.Runtime.InteropServices.Marshal.Copy(matricesBuffer, 0, matricesPtr, bonesCount*16);
            GL.UnmapBuffer(BufferTarget.UniformBuffer);

            if (this.Model.matrices_loc < 0)
                this.Model.matrices_loc = GL.GetUniformBlockIndex(this.shader.Handle, "transform_data");

            if (this.Model.matrices_loc > -1)
            {
                GL.UniformBlockBinding(this.shader.Handle, this.Model.matrices_loc, 0);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, this.Model.UniformBufferObject);
            }
        }

        public new void Draw()
        {
            this.shader.Use();

            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);

            if (IndexBufferObject>0)
                GL.DrawElements(PrimitiveType.Triangles, PrimitiveCount, DrawElementsType.UnsignedShort, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveCount);

            GL.UseProgram(0);
        }
    }
}
