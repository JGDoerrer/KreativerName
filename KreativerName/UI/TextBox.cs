using System.Drawing;
using System.Text;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

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

        int cursorAnim;
        int frameCount = 0;
        const int repeatSpeed = 3;

        public override void Update(Vector2 windowSize)
        {
            if (MouseLeftClick && Enabled)
            {
                if (MouseOver(windowSize))
                {
                    Focused = true;
                }
                else if (Focused)
                {
                    ui.ignoreShortcuts = false;
                    Focused = false;
                }

                if (Focused)
                    cursorAnim = 0;
            }

            if (Focused && Enabled)
            {
                ui.ignoreShortcuts = true;

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

                if (KeyDownOrRepeat(Key.BackSpace) && Cursor > 0 && Text.Length > 0)
                {
                    Cursor--;
                    Text = Text.Remove(Cursor, 1);
                }
                if (KeyDownOrRepeat(Key.Delete) && Cursor < Text.Length && Text.Length > 0)
                {
                    Text = Text.Remove(Cursor, 1);
                }

                if (KeyDownOrRepeat(Key.Left) && Cursor > 0)
                {
                    Cursor--;
                    cursorAnim = 0;
                }
                if (KeyDownOrRepeat(Key.Right) && Cursor < Text.Length)
                {
                    Cursor++;
                    cursorAnim = 0;
                }

                if (ui.Input.KeyPress(Key.Escape))
                {
                    ui.ignoreShortcuts = Focused = false;
                    ui.Input.ReleaseKey(Key.Escape);
                }

                if (Cursor > Text.Length)
                    Cursor = Text.Length;

                if (Cursor > MaxTextSize - 1)
                    Cursor = MaxTextSize - 1;

                if (Cursor < 0)
                    Cursor = 0;
            }

            frameCount++;
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
            Color color;
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

            for (int i = 0; i <= 2; i++)
                for (int j = 0; j <= 2; j++)
                {
                    float x1 = x + new[] { 0, 1, -1 }[i] * a * scale + (i == 2 ? w : 0);
                    float y1 = y + new[] { 0, 1, -1 }[j] * a * scale + (j == 2 ? h : 0);

                    Vector2 scl = new Vector2(i == 1 ? w / (a * scale) - 2 : 1, j == 1 ? h / (a * scale) - 2 : 1) * scale;

                    TextureRenderer.Draw(tex, new Vector2(x1, y1), scl, color, new RectangleF(offset + a * (i), a * (j), a, a));
                }

            StringBuilder text = new StringBuilder(Text.PadRight(MaxTextSize));
            if ((cursorAnim / 30) % 2 == 0 && Focused)
                text[Cursor] = '_';

            TextRenderer.RenderString(text.ToString(), new Vector2(GetX(windowSize), GetY(windowSize)) + TextOffset, TextColor, w - TextOffset.X * 3, TextSize);

            RenderChildren(windowSize);
            cursorAnim++;
        }

        private bool KeyDownOrRepeat(Key key) => ui.Input.KeyPress(key) || (ui.Input.KeyRepeat(key) && frameCount % repeatSpeed == 0);
    }
}
