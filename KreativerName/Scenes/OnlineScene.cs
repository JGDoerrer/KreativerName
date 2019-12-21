using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
                SceneManager.Client.BytesRecieved += HandleRequest;

                List<byte> bytes = new List<byte>() { 0x10, 0x02 };
                bytes.AddRange(BitConverter.GetBytes(10));

                SceneManager.Client.Send(bytes.ToArray());
            }
            else
            {
                Notification.Show("Nicht zum Server verbunden");

            }
        }
        
        List<World> worlds = new List<World>();

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
            SceneManager.Client.BytesRecieved -= HandleRequest;
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
            exitButton.OnClick += () =>
            {
                SceneManager.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

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
                button.OnClick += () =>
                {
                    Game game = new Game(world);
                    game.Exit += () => { SceneManager.LoadScene(new Transition(this, 10)); };
                    SceneManager.LoadScene(new Transition(game, 10));
                };

                TextBlock text = new TextBlock($"{world.Title}: {world.Levels.Count} Level ID: {world.ID.ToID()}", 3, 10, 10);
                text.Constraints.xCon = new CenterConstraint();
                text.Color = Color.Black;
                button.AddChild(text);

                worldFrame.AddChild(button);
                i++;
            }
        }

        #endregion

        #region Handle Request

        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x80;

        void HandleRequest(Client client, byte[] msg)
        {
            uint code = (uint)(BitConverter.ToUInt16(msg, 0) << 8);
            code |= msg[2];
            switch (code)
            {
                case 0x020000 | SuccessCode: GetWorldSuccess(client, msg); break;
                case 0x020000 | ErrorCode: Notification.Show("Konnte Welt nicht herunterladen"); break;
                case 0x021000 | SuccessCode: GetIDsSuccess(client, msg); break;
                case 0x021000 | ErrorCode: Notification.Show("Konnte IDs nicht herunterladen"); break;
                case 0x022000 | SuccessCode: UploadWorldSuccess(client, msg); break;
                case 0x022000 | ErrorCode: Notification.Show("Konnte Welt nicht hochladen"); break;
            }
        }

        void GetWorldSuccess(Client client, byte[] msg)
        {
            World world = World.LoadFromBytes(msg.Skip(3).ToArray());

            if (!worlds.Contains(world))
            {
                worlds.Add(world);
                UpdateWorlds();
            }
        }

        void GetIDsSuccess(Client client, byte[] msg)
        {
            List<uint> ids = new List<uint>();

            int count = BitConverter.ToInt32(msg, 3);
            for (int i = 0; i < count; i++)
            {
                ids.Add(BitConverter.ToUInt32(msg, 7 + i * 4));
            }

            foreach (uint id in ids)
            {
                List<byte> bytes = new List<byte>() { 0x00, 0x02 };
                bytes.AddRange(BitConverter.GetBytes(id));

                client.Send(bytes.ToArray());
            }
        }

        void UploadWorldSuccess(Client client, byte[] msg)
        {
            Notification.Show($"Welt hochgeladen\nID: {BitConverter.ToUInt32(msg, 3)}");
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
