using System;
using OpenTK;
using System.Collections.Generic;

namespace SrkOpenGLBasicSample
{
    public class Joint
    {
        public string Name;

        public List<Joint> Children;

        public Joint(string name)
        {
            this.Children = new List<Joint>(0);
            this.Name = name;
        }

        public Vector3 Rotate;
        public Vector3 Translate;
        public Vector3 Scale;

        public bool Dirty;
        public Matrix4 ComputedMatrix;
        public Matrix4 Matrix;
        public Matrix4 DummyMatrix;

        Joint parent;
        public Joint Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (value != null)
                {
                    if (!value.Children.Contains(this))
                        value.Children.Add(this);
                }
                else if (this.parent != null)
                {
                    if (this.parent.Children.Contains(this))
                    this.parent.Children.Remove(this);
                }
                this.parent = value;
            }
        }


        public void CalculateMatricesFromAngles()
        {
            this.Matrix = 
            Matrix4.CreateScale(this.Scale) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitX, this.Rotate.X) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, this.Rotate.Y) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitZ, this.Rotate.Z) *
            Matrix4.CreateTranslation(this.Translate);
        }

        public void CalculateAnglesFromMatrices()
        {
            this.Scale = this.Matrix.ExtractScale();
            this.Translate = this.Matrix.ExtractTranslation();
            Matrix4 mq = Matrix4.CreateFromQuaternion(this.Matrix.ExtractRotation());
            double sy = Math.Sqrt(mq.M11 * mq.M11 + mq.M12 * mq.M12);
            bool singular = sy < 1e-6;
            if (!singular)
            {
                this.Rotate.X = DAE.GetSingle(Math.Atan2(mq.M23, mq.M33));
                this.Rotate.Y = DAE.GetSingle(Math.Atan2(-mq.M13, sy));
                this.Rotate.Z = DAE.GetSingle(Math.Atan2(mq.M12, mq.M11));
            }
            else
            {
                this.Rotate.X = DAE.GetSingle(Math.Atan2(-mq.M32, mq.M22));
                this.Rotate.Y = DAE.GetSingle(Math.Atan2(-mq.M13, sy));
                this.Rotate.Z = 0;
            }
        }

    }
}
