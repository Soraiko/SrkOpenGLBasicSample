using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class MathHelper
    {
        public static float NormalizeAngle(float angle)
        {
            return (float)Math.Atan2(Math.Sin(angle), Math.Cos(angle));
        }
    }
}
