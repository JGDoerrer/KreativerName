using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    public static class Textures
    {
        static Dictionary<string, Texture2D> textures;

        public static Texture2D Get(string texture)
        {
            return textures[texture];
        }

        public static void LoadTextures(string directory)
        {
            string[] files = Directory.GetFiles(directory);

            textures = new Dictionary<string, Texture2D>();
            foreach (string file in files)
            {
                textures.Add(Path.GetFileNameWithoutExtension(file), LoadTexture(file));
            }
        }
               
        static Texture2D LoadTexture(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found at \"" + path + "\"");

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bitmap = new Bitmap(path);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
               ImageLockMode.ReadOnly,
               System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
               OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return new Texture2D(id, bitmap.Width, bitmap.Height);
        }
    }
}
