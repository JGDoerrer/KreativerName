using KreativerName.Rendering;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace KreativerName
{
    class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            game = new Game();
            game.input = new Input(this);
            renderer = new MasterRenderer(game);

            Textures.LoadTextures(@"Resources\Textures");
        }

        Game game;
        MasterRenderer renderer;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            game.Update();
            game.UpdateUI(new Vector2(Width, Height));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            renderer.Render(Width, Height);

            SwapBuffers();
        }
    }
}
