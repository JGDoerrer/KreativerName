using System.Collections.Generic;
using System.Drawing;
using System.IO;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class WorldMenu : Scene
    {
        public WorldMenu()
        {
            InitUI();
        }

        UI.UI ui;

        bool normalMode = true;

        private void InitUI()
        {
            List<World> worlds = new List<World>();
            int worldcount = 0;
            while (File.Exists($@"Resources\Worlds\{worldcount:000}.wld"))
            {
                worlds.Add(World.LoadFromFile($"{worldcount:000}"));
                worldcount++;
            }

            const int ButtonSize = 60;

            ui = new UI.UI();
            ui.Input = new Input(Scenes.Window);

            TextBlock title = new TextBlock("Welten", 4);
            title.Color = Color.White;
            title.SetConstraints(
                new CenterConstraint(),
                new PixelConstraint(50),
                new PixelConstraint((int)title.TextWidth),
                new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

            Frame worldFrame = new Frame();
            worldFrame.Color = Color.Transparent;
            worldFrame.SetConstraints(
                new CenterConstraint(),
                new PixelConstraint(180),
                new PixelConstraint(worldcount * (ButtonSize + 20) + 20),
                new PixelConstraint(ButtonSize + 40));

            Button modeButton = new Button();
            modeButton.Shortcut = Key.Tab;
            modeButton.Color = Color.FromArgb(100, 255, 100);
            modeButton.SetConstraints(new PixelConstraint(40, RelativeTo.Window, Direction.Left),
                new PixelConstraint(40, RelativeTo.Window, Direction.Bottom),
                new PixelConstraint(300),
                new PixelConstraint(60));

            TextBlock modeText = new TextBlock("Normal", 3);
            modeText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)modeText.TextWidth), new PixelConstraint((int)modeText.TextHeight));
            modeButton.AddChild(modeText);

            modeButton.OnClick += () =>
            {
                normalMode = !normalMode;

                modeText.Text = normalMode ? "Normal" : "Perfekt";
                modeButton.Color = normalMode ? Color.FromArgb(100, 255, 100) : Color.FromArgb(255, 100, 100);
                modeText.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)modeText.TextWidth), new PixelConstraint((int)modeText.TextHeight));
            };
            ui.Add(modeButton);

            Button exitButton = new Button(40, 40, 40, 40);
            exitButton.Shortcut = Key.Escape;
            exitButton.OnClick += () =>
            {
                Scenes.LoadScene(new Transition(new MainMenu(), 10));
            };
            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black);
            exitImage.SetConstraints(new UIConstaints(10, 10, 20, 20));

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            // Worlds
            for (int i = 0; i < worldcount; i++)
            {
                Button button = new Button((ButtonSize + 20) * i + 20, 20, ButtonSize, ButtonSize);

                List<bool> stars = new List<bool>();
                stars.Add(worlds[i].AllCompleted);
                stars.Add(worlds[i].AllPerfect);

                for (int j = 0; j < stars.Count; j++)
                {
                    UI.Image image = new UI.Image(Textures.Get("Icons"), new RectangleF(stars[j] ? 10 : 0, 0, 10, 10));
                    image.SetConstraints(new UIConstaints(
                        ButtonSize * (j + 1) / (stars.Count + 1) - 10,
                        ButtonSize - 15, 20, 20));

                    button.AddChild(image);
                }

                if (i < 10)
                    button.Shortcut = (Key)(110 + i);

                if (i > 0)
                    button.Enabled = worlds[i - 1].AllCompleted || worlds[i - 1].AllPerfect;

                int world = i;
                button.OnClick += () =>
                {
                    NewGame(world);
                };

                TextBlock text = new TextBlock((i + 1).ToString(), 3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                button.AddChild(text);
                worldFrame.AddChild(button);
            }

            ui.Add(worldFrame);
        }

        public override void Update()
        { }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);
        }

        private void NewGame(int world)
        {
            Game game = new Game(world, !normalMode);
            game.Exit += () =>
            {
                game.World.SaveToFile($"{world:000}");
                Scenes.LoadScene(new Transition(new WorldMenu(), 10));
            };

            Scenes.LoadScene(new Transition(game, 10));
        }

    }
}
