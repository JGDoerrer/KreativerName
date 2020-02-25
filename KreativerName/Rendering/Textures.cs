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
            return textures.ContainsKey(texture) ? textures[texture] : null;
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

            return new Texture2D(path);
        }


        public static void Dispose()
        {
            foreach (KeyValuePair<string, Texture2D> tex in textures)
            {
                tex.Value.Dispose();
            }
        }
    }
}
