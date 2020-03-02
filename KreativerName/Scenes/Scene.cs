using System;
using OpenTK;

namespace KreativerName.Scenes
{
    public abstract class Scene : IDisposable
    {
        public virtual void Load() { }

        public abstract void Update();

        public abstract void UpdateUI(Vector2 windowSize);

        public abstract void Render(Vector2 windowSize);

        public virtual void Unload() { Dispose(); }

        public abstract void Dispose();
    }
}
