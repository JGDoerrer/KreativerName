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
    public delegate void WorldEvent(int world);
    public delegate void LevelEvent(int level);
    public delegate void EmptyEvent();

    public class Game : Scene
    {
        #region Constructors

        public Game()
        {
            Init();
        }
        public Game(int world)
        {
            Init();
            LoadWorld(world);
            LoadLevel(0);
        }
        public Game(int world, int level)
        {
            Init();
            LoadWorld(world);
            LoadLevel(level);
        }
        public Game(int world, bool perfect)
        {
            Init();
            LoadWorld(world);
            LoadLevel(0);
            this.perfect = perfect;
        }
        public Game(int world, int level, bool perfect)
        {
            Init();
            LoadWorld(world);
            LoadLevel(level);
            this.perfect = perfect;
        }
        public Game(World world)
        {
            Init();
            LoadWorld(world);
            LoadLevel(0);
        }

        private void Init()
        {
            input = new Input(SceneManager.Window);
            InitUI();
        }

        #endregion

        bool singleLevel = false;
        bool perfect = false;
        int levelIndex = 0;
        int worldIndex = 0;
        int moves = 0;
        World world;
        Level level;
        TextBlock title;

        int worldTitle = 0;

        UI.UI ui;
        Input input;
        HexPoint selectedHex;
        HexPoint player;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        const float size = 16 * 2;
        HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);
        GridRenderer renderer = new GridRenderer();

        Vector2 scrolling;
        float scale = 1;

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }
        public World World { get => world; }
        private int Levels => world.Levels.Count;

        public override void Update()
        {
            if (worldTitle > 0)
            {
                if (input.MousePress(MouseButton.Left))
                {
                    if (worldTitle > 120)
                    {
                        worldTitle = 120;
                        input.ReleaseMouse(MouseButton.Left);
                    }
                    else
                        worldTitle = 0;
                }

                worldTitle--;
            }

            HandleInput();
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
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

                layout.size = Math.Min((windowSize.X - margin) / (sqrt3 * (maxX - minX + 1)), (windowSize.Y - margin) / (1.5f * (maxY - minY + 1.25f)));
                // Round to multiples of 16
                layout.size = (float)Math.Floor(layout.size / 16) * 16;
                layout.size = Math.Min(layout.size, 48) * scale;

                int centerX = (int)(layout.size * sqrt3 * (maxX + minX));
                int centerY = (int)(layout.size * 1.5f * (maxY + minY));

                // Center grid
                layout.origin = new Vector2((windowSize.X - centerX) / 2, (windowSize.Y - centerY) / 2) + scrolling;

                //int totalWidth = (int)(editor.layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(editor.layout.size * 1.5f * (maxY - minY + 1.25f));
                renderer.Layout = layout;
            }

            renderer.Grid = Grid;

            if (Settings.Current.ShowMoves)
                renderer.Render(player, selectedHex, level.GetPossibleMoves(player));
            else
                renderer.Render(player, selectedHex, null);

            ui.Render(windowSize);

            if (worldTitle > 0)
            {
                RenderTitle(width, height);
            }
        }

        private void UpdatePlayer()
        {
            moves++;

            if (!singleLevel)
                Stats.Current.TotalMoves++;

            HexFlags flags = Grid[player].Value.Flags;

            if (flags.HasFlag(HexFlags.Deadly))
            {
                if (!singleLevel)
                    Stats.Current.Deaths++;

                LoadLevel();
                return;
            }

            if (flags.HasFlag(HexFlags.Goal))
            {
                CompleteLevel();
                return;
            }

            if (perfect && moves >= level.minMoves)
            {
                LoadLevel();
                return;
            }
        }

        private void HandleInput()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition);
            selectedHex = mouse;

            if (input.MousePress(MouseButton.Left))
            {
                if (level.GetPossibleMoves(player).Contains(mouse))
                {
                    player = mouse;
                    level.Update(player);
                    UpdatePlayer();
                }
            }
            if (input.KeyPress(Key.Escape))
            {
                Exit?.Invoke();
            }

            float scrollSpeed = 8;

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

            input.Update();
        }

        private void CompleteLevel()
        {
            LevelCompleted?.Invoke(levelIndex);

            if (singleLevel)
                Exit?.Invoke();
            else
            {
                Level level = world.Levels[levelIndex];
                level.completed = true;
                if (perfect)
                    level.perfect = true;

                world.Levels[levelIndex] = level;
                //world.SaveToFile($"{worldIndex:000}");

                if (perfect)
                    Stats.Current.LevelsCompletedPerfect++;
                else
                    Stats.Current.LevelsCompleted++;

                if (levelIndex < Levels)
                    levelIndex++;

                if (levelIndex == Levels)
                {
                    WorldCompleted?.Invoke(worldIndex);

                    worldIndex++;
                    levelIndex = 0;
                    Exit?.Invoke();
                }

                UpdateTitle();
                LoadLevel();
            }
        }

        #region Rendering
        
        private void RenderTitle(int width, int height)
        {
            int alpha = (int)((1 - QuarticOut(1 - (float)worldTitle.Clamp(0, 120) / 120)) * 255);

            // Draw black window
            GL.Disable(EnableCap.Texture2D);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb(alpha, 0, 0, 0));
            GL.Vertex2(0, 0);
            GL.Vertex2(0, height);
            GL.Vertex2(width, height);
            GL.Vertex2(width, 0);
            GL.End();
            GL.Enable(EnableCap.Texture2D);

            TextBlock title = new TextBlock(world.Title, 6);
            title.Color = Color.FromArgb(alpha, Color.White);
            title.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            title.Render(new Vector2(width, height));
            title.Dispose();
        }

        private float QuarticOut(float t)
           => -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;

        #endregion

        #region Events

        public event EmptyEvent Exit;
        public event LevelEvent LevelCompleted;
        public event WorldEvent WorldCompleted;

        #endregion

        #region UI

        private void InitUI()
        {
            ui = new UI.UI
            {
                Input = SceneManager.Input
            };

            int size = 4;
            title = new TextBlock("Level 000/000", size);
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(40), new PixelConstraint((int)title.TextWidth), new PixelConstraint(size * 6));
            UpdateTitle();
            title.Color = Color.LightGray;

            ui.Add(title);
        }

        private void UpdateTitle() => title.Text = $"Level {levelIndex + 1:000}/{world.Levels?.Count:000}";

        #endregion

        #region Loading

        private void LoadLevel()
        {
            if (world.Levels != null && levelIndex < world.Levels.Count)
                level = world.Levels[levelIndex].Copy();
            else
                Exit?.Invoke();

            player = level.startPos;
            moves = 0;
            renderer.Grid = level.grid;

            scrolling = new Vector2();
            scale = 1;
        }

        public void LoadLevel(int index)
        {
            levelIndex = index;
            LoadLevel();
        }

        public void LoadLevel(Level level)
        {
            singleLevel = true;
            this.level = level.Copy();
            world = new World
            {
                Levels = new List<Level> { level }
            };

            UpdateTitle();

            player = level.startPos;
            moves = 0;
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            levelIndex = 0;
            singleLevel = false;
            worldTitle = 240;

            UpdateTitle();
        }

        public void LoadWorld(int index)
        {
            world = World.LoadFromFile($"{index:000}");
            worldIndex = index;
            levelIndex = 0;
            singleLevel = false;
            worldTitle = 240;

            UpdateTitle();
        }

        public void LoadWorld(World world)
        {
            this.world = world;
            worldIndex = -1;
            levelIndex = 0;
            singleLevel = false;
            worldTitle = 240;

            UpdateTitle();
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
                    title.Dispose();
                }

                world = new World();
                input = null;
                renderer = null;

                disposedValue = true;
            }
        }

        ~Game()
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