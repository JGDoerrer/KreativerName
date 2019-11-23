using System.Drawing;
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
        }

        UI.UI mainMenu;

        private void InitUI()
        {
            mainMenu = new UI.UI();

            mainMenu.Input = new Input(Scenes.Window);

            float size = 5;
            TextBlock title = new TextBlock("KREATIVER NAME", size);
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            title.Color = Color.White;
            mainMenu.Add(title);

            {
                Button button = new Button();
                button.Color = Color.FromArgb(100, 255, 100);
                button.Shortcut = OpenTK.Input.Key.S;
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += () => { Scenes.LoadScene(new Transition(new WorldMenu(), 10)); };

                TextBlock startText = new TextBlock("Spiel starten", 3);
                startText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)startText.TextWidth), new PixelConstraint((int)startText.TextHeight));
                button.AddChild(startText);

                mainMenu.Add(button);
            }
            {
                Frame frame = new Frame();
                frame.Color = Color.Transparent;
                frame.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(300), new PixelConstraint(60));

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
                    button.OnClick += () => { Scenes.LoadScene(new Transition(new Settings(), 10)); };

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(20, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                {
                    Button button = new Button(160,0,60,60);
                    button.Shortcut = OpenTK.Input.Key.E;
                    button.OnClick += NewEditor;

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(30, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                {
                    Button button = new Button(240,0,60,60);
                    button.OnClick += () => { };

                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(40, 10, 10, 10));
                    image.Color = Color.Black;
                    image.SetConstraints(new UIConstaints(10, 10, 40, 40));

                    button.AddChild(image);
                    frame.AddChild(button);
                }
                mainMenu.Add(frame);
            }
            {
                Button button = new Button();
                button.Shortcut = OpenTK.Input.Key.Escape;
                button.Color = Color.FromArgb(255, 100, 100);
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(350), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += () => { Scenes.CloseWindow(); };

                TextBlock exitText = new TextBlock("Schliessen", 3);
                exitText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)exitText.TextWidth), new PixelConstraint((int)exitText.TextHeight));
                button.AddChild(exitText);

                mainMenu.Add(button);
            }
        }

        public override void Update()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            mainMenu.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
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
