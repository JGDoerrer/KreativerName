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
        public Tetris()
        {
            NextPiece();
        }

        const int Width = 10;
        const int Height = 20;
        const float Scale = 3;

        static Random random = new Random();

        readonly byte[,,] pieces =
        {
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,1,
                    0,0,0,0,
                },
                {
                    0,0,1,0,
                    0,0,1,0,
                    0,0,1,0,
                    0,0,1,0,
                },
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,1,
                    0,0,0,0,
                },
                {
                    0,0,1,0,
                    0,0,1,0,
                    0,0,1,0,
                    0,0,1,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,1,1,0,
                    0,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,1,0,
                    0,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,1,0,
                    0,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,1,0,
                    0,1,1,0,
                    0,0,0,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,0,
                    0,0,1,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    0,1,0,0,
                    1,1,0,0,
                },
                {
                    0,0,0,0,
                    1,0,0,0,
                    1,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,1,0,
                    0,1,0,0,
                    0,1,0,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,0,
                    1,0,0,0,
                },
                {
                    0,0,0,0,
                    1,1,0,0,
                    0,1,0,0,
                    0,1,0,0,
                },
                {
                    0,0,0,0,
                    0,0,1,0,
                    1,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    0,1,0,0,
                    0,1,1,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    0,1,1,0,
                    1,1,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    0,1,1,0,
                    0,0,1,0,
                },
                {
                    0,0,0,0,
                    0,0,0,0,
                    0,1,1,0,
                    1,1,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    0,1,1,0,
                    0,0,1,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,0,0,
                    0,1,1,0,
                },
                {
                    0,0,0,0,
                    0,0,1,0,
                    0,1,1,0,
                    0,1,0,0,
                },
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,0,0,
                    0,1,1,0,
                },
                {
                    0,0,0,0,
                    0,0,1,0,
                    0,1,1,0,
                    0,1,0,0,
                },
            },
            {
                {
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,0,
                    0,1,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    1,1,0,0,
                    0,1,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    1,1,1,0,
                    0,0,0,0,
                },
                {
                    0,0,0,0,
                    0,1,0,0,
                    0,1,1,0,
                    0,1,0,0,
                },
            }
        };

        byte[,] field = new byte[Width, Height];

        int currentX, currentY;
        int currentRot;
        int currentPiece = 1;

        int nextPiece;

        int frameCount = 0;
        int speed = 48;

        int level = 0;
        int score = 0;
        int linesCleared = 0;

        public override void Update()
        {
            currentX += Scenes.Input.KeyPress(Key.Right) && Fits(currentPiece, currentRot, currentX + 1, currentY) ? 1 : 0;
            currentX -= Scenes.Input.KeyPress(Key.Left) && Fits(currentPiece, currentRot, currentX - 1, currentY) ? 1 : 0;
            currentY += Scenes.Input.KeyDown(Key.Down) && Fits(currentPiece, currentRot, currentX, currentY + 1) ? 1 : 0;

            currentRot += Scenes.Input.KeyPress(Key.X) && Fits(currentPiece, currentRot + 1, currentX, currentY) ? 1 : 0;
            currentRot += Scenes.Input.KeyPress(Key.Z) && Fits(currentPiece, currentRot + 3, currentX, currentY) ? 3 : 0;

            if (frameCount % speed == 0)
            {
                if (Fits(currentPiece, currentRot, currentX, currentY + 1))
                {
                    currentY++;
                }
                else
                {
                    // set piece
                    for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                        {
                            if (pieces[currentPiece, currentRot % 4, py * 4 + px] > 0)
                                field[currentX + px, currentY + py] = (byte)(currentPiece + 1);
                        }

                    int lines = ClearLines();

                    if (lines == 4)
                        score += 1200 * (level + 1);
                    else if (lines == 3)
                        score += 300 * (level + 1);
                    else if (lines == 2)
                        score += 100 * (level + 1);
                    else if (lines == 1)
                        score += 40 * (level + 1);

                    linesCleared += lines;

                    if (lines > 0 && linesCleared % 10 == 0)
                        level++;

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

                    NextPiece();

                    if (!Fits(currentPiece, currentRot, currentX, currentY))
                        Scenes.LoadScene(new Transition(new MainMenu(), 10));
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
                        Vector2 position = new Vector2(x, y) * 8 * Scale;
                        position += windowSize / 2;
                        position -= new Vector2(Width, Height) * 8 * Scale / 2;

                        TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, Color.White, new RectangleF((field[x, y] - 1) * 8, 0, 8, 8));
                    }

            // current piece
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    if (pieces[currentPiece, currentRot % 4, y * 4 + x] > 0)
                    {
                        Vector2 position = new Vector2(currentX + x, currentY + y) * 8 * Scale;
                        position += windowSize / 2;
                        position -= new Vector2(Width, Height) * 8 * Scale / 2;

                        TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, Color.White, new RectangleF(currentPiece * 8, 0, 8, 8));
                    }

            // next piece
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    if (pieces[nextPiece, 0, y * 4 + x] > 0)
                    {
                        Vector2 position = new Vector2(x, y) * 8 * Scale + new Vector2(50, 50);

                        TextureRenderer.Draw(Textures.Get("Tetris"), position, Vector2.One * Scale, Color.White, new RectangleF(nextPiece * 8, 0, 8, 8));
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

        private void NextPiece()
        {
            currentX = Width / 2 - 2;
            currentY = 0;
            currentRot = 0;
            currentPiece = nextPiece;

            nextPiece = random.Next(0, 7);
        }

        private bool Fits(int piece, int rotation, int x, int y)
        {
            for (int px = 0; px < 4; px++)
                for (int py = 0; py < 4; py++)
                {
                    if ((x + px < 0 || x + px >= Width ||
                        y + py < 0 || y + py >= Height ||
                        field[x + px, y + py] > 0) && pieces[piece, rotation % 4, py * 4 + px] > 0)
                    {
                        return false;
                    }
                }

            return true;
        }

        private int ClearLines()
        {
            int lines = 0;

            for (int py = 0; py < 4; py++)
            {
                if (currentY + py < Height)
                {
                    bool line = true;
                    for (int px = 0; px < Width; px++)
                        if (field[px, currentY + py] == 0)
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
                                if (y < Height)
                                    field[x, y] = field[x, y - 1];
                                else
                                    field[x, y] = 0;
                            }
                    }
                }
            }

            return lines;
        }

    }
}
