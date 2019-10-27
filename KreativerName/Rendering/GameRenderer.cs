﻿using KreativerName.Grid;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    public class GameRenderer
    {
        public GameRenderer(Game game)
        {
            this.game = game;

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        Game game;

        Color[] teamColors = new[]
        {
            Color.Blue,
            Color.Red,
        };

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        internal void Render(int width, int height)
        {
            game.ui.Render(width, height);

            GL.Enable(EnableCap.Blend);

            GL.ClearColor(Color.FromArgb(255, 50, 50, 50));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if (game.Grid == null)
                return;

            List<HexPoint> moves = game.GetMoves(game.selectedHex, game.currentTeam);

            int totalWidth = (int)(40 * sqrt3 * (game.Grid.Max(x => x.X + x.Y / 2f) - game.Grid.Min(x => x.X + x.Y / 2f)));
            int totalHeight = (int)(40 * 1.5f * (game.Grid.Max(x => x.Y) - game.Grid.Min(x => x.Y)));

            game.layout.origin = new Vector2((width - totalWidth) / 2, (height - totalHeight) / 2);

            foreach (var hex in game.Grid)
            {
                Vector2 renderPos = game.layout.HexCorner(hex, 3);
                renderPos.X -= game.layout.size / 2f * sqrt3;
                renderPos.Y -= game.layout.size / 2f;

                renderPos.X = (float)Math.Floor(renderPos.X);
                renderPos.Y = (float)Math.Floor(renderPos.Y);

                Color color = Color.White;

                if (game.selectedHex == hex.Position)
                    color = Color.FromArgb(255, 100, 100, 100);
                else if (moves.Contains(hex.Position))
                    color = Color.FromArgb(255, 200, 100, 100);
                else
                    color = Color.FromArgb(255, 200, 200, 200);

                TextureRenderer.Draw(Textures.Get("Hex"), renderPos, Vector2.One * game.layout.size / 100f, color, null);

                if (hex.Troop.HasValue)
                {
                    Color teamColor = hex.Troop.Value.Team < teamColors.Length ? teamColors[hex.Troop.Value.Team] : Color.White;
                    TextureRenderer.Draw(Textures.Get(Enum.GetName(typeof(TroopType), hex.Troop.Value.Type)), renderPos, Vector2.One * game.layout.size / 100f, teamColor, null);
                }
            }
        }
    }
}