using System;
using System.Linq;
using System.Collections.Generic;
using KreativerName.Grid;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName
{
    public class Game
    {
        public Game()
        {
            InitUI();
            LoadLevel();
        }

        private void InitUI()
        {
            ui = new UI.UI();

            Frame frame = new Frame();
            frame.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(20),
                new PixelConstraint(160),
                new PixelConstraint(200));

            Button button1 = new Button();
            button1.OnClicked += LoadLevel;
            button1.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(20),
                new PixelConstraint(80),
                new PixelConstraint(40));

            Text text1 = new Text("Load", 2);
            text1.SetConstraints(new PixelConstraint(10),
                new PixelConstraint(10),
                new PixelConstraint(80),
                new RelativeConstraint(1));

            button1.AddChild(text1);
            frame.AddChild(button1);

            Button button2 = new Button();
            button2.OnClicked += GenerateLevel;
            button2.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(80),
                new PixelConstraint(120),
                new PixelConstraint(40));

            Text text2 = new Text("Generate", 2);
            text2.SetConstraints(new PixelConstraint(10),
                new PixelConstraint(10),
                new PixelConstraint(80),
                new RelativeConstraint(1));

            button2.AddChild(text2);
            frame.AddChild(button2);

            Button button3 = new Button();
            button3.OnClicked += SaveLevel;
            button3.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(140),
                new PixelConstraint(80),
                new PixelConstraint(40));

            Text text3 = new Text("Save", 2);
            text3.SetConstraints(new PixelConstraint(10),
                new PixelConstraint(10),
                new PixelConstraint(80),
                new RelativeConstraint(1));

            button3.AddChild(text3);
            frame.AddChild(button3);

            ui.Add(frame);
        }

        int levelCount = 0;
        Level level;
        internal UI.UI ui;
        internal Input input;
        internal HexPoint selectedHex;
        internal HexPoint player;

        const float size = 16 * 2;
        internal HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);

        static Random random = new Random();

        public HexGrid<Hex> Grid { get => level.grid; set => level.grid = value; }

        public void Update()
        {
            HexPoint mouse = layout.PixelToHex(input.MousePosition());
            selectedHex = mouse;
            Console.WriteLine(mouse);

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

            if (Grid != null && Grid[player]?.Type == HexType.Deadly)
            {
                LoadLevel();
            }
            if (Grid != null && Grid[player]?.Type == HexType.Goal)
            {
                levelCount++;
                LoadLevel();
            }

            input.Update();
        }

        public void UpdateUI(Vector2 windowSize)
        {
            ui.SetMouseState(input.MouseState());
            ui.Update(windowSize);
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
