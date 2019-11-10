using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.Scenes;
using OpenTK;

namespace KreativerName.Rendering
{
    public static class GridRenderer
    {
        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        public static void RenderGrid(HexGrid<Hex> grid, HexLayout layout, List<HexPoint> moves, HexPoint selectedHex, HexPoint player)
        {
            if (grid == null)
                return;

            foreach (Hex hex in grid)
            {
                Vector2 renderPos = layout.HexCorner(hex.Position, 3);
                renderPos.X -= layout.size / 2f * sqrt3;
                renderPos.Y -= layout.size / 2f;

                renderPos.X = (float)Math.Floor(renderPos.X);
                renderPos.Y = (float)Math.Floor(renderPos.Y);

                Color color = Color.White;

                if (selectedHex == hex.Position)
                {
                    if (moves != null && moves.Contains(hex.Position))
                        color = Color.FromArgb(255, 100, 200, 100);
                    else
                        color = Color.FromArgb(255, 200, 200, 200);
                }
                else if (moves != null && moves.Contains(hex.Position))
                    color = Color.LightGreen;
                else
                    color = Color.White;

                if (hex.Position == player)
                    TextureRenderer.DrawHex(Textures.Get("Player"), hex.Position, layout, Vector2.One * layout.size / 10, color, null);
                else
                    TextureRenderer.DrawHex(Textures.Get("Hex"), hex.Position, layout, Vector2.One * layout.size / 10, color, new RectangleF(32 * (int)hex.Type, 0, 32, 32));
            }
        }
    }
}
