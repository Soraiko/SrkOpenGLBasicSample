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

        public void Compile(Skeleton skeleton)
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
            binaryReader = new BinaryReader(memoryStream);

            Vertices.Clear();
            if (Influences != null && Influences.Length > 0) Array.Clear(Influences, 0, Influences.Length);
            if (TextureCoordinates != null && TextureCoordinates.Length > 0) Array.Clear(TextureCoordinates, 0, TextureCoordinates.Length);
            if (Normals != null && Normals.Length > 0) Array.Clear(Normals, 0, Normals.Length);
            if (Colors != null && Colors.Length > 0) Array.Clear(Colors, 0, Colors.Length);
    }


        public void Draw(Skeleton skeleton)
        {
            Matrix4[] mats = new Matrix4[0];
            if (skeleton !=null && skeleton.Matrices != null && skeleton.Matrices.Length>0)
            {
                mats = new Matrix4[skeleton.Matrices.Length];
                Array.Copy(skeleton.Matrices, mats, mats.Length);
            }


            binaryReader.BaseStream.Position = 0;
            GL.Begin((PrimitiveType)binaryReader.ReadInt32());
            int count = 0;
            Vector4 somme = Vector4.Zero;

            switch (binaryReader.ReadInt32())
            {
                case 0:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 1:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 2:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 3:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 4:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 5:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 6:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;
                case 7:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.Vertex3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                    break;


                case 8:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(), 
                                binaryReader.ReadSingle(), 
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
                            count--;
                            if (count==0)
                            {
                                GL.Vertex3(somme.X, somme.Y, somme.Z);
                                break;
                            }
                        }
                        while (true);
                    }
                    break;
                case 9:
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        

                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {

                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);

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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());

                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());


                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());


                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());


                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());


                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        GL.Color4(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
                        GL.Normal3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        GL.TexCoord2(binaryReader.ReadSingle(), binaryReader.ReadSingle());


                        count = binaryReader.ReadInt32();
                        somme = Vector4.Zero;
                        do
                        {
                            somme += Vector4.Transform(new Vector4(
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle(),
                                binaryReader.ReadSingle()), mats[binaryReader.ReadInt32()]);
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
