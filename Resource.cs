using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Resource
    {
        public bool SkipRender = false;
        public string Directory;
        public string Name;

        public Configuration Configuration;
        public Resource Reference;
        static List<Resource> References;

        static Resource()
        {
            References = new List<Resource>(0);
        }

        public Resource(string filename)
        {
            this.Name = Path.GetFileNameWithoutExtension(filename);
            this.Directory = Path.GetDirectoryName(filename);

            for (int i = 0; i < References.Count; i++)
            {
                if (References[i].Name == this.Name && References[i].Directory == this.Directory)
                {
                    this.Reference = References[i];
                    break;
                }
            }

            if (this.Reference == null)
            {
                References.Add(this);

                if (File.Exists(this.Directory + @"\" + this.Name + ".conf"))
                {
                    this.Configuration = new Configuration(this.Directory + @"\" + this.Name + ".conf");
                }
            }
            else
            {
                this.Configuration = this.Reference.Configuration;
                if (this is Model)
                {
                    var model = this as Model;
                    var reference = this.Reference as Model;
                    model.Meshes = reference.Meshes;
                    model.Skeleton = reference.Skeleton.Clone();
                    model.Skeleton.Compile(model);
                }
                if (this is Animation)
                {
                    var animation = this as Animation;
                    var reference = this.Reference as Animation;
                    animation.Data = reference.Data;
                    animation.MaxFrame = reference.MaxFrame;
                    animation.MinFrame = reference.MinFrame;
                }
            }
        }
    }
}
