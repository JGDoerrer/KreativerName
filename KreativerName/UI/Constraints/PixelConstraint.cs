using System;
using System.Collections.Generic;
using System.Text;
using KreativerName.UI;
using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class PixelConstraint : Constraint
    {
        public PixelConstraint(int value)
        {
            this.value = value;
            relation = RelativeTo.Parent;
        }

        public PixelConstraint(int value, RelativeTo relation)
        {
            this.value = value;
            this.relation = relation;
        }
        public PixelConstraint(int value, RelativeTo relation, Direction direction)
        {
            this.value = value;
            this.relation = relation;
            this.direction = direction;
        }

        int value;

        public override float GetX(Vector2 windowSize, UIElement element)
        {
            float x;
            float width;
            if (relation == RelativeTo.Parent && element.HasParent)
            {
                x = element.parent.GetX(windowSize);
                width = element.parent.GetWidth(windowSize);
            }
            else 
            {
                x = 0;
                width = windowSize.X;
            }

            switch (direction)
            {
                default:
                case Direction.Left:
                    x += value;
                    break;
                case Direction.Right:
                    x += width - value- element.GetWidth(windowSize);
                    break;
            }

            return x;
        }

        public override float GetY(Vector2 windowSize, UIElement element)
        {
            float y;
            float height;
            if (relation == RelativeTo.Parent && element.HasParent)
            {
                y = element.parent.GetY(windowSize);
                height = element.parent.GetHeight(windowSize);
            }
            else
            {
                y = 0;
                height = windowSize.Y;
            }

            switch (direction)
            {
                default:
                case Direction.Top:
                    y += value;
                    break;
                case Direction.Bottom:
                    y += height - value - element.GetHeight(windowSize);
                    break;
            }

            return y;
        }

        public override float GetWidth(Vector2 windowSize, UIElement element)
        {
            return value;
        }

        public override float GetHeight(Vector2 windowSize, UIElement element)
        {
            return value;
        }
    }
}
