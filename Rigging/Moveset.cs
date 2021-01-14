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
        int animationIndex;
        public int AnimationIndex
        {
            get
            {
                return this.animationIndex;
            }
            set
            {
                if (this.animationIndex == value)
                    return;
                this.Interpolation = 1f;
                this.AnimationFrame = 0;
                this.animationIndex = value;
            }
        }
        public int AnimationFrame = 0;

        public Moveset(string folderName)
        {
            this.Animations = new List<Animation>(0);
            string[] animationsFileNames = Directory.GetFiles(folderName, "*.bin");
            for (int i=0;i< animationsFileNames.Length;i++)
                this.Animations.Add(new Animation(animationsFileNames[i]));
        }

        public float Interpolation = 0f;

        public void GetNextFrame(ref Matrix4[] rememberMatrices, List<Joint> joints)
        {
            Animation animation = this.Animations[this.AnimationIndex];
            Matrix4[] animationData = animation.Data;
            int position = this.AnimationFrame * rememberMatrices.Length;

            bool interpolating = this.Interpolation > 0;

            float interpolation = this.Interpolation;
            float oneMinusInterpolation = 1f - this.Interpolation;

            for (int i=0;i< rememberMatrices.Length;i++)
            {
                joints[i].Matrix = animationData[position++];
                if (interpolating)
                {
                    Vector3 rememberScale = rememberMatrices[i].ExtractScale();
                    Vector3 scale = rememberScale * interpolation + joints[i].Matrix.ExtractScale() * oneMinusInterpolation;

                    Vector3 rememberTranslation = rememberMatrices[i].ExtractTranslation();
                    Vector3 translation = rememberTranslation * interpolation + joints[i].Matrix.ExtractTranslation() * oneMinusInterpolation;

                    Quaternion rememberQuaternion = rememberMatrices[i].ExtractRotation();
                    Quaternion quaternion = Quaternion.Slerp(rememberQuaternion, joints[i].Matrix.ExtractRotation(), oneMinusInterpolation);

                    joints[i].Matrix = Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(quaternion) * Matrix4.CreateTranslation(translation);
                }
                else
                {
                    rememberMatrices[i] = joints[i].Matrix;
                }

            }

            if (interpolating)
                this.Interpolation -= 0.1f;

            this.AnimationFrame++;
            if (AnimationFrame>= animation.MaxFrame)
            {
                this.AnimationFrame = (int)animation.MinFrame;
            }
        }
    }
}
