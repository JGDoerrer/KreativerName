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
        EditorRenderer editorRenderer;
        GameState state = GameState.MainMenu;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (input.KeyPress(OpenTK.Input.Key.F11))
            {
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;
            }

            if (input.KeyDown(OpenTK.Input.Key.AltLeft) && input.KeyDown(OpenTK.Input.Key.F4))
                Close();

            switch (state)
            {
                case GameState.Game:
                    game.UpdateUI(new Vector2(Width, Height));
                    game.Update();
                    break;
                case GameState.MainMenu:
                    mainMenu.SetMouseState(input.MouseState());
                    mainMenu.Update(new Vector2(Width, Height));
                    break;
                case GameState.Editor:
                    editor.UpdateUI(new Vector2(Width, Height));
                    editor.Update();
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
                case GameState.Editor:
                    editorRenderer.Render(Width, Height);
                    break;
            }

            SwapBuffers();
        }

        private void InitUI()
        {
            mainMenu = new UI.UI();

            {
                float size = 5;
                Text title = new Text("KREATIVER NAME", size);
                title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)(title.String.Length * 6 * size)), new PixelConstraint((int)(6 * size)));
                title.Color = Color.White;
                mainMenu.Add(title);
            }
            {
                Button button = new Button();
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(150), new PixelConstraint(250), new PixelConstraint(60));
                button.OnClicked += NewGame;

                Text text = new Text("Spiel starten",3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint(text.String.Length * 18), new PixelConstraint(18));
                button.AddChild(text);

                mainMenu.Add(button);
            }
            {
                Button button = new Button();
                button.SetConstraints(new CenterConstraint(), new PixelConstraint(250), new PixelConstraint(250), new PixelConstraint(60));
                button.OnClicked += NewEditor;

                Text text = new Text("Editor",3);
                text.SetConstraints(new CenterConstraint(), new CenterConstraint(), new PixelConstraint(text.String.Length * 18), new PixelConstraint(18));
                button.AddChild(text);

                mainMenu.Add(button);
            }

        }

        private void NewGame()
        {
            game = new Game();
            game.input = input;
            game.LoadWorld(0);
            game.Exit += () =>
            {
                state = GameState.MainMenu;
                game = null;
            };

            gameRenderer = new GameRenderer(game);

            state = GameState.Game;
        }

        private void NewEditor()
        {
            editor = new Editor();
            editor.input = input;
            editor.Exit += () =>
            {
                state = GameState.MainMenu;
                editor = null;
            };

            editorRenderer = new EditorRenderer(editor);

            state = GameState.Editor;
        }

        enum GameState
        {
            MainMenu,
            Game,
            Editor,
        }
    }
}
