using System;
using System.Drawing;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class LevelMenu : Scene
    {
        public LevelMenu(int world)
        {
            worldIndex = world;
            this.world = World.LoadFromFile($"{world:000}");

            InitUI();
        }

        bool normalMode;
        int worldIndex;
        World world;
        UI.UI ui;

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

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(SceneManager.Window);

            TextBlock title = new TextBlock("Level", 4, 0, 50) { Color = Color.White };
            title.Constraints.xCon = new CenterConstraint();
            ui.Add(title);


            Button exitButton = new Button(20, 20, 40, 40) { Shortcut = Key.Escape };
            exitButton.OnLeftClick += () => SceneManager.LoadScene(new Transition(new WorldMenu(), 10));

            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black)
            { Constraints = new UIConstraints(10, 10, 20, 20) };

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            Button modeButton = new Button();
            modeButton.Shortcut = Key.Tab;
            modeButton.Color = Color.FromArgb(100, 255, 100);
            modeButton.SetConstraints(new PixelConstraint(40, RelativeTo.Window, Direction.Left),
                new PixelConstraint(40, RelativeTo.Window, Direction.Bottom),
                new PixelConstraint(300),
                new PixelConstraint(60));

            TextBlock modeText = new TextBlock("Normal", 3);
            modeText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)modeText.TextWidth), new PixelConstraint((int)modeText.TextHeight));
            modeButton.AddChild(modeText);

            modeButton.OnLeftClick += () =>
            {
                normalMode = !normalMode;

                modeText.Text = normalMode ? "Normal" : "Perfekt";
                modeButton.Color = normalMode ? Color.FromArgb(100, 255, 100) : Color.FromArgb(255, 100, 100);
                modeText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)modeText.TextWidth), new PixelConstraint((int)modeText.TextHeight));
            };
            ui.Add(modeButton);

            InitLevel();
        }

        private void InitLevel()
        {
            const int ButtonSize = 60;

            const int starsPerLevel = 2;
            bool[,] stars = new bool[world.Levels.Count, starsPerLevel];
            int totalStars = 0;
            bool[] showLevel = new bool[world.Levels.Count];

            for (int i = 0; i < world.Levels.Count; i++)
            {
                stars[i, 0] = world.Levels[i].Completed;
                totalStars += world.Levels[i].Completed ? 1 : 0;
                stars[i, 1] = world.Levels[i].Perfect;
                totalStars += world.Levels[i].Perfect ? 1 : 0;
            }

            for (int i = 0; i < world.Levels.Count; i++)
            {
                if (i > 1)
                {
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < starsPerLevel; k++)
                            if (stars[i - j, k])
                            {
                                showLevel[i] = true;
                                break;
                            }
                }
                else
                    showLevel[i] = true;
            }

            Frame levelFrame = new Frame()
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(180),
                    new PixelConstraint(showLevel.Count(x => x) * (ButtonSize + 20) + 20),
                    new PixelConstraint(ButtonSize + 40))
            };

            int count = 0;
            for (int i = 0; i < world.Levels.Count; i++)
            {
                if (showLevel[i])
                {
                    Button button = new Button((ButtonSize + 20) * count + 20, 20, ButtonSize, ButtonSize);

                    for (int j = 0; j < starsPerLevel; j++)
                    {
                        UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(stars[i, j] ? 10 : 0, 0, 10, 10));
                        image.SetConstraints(new UIConstraints(
                            ButtonSize * (j + 1) / (starsPerLevel + 1) - 10,
                            ButtonSize - 15, 20, 20));

                        button.AddChild(image);
                    }

                    if (i < 10)
                        button.Shortcut = (Key)(110 + i);

                    if (i > 0)
                        button.Enabled = world.Levels[i - 1].Completed || world.Levels[i - 1].Perfect;

                    int level = i;
                    button.OnLeftClick += () =>
                    {
                        NewGame(level);
                    };

                    TextBlock text = new TextBlock((i + 1).ToString(), 3);
                    text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                    button.AddChild(text);
                    levelFrame.AddChild(button);

                    count++;
                }

                ui.Add(levelFrame);
            }
        }

        private void NewGame(int level)
        {
            Game game = new Game(worldIndex, level, !normalMode);
            game.OnExit += () =>
            {
                game.World.SaveToFile($"{worldIndex:000}");
                SceneManager.LoadScene(new Transition(new WorldMenu(), 10));
            };

            SceneManager.LoadScene(new Transition(game, 10));
        }

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        /// <summary>
        /// Disposes the scene.
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Disposes the scene.
        /// </summary>
        ~LevelMenu()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(false);
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        /// <summary>
        /// Disposes the scene.
        /// </summary>
        public override void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
