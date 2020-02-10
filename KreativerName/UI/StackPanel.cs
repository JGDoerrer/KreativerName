using OpenTK;

namespace KreativerName.UI
{
    public class StackPanel : UIElement
    {
        public StackPanel() { }
        public StackPanel(int x, int y, int w, int h) : base(x, y, w, h)
        { }

        public bool Horizontal { get; set; } = true;

        public override void Update(Vector2 windowSize)
        {
            int sum = 0;

            foreach (var child in children)
            {
                if (child.HorizontalAlign == Alignment.LeftOrTop)
                {
                    if (Horizontal)
                    {
                        child.X = child.MarginLeft;
                    }
                    else
                    {
                        sum += child.MarginLeft;

                        child.X = sum;

                        sum += child.Width + child.MarginRight;
                    }
                }
                else if (child.HorizontalAlign == Alignment.Center)
                {
                    if (Horizontal)
                    {
                        child.X = child.MarginLeft;
                        child.Width = (Width - child.MarginLeft - child.MarginRight) / 2;
                    }
                    else
                    {
                        sum += child.MarginLeft;

                        child.X = sum;

                        sum += child.Width + child.MarginRight;
                    }
                }

                if (child.VerticalAlign == Alignment.LeftOrTop)
                {
                    if (Horizontal)
                    {
                        sum += child.MarginUp;

                        child.Y = sum;

                        sum += child.Height + child.MarginDown;
                    }
                    else
                    {
                        child.Y = child.MarginUp;
                    }
                }
                else if (child.VerticalAlign == Alignment.Center)
                {
                    if (Horizontal)
                    {
                        sum += child.MarginUp;

                        child.Y = sum;

                        sum += child.Width + child.MarginDown;
                    }
                    else
                    {
                        child.Y = child.MarginUp;
                        child.Height = (Width - child.MarginUp - child.MarginDown) / 2;
                    }
                }
            }
        }

        public override void Render(Vector2 windowSize)
        {
        }
    }
}
