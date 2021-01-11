using System;
using OpenTK;
using System.Collections.Generic;

namespace SrkOpenGLBasicSample
{
    public class Skeleton
    {
        public float[] MatricesBuffer;
        public List<Joint> Joints;
        Matrix4 TransformMatrix;

        Vector3 rotation;

        Vector3 direction = Vector3.UnitX;
        Vector3 dest_direction;

        public void SetDirection(Vector3 direction)
        {
            this.dest_direction = direction;
        }

        public void UpdateRotate()
        {

        }

        public float RotationX { get { return MathHelper.PrincipalAngle(this.rotation.X); } }
        public float RotationY { get { return MathHelper.PrincipalAngle(this.rotation.Y); } }
        public float RotationZ { get { return MathHelper.PrincipalAngle(this.rotation.Z); } }

        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }
        }
        public Vector3 Location;

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

        public void Compile(Controllable controllable)
        {
            for (int i = 0; i < this.Joints.Count; i++)
            {
                this.Joints[i].DummyMatrix = this.Joints[i].Matrix * 1f;
            }
            
            this.ComputeMatrices();
            for (int i = 0; i < this.Joints.Count; i++)
            {
                switch (this.Joints[i].Name)
                {
                    case "bone_head":
                        controllable.bone_head = this.Joints[i];
                        controllable.HeadPosition = this.Joints[i].ComputedMatrix.ExtractTranslation();
                    break;
                }
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
            this.TransformMatrix = 
            Matrix4.CreateFromAxisAngle(Vector3.UnitX, this.Rotation.X) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, this.Rotation.Y) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitZ, this.Rotation.Z) *
            Matrix4.CreateTranslation(this.Location);

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

        public Skeleton Clone()
        {
            Skeleton skeleton = new Skeleton();
            for (int i = 0; i < this.Joints.Count; i++)
            {
                Joint joint = new Joint(this.Joints[i].Name);
                joint.Matrix = this.Joints[i].DummyMatrix * 1f;
                joint.Scale = this.Joints[i].Scale * 1f;
                joint.Rotate = this.Joints[i].Rotate * 1f;
                joint.Translate = this.Joints[i].Translate * 1f;
                skeleton.Joints.Add(joint);
            }
            for (int i = 0; i < this.Joints.Count; i++)
            {
                if (this.Joints[i].Parent != null)
                {
                    for (int j = 0; j < this.Joints.Count; j++)
                    {
                        if (this.Joints[j].Name == this.Joints[i].Parent.Name)
                        {
                            skeleton.Joints[i].Parent = skeleton.Joints[j];
                            break;
                        }
                    }
                }
            }
            return skeleton;
        }
    }
}
