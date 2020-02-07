using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class UIConstraints
    {
        public UIConstraints() { }
        public UIConstraints(Constraint x, Constraint y, Constraint width, Constraint height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public UIConstraints(int x, int y, int width, int height)
        {
            this.x = new PixelConstraint(x);
            this.y = new PixelConstraint(y);
            this.width = new PixelConstraint(width);
            this.height = new PixelConstraint(height);
        }

        public Constraint x;
        public Constraint y;
        public Constraint width;
        public Constraint height;

        public float GetX(Vector2 windowSize, UIElement element) => x.GetX(windowSize, element);
        public float GetY(Vector2 windowSize, UIElement element) => y.GetY(windowSize, element);
        public float GetWidth(Vector2 windowSize, UIElement element) => width.GetWidth(windowSize, element);
        public float GetHeight(Vector2 windowSize, UIElement element) => height.GetHeight(windowSize, element);

        public static UIConstraints FullWindow =>
            new UIConstraints(new PixelConstraint(0), new PixelConstraint(0), new RelativeConstraint(1, RelativeTo.Window), new RelativeConstraint(1, RelativeTo.Window));
    }

    public enum RelativeTo
    {
        Window,
        Parent,
        Self,
    }

    public enum Direction
    {
        Left,
        Top,
        Right,
        Bottom,
    }
}
