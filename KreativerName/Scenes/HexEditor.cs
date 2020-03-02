using System;
using System.Collections.Generic;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    class HexEditor : Scene
    {
        public HexEditor(HexData data)
        {
            Data = data;

            InitUI();
        }

        UI.UI ui;

        public HexData Data;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        int frameCount = 0;
        HexLayout layout = new HexLayout(
            new Matrix2(sqrt3, sqrt3 / 2f, 0, 3f / 2f),
            new Matrix2(sqrt3 / 3f, -1f / 3f, 0, 2f / 3f),
            new Vector2(0, 0),
            32, 0.5f);

        public event EmptyEvent OnExit;

        private void InitUI()
        {
            ui = new UI.UI
            {
                Input = SceneManager.Input
            };

            Button exitButton = new Button(20, 20, 40, 40)
            { Shortcut = Key.Escape };
            exitButton.OnLeftClick += (sender) => OnExit?.Invoke();

            UI.Image exitImage = new UI.Image(Textures.Get("Icons"), new RectangleF(0, 10, 10, 10), Color.Black)
            { Constraints = new UIConstraints(10, 10, 20, 20) };

            exitButton.AddChild(exitImage);
            ui.Add(exitButton);

            void AddNumber(string desc, int value, int y, ValueEvent e, byte min = byte.MinValue, byte max = byte.MaxValue)
            {
                TextBlock text = new TextBlock(desc, 2, 20, y)
                {
                    Color = Color.White
                };

                NumberInput input = new NumberInput(20 + (int)text.TextWidth, y - 3, value)
                {
                    MinValue = min,
                    MaxValue = max,
                };
                input.ValueChanged += e;

                input.AddTo(ui);

                ui.Add(text);
            }

            void AddCheckBox(string desc, int y, bool check, CheckEvent e)
            {
                TextBlock text = new TextBlock(desc, 2, 20, y + 2)
                {
                    Color = Color.White
                };

                CheckBox checkBox = new CheckBox(20 + (int)text.TextWidth, y, 24, 24)
                {
                    Checked = check
                };
                checkBox.OnChecked += e;

                ui.Add(text);
                ui.Add(checkBox);
            }

            AddNumber("Id:           ", Data.ID, 100, a => Data.ID = (byte)a);
            AddNumber("Textur:       ", Data.Texture, 120, a => Data.Texture = (byte)a);
            AddNumber("Anim.länge:   ", Data.AnimationLength, 140, a => Data.AnimationLength = (byte)a);
            AddNumber("Anim.phase:   ", Data.AnimationPhase, 160, a => Data.AnimationPhase = (byte)a);
            AddNumber("Anim.geschw.: ", Data.AnimationSpeed, 180, a => Data.AnimationSpeed = (byte)a);

            AddCheckBox("Solide:       ", 200, Data.HexFlags.HasFlag(HexFlags.Solid), a => { if (a) Data.HexFlags |= HexFlags.Solid; else Data.HexFlags &= ~HexFlags.Solid; });
            AddCheckBox("Tödlich:      ", 224, Data.HexFlags.HasFlag(HexFlags.Deadly), a => { if (a) Data.HexFlags |= HexFlags.Deadly; else Data.HexFlags &= ~HexFlags.Deadly; });
            AddCheckBox("Ziel:         ", 248, Data.HexFlags.HasFlag(HexFlags.Goal), a => { if (a) Data.HexFlags |= HexFlags.Goal; else Data.HexFlags &= ~HexFlags.Goal; });

            AddCheckBox("Animiert:     ", 272, Data.RenderFlags.HasFlag(RenderFlags.Animated), a => { if (a) Data.RenderFlags |= RenderFlags.Animated; else Data.RenderFlags &= ~RenderFlags.Animated; });
            AddCheckBox("Verbunden:    ", 296, Data.RenderFlags.HasFlag(RenderFlags.Connected), a => { if (a) Data.RenderFlags |= RenderFlags.Connected; else Data.RenderFlags &= ~RenderFlags.Connected; });

            int count = 0;
            for (int i = 0; i < Data.Changes.Count; i++)
            {
                Frame frame = new Frame(400 + 300 * count, 100, 300, 200);
                frame.Color = Color.Transparent;

                void AddNumber2(string desc, int value, int y, ValueEvent e, int min = byte.MinValue, int max = byte.MaxValue)
                {
                    TextBlock text = new TextBlock(desc, 2, 0, y)
                    {
                        Color = Color.White
                    };

                    NumberInput input = new NumberInput((int)text.TextWidth, y - 3, value)
                    {
                        MinValue = min,
                        MaxValue = max,
                    };
                    input.ValueChanged += e;

                    input.AddTo(frame);
                    frame.AddChild(text);
                }

                void AddCheckBox2(string desc, int y, bool check, CheckEvent e)
                {
                    TextBlock text = new TextBlock(desc, 2, 0, y + 6)
                    {
                        Color = Color.White
                    };

                    CheckBox checkBox = new CheckBox((int)text.TextWidth, y, 24, 24)
                    {
                        Checked = check
                    };
                    checkBox.OnChecked += e;

                    frame.AddChild(text);
                    frame.AddChild(checkBox);
                }


                frame.AddChild(new TextBlock($"Aktion {i+1}:", 2, 0, 0) { Color = Color.White });

                int copy = i;
                HexAction action = Data.Changes[copy];
                AddNumber2("Ändere zu:  ", action.ChangeTo, 20, a => { action.ChangeTo = (byte)a; Data.Changes[copy] = action; });
                AddNumber2("Daten:      ", action.Data, 40, a => { action.Data = (byte)a; Data.Changes[copy] = action; });
                AddNumber2("Bewege x:   ", action.MoveX, 60, a => { action.MoveX = (sbyte)a; Data.Changes[copy] = action; }, sbyte.MinValue, sbyte.MaxValue);
                AddNumber2("Bewege y:   ", action.MoveX, 80, a => { action.MoveY = (sbyte)a; Data.Changes[copy] = action; }, sbyte.MinValue, sbyte.MaxValue);
                
                AddCheckBox2("Bewege Hex: ", 100, action.Flags.HasFlag(HexActionFlags.MoveHex), a => { if (a) action.Flags |= HexActionFlags.MoveHex; else action.Flags &= ~HexActionFlags.MoveHex; Data.Changes[copy] = action; });
                AddCheckBox2("Bewege Spieler: ", 124, action.Flags.HasFlag(HexActionFlags.MovePlayer), a => { if (a) action.Flags |= HexActionFlags.MovePlayer; else action.Flags &= ~HexActionFlags.MovePlayer; Data.Changes[copy] = action; });

                string[] conditions =
                {
                    "Zug",
                    "Spieler betreten",
                    "Spieler verlassen",
                    "Nächste Flagge",
                    "Nächste nicht Flagge",
                    "Nächste ID",
                    "Nächste nicht ID",
                    "Spieler betritt ID",
                    "Spieler verlässt ID",
                };

                TextBlock conditionText = new TextBlock(conditions[((int)action.Condition).Clamp(0, conditions.Length-1)], 2, 0, 180);
                conditionText.Color = Color.White;
                frame.AddChild(conditionText);

                AddNumber2("Bedingung: ", (byte)action.Condition, 160, a => { action.Condition = (HexCondition)a; Data.Changes[copy] = action; conditionText.Text = conditions[a.Clamp(0, conditions.Length-1)]; });

                ui.Add(frame);

                count++;
            }
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
            //GridRenderer.RenderHex(new HexPoint(0, 0), new List<HexData> { Data }, layout, Color.White, frameCount);

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

                add.OnLeftClick += (sender) =>
                {
                    if (add.ui.Input.KeyDown(Key.LControl) ||
                        add.ui.Input.KeyDown(Key.RControl))
                        Value += 10;
                    else if (add.ui.Input.KeyDown(Key.LShift) ||
                        add.ui.Input.KeyDown(Key.RShift))
                        Value += 5;
                    else
                        Value += 1;

                    Value = Value.Clamp(MinValue, MaxValue);

                    text.Text = Value.ToString();
                    ValueChanged?.Invoke(Value);
                };
                sub.OnLeftClick += (sender) =>
                {
                    if (add.ui.Input.KeyDown(Key.LControl) ||
                        add.ui.Input.KeyDown(Key.RControl))
                        Value -= 10;
                    else if (add.ui.Input.KeyDown(Key.LShift) ||
                        add.ui.Input.KeyDown(Key.RShift))
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

            public void AddTo(UIElement element)
            {
                element.AddChild(text);
                element.AddChild(add);
                element.AddChild(sub);
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
