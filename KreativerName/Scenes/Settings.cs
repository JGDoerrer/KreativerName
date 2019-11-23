using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class Settings : Scene
    {
        public Settings()
        {
            InitUI();
        }

        ~Settings()
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
            {
                Frame frame = new Frame();
                frame.Color = Color.Transparent;
                frame.SetConstraints(new CenterConstraint(), new PixelConstraint(100), new PixelConstraint(300), new PixelConstraint(36));

                CheckBox box = new CheckBox(0, 0, 36, 36);
                box.Checked = KreativerName.Settings.Current.Fullscreen;
                box.OnClick += () =>
                {
                    KreativerName.Settings.Current.Fullscreen = box.Checked;
                    Scenes.Window.WindowState = box.Checked ? WindowState.Fullscreen : WindowState.Normal;
                };
                frame.AddChild(box);

                TextBlock text = new TextBlock("Vollbild", 3, 45, 0);
                text.Constaints.yCon = new CenterConstraint();
                text.Color = Color.White;
                frame.AddChild(text);

                ui.Add(frame);
            }
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
