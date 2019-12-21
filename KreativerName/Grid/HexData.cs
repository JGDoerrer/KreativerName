using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KreativerName.Grid
{
    public struct HexData : IBytes
    {
        public const int MaxID = 32;

        public byte ID;
        public HexFlags Flags;
        public byte Texture;
        public byte AnimationLength;
        public byte AnimationSpeed;
        public byte AnimationPhase;

        public List<HexAction> Changes;

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ID);
            bytes.Add((byte)Flags);
            bytes.Add(Texture);
            bytes.Add(AnimationLength);
            bytes.Add(AnimationSpeed);
            bytes.Add(AnimationPhase);

            bytes.Add((byte)Changes.Count);

            for (int i = 0; i < (byte)Changes.Count; i++)
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
            Texture = bytes[startIndex + count];
            count += 1;
            AnimationLength = bytes[startIndex + count];
            count += 1;
            AnimationSpeed = bytes[startIndex + count];
            count += 1;
            AnimationPhase = bytes[startIndex + count];
            count += 1;

            int changes = bytes[startIndex + count];
            count += 1;

            Changes = new List<HexAction>();

            for (int i = 0; i < changes; i++)
            {
                HexAction change = new HexAction();
                count += change.FromBytes(bytes, startIndex + count);

                Changes.Add(change);
            }

            return count;
        }

        public override string ToString() => $"ID: {ID}, Flags: {Flags}, Texture: {Texture}, Changes: {Changes.Count}";
        
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

                Data = Data.OrderBy(x => x.ID).ToArray();
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

    public struct HexAction : IBytes
    {
        public byte ChangeTo;
        public sbyte MoveX;
        public sbyte MoveY;
        public HexCondition Condition;

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ChangeTo = bytes[startIndex + count];
            count += 1;
            MoveX = (sbyte)bytes[startIndex + count];
            count += 1;
            MoveY = (sbyte)bytes[startIndex + count];
            count += 1;
            Condition = (HexCondition)bytes[startIndex + count];
            count += 1;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ChangeTo);
            bytes.Add((byte)MoveX);
            bytes.Add((byte)MoveY);
            bytes.Add((byte)Condition);

            return bytes.ToArray();
        }

        public override string ToString() => $"Condition: {Condition}, Change to: {ChangeTo}, Move by: {MoveX}, {MoveY}, ";
    }

    public enum HexCondition : byte
    {
        Move,
        PlayerEnter,
        PlayerLeave,
        NextSolid,
        NextNotSolid,
    }

    [Flags]
    public enum HexFlags : byte
    {
        Solid = 1 << 0,
        Deadly = 1 << 1,
        Goal = 1 << 2,
    }
}
