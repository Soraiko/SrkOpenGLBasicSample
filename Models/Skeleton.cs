using OpenTK;

namespace SrkOpenGLBasicSample
{
    public class Skeleton
    {
        public Joint[] Joints;
        public Matrix4[] Matrices;

        public Skeleton(int joints_count)
        {
            this.Joints = new Joint[joints_count];
            this.Matrices = new Matrix4[joints_count];
        }

        public Skeleton Clone()
        {
            Skeleton output = new Skeleton(this.Joints.Length);
            for (int i = 0; i < Joints.Length; i++)
            {
                output.Joints[i] = new Joint(this.Joints[i].Name, this.Joints[i].TransformLocal);
                output.Joints[i].Parent = this.Joints[i].Parent;
            }
            return output;
        }

        public void ComputeMatrices(Matrix4 globalTransform)
        {
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].TransformModel = Joints[i].TransformLocal * 1f;

                if (Joints[i].Parent > -1)
                    Joints[i].TransformModel *= Joints[Joints[i].Parent].TransformModel;

                Matrices[i] = Joints[i].TransformModel * globalTransform;
            }
        }

        public void DeComputeMatrices()
        {
            for (int i = Joints.Length-1; i > 0; i--)
            {
                if (Joints[i].Parent > -1)
                    Joints[i].TransformModel *= Matrix4.Invert(Joints[Joints[i].Parent].TransformModel);

                Joints[i].TransformLocal = Joints[i].TransformModel * 1f;
            }
        }
    }
}
