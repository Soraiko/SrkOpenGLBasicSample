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
        public Camera() : this(350f)
        {

        }

        public Camera(float distance) : this(distance, 0.1f,0.1f, OpenTK.MathHelper.DegreesToRadians(60f))
        {
            
        }

        public Camera(float distance, float rotation_step, float translation_step, float view_angle)
        {
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
                this.dest_lookAt = value;
            }
        }

        float rotation_x;
        float dest_rotation_x;
        float rotation_y;
        float dest_rotation_y;
        float rotation_z;
        float dest_rotation_z;

        public float RotationX { get { return MathHelper.PrincipalAngle(rotation_x); } set { dest_rotation_x = value; LookAtMatrixDirty = true; } }
        public float RotationY { get { return MathHelper.PrincipalAngle(rotation_y); } set { dest_rotation_y = value; LookAtMatrixDirty = true; } }
        public float RotationZ { get { return MathHelper.PrincipalAngle(rotation_z); } set { dest_rotation_z = value; LookAtMatrixDirty = true; } }

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
            get { return this.distance; }
            set { this.dest_distance = value; }
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
                this.dest_viewAngle = value;
            }
        }

        public void MouseControl(MouseState mouseState, MouseState oldMouseState)
        {
            if (mouseState.Wheel > oldMouseState.Wheel)
            {
                this.Distance = this.dest_distance + 10f;
            }
            else if (mouseState.Wheel < oldMouseState.Wheel)
            {
                this.Distance = this.dest_distance - 10f;
            }
        }

        public void KeyboardControl(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (keyboardState.IsKeyDown(Key.Keypad4))
                this.RotationY = this.dest_rotation_y + 0.1f;
            if (keyboardState.IsKeyDown(Key.Keypad6))
                this.RotationY = this.dest_rotation_y - 0.1f;

            if (keyboardState.IsKeyDown(Key.Keypad8))
                this.RotationX = this.dest_rotation_x + 0.1f;
            if (keyboardState.IsKeyDown(Key.Keypad2))
                this.RotationX = this.dest_rotation_x - 0.1f;

            if (keyboardState.IsKeyDown(Key.Keypad1))
                this.RotationZ = this.dest_rotation_z + 0.1f;
            if (keyboardState.IsKeyDown(Key.Keypad9))
                this.RotationZ = this.dest_rotation_z - 0.1f;
        }

        public void Update(GameWindow window)
        {
            if (LookAtMatrixDirty)
            {
                Vector3 diff = Vector3.Zero;

                this.LookAtMatrixDirty = false;

                diff.X = this.dest_rotation_x - this.rotation_x;
                if (Math.Cos(this.rotation_x) < 0 ^ Math.Cos(this.rotation_x + diff.X * this.rotationStep) < 0)
                {
                    this.dest_rotation_z = OpenTK.MathHelper.Pi - this.dest_rotation_z;
                    this.rotation_z = this.dest_rotation_z;
                }
                this.rotation_x += diff.X * this.rotationStep;
                if (Math.Abs(diff.X) > 0.000001)
                    this.LookAtMatrixDirty = true;

                diff.Y = this.dest_rotation_y - this.rotation_y;
                this.rotation_y += diff.Y * this.rotationStep;
                if (Math.Abs(diff.Y) > 0.000001)
                    this.LookAtMatrixDirty = true;

                diff.Z = this.dest_rotation_z - this.rotation_z;
                this.rotation_z += diff.Z * this.rotationStep;
                if (Math.Abs(diff.Z) > 0.000001)
                    this.LookAtMatrixDirty = true;

                diff = this.dest_lookAt - this.lookAt;
                this.lookAt += diff * this.translationStep;
                if (diff.Length > 0.000001)
                    this.LookAtMatrixDirty = true;

                diff.Z = this.dest_distance - this.distance;
                this.distance += diff.Z * this.translationStep;
                if (Math.Abs(diff.Z) > 0.000001)
                    this.LookAtMatrixDirty = true;


                this.RotationMatrix =
                    Matrix3.CreateRotationX(this.rotation_x) *
                    Matrix3.CreateRotationY(this.rotation_y);

                this.Position = this.lookAt + Vector3.Transform(Vector3.UnitZ, this.RotationMatrix) * this.distance;

                this.LookAtMatrix = Matrix4.LookAt(
                    this.Position,
                    this.lookAt,
                    Vector3.Transform(Vector3.UnitY, Matrix3.CreateRotationZ(this.rotation_z)));
            }
            if (ProjectionMatrixDirty)
            {
                this.ProjectionMatrixDirty = false;
                Vector3 diff = Vector3.Zero;

                diff.X = this.dest_viewAngle - this.viewAngle;
                this.viewAngle += diff.X * this.rotationStep;
                if (Math.Abs(diff.X) > 0.000001)
                    this.ProjectionMatrixDirty = true;

                this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(this.viewAngle, window.Width/ (float)window.Height, 50f,100000f);
            }
        }
        public void SkipTransitions()
        {
            this.distance = this.dest_distance * 1f;
            this.lookAt = this.dest_lookAt * 1f;
            this.rotation_x = this.dest_rotation_x * 1f;
            this.rotation_y = this.dest_rotation_y * 1f;
            this.rotation_z = this.dest_rotation_z * 1f;
            this.viewAngle = this.dest_viewAngle * 1f;
        }
    }
}
