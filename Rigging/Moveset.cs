using System;
using OpenTK;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Moveset
    {
        public Animation Animation;
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
                this.Interpolation = 0f;
                this.AnimationFrame = 0;
                this.animationIndex = value;
                if (this.animationIndex > -1)
                    this.Animation = this.Animations[this.animationIndex];
                else
                    this.Animation = null;
            }
        }
        public float AnimationFrame = 0;

        public Moveset(string folderName)
        {
            this.Animations = new List<Animation>(0);
            string[] animationsFileNames = Directory.GetFiles(folderName, "*.bin");
            for (int i=0;i< animationsFileNames.Length;i++)
                this.Animations.Add(new Animation(animationsFileNames[i]));
        }

        public float Interpolation = 1f;

        public void GetNextFrame(ref Matrix4[] rememberMatrices, List<Joint> joints)
        {
            if (this.Animation == null)
                return;
            Animation animation = this.Animations[this.AnimationIndex];
            Matrix4[] animationData = animation.Data;

            int current_frame = (int)Math.Floor(this.AnimationFrame);
            int next_frame = (current_frame + 1) % (int)this.Animation.MaxFrame;

            float decimals = this.AnimationFrame - current_frame;
            bool decimal_ = decimals < 0.000001;
            float decimal_one = decimals;
            float decimal_one_minus = 1f - decimals;

            int current_position = current_frame * rememberMatrices.Length;
            int next_position = next_frame * rememberMatrices.Length;

            bool interpolate_ = this.Interpolation < 1f;
            float interpolate_one = this.Interpolation;
            float interpolate_one_minus = 1f - this.Interpolation;

            for (int i=0;i< rememberMatrices.Length;i++)
            {
                joints[i].Matrix = animationData[current_position++];
                if (interpolate_)
                {
                    Matrix4 matrix_a = rememberMatrices[i];
                    Matrix4 matrix_b = joints[i].Matrix;

                    Matrix4 matrix_a_b = (matrix_a * interpolate_one_minus + matrix_b * interpolate_one);

                    matrix_a_b = matrix_a_b.ClearScale();
                    //matrix_a_b *= Matrix4.CreateScale(matrix_a.ExtractScale() * interpolate_one + matrix_b.ExtractScale() * interpolate_one_minus);

                    joints[i].Matrix = matrix_a_b;
                }
                else
                {
                    if (!decimal_)
                    {
                        Matrix4 matrix_a = joints[i].Matrix;
                        Matrix4 matrix_b = animationData[next_position++];

                        Matrix4 matrix_a_b = (matrix_a * decimal_one_minus + matrix_b * decimal_one);

                        matrix_a_b = matrix_a_b.ClearScale();
                        //matrix_a_b *= Matrix4.CreateScale(matrix_a.ExtractScale() * decimal_one + matrix_b.ExtractScale() * decimal_one_minus);

                        joints[i].Matrix = matrix_a_b;
                    }
                    rememberMatrices[i] = joints[i].Matrix;
                }

            }

            if (interpolate_)
                this.Interpolation += 0.1f;

            this.AnimationFrame += 0.05f;
            if (this.AnimationFrame >= animation.MaxFrame)
            {
                this.AnimationFrame = animation.MinFrame;
            }
        }
    }
}
