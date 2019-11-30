using System.Drawing;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.UI
{
    public class Frame : UIElement
    {
        public Frame()
        {
            constaints = new UIConstaints();
        }
        public Frame(int x, int y, int w, int h)
        {
            constaints = new UIConstaints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint(w),
                new PixelConstraint(h));
        }

        public Color Color { get; set; } = Color.FromArgb(100, 100, 100);

        public override void Update(Vector2 windowSize)
        {
            foreach (UIElement element in children)
            {
                element.Update(windowSize);
            }
        }

        public override void Render(Vector2 windowSize)
        {
            float x1 = GetX(windowSize);
            float y1 = GetY(windowSize);
            float x2 = x1 + GetWidth(windowSize);
            float y2 = y1 + GetHeight(windowSize);

            GL.Disable(EnableCap.Texture2D);

            GL.Color4(Color);

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();
            GL.Enable(EnableCap.Texture2D);

            RenderChildren(windowSize);
        }
    }
}
