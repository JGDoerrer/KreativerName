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
        List<Model> models = new List<Model>();

        public void Render(Player player, HexPoint selectedHex, List<HexPoint> moves)
        {
            if (Grid == null)
                return;

            foreach (Model model in models)
            {
                Renderer.Render(model.Info);
            }

            //foreach (Hex hex in Grid)
            //{
            //    Vector2 renderPos = Layout.HexCorner(hex.Position, 3);
            //    renderPos.X -= Layout.size / 2f * sqrt3;
            //    renderPos.Y -= Layout.size / 2f;

            //    renderPos.X = (float)Math.Floor(renderPos.X);
            //    renderPos.Y = (float)Math.Floor(renderPos.Y);

            //    Color mask;

            //    if (selectedHex == hex.Position)
            //    {
            //        if (moves != null && moves.Contains(hex.Position))
            //            mask = Color.FromArgb(80, 0, 100, 0);
            //        else
            //            mask = Color.FromArgb(50, Color.Black);
            //    }
            //    else if (moves != null && moves.Contains(hex.Position))
            //        mask = Color.FromArgb(80, 0, 200, 0);
            //    else
            //        mask = Color.Transparent;

            //    RenderHex(hex.Position, hex.GetTypes(Data), Layout, Color.White, frameCount, Grid);

            //    if (mask.A > 0)
            //        TextureRenderer.DrawHex(Textures.Get("Hex\\Mask"), hex.Position, Layout, Vector2.One * Layout.size, mask, null);

            //    if (hex.Position == player.Position)
            //        TextureRenderer.DrawHex(Textures.Get("Player"), hex.Position, Layout, Vector2.One * Layout.size, player.Color, null);
            //}

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

                TextureRenderer.DrawHex(Textures.Get($"Hex\\{types[i].Texture:000}"), pos, layout, Vector2.One * layout.size, color, new RectangleF(32 * connection, animation * 32, 32, 32));
            }
        }

        public void BuildMesh()
        {
            foreach (Model model in models)
            {
                model.Dispose();
            }

            models = new List<Model>();

            for (int i = 0; i < Data.Length; i++)
            {
                List<float> vertecies = new List<float>();
                List<float> texCoords = new List<float>();
                List<uint> indices = new List<uint>();

                Texture2D texture = Textures.Get($"Hex\\{Data[i].Texture:000}");

                uint hexCount = 0;

                for (uint n = 0; n < Grid.Count; n++)
                {
                    Hex hex = Grid.ElementAt((int)n).Value;

                    if (!hex.IDs.Contains(Data[i].ID))
                        continue;

                    Vector2[] hexVertecies = new Vector2[6];
                    Vector2[] hexTexCoords = new Vector2[6];

                    uint[] hexIndices =
                    {
                        0, 1, 2,
                        0, 2, 3,
                        0, 3, 5,
                        3, 4, 5
                    };

                    const int texSize = 32;

                    int connection = 0;
                    int animation = 0;

                    for (int j = 0; j < 6; j++)
                    {
                        double angle = 2 * Math.PI * (Layout.startAngle + j) / 6;
                        hexVertecies[j] = new Vector2((float)Math.Cos(angle) + sqrt3 / 2, (float)Math.Sin(angle) + 1) / 2;
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        //hexTexCoords[j] = new Vector2(
                        //    (32 * connection + hexVertecies[j].X * texSize) / (texture.Width),
                        //    (32 * animation + hexVertecies[j].Y * texSize) / (texture.Height)
                        //);

                        hexTexCoords[j] = new Vector2(
                            (float)Math.Round((32 * connection + hexVertecies[j].X * texSize) * Layout.size) / (texture.Width * Layout.size),
                            (float)Math.Round((32 * animation + hexVertecies[j].Y * texSize) * Layout.size) / (texture.Height * Layout.size));

                        hexVertecies[j] = Layout.HexCorner((Vector2)hex.Position * Layout.spacing, j);

                        hexVertecies[j].X = hexVertecies[j].X / (16f * 80f) * 2 - 1;
                        hexVertecies[j].Y = (1 - (hexVertecies[j].Y / (9f * 80f))) * 2 - 1;
                    }

                    foreach (Vector2 vertex in hexVertecies)
                    {
                        vertecies.Add(vertex.X);
                        vertecies.Add(vertex.Y);
                        vertecies.Add(0);
                    }

                    foreach (Vector2 texCoord in hexTexCoords)
                    {
                        texCoords.Add(texCoord.X);
                        texCoords.Add(texCoord.Y);
                    }

                    foreach (uint index in hexIndices)
                    {
                        indices.Add(index + hexCount * 6);
                    }

                    hexCount++;
                }

                Mesh mesh = new Mesh(vertecies.ToArray(), texCoords.ToArray(), indices.ToArray());
                Model model = new Model(mesh, texture, Shaders.Get("shader"));
                models.Add(model);
            }
        }
    }
}
