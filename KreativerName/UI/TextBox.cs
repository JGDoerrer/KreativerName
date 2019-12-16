using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.UI
{
    public class TextBox : UIElement
    {
        public TextBox()
        { }
        public TextBox(int x, int y, int w, int h)
        {
            constraints = new UIConstraints(x, y, w, h);
        }

        public bool Focused { get; set; }
        public string Text { get; set; } = "";
        public int Cursor { get; set; } = 0;
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; } = new Vector2(8, 8);
        public float TextSize { get; set; } = 2;
        public int MaxTextSize { get; set; } = 5;

        public override void Update(Vector2 windowSize)
        {
            if (MouseLeftClick)
            {
                Focused = MouseOver(windowSize);
            }

            if (Focused)
            {
                Text = Text.Insert(Cursor, ui.Input.KeyString);

                Cursor += ui.Input.KeyString.Length;

                if (Text.Length > MaxTextSize)
                    Text = Text.Remove(MaxTextSize, Text.Length - MaxTextSize);

                if (Cursor > MaxTextSize)
                    Cursor = MaxTextSize;

                if (ui.Input.KeyPress(OpenTK.Input.Key.BackSpace) && Cursor > 0)
                {
                    Cursor--;
                    Text = Text.Remove(Cursor, 1);
                }
            }
        }

        public override void Render(Vector2 windowSize)
        {
            const int a = 8;
            const float scale = 2;

            float x = GetX(windowSize);
            float y = GetY(windowSize);
            float w = GetWidth(windowSize);
            float h = GetHeight(windowSize);

            float offset;
            Color color = Color.White;
            Texture2D tex = Textures.Get("Button");

            if (MouseOver(windowSize) || Focused)
            {
                if (MouseLeftDown)
                    offset = a * 3 * 2;
                else
                    offset = a * 3;
            }
            else
                offset = 0;

            // corner top left
            TextureRenderer.Draw(tex, new Vector2(x, y), Vector2.One * scale, color, new RectangleF(offset, 0, a, a));
            // corner top right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y), Vector2.One * scale, color, new RectangleF(offset + a * 2, 0, a, a));
            // corner bottom left
            TextureRenderer.Draw(tex, new Vector2(x, y + h - a * scale), Vector2.One * scale, color, new RectangleF(offset, a * 2, a, a));
            // corner bottom right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + h - a * scale), Vector2.One * scale, color, new RectangleF(offset + a * 2, a * 2, a, a));
            // left
            TextureRenderer.Draw(tex, new Vector2(x, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(offset, a, a, a));
            // top
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(offset + a, 0, a, a));
            // right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(offset + a * 2, a, a, a));
            // bottom
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + h - a * scale), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(offset + a, a * 2, a, a));
            // center
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + a * scale), new Vector2(w / (a * scale) - 2, h / (a * scale) - 2) * scale, color, new RectangleF(offset + a, a, a, a));

            TextBlock.RenderString(Text, new Vector2(GetX(windowSize), GetY(windowSize)) + TextOffset, TextColor, TextSize);

            RenderChildren(windowSize);
        }
    }
}
