using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
                SceneManager.Client.BytesRecieved -= HandleRequest;
                SceneManager.Client.BytesRecieved += HandleRequest;

                SceneManager.Client.Send(new byte[] { 0x00,0x02,0,0,0,0 });
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

        #region UI

        UI.UI ui;
        Frame worldFrame;

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = SceneManager.Input;

            Button exitButton = new Button(40, 40, 40, 40);
            exitButton.Shortcut = Key.Escape;
            exitButton.OnClick += () =>
            {
                SceneManager.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            worldFrame = new Frame();
            worldFrame.SetConstraints( new CenterConstraint(),
                new PixelConstraint(100),
                new PixelConstraint(200),
                new PixelConstraint(200));

            ui.Add(worldFrame);
        }

        private void UpdateWorlds()
        {
            worldFrame.ClearChildren();
            const int ButtonSize = 60;
            const int RowSize = 5;

            int i = 0;
            foreach (var world in worlds)
            {
                Button button = new Button(ButtonSize * (i % RowSize), ButtonSize * (i / RowSize), ButtonSize, ButtonSize);
                button.OnClick += () => 
                {
                    Game game = new Game(world);
                    game.Exit += () =>
                    {
                        SceneManager.LoadScene(new Transition(this, 10));
                    };
                    SceneManager.LoadScene(new Transition(game, 10));
                };
                

                worldFrame.AddChild(button);
            }
        }

        #endregion

        #region Handle Request

        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x80;

        public void HandleRequest(Client client, byte[] msg)
        {
            uint code = (uint)(BitConverter.ToUInt16(msg, 0) << 8);
            code |= msg[2];
            switch (code)
            {
                case 0x020000 | SuccessCode: GetWorldSuccess(client, msg); break;
                case 0x020000 | ErrorCode: Console.WriteLine("Could not get world"); break;
                case 0x022000 | SuccessCode: UploadWorldSuccess(client, msg); break;
                case 0x022000 | ErrorCode: Console.WriteLine("Could not upload world"); break;
            }
        }

        private void GetWorldSuccess(Client client, byte[] msg)
        {
            World world = World.LoadFromBytes(msg.Skip(3).ToArray());

            if (!worlds.Contains(world))
            {
                worlds.Add(world);
                UpdateWorlds();
            }
        }

        private void UploadWorldSuccess(Client client, byte[] msg)
        {
            Console.WriteLine($"World uploaded; ID: {BitConverter.ToUInt32(msg, 2)}");
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
