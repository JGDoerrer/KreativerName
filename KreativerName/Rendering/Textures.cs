using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    public static class Textures
    {
        static Dictionary<string, Texture2D> textures;

        public static Texture2D Get(string texture)
        {
            return textures.ContainsKey(texture) ? textures[texture] : new Texture2D();
        }

        public static void LoadTextures(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string[] directories = Directory.GetDirectories(directory);

            textures = new Dictionary<string, Texture2D>();
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                textures.Add(name, LoadTexture(file));
            }

            foreach (string dir in directories)
            {
                LoadTextures(dir, directory);
            }
        }

        static void LoadTextures(string subDir, string directory)
        {
            string[] files = Directory.GetFiles(subDir);
            string[] directories = Directory.GetDirectories(subDir);

            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                name = subDir.Replace(directory + "\\", "") + "\\" + name;

                textures.Add(name, LoadTexture(file));
            }

            foreach (string dir in directories)
            {
                LoadTextures(dir, directory);
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
