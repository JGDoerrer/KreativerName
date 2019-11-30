using OpenTK;

namespace KreativerName.Scenes
{
    public abstract class Scene
    {
        public abstract void Update();
        public abstract void UpdateUI(Vector2 windowSize);

        public abstract void Render(Vector2 windowSize);
    }
}
