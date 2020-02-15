using System;
using System.Drawing;
using System.IO;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class MainMenu : Scene
    {
        public MainMenu()
        {
            InitUI();

            if (random.NextDouble() < 0.001f)
                backScroll = new Vector2(.2f, .2f);
            else
                backScroll = new Vector2(-.2f, .2f);
        }

        UI.UI ui;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;
        const float size = 16 * 2;
        static Random random = new Random();

        HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            size, 0.5f);

        Vector2 backScroll;


        private void InitUI()
        {
            ui = new UI.UI
            {
                Input = SceneManager.Input
            };

            TextBlock title = new TextBlock("KREATIVER NAME", 5, 0, 50);
            title.Constraints.x = new CenterConstraint();
            title.Color = Color.White;
            ui.Add(title);

            TextBlock splash = new TextBlock(File.ReadAllLines(@"Resources\Splash.txt").Random(), 2, 0, 90);
            splash.Constraints.x = new CenterConstraint();
            splash.Color = Color.White;
            ui.Add(splash);

            TextBlock version = new TextBlock($"Version {MainWindow.Version}", 2, 5, 0);
            version.Constraints.y = new PixelConstraint(5, RelativeTo.Window, Direction.Bottom);
            version.Color = Color.White;
            ui.Add(version);

            Frame mainFrame = new Frame
            {
                Constraints = new UIConstraints(new CenterConstraint(), new CenterConstraint(-50), new PixelConstraint(300), new PixelConstraint(200)),
                Color = Color.Transparent
            };

            // Add start button
            Button startButton = new Button
            {
                Color = Color.FromArgb(100, 255, 100),
                Shortcut = OpenTK.Input.Key.S,
                Constraints = new UIConstraints(new CenterConstraint(), new PixelConstraint(0), new PixelConstraint(300), new PixelConstraint(60))
            };
            startButton.OnLeftClick += (sender) => { SceneManager.LoadScene(new Transition(new WorldMenu(), 10)); };

            TextBlock startText = new TextBlock("Spiel starten", 3);
            startText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)startText.TextWidth), new PixelConstraint((int)startText.TextHeight));
            startButton.AddChild(startText);

            mainFrame.AddChild(startButton);

            // Add middle frame
            Frame frame = new Frame
            {
                Color = Color.Transparent,
                Constraints = new UIConstraints(new CenterConstraint(), new PixelConstraint(100), new PixelConstraint(300), new PixelConstraint(60))
            };

            void AddButton(int x, int icon, ClickEvent click)
            {
                Button button = new Button(x, 0, 60, 60);
                button.OnLeftClick += click;

                UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(icon * 10, 10, 10, 10))
                {
                    Color = Color.Black,
                    Constraints = new UIConstraints(10, 10, 40, 40)
                };

                button.AddChild(image);
                frame.AddChild(button);
            }

            AddButton(0, 1, (sender) => { SceneManager.LoadScene(new Transition(new StatisticsScene(), 10)); });
            AddButton(80, 2, (sender) => { SceneManager.LoadScene(new Transition(new SettingsScene(), 10)); });
            AddButton(160, 3, NewEditor);
            AddButton(240, 4, (sender) => { SceneManager.LoadScene(new Transition(new OnlineScene(), 10)); });

            mainFrame.AddChild(frame);

            // Add exit button
            Button exitButton = new Button
            {
                Color = Color.FromArgb(255, 100, 100),
                Constraints = new UIConstraints(new CenterConstraint(), new PixelConstraint(200), new PixelConstraint(300), new PixelConstraint(60))
            };
            exitButton.OnLeftClick += (sender) => { SceneManager.CloseWindow(); };

            TextBlock exitText = new TextBlock("Schliessen", 3);
            exitText.Constraints = new UIConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)exitText.TextWidth), new PixelConstraint((int)exitText.TextHeight));
            exitButton.AddChild(exitText);

            mainFrame.AddChild(exitButton);

            ui.Add(mainFrame);
        }

        public override void Update()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);

            layout.origin += backScroll;

            if (layout.origin.X <= -layout.size * sqrt3)
                layout.origin.X += layout.size * sqrt3;
            if (layout.origin.X >= layout.size * sqrt3)
                layout.origin.X -= layout.size * sqrt3;
            if (layout.origin.Y >= layout.size * 3f)
                layout.origin.Y -= layout.size * 3f;
            if (layout.origin.Y <= -layout.size * 3f)
                layout.origin.Y += layout.size * 3f;
        }

        public override void Render(Vector2 windowSize)
        {
            // Render background
            float w = windowSize.X / (layout.size * sqrt3) + 8;
            float h = windowSize.Y / (layout.size * 1.5f) + 8;

            Texture2D tex = Textures.Get("Hex\\000");

            for (int y = -4; y < h - 4; y++)
            {
                for (int x = -(y / 2) - 4; x < w - (y + 1) / 2; x++)
                {
                    TextureRenderer.DrawHex(tex, new HexPoint(x, y), layout, Vector2.One, Color.FromArgb(10, 10, 10), new RectangleF(0, 0, 32, 32));
                }
            }

            ui.Render(windowSize);
        }

        private void NewEditor(UIElement sender)
        {
            Editor editor = new Editor();
            editor.OnExit += () =>
            {
                SceneManager.LoadScene(new Transition(this, 10));
                editor.Dispose();
            };

            SceneManager.LoadScene(new Transition(editor, 10));
        }

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ui.Dispose();
                }

                disposedValue = true;
            }
        }

        ~MainMenu()
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
