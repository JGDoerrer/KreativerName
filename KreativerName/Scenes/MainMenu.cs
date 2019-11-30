using System;
using System.Drawing;
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
                backScroll = new Vector2(.5f, .5f);
            else
                backScroll = new Vector2(-.5f, .5f);
        }

        UI.UI mainMenu;

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
            mainMenu = new UI.UI();

            mainMenu.Input = new Input(Scenes.Window);

            float size = 5;
            TextBlock title = new TextBlock("KREATIVER NAME", size);
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            title.Color = Color.White;
            mainMenu.Add(title);

            Frame mainFrame = new Frame();
            mainFrame.Color = Color.Transparent;
            mainFrame.SetConstraints(new CenterConstraint(), new CenterConstraint(-50), new PixelConstraint(300), new PixelConstraint(200));

            {
                Button button = new Button();
                button.Color = Color.FromArgb(100, 255, 100);
                button.Shortcut = OpenTK.Input.Key.S;
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(0), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += () => { Scenes.LoadScene(new Transition(new WorldMenu(), 10)); };

                TextBlock startText = new TextBlock("Spiel starten", 3);
                startText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)startText.TextWidth), new PixelConstraint((int)startText.TextHeight));
                button.AddChild(startText);

                mainFrame.AddChild(button);
            }
            {
                Frame frame = new Frame();
                frame.Color = Color.Transparent;
                frame.SetConstraints(new CenterConstraint(), new PixelConstraint(100), new PixelConstraint(300), new PixelConstraint(60));

                {
                    Button button = new Button(0, 0, 60, 60);
                    button.OnClick += () => { Scenes.LoadScene(new Transition(new Statistics(), 10)); };

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(10, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                {
                    Button button = new Button(80, 0, 60, 60);
                    button.OnClick += () => { Scenes.LoadScene(new Transition(new SettingsScene(), 10)); };

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(20, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                {
                    Button button = new Button(160, 0, 60, 60);
                    button.Shortcut = OpenTK.Input.Key.E;
                    button.OnClick += NewEditor;

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(30, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                {
                    Button button = new Button(240, 0, 60, 60);
                    button.OnClick += () => { };

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(40, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                mainFrame.AddChild(frame);
            }
            {
                Button button = new Button();
                button.Shortcut = OpenTK.Input.Key.Escape;
                button.Color = Color.FromArgb(255, 100, 100);
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(200), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += () => { Scenes.CloseWindow(); };

                TextBlock exitText = new TextBlock("Schliessen", 3);
                exitText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)exitText.TextWidth), new PixelConstraint((int)exitText.TextHeight));
                button.AddChild(exitText);

                mainFrame.AddChild(button);
            }

            mainMenu.Add(mainFrame);
        }

        public override void Update()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            mainMenu.Update(windowSize);

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
            float w = windowSize.X / (layout.size * sqrt3) + 8;
            float h = windowSize.Y / (layout.size * 1.5f) + 8;

            for (int y = -4; y < h - 4; y++)
            {
                for (int x = -(y / 2) - 4; x < w - (y + 1) / 2; x++)
                {
                    TextureRenderer.DrawHex(Textures.Get("Hex"), new HexPoint(x, y), layout, Vector2.One, Color.FromArgb(20, 20, 20), new RectangleF(0, 0, 32, 32));
                }
            }

            mainMenu.Render(windowSize);
        }

        private void NewEditor()
        {
            Editor editor = new Editor
            {
                input = new Input(Scenes.Window)
            };
            editor.Exit += () =>
            {
                Scenes.LoadScene(new Transition(this, 10));
            };

            Scenes.LoadScene(new Transition(editor, 10));
        }
    }
}
