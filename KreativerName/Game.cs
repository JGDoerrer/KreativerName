using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName
{
    public delegate void WorldEvent(int world);
    public delegate void LevelEvent(int level);
    public delegate void EmptyEvent();

    public class Game
    {
        public Game()
        {
            InitUI();
            LoadLevel();
        }

        bool singleLevel = true;
        int levelIndex = 0;
        int worldIndex = 0;
        int moves = 0;
        World world;
        Level level;
        Text title;

        public UI.UI ui;
        public Input input;
        public HexPoint selectedHex;
        public HexPoint player;
        public bool loadNextLevel = true;

        const float size = 16 * 2;
        public HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }
        public World World { get => world; }
        private int Levels => world.levels.Count;

        public void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition());
            selectedHex = mouse;

            if (input.MousePress(MouseButton.Left))
            {
                if (GetPlayerMoves().Contains(mouse))
                {
                    player = mouse;
                    moves++;
                }
            }
            if (input.KeyPress(Key.Escape))
            {
                Exit?.Invoke();
            }

            if (Grid != null)
            {
                switch (Grid[player]?.Type)
                {
                    case HexType.Deadly: LoadLevel(); break;
                    case HexType.Goal:
                    {
                        LevelCompleted?.Invoke(levelIndex);

                        if (loadNextLevel)
                        {
                            if (levelIndex < Levels)
                                levelIndex++;

                            if (levelIndex == Levels)
                            {
                                WorldCompleted?.Invoke(worldIndex);

                                worldIndex++;
                                levelIndex = 0;
                                LoadWorld();
                            }

                            UpdateTitle();
                            LoadLevel();
                        }
                        break;
                    }
                    default:
                        break;
                }
            }

            input.Update();
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

        #region Events

        public event EmptyEvent Exit;
        public event LevelEvent LevelCompleted;
        public event WorldEvent WorldCompleted;

        #endregion

        #region UI

        public void InitUI()
        {
            ui = new UI.UI();

            int size = 4;
            title = new Text("", size);
            UpdateTitle();
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(40), new PixelConstraint(size * 13 * 6), new PixelConstraint(size * 6));
            title.Color = Color.LightGray;

            ui.Add(title);
        }

        public void UpdateUI(Vector2 windowSize)
        {
            ui.SetMouseState(input.MouseState());
            ui.Update(windowSize);
        }

        #endregion

        #region Loading

        private void LoadLevel()
        {
            if (singleLevel)
                level = Level.LoadFromFile($"{levelIndex:000}");
            else if (world.levels != null && levelIndex < world.levels.Count)
                level = world.levels[levelIndex];
            else
                Exit?.Invoke();

            player = level.startPos;
        }

        public void LoadLevel(int index)
        {
            levelIndex = index;
            LoadLevel();
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

        #endregion

        private void UpdateTitle() => title.String = $"Level {levelIndex + 1:000}/{world.levels?.Count:000}";
    }
}
