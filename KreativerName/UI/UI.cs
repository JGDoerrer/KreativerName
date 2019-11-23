﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace KreativerName.UI
{
    public class UI : IDisposable
    {
        public UI()
        {

        }

        List<UIElement> Elements { get; set; } = new List<UIElement>();
        internal Input Input; //{ get; set; }

        internal Vector2 MousePosition => Input?.MousePosition ?? new Vector2();

        public void Update(Vector2 windowSize)
        {
            foreach (UIElement element in Elements)
            {
                element.Update(windowSize);
            }

            //Input.Update();
        }

        public void Render(Vector2 windowSize)
        {
            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            GL.Viewport(0, 0, width, height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            foreach (var element in Elements)
            {
                if (element.Visible)
                    element.Render(windowSize);
            }
        }

        public void Add(UIElement element)
        {
            element.SetUI(this);
            Elements.Add(element);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (UIElement element in Elements)
                    {
                        element.Dispose();
                    }
                }
                
                Elements = null;

                disposedValue = true;
            }
        }

        ~UI()
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
