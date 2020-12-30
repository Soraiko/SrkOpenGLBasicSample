﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using OpenTK;
using OpenTK.Graphics;

namespace SrkOpenGLBasicSample
{
    public class DAE : Model
    {
        public static System.Globalization.CultureInfo en = new System.Globalization.CultureInfo("en-US");

        public static Single GetSingle(double d)
        {
            double rounded = Math.Round(d, 7);
            float singleRound = (float)rounded;
            float singleNoRound = (float)d;

            if (Math.Abs(rounded - d) < Math.Abs((double)singleNoRound - d))
                return singleRound;
            return singleNoRound;
        }

        public XmlNodeList images;
        public XmlNodeList materials;
        public XmlNodeList effects;
        public XmlNodeList geometries;
        public XmlNodeList controllers;
        public XmlNodeList visual_scenes;
        public XmlNodeList[] surfaces;
        public XmlNodeList[] joints;

        public static DAE SampleDAE;
        public static bool SampleLoaded;

        XmlDocument Document;

        static XmlNode sampleImage;
        static XmlNode sampleMaterial;
        static XmlNode sampleEffect;
        static XmlNode sampleGeomerty;
        static XmlNode sampleController;
        static XmlNode sampleScene;
        static XmlNode sampleSurface;
        static XmlNode sampleJoint;



        public DAE(string filename)
        {
            this.Name = Path.GetFileNameWithoutExtension(filename);
            this.Directory = Path.GetDirectoryName(filename);

            byte[] fileData = File.ReadAllBytes(filename);

            for (int i = 0; i < 500; i++)
            {
                if (fileData[i + 0] != 0x78) continue;
                if (fileData[i + 1] != 0x6D) continue;
                if (fileData[i + 2] != 0x6C) continue;
                if (fileData[i + 3] != 0x6E) continue;
                if (fileData[i + 4] != 0x73) continue;
                fileData[i + 0] = 0x77;
            }

            MemoryStream memoryStream = new MemoryStream(fileData);

            this.Document = new XmlDocument();
            this.Document.Load(memoryStream);
            
            this.images = this.Document.SelectNodes("descendant::library_images/image");
            this.materials = this.Document.SelectNodes("descendant::library_materials/material");
            this.effects = this.Document.SelectNodes("descendant::library_effects/effect");
            this.geometries = this.Document.SelectNodes("descendant::library_geometries/geometry");
            this.controllers = this.Document.SelectNodes("descendant::library_controllers/controller");
            this.visual_scenes = this.Document.SelectNodes("descendant::library_visual_scenes/visual_scene");
            this.surfaces = new XmlNodeList[this.visual_scenes.Count];
            this.joints = new XmlNodeList[this.visual_scenes.Count];

            for (int i = 0; i < this.visual_scenes.Count; i++)
            {
                this.joints[i] = this.visual_scenes[i].SelectNodes("descendant::node[translate(@type, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='joint']");
                this.surfaces[i] = this.visual_scenes[i].SelectNodes("node[not(@type)]");
            }

            this.ImagesIDs = new List<string>(0);
            this.ImagesFilenames = new List<string>(0);

            this.PerGeometryMaterials = new List<string>(0);
            this.MaterialsIDs = new List<string>(0);
            this.MaterialsEffectIDs = new List<string>(0);
            this.EffectsIDs = new List<string>(0);
            this.EffectsImageIDs = new List<string>(0);
            this.GeometryIDs = new List<string>(0);

            this.GeometryDataVertex = new List<Vector3[]>(0);
            this.GeometryDataTexcoordinates = new List<Vector2[]>(0);
            this.GeometryDataNormals = new List<Vector3[]>(0);
            this.GeometryDataColors = new List<Color[]>(0);

            this.GeometryDataVertex_i = new List<List<int>>(0);
            this.GeometryDataTexcoordinates_i = new List<List<int>>(0);
            this.GeometryDataNormals_i = new List<List<int>>(0);
            this.GeometryDataColors_i = new List<List<int>>(0);

            this.ControllerDataJoints_i = new List<List<List<int>>>(0);
            this.ControllerDataMatrices_i = new List<List<List<int>>>(0);
            this.ControllerDataWeights_i = new List<List<List<int>>>(0);

            this.ControllersIDs = new List<string>(0);
            this.PerControllerGeometry = new List<string>(0);

            this.ShapeMatrices = new List<Matrix4>(0);
            this.ControllerDataJoints = new List<string[]>(0);
            this.ControllerDataMatrices = new List<Matrix4[]>(0);
            this.ControllerDataWeights = new List<float[]>(0);
            this.VisualScenesIDs = new List<string>(0);

            this.Skeleton = new Skeleton();
            this.JointsIDs = new List<List<string>>(0);
            this.JointsMatrices = new List<List<Matrix4>>(0);
            this.SurfacesIDs = new List<List<string>>(0);
            this.SurfacesMaterialsID = new List<List<string>>(0);
        }
        
