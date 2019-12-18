using System;
using System.Drawing;
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

            //next?.Update();

            if (duration == 0)
                SceneManager.LoadScene(next);
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            //next?.UpdateUI(windowSize);
        }

        private float QuadraticInOut(float t)
           => t * t / (2 * t * t - 2 * t + 1);
        private float QuarticOut(float t)
           => -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    next.Dispose();
                }

                disposedValue = true;
            }
        }

        ~Transition()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(false);
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public override void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
