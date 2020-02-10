using System.Drawing;
using KreativerName.Rendering;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    public class Button : UIElement
    {
        public Button()
        { }
        public Button(int x, int y, int w, int h) : base(x,y,w,h)
        { }

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

            if (Enabled && (!clicked && MouseOver&& MouseLeftDown && !mouseDown ||
                (ui.Input.KeyPress(Shortcut) && !ui.ignoreShortcuts)))
            {
                OnLeftClick?.Invoke(this);
            }
            if (Enabled && MouseOver&& MouseRightClick)
            {
                OnRightClick?.Invoke(this);
            }

            if (ChangeState)
            {
                if (MouseOver)
                {
                    if (mouseDown)
                        State = 2;
                    else
                        State = 1;
                }
                else
                    State = 0;
            }

            clicked = (MouseOver&& !mouseDown && MouseLeftDown) || (ui.Input.KeyPress(Shortcut) && !ui.ignoreShortcuts);

            mouseDown = MouseLeftDown;
        }

        public override void Render(Vector2 windowSize)
        {
            const int a = 8;
            const float scale = 2;


            float state = State * a * 3;
            float style = Style * a * 3;
            Color color = Color;
            Texture2D tex = Textures.Get("Button");

            if (!Enabled)
            {
                color = Color.FromArgb(Color.R / 2, Color.B / 2, Color.G / 2);
            }

            float[] xs = { ActualX, ActualX + a * scale, ActualX + Width - a * scale };
            float[] ys = { ActualY, ActualY + a * scale, ActualY + Height - a * scale };

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    Vector2 scl = new Vector2(i == 1 ? Width / (a * scale) - 2 : 1,
                                              j == 1 ? Height / (a * scale) - 2 : 1) * scale;

                    TextureRenderer.Draw(tex, new Vector2(xs[i], ys[j]), scl, color, new RectangleF(state + a * i, style + a * j, a, a));
                }
            }

            RenderChildren(windowSize);
        }
    }
}
