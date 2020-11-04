using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Mesh
    {
        public Texture Texture;
        public void Append_3D_Data(Vector4 position, Vector2 textureCoordinate, Vector3 normal, Color color, int influence)
        {

        }

        BinaryReader binaryReader;
        public byte[] Data;

        public unsafe void Compile(ref Matrix4[] matrices)
        {
            /*MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);


            Data = new byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(Data, 0, Data.Length);
            binaryWriter.Close();

            int VertexStride = 0;

            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            fixed (byte* p = this.Data)
            GL.BufferData(BufferTarget.ArrayBuffer, this.Data.Length - 8, ((IntPtr)p) + 8, BufferUsageHint.StaticDraw);

            IndexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.Indices.Count * sizeof(ushort), this.Indices.ToArray(), BufferUsageHint.StaticDraw);
            this.Indices.Clear();

            this.shader = Shader.VPNT;
            VertexStride = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float) * 3;
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3 + sizeof(float) * 2);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, 0);
            GL.EnableVertexAttribArray(3);

            this.PrimitiveCount = (this.Data.Length - 8) / VertexStride;

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            Vertices.Clear();
            if (Influences != null && Influences.Length > 0) Array.Clear(Influences, 0, Influences.Length);
            //if (TextureCoordinates != null && TextureCoordinates.Length > 0) Array.Clear(TextureCoordinates, 0, TextureCoordinates.Length);
            if (Normals != null && Normals.Length > 0) Array.Clear(Normals, 0, Normals.Length);
            if (Colors != null && Colors.Length > 0) Array.Clear(Colors, 0, Colors.Length);*/
        }

        int PrimitiveCount;
        Shader shader;

        int IndexBufferObject;
        int VertexBufferObject;
        int VertexArrayObject;

        public void Draw(Matrix4[] skeleton)
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);

            this.shader.Use();
            GL.DrawElements(PrimitiveType.Triangles, PrimitiveCount, DrawElementsType.UnsignedShort, 0);

            GL.UseProgram(0);
        }






    }
}
