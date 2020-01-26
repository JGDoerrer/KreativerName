using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        HexPoint lastPlayer;
        bool updated;

        /// <summary>
        /// Updates the level.
        /// </summary>
        /// <param name="player">The new position of the player.</param>
        public void Update(HexPoint player)
        {
            if (!updated)
            {
                lastPlayer = StartPos;
                updated = true;
            }

            HexGrid<Hex> nextGrid = Grid.Copy();

            for (int i = 0; i < Grid.Count; i++)
            {
                Hex hex = Grid.Values.ElementAt(i);

                UpdateHex(hex, player, ref nextGrid);
            }

            Grid = nextGrid;

            lastPlayer = player;
        }

        private void UpdateHex(Hex hex, HexPoint player, ref HexGrid<Hex> nextGrid)
        {
            foreach (HexData type in hex.GetTypes(Data))
            {
                HexPoint position = hex.Position;

                foreach (HexAction action in type.Changes)
                {
                    bool conditionMet = false;
                    bool move = true;
                    HexPoint nextPos = position + new HexPoint(action.MoveX, action.MoveY);

                    switch (action.Condition)
                    {
                        case HexCondition.Move:
                            conditionMet = true;
                            break;
                        case HexCondition.PlayerEnter:
                            conditionMet = position == player;
                            break;
                        case HexCondition.PlayerLeave:
                            conditionMet = position == lastPlayer;
                            break;
                        case HexCondition.NextFlag:
                            conditionMet = Grid[nextPos] == null || (Grid[nextPos]?.GetFlags(Data) & (HexFlags)action.Data) != 0;
                            move = false;
                            break;
                        case HexCondition.NextNotFlag:
                            conditionMet = Grid[nextPos] != null && (Grid[nextPos]?.GetFlags(Data) & (HexFlags)action.Data) == 0;
                            break;
                        case HexCondition.NextID:
                            conditionMet = Grid[nextPos] == null || Grid[nextPos]?.IDs.Contains(action.Data) == true;
                            move = false;
                            break;
                        case HexCondition.NextNotID:
                            conditionMet = Grid[nextPos] != null && Grid[nextPos]?.IDs.Contains(action.Data) != true;
                            break;
                    }

                    if (conditionMet && nextGrid[position].Value.IDs.Contains(type.ID))
                    {
                        if (nextPos == position || !move)
                        {
                            Hex temp = nextGrid[position].Value;
                            temp.IDs.Remove(type.ID);
                            temp.IDs.Add(action.ChangeTo);
                            nextGrid[position] = temp;
                        }
                        else if (nextGrid[nextPos].HasValue && move)
                        {
                            Hex temp = nextGrid[position].Value;
                            temp.IDs.Remove(type.ID);

                            Hex next = nextGrid[nextPos].Value;
                            next.IDs.Add(action.ChangeTo);

                            nextGrid[position] = temp;
                            nextGrid[nextPos] = next;

                            position = nextPos;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns all possible moves for a player position.
        /// </summary>
        /// <param name="player">The position of the player.</param>
        /// <returns>All possible moves for the specified player position.</returns>
        public List<HexPoint> GetPossibleMoves(HexPoint player)
        {
            HexPoint[] directions = {
                new HexPoint( 1,  0),
                new HexPoint( 1, -1),
                new HexPoint( 0, -1),
                new HexPoint(-1,  0),
                new HexPoint(-1,  1),
                new HexPoint( 0,  1),
            };

            if (Grid == null)
                return new List<HexPoint>();

            List<HexPoint> moves = new List<HexPoint>();

            for (int i = 0; i < 6; i++)
            {
                int j = 1;

                while (Grid[(directions[i] * j) + player].HasValue && !Grid[(directions[i] * j) + player].Value.GetFlags(Data).HasFlag(HexFlags.Solid))
                {
                    moves.Add(directions[i] * j + player);
                    j++;
                }
            }

            return moves;
        }

        #region Load & Save

        public void SaveToFile(string name)
        {
            byte[] bytes = Compress(ToBytes());

            File.WriteAllBytes($@"Resources\Levels\{name}.lvl", bytes);
        }

        public static Level LoadFromFile(string name)
        {
            Level level = new Level();
            string path = $@"Resources\Levels\{name}.lvl";

            if (File.Exists(path))
            {
                byte[] bytes = Decompress(File.ReadAllBytes(path));
                level.FromBytes(bytes, 0);
            }

            return level;
        }


        static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }


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
