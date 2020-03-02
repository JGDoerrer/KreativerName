using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KreativerName.Rendering;
using OpenTK;

namespace KreativerName.UI
{
    public static class Notification
    {
        static List<Note> notifications = new List<Note>();
        const int maxAnimation = 120;

        public static void Update()
        {
            for (int i = notifications.Count - 1; i >= 0; i--)
            {
                notifications[i] = notifications[i].Update();

                if (notifications[i].AnimationDone)
                    notifications.RemoveAt(i);
            }
        }

        public static void Render(Vector2 windowSize)
        {
            float y = notifications.Sum(x => x.Y);

            foreach (Note note in notifications.Reverse<Note>())
            {
                note.Render(windowSize, y);
                y += note.Height;
            }
        }

        public static void Show(string text, float textSize = 2)
        {
            notifications.Add(new Note()
            {
                Text = text,
                TextSize = textSize,
                AnimationIn = maxAnimation,
            });
        }

        private static float QuadraticInOut(float t)
           => t * t / (2 * t * t - 2 * t + 1);

        struct Note
        {
            public string Text;
            public float TextSize;
            public int AnimationIn;
            public int AnimationStay;
            public int AnimationOut;

            Model textModel;
            Model frameModel;

            public float Width => TextRenderer.GetWidth(Text, TextSize) + 16;
            public float Height => TextRenderer.GetHeight(Text, TextSize) + 16;
            public float Y => Height * -QuadraticInOut((float)(AnimationIn + 1) / maxAnimation);

            public bool AnimationDone => AnimationIn < 0 && AnimationOut < 0 && AnimationStay < 0;

            public void BuildMesh()
            {
                const int a = 8;
                const float scale = 2;

                MeshBuilder builder = new MeshBuilder();

                Texture2D tex = Textures.Get("TextBox");

                builder.AddRectangle(new RectangleF(0, 0, a, a), 
                                     new RectangleF(0, 0, a / (float)tex.Width, a / (float)tex.Height));

                frameModel = new Model(builder.Mesh, tex, Shaders.Get("Basic"));
            }

            public Note Update()
            {
                if (AnimationIn >= 0)
                    AnimationIn--;
                if (AnimationOut >= 0)
                    AnimationOut--;
                if (AnimationStay >= 0)
                    AnimationStay--;

                if (AnimationIn == 0)
                    AnimationStay = 120 + Text.Length * 5;
                if (AnimationStay == 0)
                    AnimationOut = maxAnimation;

                return this;
            }

            public void Render(Vector2 windowSize, float y)
            {
                const int a = 8;
                const float scale = 2;

                float animOut = QuadraticInOut((float)AnimationOut / maxAnimation);

                if (AnimationIn > 0 || AnimationStay > 0)
                    animOut = 1;

                float w = Width;
                float h = Height;
                float x = windowSize.X - w * animOut;

                Color color = Color.White;

                float[] xs = { x, x + a * scale, x + w - a * scale };
                float[] ys = { y, y + a * scale, y + h - a * scale };

                for (int i = 0; i <= 2; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        Vector2 scl = new Vector2(i == 1 ? w / (a * scale) - 2 : 1,
                                                  j == 1 ? h / (a * scale) - 2 : 1) * scale;

                        //TextureRenderer.Draw(tex, new Vector2(xs[i], ys[j]), scl, color, new RectangleF(a * i, a * j, a, a));
                    }
                }

                TextRenderer.RenderString(Text, new Vector2(x + 8, y + 8), Color.Black, TextSize);
            }
        }
    }
}
