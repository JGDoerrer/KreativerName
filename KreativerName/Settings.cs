using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    struct Settings : IBytes
    {
        public bool ShowMoves;
        public bool Fullscreen;

        public static Settings Current;
        public static Settings New => new Settings()
        {
            ShowMoves = true,
            Fullscreen = false,
        };


        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ShowMoves = (bytes[startIndex + count] & (1 << 0)) > 0;
            Fullscreen = (bytes[startIndex + count] & (1 << 1)) > 0;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            byte b1 = (byte)((ShowMoves ? 1 : 0) << 0 | (Fullscreen ? 1 : 0) << 1);
            bytes.Add(b1);

            return bytes.ToArray();
        }
               
        public void SaveToFile(string name)
        {
            string path = $@"Resources\{name}.set";

            byte[] bytes = ToBytes();
            File.WriteAllBytes(path, bytes);
        }

        public static Settings LoadFromFile(string name)
        {
            Settings settings = New;
            string path = $@"Resources\{name}.set";

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                settings.FromBytes(bytes, 0);
            }

            return settings;
        }
    }
}
