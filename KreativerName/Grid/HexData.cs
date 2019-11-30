using System;
using System.Collections.Generic;
using System.IO;

namespace KreativerName.Grid
{
    public struct HexData : IBytes
    {
        public const int MaxID = 32;

        public byte ID;
        public HexFlags Flags;
        public int Texture;

        public List<HexChange> Changes;

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ID);
            bytes.Add((byte)Flags);
            bytes.AddRange(Texture.ToBytes());

            bytes.AddRange(Changes.Count.ToBytes());

            for (int i = 0; i < Changes.Count; i++)
            {
                bytes.AddRange(Changes[i].ToBytes());
            }

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ID = bytes[startIndex + count];
            count += 1;
            Flags = (HexFlags)bytes[startIndex + count];
            count += 1;

            count += Texture.FromBytes(bytes, startIndex + count);

            int changes = 0;
            count += changes.FromBytes(bytes, startIndex + count);

            Changes = new List<HexChange>();

            for (int i = 0; i < changes; i++)
            {
                HexChange change = new HexChange();
                count += change.FromBytes(bytes, startIndex + count);

                Changes.Add(change);
            }

            return count;
        }

        public static HexData[] Data;

        public static void LoadData(string directory)
        {
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory);

                Data = new HexData[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    HexData hexData = LoadFromFile(file);
                    Data[i] = hexData;
                }
            }
        }

        private static HexData LoadFromFile(string path)
        {
            HexData data = new HexData();

            if (File.Exists(path) && Path.GetExtension(path) == ".hex")
            {
                byte[] bytes = File.ReadAllBytes(path);
                data.FromBytes(bytes, 0);
            }

            return data;
        }

        public void SaveToFile(string path)
        {
            path = @$"Resources\HexData\{path}.hex";

            byte[] bytes = ToBytes();

            File.WriteAllBytes(path, bytes);
        }
    }

    public struct HexChange : IBytes
    {
        public byte ChangeTo;
        public HexCondition Condition;

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ChangeTo = bytes[startIndex + count];
            count += 1;
            Condition = (HexCondition)bytes[startIndex + count];
            count += 1;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ChangeTo);
            bytes.Add((byte)Condition);

            return bytes.ToArray();
        }
    }

    public enum HexCondition : byte
    {
        Move,
        PlayerEnter,
        PlayerLeave
    }

    [Flags]
    public enum HexFlags : byte
    {
        Solid = 1 << 0,
        Deadly = 1 << 1,
        Goal = 1 << 2,
    }
}
