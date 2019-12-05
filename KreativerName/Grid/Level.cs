using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace KreativerName.Grid
{
    public struct Level : IBytes
    {
        public HexGrid<Hex> grid;
        public HexPoint startPos;
        public int minMoves;
        public bool completed;
        public bool perfect;

        HexPoint lastPlayer;
        bool updated;

        public void Update(HexPoint player)
        {
            if (!updated)
            {
                lastPlayer = startPos;
                updated = true;
            }

            HexGrid<Hex> nextGrid = grid.Copy();

            for (int i = 0; i < grid.Count; i++)
            {
                Hex hex = grid.Values.ElementAt(i);

                foreach (HexData type in hex.Types)
                {
                    foreach (HexAction action in type.Changes)
                    {
                        bool conditionMet = false;
                        bool move = true;
                        HexPoint nextPos = hex.Position + new HexPoint(action.MoveX, action.MoveY);

                        switch (action.Condition)
                        {
                            case HexCondition.Move:
                                conditionMet = true;
                                break;
                            case HexCondition.PlayerEnter:
                                conditionMet = hex.Position == player;
                                break;
                            case HexCondition.PlayerLeave:
                                conditionMet = hex.Position == lastPlayer;
                                break;
                            case HexCondition.NextSolid:
                                conditionMet = grid[nextPos] == null || grid[nextPos]?.Flags.HasFlag(HexFlags.Solid) == true;
                                move = false;
                                break;
                        }

                        if (conditionMet)
                        {
                            if (nextPos == hex.Position || !move)
                            {
                                Hex temp = nextGrid[hex.Position].Value;
                                temp.IDs.Remove(type.ID);
                                temp.IDs.Add(action.ChangeTo);
                                nextGrid[hex.Position] = temp;
                            }
                            else if (nextGrid[nextPos].HasValue && move)
                            {
                                Hex temp = nextGrid[hex.Position].Value;
                                temp.IDs.Remove(type.ID);
                                nextGrid[hex.Position] = temp;

                                Hex next = nextGrid[nextPos].Value;
                                next.IDs.Add(action.ChangeTo);
                                nextGrid[nextPos] = next;
                            }
                        }
                    }
                }
            }

            grid = nextGrid;

            lastPlayer = player;
        }
               
        #region Load & Save

        public void SaveToFile(string name)
        {
            byte[] bytes = Compress(ToBytes());
            Console.WriteLine($"Level {name}: {bytes.Length}/{ToBytes().Length} ");
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
            bytes.AddRange(grid.ToBytes());
            bytes.AddRange(startPos.ToBytes());
            bytes.AddRange(minMoves.ToBytes());
            byte flags = (byte)((completed ? 1 : 0) << 0 | (perfect ? 1 : 0) << 1);
            bytes.Add(flags);

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            grid = new HexGrid<Hex>();
            count += grid.FromBytes(bytes, startIndex + count);
            startPos = new HexPoint();
            count += startPos.FromBytes(bytes, startIndex + count);
            minMoves = BitConverter.ToInt32(bytes, startIndex + count);
            count += 4;
            completed = (bytes[startIndex + count] & (1 << 0)) > 0;
            perfect = (bytes[startIndex + count] & (1 << 1)) > 0;
            count += 1;

            return count;
        }

        public Level Copy()
        {
            Level level = new Level();
            level.FromBytes(ToBytes(), 0);
            return level;
        }

        #endregion

        public override string ToString()
        {
            string s = $"MinMoves: {minMoves}, Completed: {completed}, Perfect: {perfect}";

            return s;
        }
    }
}
