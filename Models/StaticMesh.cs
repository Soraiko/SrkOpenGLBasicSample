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

        public void Compile()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            int mode = 0;

            if (this.Positions.Count > 0)
                mode = mode | 1;
            if (this.TextureCoords.Count > 0)
                mode = mode | 2;
            if (this.Normals.Count > 0)
                mode = mode | 4;
            if (this.Colors.Count > 0)
                mode = mode | 8;

            int VertexStride = 0;

            int positionsOffset = -1;
            int textureCoordinatesOffset = -1;
            int colorsOffset = -1;
            int normalsOffset = -1;

            switch (mode)
            {
                case 1: //0001
                    this.shader = Shader.P;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                    }
                    positionsOffset = 0;
                    VertexStride = POSITIONS_SIZE;
                    break;
                case 3: //0011
                    this.shader = Shader.TP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write(this.TextureCoords[i].X);
                        binaryWriter.Write(this.TextureCoords[i].Y);
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    break;
                case 5: //0101
                    break;
                case 7: //0111
                    this.shader = Shader.NTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write(this.TextureCoords[i].X);
                        binaryWriter.Write(this.TextureCoords[i].Y);
                        binaryWriter.Write(this.Normals[i].X);
                        binaryWriter.Write(this.Normals[i].Y);
                        binaryWriter.Write(this.Normals[i].Z);
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    normalsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    break;
                case 11: //1011
                    this.shader = Shader.CTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write(this.TextureCoords[i].X);
                        binaryWriter.Write(this.TextureCoords[i].Y);
                        binaryWriter.Write(this.Colors[i].R);
                        binaryWriter.Write(this.Colors[i].G);
                        binaryWriter.Write(this.Colors[i].B);
                        binaryWriter.Write(this.Colors[i].A);
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    colorsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + COLORS_SIZE;
                    break;
                case 13: //1101
                    break;
                case 15: //1111
                    this.shader = Shader.CNTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write(this.TextureCoords[i].X);
                        binaryWriter.Write(this.TextureCoords[i].Y);
                        binaryWriter.Write(this.Normals[i].X);
                        binaryWriter.Write(this.Normals[i].Y);
                        binaryWriter.Write(this.Normals[i].Z);
                        binaryWriter.Write(this.Colors[i].R);
                        binaryWriter.Write(this.Colors[i].G);
                        binaryWriter.Write(this.Colors[i].B);
                        binaryWriter.Write(this.Colors[i].A);
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    normalsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    colorsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE + COLORS_SIZE;
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

            if (this.Indices.Count>0)
            {
                IndexBufferObject = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, this.Indices.Count * sizeof(ushort), this.Indices.ToArray(), BufferUsageHint.StaticDraw);

                this.PrimitiveCount = this.Indices.Count;
                this.Indices.Clear();
            }

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, positionsOffset);
            GL.EnableVertexAttribArray(0);

            if (textureCoordinatesOffset > -1)
            {
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, textureCoordinatesOffset);
                GL.EnableVertexAttribArray(1);
            }

            if (normalsOffset> -1)
            {
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, VertexStride, normalsOffset);
                GL.EnableVertexAttribArray(2);
            }

            if (colorsOffset > -1)
            {
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, colorsOffset);
                GL.EnableVertexAttribArray(3);
            }

            if (this.PrimitiveCount == 0)
            {
                this.PrimitiveCount = this.Data.Length / VertexStride;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);


        }

        public void Draw()
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
