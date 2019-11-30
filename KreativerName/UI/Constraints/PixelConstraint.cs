using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class PixelConstraint : Constraint
    {
        public PixelConstraint(int value)
        {
            Value = value;
            relation = RelativeTo.Parent;
        }

        public PixelConstraint(int value, RelativeTo relation)
        {
            Value = value;
            this.relation = relation;
        }
        public PixelConstraint(int value, RelativeTo relation, Direction direction)
        {
            Value = value;
            this.relation = relation;
            this.direction = direction;
        }

        public int Value;

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
                    x += Value;
                    break;
                case Direction.Right:
                    x += width - Value - element.GetWidth(windowSize);
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
                    y += Value;
                    break;
                case Direction.Bottom:
                    y += height - Value - element.GetHeight(windowSize);
                    break;
            }

            return y;
        }

        public override float GetWidth(Vector2 windowSize, UIElement element)
        {
            return Value;
        }

        public override float GetHeight(Vector2 windowSize, UIElement element)
        {
            return Value;
        }
    }
}
