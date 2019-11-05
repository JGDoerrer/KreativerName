using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace KreativerName.UI
{
    public abstract class UIElement
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

        public abstract void Update(Vector2 windowSize);

        public abstract void Render(Vector2 windowSize);

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

        protected bool MouseLeftDown => ui.mouseState.LeftButton == ButtonState.Pressed;
        protected bool MouseLeftUp => ui.mouseState.LeftButton == ButtonState.Released;
        protected bool MouseLeftClick => MouseLeftDown && ui.previousMouseState.LeftButton == ButtonState.Released;
        protected bool MouseRightDown => ui.mouseState.RightButton == ButtonState.Pressed;
        protected bool MouseRightUp => ui.mouseState.RightButton == ButtonState.Released;
        protected bool MouseRightClick => MouseRightDown && ui.previousMouseState.RightButton == ButtonState.Released;
        protected bool MouseMiddleDown => ui.mouseState.MiddleButton == ButtonState.Pressed;
        protected bool MouseMiddleUp => ui.mouseState.MiddleButton == ButtonState.Released;
        protected bool MouseMiddleClick => MouseMiddleDown && ui.previousMouseState.MiddleButton == ButtonState.Released;

        #endregion
    }
}
