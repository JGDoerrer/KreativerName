using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public class Input
    {
        public Input(GameWindow window)
        {
            Initialise(window);
        }

        private List<Key> keysDown;
        private List<Key> keysDownLast;
        private List<MouseButton> mouseDown;
        private List<MouseButton> mouseDownLast;
        private Vector2 mousePosition;

        private int mouseWheel;
        private int mouseWheelLast;
        private MouseState state;

        public void Initialise(GameWindow window)
        {
            Reset();

            window.KeyDown += KeyDown;
            window.KeyUp += KeyUp;
            window.MouseDown += MouseDown;
            window.MouseUp += MouseUp;
            window.MouseMove += MouseMove;
            window.MouseWheel += MouseWheel;
        }

        public void PressKey(Key key) => keysDown.Add(key);
        public void PressMouse(MouseButton button) => mouseDown.Add(button);
        public void MoveMouse(Vector2 pos) => mousePosition = pos;
        public void ScrollMouse(int amount) => mouseWheel += amount;

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mouseWheel = e.Value;
            state = e.Mouse;
        }
        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            while (mouseDown.Contains(e.Button))
                mouseDown.Remove(e.Button);
            state = e.Mouse;
        }
        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown.Add(e.Button);
            state = e.Mouse;
        }
        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            mousePosition = new Vector2(e.X, e.Y);
            state = e.Mouse;
        }

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            while (keysDown.Contains(e.Key))
                keysDown.Remove(e.Key);
        }
        private void KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        public void Update()
        {
            keysDownLast = new List<Key>(keysDown);
            mouseDownLast = new List<MouseButton>(mouseDown);
            mouseWheelLast = mouseWheel;
        }

        public void Reset()
        {
            ResetKeys();
            //mousePosition = new Vector2();
            mouseWheel = 0;
            mouseWheelLast = 0;
        }

        public void ResetKeys()
        {
            keysDown = new List<Key>();
            keysDownLast = new List<Key>();
            mouseDown = new List<MouseButton>();
            mouseDownLast = new List<MouseButton>();
        }

        public bool KeyPress(Key key) => keysDown.Contains(key) && !keysDownLast.Contains(key);
        public bool KeyDown(Key key) => keysDown.Contains(key);
        public bool KeyRelease(Key key) => !keysDown.Contains(key) && keysDownLast.Contains(key);

        public bool MousePress(MouseButton button) => mouseDown.Contains(button) && !mouseDownLast.Contains(button);
        public bool MouseDown(MouseButton button) => mouseDown.Contains(button);
        public bool MouseRelease(MouseButton button) => !mouseDown.Contains(button) && mouseDownLast.Contains(button);
        public Vector2 MousePosition() => mousePosition;
        public MouseState MouseState() => state;

        public int MouseWheel() => mouseWheel;
        public int MouseScroll() => mouseWheel - mouseWheelLast;
    }
}
