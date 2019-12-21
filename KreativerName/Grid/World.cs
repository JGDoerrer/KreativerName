using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KreativerName.Grid
{
    public struct World : IBytes
    {
        public List<Level> Levels;
        public string Title;
        public uint ID;
        public uint Uploader;
        public DateTime UploadTime;

        public bool AllCompleted
        {
            get
            {
                return Levels.All(x => x.completed);
            }
            set
            {
                for (int i = 0; i < Levels.Count; i++)
                {
                    Level level = Levels[i];
                    level.completed = value;
                    Levels[i] = level;
                }
            }
        }
        public bool AllPerfect
        {
            get
            {
                return Levels.All(x => x.perfect);
            }
            set
            {
                for (int i = 0; i < Levels.Count; i++)
                {
                    Level level = Levels[i];
                    level.perfect = value;
                    Levels[i] = level;
                }
            }
        }

        #region Load & Save

        public static string BasePath => @"Resources\Worlds\";

        public void SaveToFile(string name)
            => Save($@"{BasePath}{name}.wld");

        public void SaveToFile(string path, bool useBasePath)
            => Save($@"{(useBasePath ? BasePath : "")}{path}");

        private void Save(string path)
        {
            byte[] bytes = ToBytes();

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);

            bytes = bytes.Concat(hash).ToArray();

            File.WriteAllBytes(path, Compress(bytes));

            sha256.Dispose();
        }

        public static World LoadFromFile(string name)
            => Load($"{BasePath}{name}.wld");

        public static World LoadFromFile(string path, bool useBasePath)
            => Load($"{(useBasePath ? BasePath : "")}{path}");

        private static World Load(string path)
        {
            World world = new World();

            if (IsValidFile(path))
            {
                byte[] bytes = File.ReadAllBytes(path);

                world.FromBytes(Decompress(bytes), 0);
            }

            return world;
        }

        public static World LoadFromBytes(byte[] bytes)
        {
            World world = new World();
            world.FromBytes(Decompress(bytes), 0);
            return world;
        }

        public byte[] ToCompressed()
        {
            return Compress(ToBytes());
        }

        public static bool IsValidFile(string path)
        {
            if (File.Exists(path) && Path.GetExtension(path) == ".wld")
            {
                SHA256 sha256 = SHA256.Create();

                byte[] bytes = Decompress(File.ReadAllBytes(path));
                byte[] hash = sha256.ComputeHash(bytes.Take(bytes.Length - 32).ToArray());
                byte[] readHash = bytes.Skip(bytes.Length - 32).ToArray();

                // Compare hashes
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != readHash[i])
                        return false;
                }

                sha256.Dispose();
                return true;
            }

            return false;
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

            // Write title
            byte[] title = Encoding.UTF8.GetBytes(Title ?? "");
            bytes.AddRange(title.Length.ToBytes());
            bytes.AddRange(title);

            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(Uploader));
            bytes.AddRange(BitConverter.GetBytes(UploadTime.ToBinary()));

            // Write levels
            bytes.AddRange(Levels.Count.ToBytes());

            for (int i = 0; i < Levels.Count; i++)
            {
                bytes.AddRange(Levels[i].ToBytes());
            }

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            Levels = new List<Level>();

            int total = 0;

            // Read title
            int titleCount = 0;
            total += titleCount.FromBytes(bytes, startIndex + total);
            Title = Encoding.UTF8.GetString(bytes, startIndex + total, titleCount);
            total += titleCount;

            ID = BitConverter.ToUInt32(bytes, startIndex + total);
            total += 4;
            Uploader = BitConverter.ToUInt32(bytes, startIndex + total);
            total += 4;
            UploadTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, startIndex + total));
            total += 8;

            // Read levels
            int count = BitConverter.ToInt32(bytes, startIndex + total);
            total += 4;

            for (int i = 0; i < count; i++)
            {
                Level level = new Level();
                total += level.FromBytes(bytes, startIndex + total);
                Levels.Add(level);
            }

            return total;
        }

        #endregion

        public override string ToString() => $"Levels: {Levels.Count}, Completed: {AllCompleted}, Perfect: {AllPerfect}";
    }
}
