using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    public class Button : UIElement
    {
        public Button()
        {
            constraints = new UIConstraints();

        }
        public Button(int x, int y, int w, int h)
        {
            constraints = new UIConstraints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint(w),
                new PixelConstraint(h));
        }

        bool mouseDown;
        bool clicked;

        public Color Color { get; set; } = Color.White;
        public Key Shortcut { get; set; }
        public bool Enabled { get; set; } = true;
        public int Style { get; set; }
        public int State { get; set; }
        public bool ChangeState { get; set; } = true;

        public event ClickEvent OnLeftClick;
        public event ClickEvent OnRightClick;

        public override void Update(Vector2 windowSize)
        {
            UpdateChildren(windowSize);

            if (Enabled && (!clicked && MouseOver(windowSize) && MouseLeftDown && !mouseDown ||
                (ui.Input.KeyPress(Shortcut) && !ui.ignoreShortcuts)))
            {
                OnLeftClick?.Invoke(this);
            }
            if (Enabled && MouseOver(windowSize) && MouseRightClick)
            {
                OnRightClick?.Invoke(this);
            }

            if (ChangeState)
            {
                if (MouseOver(windowSize))
                {
                    if (mouseDown)
                        State = 2;
                    else
                        State = 1;
                }
                else
                    State = 0;
            }

            clicked = (MouseOver(windowSize) && !mouseDown && MouseLeftDown) || (ui.Input.KeyPress(Shortcut) && !ui.ignoreShortcuts);

            mouseDown = MouseLeftDown;
        }

        public override void Render(Vector2 windowSize)
        {
            const int a = 8;
            const float scale = 2;

            float x = GetX(windowSize);
            float y = GetY(windowSize);
            float w = GetWidth(windowSize);
            float h = GetHeight(windowSize);

            float state = State * a * 3;
            float style = Style * a * 3;
            Color color = Color;
            Texture2D tex = Textures.Get("Button");

            if (!Enabled)
            {
                color = Color.FromArgb(Color.R / 2, Color.B / 2, Color.G / 2);
            }

            float[] xs = { x, x + a * scale, x + w - a * scale };
            float[] ys = { y, y + a * scale, y + h - a * scale };

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    Vector2 scl = new Vector2(i == 1 ? w / (a * scale) - 2 : 1,
                                              j == 1 ? h / (a * scale) - 2 : 1) * scale;

                    TextureRenderer.Draw(tex, new Vector2(xs[i], ys[j]), scl, color, new RectangleF(state + a * i, style + a * j, a, a));
                }
            }

            RenderChildren(windowSize);
        }
    }
}