        public void Export(string fname)
        {
            if (!SampleLoaded)
            {
                SampleLoaded = true;
                if (!File.Exists("sample.dae"))
                    throw new Exception("The sample file does not exist. Make sure that the file sample.dae is present near the executable.");
                SampleDAE = new DAE("sample.dae");

                sampleImage = SampleDAE.images[0];
                sampleImage.ParentNode.RemoveChild(sampleImage);

                sampleMaterial = SampleDAE.materials[0];
                sampleMaterial.ParentNode.RemoveChild(sampleMaterial);

                sampleEffect = SampleDAE.effects[0];
                sampleEffect.ParentNode.RemoveChild(sampleEffect);

                sampleGeomerty = SampleDAE.geometries[0];
                sampleGeomerty.ParentNode.RemoveChild(sampleGeomerty);

                sampleController = SampleDAE.controllers[0];
                sampleController.ParentNode.RemoveChild(sampleController);

                sampleScene = SampleDAE.visual_scenes[0];
                sampleScene.ParentNode.RemoveChild(sampleScene);

                sampleJoint = sampleScene.SelectNodes("descendant::node[translate(@type, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='joint']")[0];
                sampleJoint.ParentNode.RemoveChild(sampleJoint);

                sampleSurface = sampleScene.SelectNodes("descendant::node")[0];
                sampleSurface.ParentNode.RemoveChild(sampleSurface);
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = DAE.en;
            System.Threading.Thread.CurrentThread.CurrentCulture = DAE.en;
            string dir = Path.GetDirectoryName(fname);


            XmlDocument doc = new XmlDocument();

            string sampleDocText = File.ReadAllText("sample.dae");
            doc.PreserveWhitespace = false;
            sampleDocText = sampleDocText.Replace("xmlns=", "whocares=");

            doc.LoadXml(sampleDocText);

            var libraryImages = doc.SelectNodes("descendant::library_images")[0];
            var libraryMaterials = doc.SelectNodes("descendant::library_materials")[0];
            var libraryEffects = doc.SelectNodes("descendant::library_effects")[0];

            var imageSample = libraryImages.SelectNodes("image")[0];
            var materialSample = libraryMaterials.SelectNodes("material")[0];
            var effectSample = libraryEffects.SelectNodes("effect")[0];

            libraryImages.RemoveChild(imageSample);
            libraryMaterials.RemoveChild(materialSample);
            libraryEffects.RemoveChild(effectSample);

            XmlNode contenuARemplacer;

            for (int i = 0; i < this.ImagesIDs.Count; i++)
            {
                var newImage = imageSample.CloneNode(true);
                contenuARemplacer = newImage.SelectNodes("descendant::init_from")[0];
                contenuARemplacer.InnerText = ImagesFilenames[i];
                newImage.Attributes["id"].Value = ImagesIDs[i];
                string name = ImagesIDs[i];
                if (name.Contains("-image"))
                    name = name.Remove(name.IndexOf("-image"), 6);
                newImage.Attributes["name"].Value = name;
                libraryImages.AppendChild(newImage);
            }

            for (int i = 0; i < this.MaterialsIDs.Count; i++)
            {
                var newMaterial = materialSample.CloneNode(true);
                contenuARemplacer = newMaterial.SelectNodes("descendant::instance_effect")[0];
                contenuARemplacer.Attributes["url"].Value = "#"+MaterialsEffectIDs[i];
                newMaterial.Attributes["id"].Value = MaterialsIDs[i];
                newMaterial.Attributes["name"].Value = MaterialsIDs[i];
                libraryMaterials.AppendChild(newMaterial);
            }
            for (int i = 0; i < this.EffectsIDs.Count; i++)
            {
                var newEffect = effectSample.CloneNode(true);
                newEffect.Attributes["id"].Value = EffectsIDs[i];
                string name = EffectsIDs[i];
                if (name.Contains("-fx"))
                    name = name.Remove(name.IndexOf("-fx"), 3);
                newEffect.Attributes["name"].Value = name;

                contenuARemplacer = newEffect.SelectNodes("descendant::texture")[0];
                contenuARemplacer.Attributes["texture"].Value = EffectsImageIDs[i]; /* kokodayo*/
                libraryEffects.AppendChild(newEffect);
            }

            var libraryGeometries = doc.SelectNodes("descendant::library_geometries")[0];
            var libraryControllers = doc.SelectNodes("descendant::library_controllers")[0];
            var visualScene = doc.SelectNodes("descendant::library_visual_scenes/visual_scene")[0];

            var geometrySample = libraryGeometries.SelectNodes("geometry")[0];
            var controllerSample = libraryControllers.SelectNodes("controller")[0];
            var sceneNodeSample = visualScene.SelectNodes("node[@name='polySurface☺']")[0];

            libraryGeometries.RemoveChild(geometrySample);
            libraryControllers.RemoveChild(controllerSample);
            visualScene.RemoveChild(sceneNodeSample);
            XmlNodeList recherche;

            for (int i = 0; i < this.GeometryIDs.Count; i++)
            {
                var newGeometry = geometrySample.CloneNode(true);

                recherche = newGeometry.SelectNodes("//@*[contains(., '☺')]");

                for (int mR = 0; mR < recherche.Count; mR++)
                    recherche[mR].Value = this.GeometryIDs[i].Split('-')[0] + recherche[mR].Value.Split('☺')[1];

                recherche = newGeometry.SelectNodes("//@material");
                recherche[0].Value = PerGeometryMaterials[i];

                Vector3[] vertices_List = GeometryDataVertex[i];


                recherche = newGeometry.SelectNodes("descendant::accessor[contains(@source,'POSITION-array')]/@count");
                for (int p = 0; p < recherche.Count; p++)
                    recherche[p].Value = vertices_List.Length.ToString();


                recherche = newGeometry.SelectNodes("descendant::float_array[text() = 'listeDeVertices']");
                recherche[0].Attributes["count"].Value = (vertices_List.Length * 3).ToString();
                recherche[0].InnerText = "";

                for (int j = 0; j < vertices_List.Length; j++)
                {
                    recherche[0].InnerText += vertices_List[j].X.ToString("0.000000") + " " +
                        vertices_List[j].Y.ToString("0.000000") + " " +
                        vertices_List[j].Z.ToString("0.000000") + "\r\n";
                }



                Vector2[] uvs_List = GeometryDataTexcoordinates[i];

                if (uvs_List.Length == 0)
                {
                    recherche = newGeometry.SelectNodes("descendant::triangles");
                    XmlNodeList uvs = newGeometry.SelectNodes("descendant::input[@semantic='TEXCOORD']");
                    recherche[0].RemoveChild(uvs[0]);

                    recherche = newGeometry.SelectNodes("descendant::mesh");
                    uvs = newGeometry.SelectNodes("descendant::source[contains(@id,'UV')]");
                    recherche[0].RemoveChild(uvs[0]);
                    uvs = null;

                    XmlNodeList colors = newGeometry.SelectNodes("descendant::input[@semantic='COLOR']");
                    colors[0].Attributes["offset"].Value = "1";
                    colors = null;
                }
                else
                {
                    recherche = newGeometry.SelectNodes("descendant::accessor[contains(@source,'UV0-array')]/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = uvs_List.Length.ToString();

                    recherche = newGeometry.SelectNodes("descendant::float_array[text() = 'listeDeUvs']");
                    recherche[0].Attributes["count"].Value = (uvs_List.Length * 2).ToString();
                    recherche[0].InnerText = "";
                    for (int j = 0; j < uvs_List.Length; j++)
                        recherche[0].InnerText +=
                            uvs_List[j].X.ToString("0.000000") + " " +
                            (uvs_List[j].Y).ToString("0.000000") + "\r\n";
                }


                Vector3[] normals_List = this.GeometryDataNormals[i];

                if (normals_List.Length == 0)
                {
                    recherche = newGeometry.SelectNodes("descendant::vertices");
                    XmlNodeList normals = newGeometry.SelectNodes("descendant::input[@semantic='NORMAL']");
                    recherche[0].RemoveChild(normals[0]);

                    recherche = newGeometry.SelectNodes("descendant::mesh");
                    normals = newGeometry.SelectNodes("descendant::source[contains(@id,'Normal')]");
                    recherche[0].RemoveChild(normals[0]);
                    normals = null;
                }
                else
                {
                    recherche = newGeometry.SelectNodes("descendant::accessor[contains(@source,'Normal0-array')]/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = normals_List.Length.ToString();

                    recherche = newGeometry.SelectNodes("descendant::float_array[text() = 'listeDeNormals']");
                    recherche[0].Attributes[1].Value = (normals_List.Length * 3).ToString();
                    recherche[0].InnerText = "";
                    for (int j = 0; j < normals_List.Length; j++)
                        recherche[0].InnerText += normals_List[j].X.ToString("0.000000") + " " +
                            normals_List[j].Y.ToString("0.000000") + " " +
                            normals_List[j].Z.ToString("0.000000") + "\r\n";
                }
                Color[] colors_List = this.GeometryDataColors[i];

                if (colors_List.Length == 0)
                {
                    recherche = newGeometry.SelectNodes("descendant::triangles");
                    XmlNodeList colors = newGeometry.SelectNodes("descendant::input[@semantic='COLOR']");
                    recherche[0].RemoveChild(colors[0]);

                    recherche = newGeometry.SelectNodes("descendant::mesh");
                    colors = newGeometry.SelectNodes("descendant::source[contains(@id,'COLOR')]");
                    recherche[0].RemoveChild(colors[0]);
                    colors = null;
                }
                else
                {
                    recherche = newGeometry.SelectNodes("descendant::accessor[contains(@source,'COLOR0-array')]/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = colors_List.Length.ToString();

                    recherche = newGeometry.SelectNodes("descendant::float_array[text() = 'listeDeColors']");
                    recherche[0].Attributes[1].Value = (colors_List.Length * 3).ToString();
                    recherche[0].InnerText = "";
                    for (int j = 0; j < colors_List.Length; j++)
                        recherche[0].InnerText +=
                        (colors_List[j].R / 255f).ToString("0.000000") + " " +
                        (colors_List[j].G / 255f).ToString("0.000000") + " " +
                        (colors_List[j].B / 255f).ToString("0.000000") + " " +
                        (colors_List[j].A / 255f).ToString("0.000000") + "\r\n";
                }




                recherche = newGeometry.SelectNodes("descendant::triangles");
                recherche[0].Attributes[0].Value = (GeometryDataVertex_i[i].Count / 3).ToString();
                var paragraphe = doc.CreateElement("p");

                if (uvs_List.Length == 0 && colors_List.Length == 0)
                {
                    for (int t = 0; t < GeometryDataVertex_i[i].Count; t++)
                    {
                        paragraphe.InnerText +=
                            GeometryDataVertex_i[i][t] + " ";
                    }
                }
                else
                if (uvs_List.Length == 0 && colors_List.Length > 0)
                {
                    for (int t = 0; t < GeometryDataVertex_i[i].Count; t++)
                    {
                        paragraphe.InnerText +=
                            GeometryDataVertex_i[i][t] + " " +
                            GeometryDataColors_i[i][t] + " ";
                    }
                }
                else
                if (uvs_List.Length > 0 && colors_List.Length == 0)
                {
                    for (int t = 0; t < GeometryDataVertex_i[i].Count; t++)
                    {
                        paragraphe.InnerText +=
                            GeometryDataVertex_i[i][t] + " " +
                            GeometryDataTexcoordinates_i[i][t] + " ";
                    }
                }
                else
                    for (int t = 0; t < GeometryDataVertex_i[i].Count; t++)
                    {
                        paragraphe.InnerText +=
                            GeometryDataVertex_i[i][t] + " " +
                            GeometryDataTexcoordinates_i[i][t] + " " +
                            GeometryDataColors_i[i][t] + " ";
                    }
                recherche[0].AppendChild(paragraphe);



                libraryGeometries.AppendChild(newGeometry);

                var newController = controllerSample.CloneNode(true);
                recherche = newController.SelectNodes("//@*[contains(., '@')]");

                for (int mR = 0; mR < recherche.Count; mR++)
                    recherche[mR].Value = this.GeometryIDs[i].Split('-')[0] + recherche[mR].Value.Split('@')[1];

                List<Joint> joints = new List<Joint>(0);
                List<Matrix4> matrices = new List<Matrix4>(0);
                List<float> infs = new List<float>(0);
                List<int> vcount = new List<int>(0);
                List<int> v = new List<int>(0);

                
                int controllerIndex = this.PerControllerGeometry.IndexOf(this.GeometryIDs[i]);

                if (controllerIndex >-1)
                {
                    for (int j = 0; j < this.ControllerDataJoints[controllerIndex].Length; j++)
                    {
                        joints.Add(this.Skeleton.GetJoint(this.ControllerDataJoints[controllerIndex][j]));
                        matrices.Add(this.ControllerDataMatrices[controllerIndex][j]);
                    }

                    for (int j = 0; j < this.ControllerDataWeights[controllerIndex].Length; j++)
                        infs.Add(this.ControllerDataWeights[controllerIndex][j]);

                    for (int j = 0; j < this.ControllerDataJoints_i[controllerIndex].Count; j++)
                    {
                        vcount.Add(this.ControllerDataJoints_i[controllerIndex][j].Count);
                        for (int k = 0; k < this.ControllerDataJoints_i[controllerIndex][j].Count; k++)
                        {
                            v.Add(this.ControllerDataJoints_i[controllerIndex][j][k]);
                            v.Add(this.ControllerDataWeights_i[controllerIndex][j][k]);
                        }
                    }

                    recherche = newController.SelectNodes("descendant::Name_array[text() = 'listeDeJoints']");
                    recherche[0].Attributes[1].Value = matrices.Count.ToString();
                    recherche[0].InnerText = "";
                    for (int j = 0; j < matrices.Count; j++)
                    {
                        recherche[0].InnerText += joints[j].Name + " ";
                    }
                    recherche[0].InnerText = recherche[0].InnerText.Remove(recherche[0].InnerText.Length - 1);


                    recherche = newController.SelectNodes("descendant::accessor[@stride='16']/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = matrices.Count.ToString();

                    recherche = newController.SelectNodes("descendant::accessor[contains(@source,'oints')]/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = matrices.Count.ToString();



                    recherche = newController.SelectNodes("descendant::float_array[text() = 'listeDeMatrices']");
                    recherche[0].Attributes[1].Value = (matrices.Count * 16).ToString();
                    recherche[0].InnerText = "";
                    for (int j = 0; j < matrices.Count; j++)
                    {
                        Matrix4 mat = matrices[j];

                        recherche[0].InnerText += mat.M11.ToString() + " ";
                        recherche[0].InnerText += mat.M21.ToString() + " ";
                        recherche[0].InnerText += mat.M31.ToString() + " ";
                        recherche[0].InnerText += mat.M41.ToString() + " ";

                        recherche[0].InnerText += mat.M12.ToString() + " ";
                        recherche[0].InnerText += mat.M22.ToString() + " ";
                        recherche[0].InnerText += mat.M32.ToString() + " ";
                        recherche[0].InnerText += mat.M42.ToString() + " ";

                        recherche[0].InnerText += mat.M13.ToString() + " ";
                        recherche[0].InnerText += mat.M23.ToString() + " ";
                        recherche[0].InnerText += mat.M33.ToString() + " ";
                        recherche[0].InnerText += mat.M43.ToString() + " ";

                        recherche[0].InnerText += mat.M14.ToString() + " ";
                        recherche[0].InnerText += mat.M24.ToString() + " ";
                        recherche[0].InnerText += mat.M34.ToString() + " ";
                        recherche[0].InnerText += mat.M44.ToString() + " ";
                    }
                    recherche[0].InnerText = recherche[0].InnerText.Remove(recherche[0].InnerText.Length - 1);

                    recherche = newController.SelectNodes("descendant::float_array[text() = 'listeDeWeigths']");
                    recherche[0].Attributes[1].Value = infs.Count.ToString();
                    recherche[0].InnerText = "";

                    for (int j = 0; j < infs.Count; j++)
                    {
                        recherche[0].InnerText += infs[j].ToString() + " ";
                    }
                    recherche[0].InnerText = recherche[0].InnerText.Remove(recherche[0].InnerText.Length - 1);


                    recherche = newController.SelectNodes("descendant::accessor[contains(@source,'eights')]/@count");
                    for (int p = 0; p < recherche.Count; p++)
                        recherche[p].Value = infs.Count.ToString();


                    recherche = newController.SelectNodes("descendant::vertex_weights");
                    recherche[0].Attributes[0].Value = vcount.Count.ToString();
                    recherche = newController.SelectNodes("descendant::vertex_weights/vcount");

                    for (int j = 0; j < vcount.Count; j++)
                    {
                        recherche[0].InnerText += vcount[j] + " ";
                    }
                    recherche[0].InnerText = recherche[0].InnerText.Remove(recherche[0].InnerText.Length - 1);

                    recherche = newController.SelectNodes("descendant::vertex_weights/v");

                    for (int j = 0; j < v.Count; j++)
                        recherche[0].InnerText += v[j] + " ";
                    recherche[0].InnerText = recherche[0].InnerText.Remove(recherche[0].InnerText.Length - 1);

                    libraryControllers.AppendChild(newController);
                }

                var newSceneNode = sceneNodeSample.CloneNode(true);

                recherche = newSceneNode.SelectNodes("//@*[contains(., '☺')]");
                string suffixe = controllerIndex > -1 ? "Controller" : "-lib";
                
                for (int mR = 0; mR < recherche.Count; mR++)
                {
                    recherche[mR].Value = this.GeometryIDs[i].Split('-')[0] + suffixe;
                }
                
                if (controllerIndex<0)
                {
                    newSceneNode.InnerXml = newSceneNode.InnerXml.Replace("instance_controller","instance_geometry");
                }

                recherche = newSceneNode.SelectNodes("descendant::instance_material");
                for (int mR = 0; mR < recherche.Count; mR++)
                {
                    recherche[mR].Attributes["symbol"].Value = PerGeometryMaterials[i];
                    recherche[mR].Attributes["target"].Value = "#"+PerGeometryMaterials[i];
                }

                visualScene.AppendChild(newSceneNode);
                matrices = null;
                infs = null;
                vcount = null;
                v = null;
            }

            var jointSample = visualScene.SelectNodes("node[@name = 'joint000']")[0];
            visualScene.RemoveChild(jointSample);

            for (int i = 0; i < this.Skeleton.Joints.Count; i++)
            {
                var newJoint = jointSample.CloneNode(true);
                newJoint.Attributes[0].Value = this.Skeleton.Joints[i].Name;
                newJoint.Attributes[1].Value = this.Skeleton.Joints[i].Name;// + "_" + this.Skeleton.Bones[i].Flag1+"_"+ this.Skeleton.Bones[i].Flag2;
                newJoint.Attributes[2].Value = this.Skeleton.Joints[i].Name;

                recherche = newJoint.SelectNodes("matrix");
                recherche[0].InnerText = "";
                Matrix4 mat = this.Skeleton.Joints[i].Matrix;

                recherche[0].InnerText += mat.M11.ToString() + " ";
                recherche[0].InnerText += mat.M21.ToString() + " ";
                recherche[0].InnerText += mat.M31.ToString() + " ";
                recherche[0].InnerText += mat.M41.ToString() + " ";

                recherche[0].InnerText += mat.M12.ToString() + " ";
                recherche[0].InnerText += mat.M22.ToString() + " ";
                recherche[0].InnerText += mat.M32.ToString() + " ";
                recherche[0].InnerText += mat.M42.ToString() + " ";

                recherche[0].InnerText += mat.M13.ToString() + " ";
                recherche[0].InnerText += mat.M23.ToString() + " ";
                recherche[0].InnerText += mat.M33.ToString() + " ";
                recherche[0].InnerText += mat.M43.ToString() + " ";

                recherche[0].InnerText += mat.M14.ToString() + " ";
                recherche[0].InnerText += mat.M24.ToString() + " ";
                recherche[0].InnerText += mat.M34.ToString() + " ";
                recherche[0].InnerText += mat.M44.ToString() + " ";


                Joint parent = this.Skeleton.Joints[i].Parent;
                if (parent == null)
                {
                    visualScene.AppendChild(newJoint);
                }
                else
                {
                    recherche = visualScene.SelectNodes("descendant::node[@name='" + parent.Name + "']");
                    if (recherche.Count > 0)
                    {
                        recherche[0].AppendChild(newJoint);
                    }
                    else
                    {
                        visualScene.AppendChild(newJoint);
                    }
                }

            }
            visualScene.AppendChild(visualScene.FirstChild);
            visualScene.AppendChild(visualScene.FirstChild);

            recherche = doc.SelectNodes("//*/@url");
            for (int i = 0; i < recherche.Count; i++)
                if (recherche[i].Value[0] != '#')
                    recherche[i].Value = "#" + recherche[i].Value;

            recherche = doc.SelectNodes("//*/@target");
            for (int i = 0; i < recherche.Count; i++)
                if (recherche[i].Value[0] != '#')
                    recherche[i].Value = "#" + recherche[i].Value;

            recherche = doc.SelectNodes("//*/@source");
            for (int i = 0; i < recherche.Count; i++)
                if (recherche[i].Value[0] != '#')
                    recherche[i].Value = "#" + recherche[i].Value;


            FileStream mStream = new FileStream(fname, System.IO.FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.GetEncoding("ISO-8859-1"))
            {
                Formatting = Formatting.Indented,
                Indentation = 1,
                IndentChar = '	'
            };

            XmlNode collada = doc.SelectNodes("descendant::COLLADA")[0];
            XmlAttribute xmlns = doc.CreateAttribute("xmlns");
            xmlns.InnerText = "http://www.collada.org/2005/11/COLLADASchema";
            collada.Attributes.Remove(collada.Attributes["whocares"]);
            collada.Attributes.InsertBefore(xmlns, collada.Attributes[0]);
            

            doc.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            writer.Close();
            mStream.Close();

        }
        
        public void ResolveFileName(ref string filename)
        {
            string[] possible_extenstions = new string[] { string.Empty, ".png", ".jpg", ".jpeg", ".dds" };
            for (int pe = 0; pe < possible_extenstions.Length; pe++)
            {
                string fname = filename;
                if (pe > 0)
                    fname += possible_extenstions[pe];
                int subDir = 0;

                string[] split = fname.Split(new char[] { '\\', '/' });
                if (split.Length > 0 && split[0].Length > 0)
                {
                    int countPoint = 0;
                    for (int sp = 0; sp < split[0].Length; sp++)
                    {
                        if (split[0][sp] == '.')
                        {
                            countPoint++;
                        }
                    }
                    if (countPoint == split[0].Length)
                    {
                        while (fname[0] == '.')
                        {
                            fname = fname.Remove(0, 1);
                            subDir++;
                        }
                        fname = fname.Remove(0, 1);
                        string dir = this.Directory;

                        while (subDir > 0)
                        {
                            for (int sp = dir.Length - 1; sp > 0; sp--)
                            {
                                if (dir[sp] == '\\')
                                {
                                    dir = dir.Substring(0, sp);
                                    break;
                                }
                            }
                            subDir--;
                        }
                        if (File.Exists(dir + @"\" + fname))
                        {
                            fname = dir + @"\" + fname;
                        }
                        else if (File.Exists(this.Directory + @"\" + fname))
                        {
                            fname = this.Directory + @"\" + fname;
                        }
                    }
                }


                Uri uri;

                if (Uri.TryCreate(fname, UriKind.Absolute, out uri))
                {
                    fname = uri.AbsolutePath;
                }
                if (File.Exists(this.Directory + @"\" + fname))
                {
                    fname = this.Directory + @"\" + fname;
                }
                else
                if (!File.Exists(fname))
                {
                    int fileslashIndex = fname.IndexOf("file://");
                    if (fileslashIndex > -1)
                    {
                        fname = fname.Remove(fileslashIndex, 7);
                    }
                    fname = fname.Replace("/", "\\");
                    if (File.Exists(this.Directory + @"\" + fname))
                    {
                        fname = this.Directory + @"\" + fname;
                    }
                    else
                        if (!File.Exists(fname))
                    {
                        if (!fname.Contains(":\\") && File.Exists(this.Directory + @"\" + fname))
                        {
                            fname = this.Directory + @"\" + fname;
                        }
                        else
                        {
                            if (File.Exists(this.Directory + @"\" + Path.GetFileName(fname)))
                            {
                                fname = this.Directory + @"\" + Path.GetFileName(fname);
                            }
                        }
                    }

                }
                fname = fname.Replace("%20", " ");
                if (File.Exists(fname))
                {
                    /*int ind = fname.IndexOf(Program.ExecutableDirectory + @"\");
                    if (ind > -1)
                    {
                        fname = fname.Remove(ind, Program.ExecutableDirectory.Length + 1);
                    }*/
                    filename = fname;
                    break;
                }
            }
        }

        public DAE Parse()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = DAE.en;
            System.Threading.Thread.CurrentThread.CurrentCulture = DAE.en;

            int maxOffset = -1;
            float currVal = 0;
            int valCount = 0;
            int valIndex = 0;
            char separator = ' ';

            #region Parsing Joints

            GetJoints(false);

            #endregion

            #region Parsing Images
            for (int i = 0; i < this.images.Count; i++)
            {
                var initFromNode = this.images[i].SelectNodes("init_from");
                if (initFromNode.Count > 0)
                {
                    this.ImagesIDs.Add(this.images[i].Attributes["id"].Value);
                    string fileName = initFromNode[0].InnerText;
                    ResolveFileName(ref fileName);
                    this.ImagesFilenames.Add(fileName);
                }
            }

            #endregion

            #region Parsing Materials
                    for (int i = 0; i < this.materials.Count; i++)
            {
                var instanceEffectNode = this.materials[i].SelectNodes("instance_effect");
                if (instanceEffectNode.Count > 0)
                {
                    string url = instanceEffectNode[0].Attributes["url"].Value;
                    if (url.Length > 0 && url[0] == '#')
                        url = url.Remove(0, 1);

                    this.MaterialsEffectIDs.Add(url);
                }
                else
                {
                    this.MaterialsEffectIDs.Add("");
                }
                string matID = this.materials[i].Attributes["id"].Value;
                /*var nodesProp = this.materials[i].SelectNodes("descendant::user_properties");
                if (nodesProp.Count>0 &&
                    this.Document.SelectNodes("//*[@target='#" + matID + "']").Count == 0 &&
                    this.Document.SelectNodes("//*[@material='" + matID + "']").Count == 0)
                    matID = nodesProp[0].InnerText;*/

                this.MaterialsIDs.Add(matID);
            }
            #endregion

            #region Parsing Effects
            for (int i = 0; i < this.effects.Count; i++)
            {
                this.EffectsIDs.Add(this.effects[i].Attributes["id"].Value);
                var textureNode = this.effects[i].SelectNodes("descendant::texture");
                if (textureNode.Count > 0)
                {
                    string corresponding_imageID = textureNode[0].Attributes["texture"].Value;

                    if (!ImagesIDs.Contains(corresponding_imageID))
                    {
                        var rech = this.effects[i].SelectNodes("descendant::*[@id='" + corresponding_imageID + "' or @sid='" + corresponding_imageID + "']");
                        if (rech.Count>0)
                        {
                            rech = rech[0].SelectNodes("descendant::source");
                            if (rech.Count > 0)
                            {
                                rech = this.effects[i].SelectNodes("descendant::*[@id='" + rech[0].InnerText + "' or @sid='" + rech[0].InnerText + "']");
                                if (rech.Count > 0)
                                {
                                    rech = rech[0].SelectNodes("descendant::init_from");
                                    if (rech.Count > 0)
                                    {
                                        corresponding_imageID = rech[0].InnerText;
                                    }
                                }
                            }
                        }
                    }

                    this.EffectsImageIDs.Add(corresponding_imageID);
                }
                else
                {
                    this.EffectsImageIDs.Add("");
                }
            }
            #endregion

            #region Parsing Geometries
            for (int i = 0; i < this.geometries.Count; i++)
            {
                this.GeometryIDs.Add(this.geometries[i].Attributes["id"].Value);
                string position_SourceID = "";
                string normal_SourceID = "";
                string texcoord_SourceID = "";
                string color_SourceID = "";

                int position_SourceOffset = 0;
                int normal_SourceOffset = -1;
                int texcoord_SourceOffset = -1;
                int color_SourceOffset = -1;

                var verticesNode = this.geometries[i].SelectNodes("descendant::vertices");
                var trianglesNode = this.geometries[i].SelectNodes("descendant::triangles");
                if (trianglesNode.Count == 0)
                {
                    trianglesNode = this.geometries[i].SelectNodes("descendant::polylist");
                }

                if (trianglesNode.Count>0)
                {
                    int countTri = int.Parse(trianglesNode[0].Attributes["count"].Value);
                    var rootPNode = trianglesNode[0].SelectNodes("descendant::p")[0];
                    for (int t=1;t< trianglesNode.Count;t++)
                    {
                        int count_ = int.Parse(trianglesNode[t].Attributes["count"].Value);
                        string inner = trianglesNode[t].SelectNodes("descendant::p")[0].InnerText;
                        while (inner[0]==separator)
                        {
                            inner = inner.Remove(0, 1);
                        }
                        inner = " " + inner;
                        rootPNode.InnerText += inner;
                        countTri += count_;
                    }
                }
                string matID = "";
                var materialAttribute = trianglesNode[0].Attributes["material"];
                if (materialAttribute != null)
                    matID = materialAttribute.Value;


                if (!MaterialsIDs.Contains(matID))
                {
                    string geoID = GeometryIDs[GeometryIDs.Count-1];
                    var fixMatID = this.Document.SelectNodes("//instance_geometry[@url='#" + geoID + "' or @target='#" + geoID + "']");
                    if (fixMatID.Count>0)
                    {
                        fixMatID = fixMatID[0].SelectNodes("descendant::instance_material/@target");
                        if (fixMatID.Count > 0)
                        {
                            string newMatID = fixMatID[0].InnerText.Remove(0, 1);
                            if (MaterialsIDs.Contains(newMatID))
                            matID = newMatID;
                        }
                    }
                }
                this.PerGeometryMaterials.Add(matID);

                var vertexSemanticNode = trianglesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='vertex']");
                if (vertexSemanticNode.Count > 0)
                {
                    var offsetAttribute = vertexSemanticNode[0].Attributes["offset"];
                    if (offsetAttribute != null)
                        position_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = vertexSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        position_SourceID = sourceID_Attribute.Value;
                }
                if (verticesNode.Count > 0)
                {
                    position_SourceID = verticesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='position']")[0].Attributes["source"].Value;

                    var normal_SourceNode = verticesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='normal']");
                    if (normal_SourceNode.Count > 0)
                    {
                        normal_SourceID = normal_SourceNode[0].Attributes["source"].Value;
                        normal_SourceOffset = position_SourceOffset;
                    }

                    var texcoord_SourceNode = verticesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='texcoord']");
                    if (texcoord_SourceNode.Count > 0)
                    {
                        texcoord_SourceID = texcoord_SourceNode[0].Attributes["source"].Value;
                        texcoord_SourceOffset = position_SourceOffset;
                    }

                    var color_SourceNode = verticesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='color']");
                    if (color_SourceNode.Count > 0)
                    {
                        color_SourceID = color_SourceNode[0].Attributes["source"].Value;
                        color_SourceOffset = position_SourceOffset;
                    }
                }

                var texcoordSemanticNode = trianglesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='texcoord']");
                if (texcoordSemanticNode.Count > 0)
                {
                    var offsetAttribute = texcoordSemanticNode[0].Attributes["offset"];
                    if (offsetAttribute != null)
                        texcoord_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = texcoordSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        texcoord_SourceID = sourceID_Attribute.Value;
                }

                var normalSemanticNode = trianglesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='normal']");
                if (normalSemanticNode.Count > 0)
                {
                    var offsetAttribute = normalSemanticNode[0].Attributes["offset"];
                    if (offsetAttribute != null)
                        normal_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = normalSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        normal_SourceID = sourceID_Attribute.Value;
                }

                var colorSemanticNode = trianglesNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='color']");
                if (colorSemanticNode.Count > 0)
                {
                    var offsetAttribute = colorSemanticNode[0].Attributes["offset"];
                    if (offsetAttribute != null)
                        color_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = colorSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        color_SourceID = sourceID_Attribute.Value;
                }


                if (position_SourceID.Length > 0 && position_SourceID[0] == '#')
                    position_SourceID = position_SourceID.Remove(0, 1);
                if (normal_SourceID.Length > 0 && normal_SourceID[0] == '#')
                    normal_SourceID = normal_SourceID.Remove(0, 1);
                if (texcoord_SourceID.Length > 0 && texcoord_SourceID[0] == '#')
                    texcoord_SourceID = texcoord_SourceID.Remove(0, 1);
                if (color_SourceID.Length > 0 && color_SourceID[0] == '#')
                    color_SourceID = color_SourceID.Remove(0, 1);

                Vector3[] Vertices = new Vector3[0];
                Vector2[] TexCoordinates = new Vector2[0];
                Vector3[] Normals = new Vector3[0];
                Color[] Colors = new Color[0];

                #region Parsing POSITION-Array
                XmlNode source = this.geometries[i].SelectNodes("descendant::source[@id='" + position_SourceID + "']")[0];
                XmlNode accessor = source.SelectNodes("descendant::accessor")[0];
                int count = int.Parse(accessor.Attributes["count"].Value);
                string floatArray = source.SelectNodes("float_array")[0].InnerText;

                for (int j = 2; j < floatArray.Length && j < 20; j++)
                {
                    if (floatArray[j] == 9 ||
                        floatArray[j] == 32 ||
                        floatArray[j] == 160)
                    {
                        separator = floatArray[j];
                        break;
                    }
                }

                Vertices = new Vector3[count];
                string[] split = floatArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                valIndex = 0;

                for (int j = 0; j < split.Length; j++)
                {
                    if (Single.TryParse(split[j], out currVal))
                    {
                        if (valCount % 3 == 0)
                        {
                            Vertices[valIndex].X = currVal;
                        }
                        if (valCount % 3 == 1)
                        {
                            Vertices[valIndex].Y = currVal;
                        }
                        if (valCount % 3 == 2)
                        {
                            Vertices[valIndex].Z = currVal;
                            valIndex++;
                        }
                        valCount++;
                    }
                }
                #endregion

                #region Parsing TEXCOORD-Array
                if (texcoord_SourceOffset > -1)
                {
                    source = this.geometries[i].SelectNodes("descendant::source[@id='" + texcoord_SourceID + "']")[0];
                    accessor = source.SelectNodes("descendant::accessor")[0];
                    count = int.Parse(accessor.Attributes["count"].Value);
                    floatArray = source.SelectNodes("float_array")[0].InnerText;

                    TexCoordinates = new Vector2[count];
                    split = floatArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                    currVal = 0;
                    valCount = 0;
                    valIndex = 0;

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (Single.TryParse(split[j], out currVal))
                        {
                            if (valCount % 2 == 0)
                            {
                                TexCoordinates[valIndex].X = currVal;
                            }
                            if (valCount % 2 == 1)
                            {
                                /*currVal = 1 - currVal;*/
                                TexCoordinates[valIndex].Y = currVal;
                                valIndex++;
                            }
                            valCount++;
                        }
                    }
                }
                #endregion

                #region Parsing NORMAL-Array
                if (normal_SourceOffset > -1)
                {
                    source = this.geometries[i].SelectNodes("descendant::source[@id='" + normal_SourceID + "']")[0];
                    accessor = source.SelectNodes("descendant::accessor")[0];
                    count = int.Parse(accessor.Attributes["count"].Value);
                    floatArray = source.SelectNodes("float_array")[0].InnerText;

                    Normals = new Vector3[count];
                    split = floatArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                    currVal = 0;
                    valCount = 0;
                    valIndex = 0;

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (Single.TryParse(split[j], out currVal))
                        {
                            if (valCount % 3 == 0)
                            {
                                Normals[valIndex].X = currVal;
                            }
                            if (valCount % 3 == 1)
                            {
                                Normals[valIndex].Y = currVal;
                            }
                            if (valCount % 3 == 2)
                            {
                                Normals[valIndex].Z = currVal;
                                valIndex++;
                            }
                            valCount++;
                        }
                    }
                }
                #endregion

                Vector4 currV4 = Vector4.Zero;

                #region Parsing COLOR-Array
                if (color_SourceOffset > -1)
                {
                    source = this.geometries[i].SelectNodes("descendant::source[@id='" + color_SourceID + "']")[0];
                    accessor = source.SelectNodes("descendant::accessor")[0];
                    count = int.Parse(accessor.Attributes["count"].Value);
                    floatArray = source.SelectNodes("float_array")[0].InnerText;

                    Colors = new Color[count];
                    split = floatArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                    currVal = 0;
                    valCount = 0;
                    valIndex = 0;

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (Single.TryParse(split[j], out currVal))
                        {
                            if (valCount % 4 == 0)
                            {
                                currV4.X = currVal;
                            }
                            if (valCount % 4 == 1)
                            {
                                currV4.Y = currVal;
                            }
                            if (valCount % 4 == 2)
                            {
                                currV4.Z = currVal;
                            }
                            if (valCount % 4 == 3)
                            {
                                currV4.W = currVal;
                                Colors[valIndex] = new Color((int)(currV4.X * 255), (int)(currV4.Y * 255), (int)(currV4.Z * 255), (int)(currV4.W * 255));
                                valIndex++;
                            }
                            valCount++;
                        }
                    }
                }
                #endregion

                #region Parsing Triangles-Array
                string TriangleIndices = trianglesNode[0].SelectNodes("p")[0].InnerText;
                split = TriangleIndices.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });

                maxOffset = -1;
                if (position_SourceOffset > maxOffset)
                    maxOffset = position_SourceOffset;
                if (texcoord_SourceOffset > maxOffset)
                    maxOffset = texcoord_SourceOffset;
                if (normal_SourceOffset > maxOffset)
                    maxOffset = normal_SourceOffset;
                if (color_SourceOffset > maxOffset)
                    maxOffset = color_SourceOffset;
                maxOffset++;
                valCount = 0;
                int currInt = 0;
                int[] indicesOrdered = new int[maxOffset];


                this.GeometryDataVertex.Add(Vertices);
                this.GeometryDataTexcoordinates.Add(TexCoordinates);
                this.GeometryDataNormals.Add(Normals);
                this.GeometryDataColors.Add(Colors);

                this.GeometryDataVertex_i.Add(new List<int>(0));
                this.GeometryDataTexcoordinates_i.Add(new List<int>(0));
                this.GeometryDataNormals_i.Add(new List<int>(0));
                this.GeometryDataColors_i.Add(new List<int>(0));

                for (int j = 0; j < split.Length; j++)
                {
                    if (int.TryParse(split[j], out currInt))
                    {
                        if (valCount % maxOffset == position_SourceOffset)
                        {
                            this.GeometryDataVertex_i[this.GeometryDataVertex.Count - 1].Add(currInt);
                        }
                        if (valCount % maxOffset == texcoord_SourceOffset)
                        {
                            this.GeometryDataTexcoordinates_i[this.GeometryDataTexcoordinates.Count - 1].Add(currInt);
                        }
                        if (valCount % maxOffset == normal_SourceOffset)
                        {
                            this.GeometryDataNormals_i[this.GeometryDataNormals.Count - 1].Add(currInt);
                        }
                        if (valCount % maxOffset == color_SourceOffset)
                        {
                            this.GeometryDataColors_i[this.GeometryDataColors.Count - 1].Add(currInt);
                        }
                        valCount++;
                    }
                }
                #endregion

            }
            #endregion

            #region Parse Controllers

            for (int i = 0; i < this.controllers.Count; i++)
            {
                string matID = this.PerGeometryMaterials[i];

                if (!MaterialsIDs.Contains(matID))
                {
                    string contID = this.controllers[i].Attributes["id"].Value;
                    var fixMatID = this.controllers[i].SelectNodes("//instance_controller[@url='#" + contID + "' or @target='#" + contID + "']");
                    if (fixMatID.Count > 0)
                    {
                        fixMatID = fixMatID[0].SelectNodes("descendant::instance_material/@target");
                        if (fixMatID.Count > 0)
                        {
                            string newMatID = fixMatID[0].InnerText.Remove(0, 1);
                            if (MaterialsIDs.Contains(newMatID))
                                this.PerGeometryMaterials[i] = newMatID;
                        }
                    }
                }

                Matrix4 shapeMatrix = Matrix4.Identity;

                string joints_SourceID = "";
                string matrices_SourceID = "";
                string weights_SourceID = "";

                int joints_SourceOffset = -1;
                int matrices_SourceOffset = -1;
                int weights_SourceOffset = -1;

                var shapeMatrixNode = this.controllers[i].SelectNodes("descendant::skin/bind_shape_matrix");
                if (shapeMatrixNode.Count > 0)
                    shapeMatrix = Matrix4.Identity;// ParseMatrices(shapeMatrixNode[0].InnerText, 1)[0];

                var jointsNode = this.controllers[i].SelectNodes("descendant::joints");
                var vertexWeightsNode = this.controllers[i].SelectNodes("descendant::vertex_weights");

                var vwJointSemanticNode = vertexWeightsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='joint']");
                if (vwJointSemanticNode.Count > 0)
                {
                    var offsetAttribute = vwJointSemanticNode[0].Attributes["offset"];

                    if (offsetAttribute != null)
                        joints_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = vwJointSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        joints_SourceID = sourceID_Attribute.Value;
                }

                if (jointsNode.Count > 0)
                {
                    var jointSemanticNode = jointsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='joint']");
                    if (jointSemanticNode.Count > 0)
                    {
                        var sourceID_Attribute = jointSemanticNode[0].Attributes["source"];
                        if (sourceID_Attribute != null)
                            joints_SourceID = sourceID_Attribute.Value;
                    }

                    var weightSemanticNode = jointsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='weight']");
                    if (weightSemanticNode.Count > 0)
                    {
                        var sourceID_Attribute = weightSemanticNode[0].Attributes["source"];
                        if (sourceID_Attribute != null)
                        {
                            weights_SourceID = sourceID_Attribute.Value;
                            weights_SourceOffset = joints_SourceOffset;
                        }
                    }

                    var matriceSemanticNode = jointsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='inv_bind_matrix']");
                    if (matriceSemanticNode.Count > 0)
                    {
                        var sourceID_Attribute = matriceSemanticNode[0].Attributes["source"];
                        if (sourceID_Attribute != null)
                        {
                            matrices_SourceID = sourceID_Attribute.Value;
                            matrices_SourceOffset = joints_SourceOffset;
                        }
                    }
                }

                var vwWeightSemanticNode = vertexWeightsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='weight']");
                if (vwWeightSemanticNode.Count > 0)
                {
                    var offsetAttribute = vwWeightSemanticNode[0].Attributes["offset"];

                    if (offsetAttribute != null)
                        weights_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = vwWeightSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        weights_SourceID = sourceID_Attribute.Value;
                }

                var vwMatrixSemanticNode = vertexWeightsNode[0].SelectNodes("input[translate(@semantic, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='inv_bind_matrix']");
                if (vwMatrixSemanticNode.Count > 0)
                {
                    var offsetAttribute = vwMatrixSemanticNode[0].Attributes["offset"];

                    if (offsetAttribute != null)
                        matrices_SourceOffset = int.Parse(offsetAttribute.Value);

                    var sourceID_Attribute = vwMatrixSemanticNode[0].Attributes["source"];
                    if (sourceID_Attribute != null)
                        matrices_SourceID = sourceID_Attribute.Value;
                }

                if (joints_SourceID.Length > 0 && joints_SourceID[0] == '#')
                    joints_SourceID = joints_SourceID.Remove(0, 1);
                if (matrices_SourceID.Length > 0 && matrices_SourceID[0] == '#')
                    matrices_SourceID = matrices_SourceID.Remove(0, 1);
                if (weights_SourceID.Length > 0 && weights_SourceID[0] == '#')
                    weights_SourceID = weights_SourceID.Remove(0, 1);

                #region Parsing JOINT-Array
                XmlNode source = this.controllers[i].SelectNodes("descendant::source[@id='" + joints_SourceID + "']")[0];
                XmlNode accessor = source.SelectNodes("descendant::accessor")[0];
                int count = int.Parse(accessor.Attributes["count"].Value);
                string nameArray = source.SelectNodes("Name_array")[0].InnerText;
                valIndex = 0;

                string[] Joints = new string[count];
                Matrix4[] Matrices = new Matrix4[0];
                float[] Weights = new float[0];

                string[] split = nameArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });

