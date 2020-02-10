using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.UI
{
    public class Frame : UIElement
    {
        public Frame()
        { }
        public Frame(int x, int y, int w, int h) : base(x, y, w, h)
        { }

        public Color Color { get; set; } = Color.FromArgb(100, 100, 100);

        public override void Update(Vector2 windowSize)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                UIElement element = children[i];
                element.Update(windowSize);
            }
        }

        public override void Render(Vector2 windowSize)
        {
            float x1 = ActualX;
            float y1 = ActualY;
            float x2 = x1 + Width;
            float y2 = y1 + Height;

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
