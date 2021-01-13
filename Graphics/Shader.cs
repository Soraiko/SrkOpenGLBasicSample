using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK;
using OpenTK.Input;

namespace SrkOpenGLBasicSample
{
    public class Shader : IDisposable
    {
        public int Handle;
        int VertexShader;
        int FragmentShader;

        public static Shader P;
        public static Shader skP;

        public static Shader TP;
        public static Shader skTP;

        public static Shader CTP;
        public static Shader skCTP;

        public static Shader NTP;
        public static Shader skNTP;

        public static Shader CNTP;
        public static Shader skCNTP;
        static Shader()
        {
            P = new Shader(@"resources\graphics\p_vert.c", @"resources\graphics\p_frag.c");
            skP = new Shader(@"resources\graphics\sk_p_vert.c", @"resources\graphics\p_frag.c");

            TP = new Shader(@"resources\graphics\tp_vert.c", @"resources\graphics\tp_frag.c");
            skTP = new Shader(@"resources\graphics\sk_tp_vert.c", @"resources\graphics\tp_frag.c");

            CTP = new Shader(@"resources\graphics\ctp_vert.c", @"resources\graphics\ctp_frag.c");
            skCTP = new Shader(@"resources\graphics\sk_ctp_vert.c", @"resources\graphics\ctp_frag.c");

            NTP = new Shader(@"resources\graphics\ntp_vert.c", @"resources\graphics\ntp_frag.c");
            skNTP = new Shader(@"resources\graphics\sk_ntp_vert.c", @"resources\graphics\ntp_frag.c");

            CNTP = new Shader(@"resources\graphics\cntp_vert.c", @"resources\graphics\cntp_frag.c");
            skCNTP = new Shader(@"resources\graphics\sk_cntp_vert.c", @"resources\graphics\cntp_frag.c");
        }

        public Shader(string vertexPath, string fragmentPath)
        {
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, System.IO.File.ReadAllText(vertexPath,System.Text.Encoding.ASCII));

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, System.IO.File.ReadAllText(fragmentPath, System.Text.Encoding.ASCII));

            GL.CompileShader(VertexShader);
            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);
            GL.UseProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        int light0_position = -1;
        int light0_color = -1;
        int light0_ambiant_strength = -1;
        int bump_mapping = -1;
        bool locationsFound = false;
        public void Use()
        {
            GL.UseProgram(this.Handle);

            if (locationsFound)
            {
                GL.Uniform3(light0_position, StaticReferences.Light0_Position);
                GL.Uniform3(light0_color, StaticReferences.Light0_Color);
                GL.Uniform1(light0_ambiant_strength, StaticReferences.Light0_AmbientStrentgh);
                GL.Uniform1(bump_mapping, 1);
            }
            else
            {
                light0_position = GL.GetUniformLocation(this.Handle, "light0_position");
                light0_color = GL.GetUniformLocation(this.Handle, "light0_color");
                light0_ambiant_strength = GL.GetUniformLocation(this.Handle, "light0_ambiant_strength");
                bump_mapping = GL.GetUniformLocation(this.Handle, "bump_mapping");
                if (light0_position>-1 && light0_color>-1 && light0_ambiant_strength>-1 && bump_mapping > -1)
                    locationsFound = true;
            }

        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
