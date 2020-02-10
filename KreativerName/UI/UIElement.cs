using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    public abstract class UIElement : IDisposable
    {
        public UIElement()
        { }
        public UIElement(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        internal UI ui;
        internal UIElement parent;
        protected List<UIElement> children = new List<UIElement>();

        public List<UIElement> Children => children;
        public bool Visible { get; set; } = true;

        public int X { get; set; }
        public int Y { get; set; }
        public Vector2 Position => new Vector2(X, Y);

        public int ActualX => X + parent?.ActualX ?? 0;
        public int ActualY => Y + parent?.ActualY ?? 0;
        public Vector2 ActualPosition => new Vector2(ActualX, ActualY);

        public int Width { get; set; }
        public int Height { get; set; }

        public int MarginLeft { get; set; }
        public int MarginUp { get; set; }
        public int MarginRight { get; set; }
        public int MarginDown { get; set; }
        public Vector4 Margin => new Vector4(MarginLeft, MarginUp, MarginRight, MarginDown);

        public Alignment HorizontalAlign { get; set; }
        public Alignment VerticalAlign { get; set; }

        public abstract void Update(Vector2 windowSize);

        public abstract void Render(Vector2 windowSize);

        public void ClearChildren() => children.Clear();

        protected void UpdateChildren(Vector2 windowSize)
        {
            foreach (UIElement element in children)
            {
                element.Update(windowSize);
            }
        }

        protected void RenderChildren(Vector2 windowSize)
        {
            foreach (UIElement element in children)
            {
                if (element.Visible)
                    element.Render(windowSize);
            }
        }

        internal bool MouseOverChildren(Vector2 windowSize)
        {
            foreach (UIElement child in children)
            {
                if (child.MouseOver)
                    return true;
                else if (child.MouseOverChildren(windowSize))
                    return true;
            }
            return false;
        }

        public void AddChild(UIElement element)
        {
            element.parent = this;
            element.SetUI(ui);
            children.Add(element);
        }

        internal void SetUI(UI ui)
        {
            this.ui = ui;
            foreach (UIElement child in children)
            {
                child.SetUI(ui);
            }
        }

        #region Mouse

        internal bool MouseOver => 
            ui.MousePosition.X >= X &&
            ui.MousePosition.X < X + Width &&
            ui.MousePosition.Y >= Y &&
            ui.MousePosition.Y < Y + Height;

        protected bool MouseLeftDown => ui.Input.MouseDown(MouseButton.Left);
        protected bool MouseLeftUp => !MouseLeftDown;
        protected bool MouseLeftClick => ui.Input.MousePress(MouseButton.Left);
        protected bool MouseRightDown => ui.Input.MouseDown(MouseButton.Right);
        protected bool MouseRightUp => !MouseRightDown;
        protected bool MouseRightClick => ui.Input.MousePress(MouseButton.Right);
        protected bool MouseMiddleDown => ui.Input.MouseDown(MouseButton.Middle);
        protected bool MouseMiddleUp => !MouseMiddleDown;
        protected bool MouseMiddleClick => ui.Input.MousePress(MouseButton.Middle);

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    parent?.Dispose();
                }

                ui = null;
                children = null;

                disposedValue = true;
            }
        }

        ~UIElement()
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

    public enum Alignment
    {
        LeftOrTop,
        Center,
        RightOrBottom,
    }
}
