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

        public static Shader VP;
        public static Shader VPC;
        public static Shader VPTC;
        public static Shader VPTN;
        public static Shader VPT;
        static Shader()
        {
            VP = new Shader(@"resources\graphics\vp_vert.c", @"resources\graphics\vp_frag.c");
            VPT = new Shader(@"resources\graphics\vpt_vert.c", @"resources\graphics\vpt_frag.c");
            VPC = new Shader(@"resources\graphics\vpc_vert.c", @"resources\graphics\vpc_frag.c");
            VPTC = new Shader(@"resources\graphics\vpct_vert.c", @"resources\graphics\vptc_frag.c");
            VPTN = new Shader(@"resources\graphics\vpnt_vert.c", @"resources\graphics\vptn_frag.c");
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


        public void Use()
        {
            GL.UseProgram(Handle);
            /*if (this.skipAlpha<0)
            this.skipAlpha = GL.GetUniformLocation(Handle, "skipAlpha");
            GL.Uniform1(this.skipAlpha, skipAlpha);*/
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
