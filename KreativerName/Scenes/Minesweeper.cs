using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    class Minesweeper : Scene
    {
        public Minesweeper(int w, int h, int mines)
        {
            width = w;
            height = h;
            this.mines = mines;

            tiles = new Tile[width, height];
            InitUI();
        }

        static Random random = new Random();
        readonly int width, height, mines;

        Tile[,] tiles;
        UI.UI ui;
        bool firstClick = true;
        bool gameOver = false;
        bool allowAction = true;

        public override void Update()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);
        }

        private void InitTiles(int firstX, int firstY)
        {
            List<int> mines = new List<int>();

            // Choose mines
            while (mines.Count < this.mines)
            {
                int index = random.Next(0, width * height);

                if (!mines.Contains(index) && index != firstX * height + firstY)
                    mines.Add(index);
            }

            // Set tiles
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].X = x;
                    tiles[x, y].Y = y;
                    tiles[x, y].IsMine = mines.Contains(x * height + y);
                    tiles[x, y].Revealed = false;
                    tiles[x, y].Marked = false;
                }

            // Set Neighbours
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    for (int dx = -1; dx < 2; dx++)
                        for (int dy = -1; dy < 2; dy++)
                        {
                            if (x + dx >= 0 && x + dx < width &&
                                y + dy >= 0 && y + dy < height &&
                                tiles[x + dx, y + dy].IsMine)
                                tiles[x, y].Neighbours++;
                        }
        }

        private void InitUI()
        {
            const int ButtonSize = 28;

            ui = new UI.UI();
            ui.Input = SceneManager.Input;

            Button exitButton = new Button(20, 20, 40, 40)
            {
                Shortcut = Key.Escape
            };
            exitButton.OnLeftClick += () => SceneManager.LoadScene(new Transition(new MainMenu(), 10));

            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstraints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            Frame buttonFrame = new Frame
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint(ButtonSize * width), new PixelConstraint(ButtonSize * height))
            };

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Button button = new Button(x * ButtonSize, y * ButtonSize, ButtonSize, ButtonSize)
                    {
                        Style = 1
                    };

                    int copyX = x;
                    int copyY = y;

                    button.OnLeftClick += () =>
                    {
                        if (firstClick)
                        {
                            InitTiles(copyX, copyY);
                            firstClick = false;
                        }

                        RevealTile(copyX, copyY);
                    };
                    button.OnRightClick += () => MarkTile(copyX, copyY);

                    tiles[x, y].Button = button;

                    buttonFrame.AddChild(button);
                }

            ui.Add(buttonFrame);
        }

        private void RevealTile(int x, int y, bool bypass = false)
        {
            if (tiles[x, y].Revealed)
                return;

            if (!bypass)
            {
                if (tiles[x, y].Marked)
                    return;
                if (!allowAction)
                    return;
            }

            tiles[x, y].Revealed = true;
            tiles[x, y].Button.ChangeState = false;
            tiles[x, y].Button.State = 2;

            if (tiles[x, y].IsMine)
                tiles[x, y].Button.Color = Color.Red;

            if (tiles[x, y].Neighbours > 0 && !tiles[x, y].IsMine)
            {
                // Show Neighbours
                tiles[x, y].Button.AddChild(new TextBlock(tiles[x, y].Neighbours.ToString(), 3, 6, 4));
            }
            else if (tiles[x, y].Neighbours == 0 && !tiles[x, y].IsMine)
            {
                // Reveal neighbours
                for (int dx = -1; dx < 2; dx++)
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (x + dx >= 0 && x + dx < width &&
                            y + dy >= 0 && y + dy < height)
                            RevealTile(x + dx, y + dy);
                    }
            }

            if (IsWon())
            {
                Notification.Show("Gewonnen!");
                Stats.Current.MinesweeperWon++;
                allowAction = false;
            }
            if (IsLost())
            {
                if (!gameOver)
                {
                    Notification.Show("Verloren!");
                    Stats.Current.MinesweeperLost++;
                }

                gameOver = true;
                allowAction = false;

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        if (tiles[i, j].IsMine)
                            RevealTile(i, j, true);

            }
        }

        private void MarkTile(int x, int y)
        {
            if (tiles[x, y].Revealed)
                return;
            if (!allowAction)
                return;

            tiles[x, y].Marked = !tiles[x, y].Marked;

            if (tiles[x, y].Marked)
            {
                UI.Image icon = new UI.Image(Textures.Get("Icons"), new RectangleF(20, 0, 10, 10));
                icon.Constraints = new UIConstraints(4, 4, 20, 20);
                tiles[x, y].Button.AddChild(icon);
            }
            else
                tiles[x, y].Button.ClearChildren();
        }

        private bool IsWon()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y].IsMine && tiles[x, y].Revealed)
                        return false;
                    if (!tiles[x, y].IsMine && !tiles[x, y].Revealed)
                        return false;
                }

            return true;
        }

        private bool IsLost()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (tiles[x, y].IsMine && tiles[x, y].Revealed)
                        return true;

            return false;
        }

        struct Tile
        {
            public int X, Y;
            public int Neighbours;
            public bool IsMine;
            public bool Revealed;
            public bool Marked;
            public Button Button;

            public override string ToString() => $"({X}, {Y}), Mine: {IsMine}, Revealed: {Revealed}, Neighbours: {Neighbours}";
        }

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
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the scene.
        /// </summary>
        ~Minesweeper()
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
