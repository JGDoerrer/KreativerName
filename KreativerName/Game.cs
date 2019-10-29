using System;
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
        }

        private void InitUI()
        {
            ui = new UI.UI();

            Frame frame = new Frame();
            frame.SetConstraints(new PixelConstraint(40),
                new PixelConstraint(40),
                new PixelConstraint(160),
                new PixelConstraint(160));

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

            ui.Add(frame);
        }

        Level level;
        internal UI.UI ui;
        internal Input input;
        internal HexPoint selectedHex;
        internal HexPoint player;

        const float size = 16*2;
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

            if (input.MousePress(MouseButton.Left))
            {
                if (GetPlayerMoves().Contains(mouse))
                    player = mouse;
            }
            
            if (Grid != null && Grid[player]?.Type.HasFlag(HexType.Deadly) == true)
            {
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

                while (Grid[(directions[i] * j) + player].HasValue && !Grid[(directions[i] * j) + player].Value.Type.HasFlag(HexType.Solid))
                {
                    moves.Add(directions[i] * j + player);
                    j++;
                }
            }

            return moves;
        }


        private void LoadLevel()
        {
            level = Level.LoadFromFile("000");
            player = level.startPos;
        }

        private void GenerateLevel()
        {
            Grid = new HexGrid<Hex>();
            
            int w = 11;
            int h = 10;

            HexType[] types =
            {
                0,
                HexType.Solid,
                HexType.Deadly,
            };

            for (int j = 0; j < w; j++)
            {
                for (int i = -(j / 2); i < h - (j + 1) / 2; i++)
                {
                    Grid[i, j] = new Hex(i, j, types[random.Next(0, types.Length)]);
                }
            }
            
            level.SaveToFile("000");
        }
    }
}
