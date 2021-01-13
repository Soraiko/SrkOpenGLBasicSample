using OpenTK;
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public class Camera
    {
        const float EPSILON = 0.000001f;
        public Camera() : this(350f)
        {

        }

        public Camera(float distance) : this(distance, 0.1f, 0.1f, OpenTK.MathHelper.DegreesToRadians(60f))
        {

        }

        public Camera(float distance, float rotation_step, float translation_step, float view_angle)
        {
            this.RotationMatrix = Matrix3.CreateScale(1f);
            this.rotationStep = rotation_step;
            this.translationStep = translation_step;
            this.viewAngle = view_angle;
            this.dest_viewAngle = view_angle;
            this.distance = distance;
            this.dest_distance = distance;
        }

        public static Camera Current;

        public Model Target;
        public Vector3 Position;

        Vector3 lookAt;
        Vector3 dest_lookAt;
        public Vector3 LookAt
        {
            get
            {
                return this.lookAt;
            }
            set
            {
                if (Vector3.Distance(this.dest_lookAt, value) < EPSILON) return;
                this.dest_lookAt = value;
                LookAtMatrixDirty = true;
            }
        }

        Vector3 rotation;
        Vector3 dest_rotation;

        public float RotationX { get { return this.rotation.X; } set { value = MathHelper.PrincipalAngle(value); if (this.Type > 0 && Math.Abs(value) > Math.PI * 0.499) return; this.rotation.X = GetCloserPrincipalAngle(value, this.rotation.X); dest_rotation.X = value; LookAtMatrixDirty = true; } }
        public float RotationY { get { return this.rotation.Y; } set { value = MathHelper.PrincipalAngle(value); this.rotation.Y = GetCloserPrincipalAngle(value, this.rotation.Y); dest_rotation.Y = value; LookAtMatrixDirty = true; } }
        public float RotationZ { get { return this.rotation.Z; } set { value = MathHelper.PrincipalAngle(value); this.rotation.Z = GetCloserPrincipalAngle(value, this.rotation.Z); dest_rotation.Z = value; LookAtMatrixDirty = true; } }

        public static float GetCloserPrincipalAngle(float from, float to)
        {
            while (to - from > Math.PI) to -= OpenTK.MathHelper.TwoPi;
            while (to - from < -Math.PI) to += OpenTK.MathHelper.TwoPi;
            return to;
        }

        public bool LookAtMatrixDirty = true;
        public bool ProjectionMatrixDirty = true;

        float rotationStep;
        public float RotationStep
        {
            get { return this.rotationStep; }
            set { this.rotationStep = OpenTK.MathHelper.Clamp(value, 0f, 1f); }
        }

        float translationStep;
        public float TranslationStep
        {
            get { return this.translationStep; }
            set { this.translationStep = OpenTK.MathHelper.Clamp(value, 0f, 1f); }
        }

        public Matrix4 LookAtMatrix;
        Matrix3 RotationMatrix;
        public Matrix4 ProjectionMatrix;

        float distance;
        float dest_distance;
        public float Distance
        {
            get
            {
                return this.distance;
            }
            set
            {
                if (Math.Abs(this.dest_distance - value) < EPSILON) return;
                if (value < 1) return;
                this.dest_distance = value;
                LookAtMatrixDirty = true;
            }
        }

        float viewAngle;
        float dest_viewAngle;

        public float ViewAngle
        {
            get
            {
                return this.viewAngle;
            }
            set
            {
                if (Math.Abs(this.dest_viewAngle - value) < EPSILON) return;
                this.dest_viewAngle = value;
                ProjectionMatrixDirty = true;
            }
        }

        public void MouseControl(MouseState mouseState, MouseState oldMouseState)
        {
            if (mouseState.Wheel > oldMouseState.Wheel)
            {
                this.Distance = this.dest_distance - 10f;
            }
            else if (mouseState.Wheel < oldMouseState.Wheel)
            {
                this.Distance = this.dest_distance + 10f;
            }
        }

        public void KeyboardControl(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (keyboardState.IsKeyDown(Key.Keypad4))
                this.RotationY = this.dest_rotation.Y + 0.1f;
            if (keyboardState.IsKeyDown(Key.Keypad6))
                this.RotationY = this.dest_rotation.Y - 0.1f;

            if (keyboardState.IsKeyDown(Key.Keypad8))
                this.RotationX = this.dest_rotation.X + 0.1f;
            if (keyboardState.IsKeyDown(Key.Keypad2))
                this.RotationX = this.dest_rotation.X - 0.1f;

            if (this.Type != CameraType.Grounded)
            {
                if (keyboardState.IsKeyDown(Key.Keypad1))
                    this.RotationZ = this.dest_rotation.Z + 0.1f;
                if (keyboardState.IsKeyDown(Key.Keypad9))
                    this.RotationZ = this.dest_rotation.Z - 0.1f;
            }
        }

        public void ControlTarget(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            var target = this.Target;
            if (target == null)
                return;

            var modelController = target.Controller;
            if (modelController == null)
                return;

            var skeleton = target.Skeleton;
            if (skeleton == null)
                return;

            Matrix3 rotation;
            if (this.Type == CameraType.Grounded)
            {
                rotation = Matrix3.CreateRotationY(this.RotationY);
            }
            else
            {
                rotation = this.RotationMatrix;
            }

            Vector3 locationBefore = skeleton.Position;

            if (keyboardState.IsKeyDown(Compatibility.FirstPerson_Left))
            {
                LookAtMatrixDirty = true;
                skeleton.Position += Vector3.Transform(-Vector3.UnitX * modelController.WalkSpeed, rotation);
            }

            if (keyboardState.IsKeyDown(Compatibility.FirstPerson_Right))
            {
                LookAtMatrixDirty = true;
                skeleton.Position += Vector3.Transform(Vector3.UnitX * modelController.WalkSpeed, rotation);
            }

            if (keyboardState.IsKeyDown(Compatibility.FirstPerson_Backward))
            {
                LookAtMatrixDirty = true;
                skeleton.Position += Vector3.Transform(Vector3.UnitZ * modelController.WalkSpeed, rotation);
            }

            if (keyboardState.IsKeyDown(Compatibility.FirstPerson_Forward))
            {
                LookAtMatrixDirty = true;
                skeleton.Position += Vector3.Transform(-Vector3.UnitZ * modelController.WalkSpeed, rotation);
            }

            Vector3 locationAfter = skeleton.Position;
            float distanceParcourue = Vector3.Distance(locationBefore, locationAfter);
            if (distanceParcourue>0)
            {
                Vector3 diff = (locationAfter - locationBefore) / distanceParcourue;
                float newAngle = (float)Math.Atan2(diff.X, diff.Z);
                skeleton.SetRotationY(newAngle);
            }



            if (this.LookAtMatrixDirty)
                this.dest_lookAt = skeleton.Position + modelController.HeadPosition;
        }


        public enum CameraType
        {
            ArcBall = 0,
            ArcBall_AbsoluteY = 1,
            Grounded = 2
        }
        public CameraType Type = CameraType.Grounded;

        public void Update(GameWindow window)
        {
            if (LookAtMatrixDirty)
            {
                Vector3 diff = Vector3.Zero;
                this.LookAtMatrixDirty = false;

                diff = (this.dest_rotation - this.rotation) * this.rotationStep;
                this.rotation += diff;

                ApplyRotation(diff);

                if (diff.Length > EPSILON)
                    this.LookAtMatrixDirty = true;

                diff = this.dest_lookAt - this.lookAt;
                this.lookAt += diff * this.translationStep;
                if (diff.Length > EPSILON)
                    this.LookAtMatrixDirty = true;

                diff.Z = this.dest_distance - this.distance;
                this.distance += diff.Z * this.translationStep;
                if (Math.Abs(diff.Z) > EPSILON)
                    this.LookAtMatrixDirty = true;


                this.Position = this.lookAt + Vector3.Transform(Vector3.UnitZ, this.RotationMatrix) * this.distance;
                this.LookAtMatrix = Matrix4.LookAt(this.Position, this.lookAt, Vector3.Transform(Vector3.UnitY, this.RotationMatrix));
            }
            if (ProjectionMatrixDirty)
            {
                this.ProjectionMatrixDirty = false;
                Vector3 diff = Vector3.Zero;

                diff.X = this.dest_viewAngle - this.viewAngle;
                this.viewAngle += diff.X * this.rotationStep;
                if (Math.Abs(diff.X) > EPSILON)
                    this.ProjectionMatrixDirty = true;

                this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(this.viewAngle, window.Width / (float)window.Height, 15f, 10000000f);
            }
        }

        public void ApplyRotation(Vector3 diff)
        {
            if (this.Type == CameraType.Grounded)
            {
                this.RotationMatrix = this.RotationMatrix * Matrix3.CreateRotationY(diff.Y);
            }
            else
            {
                this.RotationMatrix = Matrix3.CreateRotationZ(diff.Z) * this.RotationMatrix;
                if (this.Type == CameraType.ArcBall_AbsoluteY)
                    this.RotationMatrix = this.RotationMatrix * Matrix3.CreateRotationY(diff.Y);
                else
                    this.RotationMatrix = Matrix3.CreateRotationY(diff.Y) * this.RotationMatrix;

            }
            this.RotationMatrix = Matrix3.CreateRotationX(diff.X) * this.RotationMatrix;
        }

        public void SkipInterpolation()
        {
            this.distance = this.dest_distance * 1f;
            this.lookAt = this.dest_lookAt * 1f;
            this.rotation = this.dest_rotation * 1f;
            ApplyRotation(this.rotation);
            this.viewAngle = this.dest_viewAngle * 1f;
        }
    }
}