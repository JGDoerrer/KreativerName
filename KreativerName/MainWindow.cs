using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
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
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            Textures.LoadTextures(@"Resources\Textures");

            if (File.Exists(@"Resources\Icon.ico"))
                Icon = new Icon(@"Resources\Icon.ico");

            SceneManager.SetWindow(this);
            SceneManager.LoadScene(new LoadingScene(LoadStuff, new Transition(new MainMenu(), 30)));

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        Input input;
        public int FrameCounter;
        double fps;
        double ups;
        public static readonly Version version = new Version(0, 2, 1, 0);

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (FrameCounter % 10 == 0)
            {
                fps = RenderFrequency;
                ups = UpdateFrequency;
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

            if (FrameCounter % 600 == 0 && SceneManager.Client?.Connected == true)
            {
                SceneManager.Client.Send(new Packet(PacketCode.UploadStats, PacketInfo.None, Stats.Current.ToBytes()));
            }

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
                TextRenderer.RenderString($"{fps:00}/{ups:00}", new Vector2(Width - 68, Height - 20), Color.White);

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

            SceneManager.ConnectClient();
            if (SceneManager.Client != null)
            {
                SceneManager.Client.PacketRecieved += HandleRequest;
                Login();
                CompareVersion();
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

        private void Login()
        {
            if (SceneManager.Client == null)
                return;
            if (!Settings.Current.LoggedIn)
                return;

            byte[] msg = new byte[8];
            BitConverter.GetBytes(Settings.Current.UserID).CopyTo(msg, 0);
            BitConverter.GetBytes(Settings.Current.LoginInfo).CopyTo(msg, 4);

            static void handle(Client c, Packet p)
            {
                if (p.Code == PacketCode.LogIn && p.Info == PacketInfo.Success)
                {
                    Notification.Show($"Eingeloggt unter {Settings.Current.UserName} ({Settings.Current.UserID.ToID()})");
                    SceneManager.Client.PacketRecieved -= handle;
                }
            }
            SceneManager.Client.PacketRecieved += handle;

            SceneManager.Client.Send(new Packet(PacketCode.LogIn, PacketInfo.None, msg));
        }

        private void CompareVersion()
        {
            if (SceneManager.Client == null)
                return;

            static void handle(Client client, Packet p)
            {
                if (p.Code == PacketCode.CompareVersion)
                {
                    if (p.Info == PacketInfo.New)
                        Notification.Show("Eine neue Version ist verfügbar!");
                    else if (p.Info == PacketInfo.Error)
                        Notification.Show("Fehler beim Überprüfen der Version");

                    SceneManager.Client.PacketRecieved -= handle;
                }
            }

            SceneManager.Client.PacketRecieved += handle;

            SceneManager.Client.Send(new Packet(PacketCode.CompareVersion, PacketInfo.None, version.ToBytes()));
        }

        private void HandleRequest(Client client, Packet msg)
        {
            switch (msg.Code)
            {
                case PacketCode.RecieveNotification:
                    float size = BitConverter.ToSingle(msg.Bytes, 0);
                    int sLength = BitConverter.ToInt32(msg.Bytes, 4);
                    string s = Encoding.UTF8.GetString(msg.Bytes, 8, sLength);

                    Notification.Show(s, size);
                    break;
            }
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (World.IsValidFile(e.FileName))
            {
                Game game = new Game(World.LoadFromFile(e.FileName, false));
                game.Exit += () =>
                {
                    game.World.SaveToFile(e.FileName, false);
                    SceneManager.LoadScene(new Transition(new MainMenu(), 10));
                };
                SceneManager.LoadScene(new Transition(game, 10));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SceneManager.Client?.Send(new Packet(PacketCode.Disconnect, PacketInfo.None));

            SceneManager.Client?.Disconnect();
        }
    }
}
