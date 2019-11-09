using System.Collections.Generic;
using System.Drawing;
using KreativerName.Rendering;
using KreativerName.Scenes;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KreativerName
{
    class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            Textures.LoadTextures(@"Resources\Textures");

            scene = new MainMenu();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public static Input input;
        public static Scene scene;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (input.KeyPress(OpenTK.Input.Key.F11))
            {
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;
            }

            if (input.KeyDown(OpenTK.Input.Key.AltLeft) && input.KeyDown(OpenTK.Input.Key.F4))
                Close();

            Vector2 size = new Vector2(Width, Height);

            scene?.Update();
            scene?.UpdateUI(size);
            
            input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector2 size = new Vector2(Width, Height);

            scene?.Render(size);

            SwapBuffers();
        }
    }
}
