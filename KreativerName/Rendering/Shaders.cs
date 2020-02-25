using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KreativerName.Rendering
{
    public static class Shaders
    {

        static Dictionary<string, Shader> shaders;

        public static Shader Get(string shader)
        {
            return shaders.ContainsKey(shader) ? shaders[shader] : null;
        }

        public static void LoadShaders(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string[] directories = Directory.GetDirectories(directory);

            shaders = new Dictionary<string, Shader>();
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (files.Contains($"{directory}\\{name}.frag") && files.Contains($"{directory}\\{name}.vert") && !shaders.ContainsKey(name))
                    shaders.Add(name, LoadShader($"{directory}\\{name}"));
            }

            foreach (string dir in directories)
            {
                LoadShader(dir, directory);
            }
        }

        static void LoadShader(string subDir, string directory)
        {
            string[] files = Directory.GetFiles(subDir);
            string[] directories = Directory.GetDirectories(subDir);

            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                name = subDir.Replace(directory + "\\", "") + "\\" + name;

                if (files.Contains($"{name}.frag") && files.Contains($"{name}.vert"))
                    shaders.Add(name, LoadShader(file));
            }

            foreach (string dir in directories)
            {
                LoadShader(dir, directory);
            }
        }

        static Shader LoadShader(string path)
        {
            if (!File.Exists($"{path}.vert") || !File.Exists($"{path}.frag"))
                throw new FileNotFoundException("File not found at \"" + path + "\"");

            return new Shader($"{path}.vert", $"{path}.frag");
        }

        public static void Dispose()
        {
            foreach (KeyValuePair<string, Shader> shader in shaders)
            {
                shader.Value.Dispose();
            }
        }
    }
}
