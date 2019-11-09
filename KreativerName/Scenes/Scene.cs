using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
