using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.Scenes
{
    public class WorldMenu : Scene
    {
        public WorldMenu()
        {
            InitUI();
        }

        UI.UI worldMenu;

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
                new PixelConstraint(worldcount * 50 + 30),
                new PixelConstraint(80));

            for (int i = 0; i < worldcount; i++)
            {
                Button button = new Button(50 * i + 20, 20, 40, 40);

                button.Color = worlds[i].AllCompleted ? Color.Green : Color.White;

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

        public override void Update()
        {
            if (Scenes.Input.KeyPress(OpenTK.Input.Key.Escape))
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
            Game game = new Game(world);
            game.input = Scenes.Input;
            game.Exit += () =>
            {
                Scenes.LoadScene(new Transition(new WorldMenu(), 10));
            };

            Scenes.LoadScene(new Transition(game, 10));
        }

    }
}
