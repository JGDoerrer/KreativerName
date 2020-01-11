using System;
using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class UIConstraints
    {
        public UIConstraints() { }
        public UIConstraints(Constraint x, Constraint y, Constraint width, Constraint height)
        {
            xCon = x;
            yCon = y;
            widthCon = width;
            heightCon = height;
        }
        public UIConstraints(int x, int y, int width, int height)
        {
            xCon = new PixelConstraint(x);
            yCon = new PixelConstraint(y);
            widthCon = new PixelConstraint(width);
            heightCon = new PixelConstraint(height);
        }

        public Constraint xCon;
        public Constraint yCon;
        public Constraint widthCon;
        public Constraint heightCon;

        public float GetX(Vector2 windowSize, UIElement element) => xCon.GetX(windowSize, element);
        public float GetY(Vector2 windowSize, UIElement element) => yCon.GetY(windowSize, element);
        public float GetWidth(Vector2 windowSize, UIElement element) => widthCon.GetWidth(windowSize, element);
        public float GetHeight(Vector2 windowSize, UIElement element) => heightCon.GetHeight(windowSize, element);

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
