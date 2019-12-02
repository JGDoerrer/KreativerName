using System;
using System.Collections.Generic;
using System.Drawing;
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
                    TextureRenderer.DrawHex(Textures.Get("Player"), hex.Position, Layout, Vector2.One * Layout.size, color, null);
                else
                {
                    for (int i = 0; i < hex.Types.Count; i++)
                    {
                        int animation = 0;
                        if (hex.Types[i].AnimationLength > 0 && hex.Types[i].AnimationSpeed > 0)
                        {
                            animation = ((frameCount + hex.Types[i].AnimationPhase) / hex.Types[i].AnimationSpeed) % hex.Types[i].AnimationLength;
                        }
                        TextureRenderer.DrawHex(Textures.Get("Hex"), hex.Position, Layout, Vector2.One * Layout.size, color, new RectangleF(32 * hex.Types[i].Texture, animation * 32, 32, 32));
                    }
                }
            }

            frameCount++;
        }

    }
}
