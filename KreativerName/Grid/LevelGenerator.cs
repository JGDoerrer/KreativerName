using System;
using System.Linq;

namespace KreativerName.Grid
{
    public static class LevelGenerator
    {
        static Random random = new Random();

        public static Level GenerateLevel(HexData[] data)
        {
            Level level = new Level
            {
                Grid = GenerateGrid(),
                Data = data
            };

            // Choose random start position
            level.StartPos = level.Grid.Keys.ToList().Random();

            MakePuzzle(ref level);

            return level;
        }

        public static HexGrid<Hex> GenerateGrid()
        {
            HexGrid<Hex> grid = new HexGrid<Hex>();

            MakeBase(ref grid);

            return grid;
        }

        private static void MakeBase(ref HexGrid<Hex> grid)
        {
            int w = random.Next(5, 10);
            int h = random.Next(5, 10);

            switch (random.Next(0, 3))
            {
                case 0: // Rectangle
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = -(y / 2); x < w - (y + 1) / 2; x++)
                        {
                            grid[x, y] = new Hex(x, y, 0);
                        }
                    }
                    break;
                }
                case 1: // Parallelogram
                {
                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < h; y++)
                        {
                            grid[x, y] = new Hex(x, y, 0);
                        }
                    }
                    break;
                }
                case 2: // Hexagon
                {
                    w -= 3;

                    for (int x = -w; x <= w; x++)
                    {
                        int y1 = Math.Max(-w, -x - w);
                        int y2 = Math.Min(w, -x + w);

                        for (int y = y1; y <= y2; y++)
                        {
                            grid[x, y] = new Hex(x, y, 0);
                        }
                    }
                    break;
                }
                default:
                    break;
            }
        }

        private static void MakePuzzle(ref Level level)
        {
            // Choose goal
            HexPoint goal;

            do
            {
                goal = level.Grid.Keys.ToList().Random();
            }
            while (goal.DistanceTo(level.StartPos) < 3);



            byte id = level.Data.Where(x => x.HexFlags.HasFlag(HexFlags.Goal)).ToList().Random().ID;
            level.Grid[goal] = new Hex(goal, id);
        }
    }
}
