using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class StaticMesh : Mesh
    {
        public StaticMesh(Model model) : base(model)
        {

        }

        public new void Draw()
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);

            this.shader.Use();
            if (IndexBufferObject>0)
                GL.DrawElements(PrimitiveType.Triangles, PrimitiveCount, DrawElementsType.UnsignedShort, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveCount);

            GL.UseProgram(0);
        }
    }
}
