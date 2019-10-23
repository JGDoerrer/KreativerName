using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    class MasterRenderer
    {
        public MasterRenderer(Game game)
        {
            gameRenderer = new GameRenderer(game);
        }

        GameRenderer gameRenderer;

        public void Render(int width, int height)
        {
            gameRenderer.Render(width, height);
        }
    }
}
