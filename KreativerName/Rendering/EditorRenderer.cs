using System;
using System.Drawing;
using System.Linq;
using KreativerName.Scenes;
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

        internal void Render(Vector2 windowSize)
        {
            if (editor == null)
                return;

            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            GL.ClearColor(Color.FromArgb(255, 0, 0, 0));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if (editor.Grid != null)
            {
                const int margin = 50;

                float maxX = editor.Grid.Max(x => x.Value.X + x.Value.Y / 2f);
                float minX = editor.Grid.Min(x => x.Value.X + x.Value.Y / 2f);
                int maxY = editor.Grid.Max(x => x.Value.Y);
                int minY = editor.Grid.Min(x => x.Value.Y);

                editor.layout.size = Math.Min((width - margin) / (sqrt3 * (maxX - minX + 1)), (height - margin) / (1.5f * (maxY - minY + 1.25f)));
                // Round to multiples of 16
                editor.layout.size = (float)Math.Floor(editor.layout.size / 16) * 16;
                editor.layout.size = Math.Min(editor.layout.size,48);

                int centerX = (int)(editor.layout.size * sqrt3 * (maxX + minX));
                int centerY = (int)(editor.layout.size * 1.5f * (maxY + minY));

                // Center grid
                editor.layout.origin = new Vector2((width - centerX) / 2, (height - centerY) / 2);

                //int totalWidth = (int)(editor.layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(editor.layout.size * 1.5f * (maxY - minY + 1.25f));
            }

            GridRenderer.RenderGrid(editor.Grid, editor.layout, editor.GetPlayerMoves(), editor.selectedHex, editor.player);

            editor.editorUi.Render(windowSize);
        }
    }
}
