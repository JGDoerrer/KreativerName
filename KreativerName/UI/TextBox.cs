using System.Drawing;
using System.Text;
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
        public Color TextColor { get; set; } = Color.Black;
        public Vector2 TextOffset { get; set; } = new Vector2(8, 8);
        public float TextSize { get; set; } = 2;
        public int MaxTextSize { get; set; } = 5;
        public bool Enabled { get; set; } = true;

        private int cursorAnim;

        public override void Update(Vector2 windowSize)
        {
            if (MouseLeftClick && Enabled)
            {
                ui.ignoreShortcuts = Focused = MouseOver(windowSize);
                if (Focused)
                    cursorAnim = 0;
            }

            if (Focused && Enabled)
            {
                if (Cursor > Text.Length)
                    Cursor = Text.Length;

                if (Cursor > MaxTextSize - 1)
                    Cursor = MaxTextSize - 1;

                if (Cursor < 0)
                    Cursor = 0;

                Text = Text.Insert(Cursor, ui.Input.KeyString);

                Cursor += ui.Input.KeyString.Length;

                if (Text.Length > MaxTextSize)
                    Text = Text.Remove(MaxTextSize, Text.Length - MaxTextSize);
                
                if (ui.Input.KeyPress(OpenTK.Input.Key.BackSpace) && Cursor > 0 && Text.Length > 0)
                {
                    Cursor--;
                    Text = Text.Remove(Cursor, 1);
                }
                if (ui.Input.KeyPress(OpenTK.Input.Key.Delete) && Cursor < Text.Length && Text.Length > 0)
                {
                    Text = Text.Remove(Cursor, 1);
                }

                if (ui.Input.KeyPress(OpenTK.Input.Key.Left) && Cursor > 0)
                {
                    Cursor--;
                    cursorAnim = 0;
                }
                if (ui.Input.KeyPress(OpenTK.Input.Key.Right) && Cursor < Text.Length)
                {
                    Cursor++;
                    cursorAnim = 0;
                }

                if (ui.Input.KeyPress(OpenTK.Input.Key.Escape))
                {
                    ui.ignoreShortcuts = Focused = false;
                    ui.Input.ReleaseKey(OpenTK.Input.Key.Escape);
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

            float offset = 0;
            Color color = Color.White;
            Texture2D tex = Textures.Get("TextBox");

            if (MouseOver(windowSize) || Focused)
            {
                if (MouseLeftDown)
                    color = Color.FromArgb(100, 100, 100);
                else
                    color = Color.FromArgb(150, 150, 150);
            }
            else
                color = Color.FromArgb(255, 255, 255);

            if (!Enabled)
            {
                color = Color.FromArgb(color.R / 2, color.B / 2, color.G / 2);
            }

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

            StringBuilder text = new StringBuilder(Text.PadRight(MaxTextSize));
            if ((cursorAnim / 30) % 2 == 0 && Focused)
                text[Cursor] = '_';

            TextRenderer.RenderString(text.ToString(), new Vector2(GetX(windowSize), GetY(windowSize)) + TextOffset, TextColor, TextSize);

            RenderChildren(windowSize);
            cursorAnim++;
        }
    }
}
