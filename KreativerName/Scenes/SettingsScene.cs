using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class SettingsScene : Scene
    {
        public SettingsScene()
        {
            InitUI();
        }

        ~SettingsScene()
        {
            ui.Dispose();
        }

        UI.UI ui;

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(Scenes.Window);

            TextBlock title = new TextBlock("Einstellungen", 4);
            title.Color = Color.White;
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

            {
                Button button = new Button(40, 40, 40, 40);
                button.Shortcut = OpenTK.Input.Key.Escape;
                button.OnClick += () =>
                {
                    Scenes.LoadScene(new Transition(new MainMenu(), 10));
                };
                UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
                exitImage.SetConstraints(new UIConstaints(10, 10, 20, 20));

                button.AddChild(exitImage);
                ui.Add(button);
            }

            void AddCheckBox(string s, int y, bool state, CheckEvent clickEvent)
            {
                Frame frame = new Frame();
                frame.Color = Color.Transparent;

                CheckBox box = new CheckBox(0, 0, 36, 36);
                box.Checked = state;
                box.OnChecked += clickEvent;
                frame.AddChild(box);

                TextBlock text = new TextBlock(s, 3, 45, 0);
                text.Constaints.yCon = new CenterConstraint();
                text.Color = Color.White;
                frame.AddChild(text);

                frame.SetConstraints(new CenterConstraint(), new PixelConstraint(y), new PixelConstraint(45 + (int)text.TextWidth), new PixelConstraint(36));
                ui.Add(frame);
            }


            AddCheckBox("Vollbild", 100, Settings.Current.Fullscreen, (b) =>
            {
                Settings.Current.Fullscreen = b;
                Scenes.Window.WindowState = b ? WindowState.Fullscreen : WindowState.Normal;
            });
            AddCheckBox("Zuege anzeigen", 150, Settings.Current.ShowMoves, (b) =>
            {
                Settings.Current.ShowMoves = b;
            });
        }

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
    }
}
