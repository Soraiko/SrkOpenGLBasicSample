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

                    mesh.Texture = Texture.LoadTexture(
                        this.Name + "::texture" + alternativeMesh.TextureIndex.ToString("d3") + ".png",
                        mdlx.models[i].Textures[alternativeMesh.TextureIndex], 
                        OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, 
                        OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat, 
                        OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat);
                    mesh.BumpMapping = StaticReferences.bumpPixel1x1;


                    List<List<KeyValuePair<ushort, float>>> weight_influences = new List<List<KeyValuePair<ushort, float>>>(0);
                    List<Vector3> positions_unIndexed = new List<Vector3>(0);
                    List<Vector2> textureCoords_unIndexed = new List<Vector2>(0);
                    List<OpenTK.Color> colors_unIndexed = new List<OpenTK.Color>(0);


                    if (hasController)
                    {
                        for (int k = 0; k < alternativeMesh.reverseVertices.Count; k++)
                        {
                            Vector3 v = Vector3.Zero;
                            List<KeyValuePair<ushort, float>> weight_influence = new List<KeyValuePair<ushort, float>>(0);

                            for (int l = 0; l < alternativeMesh.reverseVertices[k].Length; l++)
                            {
                                Vector4 v4 = alternativeMesh.reverseVertices[k][l];

                                weight_influence.Add(new KeyValuePair<ushort, float>(alternativeMesh.influences[k][l], v4.W));

                                if (mdlx.models[i].Skeleton != null)
                                {
                                    v4 = Vector4.Transform(v4, mdlx.models[i].Skeleton.Joints[alternativeMesh.influences[k][l]].ComputedMatrix);
                                }
                                v.X += v4.X;
                                v.Y += v4.Y;
                                v.Z += v4.Z;
                            }
                            weight_influence.Sort((x, y) => (x.Value.CompareTo(y.Value)));
                            weight_influences.Add(weight_influence);

                            positions_unIndexed.Add(v);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < alternativeMesh.reverseVertices.Count; k++)
                        {
                            positions_unIndexed.Add(alternativeMesh.reverseVertices[k][0].Xyz);
                        }
                    }
                    List<ushort> tri = new List<ushort>(0);
                    List<OpenTK.Vector2> texCoords_triBuffer = new List<OpenTK.Vector2>(0);
                    List<OpenTK.Color> colors_triBuffer = new List<OpenTK.Color>(0);

                    bool hasColor = alternativeMesh.colors.Count > 1;

                    List<ushort> indices = new List<ushort>(0);

                    for (int k=0;k<alternativeMesh.triangleFlags.Count;k++)
                    {
                        Color col = hasColor ? alternativeMesh.colors[k] : Color.White;
                        tri.Add(alternativeMesh.vertexIndices[k]);
                        texCoords_triBuffer.Add(alternativeMesh.textureCoordinates[k]);
                        colors_triBuffer.Add(col);

                        if (tri.Count > 3)
                        {
                            tri.RemoveAt(0);
                            texCoords_triBuffer.RemoveAt(0);
                            colors_triBuffer.RemoveAt(0);
                        }

                        if (alternativeMesh.triangleFlags[k] == 0x00 || alternativeMesh.triangleFlags[k] == 0x20)
                        {
                            indices.Add(tri[0]);
                            indices.Add(tri[1]);
                            indices.Add(tri[2]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[0]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[1]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[2]);
                            colors_unIndexed.Add(colors_triBuffer[0]);
                            colors_unIndexed.Add(colors_triBuffer[1]);
                            colors_unIndexed.Add(colors_triBuffer[2]);
                        }
                        if (alternativeMesh.triangleFlags[k] == 0x00 || alternativeMesh.triangleFlags[k] == 0x30)
                        {
                            indices.Add(tri[2]);
                            indices.Add(tri[1]);
                            indices.Add(tri[0]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[2]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[1]);
                            textureCoords_unIndexed.Add(texCoords_triBuffer[0]);
                            colors_unIndexed.Add(colors_triBuffer[2]);
                            colors_unIndexed.Add(colors_triBuffer[1]);
                            colors_unIndexed.Add(colors_triBuffer[0]);
                        }
                    }
                    bool otherThanZeroFound = false;

                    List<float[]> weights = new List<float[]>(0);
                    List<ushort[]> influences = new List<ushort[]>(0);
                    List<Vector3> positions = new List<Vector3>(0);
                    List<Vector2> textureCoords = new List<Vector2>(0);
                    List<Vector3> normals = new List<Vector3>(0);
                    List<Color> colors = new List<Color>(0);

                    for (int l=0;l<indices.Count;l++)
                    {
                        int vertexIndex = indices[l];
                        Vector3 position = positions_unIndexed[vertexIndex];
                        Vector2 textureCoord = textureCoords_unIndexed[l];
                        Vector3 normal = Vector3.UnitZ;
                        Color color = Color.White;
                        List<KeyValuePair<ushort, float>> weight_influence = weight_influences[vertexIndex];

                        int foundIndex = -1;
                        for (int k = 0; k < positions.Count; k++)
                        {
                            if (colors[k].A != color.A) continue;
                            if (colors[k].R != color.R) continue;
                            if (colors[k].G != color.G) continue;
                            if (colors[k].B != color.B) continue;
                            if (Vector3.Distance(positions[k], position) > 0.0001) continue;
                            if (Vector2.Distance(textureCoords[k], textureCoord) > 0.0001) continue;
                            if (Vector3.Distance(normals[k], normal) > 0.0001) continue;
                            if (hasController && influences.Count > 0)
                            {
                                if (influences[k].Length != weight_influence.Count)
                                    continue;
                                bool cont = false;
                                for (int m = 0; m < influences[k].Length; m++)
                                {
                                    if (influences[k][m] != weight_influence[m].Key)
                                    {
                                        cont = true;
                                        break;
                                    }
                                    if (Math.Abs(weights[k][m] - weight_influence[m].Value) > 0.01)
                                    {
                                        cont = true;
                                        break;
                                    }
                                }
                                if (cont)
                                    continue;
                            }
                            foundIndex = k;
                            break;
                        }
                        if (foundIndex < 0)
                        {
                            foundIndex = positions.Count;
                            positions.Add(position);
                            textureCoords.Add(textureCoord);
                            colors.Add(color);
                            normals.Add(normal);
                            if (hasController)
                            {
                                ushort[] influence = new ushort[weight_influence.Count];
                                float[] weight = new float[weight_influence.Count];
                                for (int k = 0; k < influence.Length; k++)
                                {
                                    influence[k] = weight_influence[k].Key;
                                    weight[k] = weight_influence[k].Value;
                                }
                                influences.Add(influence);
                                weights.Add(weight);
                            }
                        }
                        mesh.Indices.Add((ushort)foundIndex);
                    }
                    mesh.Positions.AddRange(positions);
                    if (hasColor)
                        mesh.Colors.AddRange(colors);
                    mesh.TextureCoords.AddRange(textureCoords);

                    if (hasController)
                    {
                        mesh.Influences.AddRange(influences);
                        mesh.Weights.AddRange(weights);
                    }
                    meshes.Add(mesh);
                }
            }
            this.Meshes = meshes.ToArray();
            fs.Close();
        }
    }
}
