using OpenTK;

namespace SrkOpenGLBasicSample
{
    public class Joint
    {
        public int Parent;
        public string Name;

        public Matrix4 TransformLocal;
        public Matrix4 TransformModel;
        
        public Joint(string name, Matrix4  matrice)
        {
            Parent = -1;
            Name = name;
            
            TransformLocal = matrice * 1f;
            TransformModel = matrice * 1f;
        }
    }
}
