using System;
using System.Collections.Generic;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.UI
{
    public abstract class UIElement : IDisposable
    {
        public UIElement()
        {
            constaints = new UIConstaints();
        }
        public UIElement(Constraint x, Constraint y, Constraint width, Constraint height)
        {
            constaints = new UIConstaints(x, y, width, height);
        }

        protected UIConstaints constaints;

        internal UI ui;
        internal UIElement parent;
        protected List<UIElement> children = new List<UIElement>();

        internal bool HasParent => parent != null;
        public List<UIElement> Children => children;
        public bool Visible { get; set; } = true;
        public UIConstaints Constaints => constaints;

        public abstract void Update(Vector2 windowSize);

        public abstract void Render(Vector2 windowSize);

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

        public void SetConstraints(UIConstaints constaints) => this.constaints = constaints;
        public void SetConstraints(Constraint x, Constraint y, Constraint width, Constraint height) => constaints = new UIConstaints(x, y, width, height);

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

        internal float GetX(Vector2 windowSize) => constaints.GetX(windowSize, this);
        internal float GetY(Vector2 windowSize) => constaints.GetY(windowSize, this);
        internal float GetWidth(Vector2 windowSize) => constaints.GetWidth(windowSize, this);
        internal float GetHeight(Vector2 windowSize) => constaints.GetHeight(windowSize, this);

        #region Mouse

        protected bool MouseOver(Vector2 windowSize)
        {
            float x = constaints.GetX(windowSize, this);
            float y = constaints.GetY(windowSize, this);
            float w = constaints.GetWidth(windowSize, this);
            float h = constaints.GetHeight(windowSize, this);

            return ui.MousePosition.X >= x &&
                   ui.MousePosition.X < x + w &&
                   ui.MousePosition.Y >= y &&
                   ui.MousePosition.Y < y + h;
        }

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
                    constaints?.Dispose();
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
}
