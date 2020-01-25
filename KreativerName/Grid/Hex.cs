using System.Collections.Generic;
using System.Linq;

namespace KreativerName.Grid
{
    /// <summary>
    /// Stores information about a hexagon in a level.
    /// </summary>
    public struct Hex : IBytes
    {
        #region Constructors

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
        /// <param name="pos">The position of the hex.</param>
        public Hex(HexPoint pos)
        {
            Position = pos;
            IDs = new List<byte>();
        }

        /// <summary>
        /// Creates a new hex with the specified position and type.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="type">The ID of the type.</param>
        public Hex(int x, int y, byte type)
        {
            Position = new HexPoint(x, y);
            IDs = new List<byte>() { type };
        }

        /// <summary>
        /// Creates a new hex with the specified position and type.
        /// </summary>
        /// <param name="pos">The position of the hex.</param>
        /// <param name="type">The ID of the type.</param>
        public Hex(HexPoint pos, byte type)
        {
            Position = pos;
            IDs = new List<byte>() { type };
        }

        #endregion

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
        public HexFlags GetFlags(HexData[] data)
        {
            HexFlags flags = 0;

            foreach (HexData hexData in GetTypes(data))
            {
                flags |= hexData.HexFlags;
            }

            return flags;
        }

        /// <summary>
        /// The HexData of the IDs.
        /// </summary>
        public List<HexData> GetTypes(HexData[] data)
        {
            List<HexData> types = new List<HexData>();

            foreach (byte id in IDs)
            {
                foreach (HexData d in data)
                {
                    if (d.ID == id)
                    {
                        types.Add(d);
                        break;
                    }
                }
            }

            return types;
        }

        /// <summary>
        /// Returns a byte array of the hex.
        /// </summary>
        /// <returns>Returns a byte array of the hex.</returns>
        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Position.ToBytes());

            bytes.Add((byte)IDs.Count);
            bytes.AddRange(IDs);

            return bytes.ToArray();
        }

        /// <summary>
        /// Loads a hex from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="startIndex">The start of the data in the array</param>
        /// <returns>Returns the amount of bytes loaded</returns>
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

        /// <summary>
        /// Returns a string describing the hex.
        /// </summary>
        /// <returns>Returns a string describing the hex.</returns>
        public override string ToString()
        {
            string s = $"{Position};";

            foreach (var id in IDs)
            {
                s += $" {id},";
            }

            return s;
        }
    }
}
