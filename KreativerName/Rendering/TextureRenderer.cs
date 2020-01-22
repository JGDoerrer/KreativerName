using System;
using System.Drawing;
using KreativerName.Grid;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    class TextureRenderer
    {
        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, Color color, RectangleF? sourceRect)
            => Draw(texture, position, scale, color, sourceRect, new Vector2(0, 0));

        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, Color color, RectangleF? sourceRect, Vector2 origin)
        {
            Vector2[] vertecies = new Vector2[4]
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1),
            };

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);

            for (int i = 0; i < 4; i++)
            {
                if (!sourceRect.HasValue)
                    GL.TexCoord2(vertecies[i]);
                else
                    GL.TexCoord2((sourceRect.Value.Left + vertecies[i].X * sourceRect.Value.Width) / texture.Width,
                       (sourceRect.Value.Top + vertecies[i].Y * sourceRect.Value.Height) / texture.Height);

                vertecies[i].X *= !sourceRect.HasValue ? texture.Width : sourceRect.Value.Width;
                vertecies[i].Y *= !sourceRect.HasValue ? texture.Height : sourceRect.Value.Height;

                vertecies[i] -= origin;
                vertecies[i] *= scale;
                vertecies[i] += position;

                vertecies[i].X = (float)Math.Floor(vertecies[i].X);
                vertecies[i].Y = (float)Math.Floor(vertecies[i].Y);

                GL.Vertex2(vertecies[i]);
            }

            GL.End();
        }

        public static void DrawHex(Texture2D texture, HexPoint hex, HexLayout layout, Vector2 scale, Color color, RectangleF? sourceRect)
        {
            Vector2[] vertecies = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                double angle = 2 * Math.PI * (layout.startAngle + i) / 6;
                vertecies[i] = new Vector2((float)Math.Cos(angle) + sqrt3 / 2, (float)Math.Sin(angle) + 1) / 2;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(color);

            for (int i = 0; i < 6; i++)
            {
                if (!sourceRect.HasValue)
                    GL.TexCoord2(vertecies[i]);
                else
                {
                    GL.TexCoord2(Math.Round((sourceRect.Value.Left + vertecies[i].X * sourceRect.Value.Width) * scale.X) / (texture.Width * scale.X),
                                Math.Round((sourceRect.Value.Top + vertecies[i].Y * sourceRect.Value.Height) * scale.Y) / (texture.Height * scale.Y));
                }

                vertecies[i] = layout.HexCorner((Vector2)hex * layout.spacing, i);

                vertecies[i].X = (float)Math.Floor(vertecies[i].X);
                vertecies[i].Y = (float)Math.Floor(vertecies[i].Y);

                GL.Vertex2(vertecies[i]);
            }

            GL.End();
        }
    }
}
