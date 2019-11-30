using OpenTK;

namespace KreativerName.Scenes
{
    public static class Scenes
    {
        public static Scene Scene;
        public static GameWindow Window;
        public static Input Input;

        public static void LoadScene(Scene scene)
        {
            Scene = null;
            Scene = scene;
        }

        public static void Update(Vector2 windowSize)
        {
            Scene?.UpdateUI(windowSize);
            Scene?.Update();

            Input.Update();
        }

        public static void Render(Vector2 windowSize)
        {
            Scene?.Render(windowSize);
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
