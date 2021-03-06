﻿using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace KreativerName
{
    public class Input
    {
        public Input(GameWindow window)
        {
            Initialise(window);
        }

        List<Key> keysDown;
        List<Key> keysDownLast;
        List<Key> keysRepeat;
        List<MouseButton> mouseDown;
        List<MouseButton> mouseDownLast;
        Vector2 mousePosition;

        int mouseWheel;
        int mouseWheelLast;
        MouseState mouseState;
        KeyboardState keyState;

        public void Initialise(GameWindow window)
        {
            Reset();

            window.KeyDown += KeyDown;
            window.KeyUp += KeyUp;
            window.MouseDown += MouseDown;
            window.MouseUp += MouseUp;
            window.MouseMove += MouseMove;
            window.MouseWheel += MouseWheel;
            window.KeyPress += KeyPress;
        }

        public void PressKey(Key key) => keysDown.Add(key);
        public void PressMouse(MouseButton button) => mouseDown.Add(button);
        public void MoveMouse(Vector2 pos) => mousePosition = pos;
        public void ScrollMouse(int amount) => mouseWheel += amount;
        public void ReleaseKey(Key key)
        {
            while (keysDown.Contains(key))
                keysDown.Remove(key);
        }
        public void ReleaseMouse(MouseButton button)
        {
            while (mouseDown.Contains(button))
                mouseDown.Remove(button);
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mouseWheel = e.Value;
            mouseState = e.Mouse;
        }
        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            while (mouseDown.Contains(e.Button))
                mouseDown.Remove(e.Button);
            mouseState = e.Mouse;
        }
        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown.Add(e.Button);
            mouseState = e.Mouse;
        }
        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            mousePosition = new Vector2(e.X, e.Y);
            mouseState = e.Mouse;
        }

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            while (keysDown.Contains(e.Key))
                keysDown.Remove(e.Key);
            while (keysRepeat.Contains(e.Key))
                keysRepeat.Remove(e.Key);
            keyState = e.Keyboard;
        }
        private void KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.IsRepeat)
                keysRepeat.Add(e.Key);
            keysDown.Add(e.Key);
            keyState = e.Keyboard;
        }
        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyString += e.KeyChar;
        }

        public void Update()
        {
            keysDownLast = new List<Key>(keysDown);
            mouseDownLast = new List<MouseButton>(mouseDown);
            mouseWheelLast = mouseWheel;
            KeyString = "";
        }

        public void Reset()
        {
            ResetKeys();
            //mousePosition = new Vector2();
            ResetMouse();
        }

        public void ResetMouse()
        {
            mouseWheel = 0;
            mouseWheelLast = 0;
            mouseDown = new List<MouseButton>();
            mouseDownLast = new List<MouseButton>();
        }

        public void ResetKeys()
        {
            keysDown = new List<Key>();
            keysDownLast = new List<Key>();
            keysRepeat = new List<Key>();
            KeyString = "";
        }

        public bool KeyPress(Key key) => keysDown.Contains(key) && !keysDownLast.Contains(key);
        public bool KeyDown(Key key) => keysDown.Contains(key);
        public bool KeyRelease(Key key) => !keysDown.Contains(key) && keysDownLast.Contains(key);
        public bool KeyRepeat(Key key) => keysRepeat.Contains(key);

        public bool MousePress(MouseButton button) => mouseDown.Contains(button) && !mouseDownLast.Contains(button);
        public bool MouseDown(MouseButton button) => mouseDown.Contains(button);
        public bool MouseRelease(MouseButton button) => !mouseDown.Contains(button) && mouseDownLast.Contains(button);
        public Vector2 MousePosition => mousePosition;
        public MouseState MouseState => mouseState;
        public KeyboardState KeyState => keyState;
        public string KeyString { get; private set; }

        public int MouseWheel() => mouseWheel;
        public int MouseScroll() => mouseWheel - mouseWheelLast;
    }
}
