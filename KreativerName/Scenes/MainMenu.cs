using System.Drawing;
using System.IO;
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
        UI.UI worldMenu;

        bool world = false;

        private void InitUI()
        {
            // Main Menu
            {
                mainMenu = new UI.UI();

                mainMenu.Input = new Input(Scenes.Window);

                float size = 5;
                TextBlock title = new TextBlock("KREATIVER NAME", size);
                title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
                title.Color = Color.White;
                mainMenu.Add(title);

                Button startButton = new Button();
                startButton.Color = Color.FromArgb(100, 255, 100);
                startButton.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(300), new PixelConstraint(60));
                startButton.OnClick += () => { world = true; };

                TextBlock startText = new TextBlock("Spiel starten", 3);
                startText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)startText.TextWidth), new PixelConstraint((int)startText.TextHeight));
                startButton.AddChild(startText);

                mainMenu.Add(startButton);

                Button editorButton = new Button();
                editorButton.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(300), new PixelConstraint(60));
                editorButton.OnClick += NewEditor;

                TextBlock editorText = new TextBlock("Editor", 3);
                editorText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)editorText.TextWidth), new PixelConstraint((int)editorText.TextHeight));
                editorButton.AddChild(editorText);

                mainMenu.Add(editorButton);

                Button exitButton = new Button();
                exitButton.Color = Color.FromArgb(255, 100, 100);
                exitButton.SetConstraints(new CenterConstraint(), new PixelConstraint(350), new PixelConstraint(300), new PixelConstraint(60));
                exitButton.OnClick += () => { Scenes.CloseWindow(); };

                TextBlock exitText = new TextBlock("Schliessen", 3);
                exitText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)exitText.TextWidth), new PixelConstraint((int)exitText.TextHeight));
                exitButton.AddChild(exitText);

                mainMenu.Add(exitButton);
            }
            // World Menu            
            {
                int worldcount = 0;
                for (; File.Exists($@"Resources\Worlds\{worldcount:000}.wld"); worldcount++)
                {

                }

                worldMenu = new UI.UI();
                worldMenu.Input = new Input(Scenes.Window);

                TextBlock title = new TextBlock("Welten", 4);
                title.Color = Color.White;
                title.SetConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(50),
                    new PixelConstraint((int)title.TextWidth),
                    new PixelConstraint((int)title.TextHeight));
                worldMenu.Add(title);

                Frame frame = new Frame();
                frame.Color = Color.Transparent;
                frame.SetConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(180),
                    new PixelConstraint(worldcount * 50 + 30),
                    new PixelConstraint(80));

                for (int i = 0; i < worldcount; i++)
                {
                    Button button = new Button(50 * i + 20, 20, 40, 40);

                    int world = i;
                    button.OnClick += () =>
                    {
                        NewGame(world);
                    };

                    TextBlock text = new TextBlock((i + 1).ToString(), 2);
                    text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                    button.AddChild(text);
                    frame.AddChild(button);
                }

                worldMenu.Add(frame);
            }
        }

        public override void Update()
        {
            if (world && Scenes.Input.KeyPress(OpenTK.Input.Key.Escape))
                world = false;
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            if (!world)
            {
                mainMenu.Update(windowSize);
            }
            else
            {
                worldMenu.Update(windowSize);
            }
        }

        public override void Render(Vector2 windowSize)
        {
            if (!world)
                mainMenu.Render(windowSize);
            else
                worldMenu.Render(windowSize);
        }

        private void NewGame(int world)
        {
            Game game = new Game(world);
            game.input = Scenes.Input;
            game.Exit += () =>
            {
                Scenes.LoadScene(new Transition(this, 10));
                this.world = false;
            };

            Scenes.LoadScene(new Transition(game, 10));
        }

        private void NewEditor()
        {
            Editor editor = new Editor();
            editor.input = Scenes.Input;
            editor.Exit += () =>
            {
                Scenes.LoadScene(new Transition(this, 10));
                world = false;
            };

            Scenes.LoadScene(new Transition(editor, 10));
        }
    }
}
