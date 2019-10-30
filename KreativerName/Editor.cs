using System;
using System.Collections.Generic;
using System.Linq;
using KreativerName.Grid;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName
{
    public class Editor
    {
        public Editor()
        {
            InitUI();
            LoadLevel();
        }

        private void InitUI()
        {
            editorUi = new UI.UI();

            Frame frame = new Frame();
            frame.SetConstraints(new PixelConstraint(20), new PixelConstraint(20), new PixelConstraint(220), new PixelConstraint(220));

            title = new Text($"Level {levelCount:000}", 2);
            title.SetConstraints(new PixelConstraint(20), new PixelConstraint(10), new PixelConstraint(9 * 12), new PixelConstraint(12));

            frame.AddChild(title);

            {
                Button button = new Button();
                button.OnClicked += LoadLevel;
                button.SetConstraints(new PixelConstraint(20), new PixelConstraint(40), new PixelConstraint(90), new PixelConstraint(40));

                Text text = new Text("Reload", 2);
                text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(80), new RelativeConstraint(1));

                button.AddChild(text);
                frame.AddChild(button);
            }
            {
                Button button = new Button();
                button.OnClicked += GenerateLevel;
                button.SetConstraints(new PixelConstraint(20), new PixelConstraint(100), new PixelConstraint(120), new PixelConstraint(40));

                Text text = new Text("Generate", 2);
                text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(80), new RelativeConstraint(1));

                button.AddChild(text);
                frame.AddChild(button);
            }
            {
                Button button = new Button();
                button.OnClicked += SaveLevel;
                button.SetConstraints(new PixelConstraint(20), new PixelConstraint(160), new PixelConstraint(80), new PixelConstraint(40));

                Text text = new Text("Save", 2);
                text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(80), new RelativeConstraint(1));

                button.AddChild(text);
                frame.AddChild(button);
            }
            {
                Button button = new Button();
                button.OnClicked += NextLevel;
                button.SetConstraints(new PixelConstraint(120), new PixelConstraint(40), new PixelConstraint(40), new PixelConstraint(40));

                Text text = new Text("+", 2);
                text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(80), new RelativeConstraint(1));

                button.AddChild(text);
                frame.AddChild(button);
            }
            {
                Button button = new Button();
                button.OnClicked += PreviousLevel;
                button.SetConstraints(new PixelConstraint(160), new PixelConstraint(40), new PixelConstraint(40), new PixelConstraint(40));

                Text text = new Text("-", 2);
                text.SetConstraints(new PixelConstraint(10), new PixelConstraint(10), new PixelConstraint(80), new RelativeConstraint(1));

                button.AddChild(text);
                frame.AddChild(button);
            }

            editorUi.Add(frame);
        }

        int levelCount = 0;
        Level level;
        Text title;

        public UI.UI editorUi;
        public Input input;
        public HexPoint selectedHex;
        public HexPoint player;

        const float size = 16 * 2;
        public HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);

        public event LevelEvent LevelCompleted;
        public event EmptyEvent Exit;

        static Random random = new Random();

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition());
            selectedHex = mouse;

            if (input.MousePress(MouseButton.Left))
            {
                if (GetPlayerMoves().Contains(mouse))
                    player = mouse;
            }
            if (input.MousePress(MouseButton.Right))
            {
                HexType[] types =
                {
                    0,
                    HexType.Solid,
                    HexType.Deadly,
                    HexType.Goal,
                };

                if (Grid != null && Grid[mouse] != null)
                {
                    Hex hex = Grid[mouse].Value;
                    if (types.Contains(hex.Type))
                    {
                        int index = Array.IndexOf(types, hex.Type);
                        hex.Type = types[(index + 1) % types.Length];
                    }

                    Grid[mouse] = hex;
                }
            }
            if (input.MousePress(MouseButton.Middle))
            {
                if (Grid != null)
                {
                    if (Grid[mouse].HasValue)
                        Grid[mouse] = null;
                    else
                        Grid[mouse] = new Hex(mouse, 0);
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
                        LevelCompleted?.Invoke(levelCount);
                        NextLevel();
                        break;
                    default:
                        break;
                }
            }

            input.Update();
        }

        public void UpdateUI(Vector2 windowSize)
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


        private void NextLevel()
        {
            levelCount++;
            title.String = $"Level {levelCount:000}";
            LoadLevel();
        }

        private void PreviousLevel()
        {
            if (levelCount > 0)
                levelCount--;
            title.String = $"Level {levelCount:000}";
            LoadLevel();
        }

        private void LoadLevel()
        {
            level = Level.LoadFromFile($"{levelCount:000}");
            player = level.startPos;
        }

        private void SaveLevel()
        {
            level.startPos = player;
            level.SaveToFile($"{levelCount:000}");
        }

        private void GenerateLevel()
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
    }
}