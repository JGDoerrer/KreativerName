﻿using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Scenes
{
    public class Transition : Scene
    {
        public Transition(Scene next, int duration)
        {
            this.next = next;
            totalDuration = duration;
            this.duration = duration;
        }

        Scene next;
        int duration;
        int totalDuration;

        public override void Render(Vector2 windowSize)
        {
            next?.Render(windowSize);

            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            GL.Viewport(0, 0, width, height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Draw black window
            GL.Disable(EnableCap.Texture2D);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb((int)(QuarticOut((float)duration / totalDuration) * 255), 0, 0, 0));
            GL.Vertex2(0, 0);
            GL.Vertex2(0, height);
            GL.Vertex2(width, height);
            GL.Vertex2(width, 0);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }

        public override void Update()
        {
            duration--;

            if (duration == 0)
                Scenes.LoadScene(next);
        }

        public override void UpdateUI(Vector2 windowSize)
        { }

        private float QuadraticInOut(float t)
           => t * t / (2 * t * t - 2 * t + 1);
        private float QuarticOut(float t)
           => -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;

    }
}
