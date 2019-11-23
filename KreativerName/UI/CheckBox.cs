using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    class CheckBox : UIElement
    {
        public CheckBox()
        {
            constaints = new UIConstaints();
        }
        public CheckBox(int x, int y, int w, int h)
        {
            constaints = new UIConstaints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint(w),
                new PixelConstraint(h));
        }

        public bool Checked { get; set; }
        public bool Enabled { get; set; } = true;
        public Key Shortcut { get; set; }

        bool clicked;
        bool mouseDown;

        public event ClickEvent OnClick;

        public override void Update(Vector2 windowSize)
        {
            UpdateChildren(windowSize);

            bool b = (MouseOver(windowSize) && !mouseDown && MouseLeftDown) || ui.Input.KeyDown(Shortcut);
            if (Enabled && !clicked && b)
            {
                Checked = !Checked;

                OnClick?.Invoke();
            }

            clicked = b;

            mouseDown = MouseLeftDown;
        }

        public override void Render(Vector2 windowSize)
        {
            const int a = 12;

            float x = GetX(windowSize);
            float y = GetY(windowSize);
            float w = GetWidth(windowSize);
            float h = GetHeight(windowSize);

            float offset;
            Color color = Color.White;
            Texture2D tex = Textures.Get("CheckBox");

            Vector2 scale = new Vector2(w / a, h / a);

            if (!Enabled || MouseOver(windowSize))
            {
                color = Color.FromArgb(color.R / 4 * 3, color.B / 4 * 3, color.G / 4 * 3);
            }

            if (Checked)
                offset = a;
            else
                offset = 0;

            TextureRenderer.Draw(tex, new Vector2(x, y), scale, color, new RectangleF(offset, 0, a, a));

            RenderChildren(windowSize);
        }
    }
}
