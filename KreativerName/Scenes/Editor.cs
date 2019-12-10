﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class Editor : Scene
    {
        public Editor()
        {
            input = new Input(Scenes.Window);
            renderer = new GridRenderer(null, layout);

            InitUI();

            LoadWorld();
            LoadLevel();
        }

        private void InitUI()
        {
            ui = new UI.UI();

            ui.Input = input;

            // Level
            {
                Frame frame = new Frame();
                frame.SetConstraints(new UIConstaints(20, 20, 220, 240));

                textWorld = new TextBlock($"Welt  {worldIndex:000}", 2);
                textWorld.SetConstraints(new UIConstaints(20, 12, 200, 12));

                frame.AddChild(textWorld);

                textLevel = new TextBlock($"Level {levelIndex:000}", 2);
                textLevel.SetConstraints(new UIConstaints(20, 32, 200, 12));

                frame.AddChild(textLevel);

                // New
                {
                    Button button = new Button(20, 60, 60, 40);
                    button.OnClick += NewLevel;

                    TextBlock text = new TextBlock("Neu", 2, 10, 10);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Test
                {
                    Button button = new Button(100, 60, 90, 40);
                    button.OnClick += TestLevel;
                    button.Shortcut = Key.T;

                    TextBlock text = new TextBlock("Testen", 2, 10, 10);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Save
                {
                    Button button = new Button(20, 170, 130, 40);
                    button.OnClick += SaveWorld;
                    button.Shortcut = Key.S;

                    TextBlock text = new TextBlock("Speichern", 2, 10, 10);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 30, 20, 20);
                    button.OnClick += NextLevel;

                    TextBlock text = new TextBlock("+", 2f, 5, 3);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 30, 20, 20);
                    button.OnClick += PreviousLevel;

                    TextBlock text = new TextBlock("-", 2f, 5, 3);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 10, 20, 20);
                    button.OnClick += NextWorld;

                    TextBlock text = new TextBlock("+", 2f, 5, 3);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 10, 20, 20);
                    button.OnClick += PreviousWorld;

                    TextBlock text = new TextBlock("-", 2f, 5, 3);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Min Moves
                {
                    textMoves = new TextBlock($"Min. Zuege: {level.minMoves:00}", 2, 20, 110);

                    frame.AddChild(textMoves);
                    // +
                    {
                        Button button = new Button(160, 130, 20, 20);
                        button.OnClick += () =>
                        {
                            level.minMoves++;
                            textMoves.Text = $"Min. Zuege: {level.minMoves:00}";
                        };

                        TextBlock text = new TextBlock("+", 2f, 5, 3);

                        button.AddChild(text);
                        frame.AddChild(button);
                    }
                    // -
                    {
                        Button button = new Button(180, 130, 20, 20);
                        button.OnClick += () =>
                        {
                            if (level.minMoves > 0)
                                level.minMoves--;
                            textMoves.Text = $"Min. Zuege: {level.minMoves:00}";
                        };

                        TextBlock text = new TextBlock("-", 2f, 5, 3);

                        button.AddChild(text);
                        frame.AddChild(button);
                    }
                }

                ui.Add(frame);
            }
            {
                const int size = 42;
                const int rowSize = 8;
                const int margin = 5;
                byte[] values = HexData.Data.Select(x => x.ID).OrderBy(x => x).ToArray();

                Frame lowerFrame = new Frame();
                lowerFrame.SetConstraints(
                   new PixelConstraint(0),
                   new PixelConstraint(0, RelativeTo.Window, Direction.Bottom),
                   new RelativeConstraint(1, RelativeTo.Window),
                   new PixelConstraint((values.Length / rowSize + 1) * (size + margin) + (40 - margin)));

                buttonFrame = new Frame();
                buttonFrame.SetConstraints(
                    new PixelConstraint(20),
                    new PixelConstraint(20),
                    new PixelConstraint(0),
                    new PixelConstraint(0));

                // Hexagons
                for (int i = 0; i < values.Length; i++)
                {
                    Button button = new Button((i % rowSize) * (size + margin), (i / rowSize) * (size + margin), size, size);

                    UI.Image image = new UI.Image(Textures.Get("Hex"), new RectangleF(32 * values[i], 0, 32, 32));
                    image.SetConstraints(new UIConstaints(6, 5, size - 10, size - 10));
                    button.AddChild(image);
                    button.Shortcut = (Key)(110 + i);

                    int copy = i;
                    button.OnClick += () =>
                    {
                        drawType = values[copy];
                    };

                    buttonFrame.AddChild(button);
                }

                textHexDesc = new TextBlock("", 2, rowSize * (size + margin) + 40, 20);

                lowerFrame.AddChild(buttonFrame);
                lowerFrame.AddChild(textHexDesc);

                ui.Add(lowerFrame);
            }
        }

        int levelIndex = 0;
        int worldIndex = 0;
        bool ignoreMouse;
        byte? drawType = null;
        World world;
        Level level;
        Game testGame;

        TextBlock textLevel;
        TextBlock textWorld;
        TextBlock textMoves;
        TextBlock textHexDesc;
        Frame buttonFrame;

        public UI.UI ui;
        public Input input;
        public HexPoint selectedHex;
        public HexPoint player;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        const float hexSize = 16 * 2;
        public HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            hexSize, 0.5f);
        Vector2 scrolling;
        float scale = 1;
        GridRenderer renderer;

        public event EmptyEvent Exit;

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public override void Update()
        {
            for (int i = 0; i < buttonFrame.Children.Count; i++)
            {
                Button item = (Button)buttonFrame.Children[i];
                if (drawType != null && (int)drawType == i)
                {
                    item.Color = Color.Green;
                    HexData data = HexData.Data.First(x => x.ID == i);
                    textHexDesc.Text = $"Solide:  {(data.Flags.HasFlag(HexFlags.Solid) ? "Ja" : "Nein")}\nTödlich: {(data.Flags.HasFlag(HexFlags.Deadly) ? "Ja" : "Nein")}\nZiel:    {(data.Flags.HasFlag(HexFlags.Goal) ? "Ja" : "Nein")}";
                }
                else
                    item.Color = Color.White;

            }

            HandleInput();

            input.Update();
        }

        private void HandleInput()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition);
            selectedHex = mouse;
            
            float scrollSpeed = 8;

            if (!ignoreMouse)
            {
                if (input.MouseDown(MouseButton.Left))
                {
                    if (Grid != null && Grid[mouse] != null && drawType != null)
                    {
                        Hex hex = Grid[mouse].Value;

                        if (drawType.HasValue && (drawType == 0))
                            hex.IDs = new List<byte> { drawType.Value };
                        else if (drawType.HasValue && !hex.IDs.Contains(drawType.Value))
                            hex.IDs.Add(drawType.Value);

                        Grid[mouse] = hex;
                    }
                }
                if (input.MousePress(MouseButton.Right))
                {
                    if (Grid != null)
                    {
                        if (Grid[mouse].HasValue)
                            Grid[mouse] = null;
                        else
                            Grid[mouse] = new Hex(mouse, 0);
                    }
                }
            }
            if (input.KeyPress(Key.A))
            {
                //if (GetPlayerMoves().Contains(mouse))
                player = mouse;
            }
            if (input.KeyPress(Key.Escape))
            {
                Exit?.Invoke();
            }
            if (input.KeyDown(Key.Left))
            {
                scrolling.X += scrollSpeed;
            }
            if (input.KeyDown(Key.Right))
            {
                scrolling.X -= scrollSpeed;
            }
            if (input.KeyDown(Key.Up))
            {
                scrolling.Y += scrollSpeed;
            }
            if (input.KeyDown(Key.Down))
            {
                scrolling.Y -= scrollSpeed;
            }
            scale *= (float)Math.Pow(2, input.MouseScroll());

            scale = scale.Clamp(0.125f, 16);
        }

        public override void Render(Vector2 windowSize)
        {
            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            GL.ClearColor(Color.FromArgb(255, 0, 0, 0));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if (Grid != null)
            {
                const int margin = 100;

                float maxX = Grid.Max(x => x.Value.X + x.Value.Y / 2f);
                float minX = Grid.Min(x => x.Value.X + x.Value.Y / 2f);
                int maxY = Grid.Max(x => x.Value.Y);
                int minY = Grid.Min(x => x.Value.Y);

                layout.size = Math.Min((width - margin) / (sqrt3 * (maxX - minX + 1)), (height - margin) / (1.5f * (maxY - minY + 1.25f)));
                // Round to multiples of 16
                layout.size = (float)Math.Floor(layout.size / 16) * 16;
                layout.size = Math.Min(layout.size, 48) * scale;

                int centerX = (int)(layout.size * sqrt3 * (maxX + minX));
                int centerY = (int)(layout.size * 1.5f * (maxY + minY));

                // Center grid
                layout.origin = new Vector2((width - centerX) / 2, (height - centerY) / 2) + scrolling;

                //int totalWidth = (int)(layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(layout.size * 1.5f * (maxY - minY + 1.25f));

                renderer.Layout = layout;
            }

            renderer.Render(player, selectedHex, GetPlayerMoves());

            ui.Render(windowSize);
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);

            ignoreMouse = ui.MouseOver(windowSize);
        }

        public List<HexPoint> GetPlayerMoves()
        {
            HexPoint[] directions = {
                new HexPoint( 1,  0),
                new HexPoint( 1, -1),
                new HexPoint( 0, -1),
                new HexPoint(-1,  0),
                new HexPoint(-1,  1),
                new HexPoint( 0,  1),
            };

            if (Grid == null)
                return new List<HexPoint>();

            List<HexPoint> moves = new List<HexPoint>();

            for (int i = 0; i < 6; i++)
            {
                int j = 1;

                while (Grid[(directions[i] * j) + player].HasValue && !Grid[(directions[i] * j) + player].Value.Flags.HasFlag(HexFlags.Solid))
                {
                    moves.Add(directions[i] * j + player);
                    j++;
                }
            }

            return moves;
        }

        #region Loading

        private void TestLevel()
        {
            testGame = new Game();
            testGame.LoadLevel(level);
            testGame.Exit += () =>
            {
                Scenes.LoadScene(new Transition(this, 10));
            };
            Scenes.LoadScene(new Transition(testGame, 10));
        }

        private void NextLevel()
        {
            levelIndex++;
            LoadLevel();
        }

        private void PreviousLevel()
        {
            if (levelIndex > 0)
                levelIndex--;
            LoadLevel();
        }

        private void NextWorld()
        {
            worldIndex++;

            LoadWorld();

            if (world.levels == null)
                world.levels = new List<Level>();

            LoadLevel();
        }

        private void PreviousWorld()
        {
            if (worldIndex > 0)
                worldIndex--;

            LoadWorld();

            if (world.levels == null)
                world.levels = new List<Level>();

            LoadLevel();
        }

        private void LoadLevel()
        {
            if (world.levels != null && levelIndex < world.levels.Count)
                level = world.levels[levelIndex];
            else
                level = new Level();

            player = level.startPos;
            renderer.Grid = level.grid;
            scrolling = new Vector2(0, 0);

            textLevel.Text = $"Level {levelIndex + 1:000}";
            textMoves.Text = $"Min. Zuege: {level.minMoves:00}";
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            textWorld.Text = $"Welt  {worldIndex + 1:000}";
        }

        private void SaveLevel()
        {
            level.startPos = player;
            level.completed = false;

            if (world.levels == null)
                world.levels = new List<Level>();

            while (world.levels.Count - 1 < levelIndex)
                world.levels.Add(new Level());

            world.levels[levelIndex] = level;
        }

        private void SaveWorld()
        {
            SaveLevel();
            world.AllCompleted = false;
            world.AllPerfect = false;
            world.SaveToFile($"{worldIndex:000}");
        }

        private void NewLevel()
        {
            Grid = new HexGrid<Hex>();

            int w = 10;
            int h = 5;

            // make rectangle
            for (int j = 0; j < h; j++)
            {
                for (int i = -(j / 2); i < w - (j + 1) / 2; i++)
                {
                    Grid[i, j] = new Hex(i, j, 0);
                }
            }

            renderer.Grid = Grid;
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
                    ui?.Dispose();
                    textLevel?.Dispose();
                    textMoves?.Dispose();
                    textWorld?.Dispose();
                    textHexDesc?.Dispose();
                    buttonFrame?.Dispose();
                    testGame?.Dispose();
                }

                renderer = null;

                disposedValue = true;
            }
        }

        ~Editor()
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