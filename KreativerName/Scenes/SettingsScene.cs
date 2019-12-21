using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class SettingsScene : Scene
    {
        public SettingsScene()
        {
            InitUI();
        }

        UI.UI ui;

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = SceneManager.Input;

            TextBlock title = new TextBlock("Einstellungen", 4);
            title.Color = Color.White;
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

            {
                Button button = new Button(40, 40, 40, 40);
                button.Shortcut = OpenTK.Input.Key.Escape;
                button.OnClick += () =>
                {
                    SceneManager.LoadScene(new Transition(new MainMenu(), 10));
                };
                UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
                exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

                button.AddChild(exitImage);
                ui.Add(button);
            }

            void AddCheckBox(string s, int y, bool state, CheckEvent clickEvent)
            {
                Frame frame = new Frame();
                frame.Color = Color.Transparent;

                CheckBox box = new CheckBox(0, 0, 36, 36);
                box.Checked = state;
                box.OnChecked += clickEvent;
                frame.AddChild(box);

                TextBlock text = new TextBlock(s, 3, 45, 0);
                text.Constraints.yCon = new CenterConstraint();
                text.Color = Color.White;
                frame.AddChild(text);

                frame.SetConstraints(new CenterConstraint(), new PixelConstraint(y), new PixelConstraint(45 + (int)text.TextWidth), new PixelConstraint(36));
                ui.Add(frame);
            }
            void AddText(string s, int y)
            {
                TextBlock text = new TextBlock(s, 3);
                text.Color = Color.White;
                text.SetConstraints(new CenterConstraint(), new PixelConstraint(y), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                ui.Add(text);
            }

            AddCheckBox("Vollbild", 100, Settings.Current.Fullscreen, (b) =>
            {
                Settings.Current.Fullscreen = b;
                SceneManager.Window.WindowState = b ? WindowState.Fullscreen : WindowState.Normal;
            });
            AddCheckBox("Züge anzeigen", 150, Settings.Current.ShowMoves, (b) =>
            {
                Settings.Current.ShowMoves = b;
            });
            AddCheckBox("FPS anzeigen", 200, Settings.Current.ShowFps, (b) =>
            {
                Settings.Current.ShowFps = b;
            });

            TextBox textBox = new TextBox();
            textBox.SetConstraints(new CenterConstraint(), new PixelConstraint(260), new PixelConstraint(180), new PixelConstraint(34));
            textBox.Enabled = !Settings.Current.LoggedIn;
            textBox.TextColor = Color.Black;
            textBox.MaxTextSize = 15;
            ui.Add(textBox);

            Button sendButton = new Button();
            sendButton.SetConstraints(new CenterConstraint(), new PixelConstraint(300), new PixelConstraint(120), new PixelConstraint(34));
            sendButton.Enabled = !Settings.Current.LoggedIn;
            sendButton.OnClick += () => SignUp(textBox);

            TextBlock sendText = new TextBlock("Anmelden", 2, 10, 10);
            sendButton.AddChild(sendText);
            ui.Add(sendButton);

            AddText($"ID: {Settings.Current.UserID.ToString("x")}", 350);
            AddText($"LoginInfo: {Settings.Current.LoginInfo.ToString("x")}", 390);
        }

        private static void SignUp(TextBox textBox)
        {
            if (SceneManager.Client?.Connected != true)
                return;

            string s = textBox.Text.Trim();

            if (s == string.Empty)
                return;

            List<byte> bytes = new List<byte>() { 0x00, 0x01 };

            byte[] name = Encoding.UTF8.GetBytes(s);

            bytes.AddRange(name.Length.ToBytes());
            bytes.AddRange(name);

            Thread thread = null;

            void Handle(Networking.Client c, byte[] b)
            {
                ushort code = BitConverter.ToUInt16(b, 0);
                if (code == 0x0100 && b[2] == 0x80)
                {
                    Settings.Current.UserID = BitConverter.ToUInt32(b, 3);
                    Settings.Current.LoginInfo = BitConverter.ToUInt32(b, 7);
                    Settings.Current.LoggedIn = true;
                    SceneManager.Client.BytesRecieved -= Handle;
                    thread.Abort();
                }
                else if (code == 0x0100 && b[2] == 0xFF)
                {
                    Settings.Current.LoggedIn = false;
                    SceneManager.Client.BytesRecieved -= Handle;
                    thread.Abort();
                }
            }
            SceneManager.Client.BytesRecieved += Handle;

            thread = new Thread(SceneManager.Client.StartRecieve);
            thread.Start();

            SceneManager.Client.Send(bytes.ToArray());
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

        ~SettingsScene()
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
