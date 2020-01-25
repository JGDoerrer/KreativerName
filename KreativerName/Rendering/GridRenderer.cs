using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KreativerName.Grid;
using OpenTK;

namespace KreativerName.Rendering
{
    public class GridRenderer
    {
        public GridRenderer()
        { }
        public GridRenderer(HexGrid<Hex> grid, HexLayout layout)
        {
            Grid = grid;
            Layout = layout;
        }

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        public HexGrid<Hex> Grid { get; set; }
        public HexData[] Data { get; set; }
        public HexLayout Layout { get; set; }

        int frameCount = 0;

        public void Render(HexPoint player, HexPoint selectedHex, List<HexPoint> moves)
        {
            if (Grid == null)
                return;

            foreach (Hex hex in Grid)
            {
                Vector2 renderPos = Layout.HexCorner(hex.Position, 3);
                renderPos.X -= Layout.size / 2f * sqrt3;
                renderPos.Y -= Layout.size / 2f;

                renderPos.X = (float)Math.Floor(renderPos.X);
                renderPos.Y = (float)Math.Floor(renderPos.Y);

                Color mask;

                if (selectedHex == hex.Position)
                {
                    if (moves != null && moves.Contains(hex.Position))
                        mask = Color.FromArgb(80, 0, 100, 0);
                    else
                        mask = Color.FromArgb(50, Color.Black);
                }
                else if (moves != null && moves.Contains(hex.Position))
                    mask = Color.FromArgb(80, 0, 200, 0);
                else
                    mask = Color.Transparent;

                RenderHex(hex.Position, hex.GetTypes(Data), Layout, Color.White, frameCount, Grid);

                if (mask.A > 0)
                    TextureRenderer.DrawHex(Textures.Get("Hex\\Mask"), hex.Position, Layout, Vector2.One * Layout.size, mask, null);

                if (hex.Position == player)
                    TextureRenderer.DrawHex(Textures.Get("Player"), hex.Position, Layout, Vector2.One * Layout.size, Color.White, null);
            }

            frameCount++;
        }


        public static void RenderHex(HexPoint pos, List<HexData> types, HexLayout layout, Color color, int frameCount, HexGrid<Hex> grid = null)
        {
            types = types.OrderBy(x => x.ID).ToList();

            for (int i = 0; i < types.Count; i++)
            {
                int animation = 0;
                int connection = 0;

                if (types[i].RenderFlags.HasFlag(RenderFlags.Animated) && types[i].AnimationLength != 0 && types[i].AnimationSpeed != 0)
                {
                    animation = ((frameCount + types[i].AnimationPhase) / types[i].AnimationSpeed) % types[i].AnimationLength;
                }

                if (grid != null && types[i].RenderFlags.HasFlag(RenderFlags.Connected))
                {
                    HexPoint[] directions = {
                        new HexPoint( 1,  0), // E  1
                        new HexPoint( 1, -1), // NE 2
                        new HexPoint( 0, -1), // NW 4
                        new HexPoint(-1,  0), // W  8
                        new HexPoint(-1,  1), // SW 16
                        new HexPoint( 0,  1), // SE 32
                    };

                    for (int j = 0; j < 6; j++)
                    {
                        if (grid[pos + directions[j]].HasValue && grid[pos + directions[j]].Value.IDs.Contains(types[i].ID))
                            connection += 1 << j;
                    }
                }

                TextureRenderer.DrawHex(Textures.Get($"Hex\\{types[i].Texture}"), pos, layout, Vector2.One * layout.size, color, new RectangleF(32 * connection, animation * 32, 32, 32));
            }
        }
    }
}
