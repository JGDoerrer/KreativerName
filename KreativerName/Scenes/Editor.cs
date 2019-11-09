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
    public class Editor : Scene
    {
        public Editor()
        {
            InitUI();

            LoadWorld();
            LoadLevel();

            renderer = new EditorRenderer(this);
        }

        private void InitUI()
        {
            editorUi = new UI.UI();

            // Levelselection
            {
                Frame frame = new Frame();
                frame.SetConstraints(new PixelConstraint(20), new PixelConstraint(20), new PixelConstraint(220), new PixelConstraint(240));

                textWorld = new Text($"Welt  {worldIndex:000}", 2);
                textWorld.SetConstraints(new PixelConstraint(20), new PixelConstraint(12), new PixelConstraint(200), new PixelConstraint(12));

                frame.AddChild(textWorld);

                textLevel = new Text($"Level {levelIndex:000}", 2);
                textLevel.SetConstraints(new PixelConstraint(20), new PixelConstraint(32), new PixelConstraint(200), new PixelConstraint(12));

                frame.AddChild(textLevel);

                // Test
                {
                    Button button = new Button(20, 50, 90, 40);
                    button.OnClick += TestLevel;

                    Text text = new Text("Testen", 2, 10, 10);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Generate
                {
                    Button button = new Button(20, 110, 60, 40);
                    button.OnClick += NewLevel;

                    Text text = new Text("Neu", 2);
                    text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(90), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Save
                {
                    Button button = new Button(20, 170, 130, 40);
                    button.OnClick += SaveWorld;

                    Text text = new Text("Speichern", 2);
                    text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(130), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 30, 20, 20);
                    button.OnClick += NextLevel;

                    Text text = new Text("+", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 30, 20, 20);
                    button.OnClick += PreviousLevel;

                    Text text = new Text("-", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 10, 20, 20);
                    button.OnClick += NextWorld;

                    Text text = new Text("+", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 10, 20, 20);
                    button.OnClick += PreviousWorld;

                    Text text = new Text("-", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }

                editorUi.Add(frame);
            }
            // Hexagons
            {
                const int size = 42;
                HexType[] values = (HexType[])Enum.GetValues(typeof(HexType));

                buttonFrame = new Frame();
                buttonFrame.SetConstraints(
                   new PixelConstraint(20, RelativeTo.Window),
                   new PixelConstraint(20, RelativeTo.Window, Direction.Bottom),
                   new PixelConstraint(values.Length * (size + 10) + 30),
                   new PixelConstraint(size + 40));

                for (int i = 0; i < values.Length; i++)
                {
                    Button button = new Button(i * (size + 10) + 20, 20, size, size);

                    UI.Image image = new UI.Image(Textures.Get("Hex"), new RectangleF(32 * i, 0, 32, 32));
                    image.SetConstraints(
                        new PixelConstraint(6),
                        new PixelConstraint(5),
                        new PixelConstraint(size - 10),
                        new PixelConstraint(size - 10));
                    button.AddChild(image);

                    int copy = i;
                    button.OnClick += () =>
                    {
                        drawType = values[copy];
                    };

                    buttonFrame.AddChild(button);
                }
                editorUi.Add(buttonFrame);
            }
        }

        int levelIndex = 0;
        int worldIndex = 0;
        HexType? drawType = null;
        World world;
        Level level;
        Game testGame;

        Text textLevel;
        Text textWorld;
        Frame buttonFrame;
        EditorRenderer renderer;

        public UI.UI editorUi;
        public Input input;
        public HexPoint selectedHex;
        public HexPoint player;

        const float hexSize = 16 * 2;
        public HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            hexSize, 0.5f);

        public event EmptyEvent Exit;

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public override void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition());
            selectedHex = mouse;

            for (int i = 0; i < buttonFrame.Children.Count; i++)
            {
                Button item = (Button)buttonFrame.Children[i];
                item.Color = drawType != null && (int)drawType == i ? Color.Green : Color.White;
            }

            if (input.MouseDown(MouseButton.Left))
            {
                if (Grid != null && Grid[mouse] != null && drawType != null)
                {
                    Hex hex = Grid[mouse].Value;
                    hex.Type = drawType.Value;
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
            if (input.KeyPress(Key.A))
            {
                //if (GetPlayerMoves().Contains(mouse))
                player = mouse;
            }
            if (input.KeyPress(Key.Escape))
            {
                Exit?.Invoke();
            }

            input.Update();
        }

        public override void Render(Vector2 windowSize)
        {
            renderer.Render(windowSize);
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            editorUi.SetMouseState(input.MouseState());
            editorUi.Update(windowSize);
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

                while (Grid[(directions[i] * j) + player].HasValue && Grid[(directions[i] * j) + player].Value.Type != HexType.Solid)
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
            Level copy = new Level();
            // copy
            copy.FromBytes(level.ToBytes(),0);

            testGame = new Game
            {
                input = MainWindow.input
            };
            testGame.LoadLevel(level);
            testGame.Exit += () =>
            {
                MainWindow.scene = this;
                level = copy;
            };
            drawType = null;
            MainWindow.scene = testGame;
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
            if (levelIndex < world.levels.Count)
                level = world.levels[levelIndex];
            else
                level = new Level();

            player = level.startPos;
            textLevel.String = $"Level {levelIndex + 1:000}";
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            textWorld.String = $"Welt  {worldIndex + 1:000}";
        }

        private void SaveLevel()
        {
            level.startPos = player;
            while (world.levels.Count - 1 < levelIndex)
                world.levels.Add(new Level());

            world.levels[levelIndex] = level;
        }

        private void SaveWorld()
        {
            SaveLevel();
            world.SaveToFile($"{worldIndex:000}");
        }

        private void NewLevel()
        {
            Grid = new HexGrid<Hex>();

            int w = 11;
            int h = 3;

            for (int j = 0; j < h; j++)
            {
                for (int i = -(j / 2); i < w - (j + 1) / 2; i++)
                {
                    Grid[i, j] = new Hex(i, j, 0);
                }
            }
        }

        #endregion
    }
}