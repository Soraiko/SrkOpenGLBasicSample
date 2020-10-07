using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Runtime.InteropServices;

namespace SrkOpenGLBasicSample
{
    public static class Texturing
    {

        static int textureID = -1;
        public static List<int> textureIntegers = new List<int>(0);
        public static List<string> textureFilenames = new List<string>(0);

        public static int LoadTexture(string filename, TextureMinFilter textureMinFilter, TextureWrapMode textureWrapMode)
        {
            int indexOf = textureFilenames.IndexOf(filename);
            if (indexOf > -1)
                return textureIntegers[indexOf];
            if (!File.Exists(filename))
                return 0;

            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(filename);

            int depth = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat);
            if (depth != 32)
            {
                bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            textureID++;
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            PixelInternalFormat format = PixelInternalFormat.Rgba;

            GL.TexImage2D(TextureTarget.Texture2D, 0, format, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bitmap.UnlockBits(data);
            bitmap.Dispose();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMinFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)textureWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)textureWrapMode);

            textureIntegers.Add(textureID);
            textureFilenames.Add(filename);

            return textureID;
        }
    }
}
