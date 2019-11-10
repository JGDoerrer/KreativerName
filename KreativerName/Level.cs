using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using KreativerName.Grid;

namespace KreativerName
{
    public struct Level : IBytes
    {
        public HexGrid<Hex> grid;
        public HexPoint startPos;
        public int minMoves;

        public void Update()
        {
            for (int i = 0; i < grid.Count; i++)
            {
                Hex hex = grid.Values.ElementAt(i);

                if (hex.Type == HexType.DeadlyTwoStateOn)
                    hex.Type = HexType.DeadlyTwoStateOff;
                else if (hex.Type == HexType.DeadlyTwoStateOff)
                    hex.Type = HexType.DeadlyTwoStateOn;

                grid[hex.Position] = hex;
            }
        }

        #region Load & Save

        public void SaveToFile(string name)
        {
            byte[] bytes = Compress(ToBytes());
            Console.WriteLine($"Level {name}: {bytes.Length}/{ToBytes().Length} ");
            File.WriteAllBytes($@"Resources\Levels\{name}.lvl", bytes);
        }

        public static Level LoadFromFile(string name)
        {
            Level level = new Level();
            string path = $@"Resources\Levels\{name}.lvl";

            if (File.Exists(path))
            {
                byte[] bytes = Decompress(File.ReadAllBytes(path));
                level.FromBytes(bytes, 0);
            }

            return level;
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
            bytes.AddRange(grid.ToBytes());
            bytes.AddRange(startPos.ToBytes());
            bytes.AddRange(minMoves.ToBytes());

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            grid = new HexGrid<Hex>();
            count += grid.FromBytes(bytes, startIndex + count);
            startPos = new HexPoint();
            count += startPos.FromBytes(bytes, startIndex + count);
            minMoves = BitConverter.ToInt32(bytes, startIndex + count);
            count += 4;

            return count;
        }

        #endregion
    }
}
