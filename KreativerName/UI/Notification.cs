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

            foreach (var note in notifications.Reverse<Note>())
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

            public float Width => TextRenderer.GetWidth(Text, TextSize) + 16;
            public float Height => TextRenderer.GetHeight(Text, TextSize) + 16;
            public float Y => Height * -QuadraticInOut((float)(AnimationIn + 1) / maxAnimation);

            public bool AnimationDone => AnimationIn < 0 && AnimationOut < 0 && AnimationStay < 0;

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
                Texture2D tex = Textures.Get("TextBox");

                // corner top left
                TextureRenderer.Draw(tex, new Vector2(x                , y), Vector2.One * scale, color, new RectangleF(0, 0, a, a));
                // corner top right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y), Vector2.One * scale, color, new RectangleF(a * 2, 0, a, a));
                // corner bottom left
                TextureRenderer.Draw(tex, new Vector2(x                , y + h - a * scale), Vector2.One * scale, color, new RectangleF(0, a * 2, a, a));
                // corner bottom right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + h - a * scale), Vector2.One * scale, color, new RectangleF(a * 2, a * 2, a, a));
                // left
                TextureRenderer.Draw(tex, new Vector2(x                , y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(0, a, a, a));
                // top
                TextureRenderer.Draw(tex, new Vector2(x + a * scale    , y), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(a, 0, a, a));
                // right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(a * 2, a, a, a));
                // bottom
                TextureRenderer.Draw(tex, new Vector2(x + a * scale    , y + h - a * scale), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(a, a * 2, a, a));
                // center
                TextureRenderer.Draw(tex, new Vector2(x + a * scale    , y + a * scale), new Vector2(w / (a * scale) - 2, h / (a * scale) - 2) * scale, color, new RectangleF(a, a, a, a));

                TextRenderer.RenderString(Text, new Vector2(x + 8, y + 8), Color.Black, TextSize);
            }
        }
    }
}
