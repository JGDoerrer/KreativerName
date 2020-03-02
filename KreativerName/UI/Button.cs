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
                OnLeftClick?.Invoke(this);
            }
            if (Enabled && MouseOver(windowSize) && MouseRightClick)
            {
                OnRightClick?.Invoke(this);
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
            if (model == null)
                BuildMesh(windowSize);

            Color color = Color;
            if (!Enabled)
            {
                color = Color.FromArgb(Color.R / 2, Color.B / 2, Color.G / 2);
            }

            Matrix4 matrix = Matrix4.Identity;

            matrix.M11 = 2 / windowSize.X; // scale
            matrix.M22 = -2 / windowSize.Y;

            matrix.M14 = -1 + GetX(windowSize)*2 / windowSize.X;
            matrix.M24 = 1 - GetY(windowSize)*2 / windowSize.Y;

            model.Info.Shader.SetMatrix4("transform", matrix);
            model.Info.Shader.SetColor("inColor", color);

            Renderer.Render(model.Info);

            RenderChildren(windowSize);
        }

        private void BuildMesh(Vector2 windowSize)
        {
            MeshBuilder builder = new MeshBuilder();

            const int a = 8;
            const float scale = 2;

            float x = GetX(windowSize);
            float y = GetY(windowSize);
            float w = GetWidth(windowSize);
            float h = GetHeight(windowSize);

            float state = State * a * 3;
            float style = Style * a * 3;
            Texture2D tex = Textures.Get("Button");


            float[] xs = { 0, a * scale, w - a * scale };
            float[] ys = { 0, a * scale, h - a * scale };

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    Vector2 scl = new Vector2(i == 1 ? w / (a * scale) - 2 : 1,
                                              j == 1 ? h / (a * scale) - 2 : 1) * scale;

                    builder.AddRectangle(new RectangleF(xs[i], ys[j], a*scl.X, a*scl.Y), new RectangleF((state + a * i) / tex.Width, (style + a * j) / tex.Height, a / (float)tex.Width, a / (float)tex.Height));

                    //TextureRenderer.Draw(tex, new Vector2(xs[i], ys[j]), scl, color, new RectangleF(state + a * i, style + a * j, a, a));
                }
            }

            model = new Model(builder.Mesh, tex, Shaders.Get("Basic"));
        }
    }
}
