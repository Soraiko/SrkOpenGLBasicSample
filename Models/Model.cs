using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Model
    {
        public Skeleton Skeleton;
        public Mesh[] Meshes;

        /*public static Model Load(string filename)
        {
            Model model = null;
            if (Preferences.Debug || !File.Exists(@"binary_files\"+ filename+".mdl"))
            {
                model = new DAE(@"debug_files\" + filename + ".dae");
                model.Compile();
                model.SaveBinary(@"binary_files\"+Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".mdl");
            }
            else if (File.Exists(@"binary_files\" + filename + ".mdl"))
            {
                model = new Model(@"binary_files\" + filename + ".mdl");
            }
            else
            {

            }
            return model;
        }
        public Model()
        {

        }
        public Model(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            int bonesCount = br.ReadInt32();
            int meshesCount = br.ReadInt32();
            br.BaseStream.Position += 0x08;
            if (bonesCount>0)
            {
                this.Skeleton = new Skeleton(bonesCount);
                for (int j=0;j< bonesCount;j++)
                {
                    int parentIndex = br.ReadInt32();
                    br.BaseStream.Position += 0x0C;
                    Matrix4 matrix = Matrix4.Identity;
                    matrix.M11 = br.ReadSingle();
                    matrix.M12 = br.ReadSingle();
                    matrix.M13 = br.ReadSingle();
                    matrix.M14 = br.ReadSingle();

                    matrix.M21 = br.ReadSingle();
                    matrix.M22 = br.ReadSingle();
                    matrix.M23 = br.ReadSingle();
                    matrix.M24 = br.ReadSingle();

                    matrix.M31 = br.ReadSingle();
                    matrix.M32 = br.ReadSingle();
                    matrix.M33 = br.ReadSingle();
                    matrix.M34 = br.ReadSingle();

                    matrix.M41 = br.ReadSingle();
                    matrix.M42 = br.ReadSingle();
                    matrix.M43 = br.ReadSingle();
                    matrix.M44 = br.ReadSingle();

                    Joint joint = new Joint("bone" + j.ToString("d3"), matrix);
                    joint.Parent = parentIndex;
                    this.Skeleton.Joints[j] = joint;
                }
            }
            this.Meshes = new Mesh[meshesCount];
            string dirName = Path.GetDirectoryName(filename);
            for (int i=0;i< meshesCount;i++)
            {
                if (File.Exists(dirName+@"\mesh"+i.ToString("d3")+".bin"))
                {
                    FileStream meshStream = new FileStream(dirName + @"\mesh" + i.ToString("d3") + ".bin",FileMode.Open);
                    BinaryReader meshBinaryReader = new BinaryReader(meshStream);
                    Mesh mesh = new Mesh();
                    //mesh.Texture = Texturing.LoadBinaryTexture(dirName + @"\text" + meshBinaryReader.ReadInt32().ToString("d3") + ".bin",(TextureMinFilter)meshBinaryReader.ReadInt32(),(TextureWrapMode)meshBinaryReader.ReadInt32());
                    meshBinaryReader.BaseStream.Position+=0x10;

                    mesh.Data = meshBinaryReader.ReadBytes((int)(meshBinaryReader.BaseStream.Length- meshBinaryReader.BaseStream.Position));
                    meshBinaryReader.Close();
                }
            }
        }

        public void SaveBinary(string directory, string filename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(directory + @"\" + filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(directory + @"\" + filename));
            }
            FileStream fs = new FileStream(directory+@"\"+filename, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);

            int bonesCount = 0;
            int meshesCount = 0;

            if (this.Skeleton != null)
                bonesCount = this.Skeleton.Joints.Length;
            if (this.Meshes != null)
                meshesCount = this.Meshes.Length;

            bw.Write(bonesCount);
            bw.Write(meshesCount);
            bw.Write(0);
            bw.Write(0);

            for (int i = 0; i < bonesCount; i++)
            {
                bw.Write(this.Skeleton.Joints[i].Parent);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                Matrix4 matrix = this.Skeleton.Joints[i].TransformLocal;
                bw.Write(matrix.M11);
                bw.Write(matrix.M12);
                bw.Write(matrix.M13);
                bw.Write(matrix.M14);
                bw.Write(matrix.M21);
                bw.Write(matrix.M22);
                bw.Write(matrix.M23);
                bw.Write(matrix.M24);
                bw.Write(matrix.M31);
                bw.Write(matrix.M32);
                bw.Write(matrix.M33);
                bw.Write(matrix.M34);
                bw.Write(matrix.M41);
                bw.Write(matrix.M42);
                bw.Write(matrix.M43);
                bw.Write(matrix.M44);
            }
            bw.Close();
            List<int> textures = new List<int>(0);

            for (int i = 0; i < meshesCount; i++)
            {
                fs = new FileStream(directory+@"\mesh"+i.ToString("d3")+".bin", FileMode.OpenOrCreate);
                bw = new BinaryWriter(fs);

                int indexOf = textures.IndexOf(this.Meshes[i].Texture);
                if (indexOf < 0)
                {
                    indexOf = textures.Count;
                    textures.Add(this.Meshes[i].Texture);
                }
                bw.Write(indexOf);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(this.Meshes[i].Data);
                bw.Close();
            }
        }*/

        public void Compile()
        {
            Matrix4[] matrices = new Matrix4[0];
            if (this.Skeleton != null && this.Skeleton.Matrices != null && this.Skeleton.Matrices.Length > 0)
            {
                matrices = new Matrix4[this.Skeleton.Matrices.Length];
                Array.Copy(this.Skeleton.Matrices, matrices, matrices.Length);
            }
            for (int i = 0; i < this.Meshes.Length; i++)
            {
                this.Meshes[i].Compile(ref matrices);
            }
        }
        static int lastTexture = -1;
        public void Draw()
        {
            Matrix4[] mats = new Matrix4[0];
            if (this.Skeleton != null && this.Skeleton.Matrices != null && this.Skeleton.Matrices.Length > 0)
            {
                mats = new Matrix4[this.Skeleton.Matrices.Length];
                Array.Copy(this.Skeleton.Matrices, mats, mats.Length);
            }
            GL.Enable(EnableCap.Texture2D);
            for (int i = 0; i < this.Meshes.Length; i++)
            {
                if (this.Meshes[i].Texture.Integer != lastTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, this.Meshes[i].Texture.Integer);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, this.Meshes[i].Texture.TextureMinFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, this.Meshes[i].Texture.TextureMinFilter);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, this.Meshes[i].Texture.TextureWrapS);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, this.Meshes[i].Texture.TextureWrapT);
                }
                lastTexture = this.Meshes[i].Texture.Integer;

                this.Meshes[i].Draw(mats);
            }
        }
    }
}
