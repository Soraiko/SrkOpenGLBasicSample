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
        Vector3 dest_rotation;

        public void SetRotationY(float angle)
        {
            this.dest_rotation.Y = angle;
            float diff = dest_rotation.Y - rotation.Y;
            if (Math.Abs(diff)> Math.PI)
            {
                float diff_plus_TwoPi = dest_rotation.Y - (rotation.Y + OpenTK.MathHelper.TwoPi);
                if (Math.Abs(diff_plus_TwoPi) < Math.PI)
                    this.rotation.Y += OpenTK.MathHelper.TwoPi;
                else
                {
                    float diff_minus_TwoPi = dest_rotation.Y - (rotation.Y - OpenTK.MathHelper.TwoPi);
                    if (Math.Abs(diff_minus_TwoPi) < Math.PI)
                        this.rotation.Y -= OpenTK.MathHelper.TwoPi;
                }
            }
        }

        public void UpdateRotate()
        {
            rotation += (dest_rotation - rotation) / 10f;
        }

        public float RotationX { get { return MathHelper.PrincipalAngle(this.rotation.X); } }
        public float RotationY { get { return MathHelper.PrincipalAngle(this.rotation.Y); } }
        public float RotationZ { get { return MathHelper.PrincipalAngle(this.rotation.Z); } }

        public Vector3 Position;

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
            Matrix4.CreateFromAxisAngle(Vector3.UnitX, this.rotation.X) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, this.rotation.Y) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitZ, this.rotation.Z) *
            Matrix4.CreateTranslation(this.Position);

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

        Matrix4 Multiply(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = Matrix4.Identity;

            var left0 = left.M11;
            var left1 = left.M21;
            var left2 = left.M31;
            var left3 = left.M41;

            var left4 = left.M12;
            var left5 = left.M22;
            var left6 = left.M32;
            var left7 = left.M42;

            var left8 = left.M13;
            var left9 = left.M23;
            var left10 = left.M33;
            var left11 = left.M43;

            var left12 = left.M14;
            var left13 = left.M24;
            var left14 = left.M34;
            var left15 = left.M44;

            var right0 = right.M11;
            var right1 = right.M21;
            var right2 = right.M31;
            var right3 = right.M41;

            var right4 = right.M12;
            var right5 = right.M22;
            var right6 = right.M32;
            var right7 = right.M42;

            var right8 = right.M13;
            var right9 = right.M23;
            var right10 = right.M33;
            var right11 = right.M43;

            var right12 = right.M14;
            var right13 = right.M24;
            var right14 = right.M34;
            var right15 = right.M44;

            var column0Row0 = left0 * right0 + left4 * right1 + left8 * right2 + left12 * right3;
            var column0Row1 = left1 * right0 + left5 * right1 + left9 * right2 + left13 * right3;
            var column0Row2 = left2 * right0 + left6 * right1 + left10 * right2 + left14 * right3;
            var column0Row3 = left3 * right0 + left7 * right1 + left11 * right2 + left15 * right3;

            var column1Row0 = left0 * right4 + left4 * right5 + left8 * right6 + left12 * right7;
            var column1Row1 = left1 * right4 + left5 * right5 + left9 * right6 + left13 * right7;
            var column1Row2 = left2 * right4 + left6 * right5 + left10 * right6 + left14 * right7;
            var column1Row3 = left3 * right4 + left7 * right5 + left11 * right6 + left15 * right7;

            var column2Row0 = left0 * right8 + left4 * right9 + left8 * right10 + left12 * right11;
            var column2Row1 = left1 * right8 + left5 * right9 + left9 * right10 + left13 * right11;
            var column2Row2 = left2 * right8 + left6 * right9 + left10 * right10 + left14 * right11;
            var column2Row3 = left3 * right8 + left7 * right9 + left11 * right10 + left15 * right11;

            var column3Row0 = left0 * right12 + left4 * right13 + left8 * right14 + left12 * right15;
            var column3Row1 = left1 * right12 + left5 * right13 + left9 * right14 + left13 * right15;
            var column3Row2 = left2 * right12 + left6 * right13 + left10 * right14 + left14 * right15;
            var column3Row3 = left3 * right12 + left7 * right13 + left11 * right14 + left15 * right15;

            result.M11 = column0Row0;
            result.M21 = column0Row1;
            result.M31 = column0Row2;
            result.M41 = column0Row3;
            result.M12 = column1Row0;
            result.M22 = column1Row1;
            result.M32 = column1Row2;
            result.M42 = column1Row3;
            result.M13 = column2Row0;
            result.M23 = column2Row1;
            result.M33 = column2Row2;
            result.M43 = column2Row3;
            result.M14 = column3Row0;
            result.M24 = column3Row1;
            result.M34 = column3Row2;
            result.M44 = column3Row3;
            return result;
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
