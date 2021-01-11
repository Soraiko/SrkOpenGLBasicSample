using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Input;

namespace SrkOpenGLBasicSample
{
    public class Controllable
    {
        public Skeleton Skeleton;
        public Joint bone_head;
        public Vector3 HeadPosition;
        public float WalkSpeed = 140f /* cm/s */ / 30.0f /* FPS */;
    }
}
