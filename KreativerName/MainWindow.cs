using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using KreativerName.Grid;
using KreativerName.Networking;
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
            : base(16 * 80, 9 * 80, GraphicsMode.Default, $"KreativerName {Version}")
        {
            Textures.LoadTextures(@"Resources\Textures");
            Shaders.LoadShaders(@"Resources\Shaders");

            if (File.Exists(@"Resources\Icon.ico"))
                Icon = new Icon(@"Resources\Icon.ico");

            SceneManager.SetWindow(this);
            SceneManager.LoadScene(new LoadingScene(LoadStuff, "", new Transition(new MainMenu(), 30)));

            //GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public int FrameCounter;
        double fps;
        double ups;
        public static readonly Version Version = new Version(0, 4, 0, 0);

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (FrameCounter % 10 == 0)
            {
                fps = RenderFrequency;
                ups = UpdateFrequency;
            }

            if (SceneManager.Input.KeyDown(Key.AltLeft) && SceneManager.Input.KeyDown(Key.F4))
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

            // Update Stats
            Stats.Current.TimePlaying = Stats.Current.TimePlaying.Add(TimeSpan.FromSeconds(e.Time));
            Stats.Current.LastUpdated = DateTime.Now;

            if (FrameCounter % 600 == 0)
            {
                ClientManager.Send(new Packet(PacketCode.UploadStats, PacketInfo.None, Stats.Current.ToBytes()));
            }

            FrameCounter++;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
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
            {
                string s = $"{fps:00}/{ups:00}";
                TextRenderer.RenderString(s, new Vector2(Width - 65, Height - 17), Color.White);
            }

            sw.Stop();
            Console.WriteLine($"[StopWatch]: Rendertime: {sw.ElapsedMilliseconds}ms");
            
            SwapBuffers();
        }

        private void LoadStuff(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Stats.Current = Stats.LoadFromFile("statistics");
            worker.ReportProgress(20);

            Settings.Current = Settings.LoadFromFile("settings");
            worker.ReportProgress(40);

            HexData.LoadData(@"Resources\HexData");
            worker.ReportProgress(60);

            if (ClientManager.Connect())
            {
                ClientManager.Login();
                ClientManager.CompareVersion();
            }
            else
            {
                Notification.Show("Konnte nicht mit Server verbinden");
            }
            worker.ReportProgress(80);

            if (Stats.Current.FirstStart.Ticks == 0)
                Stats.Current.FirstStart = DateTime.Now;

            WindowState = Settings.Current.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;

            worker.ReportProgress(100);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (World.IsValidFile(e.FileName))
            {
                Game game = new Game(World.LoadFromFile(e.FileName, false));
                game.OnExit += () =>
                {
                    game.World.SaveToFile(e.FileName, false);
                    SceneManager.LoadScene(new Transition(new MainMenu(), 10));
                };
                SceneManager.LoadScene(new Transition(game, 10));
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            Textures.Dispose();
            Shaders.Dispose();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            ClientManager.Disconnect();
        }
    }
}
