using KreativerName.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace KreativerName.UI.Constraints
{
    public class CenterConstraint : Constraint
    {
        public CenterConstraint()
        { }
        public CenterConstraint(float offset)
        {
            this.offset = offset;
        }

        float offset;

        public override float GetX(Vector2 windowSize, UIElement element)
        {
            float width = element.HasParent ? element.parent.GetWidth(windowSize) : windowSize.X;
            float x = element.HasParent ? element.parent.GetX(windowSize) : 0;

            return width / 2 + x - element.GetWidth(windowSize) / 2 + offset;
        }

        public override float GetY(Vector2 windowSize, UIElement element)
        {
            float height = element.HasParent ? element.parent.GetHeight(windowSize) : windowSize.Y;
            float y = element.HasParent ? element.parent.GetY(windowSize) : 0;

            return height / 2 + y - element.GetHeight(windowSize) / 2 + offset;
        }

        public override float GetWidth(Vector2 windowSize, UIElement element) => windowSize.X / 2;

        public override float GetHeight(Vector2 windowSize, UIElement element) => windowSize.Y / 2;
    }
}
