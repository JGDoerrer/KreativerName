using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class WorldMenu : Scene
    {
        public WorldMenu()
        {
            InitUI();
        }

        UI.UI ui;

        bool normalMode = true;

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(SceneManager.Window);

            TextBlock title = new TextBlock("Welten", 4);
            title.Color = Color.White;
            title.SetConstraints(
                new CenterConstraint(),
                new PixelConstraint(50),
                new PixelConstraint((int)title.TextWidth),
                new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

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

            Button exitButton = new Button(40, 40, 40, 40);
            exitButton.Shortcut = Key.Escape;
            exitButton.OnLeftClick += () =>
            {
                SceneManager.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            InitWorlds();
        }

        private void InitWorlds()
        {
            const int ButtonSize = 60;
            List<World> worlds = new List<World>();
            int worldcount = 0;
            while (File.Exists($@"Resources\Worlds\{worldcount:000}.wld"))
            {
                worlds.Add(World.LoadFromFile($"{worldcount:000}"));
                worldcount++;
            }

            const int starsPerWorld = 2;
            bool[,] stars = new bool[worldcount, starsPerWorld];
            int totalStars = 0;
            bool[] showWorld = new bool[worldcount];

            for (int i = 0; i < worldcount; i++)
            {
                stars[i, 0] = worlds[i].AllCompleted;
                totalStars += worlds[i].AllCompleted ? 1 : 0;
                stars[i, 1] = worlds[i].AllPerfect;
                totalStars += worlds[i].AllPerfect ? 1 : 0;
            }

            for (int i = 0; i < worldcount; i++)
            {
                if (i > 1)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < starsPerWorld; k++)
                        {
                            if (stars[i - j, k])
                            {
                                showWorld[i] = true;
                                break;
                            }
                        }
                    }
                }
                else
                    showWorld[i] = true;
            }

            Frame worldFrame = new Frame
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(180),
                    new PixelConstraint(showWorld.Count(x => x) * (ButtonSize + 20) + 20),
                    new PixelConstraint(ButtonSize + 40))
            };

            int count = 0;
            for (int i = 0; i < worldcount; i++)
            {
                if (showWorld[i])
                {
                    Button button = new Button((ButtonSize + 20) * count + 20, 20, ButtonSize, ButtonSize);

                    for (int j = 0; j < starsPerWorld; j++)
                    {
                        UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(stars[i, j] ? 10 : 0, 0, 10, 10));
                        image.SetConstraints(new UIConstraints(
                            ButtonSize * (j + 1) / (starsPerWorld + 1) - 10,
                            ButtonSize - 15, 20, 20));

                        button.AddChild(image);
                    }

                    if (i < 10)
                        button.Shortcut = (Key)(110 + i);

                    if (i > 0)
                        button.Enabled = worlds[i - 1].AllCompleted || worlds[i - 1].AllPerfect;

                    int world = i;
                    button.OnLeftClick += () =>
                    {
                        NewGame(world);
                    };

                    TextBlock text = new TextBlock((i + 1).ToRoman().ToLower(), 3);
                    text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                    button.AddChild(text);
                    worldFrame.AddChild(button);

                    count++;
                }
            }

            ui.Add(worldFrame);
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

        private void NewGame(int world)
        {
            Game game = new Game(world, !normalMode);
            game.OnExit += () =>
            {
                game.World.SaveToFile($"{world:000}");
                SceneManager.LoadScene(new Transition(new WorldMenu(), 10));
            };

            SceneManager.LoadScene(new Transition(game, 10));
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

        ~WorldMenu()
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
