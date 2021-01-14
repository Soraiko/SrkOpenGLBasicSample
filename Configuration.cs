using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Configuration
    {
        public int[] mesh_order;
        public bool ignore_normals;
        public Configuration(string filename)
        {
            string[] configuration = File.ReadAllLines(filename);
            for (int i = 0; i < configuration.Length; i++)
            {
                string[] spli = configuration[i].Split("=");
                switch (spli[0])
                {
                    case "mesh_order":
                        string[] spli2 = spli[1].Split(",");
                        int[] mesh_order = new int[spli2.Length];
                        for (int c = 0; c < mesh_order.Length; c++)
                        {
                            mesh_order[c] = int.Parse(spli2[c]);
                        }
                        this.mesh_order = mesh_order;
                        break;
                    case "ignore_normals":
                        this.ignore_normals = true;
                        break;
                }
            }
        }
    }
}
