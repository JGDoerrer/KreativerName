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
            constaints = new UIConstaints();
        }
        public Button(int x, int y, int w, int h)
        {
            constaints = new UIConstaints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint(w),
                new PixelConstraint(h));
        }

        bool clicked;
        bool mouseDown;

        public Color Color { get; set; } = Color.White;
        public bool Clicked => clicked;
        public event ButtonClickEvent OnClick;

        public Key Shortcut { get; set; }

        public override void Update(Vector2 windowSize)
        {
            UpdateChildren(windowSize);

            if ((!clicked && MouseOver(windowSize) && !mouseDown && MouseLeftDown) || (ui.Input.KeyDown(Shortcut) && !clicked))
            {
                OnClick?.Invoke();
            }

            clicked = (MouseOver(windowSize) && MouseLeftDown) || ui.Input.KeyDown(Shortcut);
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

            float offset;
            Color color = Color;
            Texture2D tex = Textures.Get("Button");

            if (MouseOver(windowSize))
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
            TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + a * scale), new Vector2(w / (a * scale) - 2 , h / (a * scale) - 2) * scale, color, new RectangleF(offset + a, a, a, a));
            
            RenderChildren(windowSize);
        }
    }
}
