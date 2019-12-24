﻿using System;
using System.Drawing;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Scenes
{
    public class ErrorScene : Scene
    {
        public ErrorScene(Exception e)
        {
            InitUI(e);
        }

        UI.UI ui;

        private void InitUI(Exception e)
        {
            ui = new UI.UI();

            void AddText(string s, float size, int y)
            {
                TextBlock text = new TextBlock(s, size);
                text.SetConstraints(new CenterConstraint(), new PixelConstraint(y), new PixelConstraint((int)text.TextWidth), new PixelConstraint((int)text.TextHeight));

                ui.Add(text);
            }

            AddText("Fehler!", 3, 50);
            AddText($"{e.Message}", 2, 100);

            string[] stack = e.StackTrace.Split('\n');
            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = stack[i].Trim().Replace(@"D:\Programmieren\Source\KreativerName\KreativerName\", "");
                AddText($"{stack[i]}", 1, 150 + i * 10);
            }

            AddText($"Bitte an den Entwickler weiterleiten!", 3, 180 + stack.Length * 10);
        }

        public override void Update()
        {
            if (SceneManager.Input.KeyPress(OpenTK.Input.Key.Escape) ||
                SceneManager.Input.KeyPress(OpenTK.Input.Key.Space) ||
                SceneManager.Input.KeyPress(OpenTK.Input.Key.Enter))
            {
                SceneManager.CloseWindow();
            }
        }

        public override void UpdateUI(Vector2 windowSize)
        {
        }

        public override void Render(Vector2 windowSize)
        {
            GL.ClearColor(Color.FromArgb(90, 0, 0));

            ui.Render(windowSize);
        }

        #region IDisposable Support

        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ui.Dispose();
                }

                disposedValue = true;
            }
        }

        ~ErrorScene()
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