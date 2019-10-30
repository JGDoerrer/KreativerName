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
    public delegate void LevelEvent(int level);
    public delegate void EmptyEvent();

    public class Game
    {
        public Game()
        {
            LoadLevel();
        }

        int levelCount = 0;
        Level level;

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
            LoadLevel();
        }

        private void PreviousLevel()
        {
            if (levelCount > 0)
                levelCount--;
            LoadLevel();
        }

        private void LoadLevel()
        {
            level = Level.LoadFromFile($"{levelCount:000}");
            player = level.startPos;
        }
    }
}
