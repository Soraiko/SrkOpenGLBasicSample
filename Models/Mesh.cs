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
        public PrimitiveType primitiveType;
        public int Texture;
        public List<Vector4> Vertices = new List<Vector4>(0);
        public int[][] Influences;

        public Vector2[] TextureCoordinates;
        public Vector3[] Normals;
        public Color[] Colors;

        BinaryReader binaryReader;
        public byte[] Data;

        public unsafe void Compile(Skeleton skeleton)
        {
            Matrix4[] mats = new Matrix4[0];
            if (skeleton != null && skeleton.Matrices != null && skeleton.Matrices.Length > 0)
            {
                mats = new Matrix4[skeleton.Matrices.Length];
                Array.Copy(skeleton.Matrices, mats, mats.Length);
            }

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((int)this.primitiveType);
            int meshType = 0;
            if (Influences != null && Influences.Length > 0) meshType += 8;
            if (Colors != null && Colors.Length > 0) meshType += 4;
            if (Normals != null && Normals.Length > 0) meshType += 2;
            if (TextureCoordinates != null && TextureCoordinates.Length > 0) meshType += 1;
            binaryWriter.Write(meshType);

            int vertexIndex = 0;
            int VertexStride = 0; 

            switch (meshType)
            {
                case 0:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 1:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 2:
                    for (int i = 0; i < Vertices.Count; i++)
                    {

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 3:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 4:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 5:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 6:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 7:
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Vertices[i].X);
                        binaryWriter.Write(Vertices[i].Y);
                        binaryWriter.Write(Vertices[i].Z);
                    }
                    break;
                case 8:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Influences[i].Length);
                        for (int j=0;j< Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 9:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v4 = new Vector4(Vertices[vertexIndex].X, Vertices[vertexIndex].Y, Vertices[vertexIndex].Z, 1f) * Vertices[vertexIndex].W;
                            Vector4 v3Rev = Vector4.Transform(v4, Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 10:
                    for (int i = 0; i < Influences.Length; i++)
                    {

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 11:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 12:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 13:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 14:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
                case 15:
                    for (int i = 0; i < Influences.Length; i++)
                    {
                        binaryWriter.Write(Colors[i].R);
                        binaryWriter.Write(Colors[i].G);
                        binaryWriter.Write(Colors[i].B);
                        binaryWriter.Write(Colors[i].A);

                        binaryWriter.Write(Normals[i].X);
                        binaryWriter.Write(Normals[i].Y);
                        binaryWriter.Write(Normals[i].Z);

                        binaryWriter.Write(TextureCoordinates[i].X);
                        binaryWriter.Write(TextureCoordinates[i].Y);

                        binaryWriter.Write(Influences[i].Length);
                        for (int j = 0; j < Influences[i].Length; j++)
                        {
                            Vector4 v3Rev = Vector4.Transform(Vertices[vertexIndex], Matrix4.Invert(mats[Influences[i][j]]));
                            binaryWriter.Write(v3Rev.X);
                            binaryWriter.Write(v3Rev.Y);
                            binaryWriter.Write(v3Rev.Z);
                            binaryWriter.Write(Vertices[vertexIndex].W);
                            binaryWriter.Write(Influences[i][j]);
                            vertexIndex++;
                        }
                    }
                    break;
            }
            //binaryReader = new BinaryReader(memoryStream);
            Data = new byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(Data, 0, Data.Length);
            binaryWriter.Close();










            if (meshType < 8)
            {
                VertexBufferObject = GL.GenBuffer();
                VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(VertexArrayObject);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

                fixed (byte* p = this.Data)
                    GL.BufferData(BufferTarget.ArrayBuffer, this.Data.Length - 8, ((IntPtr)p) + 8, BufferUsageHint.StaticDraw);


                switch (meshType)
                {
                    case 0:
                        this.shader = new Shader(@"resources\graphics\vp_vert.c", @"resources\graphics\vp_frag.c");
                        VertexStride = sizeof(float) * 3;

                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, 0);
                        GL.EnableVertexAttribArray(0);
                        break;
                    case 1:
                        this.shader = new Shader(@"resources\graphics\vpt_vert.c", @"resources\graphics\vpt_frag.c");
                        VertexStride = sizeof(float) * 2 + sizeof(float) * 3;

                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 2);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, 0);
                        GL.EnableVertexAttribArray(1);
                        break;
                    case 2:
                        this.shader = new Shader(@"resources\graphics\vp_vert.c", @"resources\graphics\vp_frag.c");
                        VertexStride = sizeof(float) * 3 + sizeof(float) * 3;

                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, 0);
                        GL.EnableVertexAttribArray(3);
                        break;
                    case 3:
                        this.shader = new Shader(@"resources\graphics\vpnt_vert.c", @"resources\graphics\vpnt_frag.c");
                        VertexStride = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float) * 3;
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3 + sizeof(float) * 2);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, sizeof(float) * 3);
                        GL.EnableVertexAttribArray(1);

                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, 0);
                        GL.EnableVertexAttribArray(3);
                        break;
                    case 4:
                        this.shader = new Shader(@"resources\graphics\vpc_vert.c", @"resources\graphics\vpc_frag.c");
                        VertexStride = sizeof(byte) * 4 + sizeof(float) * 3;
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, 0);
                        GL.EnableVertexAttribArray(2);
                        break;
                    case 5:
                        this.shader = new Shader(@"resources\graphics\vpct_vert.c", @"resources\graphics\vpct_frag.c");
                        VertexStride = sizeof(byte) * 4 + sizeof(float) * 2 + sizeof(float) * 3;
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4 + sizeof(float) * 2);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4);
                        GL.EnableVertexAttribArray(1);

                        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, 0);
                        GL.EnableVertexAttribArray(2);
                        break;
                    case 6:
                        this.shader = new Shader(@"resources\graphics\vpc_vert.c", @"resources\graphics\vpc_frag.c");
                        VertexStride = sizeof(byte) * 4 + sizeof(float) * 3 + sizeof(float) * 3;
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4 + sizeof(float) * 3);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4);
                        GL.EnableVertexAttribArray(3);

                        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, 0);
                        GL.EnableVertexAttribArray(2);
                        break;
                    case 7:
                        this.shader = new Shader(@"resources\graphics\vpct_vert.c", @"resources\graphics\vpct_frag.c");
                        VertexStride = sizeof(byte) * 4 + sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float) * 3;

                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4 + sizeof(float) * 3 + sizeof(float) * 2);
                        GL.EnableVertexAttribArray(0);

                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4 + sizeof(float) * 3);
                        GL.EnableVertexAttribArray(1);

                        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, 0);
                        GL.EnableVertexAttribArray(2);

                        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexStride, sizeof(byte) * 4);
                        GL.EnableVertexAttribArray(3);
                        break;
                }
                this.PrimitiveCount = (this.Data.Length - 8) / VertexStride;

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            Vertices.Clear();
            if (Influences != null && Influences.Length > 0) Array.Clear(Influences, 0, Influences.Length);
            if (TextureCoordinates != null && TextureCoordinates.Length > 0) Array.Clear(TextureCoordinates, 0, TextureCoordinates.Length);
            if (Normals != null && Normals.Length > 0) Array.Clear(Normals, 0, Normals.Length);
            if (Colors != null && Colors.Length > 0) Array.Clear(Colors, 0, Colors.Length);
        }
        int PrimitiveCount;
        Shader shader;

        int VertexBufferObject;
        int VertexArrayObject;

        public void Draw(Matrix4[] skeleton)
        {
            int meshType = BitConverter.ToInt32(Data, 4);
            if (meshType >= 0 && meshType < 8)
            {
                this.shader.Use();
                GL.BindVertexArray(VertexArrayObject);
                GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveCount);
                GL.UseProgram(0);
                return;
            }

            int pos = -4;

            GL.Begin((PrimitiveType)BitConverter.ToInt32(Data, pos += 4));
            int count = 0;
            Vector4 somme = Vector4.Zero;
            GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

            switch (BitConverter.ToInt32(Data, pos += 4))
            {
                /*case 0:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 1:
                    while (pos + 4 < Data.Length)
                    {
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 2:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 3:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 4:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 5:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 6:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;
                case 7:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.Vertex3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                    }
                    break;*/


                case 8:
                    while (pos + 4 < Data.Length)
                    {
                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 9:
                    while (pos + 4 < Data.Length)
                    {
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {

                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);

                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);

                    }
                    break;
                case 10:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));

                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 11:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 12:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 13:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 14:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 15:
                    while (pos + 4 < Data.Length)
                    {
                        GL.Color4(Data[pos+4], Data[pos+5], Data[pos+6], Data[pos+7]); pos+=4;
                        GL.Normal3(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));
                        GL.TexCoord2(BitConverter.ToSingle(Data, pos += 4), BitConverter.ToSingle(Data, pos += 4));


                        count = BitConverter.ToInt32(Data, pos += 4);
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4),
                                BitConverter.ToSingle(Data, pos += 4)), skeleton[BitConverter.ToInt32(Data, pos += 4)]);
                            count--;
                            if (count == 0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
            }
            GL.End();
        }






    }
}
