using OpenTK;

namespace KreativerName.UI.Constraints
{
    public abstract class Constraint
    {
        protected RelativeTo relation;
        protected Direction direction;

        public abstract float GetX(Vector2 windowSize, UIElement element);
        public abstract float GetY(Vector2 windowSize, UIElement element);
        public abstract float GetWidth(Vector2 windowSize, UIElement element);
        public abstract float GetHeight(Vector2 windowSize, UIElement element);
    }
}
