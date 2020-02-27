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

    // TODO: Cleanup code
    /// <summary>
    /// The main scene for the game.
    /// </summary>
    public class Game : Scene
    {
        #region Constructors

        public Game()
        {
            Init();
        }
        public Game(bool perfect)
        {
            this.perfect = perfect;
            Init();
        }
        public Game(World world, int level = 0, bool perfect = false)
        {
            Init();
            LoadWorld(world);
            LoadLevel(level);
            this.perfect = perfect;
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
        World world;
        Level level;
        Engine engine;

        // max value = 240
        int titleAnim = -1;

        const int MAX_LEVEL_ANIM = 80;
        int levelAnim = -1;
        // true when level is done
        bool levelDone = false;
        bool worldDone = false;

        UI.UI ui;
        TextBlock title;
        Input input;
        HexPoint selectedHex;
        HexPoint player;

        const float SQRT3 = 1.732050807568877293527446341505872366942805253810380628055f;
        const float SIZE = 16 * 2;

        HexLayout layout = new HexLayout(
            new Matrix2(SQRT3, SQRT3 / 2f, 0, 3f / 2f),
            new Matrix2(SQRT3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            SIZE, 0.5f);
        GridRenderer renderer = new GridRenderer();

        Vector2 scrolling;
        float scale = 1;

        /// <summary>
        /// The grid of the current level.
        /// </summary>
        public HexGrid<Hex> Grid { get => engine.Level.Grid; set => engine.Level.Grid = value; }

        /// <summary>
        /// The current world.
        /// </summary>
        public World World { get => world; }

        /// <summary>
        /// The amount of moves taken in the level.
        /// </summary>
        public int Moves { get; private set; } = 0;

        private int Levels => world.Levels.Count;


        /// <summary>
        /// Updates the scene.
        /// </summary>
        public override void Update()
        {
            if (titleAnim >= 0)
            {
                if (input.MousePress(MouseButton.Left))
                {
                    if (titleAnim > 120)
                    {
                        titleAnim = 120;
                        input.ReleaseMouse(MouseButton.Left);
                    }
                    else
                        titleAnim = 0;
                }

                titleAnim--;
            }

            if (levelAnim >= 0)
            {
                levelAnim--;
            }
            if (levelAnim <= 0)
            {
                if (worldDone)
                {
                    WorldCompleted?.Invoke(worldIndex);

                    worldIndex++;
                    levelIndex = 0;
                    OnExit?.Invoke();

                    worldDone = true;
                }
                if (levelDone)
                {
                    UpdateTitle();
                    LoadLevel();
                }

                HandleInput();
            }
        }

        private void UpdatePlayer()
        {
            Moves++;

            if (!singleLevel)
                Stats.Current.TotalMoves++;

            if (engine.Players[0].IsDead)
            {
                if (!singleLevel)
                    Stats.Current.Fails++;

                LoadLevel();
                return;
            }

            if (engine.LevelDone)
            {
                CompleteLevel();
                return;
            }

            if (perfect && Moves >= level.MinMoves)
            {
                LoadLevel();
                return;
            }
        }

        private void HandleInput()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition);
            selectedHex = mouse;

            if (input.MousePress(MouseButton.Left) && !levelDone)
            {
                if (engine.GetPossibleMoves(player).Contains(mouse))
                {
                    player = mouse;
                    engine.Update(0,player);
                    UpdatePlayer();
                }
            }

            float scrollSpeed = 4 * (4 + (float)Math.Log(scale, 2));

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
            if (input.MouseScroll() != 0)
            {
                float oldScale = scale;

                scale *= 2.Pow(input.MouseScroll());
                scale = scale.Clamp(0.125f, 16);

                scrolling = scrolling * scale / oldScale;
            }

            input.Update();
        }

        private void CompleteLevel()
        {
            LevelCompleted?.Invoke(levelIndex);

            if (singleLevel)
                OnExit?.Invoke();
            else
            {
                Level level = world.Levels[levelIndex];

                level.Completed = true;
                if (perfect)
                    level.Perfect = true;

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
                    worldDone = true;
                }

                if (Settings.Current.ShowAnimations)
                {
                    levelAnim = MAX_LEVEL_ANIM;
                }

                levelDone = true;
            }
        }

        #region Rendering

        /// <summary>
        /// Renders the scene to the window.
        /// </summary>
        /// <param name="windowSize">The current window size.</param>
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

                layout.size = Math.Min((windowSize.X - margin) / (SQRT3 * (maxX - minX + 1)), (windowSize.Y - margin) / (1.5f * (maxY - minY + 1.25f)));
                // Round size to multiples of 16
                layout.size = (float)Math.Floor(layout.size / 16) * 16;
                layout.size = layout.size.Clamp(16, 64) * scale;

                // Do level animation
                if (levelAnim >= 0)
                {
                    layout.spacing = 1 + 2 * (1 - QuarticOut(levelDone ? (levelAnim / (float)MAX_LEVEL_ANIM) : (1 - levelAnim / (float)MAX_LEVEL_ANIM)));
                    layout.size *= QuarticOut(levelDone ? (levelAnim / (float)MAX_LEVEL_ANIM) : (1 - levelAnim / (float)MAX_LEVEL_ANIM));
                }
                else
                    layout.spacing = 1;

                int centerX = (int)(layout.size * layout.spacing * SQRT3 * (maxX + minX));
                int centerY = (int)(layout.size * layout.spacing * 1.5f * (maxY + minY));

                // Center grid
                layout.origin = new Vector2((windowSize.X - centerX) / 2, (windowSize.Y - centerY) / 2) + scrolling;

            }

            if (perfect)
            {
                engine.Players[0].Color = Color.FromArgb(0, 255, 0).Lerp(Color.FromArgb(255, 0, 0), ((float)Moves / (level.MinMoves)).Clamp(0, 1));
            }

            layout.size = 32;
            renderer.Layout = layout;
            renderer.Grid = Grid;

            if (Settings.Current.ShowMoves)
                renderer.Render(engine.Players[0], selectedHex, engine.GetPossibleMoves(player));
            else
                renderer.Render(engine.Players[0], selectedHex, null);

            ui.Render(windowSize);

            if (titleAnim >= 0)
            {
                RenderTitle(width, height);
            }
        }

        private void RenderTitle(int width, int height)
        {
            int alpha = (int)((1 - QuarticOut(1 - (float)titleAnim.Clamp(0, 120) / 120)) * 255);

            DrawBlackWindow(width, height, alpha);

            TextBlock title = new TextBlock(world.Title, 6)
            {
                Color = Color.FromArgb(alpha, Color.White)
            };
            title.Constraints = new UIConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            title.Render(new Vector2(width, height));
            title.Dispose();
        }

        private static void DrawBlackWindow(int width, int height, int alpha)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb(alpha, 0, 0, 0));
            GL.Vertex2(0, 0);
            GL.Vertex2(0, height);
            GL.Vertex2(width, height);
            GL.Vertex2(width, 0);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }

        private float QuarticOut(float t)
           => -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;

        #endregion

        #region Events

        public event EmptyEvent OnExit;
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

            title = new TextBlock("Level 000/000", 4, 0, 50)
            { Color = Color.LightGray };
            title.Constraints.x = new CenterConstraint();

            ui.Add(title);

            Button exitButton = new Button(20, 20, 40, 40)
            { Shortcut = Key.Escape };
            exitButton.OnLeftClick += (sender) => OnExit?.Invoke();

            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black)
            { Constraints = new UIConstraints(10, 10, 20, 20) };

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);


            Button hintButton = new Button(20, 80, 40, 40);
            hintButton.OnLeftClick += (sender) => Notification.Show($"Hinweis: {level.Hint}");

            UI.Image hintImage = new UI.Image(Textures.Get("Icons"), new RectangleF(50, 10, 10, 10), Color.Black)
            { Constraints = new UIConstraints(10, 10, 20, 20) };

            hintButton.AddChild(hintImage);
            ui.Add(hintButton);
        }

        /// <summary>
        /// Updates the UI of the scene.
        /// </summary>
        /// <param name="windowSize">The current window size.</param>
        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        private void UpdateTitle() => title.Text = $"Level {levelIndex + 1:000}/{world.Levels?.Count:000}";

        #endregion

        #region Loading

        private void LoadLevel()
        {
            if (world.Levels != null && levelIndex < world.Levels.Count)
                level = world.Levels[levelIndex].Copy();
            else
                OnExit?.Invoke();

            engine = new Engine(level);

            player = level.StartPos;
            Moves = 0;
            renderer.Grid = level.Grid;
            renderer.Data = level.Data;

            scrolling = new Vector2();
            scale = 1;

            if (Settings.Current.ShowAnimations)
            {
                levelAnim = MAX_LEVEL_ANIM;
            }

            levelDone = false;
        }

        /// <summary>
        /// Loads a level of the current world.
        /// </summary>
        /// <param name="index">The index of the level in the world.</param>
        public void LoadLevel(int index)
        {
            levelIndex = index;
            LoadLevel();

            UpdateTitle();
        }

        /// <summary>
        /// Loads the given level.
        /// </summary>
        /// <param name="level">The level to be loaded.</param>
        public void LoadLevel(Level level)
        {
            singleLevel = true;
            this.level = level.Copy();
            world = new World
            {
                Levels = new List<Level> { level }
            };

            UpdateTitle();

            engine = new Engine(this.level);
            renderer.Grid = level.Grid;
            renderer.Data = level.Data;

            player = level.StartPos;
            Moves = 0;
        }

        /// <summary>
        /// Loads the world at the current world index.
        /// </summary>
        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            levelIndex = 0;
            singleLevel = false;
            titleAnim = 240;

            UpdateTitle();
        }

        /// <summary>
        /// Loads the world at the given index.
        /// </summary>
        /// <param name="index">The index of the world to be loaded.</param>
        public void LoadWorld(int index)
        {
            world = World.LoadFromFile($"{index:000}");
            worldIndex = index;
            levelIndex = 0;
            singleLevel = false;
            titleAnim = 240;

            UpdateTitle();
        }

        /// <summary>
        /// Loads the given world.
        /// </summary>
        /// <param name="world">The world to be loaded.</param>
        public void LoadWorld(World world)
        {
            this.world = world;
            worldIndex = -1;
            levelIndex = 0;
            singleLevel = false;
            titleAnim = 240;

            UpdateTitle();
        }

        #endregion

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
                    ui.Dispose();
                    title.Dispose();
                }

                world = new World();
                input = null;
                renderer = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the scene.
        /// </summary>
        ~Game()
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