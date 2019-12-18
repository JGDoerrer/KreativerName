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
                SceneManager.Client.BytesRecieved += HandleRequest;
                new Thread(SceneManager.Client.Recieve).Start();
            }
        }

        UI.UI ui;

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

        }

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
        
        #region Handle Request

        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x40;

        public static void HandleRequest(Client client, byte[] msg)
        {
            ushort code = BitConverter.ToUInt16(msg, 0);
            switch (code)
            {
                case 0x0100 | SuccessCode: SignUpSuccess(client, msg); break;
                case 0x0100 | ErrorCode: SignUpError(client, msg); break;

                case 0x0200 | SuccessCode: GetWorldSuccess(client, msg); break;
                case 0x0200 | ErrorCode: Console.WriteLine("Could not get world"); break;
                case 0x0200 | SuccessCode + 1: UploadWorldSuccess(client, msg); break;
                case 0x0200 | ErrorCode - 1: Console.WriteLine("Could not upload world"); break;
            }
        }

        private static void SignUpSuccess(Client client, byte[] msg)
        {
            ushort id = BitConverter.ToUInt16(msg, 2);
            Settings.Current.UserID = id;
        }

        private static void SignUpError(Client client, byte[] msg)
        {
            Console.WriteLine("Error");
        }

        private static void GetWorldSuccess(Client client, byte[] msg)
        {
            World world = World.LoadFromBytes(msg.Skip(2).ToArray());
            Console.WriteLine(world.Title);
        }

        private static void UploadWorldSuccess(Client client, byte[] msg)
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
