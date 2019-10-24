using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Grid;

namespace KreativerName
{
    public struct Level : IBytes
    {
        public HexGrid<Hex> grid;

        public void SaveToFile(string name)
        {
            byte[] bytes = Encode(ToBytes());
            File.WriteAllBytes($@"Resources\Levels\{name}.lvl", bytes);
        }

        public static Level LoadFromFile(string name)
        {
            Level level = new Level();
            byte[] bytes = Decode(File.ReadAllBytes($@"Resources\Levels\{name}.lvl"));
            level.FromBytes(bytes, 0);

            return level;
        }
               

        static byte[] Encode(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        static byte[] Decode(byte[] data)
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
            return grid.ToBytes();
        }

        public void FromBytes(byte[] bytes, int startIndex)
        {
            grid = new HexGrid<Hex>();
            grid.FromBytes(bytes, startIndex);
        }
    }
}
