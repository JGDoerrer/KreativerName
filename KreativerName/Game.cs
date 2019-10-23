using KreativerName.Grid;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace KreativerName
{
    public class Game
    {
        public Game()
        {
            hexagons = new HexGrid<Hex>();

            for (int j = 0; j < 9; j++)
            {
                for (int i = -(j / 2); i < 8 - (j + 1) / 2; i++)
                {
                    if (j > 6)
                        hexagons[i, j] = new Hex(i, j, new Troop(0, TroopType.Troop1));
                    else if (j < 2)
                        hexagons[i, j] = new Hex(i, j, new Troop(1, TroopType.Troop1));
                    else
                        hexagons[i, j] = new Hex(i, j);
                }
            }

            currentTeam = 0;
            teams = 2;
        }

        public HexGrid<Hex> hexagons;
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
            if (!hexagons.Contains(point))
                return new List<HexPoint>();

            Hex hex = (Hex)hexagons[point];

            if (!hex.Troop.HasValue || (hex.Troop.HasValue && hex.Troop.Value.Team != team))
                return new List<HexPoint>();

            List<HexPoint> positions = new List<HexPoint>();
            List<HexPoint> moves = new List<HexPoint>();

            switch (hex.Troop.Value.Type)
            {
                case TroopType.Troop1:
                    positions = new List<HexPoint>()
                    {
                        new HexPoint( 1,  0),
                        new HexPoint( 1, -1),
                        new HexPoint( 0, -1),
                        new HexPoint(-1,  0),
                        new HexPoint(-1,  1),
                        new HexPoint( 0,  1),
                    };
                    break;
            }

            foreach (var move in positions)
            {
                Hex? h = hexagons[point + move];
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

        public void Move(HexPoint from, HexPoint to)
        {
            if (!hexagons[from].HasValue)
                return;

            Hex hexFrom = (Hex)hexagons[from];
            hexFrom.Position = to;

            hexagons[to] = hexFrom;
            hexagons[from] = new Hex(from);
        }
    }
}
