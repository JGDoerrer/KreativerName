using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace KreativerName.UI
{
    public class UI
    {
        public UI()
        {

        }

        List<UIElement> Elements { get; set; } = new List<UIElement>();
        internal Input Input { get; set; }

        internal Vector2 MousePosition => Input?.MousePosition ?? new Vector2();

        public void Update(Vector2 windowSize)
        {
            foreach (UIElement element in Elements)
            {
                element.Update(windowSize);
            }

            Input.Update();
        }

        public void Render(Vector2 windowSize)
        {
            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            GL.Viewport(0, 0, width, height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            foreach (var element in Elements)
            {
                if (element.Visible)
                    element.Render(windowSize);
            }
        }

        public void Add(UIElement element)
        {
            element.SetUI(this);
            Elements.Add(element);
        }
    }
}
