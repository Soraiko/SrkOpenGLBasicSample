using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class MathHelper
    {
        public static float PrincipalAngle(float angle)
        {
            return (float)Math.Atan2(Math.Sin(angle), Math.Cos(angle));
        }
        public static void GetEulerAngles(OpenTK.Matrix3 matrix, out float rX,out float rY, out float rZ)
        {
            float Singularity = 0.499f;
            OpenTK.Quaternion q = matrix.ExtractRotation();

            float ww = q.W * q.W;
            float xx = q.X * q.X;
            float yy = q.Y * q.Y;
            float zz = q.Z * q.Z;
            float lengthSqd = xx + yy + zz + ww;


            double sy = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M21 * matrix.M21);

            bool singular = sy < Single.Epsilon;

            if (!singular)
            {
                rX = (float)Math.Atan2(matrix.M32, matrix.M33);
                rY = (float)Math.Atan2(-matrix.M31, sy);
                rZ = (float)Math.Atan2(matrix.M21, matrix.M11);
            }
            else
            {
                rX = (float)Math.Atan2(-matrix.M23, matrix.M22);
                rY = (float)Math.Atan2(-matrix.M31, sy);
                rZ = (float)0;
            }



            
        }
    }
}
