using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class RelativeConstraint : Constraint
    {
        float ratio;

        public RelativeConstraint(float ratio) => this.ratio = ratio;
        public RelativeConstraint(float ratio, RelativeTo relation)
        {
            this.ratio = ratio;
            this.relation = relation;
        }

        public override float GetX(Vector2 windowSize, UIElement element)
        {
            float value;
            switch (relation)
            {
                case RelativeTo.Parent when element.HasParent:
                    value = element.parent.GetX(windowSize);
                    break;
                case RelativeTo.Self:
                    value = element.GetY(windowSize);
                    break;
                default:
                    value = 0;
                    break;
            }
            return value * ratio;
        }

        public override float GetY(Vector2 windowSize, UIElement element)
        {
            float value;
            switch (relation)
            {
                case RelativeTo.Parent when element.HasParent:
                    value = element.parent.GetY(windowSize);
                    break;
                case RelativeTo.Self:
                    value = element.GetX(windowSize);
                    break;
                default:
                    value = 0;
                    break;
            }
            return value * ratio;
        }

        public override float GetWidth(Vector2 windowSize, UIElement element)
        {
            float value;
            switch (relation)
            {
                case RelativeTo.Parent when element.HasParent:
                    value = element.parent.GetWidth(windowSize);
                    break;
                case RelativeTo.Self:
                    value = element.GetHeight(windowSize);
                    break;
                case RelativeTo.Window:
                    value = windowSize.X;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value * ratio;
        }

        public override float GetHeight(Vector2 windowSize, UIElement element)
        {
            float value;
            switch (relation)
            {
                case RelativeTo.Parent when element.HasParent:
                    value = element.parent.GetHeight(windowSize);
                    break;
                case RelativeTo.Self:
                    value = element.GetWidth(windowSize);
                    break;
                case RelativeTo.Window:
                    value = windowSize.Y;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value * ratio;
        }
    }
}
