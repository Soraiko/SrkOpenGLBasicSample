using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SrkOpenGLBasicSample
{
    public class MDLX : Model
    {

        public MDLX(string filename) : base(filename)
        {
            if (this.Reference != null)
                return;
            FileStream fs = new FileStream(filename, FileMode.Open);
            SrkAlternatives.Mdlx mdlx = new SrkAlternatives.Mdlx(fs);

            List<Mesh> meshes = new List<Mesh>(0);
            for (int i=0;i<1;i++)//for (int i = mdlx.models.Count-1; i>=0;i++)
            {
                this.Skeleton = mdlx.models[i].Skeleton;
                
                for (int j=0;j< mdlx.models[i].Meshes.Count;j++)
                {
                    var alternativeMesh = mdlx.models[i].Meshes[j];
                    bool hasController = alternativeMesh.influences.Count > 0 && alternativeMesh.influences[0].Length > 0;
                    Mesh mesh = null;
                    if (hasController)
                        mesh = new DynamicMesh(this);
                    else
                        new StaticMesh(this);

                    if (alternativeMesh.colors.Count>1)
                        mesh.Colors = alternativeMesh.colors;

                    mesh.TextureCoords = alternativeMesh.textureCoordinates;
                    mesh.Indices = alternativeMesh.vertexIndices;
                    if (hasController)
                    {
                        for (int k = 0; k < alternativeMesh.reverseVertices.Count; k++)
                        {
                            Vector4 position = new Vector4(0, 0, 0, 0);

                            for (int l = 0; l < alternativeMesh.reverseVertices[k].Length; l++)
                            {
                                
                                //alternativeMesh.reverseVertices[k][0]
                            }
                            mesh.Positions.Add(position.Xyz);
                        }
                    }
                    else
                    {
                    }
                }
            }
            fs.Close();
        }
    }
}
