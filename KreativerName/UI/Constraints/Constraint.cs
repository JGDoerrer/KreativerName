using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace KreativerName.UI.Constraints
{
    public abstract class Constraint
    {
        public abstract float GetX(Vector2 windowSize, UIElement element);
        public abstract float GetY(Vector2 windowSize, UIElement element);
        public abstract float GetWidth(Vector2 windowSize , UIElement element);
        public abstract float GetHeight(Vector2 windowSize, UIElement element);
    }
}
