using System;
using System.ComponentModel;
using System.Drawing;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Scenes
{
    class LoadingScene : Scene
    {
        public LoadingScene(DoWorkEventHandler action, Scene next = null)
        {
            worker = new BackgroundWorker();

            worker.DoWork += action;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += (o, e) => { Done?.Invoke(); Scenes.LoadScene(next); };
            worker.WorkerReportsProgress = true;

            worker.RunWorkerAsync();

            text = new TextBlock("", 5);
            text.Color = Color.Gray;
            text.SetConstraints(new CenterConstraint(),
                new CenterConstraint(),
                new PixelConstraint((int)text.TextWidth),
                new PixelConstraint((int)text.TextHeight));
        }

        BackgroundWorker worker;
        TextBlock text;

        float prevPercent;
        float percent;
        int animation = 0;

        float AnimatedPercent => (QuarticOut(1 - (float)animation / 60)) * (percent - prevPercent) / 100f + prevPercent / 100f;

        public event EmptyEvent Done;

        public override void Update()
        {
            if (animation > 0)
                animation--;
        }

        public override void UpdateUI(Vector2 windowSize)
        {
        }

        public override void Render(Vector2 windowSize)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb(255, 255, 255, 255));
            GL.Vertex2(windowSize.X / 3, windowSize.Y / 3);
            GL.Vertex2(windowSize.X / 3, windowSize.Y / 3 * 2);
            GL.Vertex2(windowSize.X / 3 * (AnimatedPercent + 1), windowSize.Y / 3 * 2);
            GL.Vertex2(windowSize.X / 3 * (AnimatedPercent + 1), windowSize.Y / 3);
            GL.End();
            GL.Enable(EnableCap.Texture2D);

            text.Render(windowSize);
        }

        private float QuadraticInOut(float t)
           => t * t / (2 * t * t - 2 * t + 1);
        private float QuarticOut(float t)
           => -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prevPercent = AnimatedPercent * 100;
            percent = e.ProgressPercentage;
            animation = 60;


            text.Text = $"{percent}%";
            text.Constraints.widthCon = new PixelConstraint((int)text.TextWidth);
        }


        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        ~LoadingScene()
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
