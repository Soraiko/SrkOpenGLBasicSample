using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace SrkOpenGLBasicSample
{
    public struct Texture
    {
        static int textureID = -1;
        static List<Texture> Textures;

        static Texture()
        {
            Textures = new List<Texture>(0);
        }

        public int TextureMinFilter;
        public int TextureWrapS;
        public int TextureWrapT;

        public int Integer;
        public string Filename;
        public static Texture LoadTexture(string filename, TextureMinFilter textureMinFilter, TextureWrapMode textureWrapS, TextureWrapMode textureWrapT)
        {
            Texture output = new Texture();
            if (!File.Exists(filename))
                return output;
            for (int i=0;i< Textures.Count;i++)
            {
                if (Textures[i].Filename.Length == filename.Length &&
                    Textures[i].Filename == filename)
                {
                    return Textures[i];
                }
            }

            output = new Texture();
            output.Filename = filename;

            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(filename);

            int depth = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat);
            if (depth != 32)
            {
                bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            textureID++;
            output.Integer = textureID;
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);


            PixelInternalFormat format = PixelInternalFormat.Rgba;

            GL.TexImage2D(TextureTarget.Texture2D, 0, format, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bitmap.UnlockBits(data);
            bitmap.Dispose();

            output.TextureMinFilter = (int)textureMinFilter;
            output.TextureWrapS = (int)textureWrapS;
            output.TextureWrapT = (int)textureWrapT;

            Textures.Add(output);
            return output;
        }
    }
}
