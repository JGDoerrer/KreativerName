using KreativerName.UI;
using OpenTK;

namespace KreativerName.Scenes
{
    public static class SceneManager
    {
        public static Scene Scene;
        public static GameWindow Window;
        public static Input Input;

        static Scene next;
        static bool loadScene = false;

        public static void LoadScene(Scene scene)
        {
            next = scene;
            loadScene = true;
        }

        public static void Update(Vector2 windowSize)
        {
            if (loadScene)
            {
                Scene?.Unload();
                Scene = next;
                Scene?.Load();
                loadScene = false;
            }

            Scene?.UpdateUI(windowSize);
            Scene?.Update();

            Notification.Update();
            Input.Update();
        }

        public static void Render(Vector2 windowSize)
        {
            Scene?.Render(windowSize);

            Notification.Render(windowSize);
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

        public static void UnloadScene()
        {
            Scene?.Unload();
        }
    }
}
