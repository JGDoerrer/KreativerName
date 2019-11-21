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
                Button button = new Button();
                button.Shortcut = OpenTK.Input.Key.E;
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += NewEditor;

                TextBlock editorText = new TextBlock("Editor", 3);
                editorText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)editorText.TextWidth), new PixelConstraint((int)editorText.TextHeight));
                button.AddChild(editorText);

                mainMenu.Add(button);
            }
            {
                Button button = new Button();
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(350), new PixelConstraint(300), new PixelConstraint(60));
                button.OnClick += () => { Scenes.LoadScene(new Transition(new Statistics(), 10)); };

                TextBlock text = new TextBlock("Statistik", 3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                button.AddChild(text);

                mainMenu.Add(button);
            }
            {
                Button button = new Button();
                button.Shortcut = OpenTK.Input.Key.Escape;
                button.Color = Color.FromArgb(255, 100, 100);
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(450), new PixelConstraint(300), new PixelConstraint(60));
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
