using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace SrkOpenGLBasicSample
{
    public abstract class Mesh
    {
        public string Name;
        public int IndexBufferObject;
        public int VertexBufferObject;
        public int VertexArrayObject;
        public Shader shader;

        public Texture Texture;
        public Texture BumpMapping;
        public byte[] Data;

        public List<Vector3> Positions;
        public List<ushort[]> Influences;
        public List<float[]> Weights;
        public List<Vector2> TextureCoords;
        public List<Color> Colors;
        public List<Vector3> Normals;
        public List<ushort> Indices;
        public int PrimitiveCount;

        public Model Model;
        public Mesh(Model model)
        {
            this.Model = model;
            this.Positions = new List<Vector3>();
            this.Influences = new List<ushort[]>();
            this.Weights = new List<float[]>();
            this.TextureCoords = new List<Vector2>();
            this.Colors = new List<Color>();
            this.Normals = new List<Vector3>();
            this.Indices = new List<ushort>();
            this.PrimitiveCount = 0;
        }

        public const int POSITIONS_SIZE = 3 * sizeof(float);
        public const int TEXTURECOORDINATES_SIZE = 2 * sizeof(short);
        public const int COLORS_SIZE = 4 * sizeof(byte);
        public const int NORMALS_SIZE = 3 * sizeof(float);

        public void Compile(Skeleton skeleton)
        {
            bool skinned = skeleton != null && skeleton.Joints.Count>0;
            int maxInfCount = 0;
            if (skinned)
            {
                for (int i = 0; i < this.Influences.Count; i++)
                {
                    while (this.Influences[i].Length > 7)
                    {
                        ushort smallest_influence = this.Influences[i][0];
                        double smallest_weight = this.Weights[i][0];

                        ushort[] influences_array = new ushort[this.Influences[i].Length - 1];
                        float[] weights_array = new float[this.Weights[i].Length - 1];

                        Array.Copy(this.Influences[i], 1, influences_array, 0, influences_array.Length);
                        Array.Copy(this.Weights[i], 1, weights_array, 0, weights_array.Length);
                        smallest_weight = smallest_weight / (double)weights_array.Length;
                        for (int j = 0; j < weights_array.Length; j++)
                            weights_array[j] = (float)(weights_array[j] + smallest_weight);
                        this.Influences[i] = influences_array;
                        this.Weights[i] = weights_array;
                    }
                    if (this.Influences[i].Length > maxInfCount)
                        maxInfCount = this.Influences[i].Length;
                }
            }

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
            int infsCountOffset = -1;

            switch (mode)
            {
                case 1: //0001
                    this.shader = skinned ? Shader.skP : Shader.P;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);

                        if (!skinned) continue;

                        int reserved = this.Influences[i].Length;
                        binaryWriter.Write((byte)reserved);
                        int free = maxInfCount - reserved;

                        for (int j = 0; j < reserved; j++)
                        {
                            binaryWriter.Write(this.Influences[i][j]);
                            binaryWriter.Write((ushort)Math.Min(ushort.MaxValue, Math.Max(0, (long)(this.Weights[i][j] * ushort.MaxValue))));
                        }
                        for (int j = 0; j < free; j++)
                        {
                            binaryWriter.Write((ushort)0);
                            binaryWriter.Write((ushort)0);
                        }
                    }
                    positionsOffset = 0;
                    infsCountOffset = POSITIONS_SIZE;
                    VertexStride = POSITIONS_SIZE;
                    if (skinned)
                        VertexStride += 1 + 4 * maxInfCount;
                    break;
                case 3: //0011
                    this.shader = skinned ? Shader.skTP: Shader.TP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write((short)(this.TextureCoords[i].X * 4096));
                        binaryWriter.Write((short)(this.TextureCoords[i].Y * 4096));

                        if (!skinned) continue;

                        int reserved = this.Influences[i].Length;
                        binaryWriter.Write((byte)reserved);
                        int free = maxInfCount - reserved;

                        for (int j = 0; j < reserved; j++)
                        {
                            binaryWriter.Write(this.Influences[i][j]);
                            binaryWriter.Write((ushort)Math.Min(ushort.MaxValue, Math.Max(0, (long)(this.Weights[i][j] * ushort.MaxValue))));
                        }
                        for (int j = 0; j < free; j++)
                        {
                            binaryWriter.Write((ushort)0);
                            binaryWriter.Write((ushort)0);
                        }
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    infsCountOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    if (skinned)
                        VertexStride += 1 + 4 * maxInfCount;
                    break;
                case 5: //0101
                    break;
                case 7: //0111
                    this.shader = skinned ? Shader.skNTP : Shader.NTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write((short)(this.TextureCoords[i].X * 4096));
                        binaryWriter.Write((short)(this.TextureCoords[i].Y * 4096));
                        binaryWriter.Write(this.Normals[i].X);
                        binaryWriter.Write(this.Normals[i].Y);
                        binaryWriter.Write(this.Normals[i].Z);

                        if (!skinned) continue;

                        int reserved = this.Influences[i].Length;
                        binaryWriter.Write((byte)reserved);
                        int free = maxInfCount - reserved;

                        for (int j = 0; j < reserved; j++)
                        {
                            binaryWriter.Write(this.Influences[i][j]);
                            binaryWriter.Write((ushort)Math.Min(ushort.MaxValue, Math.Max(0, (long)(this.Weights[i][j] * ushort.MaxValue))));
                        }
                        for (int j = 0; j < free; j++)
                        {
                            binaryWriter.Write((ushort)0);
                            binaryWriter.Write((ushort)0);
                        }
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    normalsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    infsCountOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    if (skinned)
                        VertexStride += 1 + 4 * maxInfCount;
                    break;
                case 11: //1011
                    this.shader = skinned ? Shader.skCTP : Shader.CTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write((short)(this.TextureCoords[i].X * 4096));
                        binaryWriter.Write((short)(this.TextureCoords[i].Y * 4096));
                        binaryWriter.Write(this.Colors[i].R);
                        binaryWriter.Write(this.Colors[i].G);
                        binaryWriter.Write(this.Colors[i].B);
                        binaryWriter.Write(this.Colors[i].A);

                        if (!skinned) continue;

                        int reserved = this.Influences[i].Length;
                        binaryWriter.Write((byte)reserved);
                        int free = maxInfCount - reserved;

                        for (int j = 0; j < reserved; j++)
                        {
                            binaryWriter.Write(this.Influences[i][j]);
                            binaryWriter.Write((ushort)Math.Min(ushort.MaxValue, Math.Max(0, (long)(this.Weights[i][j] * ushort.MaxValue))));
                        }
                        for (int j = 0; j < free; j++)
                        {
                            binaryWriter.Write((ushort)0);
                            binaryWriter.Write((ushort)0);
                        }
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    colorsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    infsCountOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + COLORS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + COLORS_SIZE;
                    if (skinned)
                        VertexStride += 1 + 4 * maxInfCount;
                    break;
                case 13: //1101
                    break;
                case 15: //1111
                    this.shader = skinned ? Shader.skCNTP : Shader.CNTP;
                    for (int i = 0; i < this.Positions.Count; i++)
                    {
                        binaryWriter.Write(this.Positions[i].X);
                        binaryWriter.Write(this.Positions[i].Y);
                        binaryWriter.Write(this.Positions[i].Z);
                        binaryWriter.Write((short)(this.TextureCoords[i].X * 4096));
                        binaryWriter.Write((short)(this.TextureCoords[i].Y * 4096));
                        binaryWriter.Write(this.Normals[i].X);
                        binaryWriter.Write(this.Normals[i].Y);
                        binaryWriter.Write(this.Normals[i].Z);
                        binaryWriter.Write(this.Colors[i].R);
                        binaryWriter.Write(this.Colors[i].G);
                        binaryWriter.Write(this.Colors[i].B);
                        binaryWriter.Write(this.Colors[i].A);

                        if (!skinned) continue;

                        int reserved = this.Influences[i].Length;
                        binaryWriter.Write((byte)reserved);
                        int free = maxInfCount - reserved;

                        for (int j = 0; j < reserved; j++)
                        {
                            binaryWriter.Write(this.Influences[i][j]);
                            binaryWriter.Write((ushort)Math.Min(ushort.MaxValue, Math.Max(0, (long)(this.Weights[i][j] * ushort.MaxValue))));
                        }
                        for (int j = 0; j < free; j++)
                        {
                            binaryWriter.Write((ushort)0);
                            binaryWriter.Write((ushort)0);
                        }
                    }
                    positionsOffset = 0;
                    textureCoordinatesOffset = POSITIONS_SIZE;
                    normalsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE;
                    colorsOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE;
                    infsCountOffset = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE + COLORS_SIZE;
                    VertexStride = POSITIONS_SIZE + TEXTURECOORDINATES_SIZE + NORMALS_SIZE + COLORS_SIZE;
                    if (skinned)
                        VertexStride += 1 + 4 * maxInfCount;
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

            if (this.Indices.Count > 0)
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
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Short, false, VertexStride, textureCoordinatesOffset);
                GL.EnableVertexAttribArray(1);
            }

            if (normalsOffset > -1)
            {
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, VertexStride, normalsOffset);
                GL.EnableVertexAttribArray(2);
            }

            if (colorsOffset > -1)
            {
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.UnsignedByte, true, VertexStride, colorsOffset);
                GL.EnableVertexAttribArray(3);
            }

            if (skinned)
            {
                GL.VertexAttribIPointer(8, 1, VertexAttribIntegerType.UnsignedByte, VertexStride, (IntPtr)(infsCountOffset));
                GL.EnableVertexAttribArray(8);

                for (int i = 0; i < maxInfCount; i++)
                {
                    GL.VertexAttribIPointer(9 + i, 1, VertexAttribIntegerType.UnsignedInt, VertexStride, (IntPtr)(infsCountOffset + 1 + i * 4));
                    GL.EnableVertexAttribArray(9 + i);
                }
            }

            if (this.PrimitiveCount == 0)
                this.PrimitiveCount = this.Data.Length / VertexStride;

            if (skinned && this.Model.UniformBufferObject < 0)
            {
                this.Model.UniformBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, this.Model.UniformBufferObject);
                GL.BufferData(BufferTarget.UniformBuffer, skeleton.MatricesBuffer.Length * sizeof(float), skeleton.MatricesBuffer, BufferUsageHint.DynamicCopy);
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Update(Skeleton skeleton)
        {
            if (this is DynamicMesh)
                (this as DynamicMesh).Update(skeleton.MatricesBuffer, skeleton.Joints.Count);
        }
        public void Draw(bool noReference)
        {
            if (this is StaticMesh)
                (this as StaticMesh).Draw(noReference);
            else
            if (this is DynamicMesh)
                (this as DynamicMesh).Draw(noReference);
        }
    }
}
