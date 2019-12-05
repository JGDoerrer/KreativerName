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
            IDs = new List<byte>();
        }

        public Hex(HexPoint pos)
        {
            Position = pos;
            IDs = new List<byte>();
        }

        public Hex(int x, int y, int type)
        {
            Position = new HexPoint(x, y);
            IDs = new List<byte>();
        }

        public Hex(HexPoint pos, int type)
        {
            Position = pos;
            IDs = new List<byte>();
        }

        public HexPoint Position;
        public List<byte> IDs;

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
                    if (IDs.Contains(data.ID))
                        types.Add(data);
                }

                return types.OrderBy(x => x.ID).ToList();
            }
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Position.ToBytes());

            bytes.Add((byte)IDs.Count);
            bytes.AddRange(IDs);

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            count += Position.FromBytes(bytes, startIndex + count);
            byte idCount = bytes[startIndex + count];
            count += 1;

            IDs = new List<byte>();
            for (byte i = 0; i < idCount; i++)
            {
                IDs.Add(bytes[startIndex + count]);
                count += 1;
            }

            return count;
        }

        public override string ToString()
        {
            return $"{Position}; {IDs}";
        }
    }
}
