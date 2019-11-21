using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace KreativerName.UI.Constraints
{
    public class UIConstaints
    {
        public UIConstaints() { }
        public UIConstaints(Constraint x, Constraint y, Constraint width, Constraint height)
        {
            xCon = x;
            yCon = y;
            widthCon = width;
            heightCon = height;
        }
        public UIConstaints(int x, int y, int width, int height)
        {
            xCon = new PixelConstraint(x);
            yCon = new PixelConstraint(y);
            widthCon = new PixelConstraint(width);
            heightCon = new PixelConstraint(height);
        }

        Constraint xCon;
        Constraint yCon;
        Constraint widthCon;
        Constraint heightCon;

        public float GetX(Vector2 windowSize, UIElement element) => xCon.GetX(windowSize, element);
        public float GetY(Vector2 windowSize, UIElement element) => yCon.GetY(windowSize, element);
        public float GetWidth(Vector2 windowSize, UIElement element) => widthCon.GetWidth(windowSize, element);
        public float GetHeight(Vector2 windowSize, UIElement element) => heightCon.GetHeight(windowSize, element);

        public static UIConstaints FullWindow => 
            new UIConstaints(new PixelConstraint(0), new PixelConstraint(0), new RelativeConstraint(1, RelativeTo.Window), new RelativeConstraint(1, RelativeTo.Window));
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
