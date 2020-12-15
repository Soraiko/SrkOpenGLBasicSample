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
        public StaticMesh()
        {

        }

        const int POSITIONS_SIZE = 3 * sizeof(float);
        const int TEXTURECOORDINATES_SIZE = 2 * sizeof(float);
        const int COLORS_SIZE = 4 * sizeof(byte);
        const int NORMALS_SIZE = 3 * sizeof(float);

        public unsafe void Compile()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            int mode = 0;

            if (this.positions.Count > 0)
                mode = mode | 1;
            if (this.textureCoordinates.Count > 0)
                mode = mode | 2;
            if (this.colors.Count > 0)
                mode = mode | 4;
            if (this.normals.Count > 0)
                mode = mode | 8;

            int VertexStride = 0;

            int positionsOffset = -1;
            int textureCoordinatesOffset = -1;
            int colorsOffset = -1;
            int normalsOffset = -1;

            switch (mode)
            {
                case 1: //0001
                    this.shader = Shader.VP;
                    for (int i = 0; i < this.positions.Count; i++)
                    {
                        binaryWriter.Write(this.positions[i].X);
                        binaryWriter.Write(this.positions[i].Y);
                        binaryWriter.Write(this.positions[i].Z);
                    }
                    positionsOffset = 0;
                    VertexStride = POSITIONS_SIZE;
                    break;
                case 3: //0011
                    this.shader = Shader.VPT;
                    for (int i = 0; i < this.positions.Count; i++)
                    {
                        binaryWriter.Write(this.positions[i].X);
                        binaryWriter.Write(this.positions[i].Y);
                        binaryWriter.Write(this.positions[i].Z);
                    }
                    positionsOffset = 0;
                    for (int i = 0; i < this.textureCoordinates.Count; i++)
                    {
                        binaryWriter.Write(this.textureCoordinates[i].X);
                        binaryWriter.Write(this.textureCoordinates[i].Y);
                    }
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    break;
                case 5: //0101
                    this.shader = Shader.VPC;
                    for (int i = 0; i < this.positions.Count; i++)
                    {
                        binaryWriter.Write(this.positions[i].X);
                        binaryWriter.Write(this.positions[i].Y);
                        binaryWriter.Write(this.positions[i].Z);
                    }
                    positionsOffset = 0;
                    for (int i = 0; i < this.colors.Count; i++)
                    {
                        binaryWriter.Write(this.colors[i].R);
                        binaryWriter.Write(this.colors[i].G);
                        binaryWriter.Write(this.colors[i].B);
                        binaryWriter.Write(this.colors[i].A);
                    }
                    colorsOffset = POSITIONS_SIZE;
                    VertexStride = POSITIONS_SIZE + COLORS_SIZE;
                    break;
                case 7: //0111
                    this.shader = Shader.VPTC;
                    for (int i = 0; i < this.positions.Count; i++)
                    {
                        binaryWriter.Write(this.positions[i].X);
                        binaryWriter.Write(this.positions[i].Y);
                        binaryWriter.Write(this.positions[i].Z);
                    }
                    positionsOffset = 0;
                    for (int i = 0; i < this.textureCoordinates.Count; i++)
                    {
                        binaryWriter.Write(this.textureCoordinates[i].X);
                        binaryWriter.Write(this.textureCoordinates[i].Y);
                    }
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    for (int i = 0; i < this.colors.Count; i++)
                    {
                        binaryWriter.Write(this.colors[i].R);
                        binaryWriter.Write(this.colors[i].G);
                        binaryWriter.Write(this.colors[i].B);
                        binaryWriter.Write(this.colors[i].A);
                    }
                    colorsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + COLORS_SIZE;
                    break;
                case 11: //1011
                    this.shader = Shader.VPTN;
                    for (int i = 0; i < this.positions.Count; i++)
                    {
                        binaryWriter.Write(this.positions[i].X);
                        binaryWriter.Write(this.positions[i].Y);
                        binaryWriter.Write(this.positions[i].Z);
                    }
                    positionsOffset = 0;
                    for (int i = 0; i < this.textureCoordinates.Count; i++)
                    {
                        binaryWriter.Write(this.textureCoordinates[i].X);
                        binaryWriter.Write(this.textureCoordinates[i].Y);
                    }
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    for (int i = 0; i < this.normals.Count; i++)
                    {
                        binaryWriter.Write(this.normals[i].X);
                        binaryWriter.Write(this.normals[i].Y);
                        binaryWriter.Write(this.normals[i].Z);
                    }
                    normalsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    break;
                case 13: //1101
                    break;
                case 15: //1111
                    break;
            }
            Data = new byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(Data, 0, Data.Length);
            binaryWriter.Close();


            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, this.Data.Length, this.Data, BufferUsageHint.StaticDraw);

            IndexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.Indices.Count * sizeof(ushort), this.Indices.ToArray(), BufferUsageHint.StaticDraw);
            this.Indices.Clear();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3 + sizeof(float) * 2);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, 0);
            GL.EnableVertexAttribArray(3);

            this.PrimitiveCount = this.Data.Length / VertexStride;

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
