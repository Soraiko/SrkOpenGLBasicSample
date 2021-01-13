using System;
using System.IO;
using OpenTK;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Animation : Resource
    {
        public float MaxFrame;
        public float MinFrame;
        public Matrix4[] Data;

        public Animation(string filename) : base(filename)
        {
            if (this.Reference != null)
                return;
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            this.MaxFrame = br.ReadInt32();
            //this.MaxFrame = br.ReadInt32();
            fs.Position = 0x10;
            this.Data = new Matrix4[(fs.Length-0x10)/0x40];
            int position = 0;
            while (position< this.Data.Length)
            {
                this.Data[position] = new Matrix4(
                    br.ReadSingle(),br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                position++;
            }
        }
    }
}
