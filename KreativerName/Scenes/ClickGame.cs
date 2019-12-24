using System;
using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    class ClickGame : Scene
    {
        public ClickGame()
        {
            InitUI();
        }

        public override void Update()
        {

        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);
        }

        #region UI

        UI.UI ui;
        TextBlock textClips;
        TextBlock textWire;

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = SceneManager.Input;

            void AddButton(int x, int y, string s, ClickEvent ev)
            {
                TextBlock text = new TextBlock(s, 2, 10, 10);

                Button button = new Button(x, y, (int)text.TextWidth + 18, (int)text.TextHeight + 18);
                button.OnClick += ev;

                button.AddChild(text);
                ui.Add(button);
            }

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
                        
            AddButton(190, 100, "Büroklammer machen", Click);

        }

        private static void Click()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
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
                }


                disposedValue = true;
            }
        }

        ~ClickGame()
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
