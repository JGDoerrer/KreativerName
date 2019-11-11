using OpenTK;

namespace KreativerName.Scenes
{
    public static class Scenes
    {
        static Scene scene;
        public static GameWindow Window;
        public static Input Input;

        public static void LoadScene(Scene scene)
        {
            Scenes.scene = scene;
        }

        public static void Update(Vector2 windowSize)
        {
            scene?.Update();
            scene?.UpdateUI(windowSize);
        }

        public static void Render(Vector2 windowSize)
        {
            scene?.Render(windowSize);
        }

        public static void SetWindow(GameWindow window)
        {            
            Window = window;
            Input = new Input(window);
        }

        public static void CloseWindow()
        {
            Window.Close();
        }
    }
}
