using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KreativerName.Grid
{
    /// <summary>
    /// Stores information about a level.
    /// </summary>
    public struct Level : IBytes
    {
        /// <summary>
        /// Stores the grid of the level.
        /// </summary>
        public HexGrid<Hex> Grid;

        /// <summary>
        /// Stores the start position of the player.
        /// </summary>
        public HexPoint StartPos;

        /// <summary>
        /// Stores the minimum amount of moves to complete the level.
        /// </summary>
        public int MinMoves;

        /// <summary>
        /// Stores if the level has been completed.
        /// </summary>
        public bool Completed;

        /// <summary>
        /// Stores if the level has been completed perfectly.
        /// </summary>
        public bool Perfect;

        /// <summary>
        /// Stores a hint for solving the level.
        /// </summary>
        public string Hint;

        public HexData[] Data;

        #region Load & Save

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Grid.ToBytes());
            bytes.AddRange(StartPos.ToBytes());
            bytes.AddRange(MinMoves.ToBytes());
            byte flags = (byte)((Completed ? 1 : 0) << 0 | (Perfect ? 1 : 0) << 1);
            bytes.Add(flags);

            byte[] hint = Encoding.UTF8.GetBytes(Hint ?? "");
            bytes.AddRange(hint.Length.ToBytes());
            bytes.AddRange(hint);

            // Write hex data
            bytes.AddRange(Data.Length.ToBytes());

            for (int i = 0; i < Data.Length; i++)
            {
                bytes.AddRange(Data[i].ToBytes());
            }

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            try
            {
                Grid = new HexGrid<Hex>();
                count += Grid.FromBytes(bytes, startIndex + count);
                StartPos = new HexPoint();
                count += StartPos.FromBytes(bytes, startIndex + count);
                MinMoves = BitConverter.ToInt32(bytes, startIndex + count);
                count += 4;
                Completed = (bytes[startIndex + count] & (1 << 0)) > 0;
                Perfect = (bytes[startIndex + count] & (1 << 1)) > 0;
                count += 1;

                int hintLength = 0;
                count += hintLength.FromBytes(bytes, startIndex + count);
                Hint = Encoding.UTF8.GetString(bytes, startIndex + count, hintLength);
                count += hintLength;

                // Read hex data
                int dataCount = BitConverter.ToInt32(bytes, startIndex + count);
                count += 4;

                Data = new HexData[dataCount];

                for (int i = 0; i < dataCount; i++)
                {
                    HexData hexData = new HexData();
                    count += hexData.FromBytes(bytes, startIndex + count);
                    Data[i] = hexData;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Level]: Error while loading: {e.Message}");
            }

            return count;
        }

        public Level Copy()
        {
            Level level = new Level();
            level.FromBytes(ToBytes(), 0);
            return level;
        }

        #endregion

        /// <summary>
        /// Returns a string describing the level.
        /// </summary>
        /// <returns>A string describing the level.</returns>
        public override string ToString()
        {
            string s = $"MinMoves: {MinMoves}, Completed: {Completed}, Perfect: {Perfect}";

            return s;
        }
    }
}
