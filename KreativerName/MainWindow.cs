﻿using System;
using System.Collections.Generic;
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
        public static readonly Version version = new Version(0,1,1,0);

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (FrameCounter % 10 == 0)
            {
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

            if (FrameCounter % 600 == 0 && SceneManager.Client?.Connected == true)
            {
                List<byte> bytes = new List<byte>() { 0x00, 0x03 };
                bytes.AddRange(Stats.Current.ToBytes());

                SceneManager.Client.Send(bytes.ToArray());
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
                TextRenderer.RenderString($"{fps:00} fps", new Vector2(Width - 80, Height - 20), Color.White);

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
                SceneManager.Client.BytesRecieved += HandleRequest;
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

            byte[] msg = new byte[10];
            new byte[] { 0x10, 0x01 }.CopyTo(msg, 0);
            BitConverter.GetBytes(Settings.Current.UserID).CopyTo(msg, 2);
            BitConverter.GetBytes(Settings.Current.LoginInfo).CopyTo(msg, 6);

            static void handle(Client c, byte[] b)
            {
                ushort code = BitConverter.ToUInt16(b, 0);
                if (code == 0x0110 && b[2] == 0x80)
                {
                    Notification.Show($"Eingeloggt unter {Settings.Current.UserName} ({Settings.Current.UserID.ToID()})");
                    SceneManager.Client.BytesRecieved -= handle;
                }
            }
            SceneManager.Client.BytesRecieved += handle;

            SceneManager.Client.Send(msg);
        }

        private void CompareVersion()
        {
            if (SceneManager.Client == null)
                return;

            List<byte> msg = new List<byte>() { 0x00, 0x05 };

            msg.AddRange(version.ToBytes());

            static void handle(Client client, byte[] msg)
            {
                ushort code = BitConverter.ToUInt16(msg, 0);
                if (code == 0x0500)
                {
                    if (msg[2] == 0x40)
                        Notification.Show("Eine neue Version ist verfügbar!");
                    else if (msg[2] == 0xFF)
                        Notification.Show("Fehler beim Überprüfen der Version");

                    SceneManager.Client.BytesRecieved -= handle;
                }
            }

            SceneManager.Client.BytesRecieved += handle;

            SceneManager.Client.Send(msg.ToArray());
        }

        private void HandleRequest(Client client, byte[] msg)
        {
            ushort code = BitConverter.ToUInt16(msg, 0);

            switch (code)
            {
                case 0x0400:
                    float size = BitConverter.ToSingle(msg, 2);
                    int sLength = BitConverter.ToInt32(msg, 6);
                    string s = Encoding.UTF8.GetString(msg, 10, sLength);

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
            SceneManager.Client?.StopRecieve();
            SceneManager.Client?.Disconnect();
        }
    }
}
