using System;
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
            InitUI();

            LoadWorld();
            LoadLevel();
        }

        private void InitUI()
        {
            editorUi = new UI.UI();

            editorUi.Input = new Input(Scenes.Window);

            // Levelselection
            {
                Frame frame = new Frame();
                frame.SetConstraints(new PixelConstraint(20), new PixelConstraint(20), new PixelConstraint(220), new PixelConstraint(240));

                textWorld = new TextBlock($"Welt  {worldIndex:000}", 2);
                textWorld.SetConstraints(new PixelConstraint(20), new PixelConstraint(12), new PixelConstraint(200), new PixelConstraint(12));

                frame.AddChild(textWorld);

                textLevel = new TextBlock($"Level {levelIndex:000}", 2);
                textLevel.SetConstraints(new PixelConstraint(20), new PixelConstraint(32), new PixelConstraint(200), new PixelConstraint(12));

                frame.AddChild(textLevel);

                // Test
                {
                    Button button = new Button(20, 50, 90, 40);
                    button.OnClick += TestLevel;

                    TextBlock text = new TextBlock("Testen", 2, 10, 10);

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Generate
                {
                    Button button = new Button(20, 110, 60, 40);
                    button.OnClick += NewLevel;

                    TextBlock text = new TextBlock("Neu", 2);
                    text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(90), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Save
                {
                    Button button = new Button(20, 170, 130, 40);
                    button.OnClick += SaveWorld;

                    TextBlock text = new TextBlock("Speichern", 2);
                    text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(130), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 30, 20, 20);
                    button.OnClick += NextLevel;

                    TextBlock text = new TextBlock("+", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 30, 20, 20);
                    button.OnClick += PreviousLevel;

                    TextBlock text = new TextBlock("-", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // +
                {
                    Button button = new Button(140, 10, 20, 20);
                    button.OnClick += NextWorld;

                    TextBlock text = new TextBlock("+", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // -
                {
                    Button button = new Button(160, 10, 20, 20);
                    button.OnClick += PreviousWorld;

                    TextBlock text = new TextBlock("-", 2f);
                    text.SetConstraints(new PixelConstraint(5), new PixelConstraint(3), new PixelConstraint(80), new RelativeConstraint(1));

                    button.AddChild(text);
                    frame.AddChild(button);
                }
                // Text
                {
                    TextBox box = new TextBox();
                    box.SetConstraints(
                        new PixelConstraint(20),
                        new PixelConstraint(230),
                        new PixelConstraint(150),
                        new PixelConstraint(40));

                    frame.AddChild(box);
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

        TextBlock textLevel;
        TextBlock textWorld;
        Frame buttonFrame;

        public UI.UI editorUi;
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

        public event EmptyEvent Exit;

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public override void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition);
            selectedHex = mouse;

            for (int i = 0; i < buttonFrame.Children.Count; i++)
            {
                Button item = (Button)buttonFrame.Children[i];
                item.Color = drawType != null && (int)drawType == 1 << i ? Color.Green : Color.White;
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
                layout.size = Math.Min(layout.size, 48);

                int centerX = (int)(layout.size * sqrt3 * (maxX + minX));
                int centerY = (int)(layout.size * 1.5f * (maxY + minY));

                // Center grid
                layout.origin = new Vector2((width - centerX) / 2, (height - centerY) / 2);

                //int totalWidth = (int)(layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(layout.size * 1.5f * (maxY - minY + 1.25f));
            }

            GridRenderer.RenderGrid(Grid, layout, GetPlayerMoves(), selectedHex, player);

            editorUi.Render(windowSize);
        }

        public override void UpdateUI(Vector2 windowSize)
        {
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
                input = Scenes.Input
            };
            testGame.LoadLevel(level);
            testGame.Exit += () =>
            {
                Scenes.LoadScene(this);
                level = copy;
            };
            drawType = null;
            Scenes.LoadScene(testGame);
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
            textLevel.Text = $"Level {levelIndex + 1:000}";
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            textWorld.Text = $"Welt  {worldIndex + 1:000}";
        }

        private void SaveLevel()
        {
            level.startPos = player;
            if (world.levels == null)
                world.levels = new List<Level>();

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