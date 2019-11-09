using System.Collections.Generic;
using System.Drawing;
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

        Input input;
        UI.UI mainMenu;
        UI.UI worldMenu;

        bool world = false;

        private void InitUI()
        {
            // Main Menu
            {
                mainMenu = new UI.UI();

                float size = 5;
                Text title = new Text("KREATIVER NAME", size);
                title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)(6 * size)));
                title.Color = Color.White;
                mainMenu.Add(title);

                Button startButton = new Button();
                startButton.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(300), new PixelConstraint(60));
                startButton.OnClick += () => { world = true; };

                Text startText = new Text("Spiel starten", 3);
                startText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)startText.TextWidth), new PixelConstraint(21));
                startButton.AddChild(startText);

                mainMenu.Add(startButton);

                Button editorButton = new Button();
                editorButton.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(300), new PixelConstraint(60));
                editorButton.OnClick += NewEditor;

                Text editorText = new Text("Editor", 3);
                editorText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)editorText.TextWidth), new PixelConstraint(18));
                editorButton.AddChild(editorText);

                mainMenu.Add(editorButton);
            }
            // World Menu            
            {
                List<World> worlds = new List<World>();

                int index = 0;
                while (true)
                {
                    World world = World.LoadFromFile($"{index:000}");

                    if (world.levels == null)
                        break;

                    worlds.Add(world);
                    index++;
                }

                worldMenu = new UI.UI();

                Text title = new Text("Welten", 4);
                title.Color = Color.White;
                title.SetConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(50),
                    new PixelConstraint((int)title.TextWidth),
                    new PixelConstraint((int)title.TextHeight));
                worldMenu.Add(title);

                Frame frame = new Frame();
                frame.SetConstraints(
                    new CenterConstraint(),
                    new PixelConstraint(180),
                    new PixelConstraint(worlds.Count * 50 + 30),
                    new PixelConstraint(80));

                for (int i = 0; i < worlds.Count; i++)
                {
                    Button button = new Button(50 * i + 20, 20, 40, 40);

                    int world = i;
                    button.OnClick += () =>
                    {
                        NewGame(world);
                    };

                    Text text = new Text((i+1).ToString(), 2);
                    text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                    button.AddChild(text);
                    frame.AddChild(button);
                }
                    worldMenu.Add(frame);
            }
        }

        public override void Update()
        {
            if (world && MainWindow.input.KeyPress(OpenTK.Input.Key.Escape))
                world = false;
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            if (!world)
            {
                mainMenu.Update(windowSize);
                mainMenu.SetMouseState(MainWindow.input.MouseState());
            }
            else
            {
                worldMenu.Update(windowSize);
                worldMenu.SetMouseState(MainWindow.input.MouseState());
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
            game.input = MainWindow.input;
            game.Exit += () =>
            {
                MainWindow.scene = this;
                this.world = false;
            };

            MainWindow.scene = game;
        }

        private void NewEditor()
        {
            Editor editor = new Editor();
            editor.input = MainWindow.input;
            editor.Exit += () =>
            {
                MainWindow.scene = this;
                world = false;
            };

            MainWindow.scene = editor;
        }
    }
}
