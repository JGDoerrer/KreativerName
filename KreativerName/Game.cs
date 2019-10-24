using System;
using System.Collections.Generic;
using System.Linq;
using KreativerName.Grid;
using OpenTK;
using OpenTK.Input;

namespace KreativerName
{
    public class Game
    {
        public Game()
        {
            if (false)
            {
                level = Level.LoadFromFile("000");
            }
            else
            {
                Grid = new HexGrid<Hex>();

                int w = 11;
                int h = 10;

                for (int j = 0; j < w; j++)
                {
                    for (int i = -(j / 2); i < h - (j + 1) / 2; i++)
                    {
                        if (j > h - 2)
                            Grid[i, j] = new Hex(i, j, new Troop(0, TroopType.Knight));
                        else if (j < 2)
                            Grid[i, j] = new Hex(i, j, new Troop(1, TroopType.Rook));
                        else
                            Grid[i, j] = new Hex(i, j);
                    }
                }

                level.SaveToFile("000");
            }

            currentTeam = 0;
            teams = 2;
        }

        Level level;
        internal Input input;
        internal HexPoint selectedHex;
        public int currentTeam;
        public int teams;

        const float size = 40;
        internal HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(75, 75),
            size, 0.5f);

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition());

            if (input.MousePress(MouseButton.Left))
            {
                var moves = GetMoves(selectedHex, currentTeam);
                if (moves.Contains(mouse))
                {
                    Move(selectedHex, mouse);

                    currentTeam++;
                    currentTeam %= teams;
                }

                selectedHex = mouse;
            }

            input.Update();
        }

        public List<HexPoint> GetMoves(HexPoint point, int team)
        {
            if (!Grid.Contains(point))
                return new List<HexPoint>();

            Hex hex = (Hex)Grid[point];

            if (!hex.Troop.HasValue || (hex.Troop.HasValue && hex.Troop.Value.Team != team))
                return new List<HexPoint>();

            List<HexPoint> positions = GetPossibleMoves(point, hex.Troop.Value);
            List<HexPoint> moves = new List<HexPoint>();

            foreach (var move in positions)
            {
                Hex? h = Grid[point + move];
                if (h.HasValue)
                {
                    if ((h.Value.Troop.HasValue && h.Value.Troop.Value.Team != hex.Troop.Value.Team) ||
                        !h.Value.Troop.HasValue)
                    {
                        moves.Add(point + move);
                    }
                }
            }

            return moves;
        }

        private List<HexPoint> GetPossibleMoves(HexPoint point, Troop troop)
        {
            List<HexPoint> positions = new List<HexPoint>();

            HexPoint[] directions = {
                new HexPoint( 1,  0),
                new HexPoint( 1, -1),
                new HexPoint( 0, -1),
                new HexPoint(-1,  0),
                new HexPoint(-1,  1),
                new HexPoint( 0,  1),
            };

            switch (troop.Type)
            {
                case TroopType.Pawn:
                {
                    positions = directions.ToList();
                    break;
                }
                case TroopType.Rook:
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int j = 0;
                        do
                        {
                            j++;
                            positions.Add(directions[i] * j);
                        }
                        while (Grid[(directions[i] * j) + point].HasValue && (!Grid[(directions[i] * j) + point].Value.Troop.HasValue));
                    }
                    break;
                }
                case TroopType.Knight:
                {
                    positions = new List<HexPoint>()
                    {
                        new HexPoint( 1,  1),
                        new HexPoint( 2, -1),
                        new HexPoint(-1,  2),
                        new HexPoint(-2,  1),
                        new HexPoint(-1, -1),
                        new HexPoint( 1, -2),
                    };
                    break;
                }
            }

            return positions;
        }

        public void Move(HexPoint from, HexPoint to)
        {
            if (!Grid[from].HasValue)
                return;

            Hex hexFrom = (Hex)Grid[from];
            hexFrom.Position = to;

            Grid[to] = hexFrom;
            Grid[from] = new Hex(from);
        }
    }
}
