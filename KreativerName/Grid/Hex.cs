using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[9];
            Position.ToBytes().CopyTo(bytes, 0);
            bytes[8] = (byte)Type;
            return bytes;
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            Position.FromBytes(bytes, startIndex);
            Type = (HexType)bytes[startIndex + 8];
            return 9;
        }

        public void SetType(HexType type) => Type = type;
    }

    public enum HexType : byte
    {
        Normal,
        Solid,
        Deadly,
        Goal,
    }
}
