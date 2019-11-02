using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KreativerName.Grid;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    public class EditorRenderer
    {
        public EditorRenderer(Editor editor)
        {
            this.editor = editor;
        }

        Editor editor;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        internal void Render(int width, int height)
        {
            if (editor == null)
                return;

            GL.ClearColor(Color.FromArgb(255, 0, 0, 0));
            
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Center grid
            if (editor.Grid != null)
            {
                int totalWidth = (int)(editor.layout.size * sqrt3 * (editor.Grid.Max(x => x.X + x.Y / 2f) + editor.Grid.Min(x => x.X + x.Y / 2f)));
                int totalHeight = (int)(editor.layout.size * 1.5f * (editor.Grid.Max(x => x.Y) + editor.Grid.Min(x => x.Y)));

                editor.layout.origin = new Vector2((width - totalWidth) / 2, (height - totalHeight) / 2);
            }

            RenderGrid();

            editor.editorUi.Render(width, height);
        }

        private void RenderGrid()
        {
            if (editor.Grid == null)
                return;

            List<HexPoint> moves = editor.GetPlayerMoves();

            foreach (var hex in editor.Grid)
            {
                Vector2 renderPos = editor.layout.HexCorner(hex.Position, 3);
                renderPos.X -= editor.layout.size / 2f * sqrt3;
                renderPos.Y -= editor.layout.size / 2f;

                renderPos.X = (float)Math.Floor(renderPos.X);
                renderPos.Y = (float)Math.Floor(renderPos.Y);

                Color color = Color.White;

                if (editor.selectedHex == hex.Position)
                {
                    if (moves.Contains(hex.Position))
                        color = Color.FromArgb(255, 100, 200, 100);
                    else
                        color = Color.FromArgb(255, 200, 200, 200);
                }
                else if (moves.Contains(hex.Position))
                    color = Color.LightGreen;
                else
                    color = Color.White;

                if (hex.Position == editor.player)
                    TextureRenderer.DrawHex(Textures.Get("Player"), hex.Position, editor.layout, Vector2.One * editor.layout.size / 10, color, null);
                else
                    TextureRenderer.DrawHex(Textures.Get("Hex"), hex.Position, editor.layout, Vector2.One * editor.layout.size / 10, color, new RectangleF(32 * (int)hex.Type, 0, 32, 32));
                //TextureRenderer.Draw(Textures.Get("Hex"), renderPos, Vector2.One * game.layout.size / 10, color, new RectangleF(17*(int)hex.Type,0, 17, 19));

            }
        }
    }
}
