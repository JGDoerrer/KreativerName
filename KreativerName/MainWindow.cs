using System;
using KreativerName.Rendering;
using KreativerName.Scenes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KreativerName
{
    public class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            Textures.LoadTextures(@"Resources\Textures");
            Stats.Current = Stats.LoadFromFile("statistics");

            if (Stats.Current.FirstStart.Ticks == 0)
                Stats.Current.FirstStart = DateTime.Now;

            Scenes.Scenes.SetWindow(this);
            Scenes.Scenes.LoadScene(new MainMenu());

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        Input input;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = $"KerativerName {RenderFrequency:N1} fps, {UpdateFrequency:N1} ups";

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

            Scenes.Scenes.Update(size);

            // Update TimePlaying
            Stats.Current.TimePlaying = Stats.Current.TimePlaying.Add(TimeSpan.FromSeconds(e.Time));
            
            input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector2 size = new Vector2(Width, Height);

            Scenes.Scenes.Render(size);

            SwapBuffers();
        }

        protected override void OnClosed(EventArgs e)
        {
            Stats.Current.SaveToFile("statistics");
        }
    }
}
