using System;
using OpenTK;
using System.Collections.Generic;

namespace SrkOpenGLBasicSample
{
    public class Skeleton
    {
        public float[] MatricesBuffer;

        public List<Joint> Joints;
        public Matrix4 TransformMatrix;
        public Skeleton()
        {
            this.TransformMatrix = Matrix4.CreateScale(1f);
            this.Joints = new List<Joint>(0);
            this.MatricesBuffer = new float[1024 * 16];
        }

        public Joint GetJoint(string name)
        {
            for (int i = 0; i < this.Joints.Count; i++)
            {
                if (this.Joints[i].Name == name)
                    return this.Joints[i];
            }
            return null;
        }

        public int IndexOf(string name)
        {
            for (int i = 0; i < this.Joints.Count; i++)
            {
                if (this.Joints[i].Name == name)
                    return i;
            }
            return -1;
        }

        public void Compile()
        {
            this.ComputeMatrices();
            for (int i = 0; i < this.Joints.Count; i++)
            {
                Matrix4 mat = Matrix4.Invert(this.Joints[i].ComputedMatrix);
                int pos = 512 * 16 + i * 16;
                this.MatricesBuffer[pos++] = mat.M11;
                this.MatricesBuffer[pos++] = mat.M21;
                this.MatricesBuffer[pos++] = mat.M31;
                this.MatricesBuffer[pos++] = mat.M41;

                this.MatricesBuffer[pos++] = mat.M12;
                this.MatricesBuffer[pos++] = mat.M22;
                this.MatricesBuffer[pos++] = mat.M32;
                this.MatricesBuffer[pos++] = mat.M42;

                this.MatricesBuffer[pos++] = mat.M13;
                this.MatricesBuffer[pos++] = mat.M23;
                this.MatricesBuffer[pos++] = mat.M33;
                this.MatricesBuffer[pos++] = mat.M43;

                this.MatricesBuffer[pos++] = mat.M14;
                this.MatricesBuffer[pos++] = mat.M24;
                this.MatricesBuffer[pos++] = mat.M34;
                this.MatricesBuffer[pos++] = mat.M44;
            }
        }

        public void ComputeMatrices()
        {
            for (int i = 0; i < this.Joints.Count; i++)
            {
                this.Joints[i].ComputedMatrix = this.Joints[i].Matrix * 1f;
                this.Joints[i].Dirty = true;
                if (this.Joints[i].Parent == null)
                    this.Joints[i].ComputedMatrix *= this.TransformMatrix;
            }
            int dirtyCount;
            do
            {
                dirtyCount = this.Joints.Count;
                for (int i = 0; i < this.Joints.Count; i++)
                {
                    if (this.Joints[i].Parent == null || !this.Joints[i].Parent.Dirty)
                    {
                        if (this.Joints[i].Parent != null)
                            this.Joints[i].ComputedMatrix *= this.Joints[i].Parent.ComputedMatrix;

                        Matrix4 mat = this.Joints[i].ComputedMatrix;
                        int pos = i * 16;
                        this.MatricesBuffer[pos++] = mat.M11;
                        this.MatricesBuffer[pos++] = mat.M21;
                        this.MatricesBuffer[pos++] = mat.M31;
                        this.MatricesBuffer[pos++] = mat.M41;

                        this.MatricesBuffer[pos++] = mat.M12;
                        this.MatricesBuffer[pos++] = mat.M22;
                        this.MatricesBuffer[pos++] = mat.M32;
                        this.MatricesBuffer[pos++] = mat.M42;

                        this.MatricesBuffer[pos++] = mat.M13;
                        this.MatricesBuffer[pos++] = mat.M23;
                        this.MatricesBuffer[pos++] = mat.M33;
                        this.MatricesBuffer[pos++] = mat.M43;

                        this.MatricesBuffer[pos++] = mat.M14;
                        this.MatricesBuffer[pos++] = mat.M24;
                        this.MatricesBuffer[pos++] = mat.M34;
                        this.MatricesBuffer[pos++] = mat.M44;

                        this.Joints[i].Dirty = false;
                        dirtyCount--;
                    }
                }
            }
            while (dirtyCount > 0);
        }

        public void ReverseComputedMatrices()
        {
            for (int i = 0; i < this.Joints.Count; i++)
            {
                this.Joints[i].Matrix = this.Joints[i].ComputedMatrix * 1f;
                this.Joints[i].Dirty = true;
            }
            int dirtyCount;
            do
            {
                dirtyCount = this.Joints.Count;
                for (int i = 0; i < this.Joints.Count; i++)
                {
                    if (this.Joints[i].Dirty)
                    {
                        int childrenDirtyCount = 0;
                        for (int j = 0; j < this.Joints[i].Children.Count; j++)
                        {
                            if (this.Joints[i].Children[j].Dirty)
                                childrenDirtyCount++;
                        }
                        if (childrenDirtyCount == 0) /* Children's children are all calculated already. */
                        {
                            if (this.Joints[i].Parent != null)
                                this.Joints[i].Matrix *= Matrix4.Invert(this.Joints[i].Parent.ComputedMatrix);
                            this.Joints[i].Dirty = false;
                            dirtyCount--;
                        }
                    }
                    else
                        dirtyCount--;
                }
            }
            while (dirtyCount > 0);

            for (int i = 0; i < this.Joints.Count; i++)
                this.Joints[i].CalculateAnglesFromMatrices();
        }

    }
}
