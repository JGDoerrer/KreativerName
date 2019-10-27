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

        Constraint xCon;
        Constraint yCon;
        Constraint widthCon;
        Constraint heightCon;

        public float GetX(Vector2 windowSize, UIElement element) => xCon.GetX(windowSize, element);
        public float GetY(Vector2 windowSize, UIElement element) => yCon.GetY(windowSize, element);
        public float GetWidth(Vector2 windowSize, UIElement element) => widthCon.GetWidth(windowSize, element);
        public float GetHeight(Vector2 windowSize, UIElement element) => heightCon.GetHeight(windowSize, element);
    }
}
