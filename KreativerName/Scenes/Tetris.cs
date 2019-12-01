using System;
using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class Tetris : Scene
    {
        public Tetris(int level = 0)
        {
            NextPiece();
            NextPiece();
            SetLevel(level);

            nextLevel = Math.Min((level + 1) * 10, Math.Max(100, level * 10 - 50));
        }

        const int Width = 10;
        const int Height = 20;
        const float Scale = 3;

        static Random random = new Random();

        readonly int[,] pieces =
        {
            { // T
                
                0b0000000011100100,
                0b0000010011000100,
                0b0000010011100000,
                0b0000010001100100,
            },
            { // J         
                0b0000000011100010,
                0b0000010001001100,
                0b0000100011100000,
                0b0000011001000100,
            },
            { // Z
                
                0b0000000011000110,
                0b0000001001100100,
                0b0000000011000110,
                0b0000001001100100,
            },
            { // O        
                0b0000011001100000,
                0b0000011001100000,
                0b0000011001100000,
                0b0000011001100000,
            },
            { // S
                
                0b0000000001101100,
                0b0000010001100010,
                0b0000000001101100,
                0b0000010001100010,
            },
            { // L               
                0b0000000011101000,
                0b0000110001000100,
                0b0000001011100000,
                0b0000010001000110,
            },
            { // I
                0b0000111100000000,
                0b0010001000100010,
                0b0000111100000000,
                0b0010001000100010,
            },
        };

        byte[,] field = new byte[Width, Height];

        bool gameOver = false;

        Color c1 = Color.Red;
        Color c2 = Color.Blue;

        int currentX, currentY;
        int currentRot;
        int currentPiece;

        int nextPiece;
        int nextLevel;

        int frameCount = 0;
        int speed = 48;
        int nextPieceIn = 0;

        int level = 0;
        int score = 0;
        int linesCleared = 0;

        int left = 0;
        int right = 0;
        int down = 0;

        public override void Update()
        {
            if (gameOver)
            {
                if (Scenes.Input.KeyPress(Key.Escape))
                    Scenes.LoadScene(new Transition(new MainMenu(), 10));
                return;
            }

            if (nextPieceIn > 0)
            {
                if (nextPieceIn == 1)
                {
                    NextPiece();

                    if (!Fits(currentPiece, currentRot, currentX, currentY))
                    {
                        gameOver = true;

                        if (Stats.Current.TetrisHighLevel < level)
                            Stats.Current.TetrisHighLevel = level;
                        if (Stats.Current.TetrisHighScore < score)
                            Stats.Current.TetrisHighScore = score;
                        if (Stats.Current.TetrisMostLines < linesCleared)
                            Stats.Current.TetrisMostLines = linesCleared;
                    }
                }

                nextPieceIn--;
                return;
            }

            HandleInput();

            if (frameCount % speed == 0)
            {
                if (Fits(currentPiece, currentRot, currentX, currentY + 1))
                {
                    currentY++;
                }
                else
                {
                    SetPiece();

                    ClearLines();

                    if (linesCleared >= nextLevel)
                    {
                        level++;
                        nextLevel = (level + 1) * 10;
                        SetLevel(level);
                    }
                }
            }

            frameCount++;
        }

        public override void UpdateUI(Vector2 windowSize)
        {
        }

        public override void Render(Vector2 windowSize)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (field[x, y] > 0)
                    {
                        RenderTile(windowSize, field[x, y] - 1, x, y);
                    }

            // current piece
            if (nextPieceIn == 0)
                RenderPiece(windowSize, currentPiece, currentRot, currentX, currentY);

            // next piece
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    if ((pieces[nextPiece, 0] & (1 << (y * 4 + x))) > 0)
                    {
                        Vector2 position = new Vector2(x, y) * 8 * Scale + new Vector2(50, 50);

                        TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, Color.White, new RectangleF((nextPiece % 3 == 0 ? 0 : 1) * 8, 0, 8, 8));
                        TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, nextPiece % 3 != 2 ? c1 : c2, new RectangleF((nextPiece % 3 == 0 ? 0 : 1) * 8, 8, 8, 8));
                    }

            {
                TextBlock textBlock = new TextBlock($"Punkte: {score}", 2, 200, 50)
                {
                    Color = Color.White
                };
                textBlock.Render(windowSize);
                textBlock.Dispose();
            }
            {
                TextBlock textBlock = new TextBlock($"Level: {level}", 2, 200, 70)
                {
                    Color = Color.White
                };
                textBlock.Render(windowSize);
                textBlock.Dispose();
            }
            {
                TextBlock textBlock = new TextBlock($"Linien: {linesCleared}", 2, 200, 90)
                {
                    Color = Color.White
                };
                textBlock.Render(windowSize);
                textBlock.Dispose();
            }
        }

        private void RenderPiece(Vector2 winSize, int piece, int rot, int x, int y)
        {
            for (int px = 0; px < 4; px++)
                for (int py = 0; py < 4; py++)
                    if ((pieces[piece, rot % 4] & (1 << (py * 4 + px))) > 0)
                    {
                        RenderTile(winSize, piece % 3, x + px, y + py);
                    }
        }

        private void RenderTile(Vector2 winSize, int tile, int x, int y)
        {
            Vector2 position = new Vector2(x, y) * 8 * Scale;
            position += winSize / 2;
            position -= new Vector2(Width, Height) * 8 * Scale / 2;

            TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, Color.White, new RectangleF((tile % 3 == 0 ? 0 : 1) * 8, 0, 8, 8));
            TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, tile % 3 != 2 ? c1 : c2, new RectangleF((tile % 3 == 0 ? 0 : 1) * 8, 8, 8, 8));
        }

        public void SetLevel(int level)
        {
            switch (level)
            {
                case 0: speed = 48; break;
                case 1: speed = 43; break;
                case 2: speed = 38; break;
                case 3: speed = 33; break;
                case 4: speed = 28; break;
                case 5: speed = 23; break;
                case 6: speed = 18; break;
                case 7: speed = 13; break;
                case 8: speed = 8; break;
                case 9: speed = 6; break;
                case 10: speed = 5; break;
                case 11: speed = 5; break;
                case 12: speed = 5; break;
                case 13: speed = 4; break;
                case 14: speed = 4; break;
                case 15: speed = 4; break;
                case 16: speed = 3; break;
                case 17: speed = 3; break;
                case 18: speed = 3; break;
                default:
                    if (level >= 19 && level <= 28)
                        speed = 2;
                    else
                        speed = 1;
                    break;
            }

            // Todo: optimize
            switch (level % 10)
            {
                case 0:
                    c1 = Color.FromArgb(0x00, 0x58, 0xF8);
                    c2 = Color.FromArgb(0x3C, 0xBC, 0xFC);
                    break;
                case 1:
                    c1 = Color.FromArgb(0x00, 0xA8, 0x00);
                    c2 = Color.FromArgb(0xB8, 0xF8, 0x18);
                    break;
                case 2:
                    c1 = Color.FromArgb(0xD8, 0x00, 0xCC);
                    c2 = Color.FromArgb(0xF8, 0x78, 0xF8);
                    break;
                case 3:
                    c1 = Color.FromArgb(0x00, 0x58, 0xF8);
                    c2 = Color.FromArgb(0x58, 0xD8, 0x54);
                    break;
                case 4:
                    c1 = Color.FromArgb(0xE4, 0x00, 0x58);
                    c2 = Color.FromArgb(0x58, 0xF8, 0x98);
                    break;
                case 5:
                    c2 = Color.FromArgb(0x58, 0xF8, 0x98);
                    c2 = Color.FromArgb(0x68, 0x88, 0xFC);
                    break;
                case 6:
                    c2 = Color.FromArgb(0xF8, 0x38, 0x00);
                    c2 = Color.FromArgb(0x7C, 0x7C, 0x7C);
                    break;
                case 7:
                    c2 = Color.FromArgb(0x68, 0x44, 0xFC);
                    c2 = Color.FromArgb(0xA8, 0x00, 0x20);
                    break;
                case 8:
                    c1 = Color.FromArgb(0x00, 0x58, 0xF8);
                    c2 = Color.FromArgb(0xF8, 0x38, 0x00);
                    break;
                case 9:
                    c1 = Color.FromArgb(0xF8, 0x38, 0x00);
                    c2 = Color.FromArgb(0xFC, 0xA0, 0x44);
                    break;
            }

            this.level = level;
        }

        private void HandleInput()
        {
            if (Scenes.Input.KeyDown(Key.Down))
            {
                if (down == 0)
                {
                    if (Fits(currentPiece, currentRot, currentX, currentY + 1))
                        currentY += 1;
                    else
                    {
                        SetPiece();

                        ClearLines();

                        if (linesCleared >= nextLevel)
                        {
                            level++;
                            nextLevel = (level + 1) * 10;
                            SetLevel(level);
                        }
                    }

                    down = 2;
                }
                down--;
            }

            if (Scenes.Input.KeyPress(Key.Right))
            {
                if (Fits(currentPiece, currentRot, currentX + 1, currentY))
                    currentX += 1;
                right = 16;
            }
            if (Scenes.Input.KeyDown(Key.Right))
            {
                if (right == 0)
                {
                    if (Fits(currentPiece, currentRot, currentX + 1, currentY))
                        currentX += 1;
                    right = 2;
                }
                right--;
            }

            if (Scenes.Input.KeyPress(Key.Left))
            {
                if (Fits(currentPiece, currentRot, currentX - 1, currentY))
                    currentX -= 1;
                left = 16;
            }
            if (Scenes.Input.KeyDown(Key.Left))
            {
                if (left == 0)
                {
                    if (Fits(currentPiece, currentRot, currentX - 1, currentY))
                        currentX -= 1;
                    left = 2;
                }
                left--;
            }

            currentRot += Scenes.Input.KeyPress(Key.X) && Fits(currentPiece, currentRot + 1, currentX, currentY) ? 1 : 0;
            currentRot += Scenes.Input.KeyPress(Key.Z) && Fits(currentPiece, currentRot + 3, currentX, currentY) ? 3 : 0;
        }

        private void NextPiece()
        {
            currentX = Width / 2 - 2;
            currentY = 0;
            currentRot = 0;
            currentPiece = nextPiece;

            for (int y = 0; y < 4; y++)
            {
                bool empty = true;

                for (int x = 0; x < 4; x++)
                {
                    if ((pieces[currentPiece, currentRot] & (1 << (y * 4 + x))) > 0)
                    {
                        empty = false;
                        break;
                    }
                }

                if (empty)
                    currentY--;
                else
                    break;
            }

            nextPiece = random.Next(0, 7);
        }

        private bool Fits(int piece, int rotation, int x, int y)
        {
            for (int px = 0; px < 4; px++)
                for (int py = 0; py < 4; py++)
                {
                    if (y + py < 0)
                        continue;

                    if ((x + px < 0 || x + px >= Width ||
                        /*y + py < 0 ||*/ y + py >= Height ||
                        field[x + px, y + py] > 0) && (pieces[piece, rotation % 4] & (1 << (py * 4 + px))) > 0)
                    {
                        return false;
                    }
                }

            return true;
        }

        private void ClearLines()
        {
            int lines = 0;

            for (int py = 0; py < 4; py++)
            {
                if (currentY + py < Height)
                {
                    bool line = true;
                    for (int px = 0; px < Width; px++)
                        if (currentY + py < 0 || field[px, currentY + py] == 0)
                        {
                            line = false;
                            break;
                        }

                    if (line)
                    {
                        lines++;

                        // clear line
                        for (int px = 0; px < Width; px++)
                            field[px, currentY + py] = 0;

                        // copy lines down
                        for (int y = currentY + py; y > 0; y--)
                            for (int x = 0; x < Width; x++)
                            {
                                if (y > 0)
                                    field[x, y] = field[x, y - 1];
                                else
                                    field[x, y] = 0;
                            }
                    }
                }
            }

            if (lines == 4)
                score += 1200 * (level + 1);
            else if (lines == 3)
                score += 300 * (level + 1);
            else if (lines == 2)
                score += 100 * (level + 1);
            else if (lines == 1)
                score += 40 * (level + 1);

            linesCleared += lines;
        }

        private void SetPiece()
        {
            for (int px = 0; px < 4; px++)
                for (int py = 0; py < 4; py++)
                {
                    if ((pieces[currentPiece, currentRot % 4] & (1 << (py * 4 + px))) > 0)
                        field[currentX + px, currentY + py] = (byte)(currentPiece % 3 + 1);
                }

            nextPieceIn = 15;
        }

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // verwalteten Zustand (verwaltete Objekte) entsorgen.
                }

                // nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        ~Tetris()
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
