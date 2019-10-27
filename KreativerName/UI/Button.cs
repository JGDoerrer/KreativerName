using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.UI
{
    public class Button : UIElement
    {
        bool clicked;
        bool mouseDown;

        public bool Clicked => clicked;
        public event ButtonClickEvent OnClicked;

        public override void Update(Vector2 windowSize)
        {
            if (!clicked && MouseOver(windowSize) && !mouseDown && MouseLeftDown)
            {
                OnClicked?.Invoke();
            }
            clicked = MouseOver(windowSize) && MouseLeftDown;
            mouseDown = MouseLeftDown;
        }

        public override void Render(Vector2 windowSize)
        {
            float x1 = GetX(windowSize);
            float y1 = GetY(windowSize);
            float x2 = x1 + GetWidth(windowSize);
            float y2 = y1 + GetHeight(windowSize);

            if (MouseOver(windowSize))
            {
                if (MouseLeftDown)
                    GL.Color4(new Color4(255, 0, 0, 255));
                else
                    GL.Color4(new Color4(0, 255, 0, 255));
            }
            else
                GL.Color4(new Color4(0, 0, 255, 255));

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
