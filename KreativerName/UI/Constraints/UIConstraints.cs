using System;
using OpenTK;

namespace KreativerName.UI.Constraints
{
    public class UIConstraints : IDisposable
    {
        public UIConstraints() { }
        public UIConstraints(Constraint x, Constraint y, Constraint width, Constraint height)
        {
            xCon = x;
            yCon = y;
            widthCon = width;
            heightCon = height;
        }
        public UIConstraints(int x, int y, int width, int height)
        {
            xCon = new PixelConstraint(x);
            yCon = new PixelConstraint(y);
            widthCon = new PixelConstraint(width);
            heightCon = new PixelConstraint(height);
        }

        public Constraint xCon;
        public Constraint yCon;
        public Constraint widthCon;
        public Constraint heightCon;

        public float GetX(Vector2 windowSize, UIElement element) => xCon.GetX(windowSize, element);
        public float GetY(Vector2 windowSize, UIElement element) => yCon.GetY(windowSize, element);
        public float GetWidth(Vector2 windowSize, UIElement element) => widthCon.GetWidth(windowSize, element);
        public float GetHeight(Vector2 windowSize, UIElement element) => heightCon.GetHeight(windowSize, element);

        public static UIConstraints FullWindow =>
            new UIConstraints(new PixelConstraint(0), new PixelConstraint(0), new RelativeConstraint(1, RelativeTo.Window), new RelativeConstraint(1, RelativeTo.Window));

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // verwalteten Zustand (verwaltete Objekte) entsorgen.
                }

                xCon = null;
                yCon = null;
                widthCon = null;
                heightCon = null;

                disposedValue = true;
            }
        }

        ~UIConstraints()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(false);
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public enum RelativeTo
    {
        Window,
        Parent,
        Self,
    }

    public enum Direction
    {
        Left,
        Top,
        Right,
        Bottom,
    }
}
