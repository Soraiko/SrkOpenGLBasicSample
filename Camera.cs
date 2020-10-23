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
        const float EPSILON = 0.001f;
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

        public float RotationX { get { return MathHelper.PrincipalAngle(rotation.X); } set { if (Grounded && Math.Abs(value) > Math.PI * 0.499) return; dest_rotation.X = value; LookAtMatrixDirty = true; } }
        public float RotationY { get { return MathHelper.PrincipalAngle(rotation.Y); } set { dest_rotation.Y = value; LookAtMatrixDirty = true; } }
        public float RotationZ { get { return MathHelper.PrincipalAngle(rotation.Z); } set { dest_rotation.Z = value; LookAtMatrixDirty = true; } }

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

            if (!Grounded && keyboardState.IsKeyDown(Key.Keypad1))
                this.RotationZ = this.dest_rotation.Z + 0.1f;
            if (!Grounded && keyboardState.IsKeyDown(Key.Keypad9))
                this.RotationZ = this.dest_rotation.Z - 0.1f;
        }
        public bool Grounded = true;
        public void Update(GameWindow window)
        {
            if (LookAtMatrixDirty)
            {
                Vector3 diff = Vector3.Zero;
                this.LookAtMatrixDirty = false;

                diff = (this.dest_rotation - this.rotation) * this.rotationStep;
                this.rotation += diff;

                if (diff.Length > EPSILON)
                    this.LookAtMatrixDirty = true;

                ApplyRotation(diff);

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

                this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(this.viewAngle, window.Width / (float)window.Height, 30f, 100000000f);
            }
        }

        public void ApplyRotation(Vector3 diff)
        {
            if (Grounded)
            {
                this.RotationMatrix = this.RotationMatrix * Matrix3.CreateRotationY(diff.Y);
            }
            else
            {
                this.RotationMatrix = Matrix3.CreateRotationZ(diff.Z) * this.RotationMatrix;
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