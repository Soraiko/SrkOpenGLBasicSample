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

        public static bool CountAlphaPixels = false;
        public static ulong AlphaPercentage = 0;

        public static Texture LoadTexture(string filename, System.Drawing.Bitmap bitmap, TextureMinFilter textureMinFilter, TextureWrapMode textureWrapS, TextureWrapMode textureWrapT)
        {
            Texture output = new Texture();
            if (!filename.Contains("::") && !File.Exists(filename))
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

            System.Drawing.Bitmap bmp = bitmap == null ? (System.Drawing.Bitmap)System.Drawing.Image.FromFile(filename) : bitmap;

            int depth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
            if (depth != 32)
            {
                bmp = bmp.Clone(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            textureID++;
            output.Integer = textureID;
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);


            PixelInternalFormat format = PixelInternalFormat.Rgba;


            if (CountAlphaPixels)
            {
                byte[] pixelBytes = new byte[bmp.Width * bmp.Height * 4];
                Marshal.Copy(data.Scan0, pixelBytes, 0, pixelBytes.Length);
                AlphaPercentage = 0;
                for (int i=0;i< pixelBytes.Length;i+=4)
                    AlphaPercentage += pixelBytes[i + 3];
                AlphaPercentage /= (ulong)(bmp.Width * bmp.Height);
            }
            GL.TexImage2D(TextureTarget.Texture2D, 0, format, bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bmp.UnlockBits(data);
            bmp.Dispose();

            output.TextureMinFilter = (int)textureMinFilter;
            output.TextureWrapS = (int)textureWrapS;
            output.TextureWrapT = (int)textureWrapT;

            Textures.Add(output);
            return output;
        }
    }
}
