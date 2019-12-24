using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Networking;
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
            input = SceneManager.Input;
            renderer = new GridRenderer(null, layout);

            InitUI();

            LoadWorld();
            LoadLevel();
        }

        int levelIndex = 0;
        int worldIndex = 0;
        bool ignoreMouse;
        byte? drawType = null;
        World world;
        Level level;
        Game testGame;

        UI.UI ui;
        Input input;
        HexPoint selectedHex;
        HexPoint player;

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

        public event EmptyEvent OnExit;

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }
        public string Path { get; set; } = "Worlds";

        public override void Update()
        {
            for (int i = 0; i < buttonFrame.Children.Count; i++)
            {
                Button item = (Button)buttonFrame.Children[i];
                if (drawType != null && (int)drawType == i)
                {
                    item.Color = Color.Green;
                    HexData data = HexData.Data.First(x => x.ID == i);
                    //textHexDesc.Text = $"Solide:  {(data.Flags.HasFlag(HexFlags.Solid) ? "Ja" : "Nein")}\nTödlich: {(data.Flags.HasFlag(HexFlags.Deadly) ? "Ja" : "Nein")}\nZiel:    {(data.Flags.HasFlag(HexFlags.Goal) ? "Ja" : "Nein")}";
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
            if (!ui.ignoreShortcuts)
            {
                if (input.KeyPress(Key.A))
                {
                    //if (GetPlayerMoves().Contains(mouse))
                    player = mouse;
                }
                if (input.KeyPress(Key.Escape))
                {
                    OnExit?.Invoke();
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
                const int margin = 50;

                float maxX = Grid.Max(x => x.Value.X + x.Value.Y / 2f);
                float minX = Grid.Min(x => x.Value.X + x.Value.Y / 2f);
                int maxY = Grid.Max(x => x.Value.Y);
                int minY = Grid.Min(x => x.Value.Y);

                layout.size = Math.Min((width - margin - leftFrame.GetWidth(windowSize)) / (sqrt3 * (maxX - minX + 1)), (height - margin - lowerFrame.GetHeight(windowSize)) / (1.5f * (maxY - minY + 1.25f)));
                // Round to multiples of 16
                layout.size = (float)Math.Floor(layout.size / 16) * 16;
                layout.size = Math.Min(layout.size, 48) * scale;

                int centerX = (int)(layout.size * sqrt3 * (maxX + minX));
                int centerY = (int)(layout.size * 1.5f * (maxY + minY));

                // Center grid
                layout.origin = new Vector2((width + leftFrame.GetWidth(windowSize) - centerX) / 2, (height - lowerFrame.GetHeight(windowSize) - centerY) / 2) + scrolling;

                //int totalWidth = (int)(layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(layout.size * 1.5f * (maxY - minY + 1.25f));

                renderer.Layout = layout;
            }

            renderer.Render(player, selectedHex, level.GetPossibleMoves(player));

            ui.Render(windowSize);
        }

        #region Loading

        private void TestLevel()
        {
            testGame = new Game();
            testGame.LoadLevel(level);
            testGame.Exit += () =>
            {
                SceneManager.LoadScene(new Transition(this, 10));
            };
            SceneManager.LoadScene(new Transition(testGame, 10));
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

            if (world.Levels == null)
                world.Levels = new List<Level>();

            LoadLevel();
        }

        private void PreviousWorld()
        {
            if (worldIndex > 0)
                worldIndex--;

            LoadWorld();

            if (world.Levels == null)
                world.Levels = new List<Level>();

            LoadLevel();
        }

        private void LoadLevel()
        {
            if (world.Levels != null && levelIndex < world.Levels.Count)
                level = world.Levels[levelIndex];
            else
                level = new Level();

            player = level.startPos;
            renderer.Grid = level.grid;
            scrolling = new Vector2(0, 0);

            textLevel.Text = $"Level {levelIndex + 1:000}";
            textMoves.Text = $"Min. Züge: {level.minMoves:00}";
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{Path}/{worldIndex:000}.wld", false);
            boxWorldName.Text = world.Title ?? "";
            textWorld.Text = $"Welt  {worldIndex + 1:000}";
        }

        private void SaveLevel()
        {
            if (level.grid == null)
                return;

            level.startPos = player;
            level.completed = false;

            if (world.Levels == null)
                world.Levels = new List<Level>();

            while (world.Levels.Count - 1 < levelIndex)
                world.Levels.Add(new Level());

            world.Levels[levelIndex] = level;
        }

        private void SaveWorld()
        {
            SaveLevel();
            world.AllCompleted = false;
            world.AllPerfect = false;
            world.Title = boxWorldName.Text;
            world.SaveToFile($"{Path}/{worldIndex:000}.wld", false);

            Notification.Show("Welt gespeichert!", 2);
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

        #region UI

        TextBlock textLevel;
        TextBlock textWorld;
        TextBlock textMoves;
        TextBlock textHexDesc;
        TextBox boxWorldName;
        Frame buttonFrame;
        Frame lowerFrame;
        Frame leftFrame;

        private void InitUI()
        {
            ui = new UI.UI();

            ui.Input = input;

            {
                const int size = 42;
                const int rowSize = 8;
                const int margin = 5;
                byte[] values = HexData.Data.Select(x => x.ID).OrderBy(x => x).ToArray();

                lowerFrame = new Frame();
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
                    image.SetConstraints(new UIConstraints(6, 5, size - 10, size - 10));
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
            // Level
            {
                leftFrame = new Frame();
                leftFrame.SetConstraints(new UIConstraints(0, 0, 260, 300));

                Button exitButton = new Button(20, 20, 40, 40)
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
                leftFrame.AddChild(exitButton);


                void AddButton1(int x, int y, int w, int h, string s, int tx, int ty, ClickEvent ev, Key shortcut)
                {
                    Button button = new Button(x, y, w, h);
                    button.OnClick += ev;
                    button.Shortcut = shortcut;

                    TextBlock text = new TextBlock(s, 2, tx, ty);

                    button.AddChild(text);
                    leftFrame.AddChild(button);
                }
                void AddButton2(int x, int y, string s, ClickEvent ev, Key shortcut)
                {
                    TextBlock text = new TextBlock(s, 2, 10, 10);

                    Button button = new Button(x, y, (int)text.TextWidth + 18, (int)text.TextHeight + 18);
                    button.OnClick += ev;
                    button.Shortcut = shortcut;
                    
                    button.AddChild(text);
                    leftFrame.AddChild(button);
                }

                textWorld = new TextBlock($"Welt  {worldIndex:000}", 2, 80, 22);
                leftFrame.AddChild(textWorld);

                AddButton1(200, 20, 20, 20, "+", 5, 3, NextWorld, new Key());
                AddButton1(220, 20, 20, 20, "-", 5, 3, PreviousWorld, new Key());

                textLevel = new TextBlock($"Level {levelIndex:000}", 2, 80, 42);
                leftFrame.AddChild(textLevel);

                AddButton1(200, 40, 20, 20, "+", 5, 3, NextLevel, new Key());
                AddButton1(220, 40, 20, 20, "-", 5, 3, PreviousLevel, new Key());

                AddButton2(20, 70, "Neu", NewLevel, new Key());
                AddButton2(100, 70, "Testen", TestLevel, Key.T);

                // Min Moves
                textMoves = new TextBlock($"Min. Züge: {level.minMoves:00}", 2, 20, 122);

                leftFrame.AddChild(textMoves);

                void add()
                {
                    level.minMoves++;
                    textMoves.Text = $"Min. Züge: {level.minMoves:00}";
                }
                AddButton1(180, 120, 20, 20, "+", 5, 3, add, new Key());
                void sub()
                {
                    if (level.minMoves > 0)
                        level.minMoves--;
                    textMoves.Text = $"Min. Züge: {level.minMoves:00}";
                }
                AddButton1(200, 120, 20, 20, "-", 5, 3, sub, new Key());


                AddButton2(20, 150, "Speichern", SaveWorld, Key.S);
                AddButton2(20, 200, "Lösen", () =>
                {
                    LevelSolver solver = new LevelSolver(level);
                    solver.Solved += () => { textHexDesc.Text = $"Min. Züge: {solver.MinMoves}"; };
                    SceneManager.LoadScene(new LoadingScene(solver.SolveAsync, new Transition(this, 10)));
                }, new Key());
                AddButton2(110, 200, "Upload", () =>
                {
                    if (SceneManager.Client?.Connected != true)
                    {
                        Notification.Show("Nicht mit Server verbunden");
                        return;
                    }

                    List<byte> bytes = new List<byte>() { 0x20, 0x02 };

                    bytes.AddRange(world.ToCompressed());

                    void uploaded(Client client, byte[] bytes)
                    {
                        ushort code = BitConverter.ToUInt16(bytes, 0);
                        if (code == 0x0220 && bytes[2] == 0x80)
                        {
                            uint id = BitConverter.ToUInt32(bytes, 3);

                            Notification.Show($"Hochgeladen unter {id.ToString("x")}");

                            SceneManager.Client.BytesRecieved -= uploaded;
                        }
                        else if (code == 0x0220 && bytes[2] == 0xFF)
                        {
                            Notification.Show($"Fehler beim Hochladen");
                            SceneManager.Client.BytesRecieved -= uploaded;
                        }
                    }

                    SceneManager.Client.BytesRecieved += uploaded;
                    SceneManager.Client.Send(bytes.ToArray());

                }, new Key());

                boxWorldName = new TextBox(20, 250, 200, 30)
                {
                    Text = world.Title ?? "",
                    MaxTextSize = 15
                };
                leftFrame.AddChild(boxWorldName);

                ui.Add(leftFrame);
            }
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);

            ignoreMouse = ui.MouseOver(windowSize);
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
                    ui.Dispose();
                    textLevel.Dispose();
                    textMoves.Dispose();
                    textWorld.Dispose();
                    textHexDesc.Dispose();
                    buttonFrame.Dispose();
                    lowerFrame.Dispose();
                    leftFrame.Dispose();
                    testGame?.Dispose();
                }

                renderer = null;
                world = new World();

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