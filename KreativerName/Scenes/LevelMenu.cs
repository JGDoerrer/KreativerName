﻿using System;
using System.Collections.Generic;
using System.Drawing;
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

            for (int i = 0; i < this.world.Levels.Count; i++)
            {
                if (this.world.LevelConnections[i] == null)
                    this.world.LevelConnections[i] = new List<int>();
            }

            InitUI();
            InitLevels();
        }

        bool normalMode = true;
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
            ui = new UI.UI
            {
                Input = new Input(SceneManager.Window)
            };

            TextBlock title = new TextBlock("Level", 4, 0, 50) { Color = Color.White };
            title.Constraints.x = new CenterConstraint();
            ui.Add(title);


            Button exitButton = new Button(20, 20, 40, 40) { Shortcut = Key.Escape };
            exitButton.OnLeftClick += (sender) => SceneManager.LoadScene(new Transition(new WorldMenu(), 10));

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

            modeButton.OnLeftClick += (sender) =>
            {
                normalMode = !normalMode;

                modeText.Text = normalMode ? "Normal" : "Perfekt";
                modeButton.Color = normalMode ? Color.FromArgb(100, 255, 100) : Color.FromArgb(255, 100, 100);
                modeText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)modeText.TextWidth), new PixelConstraint((int)modeText.TextHeight));
            };
            ui.Add(modeButton);
        }

        private void InitLevels()
        {
            const int ButtonSize = 60;
            const int StarsPerLevel = 2;

            int lvlCount = world.Levels.Count;

            // Read stars
            bool[,] stars = new bool[lvlCount, StarsPerLevel];
            int totalStars = 0;

            for (int i = 0; i < lvlCount; i++)
            {
                stars[i, 0] = world.Levels[i].Completed;
                totalStars += world.Levels[i].Completed ? 1 : 0;
                stars[i, 1] = world.Levels[i].Perfect;
                totalStars += world.Levels[i].Perfect ? 1 : 0;
            }

            Frame levelFrame = new Frame()
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(180),
                    new PixelConstraint(lvlCount * (ButtonSize + 20) + 20),
                    new PixelConstraint(ButtonSize + 40))
            };

            List<Tuple<int, int>> connections = new List<Tuple<int, int>>();

            for (int i = 0; i < lvlCount; i++)
            {
                for (int j = 0; j < world.LevelConnections[i].Count; j++)
                {
                    connections.Add(new Tuple<int, int>(i, world.LevelConnections[i][j]));
                }
            }

            Vector2[] positions = new Vector2[lvlCount];

            void CalcPosition(int level, int index, int prev = 0, int count = 0)
            {
                int next = world.LevelConnections[level][index];

                if (world.LevelConnections[next].Count > 0)
                {
                    for (int i = 0; i < world.LevelConnections[next].Count; i++)
                    {
                        CalcPosition(next, i, level, count + 1);
                    }
                }
                else
                    positions[next] = new Vector2(count + 1, index);

                positions[level] = new Vector2(count, index);
            }

            CalcPosition(0, 0);

            // Create buttons
            int count = 0;
            for (int i = 0; i < lvlCount; i++)
            {
                Button button = new Button((ButtonSize + 20) * (int)positions[i].X + 20, (ButtonSize + 20) * (int)positions[i].Y + 20, ButtonSize, ButtonSize);

                // Create stars
                for (int j = 0; j < StarsPerLevel; j++)
                {
                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(stars[i, j] ? 10 : 0, 0, 10, 10));
                    image.SetConstraints(new UIConstraints(
                        ButtonSize * (j + 1) / (StarsPerLevel + 1) - 10,
                        ButtonSize - 15, 20, 20));

                    button.AddChild(image);
                }

                if (i < 10)
                    button.Shortcut = (Key)(110 + i);

                if (i > 0)
                {
                    button.Enabled = false;
                    for (int j = 0; j < world.LevelConnections.Length; j++)
                    {
                        if (world.Levels[j].Completed && world.LevelConnections[j].Contains(i))
                        {
                            button.Enabled = true;
                            break;
                        }
                    }
                }

                int level = i;
                button.OnLeftClick += (sender) => NewGame(level);

                TextBlock text = new TextBlock((i + 1).ToString(), 3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                button.AddChild(text);
                levelFrame.AddChild(button);

                count++;
            }

            ui.Add(levelFrame);
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
