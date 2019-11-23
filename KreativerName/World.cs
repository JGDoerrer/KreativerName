using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace KreativerName
{
    public struct World : IBytes
    {
        public List<Level> levels;

        public bool AllCompleted
        {
            get
            {
                return levels.All(x => x.completed);
            }
            set
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    Level level = levels[i];
                    level.completed = value;
                    levels[i] = level;
                }
            }
        }
        public bool AllPerfect
        {
            get
            {
                return levels.All(x => x.perfect);
            }
            set
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    Level level = levels[i];
                    level.perfect = value;
                    levels[i] = level;
                }
            }
        }

        #region Load & Save

        public void SaveToFile(string name)
        {
            byte[] bytes = Compress(ToBytes());
            //Console.WriteLine($"World {name}: {bytes.Length}/{ToBytes().Length} {100f*bytes.Length/ToBytes().Length:N1}%");
            File.WriteAllBytes($@"{BasePath}{name}.wld", bytes);
        }

        public void SaveToFile(string path, bool useBasePath)
        {
            byte[] bytes = Compress(ToBytes());
            //Console.WriteLine($"World {name}: {bytes.Length}/{ToBytes().Length} {100f*bytes.Length/ToBytes().Length:N1}%");
            File.WriteAllBytes($@"{(useBasePath ? BasePath : "")}{path}", bytes);
        }

        public static string BasePath => @"Resources\Worlds\";

        public static World LoadFromFile(string name)
        {
            World world = new World();
            string path = $@"{BasePath}{name}.wld";

            if (IsValidFile(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                bytes = Decompress(bytes);
                world.FromBytes(bytes, 0);
            }

            return world;
        }

        public static World LoadFromFile(string path, bool useBasePath)
        {
            World world = new World();
            path = $"{(useBasePath ? BasePath : "")}{path}";

            if (IsValidFile(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                bytes = Decompress(bytes);
                world.FromBytes(bytes, 0);
            }

            return world;
        }

        public static bool IsValidFile(string path)
        {
            return File.Exists(path) && Path.GetExtension(path) == ".wld";
        }

        static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }


        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(levels.Count));

            for (int i = 0; i < levels.Count; i++)
            {
                bytes.AddRange(levels[i].ToBytes());
            }

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            levels = new List<Level>();

            int total = 0;

            int count = BitConverter.ToInt32(bytes, startIndex);
            startIndex += 4;
            total += 4;

            for (int i = 0; i < count; i++)
            {
                Level level = new Level();
                int length = level.FromBytes(bytes, startIndex);
                levels.Add(level);

                startIndex += length;
                total += length;
            }

            return total;
        }

        #endregion

        public override string ToString() => $"Levels: {levels.Count}, Completed: {AllCompleted}, Perfect: {AllPerfect}";
    }
}
