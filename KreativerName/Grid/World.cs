using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KreativerName.Grid
{
    /// <summary>
    /// Stores information about a list of levels.
    /// </summary>
    public struct World : IBytes
    {
        /// <summary>
        /// Creates a new World from a byte array.
        /// </summary>
        /// <param name="compressed">The byte array.</param>
        public World(byte[] compressed) : this()
        {
            FromBytes(Decompress(compressed), 0);
        }
        
        /// <summary>
        /// Stores all levels of the world.
        /// </summary>
        public List<Level> Levels;

        /// <summary>
        /// Stores the title of the world.
        /// </summary>
        public string Title;

        /// <summary>
        /// Stores the ID of the world.
        /// </summary>
        public uint ID;

        /// <summary>
        /// Stores the ID of the user who uploaded the world.
        /// </summary>
        public uint Uploader;

        /// <summary>
        /// Stores the time when the world was uploaded.
        /// </summary>
        public DateTime UploadTime;

        /// <summary>
        /// Defines if all levels have been completed.
        /// </summary>
        public bool AllCompleted
        {
            get
            {
                return Levels.All(x => x.Completed);
            }
            set
            {
                for (int i = 0; i < Levels.Count; i++)
                {
                    Level level = Levels[i];
                    level.Completed = value;
                    Levels[i] = level;
                }
            }
        }

        /// <summary>
        /// Defines if all levels have been completed perfectly.
        /// </summary>
        public bool AllPerfect
        {
            get
            {
                return Levels.All(x => x.Perfect);
            }
            set
            {
                for (int i = 0; i < Levels.Count; i++)
                {
                    Level level = Levels[i];
                    level.Perfect = value;
                    Levels[i] = level;
                }
            }
        }

        #region Load & Save

        /// <summary>
        /// The path where the world files are found.
        /// </summary>
        public static string BasePath => @"Resources\Worlds\";

        /// <summary>
        /// Saves the world to a file in the base path with the specified name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        public void SaveToFile(string name)
            => Save($@"{BasePath}{name}.wld");

        /// <summary>
        /// Saves the world to a file with the specified path.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="useBasePath"></param>
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

        /// <summary>
        /// Loads a world from a file in the base path with the specified name.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>Returns the world in the specified file</returns>
        public static World LoadFromFile(string name)
            => Load($"{BasePath}{name}.wld");

        /// <summary>
        /// Loads a world from a file with the specified path.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="useBasePath"></param>
        /// <returns>Returns the world in the specified file</returns>
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

        /// <summary>
        /// Loads a world from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <returns>Returns the world from the array</returns>
        public static World LoadFromBytes(byte[] bytes)
        {
            World world = new World();
            world.FromBytes(Decompress(bytes), 0);
            return world;
        }

        /// <summary>
        /// Returns a compressed byte array of the world.
        /// </summary>
        /// <returns>Returns a compressed byte array of the world.</returns>
        public byte[] ToCompressed()
        {
            return Compress(ToBytes());
        }

        /// <summary>
        /// Checks if a file is a valid world file.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>Returns if the file is valid</returns>
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

        /// <summary>
        /// Returns a byte array of the world.
        /// </summary>
        /// <returns>Returns a byte array of the world.</returns>
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

        /// <summary>
        /// Loads a world from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="startIndex">The start of the data in the array</param>
        /// <returns>Returns the amount of bytes loaded</returns>
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

        /// <summary>
        /// Returns a string describing the world.
        /// </summary>
        /// <returns>Returns a string describing the world.</returns>
        public override string ToString() => $"Levels: {Levels.Count}, Completed: {AllCompleted}, Perfect: {AllPerfect}";
    }
}
