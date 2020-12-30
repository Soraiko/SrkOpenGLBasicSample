using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

        public Mesh()
        {
            this.Positions = new List<Vector3>();
            this.Influences = new List<ushort[]>();
            this.Weights = new List<float[]>();
            this.TextureCoords = new List<Vector2>();
            this.Colors = new List<Color>();
            this.Normals = new List<Vector3>();
            this.Indices = new List<ushort>();
            this.PrimitiveCount = 0;
        }

        public void Compile(Skeleton skeleton)
        {
            if (this is StaticMesh)
                (this as StaticMesh).Compile();
            if (this is DynamicMesh)
                (this as DynamicMesh).Compile(skeleton.MatricesBuffer);
        }

        public void Update(Skeleton skeleton)
        {

            if (this is DynamicMesh)
                (this as DynamicMesh).Update(skeleton.MatricesBuffer, skeleton.Joints.Count);
        }
        public void Draw()
        {
            if (this is StaticMesh)
                (this as StaticMesh).Draw();
            if (this is DynamicMesh)
                (this as DynamicMesh).Draw();
        }
    }
}
