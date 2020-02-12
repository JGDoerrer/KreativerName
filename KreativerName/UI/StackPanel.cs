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

            foreach (UIElement child in children)
            {
                if (Horizontal)
                {
                    sum += child.MarginUp;
                    child.Y = sum;
                    sum += child.Height + child.MarginDown;

                    switch (child.HorizontalAlign)
                    {
                        case Alignment.LeftOrTop:
                            child.X = child.MarginLeft;
                            child.Width = Width - child.MarginLeft - child.MarginRight;
                            break;
                        case Alignment.Center:
                            child.X = child.MarginLeft;
                            child.Width = (Width - child.MarginLeft - child.MarginRight) / 2;
                            break;
                        case Alignment.RightOrBottom:
                            child.X = child.MarginRight;
                            child.Width = Width - child.MarginLeft - child.MarginRight;
                            break;
                    }
                }
                else
                {
                    sum += child.MarginLeft;
                    child.X = sum;
                    sum += child.Width + child.MarginRight;

                    switch (child.VerticalAlign)
                    {
                        case Alignment.LeftOrTop:
                            child.Y = child.MarginUp;
                            child.Height = Height - child.MarginUp - child.MarginDown;
                            break;
                        case Alignment.Center:
                            child.Y = child.MarginUp;
                            child.Height = (Height - child.MarginUp - child.MarginDown) / 2;
                            break;
                        case Alignment.RightOrBottom:
                            child.Y = child.MarginDown;
                            child.Height = Height - child.MarginUp - child.MarginDown;
                            break;
                    }
                }
            }
        }

        public override void Render(Vector2 windowSize)
        {
        }
    }
}
