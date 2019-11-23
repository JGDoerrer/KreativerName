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

            background = new List<Number>();
        }

        ~Statistics()
        {
            ui.Dispose();
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

            AddText("Zuege: ", Stats.Current.TotalMoves.ToString(), 100);
            AddText("Level geschafft: ", Stats.Current.LevelsCompleted.ToString(), 130);
            AddText("Level perfekt geschafft: ", Stats.Current.LevelsCompletedPerfect.ToString(), 160);
            AddText("Erstes Spiel: ", Stats.Current.FirstStart.ToString("dd.mm.yy"), 190);
            AddText("Spielzeit: ", Stats.Current.TimePlaying.ToString("hh\\:mm\\:ss"), 220);
            AddText("Tode: ", Stats.Current.Deaths.ToString(), 250);
        }

        UI.UI ui;
        List<Number> background;
        static Random random = new Random();

        public override void Update()
        {
            if (random.NextDouble() < .12)
            {
                background.Add(new Number()
                {
                    Color = Color.FromArgb(50, 50, 50),
                    Value = random.Next(0,10),
                    Position = new Vector2((float)random.NextDouble(), 0)
                });
            }

            for (int i = background.Count - 1; i >= 0; i--)
            {
                Number item = background[i];

                item.Position.Y += 0.0015f;

                if (random.NextDouble() < .01)
                    item.Value = (item.Value + 1) % 10;

                background[i] = item;

                if (item.Position.Y > 1.1)
                {
                    background.RemoveAt(i);
                }
            }
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            foreach (Number number in background)
            {
                Vector2 position = number.Position * windowSize - new Vector2(0, 12);
                position.X = (float)Math.Round(position.X / 16) * 16;
                // position.Y = (float)Math.Round(position.Y / 16) * 16;

                TextureRenderer.Draw(Textures.Get("Font"), position, Vector2.One * 2, number.Color, new RectangleF(((number.Value + 16) % 16) * 6, ((number.Value + 16) / 16) * 6, 6, 6));
            }

            ui.Render(windowSize);
        }

        struct Number
        {
            public int Value;
            public Vector2 Position;
            public Color Color;
        }
    }
}
