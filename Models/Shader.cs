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
