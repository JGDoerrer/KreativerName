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
            Grid = new HexGrid<Hex>();

            int w = 11;
            int h = 10;
            Random random = new Random();

            for (int j = 0; j < w; j++)
            {
                for (int i = -(j / 2); i < h - (j + 1) / 2; i++)
                {
                    Grid[i, j] = new Hex(i, j, (HexType)(random.Next(0, 2)));
                }
            }

            level.SaveToFile("000");

            InitUI();
        }

        private void InitUI()
        {
            ui = new UI.UI();
            Frame frame = new Frame();
            frame.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(80),
                new PixelConstraint(80),
                new RelativeConstraint(1));
            Button button = new Button();
            button.OnClicked += LoadLevel;
            button.SetConstraints(new PixelConstraint(20),
                new PixelConstraint(20),
                new PixelConstraint(40),
                new RelativeConstraint(1));
            frame.AddChild(button);
            ui.Add(frame);
        }

        Level level;
        internal UI.UI ui;
        internal Input input;
        internal HexPoint selectedHex;
        internal HexPoint player;

        const float size = 32;
        internal HexLayout layout = new HexLayout(
            new Matrix2((float)Math.Sqrt(3), (float)Math.Sqrt(3) / 2f, 0, 3f / 2f),
            new Matrix2((float)Math.Sqrt(3) / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);

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

            List<HexPoint> moves = new List<HexPoint>();

            for (int i = 0; i < 6; i++)
            {
                int j = 1;

                while (Grid[(directions[i] * j) + player].HasValue && Grid[(directions[i] * j) + player].Value.Type == HexType.Empty)
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
    }
}
