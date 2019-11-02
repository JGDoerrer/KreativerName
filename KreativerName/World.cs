using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public struct World : IBytes
    {
        public List<Level> levels;

        public void SaveToFile(string name)
        {
            byte[] bytes = Compress(ToBytes());
            Console.WriteLine($"World {name}: {bytes.Length}/{ToBytes().Length} ");
            File.WriteAllBytes($@"Resources\Worlds\{name}.wld", bytes);
        }

        public static World LoadFromFile(string name)
        {
            World world = new World();
            string path = $@"Resources\Worlds\{name}.wld";


            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                bytes = Decompress(bytes);
                world.FromBytes(bytes, 0);
            }

            return world;
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
    }
}
