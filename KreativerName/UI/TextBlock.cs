using System.Drawing;
using System.Linq;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.UI
{
    public class TextBlock : UIElement
    {
        public TextBlock()
        {
        }
        public TextBlock(string text, float size = 2)
        {
            Text = text;
            Size = size;
        }
        public TextBlock(string text, float size, int x, int y)
        {
            Text = text;
            Size = size;
            constraints = new UIConstraints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint((int)TextWidth),
                new PixelConstraint((int)TextHeight));
        }

        public string Text { get; set; }//.ToUpper(); }
        public float Size { get; set; }
        public Color Color { get; set; } = Color.Black;
        public float TextWidth => Text.Sum(x => !char.IsUpper(x) ? Size * 6 : Size * 7);
        public float TextHeight => Size * 6 + Text.Count(x => x == '\n') * Size * 8;

        public override void Update(Vector2 windowSize)
        {

        }

        public override void Render(Vector2 windowSize)
        {
            Vector2 pos = new Vector2(GetX(windowSize), GetY(windowSize));

            RenderString(Text, pos, Color, Size);
        }

        public static void RenderChar(char c, Vector2 position, Color color, float size = 2)
        {
            Texture2D tex = Textures.Get("Font");
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

                if (c <= 255 && c >= 32)
                {
                    if (!char.IsUpper(c))
                        position.X -= 1 * size;

                    RenderChar(c, position, color, size);
                    position.X += 7 * size;
                }
            }
        }
    }
}
