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
        public float WalkSpeed = 100f /* cm/s */ / 30.0f /* FPS */;
        public float RunSpeed = 180f /* cm/s */ / 30.0f /* FPS */;


        ChangingVector3 Position;
        public void ProceedCalculations()
        {
            this.Position.Value = this.Skeleton.Position;

            if (this.Position.Initialized)
            {
                float distance = Vector3.Distance(this.Position.OldValue, this.Position.Value);
            }

            this.Position.OldValue = this.Position.Value;
            this.Position.Initialized = true;
        }
    }

    public struct ChangingVector3
    {
        public Vector3 Value;
        public Vector3 OldValue;
        public bool Initialized;
    }

    public struct ChangingSingle
    {
        public Single Value;
        public Single OldValue;
        public bool Initialized;
    }

    public struct ChangingInt32
    {
        public Int32 Value;
        public Int32 OldValue;
        public bool Initialized;
    }

    public struct ChangingBoolean
    {
        public bool Value;
        public bool OldValue;
        public bool Initialized;
    }
}
