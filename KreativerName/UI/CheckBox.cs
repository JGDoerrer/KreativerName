using System.Drawing;
using KreativerName.Rendering;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    class CheckBox : UIElement
    {
        public CheckBox()
        {
        }
        public CheckBox(int x, int y, int w, int h) : base(x,y,w,h)
        {
        }

        public bool Checked { get; set; }
        public bool Enabled { get; set; } = true;
        public Key Shortcut { get; set; }

        bool clicked;
        bool mouseDown;

        public event ClickEvent OnClick;
        public event CheckEvent OnChecked;

        public override void Update(Vector2 windowSize)
        {
            UpdateChildren(windowSize);

            bool b = (MouseOver&& !mouseDown && MouseLeftDown) || ui.Input.KeyDown(Shortcut);
            if (Enabled && !clicked && b)
            {
                Checked = !Checked;

                OnClick?.Invoke(this);
                OnChecked?.Invoke(Checked);
            }

            clicked = b;

            mouseDown = MouseLeftDown;
        }

        public override void Render(Vector2 windowSize)
        {
            const int a = 12;
            
            float offset;
            Color color = Color.White;
            Texture2D tex = Textures.Get("CheckBox");

            Vector2 scale = new Vector2((float)Width / a, (float)Height / a);

            if (!Enabled || MouseOver)
            {
                color = Color.FromArgb(color.R / 4 * 3, color.B / 4 * 3, color.G / 4 * 3);
            }

            if (Checked)
                offset = a;
            else
                offset = 0;

            TextureRenderer.Draw(tex, ActualPosition, scale, color, new RectangleF(offset, 0, a, a));

            RenderChildren(windowSize);
        }
    }
}
