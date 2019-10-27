using System;
using System.Collections.Generic;
using System.Text;
using KreativerName.UI;
using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class PixelConstraint : Constraint
    {
        public PixelConstraint(int value) => this.value = value;

        int value;

        public override float GetX(Vector2 windowSize, UIElement element) => element.HasParent ? element.parent.GetX(windowSize) + value : value;
        public override float GetY(Vector2 windowSize, UIElement element) => element.HasParent ? element.parent.GetY(windowSize) + value : value;
        public override float GetWidth(Vector2 windowSize, UIElement element) => value;
        public override float GetHeight(Vector2 windowSize, UIElement element) => value;
    }
}
