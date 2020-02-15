using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KreativerName.Grid
{
    public struct HexData : IBytes
    {
        public byte ID;
        public HexFlags HexFlags;
        public RenderFlags RenderFlags;
        public byte Texture;
        public byte AnimationLength;
        public byte AnimationSpeed;
        public byte AnimationPhase;
        public List<HexAction> Changes;

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>
            {
                ID,
                (byte)HexFlags,
                (byte)RenderFlags,
                Texture,
                AnimationLength,
                AnimationSpeed,
                AnimationPhase,

                (byte)Changes.Count
            };

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
            HexFlags = (HexFlags)bytes[startIndex + count];
            count += 1;
            RenderFlags = (RenderFlags)bytes[startIndex + count];
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

        public static HexData[] StandardData;

        public static void LoadData(string directory)
        {
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory);

                StandardData = new HexData[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    HexData hexData = LoadFromFile(file);
                    StandardData[i] = hexData;
                }

                StandardData = StandardData.OrderBy(x => x.ID).ToArray();
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
            //path = @$"Resources\HexData\{path}.hex";

            byte[] bytes = ToBytes();

            File.WriteAllBytes(path, bytes);
        }

        public override string ToString() => $"ID: {ID}, Flags: {HexFlags};{RenderFlags}, Texture: {Texture}, Changes: {Changes.Count}";
    }

    public struct HexAction : IBytes
    {
        public byte ChangeTo;
        public byte Data;
        public sbyte MoveX;
        public sbyte MoveY;
        public HexActionFlags Flags;
        public HexCondition Condition;

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ChangeTo = bytes[startIndex + count];
            count += 1;
            Data = bytes[startIndex + count];
            count += 1;
            MoveX = (sbyte)bytes[startIndex + count];
            count += 1;
            MoveY = (sbyte)bytes[startIndex + count];
            count += 1;
            Flags = (HexActionFlags)bytes[startIndex + count];
            count += 1;
            Condition = (HexCondition)bytes[startIndex + count];
            count += 1;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>
            {
                ChangeTo,
                Data,
                (byte)MoveX,
                (byte)MoveY,
                (byte)Flags,
                (byte)Condition
            };

            return bytes.ToArray();
        }

        public override string ToString() => $"Condition: {Condition}, Change to: {ChangeTo}, Move by: {MoveX}, {MoveY}, ";
    }

    public enum HexCondition : byte
    {
        Move = 0,
        PlayerEnter = 1,
        PlayerLeave = 2,
        NextFlag = 3,
        NextNotFlag = 4,
        NextID = 5,
        NextNotID = 6,
        PlayerEnterID = 7,
        PlayerLeaveID = 8,
    }

    [Flags]
    public enum HexActionFlags : byte
    {
        MoveHex = 1 << 0,
        MovePlayer = 1 << 1,
    }

    [Flags]
    public enum HexFlags : byte
    {
        Solid = 1 << 0,
        Deadly = 1 << 1,
        Goal = 1 << 2,
    }

    [Flags]
    public enum RenderFlags : byte
    {
        Animated = 1 << 0,
        Connected = 1 << 1,
    }
}
