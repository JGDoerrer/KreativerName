using System;
using System.Collections.Generic;
using System.Linq;

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

        public Hex(int x, int y, int type)
        {
            Position = new HexPoint(x, y);
            Type = type;
        }

        public Hex(HexPoint pos, int type)
        {
            Position = pos;
            Type = type;
        }

        public HexPoint Position;
        public int Type;

        public int X => Position.X;
        public int Y => Position.Y;

        public HexFlags Flags
        {
            get
            {
                HexFlags flags = 0;

                foreach (HexData hexData in Types)
                {
                    flags |= hexData.Flags;
                }

                return flags;
            }
        }

        public List<HexData> Types
        {
            get
            {
                List<HexData> types = new List<HexData>();
                foreach (HexData data in HexData.Data)
                {
                    if ((Type & (1 << data.ID)) > 0)
                        types.Add(data);
                }

                return types.OrderBy(x => x.ID).ToList();
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
            Type = BitConverter.ToInt32(bytes, startIndex + 8);
            return 12;
        }

        public override string ToString()
        {
            return $"{Position}; {Type}";
        }
    }
}
