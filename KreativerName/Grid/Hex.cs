using System;
using System.Collections.Generic;

namespace KreativerName.Grid
{
    public struct Hex : IBytes
    {
        /// <summary>
        /// Creates a new hex with the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        public Hex(int x, int y)    
        {
            Position = new HexPoint(x, y);
            IDs = new List<byte>();
        }

        /// <summary>
        /// Creates a new hex with the specified position.
        /// </summary>
        /// <param name="pos">The position of the hex</param>
        public Hex(HexPoint pos)
        {
            Position = pos;
            IDs = new List<byte>();
        }

        public Hex(int x, int y, byte type)
        {
            Position = new HexPoint(x, y);
            IDs = new List<byte>() { type };
        }

        public Hex(HexPoint pos, byte type)
        {
            Position = pos;
            IDs = new List<byte>() { type };
        }

        /// <summary>
        /// The position of the hex.
        /// </summary>
        public HexPoint Position;

        /// <summary>
        /// The IDs of the types of the hex.
        /// </summary>
        public List<byte> IDs;

        /// <summary>
        /// The x-coordinate of the position.
        /// </summary>
        public int X => Position.X;

        /// <summary>
        /// The y-coordinate of the position.
        /// </summary>
        public int Y => Position.Y;
        
        /// <summary>
        /// The combined HexFlags of the types.
        /// </summary>
        public HexFlags Flags
        {
            get
            {
                HexFlags flags = 0;

                foreach (HexData hexData in Types)
                {
                    flags |= hexData.HexFlags;
                }

                return flags;
            }
        }

        public List<HexData> Types
        {
            get
            {
                List<HexData> types = new List<HexData>();

                foreach (byte id in IDs)
                {
                    // Assume Hexdata.data is ordered by id
                    if (HexData.Data.Length > id)
                        types.Add(HexData.Data[id]);
                }

                return types;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
