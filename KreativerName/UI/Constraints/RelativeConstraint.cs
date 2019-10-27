using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class RelativeConstraint : Constraint
    {
        float ratio;

        public RelativeConstraint(float ratio) => this.ratio = ratio;

        public override float GetX(Vector2 windowSize, UIElement element) => element.GetY(windowSize) * ratio;
        public override float GetY(Vector2 windowSize, UIElement element) => element.GetX(windowSize) * ratio;
        public override float GetWidth(Vector2 windowSize, UIElement element) => element.GetHeight(windowSize) * ratio;
        public override float GetHeight(Vector2 windowSize, UIElement element) => element.GetWidth(windowSize) * ratio;
    }
}
