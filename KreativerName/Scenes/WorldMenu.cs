using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        UI.UI worldMenu;

        bool normalMode = true;

        public void InitUI()
        {
            List<World> worlds = new List<World>();
            int worldcount = 0;
            while (File.Exists($@"Resources\Worlds\{worldcount:000}.wld"))
            {
                worlds.Add(World.LoadFromFile($"{worldcount:000}"));
                worldcount++;
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
                new PixelConstraint(worldcount * 80 + 20),
                new PixelConstraint(100));

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
            worldMenu.Add(modeButton);

            for (int i = 0; i < worldcount; i++)
            {
                Button button = new Button(80 * i + 20, 20, 60, 60);

                if (worlds[i].AllPerfect)
                    button.Color = Color.Gold;
                else if (worlds[i].AllCompleted)
                    button.Color = Color.Green;
                else
                    button.Color = Color.White;

                if (i < 10)
                    button.Shortcut = (Key)(110 + i);

                int world = i;
                button.OnClick += () =>
                {
                    NewGame(world);
                };

                TextBlock text = new TextBlock((i + 1).ToString(), 3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));
                button.AddChild(text);
                frame.AddChild(button);
            }

            worldMenu.Add(frame);
        }

        public override void Update()
        {
            if (Scenes.Input.KeyPress(Key.Escape))
                Scenes.LoadScene(new Transition(new MainMenu(), 10));
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            worldMenu.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            worldMenu.Render(windowSize);
        }

        private void NewGame(int world)
        {
            Game game = new Game(world, !normalMode);
            game.input = Scenes.Input;
            game.Exit += () =>
            { 
                Scenes.LoadScene(new Transition(new WorldMenu(), 10));
            };

            Scenes.LoadScene(new Transition(game, 10));
        }

    }
}
