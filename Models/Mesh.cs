using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SrkOpenGLBasicSample
{
    public abstract class Mesh
    {
        public int IndexBufferObject;
        public int VertexBufferObject;
        public int VertexArrayObject;
        public Shader shader;

        public Texture Texture;
        public byte[] Data;

        public List<Vector3> positions;
        public List<Vector2> textureCoordinates;
        public List<Color> colors;
        public List<Vector3> normals;
        public List<int> Indices;
        public int PrimitiveCount;

        public Mesh()
        {
            this.positions = new List<Vector3>();
            this.textureCoordinates = new List<Vector2>();
            this.colors = new List<Color>();
            this.normals = new List<Vector3>();
            this.Indices = new List<int>();
            this.PrimitiveCount = 0;
        }
    }
}
