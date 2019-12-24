using System;
using System.Collections.Generic;
using System.Drawing;
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
            ui.Input = SceneManager.Input;

            {
                TextBlock text = new TextBlock($"ID: {data.ID}", 2, 20, 20);
                text.Color = Color.White;
                ui.Add(text);
            }

            void AddLine(string desc, int value, int y, ValueEvent e)
            {
                TextBlock text = new TextBlock(desc, 2, 20, y);
                text.Color = Color.White;

                NumberInput input = new NumberInput(20 + (int)text.TextWidth, y - 3, value)
                {
                    MinValue = byte.MinValue,
                    MaxValue = byte.MaxValue,
                };
                input.ValueChanged += e;

                input.AddTo(ui);

                ui.Add(text);
            }

            AddLine("Textur:       ", data.Texture, 40, a => data.Texture = (byte)a);
            AddLine("Anim.länge:   ", data.AnimationLength, 60, a => data.AnimationLength = (byte)a);
            AddLine("Anim.phase:   ", data.AnimationPhase, 80, a => data.AnimationPhase = (byte)a);
            AddLine("Anim.geschw.: ", data.AnimationSpeed, 100, a => data.AnimationSpeed = (byte)a);

        }

        public override void Update()
        {
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);

            layout.origin = new Vector2(400, 400);
            GridRenderer.RenderHex(new HexPoint(0, 0), new List<HexData> { data }, layout, Color.White, frameCount);

            frameCount++;
        }

        class NumberInput
        {
            public NumberInput(int x, int y, int value = 0)
            {
                Value = value;

                text = new TextBlock(Value.ToString(), 2, x + 22, y + 3);
                add = new Button(x, y, 20, 20);
                sub = new Button(x + 72, y, 20, 20);

                text.Color = Color.White;
                add.AddChild(new TextBlock("+", 2f, 5, 3));
                sub.AddChild(new TextBlock("-", 2f, 5, 3));

                MaxValue = 9999;
                MinValue = -999;

                add.OnClick += () =>
                {
                    if (add.ui.Input.KeyDown(OpenTK.Input.Key.LControl) ||
                        add.ui.Input.KeyDown(OpenTK.Input.Key.RControl))
                        Value += 10;
                    else if (add.ui.Input.KeyDown(OpenTK.Input.Key.LShift) ||
                        add.ui.Input.KeyDown(OpenTK.Input.Key.RShift))
                        Value += 5;
                    else
                        Value += 1;

                    Value = Value.Clamp(MinValue, MaxValue);

                    text.Text = Value.ToString();
                    ValueChanged?.Invoke(Value);
                };
                sub.OnClick += () =>
                {
                    if (add.ui.Input.KeyDown(OpenTK.Input.Key.LControl) ||
                        add.ui.Input.KeyDown(OpenTK.Input.Key.RControl))
                        Value -= 10;
                    else if (add.ui.Input.KeyDown(OpenTK.Input.Key.LShift) ||
                        add.ui.Input.KeyDown(OpenTK.Input.Key.RShift))
                        Value -= 5;
                    else
                        Value -= 1;

                    Value = Value.Clamp(MinValue, MaxValue);

                    text.Text = Value.ToString();
                    ValueChanged?.Invoke(Value);
                };
            }

            public int MaxValue;
            public int MinValue;

            public int Value;

            public event ValueEvent ValueChanged;

            TextBlock text;
            Button add;
            Button sub;

            public void AddTo(UI.UI ui)
            {
                ui.Add(text);
                ui.Add(add);
                ui.Add(sub);
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
