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
                OnLeftClick?.Invoke();
            }
            if (Enabled && MouseOver(windowSize) && MouseRightClick)
            {
                OnRightClick?.Invoke();
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

            // corner top left
            TextureRenderer.Draw(tex, new Vector2(x, y), Vector2.One * scale, color, new RectangleF(state, style, a, a));
            // corner top right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y), Vector2.One * scale, color, new RectangleF(state + a * 2, style, a, a));
            // corner bottom left
            TextureRenderer.Draw(tex, new Vector2(x, y + h - a * scale), Vector2.One * scale, color, new RectangleF(state, style + a * 2, a, a));
            // corner bottom right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + h - a * scale), Vector2.One * scale, color, new RectangleF(state + a * 2, style + a * 2, a, a));
            // left
            TextureRenderer.Draw(tex, new Vector2(x, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(state, style + a, a, a));
            // top
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(state + a, style, a, a));
            // right
            TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(state + a * 2, style + a, a, a));
            // bottom
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + h - a * scale), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(state + a, style + a * 2, a, a));
            // center
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + a * scale), new Vector2(w / (a * scale) - 2, h / (a * scale) - 2) * scale, color, new RectangleF(state + a, style + a, a, a));

            RenderChildren(windowSize);
        }
    }
}
