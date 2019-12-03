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
            LoadWorld(0);
            LoadLevel(0);
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
            input = new Input(Scenes.Window);
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

        UI.UI ui;
        Input input;
        public HexPoint selectedHex;
        public HexPoint player;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        const float size = 16 * 2;
        HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);
        GridRenderer renderer = new GridRenderer();

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }
        public World World { get => world; }
        private int Levels => world.levels.Count;

        public override void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition);
            selectedHex = mouse;

            if (input.MousePress(MouseButton.Left))
            {
                if (GetPlayerMoves().Contains(mouse))
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

                //int totalWidth = (int)(editor.layout.size * sqrt3 * (maxX - minX + 1));
                //int totalHeight = (int)(editor.layout.size * 1.5f * (maxY - minY + 1.25f));
                renderer.Layout = layout;
            }

            renderer.Grid = Grid;

            if (Settings.Current.ShowMoves)
                renderer.Render(player, selectedHex, GetPlayerMoves());
            else
                renderer.Render(player, selectedHex, null);

            ui.Render(new Vector2(width, height));
            ui.Render(windowSize);
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

        private void CompleteLevel()
        {
            LevelCompleted?.Invoke(levelIndex);

            if (singleLevel)
                Exit?.Invoke();
            else
            {
                Level level = world.levels[levelIndex];
                level.completed = true;
                if (perfect)
                    level.perfect = true;

                world.levels[levelIndex] = level;
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

        #region Events

        public event EmptyEvent Exit;
        public event LevelEvent LevelCompleted;
        public event WorldEvent WorldCompleted;

        #endregion

        #region UI

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(Scenes.Window);

            int size = 4;
            title = new TextBlock("Level 000/000", size);
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(40), new PixelConstraint((int)title.TextWidth), new PixelConstraint(size * 6));
            UpdateTitle();
            title.Color = Color.LightGray;

            ui.Add(title);
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        #endregion

        #region Loading

        private void LoadLevel()
        {
            if (world.levels != null && levelIndex < world.levels.Count)
                level = world.levels[levelIndex].Copy();
            else
                Exit?.Invoke();

            player = level.startPos;
            moves = 0;
            renderer.Grid = level.grid;
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
            world = new World();

            player = level.startPos;
            moves = 0;
        }

        public void LoadWorld()
        {
            world = World.LoadFromFile($"{worldIndex:000}");
            levelIndex = 0;
            UpdateTitle();
            singleLevel = false;
        }

        public void LoadWorld(int index)
        {
            world = World.LoadFromFile($"{index:000}");
            worldIndex = index;
            levelIndex = 0;
            UpdateTitle();
            singleLevel = false;
        }

        public void LoadWorld(World world)
        {
            this.world = world;
            worldIndex = -1;
            levelIndex = 0;
            UpdateTitle();
            singleLevel = false;
        }

        #endregion

        private void UpdateTitle() => title.Text = $"Level {levelIndex + 1:000}/{world.levels?.Count:000}";

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