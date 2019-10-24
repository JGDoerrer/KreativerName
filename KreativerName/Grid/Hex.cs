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
            Troop = null;
        }

        public Hex(HexPoint pos)
        {
            Position = pos;
            Troop = null;
        }

        public Hex(int x, int y, Troop troop)
        {
            Position = new HexPoint(x, y);
            Troop = troop;
        }

        public Hex(HexPoint pos, Troop troop)
        {
            Position = pos;
            Troop = troop;
        }

        public HexPoint Position;
        public int X => Position.X;
        public int Y => Position.Y;

        public Troop? Troop;

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Troop.HasValue ? 14 : 9];
            Position.ToBytes().CopyTo(bytes, 0);
            bytes[8] = (byte)(Troop.HasValue ? 1 : 0);
            if (Troop.HasValue)
                Troop.Value.ToBytes().CopyTo(bytes, 9);
            return bytes;
        }

        public void FromBytes(byte[] bytes, int startIndex)
        {
            Position.FromBytes(bytes, startIndex);
            if (bytes[startIndex + 8] > 0)
            {
                Troop t = new Troop();
                t.FromBytes(bytes, startIndex + 9);
                Troop = t;
            }
        }
    }
}
