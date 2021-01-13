using System;
using OpenTK;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Moveset
    {
        public List<Animation> Animations;
        public int AnimationIndex = 0;
        public int AnimationFrame = 0;

        public Moveset(string folderName)
        {
            this.Animations = new List<Animation>(0);
            string[] animationsFileNames = Directory.GetFiles(folderName, "*.bin");
            for (int i=0;i< animationsFileNames.Length;i++)
                this.Animations.Add(new Animation(animationsFileNames[i]));
        }

        public void GetNextFrame(ref Matrix4[] rememberMatrices, List<Joint> joints)
        {
            Animation animation = this.Animations[this.AnimationIndex];
            Matrix4[] animationData = animation.Data;
            int position = this.AnimationFrame * rememberMatrices.Length;
            for (int i=0;i< rememberMatrices.Length;i++)
            {
                joints[i].Matrix = animationData[position++];
            }
            this.AnimationFrame++;
            if (AnimationFrame>= animation.MaxFrame)
            {
                this.AnimationFrame = (int)animation.MinFrame;
            }
        }
    }
}
