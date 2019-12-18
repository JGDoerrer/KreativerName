using System;
using System.ComponentModel;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.Scenes;
using KreativerName.UI;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace KreativerName
{
    public class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            Textures.LoadTextures(@"Resources\Textures");

            SceneManager.SetWindow(this);
            SceneManager.LoadScene(new LoadingScene(LoadStuff, new Transition(new MainMenu(), 30)));

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        Input input;
        public int FrameCounter;
        double fps;

        private void LoadStuff(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Stats.Current = Stats.LoadFromFile("statistics");
            worker.ReportProgress(20);

            Settings.Current = Settings.LoadFromFile("settings");
            worker.ReportProgress(40);

            HexData.LoadData(@"Resources\HexData");
            worker.ReportProgress(60);

            SceneManager.ConnectClient();
            Login();
            worker.ReportProgress(80);

            if (Stats.Current.FirstStart.Ticks == 0)
                Stats.Current.FirstStart = DateTime.Now;

            WindowState = Settings.Current.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;

            worker.ReportProgress(100);
        }

        private void Login()
        {
            if (Settings.Current.LoggedIn && SceneManager.Client != null)
            {
                byte[] msg = new byte[4] { 0x00, 0x01, 0, 0 };
                BitConverter.GetBytes(Settings.Current.UserID).CopyTo(msg, 2);

                SceneManager.Client.Send(msg);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (FrameCounter % 10 == 0)
            {
                Title = $"KreativerName {RenderFrequency:N1} fps, {UpdateFrequency:N1} ups";
                fps = RenderFrequency;
            }

            if (input.KeyPress(Key.F11))
            {
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;

                Settings.Current.Fullscreen = WindowState == WindowState.Fullscreen;
            }

            if (input.KeyDown(Key.AltLeft) && input.KeyDown(Key.F4))
                Close();

            Vector2 size = new Vector2(Width, Height);

            try
            {
                SceneManager.Update(size);
            }
            catch (Exception ex)
            {
                SceneManager.LoadScene(new Transition(new ErrorScene(ex), 10));
            }

            // Update TimePlaying
            Stats.Current.TimePlaying = Stats.Current.TimePlaying.Add(TimeSpan.FromSeconds(e.Time));
            
            input.Update();

            FrameCounter++;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.Black);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Vector2 size = new Vector2(Width, Height);

            try
            {
                SceneManager.Render(size);
            }
            catch (Exception ex)
            {
                SceneManager.LoadScene(new Transition(new ErrorScene(ex), 10));
            }

            // Render Fps
            if (Settings.Current.ShowFps)
                TextRenderer.RenderString($"{fps:00} fps", new Vector2(Width - 80, Height - 20), Color.White);
            
            SwapBuffers();
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (World.IsValidFile(e.FileName))
            {
                Game game = new Game(World.LoadFromFile(e.FileName, false));
                game.Exit += () =>
                {
                    game.World.SaveToFile(e.FileName, false);
                    Scenes.SceneManager.LoadScene(new Transition(new MainMenu(), 10));
                };
                Scenes.SceneManager.LoadScene(new Transition(game, 10));
            }
        }
    }
}
