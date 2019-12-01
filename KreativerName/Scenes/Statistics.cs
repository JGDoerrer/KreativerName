using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class Statistics : Scene
    {
        public Statistics()
        {
            InitUI();

            background = new List<Numbers>();
        }

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(Scenes.Window);

            void AddText(string s, string value, int y)
            {
                TextBlock text1 = new TextBlock(s, 3);
                text1.Color = Color.White;
                text1.SetConstraints(new CenterConstraint(-text1.TextWidth / 2f), new PixelConstraint(y), new PixelConstraint((int)text1.TextWidth), new PixelConstraint((int)text1.TextHeight));
                ui.Add(text1);

                TextBlock text2 = new TextBlock(value, 3);
                text2.Color = Color.White;
                text2.SetConstraints(new CenterConstraint(text2.TextWidth / 2f), new PixelConstraint(y), new PixelConstraint((int)text2.TextWidth), new PixelConstraint((int)text2.TextHeight));
                ui.Add(text2);
            }

            TextBlock title = new TextBlock("Statistik", 4);
            title.Color = Color.White;
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

            Button button = new Button(40, 40, 40, 40);
            button.Shortcut = OpenTK.Input.Key.Escape;
            button.OnClick += () =>
            {
                Scenes.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstaints(10, 10, 20, 20));

            button.AddChild(exitImage);
            ui.Add(button);

            int y = 100;

            AddText("Züge: ", Stats.Current.TotalMoves.ToString(), y += 30);
            AddText("Level geschafft: ", Stats.Current.LevelsCompleted.ToString(), y += 30);
            AddText("Level perfekt geschafft: ", Stats.Current.LevelsCompletedPerfect.ToString(), y += 30);
            AddText("Erstes Spiel: ", Stats.Current.FirstStart.ToString("dd.MM.yy"), y += 30);
            AddText("Spielzeit: ", Stats.Current.TimePlaying.ToString("hh\\:mm\\:ss"), y += 30);
            AddText("Tode: ", Stats.Current.Deaths.ToString(), y += 30);
            if (Stats.Current.TetrisHighScore > 0)
            {
                AddText("Tetris höchste Punktzahl: ", Stats.Current.TetrisHighScore.ToString(), y += 30);
                AddText("Tetris meiste Linien: ", Stats.Current.TetrisMostLines.ToString(), y += 30);
                AddText("Tetris höchstes Level: ", Stats.Current.TetrisHighLevel.ToString(), y += 30);
            }
        }

        UI.UI ui;
        List<Numbers> background;
        static Random random = new Random();

        public override void Update()
        {
            if (random.NextDouble() < .09)
            {
                int length = random.Next(3, 10);
                List<int> values = new List<int>();
                for (int i = 0; i < length; i++)
                {
                    values.Add(random.Next(0, 10));
                }

                background.Add(new Numbers()
                {
                    Color = Color.FromArgb(50, 50, 50),
                    Values = values,
                    Position = new Vector2((float)random.NextDouble(), 0)
                });
            }

            for (int i = background.Count - 1; i >= 0; i--)
            {
                Numbers item = background[i];

                if (item.Position.Y > 1.1)
                {
                    background.RemoveAt(i);
                }
            }
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            if (Scenes.Input.MouseDown(OpenTK.Input.MouseButton.Left))
            {
                foreach (Numbers number in background)
                {
                    for (int i = 0; i < number.Values.Count; i++)
                    {
                        Vector2 mousePos = Scenes.Input.MousePosition;
                        Vector2 position = number.Position * windowSize - new Vector2(0, 12);

                        position.X = (float)Math.Round(position.X / 16) * 16;
                        position.Y = (float)Math.Round((position.Y / 16) - i) * 16;

                        if (mousePos.X >= position.X &&
                            mousePos.X <= position.X + 16 &&
                            mousePos.Y >= position.Y &&
                            mousePos.Y <= position.Y + 16 &&
                            number.Values[i] == 1)
                            Scenes.LoadScene(new Transition(new Tetris(), 10));
                    }
                }
            }

            for (int i = background.Count - 1; i >= 0; i--)
            {
                Numbers number = background[i];

                Vector2 position = number.Position * windowSize - new Vector2(0, 12);
                number.Position.Y += 0.0015f;
                Vector2 newPosition = number.Position * windowSize - new Vector2(0, 12);

                float prevY = (float)Math.Round(position.Y / 16) * 16;
                float newY = (float)Math.Round((newPosition.Y) / 16) * 16;

                if (prevY != newY)
                {
                    for (int j = number.Values.Count - 1; j >= 1; j--)
                    {
                        number.Values[j] = number.Values[j - 1];

                        if (j == 1)
                            number.Values[0] = random.Next(0, 10);
                    }
                }
                background[i] = number;
            }

            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            foreach (Numbers number in background)
            {
                Vector2 position = number.Position * windowSize - new Vector2(0, 12);
                position.X = (float)Math.Round(position.X / 16) * 16;
                position.Y = (float)Math.Round(position.Y / 16) * 16;

                for (int i = 0; i < number.Values.Count; i++)
                {
                    int value = number.Values[i];
                    TextureRenderer.Draw(Textures.Get("Font"), position + new Vector2(0, -i * 16), Vector2.One * 2, number.Color, new RectangleF(((value + 16) % 16) * 6, ((value + 16) / 16) * 6, 6, 6));
                }
            }

            ui.Render(windowSize);
        }

        struct Numbers
        {
            public List<int> Values;
            public Vector2 Position;
            public Color Color;
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

                background = null;

                disposedValue = true;
            }
        }

        ~Statistics()
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
