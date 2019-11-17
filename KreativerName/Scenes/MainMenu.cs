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

            Button startButton = new Button();
            startButton.Color = Color.FromArgb(100, 255, 100);
            startButton.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(300), new PixelConstraint(60));
            startButton.OnClick += () => { Scenes.LoadScene(new Transition(new WorldMenu(), 10)); };

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
