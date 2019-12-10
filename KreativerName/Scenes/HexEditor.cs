using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using OpenTK;

namespace KreativerName.Scenes
{
    class HexEditor : Scene
    {
        public HexEditor(HexData data)
        {
            this.data = data;

            InitUI();
        }

        UI.UI ui;
        TextBlock text;

        HexData data;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        int frameCount = 0;
        HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            32, 0.5f);

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = Scenes.Input;

            text = new TextBlock("", 2, 20, 20);
            text.Color = Color.White;

            NumberInput input = new NumberInput("{0}", 200, 200);
            input.AddTo(ui);

            ui.Add(text);
        }

        public override void Update()
        {
            text.Text = $"ID: {data.ID}";
            
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);

            layout.origin = new Vector2(400, 100);
            GridRenderer.RenderHex(new Hex(0, 0, data.ID), layout, Color.White, frameCount);

            frameCount++;
        }

        struct NumberInput
        {
            public NumberInput(string format, int x, int y)
            {
                this.format = format;
                value = 0;

                text = new TextBlock(string.Format(format, value), 2, x, y);
                add = new Button(x + 30,y, 20, 20);
                sub = new Button(x + 60,y, 20, 20);

                text.Color = Color.White;
                add.AddChild(new TextBlock("+", 2f, 5, 3));
                sub.AddChild(new TextBlock("-", 2f, 5, 3));

                add.OnClick += Add;
                sub.OnClick += Subtract;
            }

            int value;
            string format;

            TextBlock text;
            Button add;
            Button sub;

            public void AddTo(UI.UI ui)
            {
                ui.Add(text);
                ui.Add(add);
                ui.Add(sub);
            }

            private void Add()
            {
                value++;
                text.Text = string.Format(format, value);
            }

            private void Subtract()
            {
                value--;
                text.Text = string.Format(format, value);
            }
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

        ~HexEditor()
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