                for (int j = 0; j < split.Length; j++)
                {
                    for (int k = 0; k < this.Skeleton.Joints.Count; k++)
                        if (this.Skeleton.Joints[k].Name ==  split[j])
                        {
                            Joints[valIndex] = split[j];
                            valIndex++;
                            break;
                        }
                }
                #endregion

                #region Parsing Matrices-Array

                if (matrices_SourceOffset > -1)
                {
                    source = this.controllers[i].SelectNodes("descendant::source[@id='" + matrices_SourceID + "']")[0];
                    accessor = source.SelectNodes("descendant::accessor")[0];
                    count = int.Parse(accessor.Attributes["count"].Value);

                    string floatArray = source.SelectNodes("float_array")[0].InnerText;

                    Matrices = ParseMatrices(floatArray, count);
                }
                #endregion

                #region Parsing Weights-Array

                if (weights_SourceOffset > -1)
                {
                    source = this.controllers[i].SelectNodes("descendant::source[@id='" + weights_SourceID + "']")[0];
                    accessor = source.SelectNodes("descendant::accessor")[0];
                    count = int.Parse(accessor.Attributes["count"].Value);

                    string floatArray = source.SelectNodes("float_array")[0].InnerText;

                    valIndex = 0;
                    Weights = new float[count];

                    split = floatArray.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (Single.TryParse(split[j], out currVal))
                        {
                            Weights[valIndex] = currVal;
                            valIndex++;
                        }
                    }
                }
                #endregion


                #region Parsing Vertex-Weights
                string vcount = vertexWeightsNode[0].SelectNodes("vcount")[0].InnerText;
                string v = vertexWeightsNode[0].SelectNodes("v")[0].InnerText;


                maxOffset = -1;
                if (joints_SourceOffset > maxOffset)
                    maxOffset = joints_SourceOffset;
                if (matrices_SourceOffset > maxOffset)
                    maxOffset = matrices_SourceOffset;
                if (weights_SourceOffset > maxOffset)
                    maxOffset = weights_SourceOffset;
                maxOffset++;

                int[] indicesOrdered = new int[maxOffset];

                this.ControllerDataJoints.Add(Joints);
                this.ControllerDataMatrices.Add(Matrices);
                this.ControllerDataWeights.Add(Weights);

                this.ControllerDataJoints_i.Add(new List<List<int>>(0));
                this.ControllerDataMatrices_i.Add(new List<List<int>>(0));
                this.ControllerDataWeights_i.Add(new List<List<int>>(0));


                split = vcount.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                List<int> vcountInt = new List<int>(0);

                int currInt = 0;

                for (int j = 0; j < split.Length; j++)
                {
                    if (int.TryParse(split[j], out currInt))
                    {
                        this.ControllerDataJoints_i[this.ControllerDataJoints_i.Count - 1].Add(new List<int>(0));
                        this.ControllerDataMatrices_i[this.ControllerDataMatrices_i.Count - 1].Add(new List<int>(0));
                        this.ControllerDataWeights_i[this.ControllerDataWeights_i.Count - 1].Add(new List<int>(0));
                        vcountInt.Add(currInt);
                    }
                }

                valCount = 0;
                currInt = 0;
                int currWeightIndex = -1;
                int currvCountIndex = 0;

                split = v.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
                for (int j = 0; j < split.Length; j++)
                {
                    if (int.TryParse(split[j], out currInt))
                    {
                        if (valCount % maxOffset == joints_SourceOffset)
                        {
                            this.ControllerDataJoints_i[this.ControllerDataJoints_i.Count - 1][currvCountIndex].Add(currInt);
                        }
                        if (valCount % maxOffset == matrices_SourceOffset)
                        {
                            this.ControllerDataMatrices_i[this.ControllerDataMatrices_i.Count - 1][currvCountIndex].Add(currInt);
                        }
                        if (valCount % maxOffset == weights_SourceOffset)
                        {
                            this.ControllerDataWeights_i[this.ControllerDataWeights_i.Count - 1][currvCountIndex].Add(currInt);
                            currWeightIndex++;
                            if (currWeightIndex == vcountInt[currvCountIndex] - 1)
                            {
                                currWeightIndex = -1;
                                currvCountIndex++;
                            }
                        }
                        valCount++;
                    }
                }
                #endregion

                this.ShapeMatrices.Add(shapeMatrix);
                this.ControllersIDs.Add(this.controllers[i].Attributes["id"].Value);
                this.PerControllerGeometry.Add(this.controllers[i].SelectSingleNode("skin").Attributes["source"].Value.Remove(0, 1));
            }

            #endregion

            #region Get Per-Geometry Textures
            for (int i = 0; i < this.PerGeometryMaterials.Count; i++)
            {
                //string currEffectID = this.MaterialsEffectIDs[MaterialsIDs.IndexOf(this.PerGeometryMaterials[i])];
                //string currImageID = this.EffectsImageIDs[this.EffectsIDs.IndexOf(currEffectID)];

                string currEffectID = "";

                int indmID = MaterialsIDs.IndexOf(this.PerGeometryMaterials[i]);
                if (indmID > -1 && indmID < this.MaterialsEffectIDs.Count)
                    currEffectID = this.MaterialsEffectIDs[indmID];

                string currImageID = "";
                int indmEID = this.EffectsIDs.IndexOf(currEffectID);
                if (indmEID > -1 && indmEID < this.EffectsImageIDs.Count)
                    currImageID = this.EffectsImageIDs[indmEID];

                string currImageFileName = "";

                int ind = this.ImagesIDs.IndexOf(currImageID);
                if (ind>-1)
                {
                    currImageFileName = this.ImagesFilenames[ind];
                }
            }

            #endregion

            /*if (this.ControllerDataJoints.Count == 0)
            {
                Joint b = new Joint("bone000");
                b.Matrix = Matrix4.CreateScale(1f);
                this.Skeleton.Joints.Add(b);
                this.Skeleton.Compile();

                for (int i=0;i<this.GeometryIDs.Count;i++)
                {
                    this.PerControllerGeometry.Add(this.GeometryIDs[i]);
                    this.ControllersIDs.Add("id" + i.ToString("d3"));
                    this.ControllerDataJoints.Add(new string[] { "bone000" });
                    this.ControllerDataWeights.Add(new float[] { 1f });
                    this.ControllerDataMatrices.Add(new Matrix4[] { Matrix4.CreateScale(1f) });


                    this.ControllerDataJoints_i.Add(new List<List<int>>(0));
                    this.ControllerDataMatrices_i.Add(new List<List<int>>(0));
                    this.ControllerDataWeights_i.Add(new List<List<int>>(0));

                    for (int j = 0; j < this.GeometryDataVertex[i].Length; j++)
                    {
                        this.ControllerDataJoints_i[this.ControllerDataJoints_i.Count - 1].Add(new List<int>(new int[] {0}));
                        this.ControllerDataMatrices_i[this.ControllerDataMatrices_i.Count - 1].Add(new List<int>(new int[] {0 }));
                        this.ControllerDataWeights_i[this.ControllerDataWeights_i.Count - 1].Add(new List<int>(new int[] {0 }));
                    }
                }
            }*/

            this.Meshes = new Mesh[this.GeometryIDs.Count];

            for (int i=0;i<this.GeometryIDs.Count;i++)
            {
                int controllerIndex = -1;
                for (int j=0;j< this.PerControllerGeometry.Count;j++)
                {
                    if (this.PerControllerGeometry[j] == this.GeometryIDs[i])
                    {
                        controllerIndex = j;
                        break;
                    }
                }


                bool hasController = controllerIndex > -1;

                Mesh mesh = null;
                if (hasController)
                    mesh = new DynamicMesh();
                else
                    mesh = new StaticMesh();

                mesh.Name = this.GeometryIDs[i];

                bool hasTexCoords = this.GeometryDataTexcoordinates[i].Length > 0;
                bool hasNormals = this.GeometryDataNormals[i].Length > 0;
                bool hasColors = this.GeometryDataColors[i].Length > 0;

                bool otherThanZeroFound = false;

                List<float[]> weights = new List<float[]>(0);
                List<ushort[]> influences = new List<ushort[]>(0);

                List<Vector3> positions = new List<Vector3>(0);
                List<Vector2> textureCoords = new List<Vector2>(0);
                List<Vector3> normals = new List<Vector3>(0);
                List<Color> colors = new List<Color>(0);

                for (int j = 0; j < this.GeometryDataVertex_i[i].Count; j++)
                {
                    int vertexIndex = this.GeometryDataVertex_i[i][j];
                    Vector3 position = Vector3.Zero;
                    if (vertexIndex< this.GeometryDataVertex[i].Length)
                        position = this.GeometryDataVertex[i][vertexIndex];
                    Vector2 textureCoord = Vector2.Zero;
                    Vector3 normal = Vector3.Zero;
                    Color color = Color.White;
                    List<KeyValuePair<ushort, float>> weight_influence = new List<KeyValuePair<ushort, float>>(0);


                    if (hasTexCoords)
                    {
                        int texCoordIndex = this.GeometryDataTexcoordinates_i[i][j];
                        if (texCoordIndex < this.GeometryDataTexcoordinates[i].Length)
                            textureCoord = this.GeometryDataTexcoordinates[i][texCoordIndex];
                    }
                    if (hasNormals)
                    {
                        int normalIndex = this.GeometryDataNormals_i[i][j];
                        if (normalIndex < this.GeometryDataNormals[i].Length)
                        {
                            normal = this.GeometryDataNormals[i][normalIndex];
                            if (!otherThanZeroFound && Vector3.Distance(normal, Vector3.Zero) > 0)
                                otherThanZeroFound = true;
                        }
                    }
                    if (hasColors)
                    {
                        int colorIndex = this.GeometryDataColors_i[i][j];
                        if (colorIndex < this.GeometryDataColors[i].Length)
                            color = this.GeometryDataColors[i][colorIndex];
                    }
                    if (hasController)
                    {
                        int infCount = 0;
                        if (vertexIndex < this.ControllerDataWeights_i[controllerIndex].Count)
                            infCount = this.ControllerDataWeights_i[controllerIndex][vertexIndex].Count;
                        for (int k=0;k< infCount;k++)
                        {
                            int weightIndex = this.ControllerDataWeights_i[controllerIndex][vertexIndex][k];
                            int influenceIndex = this.ControllerDataJoints_i[controllerIndex][vertexIndex][k];
                            float weight = this.ControllerDataWeights[controllerIndex][weightIndex];
                            ushort influence = (ushort)this.Skeleton.IndexOf(this.ControllerDataJoints[controllerIndex][influenceIndex]);
                            weight_influence.Add(new KeyValuePair<ushort, float>(influence, weight));
                        }
                        weight_influence.Sort((x, y) => (x.Value.CompareTo(y.Value)));
                    }

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
                        if (hasController && influences.Count>0)
                        {
                            if (influences[k].Length != weight_influence.Count)
                                continue;
                            bool cont = false;
                            for (int l = 0; l < influences[k].Length; l++)
                            {
                                if (influences[k][l] != weight_influence[l].Key)
                                {
                                    cont = true;
                                    break;
                                }
                                if (Math.Abs(weights[k][l] - weight_influence[l].Value) > 0.01)
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
                            for (int k=0;k< influence.Length;k++)
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
                if (hasTexCoords)
                    mesh.TextureCoords.AddRange(textureCoords);
                if (hasNormals && otherThanZeroFound)
                    mesh.Normals.AddRange(normals);
                if (hasColors)
                    mesh.Colors.AddRange(colors);

                if (hasController)
                {
                    mesh.Influences.AddRange(influences);
                    mesh.Weights.AddRange(weights);
                }

                mesh.Texture = StaticReferences.whitePixel1x1;
                mesh.BumpMapping = StaticReferences.bumpPixel1x1;

                int materialIndex = this.MaterialsIDs.IndexOf(this.PerGeometryMaterials[i]);
                if (materialIndex>-1)
                {
                    string effect = this.MaterialsEffectIDs[materialIndex];
                    int effectIndex = this.EffectsIDs.IndexOf(effect);
                    if (effectIndex > -1)
                    {
                        string imageID = this.EffectsImageIDs[effectIndex];
                        int imageIndex = this.ImagesIDs.IndexOf(imageID);
                        if (imageIndex > -1)
                        {
                            mesh.Texture = Texture.LoadTexture(this.ImagesFilenames[imageIndex], OpenTK.Graphics.OpenGL.TextureMinFilter.Linear,
                                OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat, OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat);

                            string assumed_bump = Path.GetDirectoryName(this.ImagesFilenames[imageIndex]) + @"\" + Path.GetFileNameWithoutExtension(this.ImagesFilenames[imageIndex]) + "_bump" + Path.GetExtension(this.ImagesFilenames[imageIndex]);
                            if (File.Exists(assumed_bump))
                            {
                                mesh.BumpMapping = Texture.LoadTexture(assumed_bump, OpenTK.Graphics.OpenGL.TextureMinFilter.Linear,
                                    OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat, OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat);
                            }
                        }
                    }
                }


                this.Meshes[i] = mesh;
            }
            return this;
        }


        public int MultiAnim_Count;
        public int[] FrameCounts;
        public Matrix4[][] Anims = new Matrix4[0][];
        public Matrix4[][] AnimsBackup = new Matrix4[0][];

        public Matrix4[] Anim = new Matrix4[0];
        public Matrix4[] AnimBackup = new Matrix4[0];
        public int FrameCount = 0;


        public int GetJoints(bool removeTransforms)
        {
            int transformCount = 0;
            this.Skeleton.Joints.Clear();

            if (removeTransforms)
            {
                XmlNodeList transforms = this.Document.SelectNodes("descendant::library_visual_scenes//node[matrix]");
                for (int i = 0; i < transforms.Count; i++)
                {
                    if (transforms[i].Attributes["type"] == null || transforms[i].Attributes["type"].Value.ToLower() != "joint")
                    {
                        Matrix4 m = ParseMatrices(transforms[i].SelectNodes("matrix")[0].InnerText, 1)[0];

                        XmlNodeList children = transforms[i].SelectNodes("node[matrix]");
                        int childrenCount = children.Count;

                        for (int c = 0; c < children.Count; c++)
                        {
                            if (children[c].Attributes["type"] != null && children[c].Attributes["type"].Value.ToLower() == "joint")
                            {
                                childrenCount--;
                            }
                            Matrix4 m2 = ParseMatrices(children[c].SelectNodes("matrix")[0].InnerText, 1)[0];
                            m2 *= m;
                            transformCount++;
                            children[c].SelectNodes("matrix")[0].InnerText =
                                m2.M11 + " " + m2.M21 + " " + m2.M31 + " " + m2.M41 + " " +
                                m2.M12 + " " + m2.M22 + " " + m2.M32 + " " + m2.M42 + " " +
                                m2.M13 + " " + m2.M23 + " " + m2.M33 + " " + m2.M43 + " " +
                                m2.M14 + " " + m2.M24 + " " + m2.M34 + " " + m2.M44;
                        }
                        if (childrenCount == 0)
                        {
                            var parent = transforms[i].ParentNode;

                            parent.RemoveChild(transforms[i]);
                            for (int c = 0; c < children.Count; c++)
                                parent.AppendChild(children[c]);
                        }
                    }
                }
            }
            
            XmlNodeList joints = this.Document.SelectNodes("descendant::library_visual_scenes//node[translate(@type, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='joint']");

            for (int i = 0; i < joints.Count; i++)
            {
                string jointName = joints[i].Attributes["name"].Value;
                Joint joint = new Joint(jointName);
                joint.Matrix = this.ParseMatrices(joints[i].SelectNodes("matrix")[0].InnerText, 1)[0];
                this.Skeleton.Joints.Add(joint);
            }
            for (int i = 0; i < joints.Count; i++)
            {
                XmlNode parent = joints[i].ParentNode;
                if (parent.Attributes["type"] != null && parent.Attributes["type"].Value.ToLower() == "joint")
                {
                    string jointName = joints[i].Attributes["name"].Value;
                    string parentJointName = parent.Attributes["name"].Value;
                    this.Skeleton.GetJoint(jointName).Parent = this.Skeleton.GetJoint(parentJointName);
                }
            }

            /* Computing Joints */

            this.Skeleton.Compile();
            return transformCount;
        }

        readonly List<string> ImagesIDs;
        readonly List<string> ImagesFilenames;
        readonly List<string> PerGeometryMaterials;
        List<string> MaterialsIDs;
        readonly List<string> MaterialsEffectIDs; /* Data is corresponding effect ID (an URL, with #) */
        readonly List<string> EffectsIDs;
        readonly List<string> EffectsImageIDs; /* Data is corresponding image ID */

        public readonly List<string> GeometryIDs;
        public readonly List<Vector3[]> GeometryDataVertex;
        public readonly List<Vector2[]> GeometryDataTexcoordinates;
        public readonly List<Vector3[]> GeometryDataNormals;
        public readonly List<Color[]> GeometryDataColors;
        public readonly List<List<int>> GeometryDataVertex_i;
        public readonly List<List<int>> GeometryDataTexcoordinates_i;
        public readonly List<List<int>> GeometryDataNormals_i;
        public readonly List<List<int>> GeometryDataColors_i;

        readonly List<Matrix4> ShapeMatrices;
        public readonly List<string> ControllersIDs;
        public readonly List<string> PerControllerGeometry;
        public readonly List<string[]> ControllerDataJoints;
        public readonly List<Matrix4[]> ControllerDataMatrices;
        public readonly List<float[]> ControllerDataWeights;
        public readonly List<List<List<int>>> ControllerDataJoints_i;
        public readonly List<List<List<int>>> ControllerDataMatrices_i;
        public readonly List<List<List<int>>> ControllerDataWeights_i;
        readonly List<string> VisualScenesIDs;
        readonly List<List<string>> JointsIDs;
        readonly List<List<Matrix4>> JointsMatrices;
        readonly List<List<string>> SurfacesIDs;
        readonly List<List<string>> SurfacesMaterialsID;

        public Matrix4[] ParseMatrices(string content, int count)
        {
            Matrix4[] output = new Matrix4[count];
            char separator = ' ';

            for (int j = 2; j < content.Length && j < 20; j++)
            {
                if (content[j] == 9 ||
                    content[j] == 32 ||
                    content[j] == 160)
                {
                    separator = content[j];
                    break;
                }
            }

            string[] split = content.Split(new char[] { separator, '\x09', '\x20', '\xA0', '\r', '\n' });
            float currVal = 0;
            int valCount = 0;
            int valIndex = 0;

            for (int j = 0; j < split.Length; j++)
            {
                if (Single.TryParse(split[j], out currVal))
                {
                    if (valCount % 16 == 0)
                        output[valIndex].M11 = currVal;
                    if (valCount % 16 == 1)
                        output[valIndex].M21 = currVal;
                    if (valCount % 16 == 2)
                        output[valIndex].M31 = currVal;
                    if (valCount % 16 == 3)
                        output[valIndex].M41 = currVal;

                    if (valCount % 16 == 4)
                        output[valIndex].M12 = currVal;
                    if (valCount % 16 == 5)
                        output[valIndex].M22 = currVal;
                    if (valCount % 16 == 6)
                        output[valIndex].M32 = currVal;
                    if (valCount % 16 == 7)
                        output[valIndex].M42 = currVal;

                    if (valCount % 16 == 8)
                        output[valIndex].M13 = currVal;
                    if (valCount % 16 == 9)
                        output[valIndex].M23 = currVal;
                    if (valCount % 16 == 10)
                        output[valIndex].M33 = currVal;
                    if (valCount % 16 == 11)
                        output[valIndex].M43 = currVal;

                    if (valCount % 16 == 12)
                        output[valIndex].M14 = currVal;
                    if (valCount % 16 == 13)
                        output[valIndex].M24 = currVal;
                    if (valCount % 16 == 14)
                        output[valIndex].M34 = currVal;
                    if (valCount % 16 == 15)
                    {
                        output[valIndex].M44 = currVal;
                        valIndex++;
                    }
                    valCount++;
                }
            }
            return output;
        }

        public static string ToString(Matrix4 m)
        {
            string s = "";
            s += m.M11.ToString("0.000000") + " " + m.M21.ToString("0.000000") + " " + m.M31.ToString("0.000000") + " " + m.M41.ToString("0.000000") + "\r\n";
            s += m.M12.ToString("0.000000") + " " + m.M22.ToString("0.000000") + " " + m.M32.ToString("0.000000") + " " + m.M42.ToString("0.000000") + "\r\n";
            s += m.M13.ToString("0.000000") + " " + m.M23.ToString("0.000000") + " " + m.M33.ToString("0.000000") + " " + m.M43.ToString("0.000000") + "\r\n";
            s += m.M14.ToString("0.000000") + " " + m.M24.ToString("0.000000") + " " + m.M34.ToString("0.000000") + " " + m.M44.ToString("0.000000") + "\r\n";
            return s;
        }
        public static string ToStringAccurate(Matrix4 m)
        {
            string s = "";
            s += ((Decimal)m.M11) + " " + ((Decimal)m.M21) + " " + ((Decimal)m.M31) + " " + ((Decimal)m.M41) + "\r\n";
            s += ((Decimal)m.M12) + " " + ((Decimal)m.M22) + " " + ((Decimal)m.M32) + " " + ((Decimal)m.M42) + "\r\n";
            s += ((Decimal)m.M13) + " " + ((Decimal)m.M23) + " " + ((Decimal)m.M33) + " " + ((Decimal)m.M43) + "\r\n";
            s += ((Decimal)m.M14) + " " + ((Decimal)m.M24) + " " + ((Decimal)m.M34) + " " + ((Decimal)m.M44) + "\r\n";
            return s;
        }
    }
}
