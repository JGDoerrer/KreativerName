using System.Drawing;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KreativerName
{
    class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            editor = new Editor();
            editor.input = input;

            InitUI();

            Textures.LoadTextures(@"Resources\Textures");

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        UI.UI mainMenu;
        Input input;
        Game game;
        Editor editor;
        GameRenderer gameRenderer;
        GameState state = GameState.MainMenu;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            switch (state)
            {
                case GameState.Game:
                    game.Update();
                    break;
                case GameState.MainMenu:
                    mainMenu.SetMouseState(input.MouseState());
                    mainMenu.Update(new Vector2(Width, Height));
                    break;
            }

            input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            switch (state)
            {
                case GameState.Game:
                    gameRenderer.Render(Width, Height);
                    break;
                case GameState.MainMenu:
                    mainMenu.Render(Width, Height);
                    break;
            }

            SwapBuffers();
        }

        private void InitUI()
        {
            mainMenu = new UI.UI();

            {
                float size = 4;
                Text title = new Text("KREATIVER NAME", size);
                title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)(title.String.Length * 6 * size)), new PixelConstraint((int)(6 * size)));
                title.Color = Color.White;
                mainMenu.Add(title);
            }

            {
                Button button = new Button();
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(200), new PixelConstraint(60));
                button.OnClicked += NewGame;

                Text text = new Text("Spiel starten");
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint(text.String.Length * 12), new PixelConstraint(12));
                button.AddChild(text);

                mainMenu.Add(button);
            }

            {
                Button button = new Button();
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(200), new PixelConstraint(60));
                button.OnClicked += () => state = GameState.Editor;

                Text text = new Text("Editor");
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint(text.String.Length * 12), new PixelConstraint(12));
                button.AddChild(text);

                mainMenu.Add(button);
            }

        }

        private void NewGame()
        {
            game = new Game();
            game.input = input;
            game.Exit += () => state = GameState.MainMenu;

            gameRenderer = new GameRenderer(game);

            state = GameState.Game;
        }

        enum GameState
        {
            MainMenu,
            Game,
            Editor,
        }
    }
}
