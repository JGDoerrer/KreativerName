using System.Drawing;
using OpenTK;

namespace KreativerName.Rendering
{
    public static class TextRenderer
    {
        static Texture2D tex = Textures.Get("Font");

        public static void RenderChar(char c, Vector2 position, Color color, float size = 2)
        {
            RectangleF sourceRect = new RectangleF(((c - 32) % 16) * 6, ((c - 32) / 16) * 6, 6, 6);
            TextureRenderer.Draw(tex, position, Vector2.One * size, color, sourceRect);
        }

        public static void RenderString(string s, Vector2 position, Color color, float size = 2)
        {
            if (s == null)
                return;

            float startX = position.X;

            foreach (char c in s)
            {
                if (c == '\n')
                {
                    position.Y += 8 * size;
                    position.X = startX;
                }

                if (c <= 287 && c >= 32)
                {
                    if (!char.IsUpper(c))
                        position.X -= 1 * size;

                    RenderChar(c, position, color, size);
                    position.X += 7 * size;
                }
            }
        }

        public static float GetWidth(string s, float size = 2)
        {
            float x = 0;
            float maxX = 0;

            foreach (char c in s)
            {
                if (c == '\n')
                    x = 0;

                if (c <= 255 && c >= 32)
                {
                    if (!char.IsUpper(c))
                        x -= 1 * size;
                    x += 7 * size;
                }

                if (x > maxX)
                    maxX = x;
            }

            return maxX;
        }

        public static float GetHeight(string s, float size = 2)
        {
            float y = 8 * size;

            foreach (char c in s)
            {
                if (c == '\n')
                    y += 8 * size;
            }

            return y;
        }
    }
}
