using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Input;

namespace SrkOpenGLBasicSample
{
    public class ModelController
    {
        public Model Controlled;
        public Moveset Moveset;
        public Matrix4[] rememberMatrices;

        public Joint bone_head;
        public Vector3 HeadPosition;
        public float WalkSpeed = 100f /* cm/s */ / 30.0f /* FPS */;
        public float RunSpeed = 180f /* cm/s */ / 30.0f /* FPS */;

        public ModelController(Model controlled)
        {
            this.Controlled = controlled;
            if (System.IO.Directory.Exists(controlled.Directory.Replace("debug_files","binary_files") + @"\Moveset"))
            {
                this.Moveset = new Moveset(controlled.Directory.Replace("debug_files", "binary_files") + @"\Moveset");
            }
            this.rememberMatrices = new Matrix4[controlled.Skeleton.Joints.Count];
            for (int i = 0; i < this.rememberMatrices.Length; i++)
                this.rememberMatrices[i] = controlled.Skeleton.Joints[i].Matrix;
        }

        public void RenderNextFrame()
        {
            this.Moveset.GetNextFrame(ref this.rememberMatrices, this.Controlled.Skeleton.Joints);
        }

        ChangingVector3 Position;
        public void ProceedCalculations()
        {
            var controlledSkeleton = this.Controlled.Skeleton;
            this.Position.Value = controlledSkeleton.Position;

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
