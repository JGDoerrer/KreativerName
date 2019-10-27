using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.UI
{
    public class Frame : UIElement
    {
        public override void Update(Vector2 windowSize)
        {
            foreach (var element in children)
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

            GL.Color4(new Color4(100, 100, 100, 255));

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();

            RenderChildren(windowSize);
        }
    }
}
