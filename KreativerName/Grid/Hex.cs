using System;
using System.Collections.Generic;

namespace KreativerName.Grid
{
    public struct Hex : IBytes
    {
        public Hex(int x, int y)
        {
            Position = new HexPoint(x, y);
            Type = 0;
        }

        public Hex(HexPoint pos)
        {
            Position = pos;
            Type = 0;
        }

        public Hex(int x, int y, HexType type)
        {
            Position = new HexPoint(x, y);
            Type = type;
        }

        public Hex(HexPoint pos, HexType type)
        {
            Position = pos;
            Type = type;
        }

        public HexPoint Position;
        public HexType Type;
        public int X => Position.X;
        public int Y => Position.Y;

        public HexFlags Flags
        {
            get
            {
                HexFlags flags = 0;

                foreach (HexType value in Enum.GetValues(typeof(HexType)))
                {
                    if (Type.HasFlag(value))
                        flags |= value switch
                        {
                            HexType.Normal => 0,
                            HexType.Solid => HexFlags.Solid,
                            HexType.Deadly => HexFlags.Deadly,
                            HexType.Goal => HexFlags.Goal,
                            HexType.DeadlyTwoStateOn => HexFlags.Deadly,
                            HexType.DeadlyTwoStateOff => 0,
                            HexType.DeadlyOneUseOn => HexFlags.Deadly,
                            HexType.DeadlyOneUseOff => 0,

                            _ => 0,
                        };
                }

                return flags;
            }
        }

        public List<HexType> Types
        {
            get
            {
                List<HexType> types = new List<HexType>();
                foreach (HexType type in Enum.GetValues(typeof(HexType)))
                {
                    if (Type.HasFlag(type))
                        types.Add(type);
                }
                return types;
            }
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[12];
            Position.ToBytes().CopyTo(bytes, 0);
            ((int)Type).ToBytes().CopyTo(bytes, 8);
            return bytes;
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            Position.FromBytes(bytes, startIndex);            
            Type = (HexType)BitConverter.ToInt32(bytes, startIndex + 8);
            return 12;
        }

        public override string ToString()
        {
            return $"{Position}; {Type}";
        }
    }

    [Flags]
    public enum HexType : int
    {
        Normal = 1 << 0,
        Solid = 1 << 1,
        Deadly = 1 << 2,
        Goal = 1 << 3,
        DeadlyTwoStateOn = 1 << 4,
        DeadlyTwoStateOff = 1 << 5,
        DeadlyOneUseOff = 1 << 6,
        DeadlyOneUseOn = 1 << 7,
    }

    [Flags]
    public enum HexFlags
    {
        Solid = 1 << 0,
        Deadly = 1 << 1,
        Goal = 1 << 2,
    }
}
