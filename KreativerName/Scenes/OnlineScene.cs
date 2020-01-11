using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.Networking;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    class OnlineScene : Scene
    {
        public OnlineScene()
        {
            InitUI();

            if (SceneManager.Client?.Connected == true)
            {
                SceneManager.Client.PacketRecieved += HandleRequest;

                SceneManager.Client.Send(new Packet(PacketCode.GetIDs, BitConverter.GetBytes(10)));
                SceneManager.Client.Send(new Packet(PacketCode.GetWeeklyWorld, PacketInfo.None));
            }
            else
            {
                Notification.Show("Nicht zum Server verbunden");
            }
        }

        List<World> worlds = new List<World>();
        World? weekly = null;

        public override void Update()
        {

        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);
        }

        public override void Exit()
        {
            if (SceneManager.Client != null)
                SceneManager.Client.PacketRecieved -= HandleRequest;
        }

        #region UI

        UI.UI ui;
        Frame worldFrame;

        private void InitUI()
        {
            ui = new UI.UI
            {
                Input = SceneManager.Input
            };

            Button exitButton = new Button(40, 40, 40, 40)
            {
                Shortcut = Key.Escape
            };
            exitButton.OnLeftClick += () =>
            {
                SceneManager.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            Button weeklyButton = new Button(200, 40, 40, 40);
            weeklyButton.OnLeftClick += () =>
            {
                if (!weekly.HasValue)
                {
                    Notification.Show("Wöchentliche Welt ist nicht verfügbar");
                    return;
                }

                Game game = new Game(weekly.Value);
                game.Exit += () => { SceneManager.LoadScene(new Transition(this, 10)); };
                SceneManager.LoadScene(new Transition(game, 10));
            };

            ui.Add(weeklyButton);

            worldFrame = new Frame
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(new CenterConstraint(),
                    new PixelConstraint(100),
                    new RelativeConstraint(0.8f, RelativeTo.Window),
                    new PixelConstraint(200))
            };

            ui.Add(worldFrame);
        }

        private void UpdateWorlds()
        {
            worldFrame.ClearChildren();

            const int ButtonHeight = 60;
            const int Margin = 10;

            int i = 0;
            foreach (var world in worlds)
            {
                Button button = new Button(0, i * (ButtonHeight + Margin), 0, ButtonHeight);
                button.Constraints.widthCon = new RelativeConstraint(1, RelativeTo.Parent);
                button.OnLeftClick += () =>
                {
                    Game game = new Game(world);
                    game.Exit += () => { SceneManager.LoadScene(new Transition(this, 10)); };
                    SceneManager.LoadScene(new Transition(game, 10));
                };

                TextBlock text = new TextBlock($"{world.Title}: {world.Levels.Count} Level", 3, 10, 10);
                text.Constraints.xCon = new CenterConstraint();
                text.Constraints.yCon = new CenterConstraint();
                text.Color = Color.Black;
                button.AddChild(text);

                worldFrame.AddChild(button);
                i++;
            }
        }

        #endregion

        #region Handle Request

        void HandleRequest(Client client, Packet p)
        {
            switch (p.Code)
            {
                case PacketCode.GetWorldByID when p.Info == PacketInfo.Success:
                    GetWorldSuccess(client, p); break;
                case PacketCode.GetWorldByID when p.Info == PacketInfo.Error:
                    Notification.Show("Konnte Welt nicht herunterladen"); break;
                case PacketCode.GetIDs when p.Info == PacketInfo.Success:
                    GetIDsSuccess(client, p); break;
                case PacketCode.GetIDs when p.Info == PacketInfo.Error:
                    Notification.Show("Konnte IDs nicht herunterladen"); break;
                case PacketCode.UploadWorld when p.Info == PacketInfo.Success:
                    UploadWorldSuccess(client, p); break;
                case PacketCode.UploadWorld when p.Info == PacketInfo.Error:
                    Notification.Show("Konnte Welt nicht hochladen"); break;
                case PacketCode.GetWeeklyWorld when p.Info == PacketInfo.Success:
                    GetWeeklySuccess(client, p); break;
                case PacketCode.GetWeeklyWorld when p.Info == PacketInfo.Error:
                    Notification.Show("Konnte wöchentliche Welt nicht herunterladen"); break;
            }
        }

        void GetWorldSuccess(Client client, Packet msg)
        {
            World world = msg.World;

            if (!worlds.Contains(world))
            {
                worlds.Add(world);
                UpdateWorlds();
            }
        }

        void GetIDsSuccess(Client client, Packet p)
        {
            List<uint> ids = new List<uint>();

            int count = BitConverter.ToInt32(p.Bytes, 0);
            for (int i = 0; i < count; i++)
            {
                ids.Add(BitConverter.ToUInt32(p.Bytes, 4 + i * 4));
            }

            foreach (uint id in ids)
            {
                client.Send(new Packet(PacketCode.GetWorldByID, PacketInfo.None, BitConverter.GetBytes(id)));
            }
        }

        void UploadWorldSuccess(Client client, Packet msg)
        {
            Notification.Show($"Welt hochgeladen\nID: {BitConverter.ToUInt32(msg.Bytes, 0)}");
        }

        void GetWeeklySuccess(Client client, Packet msg)
        {
            World world = msg.World;

            weekly = world;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ui.Dispose();
                    worldFrame.Dispose();
                }

                disposedValue = true;
            }
        }

        ~OnlineScene()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(false);
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public override void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
